using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Excel.Tools.Interfaces;
using IARA.Excel.Tools.Models;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Infrastructure.Helpers;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.Applications;
using IARA.Interfaces.FluxIntegrations.Ships;
using IARA.Interfaces.FSM;
using IARA.Interfaces.Legals;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class ShipsRegisterService : Service, IShipsRegisterService
    {
        private readonly ILegalService legalService;
        private readonly IPersonService personService;
        private readonly IApplicationService applicationService;
        private readonly ICancellationService cancellationService;
        private readonly IChangeOfCircumstancesService changeOfCircumstancesService;
        private readonly IFishingCapacityService fishingCapacityService;
        private readonly IDeliveryService deliveryService;
        private readonly IApplicationStateMachine stateMachine;
        private readonly IRegixApplicationInterfaceService regixApplicationService;
        private readonly IVesselsDomainService vesselDomainService;
        private readonly IVesselToFluxVesselReportMapper vesselToFluxVesselReportMapper;
        private readonly IExcelExporterService excelExporterService;

        public ShipsRegisterService(IARADbContext db,
                                    ILegalService legalService,
                                    IPersonService personService,
                                    IApplicationService applicationService,
                                    ICancellationService cancellationService,
                                    IChangeOfCircumstancesService changeOfCircumstancesService,
                                    IFishingCapacityService fishingCapacityService,
                                    IDeliveryService deliveryService,
                                    IApplicationStateMachine stateMachine,
                                    IRegixApplicationInterfaceService regixApplicationService,
                                    IVesselsDomainService vesselDomainService,
                                    IVesselToFluxVesselReportMapper vesselToFluxVesselReportMapper,
                                    IExcelExporterService excelExporterService)
            : base(db)
        {
            this.legalService = legalService;
            this.personService = personService;
            this.applicationService = applicationService;
            this.cancellationService = cancellationService;
            this.changeOfCircumstancesService = changeOfCircumstancesService;
            this.fishingCapacityService = fishingCapacityService;
            this.deliveryService = deliveryService;
            this.stateMachine = stateMachine;
            this.regixApplicationService = regixApplicationService;
            this.vesselDomainService = vesselDomainService;
            this.vesselToFluxVesselReportMapper = vesselToFluxVesselReportMapper;
            this.excelExporterService = excelExporterService;
        }

        public IQueryable<ShipRegisterDTO> GetAllShips(ShipsRegisterFilters filters)
        {
            IQueryable<ShipRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllShips();
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredShips(filters.FreeTextSearch, filters.TerritoryUnitId)
                    : GetParametersFilteredShips(filters);
            }

            return result;
        }

        public List<NomenclatureDTO> GetShipCatchQuotaNomenclatures(int shipUId)
        {
            List<int> shipIds = (from ship in Db.ShipsRegister
                                 where ship.ShipUid == shipUId
                                 select ship.Id).ToList();

            List<NomenclatureDTO> result = (from shipQuota in Db.ShipCatchQuotas
                                            join quota in Db.CatchQuotas on shipQuota.CatchQuotaId equals quota.Id
                                            join fish in Db.Nfishes on quota.FishId equals fish.Id
                                            where shipIds.Contains(shipQuota.ShipId)
                                                && shipQuota.IsActive
                                                && quota.IsActive
                                            orderby quota.PeriodStart descending
                                            select new NomenclatureDTO
                                            {
                                                Value = shipQuota.Id,
                                                DisplayName = $"{quota.PeriodStart.Year} — {fish.Name}",
                                                IsActive = true
                                            }).ToList();

            return result;
        }

        public ShipRegisterYearlyQuotaDTO GetShipYearlyQuota(int shipCatchQuotaId)
        {
            var data = (from shipCatchQuota in Db.ShipCatchQuotas
                        join catchQuota in Db.CatchQuotas on shipCatchQuota.CatchQuotaId equals catchQuota.Id
                        join ship in Db.ShipsRegister on shipCatchQuota.ShipId equals ship.Id
                        where shipCatchQuota.Id == shipCatchQuotaId
                        select new
                        {
                            shipCatchQuota.ShipQuotaSize,
                            ship.ShipUid,
                            catchQuota.FishId
                        }).First();

            List<int> shipIds = (from ship in Db.ShipsRegister
                                 where ship.ShipUid == data.ShipUid
                                 select ship.Id).ToList();

            ShipRegisterYearlyQuotaDTO result = new ShipRegisterYearlyQuotaDTO
            {
                CatchQuotaId = shipCatchQuotaId,
                QuotaKg = data.ShipQuotaSize
            };

            result.QuotaHistory = (from hist in Db.ShipCatchQuotasHists
                                   where hist.ShipCatchQuotaId == result.CatchQuotaId
                                   orderby hist.ValidFrom descending
                                   select new ShipRegisterQuotaHistoryDTO
                                   {
                                       DateOfChange = hist.ValidFrom,
                                       ShipQuotaSize = hist.ShipQuotaSize,
                                       ShipQuotaIncrement = hist.ShipQuotaIncrement,
                                       IncrementReason = hist.IncrementReason
                                   }).ToList();

            result.CatchHistory = (from logbook in Db.LogBooks
                                   join logbookType in Db.NlogBookTypes on logbook.LogBookTypeId equals logbookType.Id
                                   join shipLogBookPage in Db.ShipLogBookPages on logbook.Id equals shipLogBookPage.LogBookId
                                   join catchRecord in Db.CatchRecords on shipLogBookPage.Id equals catchRecord.LogBookPageId
                                   join catchRecordFish in Db.CatchRecordFish on catchRecord.Id equals catchRecordFish.CatchRecordId
                                   join fish in Db.Nfishes on catchRecordFish.FishId equals fish.Id
                                   join catchZone in Db.NcatchZones on catchRecordFish.CatchZoneId equals catchZone.Id
                                   where logbookType.Code == nameof(LogBookTypesEnum.Ship)
                                        && fish.Id == data.FishId
                                        && shipIds.Contains(logbook.ShipId.Value)
                                        && catchRecordFish.IsActive
                                        && catchRecord.IsActive
                                        && shipLogBookPage.IsActive
                                        && logbook.IsActive
                                   orderby catchRecord.GearEntryTime descending
                                   select new ShipRegisterCatchHistoryDTO
                                   {
                                       DateOfCatch = catchRecord.GearExitTime.HasValue ? catchRecord.GearExitTime.Value.Date : catchRecord.GearEntryTime.Date,
                                       QuantityKg = catchRecordFish.Quantity,
                                       PlaceOfCatch = catchZone.Gfcmquadrant,
                                       LogbookPage = shipLogBookPage.PageNum
                                   }).ToList();

            result.TotalCatch = result.CatchHistory.Sum(x => x.QuantityKg);
            result.LeftoverQuotaKg = result.QuotaKg - result.TotalCatch;

            return result;
        }

        public IQueryable<ShipRegisterOriginDeclarationFishDTO> GetShipOriginDeclarations(ShipRegisterOriginDeclarationsFilters filters)
        {
            List<int> shipIds = (from ship in Db.ShipsRegister
                                 where ship.ShipUid == filters.ShipUID.Value
                                 select ship.Id).ToList();

            var result = from logBook in Db.LogBooks
                         join shipPage in Db.ShipLogBookPages on logBook.Id equals shipPage.LogBookId
                         join fishingGearRegister in Db.FishingGearRegisters on shipPage.FishingGearRegisterId equals fishingGearRegister.Id into fgr
                         from fishingGearRegister in fgr.DefaultIfEmpty()
                         join fishingGearType in Db.NfishingGears on fishingGearRegister.FishingGearTypeId equals fishingGearType.Id into fgt
                         from fishingGearType in fgt.DefaultIfEmpty()
                         join originDeclaration in Db.OriginDeclarations on shipPage.Id equals originDeclaration.LogBookPageId
                         join originDeclarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals originDeclarationFish.OriginDeclarationId
                         join catchRecordFish in Db.CatchRecordFish on originDeclarationFish.CatchRecordFishId equals catchRecordFish.Id
                         join fishType in Db.Nfishes on catchRecordFish.FishId equals fishType.Id
                         join catchQuandrant in Db.NcatchZones on catchRecordFish.CatchZoneId equals catchQuandrant.Id
                         where shipIds.Contains(logBook.ShipId.Value)
                         orderby shipPage.PageNum.Length descending, shipPage.PageNum descending
                         select new ShipRegisterOriginDeclarationFishDTO
                         {
                             Id = originDeclarationFish.Id,
                             LogBookNum = logBook.LogNum,
                             LogBookPageNum = shipPage.PageNum,
                             Date = originDeclarationFish.UnloadDateTime ?? originDeclarationFish.TransboardDateTime,
                             FishingGearTypeName = fishingGearType != null ? fishingGearType.Name : null,
                             CatchQuadrantId = catchRecordFish.CatchZoneId,
                             CatchRecordFishId = catchRecordFish.Id,
                             FishId = catchRecordFish.FishId,
                             FishName = fishType.Name,
                             CatchQuadrant = catchQuandrant.Gfcmquadrant,
                             CatchZone = catchQuandrant.ZoneNum.ToString(),
                             CatchFishPresentationId = originDeclarationFish.CatchFishPresentationId,
                             CatchFishStateId = originDeclarationFish.CatchFishFreshnessId,
                             IsProcessedOnBoard = originDeclarationFish.IsProcessedOnBoard,
                             QuantityKg = originDeclarationFish.Quantity,
                             UnloadedProcessedQuantityKg = originDeclarationFish.UnloadedProcessedQuantity,
                             UnloadTypeId = originDeclarationFish.UnloadTypeId,
                             UnloadPortId = originDeclarationFish.UnloadPortId,
                             UnloadDateTime = originDeclarationFish.UnloadDateTime,
                             TransboardShipId = originDeclarationFish.TransboardShipId,
                             TransboardTargetPortId = originDeclarationFish.TransboardTargetPortId,
                             TransboradDateTime = originDeclarationFish.TransboardDateTime,
                             Comments = originDeclarationFish.Comments,
                             IsActive = originDeclarationFish.IsActive,
                             IsValid = true
                         };

            return result;
        }

        public ShipRegisterEditDTO GetShip(int id)
        {
            ShipRegister dbShip = (from ship in Db.ShipsRegister
                                   where ship.Id == id
                                        && ship.RecordType == nameof(RecordTypesEnum.Register)
                                   select ship).First();

            ShipRegisterEditDTO result = MapDbShipToDTO(dbShip);
            return result;
        }

        public ShipRegisterEditDTO GetRegisterByApplicationId(int applicationId)
        {
            // взима се първия Register запис за съответния кораб, пряко породен от заявлението
            int id = (from ship in Db.ShipsRegister
                      where ship.ApplicationId == applicationId
                            && ship.RecordType == nameof(RecordTypesEnum.Register)
                      orderby ship.EventDate ascending
                      select ship.Id).First();

            return GetShip(id);
        }

        public ShipRegisterEditDTO GetRegisterByChangeOfCircumstancesApplicationId(int applicationId)
        {
            // за ApplicationId на записа в ShipsRegister е сложено заявлението за промяна в обстоятелствата
            // понеже може да има няколко събития за едно заявление, трябва да вземем последното
            int id = (from ship in Db.ShipsRegister
                      where ship.ApplicationId == applicationId
                        && ship.RecordType == nameof(RecordTypesEnum.Register)
                      orderby ship.EventDate descending
                      select ship.Id).First();

            return GetShip(id);
        }

        public ShipRegisterEditDTO GetRegisterByChangeCapacityApplicationId(int applicationId)
        {
            // за ApplicationId на записа в ShipsRegister е сложено заявлението за увеличаване/намаляване на капацитет
            // понеже може да има няколко събития за едно заявление, трябва да вземем последното
            int id = (from ship in Db.ShipsRegister
                      where ship.ApplicationId == applicationId
                        && ship.RecordType == nameof(RecordTypesEnum.Register)
                      orderby ship.EventDate descending
                      select ship.Id).First();

            return GetShip(id);
        }

        public List<ShipRegisterEventDTO> GetShipEventHistory(int shipId)
        {
            int uid = (from ship in Db.ShipsRegister
                       where ship.Id == shipId
                       select ship.ShipUid).First();

            List<ShipRegisterEventDTO> events = (from ship in Db.ShipsRegister
                                                 join eventType in Db.NeventTypes on ship.EventTypeId equals eventType.Id
                                                 where ship.ShipUid == uid
                                                    && ship.RecordType == nameof(RecordTypesEnum.Register)
                                                 orderby ship.EventDate descending
                                                 select new ShipRegisterEventDTO
                                                 {
                                                     ShipId = ship.Id,
                                                     Type = eventType.Code,
                                                     Date = ship.EventDate,
                                                     UsrRsr = ship.HasFishingPermit
                                                 }).ToList();

            for (int i = events.Count; i > 0; --i)
            {
                events[events.Count - i].No = i;
            }

            return events;
        }

        public Stream DownloadShipRegisterExcel(ExcelExporterRequestModel<ShipsRegisterFilters> request)
        {
            ExcelExporterData<ShipRegisterDTO> data = new ExcelExporterData<ShipRegisterDTO>
            {
                PrimaryKey = nameof(ShipRegisterDTO.Id),
                Query = GetAllShips(request.Filters),
                HeaderNames = request.HeaderNames
            };

            return excelExporterService.BuildExcelFile(request, data);
        }

        public int AddShip(ShipRegisterEditDTO ship)
        {
            DateTime now = DateTime.Now;

            ShipRegister entry;

            using (TransactionScope scope = new TransactionScope())
            {
                ShipRegister registerApplication = null;

                if (!ship.IsThirdPartyShip.Value)
                {
                    registerApplication = (from sh in Db.ShipsRegister
                                           where sh.ApplicationId == ship.ApplicationId
                                                && sh.RecordType == nameof(RecordTypesEnum.Application)
                                           select sh).First();

                    registerApplication.ValidTo = now;
                }

                entry = new ShipRegister
                {
                    ApplicationId = ship.ApplicationId,
                    RegisterApplicationId = registerApplication?.Id,
                    RecordType = nameof(RecordTypesEnum.Register),
                    IsThirdPartyShip = ship.IsThirdPartyShip.Value,
                    Cfr = ship.CFR,
                    Name = ship.Name,
                    ExternalMark = ship.ExternalMark,
                    RegistrationNum = ship.RegistrationNumber,
                    RegistrationDate = ship.RegistrationDate.Value,
                    FleetTypeId = ship.FleetTypeId.Value,
                    FleetSegmentId = ship.FleetSegmentId.Value,
                    IrcscallSign = ship.IRCSCallSign,
                    Mmsi = ship.MMSI,
                    Uvi = ship.UVI,
                    FlagCountryId = ship.CountryFlagId.Value,
                    HasAis = ship.HasAIS.Value,
                    HasErs = ship.HasERS.Value,
                    HasVms = ship.HasVMS.Value,
                    VesselTypeId = ship.VesselTypeId,
                    RegLicenceNum = ship.RegLicenceNum,
                    RegLicenseDate = ship.RegLicenceDate,
                    RegLicensePublisher = ship.RegLicencePublisher,
                    RegLicensePublishVolume = ship.RegLicencePublishVolume,
                    RegLicensePublishPage = ship.RegLicencePublishPage,
                    RegLicensePublishNum = ship.RegLicencePublishNum,
                    ExploitationStartDate = ship.ExploitationStartDate,
                    BuildYear = ship.BuildYear.Value,
                    BuildPlace = ship.BuildPlace,
                    AdminDecisionNum = ship.AdminDecisionNum,
                    AdminDecisionDate = ship.AdminDecisionDate,
                    PublicAidTypeId = ship.PublicAidTypeId.Value,
                    PortId = ship.PortId.Value,
                    SailAreaId = ship.SailAreaId,
                    TotalLength = ship.TotalLength.Value,
                    TotalWidth = ship.TotalWidth.Value,
                    GrossTonnage = ship.GrossTonnage.Value,
                    NetTonnage = ship.NetTonnage,
                    OtherTonnage = ship.OtherTonnage,
                    BoardHeight = ship.BoardHeight.Value,
                    ShipDraught = ship.ShipDraught.Value,
                    LengthBetweenPerpendiculars = ship.LengthBetweenPerpendiculars,
                    MainEnginePower = ship.MainEnginePower.Value,
                    AuxiliaryEnginePower = ship.AuxiliaryEnginePower,
                    MainEngineNum = ship.MainEngineNum,
                    MainEngineModel = ship.MainEngineModel,
                    MainFishingGearId = ship.MainFishingGearId.Value,
                    AdditionalFishingGearId = ship.AdditionalFishingGearId,
                    HullMaterialId = ship.HullMaterialId.Value,
                    FuelTypeId = ship.FuelTypeId.Value,
                    TotalPassengerCapacity = ship.TotalPassengerCapacity.Value,
                    CrewCount = ship.CrewCount.Value,
                    HasFoodLawLicense = ship.HasFoodLawLicence.Value,
                    HasControlCard = ship.HasControlCard.Value,
                    HasValidityCertificate = ship.HasValidityCertificate.Value,
                    ShipAssociationId = ship.ShipAssociationId,
                    ImportCountryId = ship.ImportCountryId,
                    Comments = ship.Comments,
                    ValidFrom = DateTime.Now,
                    ValidTo = DefaultConstants.MAX_VALID_DATE
                };

                // ако корабът вече е бил вписван в регистъра, закачаме му старото ShipUId
                ShipRegister oldShip = (from sh in Db.ShipsRegister
                                        where sh.Cfr == ship.CFR
                                             && sh.RecordType == nameof(RecordTypesEnum.Register)
                                             && sh.ValidFrom <= now
                                             && sh.ValidTo > now
                                        select sh).FirstOrDefault();

                if (oldShip != null)
                {
                    oldShip.ValidTo = now;
                    entry.ShipUid = oldShip.ShipUid;
                }

                // EventTypeId се прехвърля от заявлението
                if (ship.EventType.HasValue && ship.EventDate.HasValue)
                {
                    entry.EventTypeId = oldShip != null
                        ? ship.EventType.Value == ShipEventTypeEnum.IMP
                            ? GetEventTypeId(ShipEventTypeEnum.IMP)
                            : GetEventTypeId(ShipEventTypeEnum.CHA)
                        : GetEventTypeId(ship.EventType.Value);
                    entry.EventDate = now;
                }
                else
                {
                    // добавяне на кораб от трети страни не минава през заявление
                    // съответно трябва EventTypeId да се изчисли тук
                    entry.EventTypeId = CalculateNewShipEventTypeId(ship.BuildYear.Value, ship.ImportCountryId, oldShip != null);
                    entry.EventDate = now;
                }

                if (ship.SailAreaId.HasValue)
                {
                    entry.SailAreaId = ship.SailAreaId;
                }
                else if (!string.IsNullOrEmpty(ship.SailAreaName))
                {
                    NsailArea sailArea = new NsailArea { Name = ship.SailAreaName };
                    entry.SailArea = sailArea;
                }

                if (ship.HasControlCard.Value)
                {
                    entry.ControlCardNum = ship.ControlCardNum;
                    entry.ControlCardDate = ship.ControlCardDate.Value;
                }

                if (ship.HasValidityCertificate.Value)
                {
                    entry.ControlCardValidityCertificateNum = ship.ControlCardValidityCertificateNum;
                    entry.ControlCardValidityCertificateDate = ship.ControlCardValidityCertificateDate.Value;
                    entry.ControlCardDateOfLastAttestation = ship.ControlCardDateOfLastAttestation.Value;
                }

                if (ship.HasFoodLawLicence.Value)
                {
                    entry.FoodLawLicenseNum = ship.FoodLawLicenceNum;
                    entry.FoodLawLicenseDate = ship.FoodLawLicenceDate;
                }

                if (ship.Files != null)
                {
                    foreach (FileInfoDTO file in ship.Files)
                    {
                        Db.AddOrEditFile(entry, entry.ShipRegisterFiles, file);
                    }
                }

                Db.ShipsRegister.Add(entry);
                Db.SaveChanges();

                foreach (ShipOwnerDTO owner in ship.Owners)
                {
                    AddShipOwner(entry, owner);
                    Db.SaveChanges();
                }

                FleetTypeNomenclatureDTO fleet = GetFleetType(ship.FleetTypeId.Value);

                if (!ship.IsThirdPartyShip.Value)
                {
                    if (fleet.HasFishingCapacity)
                    {
                        CapacityChangeHistoryDTO changeHistory = fishingCapacityService.GetCapacityChangeHistory(entry.ApplicationId.Value, RecordTypesEnum.Application);

                        AcquiredFishingCapacityDTO acquiredCapacity = changeHistory.AcquiredFishingCapacityId.HasValue
                            ? fishingCapacityService.GetAcquiredFishingCapacity(changeHistory.AcquiredFishingCapacityId.Value)
                            : null;

                        FishingCapacityFreedActionsDTO actions = fishingCapacityService.GetCapacityFreeActionsFromChangeHistory(changeHistory);

                        fishingCapacityService.AddIncreaseCapacityChangeHistory(RecordTypesEnum.Register,
                                                                                entry.ApplicationId.Value,
                                                                                entry.Id,
                                                                                entry.GrossTonnage,
                                                                                entry.MainEnginePower,
                                                                                acquiredCapacity,
                                                                                actions);
                    }
                    else
                    {
                        fishingCapacityService.EditIncreaseCapacityChangeHistory(entry.ApplicationId.Value,
                                                                                 entry.GrossTonnage,
                                                                                 entry.MainEnginePower,
                                                                                 null,
                                                                                 null,
                                                                                 isActive: false);
                    }
                }

                Db.SaveChanges();

                if (!ship.IsThirdPartyShip.Value && fleet.HasFishingCapacity)
                {
                    FLUXReportVesselInformationType vesselInformation = vesselToFluxVesselReportMapper.MapVesselToFluxSubVcd(ship, ReportPurposeCodes.Original);
                    vesselDomainService.ReportVesselChange(vesselInformation);
                }

                if (!ship.IsThirdPartyShip.Value)
                {
                    stateMachine.Act(ship.ApplicationId.Value);
                }

                scope.Complete();
            }

            return entry.Id;
        }

        public void EditShip(ShipRegisterEditDTO ship, int? applicationId = null, int? uid = null)
        {
            using TransactionScope scope = new TransactionScope();

            DateTime now = DateTime.Now;

            ShipRegister dbShip;

            if (uid.HasValue)
            {
                dbShip = (from sh in Db.ShipsRegister
                          where sh.ShipUid == uid.Value
                            && sh.ValidFrom <= now
                            && sh.ValidTo > now
                          select sh).First();
            }
            else
            {
                dbShip = (from sh in Db.ShipsRegister
                          where sh.Id == ship.Id
                          select sh).First();
            }

            ShipRegister entry = CopyShip(dbShip);

            dbShip.ValidTo = now;
            entry.ValidFrom = now;
            entry.EventTypeId = GetEventTypeId(ship.EventType.Value);
            entry.EventDate = now;

            if (applicationId != null)
            {
                entry.ApplicationId = applicationId;
            }

            Db.ShipsRegister.Add(entry);
            Db.SaveChanges();

            switch (ship.EventType.Value)
            {
                case ShipEventTypeEnum.EDIT:
                    EditShipEditEvent(entry, ship);
                    break;
                case ShipEventTypeEnum.CHA:
                    EditShipChaEvent(entry, ship);
                    break;
                case ShipEventTypeEnum.MOD:
                    EditShipModEvent(entry, ship);
                    break;
                case ShipEventTypeEnum.EXP:
                    EditShipExpEvent(entry, ship);
                    break;
                case ShipEventTypeEnum.RET:
                    EditShipRetEvent(entry, ship);
                    break;
                case ShipEventTypeEnum.DES:
                    EditShipDesEvent(entry, ship);
                    break;
                default:
                    throw new ArgumentException("Cannot execute ship modification with this event.");
            }

            if (ship.EventType.Value == ShipEventTypeEnum.MOD)
            {
                foreach (ShipOwnerDTO owner in ship.Owners)
                {
                    AddShipOwner(entry, owner);
                    Db.SaveChanges();
                }
            }
            else
            {
                CopyShipOwners(dbShip, entry);
            }

            if (ship.Files != null)
            {
                foreach (FileInfoDTO file in ship.Files)
                {
                    Db.AddOrEditFile(entry, entry.ShipRegisterFiles, file);
                }
            }

            TransferShipQuotas(dbShip.Id, entry);

            Db.SaveChanges();

            if (ship.EventType.Value != ShipEventTypeEnum.EDIT && !ship.IsThirdPartyShip.Value)
            {
                FleetTypeNomenclatureDTO fleet = GetFleetType(ship.FleetTypeId.Value);

                if (fleet.HasFishingCapacity)
                {
                    FLUXReportVesselInformationType vesselInformation = vesselToFluxVesselReportMapper.MapVesselToFluxSubVcd(ship, ReportPurposeCodes.Original);
                    vesselDomainService.ReportVesselChange(vesselInformation);
                }
            }

            scope.Complete();
        }

        public int EditShipRsr(int shipId, bool hasFishingPermit)
        {
            using TransactionScope scope = new TransactionScope();

            DateTime now = DateTime.Now;

            int shipUId = (from ship in Db.ShipsRegister
                           where ship.Id == shipId
                           select ship.ShipUid).First();

            ShipRegister dbShip = (from ship in Db.ShipsRegister
                                   where ship.RecordType == nameof(RecordTypesEnum.Register)
                                         && ship.ShipUid == shipUId
                                   orderby ship.EventDate descending
                                   select ship).First();

            ShipRegister entry = CopyShip(dbShip);

            dbShip.ValidTo = now;
            entry.ValidFrom = now;
            entry.EventTypeId = GetEventTypeId(ShipEventTypeEnum.MOD);
            entry.EventDate = now;
            entry.HasFishingPermit = hasFishingPermit;

            Db.ShipsRegister.Add(entry);
            Db.SaveChanges();

            CopyShipOwners(dbShip, entry);
            CopyShipFiles(dbShip, entry);

            TransferShipQuotas(dbShip.Id, entry);

            Db.SaveChanges();

            FleetTypeNomenclatureDTO fleet = GetFleetType(entry.FleetTypeId);

            if (fleet.HasFishingCapacity)
            {
                ShipRegisterEditDTO ship = GetShip(entry.Id);

                FLUXReportVesselInformationType vesselInformation = vesselToFluxVesselReportMapper.MapVesselToFluxSubVcd(ship, ReportPurposeCodes.Original);
                vesselDomainService.ReportVesselChange(vesselInformation);
            }

            scope.Complete();

            return entry.Id;
        }

        public SimpleAuditDTO GetShipOwnerSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.ShipOwners, id);
            return audit;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.ShipsRegister, id);
            return audit;
        }

        private IQueryable<ShipRegisterDTO> GetAllShips()
        {
            DateTime now = DateTime.Now;

            IQueryable<ShipRegisterDTO> result = from ship in Db.ShipsRegister
                                                 join eventType in Db.NeventTypes on ship.EventTypeId equals eventType.Id
                                                 where ship.RecordType == nameof(RecordTypesEnum.Register)
                                                       && ship.ValidFrom <= now
                                                       && ship.ValidTo > now
                                                 orderby ship.EventDate descending
                                                 select new ShipRegisterDTO
                                                 {
                                                     Id = ship.Id,
                                                     CFR = ship.Cfr,
                                                     Name = ship.Name,
                                                     Owners = string.Join(", ", from owner in Db.ShipOwners
                                                                                join person in Db.Persons on owner.OwnerPersonId equals person.Id into per
                                                                                from person in per.DefaultIfEmpty()
                                                                                join legal in Db.Legals on owner.OwnerLegalId equals legal.Id into leg
                                                                                from legal in leg.DefaultIfEmpty()
                                                                                where owner.ShipRegisterId == ship.Id
                                                                                select owner.OwnerIsPerson ? person.FirstName + " " + person.LastName : legal.Name),
                                                     ExternalMark = ship.ExternalMark,
                                                     LastChangeDate = ship.EventDate,
                                                     LastChangeStatus = eventType.Code + ": " + eventType.Name,
                                                     EventType = Enum.Parse<ShipEventTypeEnum>(eventType.Code),
                                                     IsThirdPartyShip = ship.IsThirdPartyShip
                                                 };
            return result;
        }

        private IQueryable<ShipRegisterDTO> GetParametersFilteredShips(ShipsRegisterFilters filters)
        {
            DateTime now = DateTime.Now;

            IQueryable<ShipRegister> query = from ship in Db.ShipsRegister
                                             where ship.RecordType == nameof(RecordTypesEnum.Register)
                                                 && ship.ValidFrom <= now
                                                 && ship.ValidTo > now
                                             select ship;

            if (filters.TerritoryUnitId.HasValue)
            {
                List<int> shipUids = (from ship in query
                                      join appl in Db.Applications on ship.ApplicationId equals appl.Id
                                      where appl.TerritoryUnitId == filters.TerritoryUnitId.Value
                                      select ship.ShipUid).ToList();

                query = from ship in query
                        where shipUids.Contains(ship.ShipUid)
                        select ship;
            }

            if (filters.EventTypeId.HasValue)
            {
                query = query.Where(x => x.EventTypeId == filters.EventTypeId.Value);
            }

            if (filters.EventDateFrom.HasValue)
            {
                query = query.Where(x => x.EventDate >= filters.EventDateFrom.Value);
            }

            if (filters.EventDateTo.HasValue)
            {
                query = query.Where(x => x.EventDate <= filters.EventDateTo.Value);
            }

            if (filters.CancellationReasonId.HasValue)
            {
                query = from ship in query
                        join cancellation in Db.CancellationDetails on ship.CancellationDetailsId equals cancellation.Id into c
                        from cancellation in c.DefaultIfEmpty()
                        where cancellation != null && cancellation.CancelReasonId == filters.CancellationReasonId.Value
                        select ship;
            }

            if (filters.IsCancelled.HasValue)
            {
                List<string> eventTypes = new List<string>
                {
                    nameof(ShipEventTypeEnum.DES), nameof(ShipEventTypeEnum.EXP), nameof(ShipEventTypeEnum.RET )
                };

                List<int> eventTypeIds = (from ev in Db.NeventTypes
                                          where eventTypes.Contains(ev.Code)
                                            && ev.ValidFrom <= now
                                            && ev.ValidTo > now
                                          select ev.Id).ToList();

                if (filters.IsCancelled.Value)
                {
                    query = from ship in query
                            where eventTypeIds.Contains(ship.EventTypeId)
                            select ship;
                }
                else
                {
                    query = from ship in query
                            where !eventTypeIds.Contains(ship.EventTypeId)
                            select ship;
                }
            }

            if (filters.IsThirdPartyShip.HasValue)
            {
                query = query.Where(x => x.IsThirdPartyShip == filters.IsThirdPartyShip.Value);
            }

            if (!string.IsNullOrEmpty(filters.ShipOwnerName))
            {
                HashSet<int> shipIds = (from owner in Db.ShipOwners
                                        join person in Db.Persons on owner.OwnerPersonId equals person.Id into p
                                        from person in p.DefaultIfEmpty()
                                        join legal in Db.Legals on owner.OwnerLegalId equals legal.Id into l
                                        from legal in l.DefaultIfEmpty()
                                        where owner.IsActive
                                            && ((owner.OwnerIsPerson && (person.FirstName + " " + person.LastName).ToLower().Contains(filters.ShipOwnerName.ToLower()))
                                                || (!owner.OwnerIsPerson && legal.Name.ToLower().Contains(filters.ShipOwnerName.ToLower())))
                                        select owner.ShipRegisterId).ToHashSet();

                query = query.Where(x => shipIds.Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(filters.ShipOwnerEgnLnc))
            {
                HashSet<int> shipIds = (from owner in Db.ShipOwners
                                        join person in Db.Persons on owner.OwnerPersonId equals person.Id into p
                                        from person in p.DefaultIfEmpty()
                                        join legal in Db.Legals on owner.OwnerLegalId equals legal.Id into l
                                        from legal in l.DefaultIfEmpty()
                                        where owner.IsActive
                                            && ((owner.OwnerIsPerson && person.EgnLnc == filters.ShipOwnerEgnLnc)
                                                || (!owner.OwnerIsPerson && legal.Eik.Contains(filters.ShipOwnerEgnLnc)))
                                        select owner.ShipRegisterId).ToHashSet();

                query = query.Where(x => shipIds.Contains(x.Id));
            }

            if (filters.FleetId.HasValue)
            {
                query = from ship in query
                        where ship.FleetTypeId == filters.FleetId.Value
                        select ship;
            }

            if (filters.VesselTypeId.HasValue)
            {
                query = query.Where(x => x.VesselTypeId == filters.VesselTypeId.Value);
            }

            if (filters.HasCommercialFishingLicence.HasValue)
            {
                query = from ship in query
                        join permit in Db.CommercialFishingPermitRegisters on ship.Id equals permit.ShipId
                        where permit.IsActive
                            && permit.RecordType == nameof(RecordTypesEnum.Register)
                            && permit.PermitValidFrom <= now
                            && (permit.PermitValidTo > now || permit.IsPermitUnlimited.Value)
                        select ship;
            }

            if (filters.HasVMS.HasValue)
            {
                query = query.Where(x => x.HasVms == filters.HasVMS.HasValue);
            }

            if (!string.IsNullOrEmpty(filters.CFR))
            {
                query = query.Where(x => x.Cfr.ToLower().Contains(filters.CFR.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.ExternalMark))
            {
                query = query.Where(x => x.ExternalMark.ToLower().Contains(filters.ExternalMark.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.IRCSCallSign))
            {
                query = query.Where(x => x.IrcscallSign.ToLower().Contains(filters.IRCSCallSign.ToLower()));
            }

            if (filters.PublicAidTypeId.HasValue)
            {
                query = query.Where(x => x.PublicAidTypeId == filters.PublicAidTypeId.Value);
            }

            if (filters.TotalLengthFrom.HasValue)
            {
                query = query.Where(x => x.TotalLength >= filters.TotalLengthFrom.Value);
            }

            if (filters.TotalLengthTo.HasValue)
            {
                query = query.Where(x => x.TotalLength <= filters.TotalLengthTo.Value);
            }

            if (filters.GrossTonnageFrom.HasValue)
            {
                query = query.Where(x => x.GrossTonnage >= filters.GrossTonnageFrom.Value);
            }

            if (filters.GrossTonnageTo.HasValue)
            {
                query = query.Where(x => x.GrossTonnage <= filters.GrossTonnageTo.Value);
            }

            if (filters.NetTonnageFrom.HasValue)
            {
                query = query.Where(x => x.NetTonnage >= filters.NetTonnageFrom.Value);
            }

            if (filters.NetTonnageTo.HasValue)
            {
                query = query.Where(x => x.NetTonnage <= filters.NetTonnageTo.Value);
            }

            if (filters.MainEnginePowerFrom.HasValue)
            {
                query = query.Where(x => x.MainEnginePower >= filters.MainEnginePowerFrom.Value);
            }

            if (filters.MainEnginePowerTo.HasValue)
            {
                query = query.Where(x => x.MainEnginePower <= filters.MainEnginePowerTo.Value);
            }

            if (filters.MainFishingGearId.HasValue)
            {
                query = query.Where(x => x.MainFishingGearId == filters.MainFishingGearId.Value);
            }

            if (filters.AdditionalFishingGearId.HasValue)
            {
                query = query.Where(x => x.AdditionalFishingGearId == filters.AdditionalFishingGearId.Value);
            }

            if (filters.FoodLawLicenceDateFrom.HasValue)
            {
                query = query.Where(x => x.HasFoodLawLicense && x.FoodLawLicenseDate >= filters.FoodLawLicenceDateFrom.Value);
            }

            if (filters.FoodLawLicenceDateTo.HasValue)
            {
                query = query.Where(x => x.HasFoodLawLicense && x.FoodLawLicenseDate <= filters.FoodLawLicenceDateTo.Value);
            }

            if (filters.ShipAssociationId.HasValue)
            {
                query = query.Where(x => x.ShipAssociationId == filters.ShipAssociationId.Value);
            }

            if (filters.AllowedForQuotaFishId.HasValue)
            {
                HashSet<int> shipIds = (from catchQuota in Db.CatchQuotas
                                        join shipCatchQuota in Db.ShipCatchQuotas on catchQuota.Id equals shipCatchQuota.CatchQuotaId
                                        where catchQuota.FishId == filters.AllowedForQuotaFishId.Value
                                            && shipCatchQuota.IsActive
                                            && catchQuota.IsActive
                                        select shipCatchQuota.ShipId).ToHashSet();

                query = from ship in query
                        where shipIds.Contains(ship.Id)
                        select ship;
            }

            if (filters.PersonId.HasValue)
            {
                HashSet<int> shipIds = (from owner in Db.ShipOwners
                                        where owner.IsActive
                                            && owner.OwnerIsPerson
                                            && owner.OwnerPersonId == filters.PersonId.Value
                                        select owner.ShipRegisterId).ToHashSet();

                query = query.Where(x => shipIds.Contains(x.Id));
            }

            if (filters.LegalId.HasValue)
            {
                HashSet<int> shipIds = (from owner in Db.ShipOwners
                                        where owner.IsActive
                                            && !owner.OwnerIsPerson
                                            && owner.OwnerLegalId == filters.LegalId.Value
                                        select owner.ShipRegisterId).ToHashSet();

                query = query.Where(x => shipIds.Contains(x.Id));
            }

            IQueryable<ShipRegisterDTO> result = from ship in query
                                                 join eventType in Db.NeventTypes on ship.EventTypeId equals eventType.Id
                                                 orderby ship.EventDate descending
                                                 select new ShipRegisterDTO
                                                 {
                                                     Id = ship.Id,
                                                     CFR = ship.Cfr,
                                                     Name = ship.Name,
                                                     Owners = string.Join(", ", from owner in Db.ShipOwners
                                                                                join person in Db.Persons on owner.OwnerPersonId equals person.Id into per
                                                                                from person in per.DefaultIfEmpty()
                                                                                join legal in Db.Legals on owner.OwnerLegalId equals legal.Id into leg
                                                                                from legal in leg.DefaultIfEmpty()
                                                                                where owner.ShipRegisterId == ship.Id
                                                                                select owner.OwnerIsPerson ? person.FirstName + " " + person.LastName : legal.Name),
                                                     ExternalMark = ship.ExternalMark,
                                                     LastChangeDate = ship.EventDate,
                                                     LastChangeStatus = eventType.Code + ": " + eventType.Name,
                                                     EventType = Enum.Parse<ShipEventTypeEnum>(eventType.Code),
                                                     IsThirdPartyShip = ship.IsThirdPartyShip
                                                 };

            return result;
        }

        private IQueryable<ShipRegisterDTO> GetFreeTextFilteredShips(string text, int? territoryUnitId)
        {
            DateTime now = DateTime.Now;
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            HashSet<int> shipIds = (from owner in Db.ShipOwners
                                    join person in Db.Persons on owner.OwnerPersonId equals person.Id into p
                                    from person in p.DefaultIfEmpty()
                                    join legal in Db.Legals on owner.OwnerLegalId equals legal.Id into l
                                    from legal in l.DefaultIfEmpty()
                                    where owner.IsActive
                                        && ((owner.OwnerIsPerson && (person.FirstName + " " + person.LastName).ToLower().Contains(text))
                                            || (!owner.OwnerIsPerson && legal.Name.ToLower().Contains(text)))
                                    select owner.ShipRegisterId).ToHashSet();

            IQueryable<ShipRegisterDTO> result = from ship in Db.ShipsRegister
                                                 join eventType in Db.NeventTypes on ship.EventTypeId equals eventType.Id
                                                 join appl in Db.Applications on ship.ApplicationId equals appl.Id into shipAppl
                                                 from appl in shipAppl.DefaultIfEmpty()
                                                 where ship.RecordType == nameof(RecordTypesEnum.Register)
                                                    && ship.ValidFrom <= now
                                                    && ship.ValidTo > now
                                                    && (ship.Cfr.ToLower().Contains(text)
                                                        || ship.Name.ToLower().Contains(text)
                                                        || ship.ExternalMark.ToLower().Contains(text)
                                                        || eventType.Code.ToLower().Contains(text)
                                                        || eventType.Name.ToLower().Contains(text)
                                                        || (searchDate.HasValue && ship.EventDate == searchDate.Value)
                                                        || shipIds.Contains(ship.Id))
                                                    && (!territoryUnitId.HasValue || (appl != null && appl.TerritoryUnitId == territoryUnitId.Value))
                                                 orderby ship.EventDate descending
                                                 select new ShipRegisterDTO
                                                 {
                                                     Id = ship.Id,
                                                     CFR = ship.Cfr,
                                                     Name = ship.Name,
                                                     Owners = string.Join(", ", from owner in Db.ShipOwners
                                                                                join person in Db.Persons on owner.OwnerPersonId equals person.Id into per
                                                                                from person in per.DefaultIfEmpty()
                                                                                join legal in Db.Legals on owner.OwnerLegalId equals legal.Id into leg
                                                                                from legal in leg.DefaultIfEmpty()
                                                                                where owner.ShipRegisterId == ship.Id
                                                                                select owner.OwnerIsPerson ? person.FirstName + " " + person.LastName : legal.Name),
                                                     ExternalMark = ship.ExternalMark,
                                                     LastChangeDate = ship.EventDate,
                                                     LastChangeStatus = eventType.Code + ": " + eventType.Name,
                                                     EventType = Enum.Parse<ShipEventTypeEnum>(eventType.Code),
                                                     IsThirdPartyShip = ship.IsThirdPartyShip
                                                 };
            return result;
        }

        private ShipRegisterEditDTO MapDbShipToDTO(ShipRegister dbShip, bool includeAcquiredCapacity = false)
        {
            ShipRegisterEditDTO result = new ShipRegisterEditDTO
            {
                Id = dbShip.Id,
                ShipUID = dbShip.ShipUid,
                ApplicationId = dbShip.ApplicationId,
                EventType = GetEventTypeById(dbShip.EventTypeId),
                EventDate = dbShip.EventDate,
                IsThirdPartyShip = dbShip.IsThirdPartyShip,
                CFR = dbShip.Cfr,
                Name = dbShip.Name,
                ExternalMark = dbShip.ExternalMark,
                RegistrationNumber = dbShip.RegistrationNum,
                RegistrationDate = dbShip.RegistrationDate,
                FleetTypeId = dbShip.FleetTypeId,
                FleetSegmentId = dbShip.FleetSegmentId,
                IRCSCallSign = dbShip.IrcscallSign,
                MMSI = dbShip.Mmsi,
                UVI = dbShip.Uvi,
                CountryFlagId = dbShip.FlagCountryId,
                HasAIS = dbShip.HasAis,
                HasERS = dbShip.HasErs,
                HasVMS = dbShip.HasVms,
                VesselTypeId = dbShip.VesselTypeId,
                RegLicenceNum = dbShip.RegLicenceNum,
                RegLicenceDate = dbShip.RegLicenseDate,
                RegLicencePublisher = dbShip.RegLicensePublisher,
                RegLicencePublishVolume = dbShip.RegLicensePublishVolume,
                RegLicencePublishPage = dbShip.RegLicensePublishPage,
                RegLicencePublishNum = dbShip.RegLicensePublishNum,
                ExploitationStartDate = dbShip.ExploitationStartDate,
                BuildYear = dbShip.BuildYear,
                BuildPlace = dbShip.BuildPlace,
                AdminDecisionNum = dbShip.AdminDecisionNum,
                AdminDecisionDate = dbShip.AdminDecisionDate,
                PublicAidTypeId = dbShip.PublicAidTypeId,
                PortId = dbShip.PortId,
                StayPortId = dbShip.StayPortId,
                SailAreaId = dbShip.SailAreaId,
                TotalLength = dbShip.TotalLength,
                TotalWidth = dbShip.TotalWidth,
                GrossTonnage = dbShip.GrossTonnage,
                NetTonnage = dbShip.NetTonnage,
                OtherTonnage = dbShip.OtherTonnage,
                BoardHeight = dbShip.BoardHeight,
                ShipDraught = dbShip.ShipDraught,
                LengthBetweenPerpendiculars = dbShip.LengthBetweenPerpendiculars,
                MainEnginePower = dbShip.MainEnginePower,
                AuxiliaryEnginePower = dbShip.AuxiliaryEnginePower,
                MainEngineNum = dbShip.MainEngineNum,
                MainEngineModel = dbShip.MainEngineModel,
                MainFishingGearId = dbShip.MainFishingGearId,
                AdditionalFishingGearId = dbShip.AdditionalFishingGearId,
                HullMaterialId = dbShip.HullMaterialId,
                FuelTypeId = dbShip.FuelTypeId,
                TotalPassengerCapacity = dbShip.TotalPassengerCapacity,
                CrewCount = dbShip.CrewCount,
                HasControlCard = dbShip.HasControlCard,
                HasValidityCertificate = dbShip.HasValidityCertificate,
                HasFoodLawLicence = dbShip.HasFoodLawLicense,
                ShipAssociationId = dbShip.ShipAssociationId,
                ImportCountryId = dbShip.ImportCountryId,
                ExportCountryId = dbShip.ExportCountryId,
                ExportType = dbShip.ExportType == null ? default(ShipExportTypeEnum?) : Enum.Parse<ShipExportTypeEnum>(dbShip.ExportType),
                HasFishingPermit = dbShip.HasFishingPermit,
                IsForbiddenForRSR = dbShip.IsForbidden,
                ForbiddenStartDate = dbShip.ForbiddenStartDate,
                ForbiddenEndDate = dbShip.ForbiddenEndDate,
                ForbiddenReason = dbShip.ForbiddenReason,
                Comments = dbShip.Comments
            };

            if (result.HasControlCard.Value)
            {
                result.ControlCardNum = dbShip.ControlCardNum;
                result.ControlCardDate = dbShip.ControlCardDate.Value == DefaultConstants.MIN_VALID_DATE
                                    ? default
                                    : dbShip.ControlCardDate.Value;
            }

            if (result.HasValidityCertificate.Value)
            {
                result.ControlCardValidityCertificateNum = dbShip.ControlCardValidityCertificateNum;
                result.ControlCardValidityCertificateDate = dbShip.ControlCardValidityCertificateDate.Value == DefaultConstants.MIN_VALID_DATE
                                                        ? default
                                                        : dbShip.ControlCardValidityCertificateDate.Value;
                result.ControlCardDateOfLastAttestation = dbShip.ControlCardDateOfLastAttestation.Value == DefaultConstants.MIN_VALID_DATE
                                                        ? default
                                                        : dbShip.ControlCardDateOfLastAttestation.Value;
            }

            if (result.HasFoodLawLicence.Value)
            {
                result.FoodLawLicenceNum = dbShip.FoodLawLicenseNum;
                result.FoodLawLicenceDate = dbShip.FoodLawLicenseDate.Value;
            }

            FleetTypeNomenclatureDTO fleet = GetFleetType(result.FleetTypeId.Value);
            if (includeAcquiredCapacity && fleet.HasFishingCapacity)
            {
                result.AcquiredFishingCapacity = fishingCapacityService.GetAcquiredFishingCapacityByApplicationId(dbShip.ApplicationId.Value);

                CapacityChangeHistoryDTO change = fishingCapacityService.GetCapacityChangeHistory(dbShip.ApplicationId.Value, RecordTypesEnum.Application);
                result.RemainingCapacityAction = fishingCapacityService.GetCapacityFreeActionsFromChangeHistory(change);
            }

            result.CancellationDetails = cancellationService.GetCancellationDetails(dbShip.CancellationDetailsId);
            result.Owners = GetOwners(result.Id.Value);
            result.ShipUsers = GetShipUsers(result.ShipUID.Value);
            result.UsedCapacityCertificates = GetShipUsedCapacityCertificates(result.ShipUID.Value);
            result.Files = Db.GetFiles(Db.ShipRegisterFiles, result.Id.Value);

            return result;
        }

        private int GetEventTypeId(ShipEventTypeEnum type)
        {
            DateTime now = DateTime.Now;

            int id = (from eventType in Db.NeventTypes
                      where eventType.Code == type.ToString()
                        && eventType.ValidFrom <= now
                        && eventType.ValidTo > now
                      select eventType.Id).First();

            return id;
        }

        private ShipEventTypeEnum GetEventTypeById(int id)
        {
            ShipEventTypeEnum type = (from eventType in Db.NeventTypes
                                      where eventType.Id == id
                                      select Enum.Parse<ShipEventTypeEnum>(eventType.Code)).First();

            return type;
        }

        private static ShipRegister CopyShip(ShipRegister ship)
        {
            ShipRegister result = new ShipRegister
            {
                ShipUid = ship.ShipUid,
                ApplicationId = ship.ApplicationId,
                RecordType = ship.RecordType,
                IsThirdPartyShip = ship.IsThirdPartyShip,
                RegisterApplicationId = ship.RegisterApplicationId,
                Cfr = ship.Cfr,
                Name = ship.Name,
                ExternalMark = ship.ExternalMark,
                RegistrationNum = ship.RegistrationNum,
                RegistrationDate = ship.RegistrationDate,
                FleetTypeId = ship.FleetTypeId,
                FleetSegmentId = ship.FleetSegmentId,
                IrcscallSign = ship.IrcscallSign,
                Mmsi = ship.Mmsi,
                Uvi = ship.Uvi,
                EventTypeId = ship.EventTypeId,
                EventDate = ship.EventDate,
                FlagCountryId = ship.FlagCountryId,
                HasAis = ship.HasAis,
                HasErs = ship.HasErs,
                HasVms = ship.HasVms,
                VesselTypeId = ship.VesselTypeId,
                RegLicenceNum = ship.RegLicenceNum,
                RegLicenseDate = ship.RegLicenseDate,
                RegLicensePublisher = ship.RegLicensePublisher,
                RegLicensePublishVolume = ship.RegLicensePublishVolume,
                RegLicensePublishPage = ship.RegLicensePublishPage,
                RegLicensePublishNum = ship.RegLicensePublishNum,
                ExploitationStartDate = ship.ExploitationStartDate,
                BuildYear = ship.BuildYear,
                BuildPlace = ship.BuildPlace,
                AdminDecisionNum = ship.AdminDecisionNum,
                AdminDecisionDate = ship.AdminDecisionDate,
                PublicAidTypeId = ship.PublicAidTypeId,
                PortId = ship.PortId,
                StayPortId = ship.StayPortId,
                SailAreaId = ship.SailAreaId,
                TotalLength = ship.TotalLength,
                TotalWidth = ship.TotalWidth,
                GrossTonnage = ship.GrossTonnage,
                NetTonnage = ship.NetTonnage,
                OtherTonnage = ship.OtherTonnage,
                BoardHeight = ship.BoardHeight,
                ShipDraught = ship.ShipDraught,
                LengthBetweenPerpendiculars = ship.LengthBetweenPerpendiculars,
                MainEnginePower = ship.MainEnginePower,
                AuxiliaryEnginePower = ship.AuxiliaryEnginePower,
                MainEngineNum = ship.MainEngineNum,
                MainEngineModel = ship.MainEngineModel,
                MainFishingGearId = ship.MainFishingGearId,
                AdditionalFishingGearId = ship.AdditionalFishingGearId,
                HullMaterialId = ship.HullMaterialId,
                FuelTypeId = ship.FuelTypeId,
                TotalPassengerCapacity = ship.TotalPassengerCapacity,
                CrewCount = ship.CrewCount,
                HasControlCard = ship.HasControlCard,
                ControlCardNum = ship.ControlCardNum,
                ControlCardDate = ship.ControlCardDate,
                HasValidityCertificate = ship.HasValidityCertificate,
                ControlCardValidityCertificateNum = ship.ControlCardValidityCertificateNum,
                ControlCardValidityCertificateDate = ship.ControlCardValidityCertificateDate,
                ControlCardDateOfLastAttestation = ship.ControlCardDateOfLastAttestation,
                HasFoodLawLicense = ship.HasFoodLawLicense,
                FoodLawLicenseNum = ship.FoodLawLicenseNum,
                FoodLawLicenseDate = ship.FoodLawLicenseDate,
                ShipAssociationId = ship.ShipAssociationId,
                ImportCountryId = ship.ImportCountryId,
                ExportCountryId = ship.ExportCountryId,
                ExportType = ship.ExportType,
                Comments = ship.Comments,
                CancellationDetailsId = ship.CancellationDetailsId,
                ValidFrom = ship.ValidFrom,
                ValidTo = ship.ValidTo,
                HasFishingPermit = ship.HasFishingPermit,
                IsForbidden = ship.IsForbidden,
                ForbiddenStartDate = ship.ForbiddenStartDate,
                ForbiddenEndDate = ship.ForbiddenEndDate,
                ForbiddenReason = ship.ForbiddenReason
            };

            return result;
        }

        private void CopyShipFiles(ShipRegister oldShip, ShipRegister newShip)
        {
            List<FileInfoDTO> files = Db.GetFiles(Db.ShipRegisterFiles, oldShip.Id);

            foreach (FileInfoDTO file in files)
            {
                Db.AddOrEditFile(newShip, newShip.ShipRegisterFiles, file);
            }
        }

        private void CopyShipOwners(ShipRegister oldShip, ShipRegister newShip)
        {
            List<ShipOwnerDTO> owners = GetOwners(oldShip.Id);

            foreach (ShipOwnerDTO owner in owners)
            {
                AddShipOwner(newShip, owner);
                Db.SaveChanges();
            }
        }

        private List<ShipOwnerDTO> GetOwners(int shipId)
        {
            List<ShipOwnerHelper> result = (from shipOwner in Db.ShipOwners
                                            where shipOwner.ShipRegisterId == shipId
                                            orderby shipOwner.OwnedShare descending
                                            select new ShipOwnerHelper
                                            {
                                                Id = shipOwner.Id,
                                                IsOwnerPerson = shipOwner.OwnerIsPerson,
                                                PersonId = shipOwner.OwnerPersonId,
                                                LegalId = shipOwner.OwnerLegalId,
                                                IsShipHolder = shipOwner.IsShipHolder,
                                                OwnedShare = shipOwner.OwnedShare,
                                                IsActive = shipOwner.IsActive
                                            }).ToList();

            ShipOwnersRegixData data = GetShipOwnersRegixData(shipId);
            SetOwnersRegixDataAndAddresses(result, data);

            return result.Select(x => x as ShipOwnerDTO).ToList();
        }

        private List<ShipOwnerRegixDataDTO> GetOwnersRegix(int shipId)
        {
            List<ShipOwnerRegixHelper> result = (from shipOwner in Db.ShipOwners
                                                 where shipOwner.ShipRegisterId == shipId
                                                 orderby shipOwner.OwnedShare descending
                                                 select new ShipOwnerRegixHelper
                                                 {
                                                     Id = shipOwner.Id,
                                                     IsOwnerPerson = shipOwner.OwnerIsPerson,
                                                     PersonId = shipOwner.OwnerPersonId,
                                                     LegalId = shipOwner.OwnerLegalId,
                                                     IsActive = shipOwner.IsActive
                                                 }).ToList();

            ShipOwnersRegixData data = GetShipOwnersRegixData(shipId);
            SetOwnersRegixDataAndAddresses(result, data);

            return result.Select(x => x as ShipOwnerRegixDataDTO).ToList();
        }

        private ShipOwnersRegixData GetShipOwnersRegixData(int shipId)
        {
            List<int> personIds = GetOwnersPersonIds(shipId);
            List<int> legalIds = GetOwnersLegalIds(shipId);

            ShipOwnersRegixData data = new ShipOwnersRegixData
            {
                PersonRegixData = personService.GetRegixPersonsData(personIds),
                LegalRegixData = legalService.GetRegixLegalsData(legalIds),
                PersonAddressData = personService.GetAddressRegistrations(personIds),
                LegalAddressData = legalService.GetAddressRegistrations(legalIds)
            };

            return data;
        }

        private static void SetOwnersRegixDataAndAddresses<T>(List<T> owners, ShipOwnersRegixData data)
            where T : IShipOwnerHelper
        {
            foreach (T owner in owners)
            {
                if (owner.IsOwnerPerson.Value)
                {
                    owner.RegixPersonData = data.PersonRegixData[owner.PersonId.Value];
                    owner.AddressRegistrations = data.PersonAddressData[owner.PersonId.Value].ToList();
                }
                else
                {
                    owner.RegixLegalData = data.LegalRegixData[owner.LegalId.Value];
                    owner.AddressRegistrations = data.LegalAddressData[owner.LegalId.Value].ToList();
                }
            }
        }

        private List<int> GetOwnersPersonIds(int shipId)
        {
            List<int> personIds = (from shipOwner in Db.ShipOwners
                                   where shipOwner.ShipRegisterId == shipId
                                        && shipOwner.OwnerIsPerson
                                   select shipOwner.OwnerPersonId.Value).ToList();
            return personIds;
        }

        private List<int> GetOwnersLegalIds(int shipId)
        {
            List<int> legalIds = (from shipOwner in Db.ShipOwners
                                  where shipOwner.ShipRegisterId == shipId
                                       && !shipOwner.OwnerIsPerson
                                  select shipOwner.OwnerLegalId.Value).ToList();
            return legalIds;
        }

        private BaseRegixApplicationDataIds GetRegixDataIds(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = (from appl in Db.Applications
                                                        join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                                        where appl.Id == applicationId
                                                        select new BaseRegixApplicationDataIds
                                                        {
                                                            ApplicationId = applicationId,
                                                            PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode)
                                                        }).First();

            return regixDataIds;
        }

        private List<ShipRegisterUserDTO> GetShipUsers(int shipUId)
        {
            DateTime now = DateTime.Now;

            List<int> shipIds = (from ship in Db.ShipsRegister
                                 where ship.ShipUid == shipUId
                                 select ship.Id).ToList();

            List<ShipRegisterUserDTO> result = (from licence in Db.CommercialFishingPermitLicensesRegisters
                                                join licenceType in Db.NcommercialFishingPermitLicenseTypes on licence.PermitLicenseTypeId equals licenceType.Id
                                                join groundsForUse in Db.HolderGroundsForUses on licence.ShipGroundsForUseId equals groundsForUse.Id
                                                join groundsForUseType in Db.NholderGroundsForUseTypes on groundsForUse.GroundsForUseTypeId equals groundsForUseType.Id
                                                join person in Db.Persons on licence.SubmittedForPersonId equals person.Id into per
                                                from person in per.DefaultIfEmpty()
                                                join legal in Db.Legals on licence.SubmittedForLegalId equals legal.Id into leg
                                                from legal in leg.DefaultIfEmpty()
                                                where licence.RecordType == nameof(RecordTypesEnum.Register)
                                                   && !licence.IsHolderShipOwner.Value
                                                   && shipIds.Contains(licence.ShipId)
                                                   && licence.IsActive
                                                   && licence.PermitLicenseValidFrom <= now
                                                   && ((licence.PermitLicenseValidTo.HasValue && licence.PermitLicenseValidTo > now) || !licence.PermitLicenseValidTo.HasValue)
                                                   && !licence.IsSuspended
                                                select new ShipRegisterUserDTO
                                                {
                                                    Id = licence.Id,
                                                    Name = person != null
                                                        ? person.FirstName + " " + (!string.IsNullOrEmpty(person.MiddleName) ? person.MiddleName + " " : "") + person.LastName
                                                        : legal.Name,
                                                    EgnLncEik = person != null ? person.EgnLnc : legal.Eik,
                                                    PermitLicenceType = licenceType.Name,
                                                    GroundsForUse = groundsForUseType.Name,
                                                    IsActive = true
                                                }).ToList();

            List<ShipRegisterUserDTO> poundnetUsers = (from permit in Db.CommercialFishingPermitRegisters
                                                       join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                                                       join groundsForUse in Db.HolderGroundsForUses on permit.ShipGroundsForUseId equals groundsForUse.Id
                                                       join groundsForUseType in Db.NholderGroundsForUseTypes on groundsForUse.GroundsForUseTypeId equals groundsForUseType.Id
                                                       join person in Db.Persons on permit.SubmittedForPersonId equals person.Id into per
                                                       from person in per.DefaultIfEmpty()
                                                       join legal in Db.Legals on permit.SubmittedForLegalId equals legal.Id into leg
                                                       from legal in leg.DefaultIfEmpty()
                                                       where permit.RecordType == nameof(RecordTypesEnum.Register)
                                                          && shipIds.Contains(permit.ShipId)
                                                          && !permit.IsHolderShipOwner.Value
                                                          && permit.IsActive
                                                          && permit.PermitValidFrom <= now
                                                          && (permit.IsPermitUnlimited.Value || permit.PermitValidTo.Value > now)
                                                          && !permit.IsSuspended
                                                       select new ShipRegisterUserDTO
                                                       {
                                                           Id = permit.Id,
                                                           Name = person != null
                                                               ? person.FirstName + " " + (!string.IsNullOrEmpty(person.MiddleName) ? person.MiddleName + " " : "") + person.LastName
                                                               : legal.Name,
                                                           EgnLncEik = person != null ? person.EgnLnc : legal.Eik,
                                                           PermitLicenceType = permitType.Name,
                                                           GroundsForUse = groundsForUseType.Name,
                                                           IsActive = true
                                                       }).ToList();

            return result.Concat(poundnetUsers).ToList();
        }

        private List<ShipRegisterUsedCertificateDTO> GetShipUsedCapacityCertificates(int shipUId)
        {
            // Използвани удостоверения за кораб са или при регистрация, или при увеличаване на капацитет
            // И в двата случая има запис в CapacityChangeHistory с AcquiredFishingCapacityId not null
            List<int> shipIds = (from ship in Db.ShipsRegister
                                 where ship.ShipUid == shipUId
                                 select ship.Id).ToList();

            List<ShipRegisterUsedCertificateDTO> result = (from change in Db.CapacityChangeHistory
                                                           join acquired in Db.AcquiredCapacityRegister on change.AcquiredFishingCapacityId equals acquired.Id
                                                           join acquiredCert in Db.AcquiredCapacityCertificates on acquired.Id equals acquiredCert.AcquiredCapacityId
                                                           join cert in Db.CapacityCertificatesRegister on acquiredCert.CapacityCertificateId equals cert.Id
                                                           join cap in Db.ShipCapacityRegister on change.ShipCapacityId equals cap.Id
                                                           join ship in Db.ShipsRegister on cap.ShipId equals ship.Id
                                                           where change.RecordType == nameof(RecordTypesEnum.Register)
                                                                 && acquired.AcquiredType == nameof(AcquiredCapacityMannerEnum.FreeCapLicence)
                                                                 && shipIds.Contains(ship.Id)
                                                           orderby change.DateOfChange descending
                                                           select new ShipRegisterUsedCertificateDTO
                                                           {
                                                               Date = change.DateOfChange,
                                                               Num = cert.Id,
                                                               EnginePower = cert.MainEnginePower,
                                                               GrossTonnage = cert.GrossTonnage,
                                                               IsActive = true
                                                           }).ToList();

            return result;
        }

        private static void EditShipEditEvent(ShipRegister entry, ShipRegisterEditDTO ship)
        {
            entry.RegLicenceNum = ship.RegLicenceNum;
            entry.RegLicenseDate = ship.RegLicenceDate;
            entry.RegLicensePublisher = ship.RegLicencePublisher;
            entry.RegLicensePublishVolume = ship.RegLicencePublishVolume;
            entry.RegLicensePublishNum = ship.RegLicencePublishNum;
            entry.RegLicensePublishPage = ship.RegLicencePublishPage;
            entry.AdminDecisionNum = ship.AdminDecisionNum;
            entry.AdminDecisionDate = ship.AdminDecisionDate;
            entry.StayPortId = ship.StayPortId;
            entry.MainEngineNum = ship.MainEngineNum;
            entry.FuelTypeId = ship.FuelTypeId.Value;
            entry.TotalPassengerCapacity = ship.TotalPassengerCapacity.Value;
            entry.ShipAssociationId = ship.ShipAssociationId;
            entry.Comments = ship.Comments;

            if (ship.SailAreaId.HasValue)
            {
                entry.SailAreaId = ship.SailAreaId;
            }
            else if (!string.IsNullOrEmpty(ship.SailAreaName))
            {
                NsailArea sailArea = new NsailArea { Name = ship.SailAreaName };
                entry.SailArea = sailArea;
            }

            if (ship.HasControlCard.Value)
            {
                entry.ControlCardNum = ship.ControlCardNum;
                entry.ControlCardDate = ship.ControlCardDate.Value;
            }
            else
            {
                entry.ControlCardNum = null;
                entry.ControlCardDate = null;
            }

            if (ship.HasValidityCertificate.Value)
            {
                entry.ControlCardValidityCertificateNum = ship.ControlCardValidityCertificateNum;
                entry.ControlCardValidityCertificateDate = ship.ControlCardValidityCertificateDate.Value;
                entry.ControlCardDateOfLastAttestation = ship.ControlCardDateOfLastAttestation.Value;
            }
            else
            {
                entry.ControlCardValidityCertificateNum = null;
                entry.ControlCardValidityCertificateDate = null;
                entry.ControlCardDateOfLastAttestation = null;
            }

            if (entry.HasFoodLawLicense)
            {
                entry.FoodLawLicenseNum = ship.FoodLawLicenceNum;
                entry.FoodLawLicenseDate = ship.FoodLawLicenceDate.Value;
            }
            else
            {
                entry.FoodLawLicenseNum = null;
                entry.FoodLawLicenseDate = null;
            }

            if (entry.IsForbidden)
            {
                entry.ForbiddenStartDate = ship.ForbiddenStartDate.Value;
                entry.ForbiddenEndDate = ship.ForbiddenEndDate.Value;
                entry.ForbiddenReason = ship.ForbiddenReason;
            }
            else
            {
                entry.ForbiddenStartDate = null;
                entry.ForbiddenEndDate = null;
                entry.ForbiddenReason = null;
            }
        }

        private static void EditShipChaEvent(ShipRegister entry, ShipRegisterEditDTO ship)
        {
            entry.Comments = ship.Comments;
        }

        private static void EditShipModEvent(ShipRegister entry, ShipRegisterEditDTO ship)
        {
            entry.Name = ship.Name;
            entry.VesselTypeId = ship.VesselTypeId;
            entry.ExternalMark = ship.ExternalMark;
            entry.RegistrationNum = ship.RegistrationNumber;
            entry.RegistrationDate = ship.RegistrationDate.Value;
            entry.IrcscallSign = ship.IRCSCallSign;
            entry.Mmsi = ship.MMSI;
            entry.Uvi = ship.UVI;
            entry.HasAis = ship.HasAIS.Value;
            entry.HasErs = ship.HasERS.Value;
            entry.HasVms = ship.HasVMS.Value;
            entry.ExploitationStartDate = ship.ExploitationStartDate;
            entry.BuildYear = ship.BuildYear.Value;
            entry.BuildPlace = ship.BuildPlace;
            entry.PublicAidTypeId = ship.PublicAidTypeId.Value;
            entry.PortId = ship.PortId.Value;
            entry.TotalLength = ship.TotalLength.Value;
            entry.TotalWidth = ship.TotalWidth.Value;
            entry.GrossTonnage = ship.GrossTonnage.Value;
            entry.NetTonnage = ship.NetTonnage;
            entry.OtherTonnage = ship.OtherTonnage;
            entry.MainEnginePower = ship.MainEnginePower.Value;
            entry.AuxiliaryEnginePower = ship.AuxiliaryEnginePower;
            entry.MainEngineModel = ship.MainEngineModel;
            entry.MainFishingGearId = ship.MainFishingGearId.Value;
            entry.AdditionalFishingGearId = ship.AdditionalFishingGearId;
            entry.BoardHeight = ship.BoardHeight.Value;
            entry.ShipDraught = ship.ShipDraught.Value;
            entry.LengthBetweenPerpendiculars = ship.LengthBetweenPerpendiculars;
            entry.HullMaterialId = ship.HullMaterialId.Value;
            entry.CrewCount = ship.CrewCount.Value;
            entry.FleetSegmentId = ship.FleetSegmentId.Value;
            entry.Comments = ship.Comments;
        }

        private void EditShipExpEvent(ShipRegister entry, ShipRegisterEditDTO ship)
        {
            entry.ExportCountryId = ship.ExportCountryId;
            entry.ExportType = ship.ExportType.ToString();
            entry.Comments = ship.Comments;

            if (!ship.IsThirdPartyShip.Value && ship.IsNoApplicationDeregistration.Value)
            {
                EditCapacityNoApplicationDeregistration(ship);
            }
        }

        private void EditShipRetEvent(ShipRegister entry, ShipRegisterEditDTO ship)
        {
            Db.AddOrEditCancellationDetails(entry, ship.CancellationDetails);

            entry.Comments = ship.Comments;

            if (!ship.IsThirdPartyShip.Value && ship.IsNoApplicationDeregistration.Value)
            {
                EditCapacityNoApplicationDeregistration(ship);
            }
        }

        private void EditShipDesEvent(ShipRegister entry, ShipRegisterEditDTO ship)
        {
            Db.AddOrEditCancellationDetails(entry, ship.CancellationDetails);
            entry.Comments = ship.Comments;

            if (!ship.IsThirdPartyShip.Value && ship.IsNoApplicationDeregistration.Value)
            {
                EditCapacityNoApplicationDeregistration(ship);
            }
        }

        private void EditCapacityNoApplicationDeregistration(ShipRegisterEditDTO ship)
        {
            FleetTypeNomenclatureDTO fleet = GetFleetType(ship.FleetTypeId.Value);

            if (fleet.HasFishingCapacity)
            {
                ShipCapacityRegister latestShipCapacity = GetLatestShipCapacity(ship.Id.Value);

                FishingCapacityFreedActionsDTO freedCapacityAction = new FishingCapacityFreedActionsDTO
                {
                    Action = FishingCapacityRemainderActionEnum.NoCertificate
                };

                fishingCapacityService.AddReduceCapacityChangeHistory(RecordTypesEnum.Register,
                                                                      null,
                                                                      latestShipCapacity.Id,
                                                                      latestShipCapacity.GrossTonnage,
                                                                      latestShipCapacity.EnginePower,
                                                                      freedCapacityAction);
            }
        }

        private void AddShipOwner(ShipRegister dbShip, ShipOwnerDTO owner)
        {
            Person person = null;
            Legal legal = null;

            if (owner.IsOwnerPerson.Value)
            {
                person = Db.AddOrEditPerson(owner.RegixPersonData, owner.AddressRegistrations);
            }
            else
            {
                owner.RegixLegalData.Id = null;

                RecordTypesEnum recordType = Enum.Parse<RecordTypesEnum>(dbShip.RecordType);

                legal = Db.AddOrEditLegal(
                    new ApplicationRegisterDataDTO
                    {
                        ApplicationId = recordType == RecordTypesEnum.Application ? dbShip.ApplicationId : null,
                        RecordType = recordType
                    },
                    owner.RegixLegalData,
                    owner.AddressRegistrations
                );
            }

            ShipOwner entry = new ShipOwner
            {
                ShipRegister = dbShip,
                OwnerIsPerson = owner.IsOwnerPerson.Value,
                OwnerPerson = person,
                OwnerLegal = legal,
                OwnedShare = owner.OwnedShare.Value,
                IsShipHolder = owner.IsShipHolder.Value,
                IsActive = owner.IsActive.Value
            };

            Db.ShipOwners.Add(entry);
        }

        private int CalculateNewShipEventTypeId(short buildYear, int? importCountryId, bool oldShip = false)
        {
            ShipEventTypeEnum eventType = importCountryId.HasValue
                ? ShipEventTypeEnum.IMP
                : DateTime.Now.Year - buildYear > 2
                    ? ShipEventTypeEnum.CHA
                    : ShipEventTypeEnum.CST;

            if (eventType != ShipEventTypeEnum.IMP && oldShip == true)
            {
                eventType = ShipEventTypeEnum.CHA;
            }

            int eventId = GetEventTypeId(eventType);
            return eventId;
        }

        private void TransferShipQuotas(int oldShipId, ShipRegister entry)
        {
            List<ShipCatchQuota> shipCatchQuotas = (from scq in Db.ShipCatchQuotas
                                                    where scq.ShipId == oldShipId
                                                    select scq).ToList();

            foreach (ShipCatchQuota scq in shipCatchQuotas)
            {
                scq.Ship = entry;
            }
        }

        private FleetTypeNomenclatureDTO GetFleetType(int fleetTypeId)
        {
            DateTime now = DateTime.Now;

            FleetTypeNomenclatureDTO result = (from fleet in Db.NfleetTypes
                                               where fleet.Id == fleetTypeId
                                               select new FleetTypeNomenclatureDTO
                                               {
                                                   Value = fleet.Id,
                                                   DisplayName = fleet.Name,
                                                   HasControlCard = fleet.HasControlCard,
                                                   HasFitnessCertificate = fleet.HasFitnessCertificate,
                                                   HasFishingCapacity = fleet.HasFishingCapacity
                                               }).First();

            return result;
        }
    }
}
