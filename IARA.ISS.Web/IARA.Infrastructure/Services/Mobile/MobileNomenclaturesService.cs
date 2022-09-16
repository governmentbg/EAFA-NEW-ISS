using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Address;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.FishingTickets;
using IARA.DomainModels.DTOModels.Mobile.Nomenclatures;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;

namespace IARA.Infrastructure.Services.Nomenclatures
{
    public class MobileNomenclaturesService : Service, IMobileNomenclaturesService
    {
        public MobileNomenclaturesService(IARADbContext dbContext)
            : base(dbContext)
        { }

        public List<MobileNomenclatureDTO> GetNomenclatureTables(MobileTypeEnum mobileType)
        {
            List<string> nomTables = (mobileType switch
            {
                MobileTypeEnum.Pub => new List<IListSource>
                {
                    this.Db.Ncountries,
                    this.Db.Ndistricts,
                    this.Db.NdocumentTypes,
                    this.Db.Nfishes,
                    this.Db.Nmunicipalities,
                    this.Db.NpopulatedAreas,
                    this.Db.NticketTypes,
                    this.Db.NticketPeriods,
                    this.Db.NtranslationResources,
                    this.Db.NtranslationGroups,
                    this.Db.NpermitReasons,
                    this.Db.NfileTypes,
                    this.Db.NviolationSignalTypes,
                    this.Db.NmobileVersions,
                    this.Db.Ntariffs,
                    this.Db.Ngenders,
                    this.Db.NsystemParameters,
                    this.Db.NpaymentTypes,
                },
                MobileTypeEnum.Insp => new List<IListSource>
                {
                    this.Db.Ncountries,
                    this.Db.Ndistricts,
                    this.Db.NdocumentTypes,
                    this.Db.NfileTypes,
                    this.Db.NinspectionStates,
                    this.Db.NinspectionTypes,
                    this.Db.Ninstitutions,
                    this.Db.Nmunicipalities,
                    this.Db.NobservationTools,
                    this.Db.NpatrolVehicleTypes,
                    this.Db.NpopulatedAreas,
                    this.Db.NvesselActivities,
                    this.Db.NvesselTypes,
                    this.Db.NrequiredFileTypes,
                    this.Db.NshipAssociations,
                    this.Db.NinspectionCheckTypes,
                    this.Db.Nfishes,
                    this.Db.NcatchInspectionTypes,
                    this.Db.NfishingGears,
                    this.Db.NinspectedPersonTypes,
                    this.Db.NcatchZones,
                    this.Db.NwaterBodyTypes,
                    this.Db.Nports,
                    this.Db.NfishingGearMarkStatuses,
                    this.Db.NtransportVehicleTypes,
                    this.Db.Ngenders,
                    this.Db.NtranslationResources,
                    this.Db.NtranslationGroups,
                    this.Db.NcommercialFishingPermitLicenseTypes,
                    this.Db.NfleetTypes,
                    this.Db.NfishPresentations,
                    this.Db.NfishSexes,
                    this.Db.NfishingGearPingerStatuses,
                    this.Db.NfishingGearCheckReasons,
                    this.Db.NfishingGearRecheckReasons,
                },
                _ => throw new NotImplementedException($"{nameof(MobileTypeEnum)} not implemented in {nameof(GetNomenclatureTables)}"),
            }).ConvertAll(f => f.GetType().GetDbSetTableName());

            return (
                from table in this.Db.NnomenclatureTables
                where nomTables.Contains(table.Name)
                orderby table.Name
                select new MobileNomenclatureDTO
                {
                    LastUpdated = table.DataLastEditOn,
                    NomenclatureName = table.Name[0] == 'N'
                        ? table.Name.Substring(1)
                        : table.Name
                }
            ).ToList();
        }

        public List<NomenclatureDTO> GetCountries()
        {
            return this.GetCodeNomenclature(this.Db.Ncountries);
        }

        public List<NomenclatureDTO> GetDistricts()
        {
            return this.GetCodeNomenclature(this.Db.Ndistricts);
        }

        public List<NomenclatureDTO> GetDocumentTypes()
        {
            return this.GetCodeNomenclature(this.Db.NdocumentTypes);
        }

