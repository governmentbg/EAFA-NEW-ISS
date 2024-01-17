using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Exceptions;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.DTObjects.Translations;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.DTObjects.Versions;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Entities.Exceptions;
using IARA.Mobile.Domain.Entities.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.AddressNomenclatures.API;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.Interfaces.Database;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Application.Transactions.Base;
using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;
using IARA.Mobile.Pub.Domain.Enums;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class StartupTransaction : BaseTransaction, IStartupTransaction
    {
        private class Nom
        {
            public Action<IAppDbContext> UpdateDatabase { get; set; }
            public int EntitiesCount { get; set; }
        }

        private readonly INomenclatureDates nomenclatureDates;
        private readonly IMobileInfo mobileInfo;

        public StartupTransaction(BaseTransactionProvider provider, INomenclatureDates nomenclatureDates, IMobileInfo mobileInfo)
            : base(provider)
        {
            this.nomenclatureDates = nomenclatureDates ?? throw new ArgumentNullException(nameof(nomenclatureDates));
            this.mobileInfo = mobileInfo ?? throw new ArgumentNullException(nameof(mobileInfo));
        }

        public Task<bool> HealthCheck()
        {
            return RestClient.HealthCheckAsync().IsSuccessfulResult();
        }

        public async Task<bool> IsAppOutdated(int version, string platform)
        {
            HttpResult<IsAppOutdatedDto> result = await RestClient.GetAsync<IsAppOutdatedDto>("Version/IsAppOutdated", new { version, platform }, alertOnException: false);

            if (result.IsSuccessful && result.Content != null)
            {
                return result.Content.IsAppOutdated;
            }

            return false;
        }

        public async Task PostOfflineData()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                List<ErrorLog> errors = context.ErrorLogs.ToList();

                if (errors.Count > 0)
                {
                    HttpResult result = await RestClient.PostAsync("Logger/LogErrors",
                        errors.ConvertAll(error => new ExceptionApiDto
                        {
                            ExceptionSource = error.ExceptionSource,
                            Level = error.Level,
                            LogDate = error.LogDate,
                            Message = error.Message,
                            StackTrace = error.StackTrace,
                            Client = mobileInfo.Info
                        }));

                    if (result.IsSuccessful)
                    {
                        context.ErrorLogs.Clear();
                    }
                }
            }
        }

        public async Task<bool> GetInitialData(bool isLoginRequest, Action<int> countCallback, Action finishCallback)
        {
            if (CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected)
            {
                return !isLoginRequest;
            }

            if (isLoginRequest)
            {
                await PullUserAuthInfo();
            }

            List<Task<Nom>> nomTasks = await PullNomenclatureTables();

            if (nomTasks?.Count > 0)
            {
                countCallback?.Invoke(nomTasks.Count);

                List<Nom> results = (await Task.WhenAll(nomTasks))
                    .OrderBy(f => f.EntitiesCount)
                    .ToList();

                finishCallback?.Invoke();

                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    foreach (Nom result in results)
                    {
                        if (result == null)
                        {
                            return false;
                        }
                        else if (result.UpdateDatabase == null)
                        {
                            finishCallback?.Invoke();
                            continue;
                        }

                        result.UpdateDatabase(context);

                        if (finishCallback != null)
                        {
                            finishCallback();
                            await Task.Yield();
                        }
                    }
                }
            }

            return true;
        }

        public async Task<UserAuthDto> GetUserAuthInfo()
        {
            HttpResult<UserAuthDto> result = await RestClient.GetAsync<UserAuthDto>("User/GetUserAuthInfo", urlExtension: "Common");

            return result.Content;
        }

        private async Task<bool> PullUserAuthInfo()
        {
            HttpResult<UserAuthDto> result = await RestClient.GetAsync<UserAuthDto>("User/GetUserAuthInfo", urlExtension: "Common", alertOnException: false);

            if (result.IsSuccessful && result.Content != null)
            {
                CurrentUser.EgnLnch = result.Content.EgnLnch;
                CurrentUser.FirstName = result.Content.FirstName;
                CurrentUser.MiddleName = result.Content.MiddleName;
                CurrentUser.LastName = result.Content.LastName;
                CurrentUser.Id = result.Content.Id;

                if (result.Content.Permissions?.Count > 0)
                {
                    using (IAppDbContext context = ContextBuilder.CreateContext())
                    {
                        context.NPermissions.AddRange(result.Content.Permissions.ConvertAll(f => new NPermission
                        {
                            Permission = f
                        }));
                    }
                }
            }

            return result.IsSuccessful;
        }

        private Task<List<Task<Nom>>> PullNomenclatureTables()
        {
            return RestClient.GetAsync<List<MobileNomenclatureDto>>("Nomenclatures/GetNomenclatureTables", alertOnException: false)
                .ContinueWith(task =>
                {
                    HttpResult<List<MobileNomenclatureDto>> result = task.Result;

                    List<(NomenclatureEnum? NomEnum, DateTime LastUpdated)> deserialisedData = result.Content
                        .Select(f => (NomEnum: MapToNomenclatureEnum(f.NomenclatureName), f.LastUpdated))
                        .ToList();

                    return result.IsSuccessful && result.Content?.Count > 0
                        ? (
                            from mobileNom in deserialisedData
                            where mobileNom.NomEnum.HasValue && (
                                    !(nomenclatureDates[mobileNom.NomEnum.Value] is DateTime date)
                                    || date < mobileNom.LastUpdated
                                )
                            group mobileNom.LastUpdated by mobileNom.NomEnum.Value into groupedNom
                            select MapMobileNomenclatureToInvokePullNomenclatures(groupedNom.Key, groupedNom.Max(f => f))
                        ).ToList() : null;
                });
        }

        private Task<Nom> PullNomenclatures<TNomenclatureDto, TNomenclatureEntity>(NomenclatureEnum nomenclatureEnum, string url, DateTime lastUpdated, Converter<TNomenclatureDto, TNomenclatureEntity> selector = null)
            where TNomenclatureDto : NomenclatureDto
            where TNomenclatureEntity : INomenclature, new()
        {
            return selector == null
                ? PullCustomNomenclatures(nomenclatureEnum, url, lastUpdated,
                    (List<TNomenclatureDto> dtos) =>
                        from dto in dtos
                        select new TNomenclatureEntity
                        {
                            Id = dto.Value,
                            IsActive = dto.IsActive,
                            Name = dto.DisplayName
                        })
                : PullCustomNomenclatures(nomenclatureEnum, url, lastUpdated,
                    (List<TNomenclatureDto> dtos) => dtos.ConvertAll(selector));
        }

        private Task<Nom> PullCustomNomenclatures<TNomenclatureDto, TNomenclatureEntity>(NomenclatureEnum nomenclatureEnum, string url, DateTime lastUpdated, Func<List<TNomenclatureDto>, IEnumerable<TNomenclatureEntity>> selector)
            where TNomenclatureEntity : new()
        {
            return RestClient.GetAsync<List<TNomenclatureDto>>(url, alertOnException: false)
                .ContinueWith(task =>
                {
                    HttpResult<List<TNomenclatureDto>> result = task.Result;

                    return result.IsSuccessful && result.Content?.Count > 0
                        ? new Nom
                        {
                            EntitiesCount = result.Content.Count,
                            UpdateDatabase = (IAppDbContext context) =>
                            {
                                TLTableQuery<TNomenclatureEntity> table = context.TLTable<TNomenclatureEntity>();

                                table.Clear();
                                table.AddRange(selector(result.Content));
                                nomenclatureDates[nomenclatureEnum] = lastUpdated;
                            }
                        }
                        : result.IsSuccessful ? new Nom() : null;
                });
        }

        private Task<Nom> PullTranslations(DateTime lastUpdated)
        {
            return RestClient.GetAsync<List<TranslationLanguageApiDto>>("Translations/GetAll", alertOnException: false)
                .ContinueWith(task =>
                {
                    HttpResult<List<TranslationLanguageApiDto>> result = task.Result;

                    return result.IsSuccessful && result.Content?.Count > 0
                        ? new Nom
                        {
                            EntitiesCount = result.Content.Count,
                            UpdateDatabase = (IAppDbContext context) =>
                            {
                                context.NTranslationGroups.Clear();
                                context.NTranslationResources.Clear();
                                context.NTranslationGroups.AddRange(
                                    from language in result.Content
                                    from page in language.Groups
                                    select new NTranslationGroup
                                    {
                                        Language = language.Language,
                                        Page = page.Code
                                    }
                                );

                                List<NTranslationGroup> groups = context.NTranslationGroups.ToList();

                                context.NTranslationResources.AddRange(
                                    from language in result.Content
                                    from page in language.Groups
                                    from translation in page.Translations
                                    select new NTranslationResource
                                    {
                                        Code = translation.Code,
                                        Value = translation.Translation,
                                        GroupId = groups.Find(f => f.Page == page.Code && f.Language == language.Language).Id
                                    }
                                );
                                nomenclatureDates[NomenclatureEnum.Translation] = lastUpdated;
                            }
                        }
                        : result.IsSuccessful ? new Nom() : null;
                });
        }

        private Task<Nom> MapMobileNomenclatureToInvokePullNomenclatures(NomenclatureEnum nomenclatureEnum, DateTime lastUpdated)
        {
            const string prefix = "Nomenclatures/Get";

            switch (nomenclatureEnum)
            {
                case NomenclatureEnum.Translation:
                    return PullTranslations(lastUpdated);
                case NomenclatureEnum.Country:
                    return PullNomenclatures(nomenclatureEnum, prefix + "Countries", lastUpdated, (NomenclatureDto dto) => new NCountry
                    {
                        Id = dto.Value,
                        Code = dto.Code,
                        Name = dto.DisplayName,
                        IsActive = dto.IsActive
                    });
                case NomenclatureEnum.District:
                    return PullNomenclatures<NomenclatureDto, NDistrict>(nomenclatureEnum, prefix + "Districts", lastUpdated);
                case NomenclatureEnum.Municipality:
                    return PullNomenclatures(nomenclatureEnum, prefix + "Municipalities", lastUpdated,
                        (MunicipalityApiDto dto) => new NMunicipality
                        {
                            Id = dto.Value,
                            Name = dto.DisplayName,
                            IsActive = dto.IsActive,
                            DistrictId = dto.DistrictId
                        });
                case NomenclatureEnum.PopulatedArea:
                    return PullNomenclatures(nomenclatureEnum, prefix + "PopulatedAreas", lastUpdated,
                       (PopulatedAreaApiDto dto) => new NPopulatedArea
                       {
                           Id = dto.Value,
                           Name = dto.DisplayName,
                           IsActive = dto.IsActive,
                           MunicipalityId = dto.MunicipalityId
                       });
                case NomenclatureEnum.DocumentType:
                    return PullNomenclatures(nomenclatureEnum, prefix + "DocumentTypes", lastUpdated, (NomenclatureDto dto) => new NDocumentType
                    {
                        Id = dto.Value,
                        Code = dto.Code,
                        Name = dto.DisplayName,
                        IsActive = dto.IsActive
                    });
                case NomenclatureEnum.Fish:
                    return PullNomenclatures<NomenclatureDto, NFish>(nomenclatureEnum, prefix + "Fishes", lastUpdated);
                case NomenclatureEnum.TicketType:
                    return PullNomenclatures(nomenclatureEnum, prefix + "TicketTypes", lastUpdated,
                       (TicketTypeApiDto dto) => new NTicketType
                       {
                           Id = dto.Value,
                           Code = dto.Code,
                           Name = dto.DisplayName,
                           IsActive = dto.IsActive,
                           OrderNo = dto.OrderNo
                       });
                case NomenclatureEnum.TicketPeriod:
                    return PullNomenclatures(nomenclatureEnum, prefix + "TicketPeriods", lastUpdated,
                       (NomenclatureDto dto) => new NTicketPeriod
                       {
                           Id = dto.Value,
                           Code = dto.Code,
                           Name = dto.DisplayName,
                           IsActive = dto.IsActive
                       });
                case NomenclatureEnum.TicketTariff:
                    return PullNomenclatures(nomenclatureEnum, prefix + "TicketTariffs", lastUpdated,
                       (TicketTariffApiDto dto) => new NTicketTariff
                       {
                           Id = dto.Value,
                           Code = dto.Code,
                           Name = dto.DisplayName,
                           Price = dto.Price,
                           IsActive = dto.IsActive
                       });
                case NomenclatureEnum.PermitReason:
                    return PullNomenclatures<NomenclatureDto, NPermitReason>(nomenclatureEnum, prefix + "PermitReasons", lastUpdated);
                case NomenclatureEnum.FileType:
                    return PullNomenclatures(nomenclatureEnum, prefix + "FileTypes", lastUpdated, (NomenclatureDto dto) => new NFileType
                    {
                        Id = dto.Value,
                        Code = dto.Code,
                        Name = dto.DisplayName,
                        IsActive = dto.IsActive
                    });
                case NomenclatureEnum.ViolationSignalType:
                    return PullNomenclatures<NomenclatureDto, NViolationSignalType>(nomenclatureEnum, prefix + "ViolationSignalTypes", lastUpdated);
                case NomenclatureEnum.Version:
                    return PullNomenclatures(nomenclatureEnum, prefix + "MobileVersions", lastUpdated,
                        (VersionNomenclatureDto dto) => new NVersion
                        {
                            Id = dto.Value,
                            Name = dto.Code,
                            Version = dto.Version,
                            OSType = dto.OSType,
                            IsActive = dto.IsActive
                        });
                case NomenclatureEnum.Genders:
                    return PullNomenclatures(nomenclatureEnum, prefix + "Genders", lastUpdated, (NomenclatureDto dto) => new NGender
                    {
                        Id = dto.Value,
                        Code = dto.Code,
                        Name = dto.DisplayName,
                        IsActive = dto.IsActive
                    });
                case NomenclatureEnum.SystemParameters:
                    return PullNomenclatures(nomenclatureEnum, prefix + "SystemParameters", lastUpdated,
                       (SystemParameterDto dto) => new NSystemParameter
                       {
                           Id = dto.Value,
                           Code = dto.Code,
                           Name = dto.DisplayName,
                           IsActive = dto.IsActive,
                           ParamValue = dto.ParamValue
                       });
                case NomenclatureEnum.PaymentTypes:
                    return PullNomenclatures(nomenclatureEnum, prefix + "PaymentTypes", lastUpdated, (NomenclatureDto dto) => new NPaymentType
                    {
                        Id = dto.Value,
                        Code = dto.Code,
                        Name = dto.DisplayName,
                        IsActive = dto.IsActive
                    });
                case NomenclatureEnum.TerritorialUnit:
                    return PullNomenclatures(nomenclatureEnum, prefix + "TerritoryUnits", lastUpdated, (NomenclatureDto dto) => new NTerritorialUnit
                    {
                        Id = dto.Value,
                        Code = dto.Code,
                        Name = dto.DisplayName,
                        IsActive = dto.IsActive
                    });
                default:
                    throw new NotImplementedException($"{nameof(nomenclatureEnum)} doesn't have specified {nameof(NomenclatureEnum)} implemented");
            }
        }

        private NomenclatureEnum? MapToNomenclatureEnum(string nomenclatureName)
        {
            switch (nomenclatureName)
            {
                case "Countries":
                    return NomenclatureEnum.Country;
                case "Districts":
                    return NomenclatureEnum.District;
                case "Municipalities":
                    return NomenclatureEnum.Municipality;
                case "PopulatedAreas":
                    return NomenclatureEnum.PopulatedArea;
                case "DocumentTypes":
                    return NomenclatureEnum.DocumentType;
                case "Fishes":
                    return NomenclatureEnum.Fish;
                case "TicketTypes":
                    return NomenclatureEnum.TicketType;
                case "TicketPeriods":
                    return NomenclatureEnum.TicketPeriod;
                case "Tariff":
                    return NomenclatureEnum.TicketTariff;
                case "TranslationGroups":
                case "TranslationResources":
                    return NomenclatureEnum.Translation;
                case "PermitReasons":
                    return NomenclatureEnum.PermitReason;
                case "FileTypes":
                    return NomenclatureEnum.FileType;
                case "ViolationSignalTypes":
                    return NomenclatureEnum.ViolationSignalType;
                case "MobileVersions":
                    return NomenclatureEnum.Version;
                case "Genders":
                    return NomenclatureEnum.Genders;
                case "SystemParameters":
                    return NomenclatureEnum.SystemParameters;
                case "PaymentTypes":
                    return NomenclatureEnum.PaymentTypes;
                case "TerritoryUnits":
                    return NomenclatureEnum.TerritorialUnit;
                default:
                    return null;
            }
        }
    }
}
