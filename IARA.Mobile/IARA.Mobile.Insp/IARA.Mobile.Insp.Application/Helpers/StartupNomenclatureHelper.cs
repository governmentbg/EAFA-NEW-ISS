using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.DTObjects.Translations;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API;
using IARA.Mobile.Insp.Application.Interfaces.Database;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Domain.Entities.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IARA.Mobile.Insp.Application.Helpers
{
    [DebuggerDisplay("{NomEnum}")]
    internal class Nom
    {
        public NomenclatureEnum NomEnum { get; set; }
        public Action<IAppDbContext> UpdateDatabase { get; set; }
        public int EntitiesCount { get; set; }
    }

    internal static class StartupNomenclatureHelper
    {
        public static Task<Task<Nom>[]> PullNomenclatureTables(IRestClient restClient, INomenclatureDates nomenclatureDates)
        {
            return restClient.GetAsync<List<MobileNomenclatureDto>>("Nomenclatures/GetNomenclatureTables", alertOnException: false)
                .ContinueWith(task =>
                {
                    HttpResult<List<MobileNomenclatureDto>> result = task.Result;

                    return result.IsSuccessful && result.Content?.Count > 0
                        ? (
                            from mobileNom in result.Content.Select(f =>
                                (NomEnum: MapToNomenclatureEnum(f.NomenclatureName), f.LastUpdated)
                            )
                            where mobileNom.NomEnum.HasValue
                                && (
                                    !(nomenclatureDates[mobileNom.NomEnum.Value] is DateTime date)
                                    || date < mobileNom.LastUpdated
                                )
                            group mobileNom.LastUpdated by mobileNom.NomEnum.Value into groupedNom
                            select MapMobileNomenclatureToInvokePullNomenclatures(restClient, nomenclatureDates, groupedNom.Key, groupedNom.Max(f => f))
                        ).ToArray() : null;
                });
        }

        private static Task<Nom> PullNomenclatures<TNomenclatureDto, TNomenclatureEntity>(IRestClient restClient, INomenclatureDates nomenclatureDates, NomenclatureEnum nomenclatureEnum, string url, DateTime lastUpdated, Converter<TNomenclatureDto, TNomenclatureEntity> selector)
            where TNomenclatureDto : NomenclatureDto
            where TNomenclatureEntity : INomenclature, new()
        {
            return PullCustomNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, url, lastUpdated,
                (List<TNomenclatureDto> dtos) => dtos.ConvertAll(selector));
        }

        private static Task<Nom> PullNomenclatures<TNomenclatureEntity>(IRestClient restClient, INomenclatureDates nomenclatureDates, NomenclatureEnum nomenclatureEnum, string url, DateTime lastUpdated)
            where TNomenclatureEntity : ICodeNomenclature, new()
        {
            return PullCustomNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, url, lastUpdated,
                (List<NomenclatureDto> dtos) =>
                    from dto in dtos
                    select new TNomenclatureEntity
                    {
                        Id = dto.Value,
                        IsActive = dto.IsActive,
                        Name = dto.DisplayName,
                        Code = dto.Code,
                    });
        }

        private static Task<Nom> PullCustomNomenclatures<TNomenclatureDto, TNomenclatureEntity>(IRestClient restClient, INomenclatureDates nomenclatureDates, NomenclatureEnum nomenclatureEnum, string url, DateTime lastUpdated, Func<List<TNomenclatureDto>, IEnumerable<TNomenclatureEntity>> selector)
            where TNomenclatureEntity : new()
        {
            return restClient.GetAsync<List<TNomenclatureDto>>(url, alertOnException: false)
                .ContinueWith(task =>
                {
                    HttpResult<List<TNomenclatureDto>> result = task.Result;

                    if (result.IsSuccessful && result.Content?.Count > 0)
                    {
                        return new Nom
                        {
                            NomEnum = nomenclatureEnum,
                            EntitiesCount = result.Content.Count,
                            UpdateDatabase = (IAppDbContext context) =>
                            {
                                TLTableQuery<TNomenclatureEntity> table = context.TLTable<TNomenclatureEntity>();

                                table.Clear();
                                table.AddRange(selector(result.Content));
                                nomenclatureDates[nomenclatureEnum] = lastUpdated;
                            }
                        };
                    }
                    else if (result.IsSuccessful)
                    {
                        return new Nom
                        {
                            NomEnum = nomenclatureEnum
                        };
                    }
                    else
                    {
                        return null;
                    }
                });
        }

        private static Task<Nom> PullTranslations(IRestClient restClient, INomenclatureDates nomenclatureDates, DateTime lastUpdated)
        {
            return restClient.GetAsync<List<TranslationLanguageApiDto>>("Translations/GetAll", alertOnException: false)
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

        private static Task<Nom> MapMobileNomenclatureToInvokePullNomenclatures(IRestClient restClient, INomenclatureDates nomenclatureDates, NomenclatureEnum nomenclatureEnum, DateTime lastUpdated)
        {
            const string prefix = "Nomenclatures/Get";

            switch (nomenclatureEnum)
            {
                case NomenclatureEnum.Law:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "Laws", lastUpdated,
                        (LawApiDto dto) => new NLaws
                        {
                            Code = dto.Code,
                            Name = dto.DisplayName,
                            Id = dto.Value,
                            Article = dto.Article,
                            Paragraph = dto.Paragraph,
                            Section = dto.Section,
                            Letter = dto.Letter,
                            SectionType = dto.SectionType,
                            LawSectionId = dto.LawSectionId,
                            LawSection = dto.Law,
                            LawText = dto.LawText,
                            Comments = dto.Comments,
                            IsActive = dto.IsActive,
                        });
                case NomenclatureEnum.Country:
                    return PullNomenclatures<NCountry>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "Countries", lastUpdated);
                case NomenclatureEnum.District:
                    return PullNomenclatures<NDistrict>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "Districts", lastUpdated);
                case NomenclatureEnum.DocumentType:
                    return PullNomenclatures<NDocumentType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "DocumentTypes", lastUpdated);
                case NomenclatureEnum.FileType:
                    return PullNomenclatures<NFileType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "FileTypes", lastUpdated);
                case NomenclatureEnum.InspectionState:
                    return PullNomenclatures<NInspectionState>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "InspectionStates", lastUpdated);
                case NomenclatureEnum.InspectionType:
                    return PullNomenclatures<NInspectionType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "InspectionTypes", lastUpdated);
                case NomenclatureEnum.Institution:
                    return PullNomenclatures<NInstitution>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "Institutions", lastUpdated);
                case NomenclatureEnum.Municipality:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "Municipalities", lastUpdated,
                        (MunicipalityApiDto dto) => new NMunicipality
                        {
                            Id = dto.Value,
                            Name = dto.DisplayName,
                            Code = dto.Code,
                            IsActive = dto.IsActive,
                            DistrictId = dto.DistrictId
                        });
                case NomenclatureEnum.ObservationTool:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "ObservationTools", lastUpdated,
                        (ObservationToolApiDto dto) => new NObservationTool
                        {
                            Id = dto.Value,
                            Name = dto.DisplayName,
                            Code = dto.Code,
                            OnBoard = dto.OnBoard,
                            IsActive = dto.IsActive,
                        });
                case NomenclatureEnum.PatrolVehicleType:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "PatrolVehicleTypes", lastUpdated,
                       (PatrolVehicleTypeApiDto dto) => new NPatrolVehicleType
                       {
                           Id = dto.Value,
                           Name = dto.DisplayName,
                           Code = dto.Code,
                           IsActive = dto.IsActive,
                           VehicleType = dto.VehicleType
                       });
                case NomenclatureEnum.PopulatedArea:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "PopulatedAreas", lastUpdated,
                       (PopulatedAreaApiDto dto) => new NPopulatedArea
                       {
                           Id = dto.Value,
                           Name = dto.DisplayName,
                           Code = dto.Code,
                           IsActive = dto.IsActive,
                           MunicipalityId = dto.MunicipalityId
                       });
                case NomenclatureEnum.VesselActivity:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "VesselActivities", lastUpdated,
                       (VesselActivityApiDto dto) => new NVesselActivity
                       {
                           Id = dto.Value,
                           Name = dto.DisplayName,
                           Code = dto.Code,
                           IsActive = dto.IsActive,
                           HasAdditionalDescr = dto.HasAdditionalDescr,
                           IsFishingActivity = dto.IsFishingActivity,
                       });
                case NomenclatureEnum.VesselType:
                    return PullNomenclatures<NVesselType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "VesselTypes", lastUpdated);
                case NomenclatureEnum.RequiredFileType:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "RequiredFileTypes", lastUpdated,
                       (RequiredFileTypeDto dto) => new NRequiredFileType
                       {
                           Id = dto.Value,
                           Name = dto.Code,
                           IsActive = dto.IsActive,
                           FileTypeId = dto.FileTypeId,
                           IsMandatory = dto.IsMandatory,
                       });
                case NomenclatureEnum.ShipAssociation:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "ShipAssociations", lastUpdated,
                       (NomenclatureDto dto) => new NShipAssociation
                       {
                           Id = dto.Value,
                           Code = dto.Code,
                           Name = dto.DisplayName,
                           IsActive = dto.IsActive,
                           NormalizedName = dto.DisplayName?.ToLower(),
                       });
                case NomenclatureEnum.InspectionCheckType:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "InspectionCheckTypes", lastUpdated,
                       (InspectionCheckTypeApiDto dto) => new NInspectionCheckType
                       {
                           Id = dto.Value,
                           Code = dto.Code,
                           Name = dto.DisplayName,
                           IsActive = dto.IsActive,
                           IsMandatory = dto.IsMandatory,
                           HasDescription = dto.HasAdditionalDescr,
                           InsectionTypeId = dto.InspectionTypeId,
                           Type = dto.CheckType,
                           DescriptionLabel = dto.DescriptionLabel,
                       });
                case NomenclatureEnum.CatchInspectionType:
                    return PullNomenclatures<NCatchInspectionType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "CatchInspectionTypes", lastUpdated);
                case NomenclatureEnum.Fish:
                    return PullNomenclatures<NFish>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "Fishes", lastUpdated);
                case NomenclatureEnum.FishingGear:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "FishingGears", lastUpdated,
                        (NomenclatureFishingGearDto dto) =>
                        {
                            if (dto.HasHooks)
                            {

                            }
                            return new NFishingGear()
                            {
                                Id = dto.Value,
                                Code = dto.Code,
                                Name = dto.DisplayName,
                                IsActive = dto.IsActive,
                                HasHooks = dto.HasHooks,
                            };
                        });
                case NomenclatureEnum.InspectedPersonType:
                    return PullNomenclatures<NInspectedPersonType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "InspectedPersonTypes", lastUpdated);
                case NomenclatureEnum.CatchZone:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "CatchZones", lastUpdated,
                       (NomenclatureCatchZoneDto dto) =>
                       {
                           NCatchZone entity = new NCatchZone
                           {
                               Id = dto.Value,
                               Code = dto.Code,
                               Name = dto.DisplayName,
                               IsActive = dto.IsActive,
                           };

                           if (dto.Block != null)
                           {
                               entity.X = dto.Block.X;
                               entity.Y = dto.Block.Y;
                               entity.Width = dto.Block.Width;
                               entity.Height = dto.Block.Height;
                           }

                           return entity;
                       });
                case NomenclatureEnum.WaterBodyType:
                    return PullNomenclatures<NWaterBodyType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "WaterBodyTypes", lastUpdated);
                case NomenclatureEnum.Port:
                    return PullNomenclatures(restClient, nomenclatureDates, nomenclatureEnum, prefix + "Ports", lastUpdated,
                       (NomenclatureDto dto) => new NPort
                       {
                           Id = dto.Value,
                           Code = dto.Code,
                           Name = dto.DisplayName,
                           IsActive = dto.IsActive,
                           NormalizedName = dto.DisplayName?.ToLower(),
                       });
                case NomenclatureEnum.FishingGearMarkStatus:
                    return PullNomenclatures<NFishingGearMarkStatus>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "FishingGearMarkStatuses", lastUpdated);
                case NomenclatureEnum.TransportVehicleType:
                    return PullNomenclatures<NTransportVehicleType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "TransportVehicleTypes", lastUpdated);
                case NomenclatureEnum.Gender:
                    return PullNomenclatures<NGender>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "Genders", lastUpdated);
                case NomenclatureEnum.PermitLicenseType:
                    return PullNomenclatures<NPermitLicenseType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "PermitLicenseTypes", lastUpdated);
                case NomenclatureEnum.FleetType:
                    return PullNomenclatures<NFleetType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "FleetTypes", lastUpdated);
                case NomenclatureEnum.FishPresentation:
                    return PullNomenclatures<NFishPresentation>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "FishPresentations", lastUpdated);
                case NomenclatureEnum.FishSex:
                    return PullNomenclatures<NFishSex>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "FishSex", lastUpdated);
                case NomenclatureEnum.FishingGearPingerStatus:
                    return PullNomenclatures<NFishingGearPingerStatus>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "FishingGearPingerStatuses", lastUpdated);
                case NomenclatureEnum.FishingGearCheckReason:
                    return PullNomenclatures<NFishingGearCheckReason>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "FishingGearCheckReasons", lastUpdated);
                case NomenclatureEnum.FishingGearRecheckReason:
                    return PullNomenclatures<NFishingGearRecheckReason>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "FishingGearRecheckReasons", lastUpdated);
                case NomenclatureEnum.PermitType:
                    return PullNomenclatures<NPermitType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "PermitTypes", lastUpdated);
                case NomenclatureEnum.InspectionVesselType:
                    return PullNomenclatures<NInspectionVesselType>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "InspectionVesselTypes", lastUpdated);
                case NomenclatureEnum.TurbotSizeGroup:
                    return PullNomenclatures<NTurbotSizeGroup>(restClient, nomenclatureDates, nomenclatureEnum, prefix + "TurbotSizeGroups", lastUpdated);
                case NomenclatureEnum.Translation:
                    return PullTranslations(restClient, nomenclatureDates, lastUpdated);
                default:
                    throw new NotImplementedException($"{nameof(nomenclatureEnum)} doesn't have specified {nameof(NomenclatureEnum)} implemented");
            }
        }

        private static NomenclatureEnum? MapToNomenclatureEnum(string nomenclatureName)
        {
            switch (nomenclatureName)
            {
                case "Laws":
                    return NomenclatureEnum.Law;
                case "Countries":
                    return NomenclatureEnum.Country;
                case "Districts":
                    return NomenclatureEnum.District;
                case "DocumentTypes":
                    return NomenclatureEnum.DocumentType;
                case "FileTypes":
                    return NomenclatureEnum.FileType;
                case "InspectionStates":
                    return NomenclatureEnum.InspectionState;
                case "InspectionTypes":
                    return NomenclatureEnum.InspectionType;
                case "Institutions":
                    return NomenclatureEnum.Institution;
                case "Municipalities":
                    return NomenclatureEnum.Municipality;
                case "ObservationTools":
                    return NomenclatureEnum.ObservationTool;
                case "PatrolVehicleTypes":
                    return NomenclatureEnum.PatrolVehicleType;
                case "PopulatedAreas":
                    return NomenclatureEnum.PopulatedArea;
                case "VesselActivities":
                    return NomenclatureEnum.VesselActivity;
                case "VesselTypes":
                    return NomenclatureEnum.VesselType;
                case "RequiredFileTypes":
                    return NomenclatureEnum.RequiredFileType;
                case "ShipAssociations":
                    return NomenclatureEnum.ShipAssociation;
                case "InspectionCheckTypes":
                    return NomenclatureEnum.InspectionCheckType;
                case "CatchInspectionTypes":
                    return NomenclatureEnum.CatchInspectionType;
                case "Fishes":
                    return NomenclatureEnum.Fish;
                case "FishingGears":
                    return NomenclatureEnum.FishingGear;
                case "InspectedPersonTypes":
                    return NomenclatureEnum.InspectedPersonType;
                case "CatchZones":
                    return NomenclatureEnum.CatchZone;
                case "WaterBodyTypes":
                    return NomenclatureEnum.WaterBodyType;
                case "Ports":
                    return NomenclatureEnum.Port;
                case "FishingGearMarkStatuses":
                    return NomenclatureEnum.FishingGearMarkStatus;
                case "TransportVehicleTypes":
                    return NomenclatureEnum.TransportVehicleType;
                case "Genders":
                    return NomenclatureEnum.Gender;
                case "CommercialFishingPermitLicenseTypes":
                    return NomenclatureEnum.PermitLicenseType;
                case "FleetTypes":
                    return NomenclatureEnum.FleetType;
                case "FishPresentations":
                    return NomenclatureEnum.FishPresentation;
                case "FishSex":
                    return NomenclatureEnum.FishSex;
                case "FishingGearPingerStatuses":
                    return NomenclatureEnum.FishingGearPingerStatus;
                case "FishingGearCheckReasons":
                    return NomenclatureEnum.FishingGearCheckReason;
                case "FishingGearRecheckReasons":
                    return NomenclatureEnum.FishingGearRecheckReason;
                case "CommercialFishingPermitTypes":
                    return NomenclatureEnum.PermitType;
                case "TurbotSizeGroups":
                    return NomenclatureEnum.TurbotSizeGroup;
                case "InspectionVesselTypes":
                    return NomenclatureEnum.InspectionVesselType;
                case "TranslationGroups":
                case "TranslationResources":
                    return NomenclatureEnum.Translation;
                default:
                    return null;
            }
        }
    }
}
