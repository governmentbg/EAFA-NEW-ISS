using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Interfaces;
using IARA.Interfaces;
using IARA.Interfaces.FSM;
using IARA.Interfaces.Legals;
using IARA.Interfaces.Reports;
using IARA.RegixAbstractions.Interfaces;

namespace IARA.Infrastructure.Services
{
    public partial class FishingCapacityService : Service, IFishingCapacityService
    {
        private readonly IApplicationService applicationService;
        private readonly IUserService userService;
        private readonly IPersonService personService;
        private readonly ILegalService legalService;
        private readonly IApplicationStateMachine stateMachine;
        private readonly IRegixApplicationInterfaceService regixApplicationService;
        private readonly IJasperReportExecutionService jasperReportExecutionService;
        private readonly IDeliveryService deliveryService;
        private readonly IExcelExporterService excelExporterService;

        public FishingCapacityService(IARADbContext db,
                                      IApplicationService applicationService,
                                      IUserService userService,
                                      IPersonService personService,
                                      ILegalService legalService,
                                      IApplicationStateMachine stateMachine,
                                      IRegixApplicationInterfaceService regixApplicationService,
                                      IJasperReportExecutionService jasperReportExecutionService,
                                      IDeliveryService deliveryService,
                                      IExcelExporterService excelExporterService)
            : base(db)
        {
            this.applicationService = applicationService;
            this.userService = userService;
            this.personService = personService;
            this.legalService = legalService;
            this.stateMachine = stateMachine;
            this.regixApplicationService = regixApplicationService;
            this.jasperReportExecutionService = jasperReportExecutionService;
            this.deliveryService = deliveryService;
            this.excelExporterService = excelExporterService;
        }

        // Acquired fishing capacity
        public AcquiredFishingCapacityDTO GetAcquiredFishingCapacity(int id)
        {
            AcquiredFishingCapacityDTO result = (from cap in Db.AcquiredCapacityRegister
                                                 where cap.Id == id
                                                    && cap.IsActive
                                                 select new AcquiredFishingCapacityDTO
                                                 {
                                                     AcquiredManner = Enum.Parse<AcquiredCapacityMannerEnum>(cap.AcquiredType),
                                                     GrossTonnage = cap.GrossTonnage,
                                                     Power = cap.EnginePower
                                                 }).FirstOrDefault();

            if (result != null)
            {
                result.CapacityLicenceIds = (from acquiredCert in Db.AcquiredCapacityCertificates
                                             where acquiredCert.AcquiredCapacityId == id
                                                && acquiredCert.IsActive
                                             select acquiredCert.CapacityCertificateId).ToList();
            }

            return result;
        }

        public AcquiredFishingCapacityDTO GetAcquiredFishingCapacityByApplicationId(int applicationId, RecordTypesEnum recordType = RecordTypesEnum.Application)
        {
            int? acquiredFishingCapacityId = (from change in Db.CapacityChangeHistory
                                              where change.ApplicationId == applicationId
                                                    && change.RecordType == recordType.ToString()
                                              select change.AcquiredFishingCapacityId).FirstOrDefault();

            return acquiredFishingCapacityId.HasValue ? GetAcquiredFishingCapacity(acquiredFishingCapacityId.Value) : null;
        }

        // Register
        public IQueryable<ShipFishingCapacityDTO> GetAllShipCapacities(FishingCapacityFilters filters)
        {
            IQueryable<ShipFishingCapacityDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllShipCapacities();
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredShipCapacities(filters.FreeTextSearch, filters.TerritoryUnitId)
                    : GetParametersFilteredShipCapacities(filters);
            }
            return result;
        }

