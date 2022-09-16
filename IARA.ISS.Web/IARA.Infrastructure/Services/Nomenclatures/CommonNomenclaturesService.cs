using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Address;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public class CommonNomenclaturesService : Service, ICommonNomenclaturesService
    {
        public static readonly string BG_CODE = "BGR";

        public CommonNomenclaturesService(IARADbContext dbContext)
                : base(dbContext)
        {
        }

        public List<NomenclatureDTO> GetCountries()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> countries = (from country in this.Db.Ncountries
                                               orderby country.Code == BG_CODE descending, country.Name
                                               select new NomenclatureDTO
                                               {
                                                   Value = country.Id,
                                                   Code = country.Code,
                                                   DisplayName = country.Name,
                                                   IsActive = country.ValidFrom <= now && country.ValidTo > now
                                               }).ToList();
            return countries;
        }

        public List<NomenclatureDTO> GetDistricts()
        {
            return this.GetCodeNomenclature(this.Db.Ndistricts);
        }

        public List<MunicipalityNomenclatureExtendedDTO> GetMuncipalities()
        {
            DateTime now = DateTime.Now;

            List<MunicipalityNomenclatureExtendedDTO> municipalities = (from mun in this.Db.Nmunicipalities
                                                                        orderby mun.Name
                                                                        select new MunicipalityNomenclatureExtendedDTO
                                                                        {
                                                                            Value = mun.Id,
                                                                            Code = mun.Code,
                                                                            DisplayName = mun.Name,
                                                                            DistrictId = mun.DistrictId,
                                                                            IsActive = mun.ValidFrom <= now && mun.ValidTo > now
                                                                        }).ToList();

            return municipalities;
        }

        public List<PopulatedAreaNomenclatureExtendedDTO> GetPopulatedAreas()
        {
            DateTime now = DateTime.Now;

            List<PopulatedAreaNomenclatureExtendedDTO> pops = (from pop in this.Db.NpopulatedAreas
                                                               join mun in this.Db.Nmunicipalities on pop.MunicipalityId equals mun.Id
                                                               join dis in this.Db.Ndistricts on mun.DistrictId equals dis.Id
                                                               orderby pop.Name
                                                               select new PopulatedAreaNomenclatureExtendedDTO
                                                               {
                                                                   Value = pop.Id,
                                                                   Code = pop.Code,
                                                                   AreaType = pop.AreaType,
                                                                   DisplayName = $"{{ARE}} {pop.Name}",
                                                                   Description = $"{{DIS}} {dis.Name}, {{MUN}} {mun.Name}",
                                                                   MunicipalityId = pop.MunicipalityId,
                                                                   IsActive = pop.ValidFrom <= now && pop.ValidTo > now
                                                               }).ToList();

            return pops;
        }

        public List<FishNomenclatureDTO> GetFishes()
        {
            DateTime now = DateTime.Now;
            List<FishNomenclatureDTO> results = new List<FishNomenclatureDTO>();

            var fishes = (from fish in this.Db.Nfishes
                          join fishFamily in this.Db.NfishFamilies on fish.FishFamilyId equals fishFamily.Id into fm
                          from fishFamily in fm.DefaultIfEmpty()
                          select new
                          {
                              Value = fish.Id,
                              Code = fish.Code,
                              DisplayName = fish.Name,
                              IsActive = fish.ValidFrom <= now && fish.ValidTo > now,
                              FamilyTypeCode = fishFamily != null ? fishFamily.FamilyType : null,
                              IsDanube = fish.IsDanube,
                              IsBlackSea = fish.IsBlackSea,
                              IsInternal = fish.IsInternal
                          }).ToList();

            List<int> fishIds = fishes.Select(x => x.Value).ToList();

            var quotaFishPorts = (from quotaFish in this.Db.CatchQuotas
                                  join quotaPort in this.Db.CatchQuotaUnloadPorts on quotaFish.Id equals quotaPort.CatchQuotaId
                                  where fishIds.Contains(quotaFish.FishId)
                                  select new
                                  {
                                      quotaFish.Id,
                                      quotaFish.FishId,
                                      quotaPort.PortId,
                                      quotaPort.IsActive,
                                      QuotaPeriodStart = quotaFish.PeriodStart,
                                      QuotaPeriodEnd = quotaFish.PeriodEnd
                                  }).ToLookup(x => x.FishId,
                                              y => new
                                              {
                                                  QuotaId = y.Id,
                                                  y.QuotaPeriodStart,
                                                  y.QuotaPeriodEnd,
                                                  QuotaPort = new QuotaSpiciesPortDTO
                                                  {
                                                      PortId = y.PortId,
                                                      IsActive = y.IsActive
                                                  }
                                              });

            foreach (var fish in fishes)
            {
                bool succFamilyTypeCast = Enum.TryParse<FishFamilyTypesEnum>(fish.FamilyTypeCode, out FishFamilyTypesEnum familyType);

                FishNomenclatureDTO fishNomenclature = new FishNomenclatureDTO
                {
                    Value = fish.Value,
                    DisplayName = fish.DisplayName,
                    Code = fish.Code,
                    FamilyType = succFamilyTypeCast ? familyType : default(FishFamilyTypesEnum?),
                    IsBlackSea = fish.IsBlackSea,
                    IsDanube = fish.IsDanube,
                    IsInternal = fish.IsInternal,
                    IsActive = fish.IsActive
                };

                if (quotaFishPorts.Contains(fish.Value))
                {
                    var quotaPorts = quotaFishPorts[fish.Value].ToList();
                    fishNomenclature.QuotaId = quotaPorts.First().QuotaId;
                    fishNomenclature.QuotaSpiciesPermittedPortIds = quotaPorts.Select(x => x.QuotaPort).ToList();
                    fishNomenclature.QuotaPeriodFrom = quotaPorts.First().QuotaPeriodStart;
                    fishNomenclature.QuotaPeriodTo = quotaPorts.First().QuotaPeriodEnd;
                }

                results.Add(fishNomenclature);
            }

            return results;
        }

        public List<NomenclatureDTO> GetDepartments()
        {
            return this.GetNomenclature(this.Db.Ndepartments);
        }

        public List<NomenclatureDTO> GetDocumentTypes()
        {
            return this.GetCodeNomenclature(this.Db.NdocumentTypes);
        }

        public List<NomenclatureDTO> GetSectors()
        {
            return this.GetNomenclature(this.Db.Nsectors);
        }

        public List<NomenclatureDTO> GetTerritoryUnits()
        {
            return this.GetCodeNomenclature(this.Db.NterritoryUnits);
        }

        public AddressNomenclaturesDTO GetAddressNomenclatures()
        {
            DateTime now = DateTime.Now;

            AddressNomenclaturesDTO result = new AddressNomenclaturesDTO
            {
                Countries = this.GetCountries(),
                Districts = this.GetDistricts(),
                Municipalities = (from mun in this.Db.Nmunicipalities
                                  orderby mun.Name
                                  select new MunicipalityNomenclatureExtendedDTO
                                  {
                                      Value = mun.Id,
                                      DisplayName = mun.Name,
                                      IsActive = mun.ValidFrom <= now && mun.ValidTo >= now,
                                      DistrictId = mun.DistrictId
                                  }).ToList(),
                PopulatedAreas = (from pop in this.Db.NpopulatedAreas
                                  orderby pop.Name
                                  select new PopulatedAreaNomenclatureExtendedDTO
                                  {
                                      Value = pop.Id,
                                      DisplayName = pop.Name,
                                      IsActive = pop.ValidFrom <= now && pop.ValidTo >= now,
                                      MunicipalityId = pop.MunicipalityId
                                  }).ToList()
            };
            return result;
        }

        public List<NomenclatureDTO> GetFileTypes()
        {
            return this.GetCodeNomenclature(this.Db.NfileTypes);
        }

        public List<NomenclatureDTO> GetPermissions()
        {
            return this.GetNomenclature(this.Db.Npermissions);
        }

        public List<NomenclatureDTO> GetUserNames()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> users = (from user in this.Db.Users
                                           join person in this.Db.Persons on user.PersonId equals person.Id
                                           orderby person.FirstName, person.LastName, user.Username
                                           select new NomenclatureDTO
                                           {
                                               Value = user.Id,
                                               DisplayName = $"{person.FirstName} {person.LastName} ({user.Username})",
                                               IsActive = user.ValidFrom <= now && user.ValidTo > now
                                           }).ToList();
            return users;
        }

        public List<NomenclatureDTO> GetOfflinePaymentTypes()
        {
            return this.GetPaymentTypes(online: false);
        }

        public List<NomenclatureDTO> GetOnlinePaymentTypes()
        {
            return this.GetPaymentTypes(online: true);
        }

        public List<NomenclatureDTO> GetGenders()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> genders = (from gender in this.Db.Ngenders
                                             orderby gender.Name descending
                                             select new NomenclatureDTO
                                             {
                                                 Value = gender.Id,
                                                 Code = gender.Code,
                                                 DisplayName = gender.Name,
                                                 IsActive = gender.ValidFrom <= now && gender.ValidTo > now
                                             }).ToList();
            return genders;
        }

        public List<SubmittedByRoleNomenclatureDTO> GetSubmittedByRoles()
        {
            DateTime now = DateTime.Now;

            List<SubmittedByRoleNomenclatureDTO> roles = (from applTypeRole in this.Db.MapApplicationTypeSubmittedByRoles
                                                          join applicationType in this.Db.NapplicationTypes on applTypeRole.ApplicationTypeId equals applicationType.Id
                                                          join role in this.Db.NsubmittedByRoles on applTypeRole.SubmittedByRoleId equals role.Id
                                                          where applTypeRole.IsActive
                                                          select new SubmittedByRoleNomenclatureDTO
                                                          {
                                                              Value = role.Id,
                                                              Code = role.Code,
                                                              DisplayName = role.Name,
                                                              ApplicationPageCode = Enum.Parse<PageCodeEnum>(applicationType.PageCode),
                                                              HasLetterOfAttorney = role.HasLetterOfAttorney,
                                                              IsActive = role.ValidFrom <= now && role.ValidTo > now
                                                          }).ToList();

            return roles;
        }

        public List<CancellationReasonDTO> GetCancellationReasons()
        {
            DateTime now = DateTime.Now;

            List<CancellationReasonDTO> result = (from reason in this.Db.NcancellationReasons
                                                  orderby reason.Name
                                                  select new CancellationReasonDTO
                                                  {
                                                      Value = reason.Id,
                                                      Code = reason.Code,
                                                      DisplayName = reason.Name,
                                                      Group = Enum.Parse<CancellationReasonGroupEnum>(reason.Group),
                                                      IsActive = reason.ValidFrom <= now && reason.ValidTo >= now
                                                  }).ToList();
            return result;
        }

        public List<PermittedFileTypeDTO> GetPermittedFileTypes()
        {
            DateTime now = DateTime.Now;

            List<PermittedFileTypeDTO> result = (from requiredFileType in this.Db.NrequiredFileTypes
                                                 join fileType in this.Db.NfileTypes on requiredFileType.FileTypeId equals fileType.Id
                                                 orderby requiredFileType.IsMandatory descending
                                                 select new PermittedFileTypeDTO
                                                 {
                                                     Value = fileType.Id,
                                                     Code = fileType.Code,
                                                     DisplayName = fileType.Name,
                                                     Description = fileType.Description,
                                                     IsRequired = requiredFileType.IsMandatory,
                                                     PageCode = Enum.Parse<PageCodeEnum>(requiredFileType.PageCode),
                                                     IsActive = requiredFileType.ValidFrom <= now
                                                                && requiredFileType.ValidTo > now
                                                                && fileType.ValidFrom <= now
                                                                && fileType.ValidTo > now
                                                 }).ToList();

            return result;
        }

        public List<ApplicationDeliveryTypeDTO> GetDeliveryTypes()
        {
            DateTime now = DateTime.Now;

            List<ApplicationDeliveryTypeDTO> result = (from deliveryType in this.Db.NdeliveryTypes
                                                       join applTypeDeliveryType in this.Db.MapApplicationTypeDeliveryTypes on deliveryType.Id equals applTypeDeliveryType.DeliveryTypeId
                                                       join applType in this.Db.NapplicationTypes on applTypeDeliveryType.ApplicationTypeId equals applType.Id
                                                       where applTypeDeliveryType.IsActive
                                                       orderby deliveryType.OrderNum
                                                       select new ApplicationDeliveryTypeDTO
                                                       {
                                                           Value = deliveryType.Id,
                                                           Code = deliveryType.Code,
                                                           DisplayName = deliveryType.Name,
                                                           IsActive = deliveryType.ValidFrom <= now && deliveryType.ValidTo > now,
                                                           PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode)
                                                       }).ToList();

            return result;
        }

        public List<ChangeOfCircumstancesTypeDTO> GetChangeOfCircumstancesTypes()
        {
            DateTime now = DateTime.Now;

            List<ChangeOfCircumstancesTypeDTO> types = (from type in this.Db.NchangeOfCircumstancesTypes
                                                        select new ChangeOfCircumstancesTypeDTO
                                                        {
                                                            Value = type.Id,
                                                            Code = type.Code,
                                                            DisplayName = type.Name,
                                                            PageCode = Enum.Parse<PageCodeEnum>(type.PageCode),
                                                            DataType = Enum.Parse<ChangeOfCircumstancesDataTypeEnum>(type.DataType),
                                                            IsDeletion = type.IsDeletion,
                                                            IsActive = type.ValidFrom <= now && type.ValidTo > now
                                                        }).ToList();

            return types;
        }

        public List<FishingGearNomenclatureDTO> GetFishingGear()
        {
            DateTime now = DateTime.Now;

            List<FishingGearNomenclatureDTO> gears = (from fishingGear in this.Db.NfishingGears
                                                      join fishingGearType in this.Db.NfishingGearTypes on fishingGear.GearTypeId equals fishingGearType.Id
                                                      select new FishingGearNomenclatureDTO
                                                      {
                                                          Value = fishingGear.Id,
                                                          Code = fishingGear.Code,
                                                          DisplayName = fishingGear.Name,
                                                          Type = Enum.Parse<FishingGearParameterTypesEnum>(fishingGear.GearParametersType),
                                                          IsForMutualFishing = fishingGear.IsForMutualFishing,
                                                          HasHooks = fishingGearType.HasHooks,
                                                          IsActive = fishingGear.ValidFrom <= now && fishingGear.ValidTo > now
                                                      }).ToList();

            ILookup<int, int> fishingGearPermitLicenses = (from fishingGear in this.Db.NfishingGears
                                                           join fishingGearRegister in this.Db.FishingGearRegisters on fishingGear.Id equals fishingGearRegister.FishingGearTypeId
                                                           join permitLicense in this.Db.CommercialFishingPermitLicensesRegisters on fishingGearRegister.PermitLicenseId equals permitLicense.Id
                                                           select new
                                                           {
                                                               FishingGearId = fishingGear.Id,
                                                               PermitLicense = permitLicense.Id
                                                           }).ToLookup(x => x.FishingGearId, y => y.PermitLicense);

            foreach (FishingGearNomenclatureDTO gear in gears)
            {
                gear.PermitLicenseIds = fishingGearPermitLicenses[gear.Value].ToHashSet();
            }

            return gears;
        }

        public List<NomenclatureDTO> GetFishingGearMarkStatuses()
        {
            return this.GetCodeNomenclature(this.Db.NfishingGearMarkStatuses);
        }

        public List<NomenclatureDTO> GetFishingGearPingerStatuses()
        {
            return this.GetCodeNomenclature(this.Db.NfishingGearPingerStatuses);
        }

        public List<NomenclatureDTO> GetInstitutions()
        {
            return this.GetCodeNomenclature(this.Db.Ninstitutions);
        }

        public List<NomenclatureDTO> GetVesselTypes()
        {
            return this.GetCodeNomenclature(this.Db.NvesselTypes);
        }

        public List<PatrolVehicleTypeNomenclatureDTO> GetPatrolVehicleTypes()
        {
            DateTime now = DateTime.Now;

            List<PatrolVehicleTypeNomenclatureDTO> result = (
                from pvt in this.Db.NpatrolVehicleTypes
                orderby pvt.Name
                select new PatrolVehicleTypeNomenclatureDTO
                {
                    Value = pvt.Id,
                    Code = pvt.Code,
                    DisplayName = pvt.Name,
                    VehicleType = Enum.Parse<PatrolVehicleTypeEnum>(pvt.VehicleType),
                    IsActive = pvt.ValidFrom <= now && pvt.ValidTo >= now
                }).ToList();

            return result;
        }

        public List<NomenclatureDTO> GetCatchCheckTypes()
        {
            return this.GetCodeNomenclature(this.Db.NcatchInspectionTypes);
        }

        public List<NomenclatureDTO> GetPorts()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from port in this.Db.Nports
                                            join portGroup in this.Db.NportGroups on port.PortGroupId equals portGroup.Id
                                            orderby port.Name
                                            select new NomenclatureDTO
                                            {
                                                Value = port.Id,
                                                Code = port.Code,
                                                DisplayName = port.Name,
                                                Description = portGroup.Name,
                                                IsActive = port.ValidFrom <= now && port.ValidTo >= now
                                            }).ToList();
            return result;
        }

        public List<InspectionObservationToolNomenclatureDTO> GetObservationTools()
        {
            DateTime now = DateTime.Now;

            List<InspectionObservationToolNomenclatureDTO> result = (
                from item in this.Db.NobservationTools
                orderby item.Id
                select new InspectionObservationToolNomenclatureDTO
                {
                    Value = item.Id,
                    Code = item.Code,
                    DisplayName = item.Name,
                    OnBoard = Enum.Parse<ObservationToolOnBoardEnum>(item.IsOnBoardType),
                    IsActive = item.ValidFrom <= now && item.ValidTo >= now
                }).ToList();

            return result;
        }

        public List<InspectionVesselActivityNomenclatureDTO> GetVesselActivities()
        {
            DateTime now = DateTime.Now;

            List<InspectionVesselActivityNomenclatureDTO> result = (
                from item in this.Db.NvesselActivities
                orderby item.Id
                select new InspectionVesselActivityNomenclatureDTO
                {
                    Value = item.Id,
                    Code = item.Code,
                    DisplayName = item.Name,
                    HasAdditionalDescr = item.HasAdditionalDescr,
                    IsFishingActivity = item.IsFishingActivity,
                    IsActive = item.ValidFrom <= now && item.ValidTo >= now
                }).ToList();

            return result;
        }

        public List<CatchZoneNomenclatureDTO> GetCatchZones()
        {
            DateTime now = DateTime.Now;

            List<CatchZoneNomenclatureDTO> result = (from quadrant in this.Db.NcatchZones
                                                     select new CatchZoneNomenclatureDTO
                                                     {
                                                         Value = quadrant.Id,
                                                         Code = quadrant.Gfcmquadrant,
                                                         DisplayName = quadrant.Gfcmquadrant,
                                                         Zone = quadrant.ZoneNum,
                                                         IsActive = quadrant.ValidFrom <= now && quadrant.ValidTo > now
                                                     }).ToList();
            return result;
        }

        public List<NomenclatureDTO> GetUsageDocumentTypes()
        {
            List<NomenclatureDTO> result = this.GetCodeNomenclature(this.Db.NusageDocumentTypes);
            return result;
        }

        public IEnumerable<ShipNomenclatureDTO> GetShips()
        {
            DateTime now = DateTime.Now;

            Dictionary<int, ShipNomenclatureHelper> ships = GetActiveShipEvents();
            List<int> shipUIdsWithMultipleEvents = GetShipUIdsWithMultipleEvents();
            ILookup<int, ShipNomenclatureHelper> shipEvents = GetInactiveShipEvents(shipUIdsWithMultipleEvents);

            List<int> shipIds = new List<int>(100_000);

            foreach (KeyValuePair<int, ShipNomenclatureHelper> ship in ships)
            {
                List<ShipNomenclatureHelper> events = shipEvents[ship.Key].ToList();

                ship.Value.EventData = new Dictionary<int, ShipNomenclatureDTO>();

                foreach (ShipNomenclatureHelper eventData in events)
                {
                    ShipNomenclatureChangeFlags changes = ShipNomenclaturesDiffer(ship.Value, eventData);

                    if (changes != ShipNomenclatureChangeFlags.None)
                    {
                        ShipNomenclatureDTO data = MapShipEventDataFromChangesFlags(eventData, changes);
                        ship.Value.EventData.Add(eventData.Value, data);
                    }
                }

                if (ship.Value.ShipIds == null)
                {
                    ship.Value.ShipIds = new List<int>(50);
                }

                ship.Value.ShipIds.AddRange(events.Select(x => x.Value));
                shipIds.AddRange(ship.Value.ShipIds);
            }

            // commercial fishing permits
            ILookup<int, ShipPermitFlags> shipUIdsWithPermit = GetShipUIdsWithPermit(shipIds);

            // commercial fishing permit applications
            ILookup<int, ShipPermitFlags> shipUIdsWithPermitApplication = GetShipUIdsWithPermitApplication(shipIds);

            // commercial fishing pound net permits
            HashSet<int> shipUIdsWithPoundNetPermit = GetShipUIdsWithPoundNetPermit(shipIds);

            // commercial fishing pound net permit applications
            HashSet<int> shipUIdsWithPoundNetPermitApplication = GetShipUIdsWithPoundNetPermitApplication(shipIds);

            // Quotas
            HashSet<int> shipUIdsWithQuota = GetShipUIdsWithQuota(shipIds);

            // Result
            foreach (ShipNomenclatureHelper ship in ships.Values)
            {
                yield return new ShipNomenclatureDTO
                {
                    Value = ship.Value,
                    Name = ship.Name,

                    Cfr = ship.Cfr,
                    ExternalMark = ship.ExternalMark,
                    TotalLength = ship.TotalLength,

                    GrossTonnage = ship.GrossTonnage,
                    MainEnginePower = ship.MainEnginePower,

                    Flags = BuildShipNomenclatureFlags(ship,
                                                       shipUIdsWithPermit,
                                                       shipUIdsWithPermitApplication,
                                                       shipUIdsWithPoundNetPermit,
                                                       shipUIdsWithPoundNetPermitApplication,
                                                       shipUIdsWithQuota),

                    IsActive = ship.IsActive,

                    ShipIds = ship.ShipIds,
                    EventData = ship.EventData
                };
            }
        }

        public List<NomenclatureDTO> GetLogBookTypes()
        {
            List<NomenclatureDTO> result = this.GetCodeNomenclature(this.Db.NlogBookTypes);
            return result;
        }

        public List<NomenclatureDTO> GetLogBookStatuses()
        {
            List<NomenclatureDTO> result = this.GetCodeNomenclature(this.Db.NlogBookStatuses);
            return result;
        }

        public List<NomenclatureDTO> GetInspectionTypes()
        {
            List<NomenclatureDTO> result = this.GetCodeNomenclature(this.Db.NinspectionTypes);
            return result;
        }

        public List<TariffNomenclatureDTO> GetPaymentTariffs()
        {
            DateTime now = DateTime.Now;

            List<TariffNomenclatureDTO> result = (from tariff in this.Db.Ntariffs
                                                  select new TariffNomenclatureDTO
                                                  {
                                                      Value = tariff.Id,
                                                      Code = tariff.Code,
                                                      DisplayName = tariff.Name,
                                                      Description = tariff.Description,
                                                      Price = tariff.Price,
                                                      BasedOnPlea = tariff.BasedOnPlea,
                                                      IsCalculated = tariff.IsCalculated,
                                                      IsActive = tariff.ValidFrom <= now && tariff.ValidTo > now
                                                  }).ToList();

            return result;
        }

        public List<NomenclatureDTO> GetShipAssociations()
        {
            List<NomenclatureDTO> result = this.GetCodeNomenclature(this.Db.NshipAssociations);
            return result;
        }

        public List<NomenclatureDTO> GetCatchInspectionTypes()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = this.Db.NcatchInspectionTypes
                .Select(x => new NomenclatureDTO
                {
                    DisplayName = x.Name + " (" + x.Code + ")",
                    Value = x.Id,
                    Code = x.Code,
                    IsActive = x.ValidFrom < now && x.ValidTo > now
                })
                .OrderBy(x => x.DisplayName)
                .ToList();

            return result;
        }

        public List<NomenclatureDTO> GetTransportVehicleTypes()
        {
            List<NomenclatureDTO> result = this.GetCodeNomenclature(this.Db.NtransportVehicleTypes);
            return result;
        }

        public List<NomenclatureDTO> GetFishSex()
        {
            List<NomenclatureDTO> result = this.GetCodeNomenclature(this.Db.NfishSexes);
            return result;
        }

        public List<NomenclatureDTO> GetWaterBodyTypes()
        {
            List<NomenclatureDTO> result = this.GetCodeNomenclature(this.Db.NwaterBodyTypes);
            return result;
        }

        public List<NomenclatureDTO> GetCatchPresentations()
        {
            return this.GetCodeNomenclature(this.Db.NfishPresentations);
        }

        public List<NomenclatureDTO> GetMarkReasons()
        {
            List<NomenclatureDTO> result = this.GetCodeNomenclature(this.Db.NfishingGearCheckReasons);
            return result;
        }

        public List<NomenclatureDTO> GetRemarkReasons()
        {
            List<NomenclatureDTO> result = this.GetCodeNomenclature(this.Db.NfishingGearRecheckReasons);
            return result;
        }

        public List<NomenclatureDTO> GetPoundNets()
        {
            List<NomenclatureDTO> result = this.Db.PoundNetRegisters
                .Select(x => new NomenclatureDTO
                {
                    DisplayName = x.Name + " (" + x.PoundNetNum + ")",
                    Value = x.Id,
                    Code = x.PoundNetNum,
                    IsActive = x.IsActive
                })
                .OrderBy(x => x.DisplayName)
                .ToList();

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }

        private List<NomenclatureDTO> GetPaymentTypes(bool online)
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> types = (from type in this.Db.NpaymentTypes
                                           where type.IsOnline == online
                                           select new NomenclatureDTO
                                           {
                                               Value = type.Id,
                                               Code = type.Code,
                                               DisplayName = type.Name,
                                               IsActive = type.ValidFrom <= now && type.ValidTo > now
                                           }).ToList();
            return types;
        }

        private Dictionary<int, ShipNomenclatureHelper> GetActiveShipEvents()
        {
            DateTime now = DateTime.Now;

            var ships = (from ship in Db.ShipsRegister
                         join eventType in Db.NeventTypes on ship.EventTypeId equals eventType.Id
                         join fleet in Db.NfleetTypes on ship.FleetTypeId equals fleet.Id
                         where ship.RecordType == nameof(RecordTypesEnum.Register)
                            && ship.ValidFrom <= now
                            && ship.ValidTo > now
                         orderby ship.Name
                         select new
                         {
                             ship.ShipUid,
                             Event = new ShipNomenclatureHelper
                             {
                                 Value = ship.Id,
                                 ShipUid = ship.ShipUid,
                                 ShipIds = new List<int> { ship.Id },
                                 Name = ship.Name,
                                 Cfr = ship.Cfr,
                                 ExternalMark = ship.ExternalMark,
                                 TotalLength = ship.TotalLength,
                                 GrossTonnage = ship.GrossTonnage,
                                 MainEnginePower = ship.MainEnginePower,
                                 EventCode = eventType.Code,
                                 IsForbidden = ship.IsForbidden,
                                 IsThirdPartyShip = ship.IsThirdPartyShip,
                                 HasFishingCapacity = fleet.HasFishingCapacity,
                                 IsActive = true
                             }
                         }).ToDictionary(x => x.ShipUid, y => y.Event);

            return ships;
        }

        private List<int> GetShipUIdsWithMultipleEvents()
        {
            List<int> result = (from ship in Db.ShipsRegister
                                where ship.RecordType == nameof(RecordTypesEnum.Register)
                                group ship by ship.ShipUid into grouped
                                where grouped.Count() > 1
                                select grouped.Key).ToList();

            return result;
        }

        private ILookup<int, ShipNomenclatureHelper> GetInactiveShipEvents(List<int> shipUIds)
        {
            DateTime now = DateTime.Now;

            var shipEvents = (from ship in Db.ShipsRegister
                              join eventType in Db.NeventTypes on ship.EventTypeId equals eventType.Id
                              where shipUIds.Contains(ship.ShipUid)
                                    && ship.ValidTo < now
                              orderby ship.EventDate descending
                              select new
                              {
                                  ship.ShipUid,
                                  Event = new ShipNomenclatureHelper
                                  {
                                      Value = ship.Id,
                                      ShipUid = ship.ShipUid,
                                      Name = ship.Name,
                                      Cfr = ship.Cfr,
                                      ExternalMark = ship.ExternalMark,
                                      TotalLength = ship.TotalLength,
                                      GrossTonnage = ship.GrossTonnage,
                                      MainEnginePower = ship.MainEnginePower,
                                      EventCode = eventType.Code,
                                      IsActive = false
                                  }
                              }).ToLookup(x => x.ShipUid, y => y.Event);

            return shipEvents;
        }

        private ILookup<int, ShipPermitFlags> GetShipUIdsWithPermit(List<int> shipIds)
        {
            DateTime now = DateTime.Now;

            var result = (from ship in Db.ShipsRegister
                          join permit in Db.CommercialFishingPermitRegisters on ship.Id equals permit.ShipId
                          join waterType in Db.NwaterTypes on permit.WaterTypeId equals waterType.Id
                          join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                          join appl in Db.Applications on permit.ApplicationId equals appl.Id
                          join applStatus in Db.NapplicationStatuses on appl.ApplicationStatusId equals applStatus.Id
                          where shipIds.Contains(ship.Id)
                                && permit.RecordType == nameof(RecordTypesEnum.Register)
                                && permit.PermitValidFrom <= now
                                && (permit.PermitValidTo > now
                                    || permit.IsPermitUnlimited.Value)
                                && (permitType.Code == nameof(CommercialFishingTypesEnum.Permit) || permitType.Code == nameof(CommercialFishingTypesEnum.ThirdCountryPermit))
                                && applStatus.Code != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                                && permit.IsActive
                                && appl.IsActive
                                && !permit.IsSuspended
                          select new
                          {
                              ShipUId = ship.ShipUid,
                              PermitFlags = new ShipPermitFlags
                              {
                                  HasBlackSea = waterType.Code == nameof(WaterTypesEnum.BLACK_SEA),
                                  HasDanube = waterType.Code == nameof(WaterTypesEnum.DANUBE)
                              }
                          }).ToLookup(x => x.ShipUId, y => y.PermitFlags);

            return result;
        }

        private ILookup<int, ShipPermitFlags> GetShipUIdsWithPermitApplication(List<int> shipIds)
        {
            DateTime now = DateTime.Now;

            var result = (from ship in Db.ShipsRegister
                          join permit in Db.CommercialFishingPermitRegisters on ship.Id equals permit.ShipId
                          join waterType in Db.NwaterTypes on permit.WaterTypeId equals waterType.Id
                          join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                          join appl in Db.Applications on permit.ApplicationId equals appl.Id
                          join applStatus in Db.NapplicationStatuses on appl.ApplicationStatusId equals applStatus.Id
                          where shipIds.Contains(ship.Id)
                                && permit.RecordType == nameof(RecordTypesEnum.Application)
                                && (permitType.Code == nameof(CommercialFishingTypesEnum.Permit) || permitType.Code == nameof(CommercialFishingTypesEnum.ThirdCountryPermit))
                                && applStatus.Code != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                                && permit.IsActive
                                && appl.IsActive
                          select new
                          {
                              ShipUId = ship.ShipUid,
                              PermitFlags = new ShipPermitFlags
                              {
                                  HasBlackSea = waterType.Code == nameof(WaterTypesEnum.BLACK_SEA),
                                  HasDanube = waterType.Code == nameof(WaterTypesEnum.DANUBE)
                              }
                          }).ToLookup(x => x.ShipUId, y => y.PermitFlags);

            return result;
        }

        private HashSet<int> GetShipUIdsWithPoundNetPermit(List<int> shipIds)
        {
            DateTime now = DateTime.Now;

            HashSet<int> shipUIds = (from permit in Db.CommercialFishingPermitRegisters
                                     join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                                     join ship in Db.ShipsRegister on permit.ShipId equals ship.Id
                                     join appl in Db.Applications on permit.ApplicationId equals appl.Id
                                     join applStatus in Db.NapplicationStatuses on appl.ApplicationStatusId equals applStatus.Id
                                     where shipIds.Contains(permit.ShipId)
                                           && permit.RecordType == nameof(RecordTypesEnum.Register)
                                           && permit.PermitValidFrom <= now
                                           && (permit.PermitValidTo > now
                                               || permit.IsPermitUnlimited.Value)
                                           && permitType.Code == nameof(CommercialFishingTypesEnum.PoundNetPermit)
                                           && applStatus.Code != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                                           && permit.IsActive
                                           && appl.IsActive
                                           && !permit.IsSuspended
                                     select ship.ShipUid).ToHashSet();

            HashSet<int> suspendedShipUIds = (from permit in Db.CommercialFishingPermitRegisters
                                              join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                                              join ship in Db.ShipsRegister on permit.ShipId equals ship.Id
                                              join permitSuspension in Db.CommercialFishingPermitSuspensionChangeHistories on permit.Id equals permitSuspension.PermitId
                                              where shipUIds.Contains(ship.ShipUid)
                                                    && permitSuspension.SuspensionValidFrom <= now
                                                    && permitSuspension.SuspensionValidTo > now
                                                    && permitType.Code == nameof(CommercialFishingTypesEnum.PoundNetPermit)
                                              select permit.ShipId).ToHashSet();

            shipUIds = shipUIds.Except(suspendedShipUIds).ToHashSet();

            return shipUIds;
        }

        private HashSet<int> GetShipUIdsWithPoundNetPermitApplication(List<int> shipIds)
        {
            HashSet<int> shipUIds = (from permit in Db.CommercialFishingPermitRegisters
                                     join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                                     join ship in Db.ShipsRegister on permit.ShipId equals ship.Id
                                     join appl in Db.Applications on permit.ApplicationId equals appl.Id
                                     join applStatus in Db.NapplicationStatuses on appl.ApplicationStatusId equals applStatus.Id
                                     where shipIds.Contains(permit.ShipId)
                                           && permit.RecordType == nameof(RecordTypesEnum.Application)
                                           && permitType.Code == nameof(CommercialFishingTypesEnum.PoundNetPermit)
                                           && applStatus.Code != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                                           && permit.IsActive
                                           && appl.IsActive
                                           && !permit.IsSuspended
                                     select ship.ShipUid).ToHashSet();

            return shipUIds;
        }

        private HashSet<int> GetShipUIdsWithQuota(List<int> shipIds)
        {
            DateTime now = DateTime.Now;

            HashSet<int> result = (from shipQuota in Db.ShipCatchQuotas
                                   join ship in Db.ShipsRegister on shipQuota.ShipId equals ship.Id
                                   join quota in Db.CatchQuotas on shipQuota.CatchQuotaId equals quota.Id
                                   where quota.PeriodStart <= now
                                      && quota.PeriodEnd > now
                                      && shipIds.Contains(shipQuota.ShipId)
                                   select ship.ShipUid).ToHashSet();

            return result;
        }

        private static ShipNomenclatureChangeFlags ShipNomenclaturesDiffer(ShipNomenclatureHelper lhs, ShipNomenclatureHelper rhs)
        {
            ShipNomenclatureChangeFlags result = ShipNomenclatureChangeFlags.None;

            if (lhs.Name != rhs.Name)
            {
                result |= ShipNomenclatureChangeFlags.Name;
            }

            if (lhs.Cfr != rhs.Cfr)
            {
                result |= ShipNomenclatureChangeFlags.Cfr;
            }

            if (lhs.ExternalMark != rhs.ExternalMark)
            {
                result |= ShipNomenclatureChangeFlags.ExternalMark;
            }

            if (lhs.TotalLength != rhs.TotalLength)
            {
                result |= ShipNomenclatureChangeFlags.TotalLength;
            }

            if (lhs.GrossTonnage != rhs.GrossTonnage)
            {
                result |= ShipNomenclatureChangeFlags.GrossTonnage;
            }

            if (lhs.MainEnginePower != rhs.MainEnginePower)
            {
                result |= ShipNomenclatureChangeFlags.MainEnginePower;
            }

            return result;
        }

        private static ShipNomenclatureFlags BuildShipNomenclatureFlags(ShipNomenclatureHelper ship,
                                                                        ILookup<int, ShipPermitFlags> shipUIdsWithPermit,
                                                                        ILookup<int, ShipPermitFlags> shipUIdsWithPermitApplication,
                                                                        HashSet<int> shipUIdsWithPoundNetPermit,
                                                                        HashSet<int> shipUIdsWithPoundNetPermitApplication,
                                                                        HashSet<int> shipUIdsWithQuota)
        {
            string[] deregEvents = new string[]
            {
                nameof(ShipEventTypeEnum.EXP),
                nameof(ShipEventTypeEnum.DES),
                nameof(ShipEventTypeEnum.RET)
            };

            ShipNomenclatureFlags flags = ShipNomenclatureFlags.None;

            if (ship.IsThirdPartyShip)
            {
                flags |= ShipNomenclatureFlags.ThirdPartyShip;
            }

            if (ship.IsForbidden)
            {
                flags |= ShipNomenclatureFlags.Forbidden;
            }

            if (deregEvents.Contains(ship.EventCode))
            {
                flags |= ShipNomenclatureFlags.DestOrDereg;
            }

            if (ship.HasFishingCapacity)
            {
                flags |= ShipNomenclatureFlags.FishingCapacity;
            }

            if (shipUIdsWithPermit[ship.ShipUid].Any(x => x.HasBlackSea))
            {
                flags |= ShipNomenclatureFlags.BlackSeaPermit;
            }

            if (shipUIdsWithPermit[ship.ShipUid].Any(x => x.HasDanube))
            {
                flags |= ShipNomenclatureFlags.DanubePermit;
            }

            if (shipUIdsWithPermitApplication[ship.ShipUid].Any(x => x.HasBlackSea))
            {
                flags |= ShipNomenclatureFlags.BlackSeaPermitAppl;
            }

            if (shipUIdsWithPermitApplication[ship.ShipUid].Any(x => x.HasDanube))
            {
                flags |= ShipNomenclatureFlags.DanubePermitAppl;
            }

            if (shipUIdsWithPoundNetPermit.Contains(ship.ShipUid))
            {
                flags |= ShipNomenclatureFlags.PoundNetPermit;
            }

            if (shipUIdsWithPoundNetPermitApplication.Contains(ship.ShipUid))
            {
                flags |= ShipNomenclatureFlags.PoundNetPermitAppl;
            }

            if (shipUIdsWithQuota.Contains(ship.ShipUid))
            {
                flags |= ShipNomenclatureFlags.ActiveFishQuota;
            }

            return flags;
        }

        private static ShipNomenclatureDTO MapShipEventDataFromChangesFlags(ShipNomenclatureDTO eventData, ShipNomenclatureChangeFlags flags)
        {
            ShipNomenclatureDTO result = new ShipNomenclatureDTO
            {
                Value = eventData.Value,
                ChangeFlags = flags,
                IsActive = eventData.IsActive
            };

            if (flags != ShipNomenclatureChangeFlags.None)
            {
                if ((flags & ShipNomenclatureChangeFlags.Name) != 0)
                {
                    result.Name = eventData.Name;
                }

                if ((flags & ShipNomenclatureChangeFlags.Cfr) != 0)
                {
                    result.Cfr = eventData.Cfr;
                }

                if ((flags & ShipNomenclatureChangeFlags.ExternalMark) != 0)
                {
                    result.ExternalMark = eventData.ExternalMark;
                }

                if ((flags & ShipNomenclatureChangeFlags.TotalLength) != 0)
                {
                    result.TotalLength = eventData.TotalLength;
                }

                if ((flags & ShipNomenclatureChangeFlags.GrossTonnage) != 0)
                {
                    result.GrossTonnage = eventData.GrossTonnage;
                }

                if ((flags & ShipNomenclatureChangeFlags.MainEnginePower) != 0)
                {
                    result.MainEnginePower = eventData.MainEnginePower;
                }
            }

            return result;
        }
    }

    internal class ShipPermitFlags
    {
        public bool HasBlackSea { get; set; }

        public bool HasDanube { get; set; }
    }

    internal class ShipNomenclatureHelper : ShipNomenclatureDTO
    {
        public int ShipUid { get; set; }

        public bool IsThirdPartyShip { get; set; }

        public bool IsForbidden { get; set; }

        public bool HasFishingCapacity { get; set; }

        public string EventCode { get; set; }
    }
}