        public List<NomenclatureDTO> GetFishes()
        {
            return this.GetCodeNomenclature(this.Db.Nfishes);
        }

        public List<NomenclatureDTO> GetPermitReasons()
        {
            return this.GetNomenclature(this.Db.NpermitReasons);
        }

        public List<NomenclatureDTO> GetFileTypes()
        {
            return this.GetCodeNomenclature(this.Db.NfileTypes);
        }

        public List<NomenclatureDTO> GetViolationSignalTypes()
        {
            return this.GetCodeNomenclature(this.Db.NviolationSignalTypes);
        }

        public List<MunicipalityNomenclatureExtendedDTO> GetMunicipalities()
        {
            DateTime now = DateTime.Now;

            return (
                from multiplicity in this.Db.Nmunicipalities
                orderby multiplicity.Name
                select new MunicipalityNomenclatureExtendedDTO
                {
                    DisplayName = multiplicity.Name,
                    Value = multiplicity.Id,
                    IsActive = multiplicity.ValidFrom < now && multiplicity.ValidTo > now,
                    DistrictId = multiplicity.DistrictId
                }
            ).ToList();
        }

        public List<InspectionObservationToolNomenclatureDTO> GetObservationTools()
        {
            DateTime now = DateTime.Now;

            return (
                from obsTool in this.Db.NobservationTools
                orderby obsTool.Name
                select new InspectionObservationToolNomenclatureDTO
                {
                    Value = obsTool.Id,
                    Code = obsTool.Code,
                    DisplayName = obsTool.Name,
                    OnBoard = Enum.Parse<ObservationToolOnBoardEnum>(obsTool.IsOnBoardType),
                    IsActive = obsTool.ValidFrom < now && obsTool.ValidTo > now,
                }
            ).ToList();
        }

        public List<PopulatedAreaNomenclatureExtendedDTO> GetPopulatedAreas()
        {
            DateTime now = DateTime.Now;

            return (
                from populatedArea in this.Db.NpopulatedAreas
                orderby populatedArea.Name
                select new PopulatedAreaNomenclatureExtendedDTO
                {
                    DisplayName = populatedArea.Name,
                    Value = populatedArea.Id,
                    IsActive = populatedArea.ValidFrom < now && populatedArea.ValidTo > now,
                    MunicipalityId = populatedArea.MunicipalityId
                }
            ).ToList();
        }

        public List<TicketTypeNomenclatureDTO> GetTicketTypes()
        {
            DateTime now = DateTime.Now;

            return (
                from ticketType in this.Db.NticketTypes
                orderby ticketType.Name
                select new TicketTypeNomenclatureDTO
                {
                    DisplayName = ticketType.Name,
                    Code = ticketType.Code,
                    Value = ticketType.Id,
                    IsActive = ticketType.ValidFrom < now && ticketType.ValidTo > now,
                    OrderNo = ticketType.OrderNo,
                }
            ).ToList();
        }

        public List<NomenclatureDTO> GetTicketPeriods()
        {
            return this.GetCodeNomenclature(this.Db.NticketPeriods);
        }

        public List<NomenclatureDTO> GetGenders()
        {
            return this.GetCodeNomenclature(this.Db.Ngenders);
        }

        public List<TicketPeriodPriceDTO> GetTicketTariffs()
        {
            DateTime now = DateTime.Now;
            return (
               from tariff in this.Db.Ntariffs
               where tariff.Code.StartsWith("Ticket_")
               orderby tariff.Name
               select new TicketPeriodPriceDTO
               {
                   Value = tariff.Id,
                   Code = tariff.Code,
                   DisplayName = tariff.Name,
                   Price = tariff.Price,
                   IsActive = tariff.ValidFrom < now && tariff.ValidTo > now
               }
           ).ToList();
        }

        public List<RequiredFileTypeNomenclatureDTO> GetRequiredFileTypes()
        {
            DateTime now = DateTime.Now;
            return (
               from rft in this.Db.NrequiredFileTypes
               orderby rft.Id
               select new RequiredFileTypeNomenclatureDTO
               {
                   Value = rft.Id,
                   Code = rft.PageCode,
                   FileTypeId = rft.FileTypeId,
                   IsMandatory = rft.IsMandatory,
                   IsActive = rft.ValidFrom < now && rft.ValidTo > now
               }
           ).ToList();
        }