        public void SetShipCapacityHistories(List<ShipFishingCapacityDTO> caps)
        {
            List<int> shipIds = caps.Select(x => x.ShipId).ToList();

            DateTime now = DateTime.Now;

            ILookup<int, int> uidIds = (from ship in Db.ShipsRegister
                                        where shipIds.Contains(ship.Id)
                                        select new
                                        {
                                            ship.ShipUid,
                                            ship.Id
                                        }).ToLookup(x => x.ShipUid, y => y.Id);

            List<int> uids = uidIds.Select(x => x.Key).ToList();

            var result = (from cap in Db.ShipCapacityRegister
                          join change in Db.CapacityChangeHistory on cap.Id equals change.ShipCapacityId
                          join ship in Db.ShipsRegister on cap.ShipId equals ship.Id
                          where change.RecordType == nameof(RecordTypesEnum.Register)
                                && uids.Contains(ship.ShipUid)
                          orderby change.DateOfChange descending
                          select new ShipFishingCapacityHistoryDTO
                          {
                              Id = cap.Id,
                              ShipCfr = ship.Cfr,
                              ShipUID = ship.ShipUid,
                              GrossTonnage = cap.GrossTonnage,
                              Power = cap.EnginePower,
                              DateFrom = cap.ValidFrom,
                              DateTo = cap.ValidTo,
                              ApplicationId = change.ApplicationId,
                              IsActive = true
                          }).ToList();

            List<int> applicationIds = result.Where(x => x.ApplicationId.HasValue).Select(x => x.ApplicationId.Value).ToList();

            var pageCodes = (from application in Db.Applications
                             join applType in Db.NapplicationTypes on application.ApplicationTypeId equals applType.Id
                             where applicationIds.Contains(application.Id)
                             select new
                             {
                                 ApplicationId = application.Id,
                                 Data = new
                                 {
                                     Reason = applType.Name,
                                     PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode)
                                 }
                             }).ToDictionary(x => x.ApplicationId, y => y.Data);

            foreach (ShipFishingCapacityHistoryDTO entry in result)
            {
                if (entry.ApplicationId.HasValue && pageCodes.TryGetValue(entry.ApplicationId.Value, out var data))
                {
                    entry.Reason = data.Reason;
                    entry.PageCode = data.PageCode;
                }
            }

            Dictionary<int, (decimal tonnage, decimal power)> prev = new Dictionary<int, (decimal tonnage, decimal power)>();

            for (int i = result.Count - 1; i >= 0; --i)
            {
                result[i].ShipId = uidIds[result[i].ShipUID].Where(x => shipIds.Contains(x)).Select(x => x).Single();

                bool exists = prev.TryGetValue(result[i].ShipId, out (decimal tonnage, decimal power) previous);

                if (!exists)
                {
                    previous.tonnage = 0.0M;
                    previous.power = 0.0M;
                    prev.Add(result[i].ShipId, (0.0M, 0.0M));
                }

                result[i].GrossTonnageChange = result[i].GrossTonnage - previous.tonnage;
                result[i].PowerChange = result[i].Power - previous.power;

                prev[result[i].ShipId] = (result[i].GrossTonnage, result[i].Power);
            }

