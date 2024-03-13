using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Exceptions;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.DTObjects.Versions;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Entities.Exceptions;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API;
using IARA.Mobile.Insp.Application.Helpers;
using IARA.Mobile.Insp.Application.Interfaces.Database;
using IARA.Mobile.Insp.Application.Interfaces.Dtos;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Application.Transactions.Base;
using IARA.Mobile.Insp.Domain.Entities.Inspections;
using IARA.Mobile.Insp.Domain.Entities.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Domain.Interfaces;
using IARA.Mobile.Pub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IARA.Mobile.Insp.Application.Transactions
{
    public class StartupTransaction : BaseTransaction, IStartupTransaction
    {
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

        public async Task<bool?> IsDeviceAllowed(string imei, IAuthTokenProvider tokenProvider)
        {
            HttpResult<bool> result = await RestClient.GetAsync<bool>("InspectionData/IsDeviceAllowed", new { imei });
            if (result.IsSuccessful)
            {
                return result.Content;
            }
            else if (result.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(tokenProvider.RefreshToken))
            {
                JwtToken oldToken = new JwtToken
                {
                    Token = tokenProvider.RefreshToken
                };
                HttpResult<JwtToken> newToken = await RestClient.PostAsync<JwtToken>("Security/RefreshToken", "Common", false, oldToken);

                if (newToken.IsSuccessful)
                {
                    tokenProvider.Token = newToken.Content.Token;
                    tokenProvider.RefreshToken = newToken.Content.RefreshToken;
                    tokenProvider.AccessTokenExpiration = newToken.Content.ValidTo;
                    return await RestClient.GetAsync<bool>("InspectionData/IsDeviceAllowed", new { imei }).GetHttpContent();
                }
            }
            return null;
        }

        public async Task<UserAuthDto> GetUserAuthInfo()
        {
            HttpResult<UserAuthDto> result = await RestClient.GetAsync<UserAuthDto>("Security/GetUser", urlExtension: "Common");

            return result.Content;
        }

        public Task SendUserDeviceInfo(PublicMobileDeviceDto info)
        {
            return RestClient.PostAsync("UserManagement/DeviceInfo", info, alertOnException: false).IsSuccessfulResult();
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

            Task<Nom>[] nomTasks = await StartupNomenclatureHelper.PullNomenclatureTables(RestClient, nomenclatureDates);

            NomenclatureEnum[] inspNoms = new[]
            {
                NomenclatureEnum.Inspector,
                NomenclatureEnum.PatrolVehicle,
                NomenclatureEnum.Ship,
                NomenclatureEnum.ShipOwner,
                NomenclatureEnum.ShipFishingGear,
                NomenclatureEnum.FishingGearMark,
                NomenclatureEnum.FishingGearPinger,
                NomenclatureEnum.PoundNet,
                NomenclatureEnum.PoundNetPermitLicense,
                NomenclatureEnum.PoundNetFishingGear,
                NomenclatureEnum.PoundNetFishingGearMark,
                NomenclatureEnum.PoundNetFishingGearPinger,
                NomenclatureEnum.PermitLicense,
                NomenclatureEnum.LogBook,
                NomenclatureEnum.Buyer,
                NomenclatureEnum.Aquaculture,
                NomenclatureEnum.Permit,
            };

            if (nomTasks?.Length > 0)
            {
                countCallback?.Invoke(nomTasks.Length + inspNoms.Length + 1);

                List<Nom> noms = (await Task.WhenAll(nomTasks))
                    .OrderBy(f => f?.EntitiesCount ?? 0)
                    .ToList();

                finishCallback?.Invoke();

                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    foreach (Nom nom in noms)
                    {
                        if (nom == null)
                        {
                            return false;
                        }
                        else if (nom.UpdateDatabase == null)
                        {
                            finishCallback?.Invoke();
                            continue;
                        }

                        nom.UpdateDatabase(context);

                        if (finishCallback != null)
                        {
                            finishCallback();
                            await Task.Yield();
                        }
                    }
                }
            }
            else
            {
                //return false;
            }

            HashSet<int> personsToPull = new HashSet<int>();
            HashSet<int> legalsToPull = new HashSet<int>();
            HashSet<int> personsToDelete = new HashSet<int>();
            HashSet<int> legalsToDelete = new HashSet<int>();

            for (int i = 0; i < inspNoms.Length; i++)
            {
                bool pullResult = await StartupInspectionHelper.MapInspectionNomenclature(
                    RestClient,
                    nomenclatureDates,
                    DateTimeProvider,
                    ContextBuilder,
                    inspNoms[i],
                    personsToPull,
                    legalsToPull,
                    personsToDelete,
                    legalsToDelete
                );

                if (!pullResult)
                {
                    return false;
                }

                finishCallback?.Invoke();
            }

            bool deletePersons = personsToDelete.Count > 0,
                deleteLegals = legalsToDelete.Count > 0,
                pullPersons = personsToPull.Count > 0,
                pullLegals = legalsToPull.Count > 0;

            if (deletePersons || deleteLegals || pullPersons || pullLegals)
            {
                if (deletePersons && pullPersons)
                {
                    personsToDelete.ExceptWith(personsToPull);
                }

                if (deleteLegals && pullLegals)
                {
                    legalsToDelete.ExceptWith(legalsToPull);
                }

                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    if (deletePersons)
                    {
                        RemovePersons(context, personsToDelete);
                    }
                    if (deleteLegals)
                    {
                        RemoveLegals(context, legalsToDelete);
                    }

                    if (pullPersons)
                    {
                        bool pulled = await PullPersonnel<Person, PersonApiDto>(
                            context.Persons,
                            "InspectionData/GetPersons",
                            personsToPull,
                            f => new Person
                            {
                                Id = f.Id,
                                EgnLnc = f.EgnLnc.EgnLnc,
                                FirstName = f.FirstName,
                                MiddleName = f.MiddleName,
                                LastName = f.LastName,
                                IdentifierType = f.EgnLnc.IdentifierType,
                            }
                        );

                        if (!pulled)
                        {
                            return false;
                        }
                    }
                    if (pullLegals)
                    {
                        bool pulled = await PullPersonnel<Legal, LegalApiDto>(
                            context.Legals,
                            "InspectionData/GetLegals",
                            legalsToPull,
                            f => new Legal
                            {
                                Id = f.Id,
                                Eik = f.Eik,
                                Name = f.Name,
                            }
                        );

                        if (!pulled)
                        {
                            return false;
                        }
                    }
                }
            }

            finishCallback?.Invoke();

            return true;
        }

        private async Task<bool> PullUserAuthInfo()
        {
            HttpResult<UserAuthDto> result = await RestClient.GetAsync<UserAuthDto>("Security/GetUser", urlExtension: "Common", alertOnException: false);

            if (result.IsSuccessful && result.Content != null)
            {
                CurrentUser.EgnLnch = result.Content.EgnLnch;
                CurrentUser.FirstName = result.Content.FirstName;
                CurrentUser.MiddleName = result.Content.MiddleName;
                CurrentUser.LastName = result.Content.LastName;
                CurrentUser.Id = result.Content.UserId;
                CurrentUser.MustChangePassword = result.Content.UserMustChangePassword;

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

        private void RemovePersons(IAppDbContext context, HashSet<int> personsToDelete)
        {
            List<int> personIds = (
                from shipOwner in context.ShipOwners
                where shipOwner.PersonId.HasValue
                    && personsToDelete.Contains(shipOwner.PersonId.Value)
                select shipOwner.PersonId.Value
            ).ToList();

            personIds.AddRange((
                from permit in context.PermitLicenses
                where permit.PersonId.HasValue
                    && personsToDelete.Contains(permit.PersonId.Value)
                select permit.PersonId.Value
            ).ToList());

            personIds.AddRange((
                from permit in context.PermitLicenses
                where personsToDelete.Contains(permit.PersonCaptainId)
                select permit.PersonCaptainId
            ).ToList());

            personIds.AddRange((
                from buyer in context.Buyers
                where buyer.PersonId.HasValue
                    && personsToDelete.Contains(buyer.PersonId.Value)
                select buyer.PersonId.Value
            ).ToList());

            personIds.AddRange((
                from permit in context.PoundNetPermitLicenses
                where permit.PersonId.HasValue
                    && personsToDelete.Contains(permit.PersonId.Value)
                select permit.PersonId.Value
            ).ToList());

            if (personIds.Count > 0)
            {
                personsToDelete.RemoveWhere(personIds.Contains);
            }

            if (personsToDelete.Count > 0)
            {
                context.Persons.Delete(f => personsToDelete.Contains(f.Id));
            }
        }

        private void RemoveLegals(IAppDbContext context, HashSet<int> legalsToDelete)
        {
            List<int> legalIds = (
                from shipOwner in context.ShipOwners
                where shipOwner.LegalId.HasValue
                    && legalsToDelete.Contains(shipOwner.LegalId.Value)
                select shipOwner.LegalId.Value
            ).ToList();

            legalIds.AddRange((
                from permit in context.PermitLicenses
                where permit.LegalId.HasValue
                    && legalsToDelete.Contains(permit.LegalId.Value)
                select permit.LegalId.Value
            ).ToList());

            legalIds.AddRange((
                from buyer in context.Buyers
                where buyer.LegalId.HasValue
                    && legalsToDelete.Contains(buyer.LegalId.Value)
                select buyer.LegalId.Value
            ).ToList());

            legalIds.AddRange((
                from permit in context.PoundNetPermitLicenses
                where permit.LegalId.HasValue
                    && legalsToDelete.Contains(permit.LegalId.Value)
                select permit.LegalId.Value
            ).ToList());

            legalIds.AddRange((
                from aquaculture in context.Aquacultures
                select aquaculture.LegalId
            ).ToList());

            if (legalIds.Count > 0)
            {
                legalsToDelete.RemoveWhere(legalIds.Contains);
            }

            if (legalsToDelete.Count > 0)
            {
                context.Legals.Delete(f => legalsToDelete.Contains(f.Id));
            }
        }

        private async Task<bool> PullPersonnel<TEntity, TDto>(TLTableQuery<TEntity> table, string url, HashSet<int> toPull, Func<TDto, TEntity> createEntity)
            where TEntity : IAddressEntity
            where TDto : IAddressDto
        {
            List<int> ids = (
                from entity in table
                where toPull.Contains(entity.Id)
                select entity.Id
            ).ToList();

            if (ids.Count > 0)
            {
                toPull.RemoveWhere(ids.Contains);
            }

            if (toPull.Count > 0)
            {
                HttpResult<List<TDto>> result = await RestClient.PostAsync<List<TDto>>(url, toPull, alertOnException: false);

                if (result.IsSuccessful && result.Content?.Count > 0)
                {
                    table.AddRange(result.Content.ConvertAll(f =>
                    {
                        TEntity entity = createEntity(f);

                        if (f.Address != null)
                        {
                            entity.HasAddress = true;
                            entity.CountryId = f.Address.CountryId;
                            entity.DistrictId = f.Address.DistrictId;
                            entity.MunicipalityId = f.Address.MunicipalityId;
                            entity.PopulatedAreaId = f.Address.PopulatedAreaId;
                            entity.Region = f.Address.Region;
                            entity.PostalCode = f.Address.PostalCode;
                            entity.Street = f.Address.Street;
                            entity.StreetNum = f.Address.StreetNum;
                            entity.BlockNum = f.Address.BlockNum;
                            entity.EntranceNum = f.Address.EntranceNum;
                            entity.FloorNum = f.Address.FloorNum;
                            entity.ApartmentNum = f.Address.ApartmentNum;
                        }
                        else
                        {
                            entity.HasAddress = false;
                        }

                        return entity;
                    }));
                }

                return result.IsSuccessful;
            }

            return true;
        }
    }
}