        public List<MobileVersionNomenclatureDTO> GetMobileVersions()
        {
            return (
                from version in this.Db.NmobileVersions
                where !version.PageCode.StartsWith("ALL_")
                orderby version.Id
                select new MobileVersionNomenclatureDTO
                {
                    Value = version.Id,
                    Code = version.PageCode,
                    Version = version.ForceMinBuildNum,
                    IsActive = version.IsActive,
                    OSType = version.Ostype,
                }
            ).ToList();
        }

        public List<NomenclatureDTO> GetShipAssociations()
        {
            return this.GetCodeNomenclature(this.Db.NshipAssociations);
        }

        public List<SystemParameterNomenclatureDTO> GetSystemParameters()
        {
            DateTime now = DateTime.Now;

            string[] parameters = new string[]
            {
                "ELDER_TICKET_FEMALE_AGE",
                "ELDER_TICKET_MALE_AGE",
                "MAX_NUMBER_OF_UNDER14_TICKETS"
            };

            return this.Db.NsystemParameters
                .Where(x => parameters.Contains(x.Code) && x.ValidFrom < now && x.ValidTo > now)
                .OrderBy(f => f.Name)
                .Select(x => new SystemParameterNomenclatureDTO
                {
                    ParamValue = x.ParamValue,
                    DisplayName = x.Name,
                    Value = x.Id,
                    Code = x.Code,
                    IsActive = true
                })
                .ToList();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }

        public List<NomenclatureDTO> GetInspectionStates()
        {
            return this.GetCodeNomenclature(this.Db.NinspectionStates);
        }

        public List<NomenclatureDTO> GetInspectionTypes()
        {
            return this.GetCodeNomenclature(this.Db.NinspectionTypes);
        }

        public List<NomenclatureDTO> GetInstitutions()
        {
            return this.GetCodeNomenclature(this.Db.Ninstitutions);
        }

        public List<PatrolVehicleTypeNomenclatureDTO> GetPatrolVehicleTypes()
        {
            DateTime now = DateTime.Now;

            return (
                from pvt in this.Db.NpatrolVehicleTypes
                orderby pvt.Name
                select new PatrolVehicleTypeNomenclatureDTO
                {
                    DisplayName = pvt.Name,
                    Value = pvt.Id,
                    Code = pvt.Code,
                    VehicleType = Enum.Parse<PatrolVehicleTypeEnum>(pvt.VehicleType),
                    IsActive = pvt.ValidFrom < now && pvt.ValidTo > now
                }
            ).ToList();
        }

        public List<InspectionVesselActivityNomenclatureDTO> GetVesselActivities()
        {
            DateTime now = DateTime.Now;

            return (
                from va in this.Db.NvesselActivities
                orderby va.Name
                select new InspectionVesselActivityNomenclatureDTO
                {
                    DisplayName = va.Name,
                    Value = va.Id,
                    Code = va.Code,
                    HasAdditionalDescr = va.HasAdditionalDescr,
                    IsFishingActivity = va.IsFishingActivity,
                    IsActive = va.ValidFrom < now && va.ValidTo > now
                }
            ).ToList();
        }

        public List<NomenclatureDTO> GetVesselTypes()
        {
            return this.GetCodeNomenclature(this.Db.NvesselTypes);
        }

        public List<InspectionCheckTypeNomenclatureDTO> GetInspectionCheckTypes()
        {
            DateTime now = DateTime.Now;

            return (
                from ict in this.Db.NinspectionCheckTypes
                orderby ict.Name
                select new InspectionCheckTypeNomenclatureDTO
                {
                    DisplayName = ict.Name,
                    Value = ict.Id,
                    Code = ict.Code,
                    CheckType = Enum.Parse<InspectionCheckTypesEnum>(ict.CheckType, true),
                    HasAdditionalDescr = ict.HasDescription,
                    InspectionTypeId = ict.InspectionTypeId,
                    IsMandatory = ict.IsMandatory,
                    DescriptionLabel = ict.DescriptionLabel,
                    IsActive = ict.ValidFrom < now && ict.ValidTo > now
                }
            ).ToList();
        }