            foreach (ShipFishingCapacityHistoryDTO hist in result)
            {
                ShipFishingCapacityDTO entry = caps.Where(x => x.ShipId == hist.ShipId).Single();

                if (entry.History != null)
                {
                    entry.History.Add(hist);
                }
                else
                {
                    entry.History = new List<ShipFishingCapacityHistoryDTO> { hist };
                }
            }
        }

        public CapacityChangeHistoryDTO GetCapacityChangeHistory(int applicationId, RecordTypesEnum recordType)
        {
            CapacityChangeHistoryDTO result = (from change in Db.CapacityChangeHistory
                                               where change.ApplicationId == applicationId
                                                    && change.RecordType == recordType.ToString()
                                               select new CapacityChangeHistoryDTO
                                               {
                                                   Id = change.Id,
                                                   ApplicationId = change.ApplicationId,
                                                   DateOfChange = change.DateOfChange,
                                                   TypeOfChange = Enum.Parse<FishingCapacityChangeTypeEnum>(change.TypeOfChange),
                                                   ShipCapacityId = change.ShipCapacityId,
                                                   AcquiredFishingCapacityId = change.AcquiredFishingCapacityId,
                                                   CapacityCertificateTransferId = change.CapacityCertificateTransferId,
                                                   GrossTonnageChange = change.GrossTonnageChange,
                                                   PowerChange = change.PowerChange,
                                                   ReasonOfChange = change.ReasonOfChange,
                                                   IsActive = change.IsActive
                                               }).FirstOrDefault();

            if (result != null)
            {
                result.CapacityCertificateIds = (from changeCert in Db.CapacityChangeHistoryCertificates
                                                 where changeCert.CapacityChangeHistoryId == result.Id
                                                    && changeCert.IsActive
                                                 select changeCert.CapacityCertificateId).ToList();
            }

            return result;
        }

        public FishingCapacityFreedActionsDTO GetCapacityFreeActionsFromChangeHistory(CapacityChangeHistoryDTO change)
        {
            FishingCapacityFreedActionsDTO result = new FishingCapacityFreedActionsDTO
            {
                Holders = new List<FishingCapacityHolderDTO>()
            };

            if (change.CapacityCertificateIds.Count == 0)
            {
                result.Action = FishingCapacityRemainderActionEnum.NoCertificate;
            }
            else
            {
                var submittedFor = (from appl in Db.Applications
                                    where appl.Id == change.ApplicationId
                                    select new
                                    {
                                        appl.SubmittedForPersonId,
                                        appl.SubmittedForLegalId
                                    }).First();

                List<CapacityHolderHelper> holders = GetCapacityHolderHelpers(change.CapacityCertificateIds);

                bool isFirstHolderSubmittedFor = holders.Count == 1
                    && holders[0].IsActive.Value
                    && ((holders[0].IsHolderPerson.Value && submittedFor.SubmittedForPersonId == holders[0].PersonId)
                        || (!holders[0].IsHolderPerson.Value && submittedFor.SubmittedForLegalId == holders[0].LegalId));

                if (isFirstHolderSubmittedFor)
                {
                    result.Action = FishingCapacityRemainderActionEnum.Certificate;
                }
                else
                {
                    result.Action = FishingCapacityRemainderActionEnum.Transfer;

                    result.Holders = (from holder in holders
                                      select new FishingCapacityHolderDTO
                                      {
                                          Id = holder.Id,
                                          IsHolderPerson = holder.IsHolderPerson,
                                          TransferredTonnage = holder.TransferredTonnage,
                                          TransferredPower = holder.TransferredPower,
                                          IsActive = holder.IsActive
                                      }).ToList();

                    SetCapacityHoldersPersonLegal(result.Holders, holders);
                }
            }

            return result;
        }

        public FishingCapacityFreedActionsRegixDataDTO GetCapacityFreeActionsRegixFromChangeHistory(CapacityChangeHistoryDTO change)
        {
            FishingCapacityFreedActionsRegixDataDTO result = new FishingCapacityFreedActionsRegixDataDTO
            {
                Holders = new List<FishingCapacityHolderRegixDataDTO>()
            };

            if (change.CapacityCertificateIds.Count == 0)
            {
                result.Action = FishingCapacityRemainderActionEnum.NoCertificate;
            }
            else
            {
                var submittedFor = (from appl in Db.Applications
                                    where appl.Id == change.ApplicationId
                                    select new
                                    {
                                        appl.SubmittedForPersonId,
                                        appl.SubmittedForLegalId
                                    }).First();

                List<CapacityHolderRegixHelper> holders = GetCapacityHolderRegixHelpers(change.CapacityCertificateIds);

                bool isFirstHolderSubmittedFor = holders.Count == 1
                    && holders[0].IsActive.Value
                    && ((holders[0].IsHolderPerson.Value && submittedFor.SubmittedForPersonId == holders[0].PersonId)
                        || (!holders[0].IsHolderPerson.Value && submittedFor.SubmittedForLegalId == holders[0].LegalId));

                if (isFirstHolderSubmittedFor)
                {
                    result.Action = FishingCapacityRemainderActionEnum.Certificate;
                }
                else
                {
                    result.Action = FishingCapacityRemainderActionEnum.Transfer;

                    result.Holders = (from holder in holders
                                      select new FishingCapacityHolderRegixDataDTO
                                      {
                                          Id = holder.Id,
                                          IsHolderPerson = holder.IsHolderPerson,
                                          IsActive = holder.IsActive
                                      }).ToList();

                    SetCapacityHoldersPersonLegal(result.Holders, holders);
                }
            }

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.ShipCapacityRegister, id);
        }

        public SimpleAuditDTO GetFishingCapacityHolderSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.CapacityCertificatesRegister, id);
        }

        public async Task<ApplicationEDeliveryInfo> GetEDeliveryInfo(int applicationId, PageCodeEnum pageCode)
        {
            if (pageCode == PageCodeEnum.ReduceFishCap)
            {
                // TODO
            }
            else if (pageCode == PageCodeEnum.IncreaseFishCap)
            {
                // TODO
            }
            else if (pageCode == PageCodeEnum.TransferFishCap)
            {
                // TODO
            }
            else if (pageCode == PageCodeEnum.CapacityCertDup)
            {
                // TODO
            }

            throw new ArgumentException("Nothing to deliver for page code: " + pageCode.ToString());
        }

        // Ship capacity helpers
        private IQueryable<ShipFishingCapacityDTO> GetAllShipCapacities()
        {
            DateTime now = DateTime.Now;

            IQueryable<ShipFishingCapacityDTO> result = from cap in Db.ShipCapacityRegister
                                                        join change in Db.CapacityChangeHistory on cap.Id equals change.ShipCapacityId
                                                        join ship in Db.ShipsRegister on cap.ShipId equals ship.Id
                                                        where change.RecordType == nameof(RecordTypesEnum.Register)
                                                            && cap.ValidFrom <= now
                                                            && cap.ValidTo > now
                                                        orderby change.DateOfChange descending
                                                        select new ShipFishingCapacityDTO
                                                        {
                                                            Id = cap.Id,
                                                            ShipId = ship.Id,
                                                            ShipCfr = ship.Cfr,
                                                            ShipName = ship.Name,
                                                            GrossTonnage = cap.GrossTonnage,
                                                            Power = cap.EnginePower,
                                                            DateOfChange = cap.ValidFrom,
                                                            IsActive = true
                                                        };

            return result;
        }

        private IQueryable<ShipFishingCapacityDTO> GetParametersFilteredShipCapacities(FishingCapacityFilters filters)
        {
            DateTime now = DateTime.Now;

            var query = from cap in Db.ShipCapacityRegister
                        join change in Db.CapacityChangeHistory on cap.Id equals change.ShipCapacityId
                        join ship in Db.ShipsRegister on cap.ShipId equals ship.Id
                        join appl in Db.Applications on change.ApplicationId equals appl.Id
                        where change.RecordType == nameof(RecordTypesEnum.Register)
                            && cap.ValidFrom <= now
                            && cap.ValidTo > now
                        orderby change.DateOfChange descending
                        select new
                        {
                            Id = cap.Id,
                            ShipId = ship.Id,
                            ShipCfr = ship.Cfr,
                            ShipName = ship.Name,
                            GrossTonnage = cap.GrossTonnage,
                            Power = cap.EnginePower,
                            DateOfChange = cap.ValidFrom,
                            TeritorryUnitId = appl.TerritoryUnitId,
                            IsActive = true
                        };

            if (!string.IsNullOrEmpty(filters.ShipCfr))
            {
                query = query.Where(x => x.ShipCfr.ToLower().Contains(filters.ShipCfr.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.ShipName))
            {
                query = query.Where(x => x.ShipName.ToLower().Contains(filters.ShipName.ToLower()));
            }

            if (filters.GrossTonnageFrom.HasValue)
            {
                query = query.Where(x => x.GrossTonnage >= filters.GrossTonnageFrom.Value);
            }

            if (filters.GrossTonnageTo.HasValue)
            {
                query = query.Where(x => x.GrossTonnage <= filters.GrossTonnageTo.Value);
            }

            if (filters.PowerFrom.HasValue)
            {
                query = query.Where(x => x.Power >= filters.PowerFrom.Value);
            }

            if (filters.PowerTo.HasValue)
            {
                query = query.Where(x => x.Power <= filters.PowerTo.Value);
            }

            if (filters.TerritoryUnitId.HasValue)
            {
                query = query.Where(x => x.TeritorryUnitId == filters.TerritoryUnitId);
            }

            IQueryable<ShipFishingCapacityDTO> result = from res in query
                                                        select new ShipFishingCapacityDTO
                                                        {
                                                            Id = res.Id,
                                                            ShipId = res.ShipId,
                                                            ShipCfr = res.ShipCfr,
                                                            ShipName = res.ShipName,
                                                            GrossTonnage = res.GrossTonnage,
                                                            Power = res.Power,
                                                            DateOfChange = res.DateOfChange,
                                                            IsActive = true
                                                        };

            return result;
        }

        private IQueryable<ShipFishingCapacityDTO> GetFreeTextFilteredShipCapacities(string text, int? territoryUnitId)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            DateTime now = DateTime.Now;

            IQueryable<ShipFishingCapacityDTO> result = from cap in Db.ShipCapacityRegister
                                                        join change in Db.CapacityChangeHistory on cap.Id equals change.ShipCapacityId
                                                        join ship in Db.ShipsRegister on cap.ShipId equals ship.Id
                                                        join appl in Db.Applications on change.ApplicationId equals appl.Id
                                                        where change.RecordType == nameof(RecordTypesEnum.Register)
                                                            && cap.ValidFrom <= now
                                                            && cap.ValidTo > now
                                                            && (ship.Cfr.ToLower().Contains(text)
                                                                || ship.Name.ToLower().Contains(text)
                                                                || cap.GrossTonnage.ToString().Contains(text)
                                                                || cap.EnginePower.ToString().Contains(text)
                                                                || (searchDate.HasValue && searchDate.Value == cap.ValidFrom))
                                                            && (!territoryUnitId.HasValue || appl.TerritoryUnitId == territoryUnitId.Value)
                                                        orderby change.DateOfChange descending
                                                        select new ShipFishingCapacityDTO
                                                        {
                                                            Id = cap.Id,
                                                            ShipId = ship.Id,
                                                            ShipCfr = ship.Cfr,
                                                            ShipName = ship.Name,
                                                            GrossTonnage = cap.GrossTonnage,
                                                            Power = cap.EnginePower,
                                                            DateOfChange = cap.ValidFrom,
                                                            IsActive = true
                                                        };

            return result;
        }

        private List<CapacityHolderHelper> GetCapacityHolderHelpers(List<int> capacityCertificateIds)
        {
            List<CapacityHolderHelper> holders = (from changeCert in Db.CapacityChangeHistoryCertificates
                                                  join cert in Db.CapacityCertificatesRegister on changeCert.CapacityCertificateId equals cert.Id
                                                  where capacityCertificateIds.Contains(cert.Id)
                                                  orderby changeCert.Id
                                                  select new CapacityHolderHelper
                                                  {
                                                      Id = changeCert.Id,
                                                      IsHolderPerson = cert.PersonId.HasValue,
                                                      PersonId = cert.PersonId,
                                                      LegalId = cert.LegalId,
                                                      TransferredTonnage = cert.GrossTonnage,
                                                      TransferredPower = cert.MainEnginePower,
                                                      IsActive = changeCert.IsActive
                                                  }).ToList();

            return holders;
        }

        private List<CapacityHolderRegixHelper> GetCapacityHolderRegixHelpers(List<int> capacityCertificateIds)
        {
            List<CapacityHolderRegixHelper> holders = (from changeCert in Db.CapacityChangeHistoryCertificates
                                                       join cert in Db.CapacityCertificatesRegister on changeCert.CapacityCertificateId equals cert.Id
                                                       where capacityCertificateIds.Contains(cert.Id)
                                                       orderby changeCert.Id
                                                       select new CapacityHolderRegixHelper
                                                       {
                                                           Id = changeCert.Id,
                                                           IsHolderPerson = cert.PersonId.HasValue,
                                                           PersonId = cert.PersonId,
                                                           LegalId = cert.LegalId,
                                                           IsActive = changeCert.IsActive
                                                       }).ToList();

            return holders;
        }

        private void SetCapacityHoldersPersonLegal(List<FishingCapacityHolderDTO> result, List<CapacityHolderHelper> holders)
        {
            List<int> personIds = holders.Where(x => x.IsHolderPerson.Value).Select(x => x.PersonId.Value).ToList();
            List<int> legalIds = holders.Where(x => !x.IsHolderPerson.Value).Select(x => x.LegalId.Value).ToList();

            CapacityHoldersRegixData data = GetCapacityHoldersRegixData(personIds, legalIds);

            foreach (FishingCapacityHolderRegixDataDTO holder in result)
            {
                if (holder.IsHolderPerson.Value)
                {
                    int personId = holders.Where(x => x.Id == holder.Id).Select(x => x.PersonId.Value).Single();
                    holder.Person = data.Persons[personId];
                    holder.Addresses = data.PersonAddresses[personId].ToList();
                }
                else
                {
                    int legalId = holders.Where(x => x.Id == holder.Id).Select(x => x.LegalId.Value).Single();
                    holder.Legal = data.Legals[legalId];
                    holder.Addresses = data.LegalAddresses[legalId].ToList();
                }
            }
        }

        private void SetCapacityHoldersPersonLegal(List<FishingCapacityHolderRegixDataDTO> result, List<CapacityHolderRegixHelper> holders)
        {
            List<int> personIds = holders.Where(x => x.IsHolderPerson.Value).Select(x => x.PersonId.Value).ToList();
            List<int> legalIds = holders.Where(x => !x.IsHolderPerson.Value).Select(x => x.LegalId.Value).ToList();

            CapacityHoldersRegixData data = GetCapacityHoldersRegixData(personIds, legalIds);

            foreach (FishingCapacityHolderRegixDataDTO holder in result)
            {
                if (holder.IsHolderPerson.Value)
                {
                    int personId = holders.Where(x => x.Id == holder.Id).Select(x => x.PersonId.Value).Single();
                    holder.Person = data.Persons[personId];
                    holder.Addresses = data.PersonAddresses[personId].ToList();
                }
                else
                {
                    int legalId = holders.Where(x => x.Id == holder.Id).Select(x => x.LegalId.Value).Single();
                    holder.Legal = data.Legals[legalId];
                    holder.Addresses = data.LegalAddresses[legalId].ToList();
                }
            }
        }

        private CapacityHoldersRegixData GetCapacityHoldersRegixData(List<int> personIds, List<int> legalIds)
        {
            Dictionary<int, RegixPersonDataDTO> personRegixData = personService.GetRegixPersonsData(personIds);
            Dictionary<int, RegixLegalDataDTO> legalRegixData = legalService.GetRegixLegalsData(legalIds);

            ILookup<int, AddressRegistrationDTO> personAddresses = personService.GetAddressRegistrations(personIds);
            ILookup<int, AddressRegistrationDTO> legalAddresses = legalService.GetAddressRegistrations(legalIds);

            CapacityHoldersRegixData result = new CapacityHoldersRegixData
            {
                Persons = personRegixData,
                Legals = legalRegixData,
                PersonAddresses = personAddresses,
                LegalAddresses = legalAddresses
            };

            return result;
        }
    }
}