        public List<NomenclatureDTO> GetCatchInspectionTypes()
        {
            return this.GetCodeNomenclature(this.Db.NcatchInspectionTypes);
        }

        public List<NomenclatureDTO> GetFishingGears()
        {
            return this.GetCodeNomenclature(this.Db.NfishingGears);
        }

        public List<NomenclatureDTO> GetPaymentTypes()
        {
            return this.GetCodeNomenclature(this.Db.NpaymentTypes);
        }

        public List<NomenclatureDTO> GetInspectedPersonTypes()
        {
            return this.GetCodeNomenclature(this.Db.NinspectedPersonTypes);
        }

        public List<NomenclatureCatchZoneDTO> GetCatchZones()
        {
            DateTime now = DateTime.Now;

            var result = (
                from catchZone in this.Db.NcatchZones
                orderby catchZone.Gfcmquadrant
                select new
                {
                    Value = catchZone.Id,
                    Code = catchZone.ZoneNum.ToString(),
                    DisplayName = catchZone.Gfcmquadrant,
                    Points = catchZone.QuadrantCoodtinates.Coordinates,
                    IsActive = catchZone.ValidFrom < now && catchZone.ValidTo > now
                }
            ).ToList();

            return (
                from catchZone in result
                select new NomenclatureCatchZoneDTO
                {
                    Value = catchZone.Value,
                    Code = catchZone.Code,
                    DisplayName = catchZone.DisplayName,
                    Block = catchZone.Points == null ? null
                        : new RectangleDTO
                        {
                            X = catchZone.Points[0].X,
                            Y = catchZone.Points[0].Y,
                            Width = catchZone.Points[1].X - catchZone.Points[0].X,
                            Height = catchZone.Points[0].Y - catchZone.Points[2].Y,
                        },
                    IsActive = catchZone.IsActive
                }
            ).ToList();
        }

        public List<NomenclatureDTO> GetWaterBodyTypes()
        {
            return this.GetCodeNomenclature(this.Db.NwaterBodyTypes);
        }

        public List<NomenclatureDTO> GetPorts()
        {
            return this.GetCodeNomenclature(this.Db.Nports);
        }

        public List<NomenclatureDTO> GetFishingGearMarkStatuses()
        {
            return this.GetCodeNomenclature(this.Db.NfishingGearMarkStatuses);
        }

        public List<NomenclatureDTO> GetFishingGearPingerStatuses()
        {
            return this.GetCodeNomenclature(this.Db.NfishingGearPingerStatuses);
        }

        public List<NomenclatureDTO> GetTransportVehicleTypes()
        {
            return this.GetCodeNomenclature(this.Db.NtransportVehicleTypes);
        }

        public List<NomenclatureDTO> GetPermitLicenseTypes()
        {
            DateTime now = DateTime.Now;

            return (
                from type in this.Db.NcommercialFishingPermitLicenseTypes
                orderby type.Name
                select new NomenclatureDTO
                {
                    DisplayName = type.ShortName,
                    Value = type.Id,
                    Code = type.Code,
                    Description = type.Name,
                    IsActive = type.ValidFrom < now && type.ValidTo > now
                }
            ).ToList();
        }

        public List<NomenclatureDTO> GetFleetTypes()
        {
            return this.GetCodeNomenclature(this.Db.NfleetTypes);
        }

        public List<NomenclatureDTO> GetFishPresentations()
        {
            return this.GetCodeNomenclature(this.Db.NfishPresentations);
        }

        public List<NomenclatureDTO> GetFishSex()
        {
            return this.GetCodeNomenclature(this.Db.NfishSexes);
        }

        public List<NomenclatureDTO> GetFishingGearCheckReasons()
        {
            return this.GetCodeNomenclature(this.Db.NfishingGearCheckReasons);
        }

        public List<NomenclatureDTO> GetFishingGearRecheckReasons()
        {
            return this.GetCodeNomenclature(this.Db.NfishingGearRecheckReasons);
        }
    }
}
