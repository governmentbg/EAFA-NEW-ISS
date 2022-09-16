using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Buyers;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Helpers;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.Applications;
using IARA.Interfaces.CatchSales;
using IARA.Interfaces.FSM;
using IARA.Interfaces.Legals;
using IARA.Interfaces.Reports;
using IARA.RegixAbstractions.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.Buyers
{
    public partial class BuyersService : Service, IBuyersService
    {
        private readonly IPersonService personService;
        private readonly ILegalService legalService;
        private readonly IApplicationService applicationService;
        private readonly IApplicationStateMachine applicationStateMachine;
        private readonly IAddressService addressService;
        private readonly ILogBooksService logBooksService;
        private readonly IDeliveryService deliveryService;
        private readonly IUsageDocumentsService usageDocumentsService;
        private readonly IRegixApplicationInterfaceService regixApplicationService;
        private readonly IChangeOfCircumstancesService changeOfCircumstancesService;
        private readonly IJasperReportExecutionService jasperReportsService;
        private readonly IDuplicatesRegisterService duplicatesRegisterService;

        public BuyersService(IARADbContext db,
                             IPersonService personService,
                             ILegalService legalService,
                             IApplicationService applicationService,
                             IApplicationStateMachine applicationStateMachine,
                             IAddressService addressService,
                             ILogBooksService logBooksService,
                             IDeliveryService deliveryService,
                             IUsageDocumentsService usageDocumentsService,
                             IRegixApplicationInterfaceService regixApplicationService,
                             IChangeOfCircumstancesService changeOfCircumstancesService,
                             IJasperReportExecutionService jasperReportsService,
                             IDuplicatesRegisterService duplicatesRegisterService)
                 : base(db)
        {
            this.personService = personService;
            this.legalService = legalService;
            this.applicationService = applicationService;
            this.applicationStateMachine = applicationStateMachine;
            this.addressService = addressService;
            this.logBooksService = logBooksService;
            this.deliveryService = deliveryService;
            this.usageDocumentsService = usageDocumentsService;
            this.regixApplicationService = regixApplicationService;
            this.changeOfCircumstancesService = changeOfCircumstancesService;
            this.jasperReportsService = jasperReportsService;
            this.duplicatesRegisterService = duplicatesRegisterService;
        }

        // Register
        public IQueryable<BuyerDTO> GetAll(BuyersFilters filters)
        {
            IQueryable<BuyerDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllNoFilter(showInactive);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetAllFreeTextFilter(filters.FreeTextSearch, filters.ShowInactiveRecords, filters.TerritoryUnitId)
                    : GetAllFilter(filters);
            }

            return result;
        }

        public BuyerEditDTO GetRegisterEntry(int id)
        {
            BuyerEditDTO result = (from buyer in Db.BuyerRegisters
                                   join status in Db.NbuyerStatuses on buyer.BuyerStatusId equals status.Id
                                   join appl in Db.Applications on buyer.ApplicationId equals appl.Id
                                   join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                   join buyerType in Db.NbuyerTypes on buyer.BuyerTypeId equals buyerType.Id
                                   where buyer.Id == id
                                   select new BuyerEditDTO
                                   {
                                       Id = buyer.Id,
                                       BuyerStatus = Enum.Parse<BuyerStatusesEnum>(status.Code),
                                       HasUtility = buyer.HasUtility,
                                       UtilityName = buyer.UtilityName,
                                       HasVehicle = buyer.HasVehicle,
                                       VehicleNumber = buyer.VehicleNumber,
                                       PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                       BuyerType = Enum.Parse<BuyerTypesEnum>(buyerType.Code),
                                       UrorrNumber = buyer.UrrorNum,
                                       RegistrationNumber = buyer.RegistrationNum,
                                       RegistrationDate = buyer.RegistrationDate,
                                       IsActive = buyer.IsActive,
                                       Comments = buyer.Comments,
                                       ApplicationId = buyer.ApplicationId,
                                       SubmittedForLegalId = buyer.SubmittedForLegalId,
                                       SubmittedForPersonId = buyer.SubmittedForPersonId,
                                       OrganizerPersonId = buyer.OrganizingPersonId,
                                       AgentId = buyer.AgentId,
                                       PremiseAddressId = buyer.UtilityAddressId,
                                       IsAgentSameAsSubmittedBy = buyer.IsAgentSameAsSubmittedBy,
                                       IsAgentSameAsSubmittedForCustodianOfProperty = buyer.IsAgentSameAsSubmittedForCustodianOfProperty,
                                       OrganizerSameAsSubmittedBy = buyer.IsOrganizingPersonSameAsSubmittedBy,
                                       TerritoryUnitId = buyer.TerritoryUnitId,
                                       AnnualTurnover = buyer.AnnualTurnoverBgn
                                   }).First();

            result.SubmittedFor = applicationService.GetRegisterSubmittedFor(result.ApplicationId, result.SubmittedForPersonId, result.SubmittedForLegalId);
            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(result.ApplicationId, ApplicationHierarchyTypesEnum.Online);

            if (result.HasUtility.HasValue && result.HasUtility.Value)
            {
                result.PremiseAddress = addressService.GetAddressRegistration(result.PremiseAddressId.Value);
                result.BabhLawLicenseDocuments = GetLicenseDocuments(result.Id.Value, BuyerLicenseTypesEnum.Food);
            }

            if (result.HasVehicle.HasValue && result.HasVehicle.Value)
            {
                result.VeteniraryVehicleRegLicenseDocuments = GetLicenseDocuments(result.Id.Value, BuyerLicenseTypesEnum.Vehicle);
            }

            if (result.PageCode == PageCodeEnum.RegFirstSaleBuyer)
            {
                result.Agent = personService.GetRegixPersonData(result.AgentId.Value);
            }
            else if (result.PageCode == PageCodeEnum.RegFirstSaleCenter)
            {
                result.Organizer = personService.GetRegixPersonData(result.OrganizerPersonId.Value);

                if (result.HasUtility.Value)
                {
                    result.PremiseUsageDocuments = GetPremiseUsageDocuments(result.Id.Value);
                }
            }

            result.LogBooks = logBooksService.GetBuyerLogBooks(id);
            result.CancellationHistory = GetBuyerStatuses(result.Id.Value);
            result.DuplicateEntries = duplicatesRegisterService.GetDuplicateEntries(buyerId: result.Id.Value);
            result.Files = Db.GetFiles(Db.BuyersRegisterFiles, result.Id.Value);

            return result;
        }

        public int AddRegisterEntry(BuyerEditDTO buyer, bool ignoreLogBookConflicts)
        {
            BuyerRegister dbBuyer;

            using (TransactionScope scope = new TransactionScope())
            {
                Application application = (from appl in Db.Applications
                                           where appl.Id == buyer.ApplicationId
                                           select appl).First();

                int buyerTypeId = GetTypeIdByPageCode(buyer.PageCode);
                string urorrNumber = GenerateUrorrNumber(buyer.TerritoryUnitId.Value);

                dbBuyer = new BuyerRegister
                {
                    ApplicationId = buyer.ApplicationId,
                    TerritoryUnitId = buyer.TerritoryUnitId.Value,
                    BuyerTypeId = buyerTypeId,
                    RecordType = nameof(RecordTypesEnum.Register),
                    RegistrationDate = buyer.RegistrationDate.Value,
                    UrrorNum = urorrNumber,
                    Comments = buyer.Comments,
                    AnnualTurnoverBgn = buyer.AnnualTurnover
                };

                dbBuyer.BuyerStatusId = GetBuyerStatusId(buyer.BuyerStatus.Value);

                int registerApplicationId = (from b in Db.BuyerRegisters
                                             where b.ApplicationId == buyer.ApplicationId
                                                   && b.RecordType == nameof(RecordTypesEnum.Application)
                                             select b.Id).Single();

                dbBuyer.RegisterApplicationId = registerApplicationId;

                Db.AddOrEditRegisterSubmittedFor(dbBuyer, buyer.SubmittedFor);
                Db.SaveChanges();

                Db.BuyerRegisters.Add(dbBuyer);
                Db.SaveChanges();

                dbBuyer.HasUtility = buyer.HasUtility.Value;
                dbBuyer.HasVehicle = buyer.HasVehicle.Value;

                if (buyer.HasUtility.Value)
                {
                    dbBuyer.UtilityName = buyer.UtilityName;
                    Address premiseAddress = Db.AddOrEditAddress(buyer.PremiseAddress, true);
                    dbBuyer.UtilityAddress = premiseAddress;

                    foreach (CommonDocumentDTO document in buyer.BabhLawLicenseDocuments)
                    {
                        AddLicenseDocument(document, dbBuyer.Id, BuyerLicenseTypesEnum.Food);
                    }
                }
                else
                {
                    dbBuyer.UtilityName = null;
                }

                if (buyer.HasVehicle.Value)
                {
                    dbBuyer.VehicleNumber = buyer.VehicleNumber;

                    foreach (CommonDocumentDTO document in buyer.VeteniraryVehicleRegLicenseDocuments)
                    {
                        AddLicenseDocument(document, dbBuyer.Id, BuyerLicenseTypesEnum.Vehicle);
                    }
                }
                else
                {
                    dbBuyer.VehicleNumber = null;
                }

                if (buyer.PageCode == PageCodeEnum.RegFirstSaleBuyer)
                {
                    Person agent = Db.AddOrEditPerson(buyer.Agent);
                    dbBuyer.Agent = agent;
                    Db.SaveChanges();
                    dbBuyer.IsAgentSameAsSubmittedBy = buyer.IsAgentSameAsSubmittedBy;
                    dbBuyer.IsAgentSameAsSubmittedForCustodianOfProperty = buyer.IsAgentSameAsSubmittedForCustodianOfProperty;
                }
                else if (buyer.PageCode == PageCodeEnum.RegFirstSaleCenter)
                {
                    Person organizer = Db.AddOrEditPerson(buyer.Organizer);
                    dbBuyer.OrganizingPerson = organizer;
                    Db.SaveChanges();
                    dbBuyer.IsOrganizingPersonSameAsSubmittedBy = buyer.OrganizerSameAsSubmittedBy;

                    if (buyer.HasUtility.Value)
                    {
                        AddPremiseUsageDocuments(dbBuyer, buyer.PremiseUsageDocuments);
                    }
                }

                if (buyer.LogBooks != null)
                {
                    AddOrEditLogBooks(buyer.LogBooks, dbBuyer.Id, ignoreLogBookConflicts);
                }

                if (buyer.Files != null)
                {
                    foreach (FileInfoDTO file in buyer.Files)
                    {
                        Db.AddOrEditFile(dbBuyer, dbBuyer.BuyerRegisterFiles, file);
                    }
                }

                Db.SaveChanges();

                applicationStateMachine.Act(dbBuyer.ApplicationId);

                scope.Complete();
            }

            return dbBuyer.Id;
        }

        public void EditRegisterEntry(BuyerEditDTO buyer, bool ignoreLogBookConflicts)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                BuyerRegister dbBuyer = (from b in Db.BuyerRegisters
                                                 .AsSplitQuery()
                                                 .Include(x => x.BuyerRegisterFiles)
                                         where b.Id == buyer.Id.Value
                                         select b).First();

                dbBuyer.BuyerStatusId = GetBuyerStatusId(buyer.BuyerStatus.Value);
                dbBuyer.UtilityName = buyer.UtilityName;
                dbBuyer.RegistrationDate = buyer.RegistrationDate.Value;
                dbBuyer.TerritoryUnitId = buyer.TerritoryUnitId.Value;
                dbBuyer.Comments = buyer.Comments;
                dbBuyer.AnnualTurnoverBgn = buyer.AnnualTurnover;

                Db.AddOrEditRegisterSubmittedFor(dbBuyer, buyer.SubmittedFor);

                dbBuyer.HasUtility = buyer.HasUtility.Value;
                dbBuyer.HasVehicle = buyer.HasVehicle.Value;

                if (buyer.HasUtility.Value)
                {
                    dbBuyer.UtilityName = buyer.UtilityName;
                    dbBuyer.UtilityAddress = Db.AddOrEditAddress(buyer.PremiseAddress, true, dbBuyer.UtilityAddressId);

                    foreach (CommonDocumentDTO document in buyer.BabhLawLicenseDocuments)
                    {
                        AddOrEditLicenseDocument(document, dbBuyer.Id, BuyerLicenseTypesEnum.Food);
                    }
                }
                else
                {
                    dbBuyer.UtilityName = null;
                    dbBuyer.UtilityAddressId = null;
                }

                if (buyer.HasVehicle.Value)
                {
                    dbBuyer.VehicleNumber = buyer.VehicleNumber;

                    foreach (CommonDocumentDTO document in buyer.VeteniraryVehicleRegLicenseDocuments)
                    {
                        AddOrEditLicenseDocument(document, dbBuyer.Id, BuyerLicenseTypesEnum.Vehicle);
                    }
                }
                else
                {
                    dbBuyer.VehicleNumber = null;
                }

                if (buyer.PageCode == PageCodeEnum.RegFirstSaleBuyer)
                {
                    dbBuyer.Agent = Db.AddOrEditPerson(buyer.Agent, null, dbBuyer.AgentId);
                    Db.SaveChanges();
                    dbBuyer.IsAgentSameAsSubmittedBy = buyer.IsAgentSameAsSubmittedBy;
                    dbBuyer.IsAgentSameAsSubmittedForCustodianOfProperty = buyer.IsAgentSameAsSubmittedForCustodianOfProperty;
                }
                else if (buyer.PageCode == PageCodeEnum.RegFirstSaleCenter)
                {
                    dbBuyer.OrganizingPerson = Db.AddOrEditPerson(buyer.Organizer, null, dbBuyer.OrganizingPersonId);
                    Db.SaveChanges();
                    dbBuyer.IsOrganizingPersonSameAsSubmittedBy = buyer.OrganizerSameAsSubmittedBy;

                    if (buyer.HasUtility.Value)
                    {
                        AddOrEditPremiseUsageDocuments(dbBuyer, buyer.PremiseUsageDocuments);
                    }
                }

                if (buyer.LogBooks != null)
                {
                    AddOrEditLogBooks(buyer.LogBooks, dbBuyer.Id, ignoreLogBookConflicts);
                }

                if (buyer.Files != null)
                {
                    foreach (FileInfoDTO file in buyer.Files)
                    {
                        Db.AddOrEditFile(dbBuyer, dbBuyer.BuyerRegisterFiles, file);
                    }
                }

                Db.SaveChanges();
                scope.Complete();
            }
        }

        public void Delete(int id)
        {
            DeleteRecordWithId(Db.BuyerRegisters, id);
            Db.SaveChanges();
        }

        public void Restore(int id)
        {
            UndoDeleteRecordWithId(Db.BuyerRegisters, id);
            Db.SaveChanges();
        }

        public void UpdateBuyerStatus(int buyerId, CancellationHistoryEntryDTO status, int? applicationId)
        {
            DateTime now = DateTime.Now;

            BuyerRegisterStatus lastStatus = (from bs in Db.BuyerRegisterStatuses
                                              where bs.BuyersRegisterId == buyerId
                                                    && bs.ValidFrom <= now
                                                    && bs.ValidTo > now
                                              select bs).FirstOrDefault();

            if (lastStatus != null)
            {
                lastStatus.ValidTo = now;
            }

            BuyerRegisterStatus entry = new BuyerRegisterStatus
            {
                BuyersRegisterId = buyerId,
                IsCancelled = status.IsCancelled.Value,
                CancellationReasonId = status.CancellationReasonId.Value,
                DateOfChange = now,
                IssueOrderNum = status.IssueOrderNum,
                Description = status.Description,
                ValidFrom = now,
                ValidTo = DefaultConstants.MAX_VALID_DATE
            };

            BuyerRegister buyer = (from buy in Db.BuyerRegisters
                                   where buy.Id == buyerId
                                   select buy).First();

            if (status.IsCancelled.Value)
            {
                buyer.BuyerStatusId = GetBuyerStatusId(BuyerStatusesEnum.Canceled);
            }
            else
            {
                buyer.BuyerStatusId = GetBuyerStatusId(BuyerStatusesEnum.Active);
            }

            Db.BuyerRegisterStatuses.Add(entry);

            // complete deregistration application
            if (applicationId.HasValue)
            {
                applicationStateMachine.Act(applicationId.Value);
            }

            Db.SaveChanges();
        }

        public async Task<DownloadableFileDTO> GetRegisterFileForDownload(int registerId, BuyerTypesEnum buyerType, bool duplicate = false)
        {
            DownloadableFileDTO downloadableFile = new DownloadableFileDTO();
            downloadableFile.MimeType = "application/pdf";

            var buyerData = (from buyer in Db.BuyerRegisters
                             where buyer.Id == registerId
                             select new
                             {
                                 buyer.HasUtility,
                                 buyer.HasVehicle,
                                 buyer.UtilityName,
                                 buyer.VehicleNumber
                             }).First();

            StringBuilder submittedForNameBuilder = new StringBuilder();

            if (buyerData.HasUtility.HasValue && buyerData.HasUtility.Value)
            {
                submittedForNameBuilder.Append($"{buyerData.UtilityName}");
            }

            if (buyerData.HasVehicle.HasValue && buyerData.HasVehicle.Value)
            {
                if (buyerData.HasUtility.HasValue && buyerData.HasUtility.Value)
                {
                    submittedForNameBuilder.Append("_");
                }

                submittedForNameBuilder.Append($"{buyerData.VehicleNumber}");
            }

            switch (buyerType)
            {
                case BuyerTypesEnum.Buyer:
                    {
                        if (duplicate)
                        {
                            downloadableFile.FileName = $"Дубликат-на-регистриран-купувач-за-първа-продажба_{submittedForNameBuilder.ToString()}.pdf".Replace("  ", "");
                        }
                        else
                        {
                            downloadableFile.FileName = $"Регистриран-купувач-за-първа-продажба_{submittedForNameBuilder.ToString()}.pdf".Replace("  ", "");
                        }

                        downloadableFile.Bytes = await jasperReportsService.GetFirstSaleBuyerRegister(registerId, duplicate);
                    }
                    break;
                case BuyerTypesEnum.CPP:
                    {
                        if (duplicate)
                        {
                            downloadableFile.FileName = $"Дубликат-на-регистриран-център-за-първа-продажба_{submittedForNameBuilder.ToString()}.pdf".Replace("  ", "");
                        }
                        else
                        {
                            downloadableFile.FileName = $"Регистриран-център-за-първа-продажба_{submittedForNameBuilder.ToString()}.pdf".Replace("  ", "");
                        }

                        downloadableFile.Bytes = await jasperReportsService.GetFirstSaleCenterRegister(registerId, duplicate);
                    }
                    break;
            }

            return downloadableFile;
        }

        // Audits
        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.BuyerRegisters, id);
            return audit;
        }

        public SimpleAuditDTO GetPremiseUsageDocumentSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.BuyerPremiseUsageDocuments, id);
            return audit;
        }

        public SimpleAuditDTO GetBuyerLicenseSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.BuyerLicenses, id);
            return audit;
        }

        // nomenclatures
        public IEnumerable<NomenclatureDTO> GetEntityTypes()
        {
            List<NomenclatureDTO> result = GetCodeNomenclature(Db.NbuyerTypes);
            return result;
        }

        public IEnumerable<NomenclatureDTO> GetBuyerStatuses()
        {
            List<NomenclatureDTO> results = GetCodeNomenclature(Db.NbuyerStatuses);
            return results;
        }

        public IEnumerable<NomenclatureDTO> GetAllBuyersNomenclatures()
        {
            List<NomenclatureDTO> results = (from buyer in Db.BuyerRegisters
                                             join buyerType in Db.NbuyerTypes on buyer.BuyerTypeId equals buyerType.Id
                                             join legal in Db.Legals on buyer.SubmittedForLegalId equals legal.Id into buyerLegal
                                             from legal in buyerLegal.DefaultIfEmpty()
                                             join person in Db.Persons on buyer.SubmittedForPersonId equals person.Id into buyerPerson
                                             from person in buyerPerson.DefaultIfEmpty()
                                             where buyer.RecordType == nameof(RecordTypesEnum.Register)
                                                && buyerType.Code == nameof(BuyerTypesEnum.Buyer)
                                             orderby buyer.RegistrationDate descending
                                             select new NomenclatureDTO
                                             {
                                                 Value = buyer.Id,
                                                 DisplayName = buyer.HasUtility.HasValue && buyer.HasUtility.Value
                                                               ? $"{buyer.UrrorNum} - {buyer.UtilityName}"
                                                               : $"{buyer.UrrorNum} - {buyer.VehicleNumber}",
                                                 Description = legal != null
                                                               ? $"{legal.Name} ({legal.Eik})"
                                                               : $"{person.FirstName} {person.LastName}",
                                                 IsActive = buyer.IsActive
                                             }).ToList();

            return results;
        }

        public IEnumerable<NomenclatureDTO> GetAllFirstSaleCentersNomenclatures()
        {
            List<NomenclatureDTO> results = (from buyer in Db.BuyerRegisters
                                             join buyerType in Db.NbuyerTypes on buyer.BuyerTypeId equals buyerType.Id
                                             join legal in Db.Legals on buyer.SubmittedForLegalId equals legal.Id into buyerLegal
                                             from legal in buyerLegal.DefaultIfEmpty()
                                             join person in Db.Persons on buyer.SubmittedForPersonId equals person.Id into buyerPerson
                                             from person in buyerPerson.DefaultIfEmpty()
                                             where buyer.RecordType == nameof(RecordTypesEnum.Register)
                                                && buyerType.Code == nameof(BuyerTypesEnum.CPP)
                                             orderby buyer.RegistrationDate descending
                                             select new NomenclatureDTO
                                             {
                                                 Value = buyer.Id,
                                                 DisplayName = $"{buyer.UrrorNum} - {buyer.UtilityName}",
                                                 Description = legal != null
                                                               ? $"{legal.Name} ({legal.Eik})"
                                                               : $"{person.FirstName} {person.LastName}",
                                                 IsActive = buyer.IsActive
                                             }).ToList();

            return results;
        }

        // register
        private IQueryable<BuyerDTO> GetAllNoFilter(bool showInactive)
        {
            var result = from buyer in Db.BuyerRegisters
                         join status in Db.NbuyerStatuses on buyer.BuyerStatusId equals status.Id
                         join buyerType in Db.NbuyerTypes on buyer.BuyerTypeId equals buyerType.Id
                         join submittedForLegal in Db.Legals on buyer.SubmittedForLegalId equals submittedForLegal.Id into subLegal
                         from submittedForLegal in subLegal.DefaultIfEmpty()
                         join submittedForPerson in Db.Persons on buyer.SubmittedForPersonId equals submittedForPerson.Id into subPerson
                         from submittedForPerson in subPerson.DefaultIfEmpty()
                         join appl in Db.Applications on buyer.ApplicationId equals appl.Id
                         join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                         where buyer.IsActive == !showInactive && buyer.RecordType == nameof(RecordTypesEnum.Register)
                         orderby buyer.RegistrationDate descending
                         select new BuyerDTO
                         {
                             Id = buyer.Id,
                             Status = Enum.Parse<BuyerStatusesEnum>(status.Code),
                             SubmittedForName = submittedForLegal != null
                                                ? submittedForLegal.Name + " (" + submittedForLegal.Eik + ")"
                                                : submittedForPerson.FirstName + " " + submittedForPerson.LastName,
                             SubjectNames = buyer.HasUtility.HasValue && buyer.HasUtility.Value
                                            ? buyer.HasVehicle.HasValue && buyer.HasVehicle.Value
                                                ? buyer.UtilityName + ", " + buyer.VehicleNumber
                                                : buyer.UtilityName
                                            : buyer.VehicleNumber,
                             ApplicationId = appl.Id,
                             PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                             BuyerStatusName = status.Name,
                             BuyerTypeName = buyerType.Name,
                             Comments = buyer.Comments,
                             RegistrationDate = buyer.RegistrationDate.Value,
                             UrorrNumber = buyer.UrrorNum,
                             RegistrationNumber = buyer.RegistrationNum,
                             IsActive = buyer.IsActive
                         };

            return result;
        }

        private IQueryable<BuyerDTO> GetAllFilter(BuyersFilters filters)
        {
            var query = from buyer in Db.BuyerRegisters
                        join status in Db.NbuyerStatuses on buyer.BuyerStatusId equals status.Id
                        join buyerType in Db.NbuyerTypes on buyer.BuyerTypeId equals buyerType.Id
                        join appl in Db.Applications on buyer.ApplicationId equals appl.Id
                        join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                        join legalOwner in Db.Legals on buyer.SubmittedForLegalId equals legalOwner.Id into submittedForLegal
                        from legalOwner in submittedForLegal.DefaultIfEmpty()
                        join personOwner in Db.Persons on buyer.SubmittedForPersonId equals personOwner.Id into submittedForPerson
                        from personOwner in submittedForPerson.DefaultIfEmpty()
                        join organizer in Db.Persons on buyer.OrganizingPersonId equals organizer.Id into cppOrganizer
                        from organizer in cppOrganizer.DefaultIfEmpty()
                        join organizerAddress in Db.Addresses on buyer.UtilityAddressId equals organizerAddress.Id into cppOrganizerAddress
                        from organizerAddress in cppOrganizerAddress.DefaultIfEmpty()
                        join utilityAddress in Db.Addresses on buyer.UtilityAddressId equals utilityAddress.Id into uAddr
                        from utilityAddress in uAddr.DefaultIfEmpty()
                        where buyer.IsActive == !filters.ShowInactiveRecords && buyer.RecordType == nameof(RecordTypesEnum.Register)
                        select new
                        {
                            Id = buyer.Id,
                            PageCode = applType.PageCode,
                            TerritoryUnitId = buyer.TerritoryUnitId,
                            ApplicationId = appl.Id,
                            StatusId = status.Id,
                            Status = Enum.Parse<BuyerStatusesEnum>(status.Code),
                            SubmittedForName = legalOwner != null
                                                ? legalOwner.Name + " (" + legalOwner.Eik + ")"
                                                : personOwner.FirstName + " " + personOwner.LastName,
                            SubjectNames = buyer.HasUtility.HasValue && buyer.HasUtility.Value
                                            ? buyer.HasVehicle.HasValue && buyer.HasVehicle.Value
                                                ? buyer.UtilityName + ", " + buyer.VehicleNumber
                                                : buyer.UtilityName
                                            : buyer.VehicleNumber,
                            OrganizerPersonNames = organizer != null ? organizer.FirstName + " " + organizer.MiddleName + " " + organizer.LastName : "",
                            OrganizerPersonEgnLnch = organizer != null ? organizer.EgnLnc : "",
                            UnitilityAddressId = buyer.UtilityAddressId,
                            UtilitityAddressStreet = utilityAddress != null ? utilityAddress.Street : "",
                            LegalOwnerName = legalOwner != null ? legalOwner.Name : "",
                            LegalEik = legalOwner != null ? legalOwner.Eik : "",
                            PersonOwnerName = personOwner != null ? personOwner.FirstName + " " + personOwner.MiddleName + " " + personOwner.LastName : "",
                            EntryTypeId = buyerType.Id,
                            EntryTypeName = buyerType.Name,
                            BuyerStatusName = status.Name,
                            Comments = buyer.Comments,
                            RegistrationDate = buyer.RegistrationDate.Value,
                            UrorrNumber = buyer.UrrorNum,
                            RegistrationNumber = buyer.RegistrationNum,
                            PopulatedAreaId = utilityAddress != null ? utilityAddress.PopulatedAreaId : default,
                            DistrictId = utilityAddress != null ? utilityAddress.DistrictId : default,
                            AgentId = buyer.AgentId,
                            OrganizerPersonId = buyer.OrganizingPersonId,
                            SubmittedForPersonId = buyer.SubmittedForPersonId,
                            SubmittedForLegalId = buyer.SubmittedForLegalId,
                            IsActive = buyer.IsActive
                        };

            if (filters.EntryTypeId.HasValue)
            {
                query = from buyer in query
                        where buyer.EntryTypeId == filters.EntryTypeId
                        select buyer;
            }

            if (!string.IsNullOrEmpty(filters.UrorrNumber))
            {
                query = from buyer in query
                        where buyer.UrorrNumber.ToLower().Replace(" ", "").Contains(filters.UrorrNumber.ToLower().Replace(" ", ""))
                        select buyer;
            }

            if (!string.IsNullOrEmpty(filters.RegistrationNumber))
            {
                query = from buyer in query
                        where buyer.RegistrationNumber.ToLower().Contains(filters.RegistrationNumber.ToLower())
                        select buyer;
            }

            if (!string.IsNullOrEmpty(filters.UtilityName))
            {
                query = from buyer in query
                        where buyer.SubjectNames.ToLower().Contains(filters.UtilityName.ToLower())
                        select buyer;
            }

            if (!string.IsNullOrEmpty(filters.OwnerName))
            {
                query = from buyer in query
                        where buyer.LegalOwnerName.ToLower().Contains(filters.OwnerName.ToLower())
                              || buyer.PersonOwnerName.ToLower().Contains(filters.OwnerName.ToLower())
                        select buyer;
            }

            if (!string.IsNullOrEmpty(filters.OwnerEIK))
            {
                query = from buyer in query
                        where buyer.LegalEik == filters.OwnerEIK
                        select buyer;
            }

            if (!string.IsNullOrEmpty(filters.OrganizerName))
            {
                query = from buyer in query
                        where buyer.OrganizerPersonNames.ToLower().Contains(filters.OrganizerName.ToLower())
                        select buyer;
            }

            if (!string.IsNullOrEmpty(filters.OrganizerEGN))
            {
                query = from buyer in query
                        where buyer.OrganizerPersonEgnLnch == filters.OrganizerEGN
                        select buyer;
            }

            if (!string.IsNullOrEmpty(filters.LogBookNumber))
            {
                query = from buyer in query
                        join logBook in Db.LogBooks on buyer.Id equals logBook.RegisteredBuyerId
                        where logBook.LogNum.ToLower().Contains(filters.LogBookNumber.ToLower())
                        select buyer;
            }

            if (filters.RegisteredDateFrom != null && filters.RegisteredDateTo != null)
            {
                query = from buyer in query
                        where buyer.RegistrationDate >= filters.RegisteredDateFrom && buyer.RegistrationDate <= filters.RegisteredDateTo
                        select buyer;
            }

            if (filters.PopulatedAreaId.HasValue)
            {
                query = from buyer in query
                        where buyer.PopulatedAreaId == filters.PopulatedAreaId.Value
                        select buyer;
            }

            if (filters.DistrictId.HasValue)
            {
                query = from buyer in query
                        where buyer.DistrictId == filters.DistrictId.Value
                        select buyer;
            }

            if (filters.TerritoryUnitId.HasValue)
            {
                query = from buyer in query
                        where buyer.TerritoryUnitId == filters.TerritoryUnitId.Value
                        select buyer;
            }

            if (filters.StatusIds != null && filters.StatusIds.Count > 0)
            {
                query = from buyer in query
                        where filters.StatusIds.Contains(buyer.StatusId)
                        select buyer;
            }

            if (filters.PersonId.HasValue)
            {
                query = from buyer in query
                        where buyer.AgentId == filters.PersonId
                            || buyer.OrganizerPersonId == filters.PersonId
                            || buyer.SubmittedForPersonId == filters.PersonId
                            || buyer.SubmittedForLegalId == filters.PersonId
                        select buyer;
            }

            if (filters.LegalId.HasValue)
            {
                query = from buyer in query
                        where buyer.SubmittedForLegalId == filters.LegalId
                        select buyer;
            }

            IQueryable<BuyerDTO> results = from buyer in query
                                           orderby buyer.RegistrationDate descending
                                           select new BuyerDTO
                                           {
                                               Id = buyer.Id,
                                               Status = buyer.Status,
                                               SubjectNames = buyer.SubjectNames,
                                               SubmittedForName = buyer.SubmittedForName,
                                               ApplicationId = buyer.ApplicationId,
                                               PageCode = Enum.Parse<PageCodeEnum>(buyer.PageCode),
                                               BuyerTypeName = buyer.EntryTypeName,
                                               BuyerStatusName = buyer.BuyerStatusName,
                                               RegistrationDate = buyer.RegistrationDate,
                                               UrorrNumber = buyer.UrorrNumber,
                                               RegistrationNumber = buyer.RegistrationNumber,
                                               Comments = buyer.Comments,
                                               IsActive = buyer.IsActive
                                           };

            return results;
        }

        private IQueryable<BuyerDTO> GetAllFreeTextFilter(string text, bool isActive, int? territoryUnitId)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            IQueryable<BuyerDTO> result = from buyer in Db.BuyerRegisters
                                          join buyerType in Db.NbuyerTypes on buyer.BuyerTypeId equals buyerType.Id
                                          join submittedForLegal in Db.Legals on buyer.SubmittedForLegalId equals submittedForLegal.Id into subLegal
                                          from submittedForLegal in subLegal.DefaultIfEmpty()
                                          join submittedForPerson in Db.Persons on buyer.SubmittedForPersonId equals submittedForPerson.Id into subPerson
                                          from submittedForPerson in subPerson.DefaultIfEmpty()
                                          join appl in Db.Applications on buyer.ApplicationId equals appl.Id
                                          join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                          join status in Db.NbuyerStatuses on buyer.BuyerStatusId equals status.Id
                                          orderby buyer.RegistrationDate descending
                                          where buyer.IsActive == isActive && buyer.RecordType == nameof(RecordTypesEnum.Register)
                                                && (buyerType.Name.ToLower().Contains(text)
                                                    || buyer.Comments.ToLower().Contains(text)
                                                    || (searchDate.HasValue && buyer.RegistrationDate.Value.Date == searchDate.Value.Date)
                                                    || buyer.UrrorNum.ToLower().Replace(" ", "").Contains(text.Replace(" ", ""))
                                                    || buyer.RegistrationNum.ToLower().Contains(text)
                                                    || (buyer.HasUtility.HasValue && buyer.HasUtility.Value && buyer.UtilityName.ToLower().Contains(text))
                                                    || (buyer.HasVehicle.HasValue && buyer.HasVehicle.Value && buyer.VehicleNumber.ToLower().Contains(text))
                                                    || (submittedForLegal != null && submittedForLegal.Name.ToLower().Contains(text))
                                                    || (submittedForLegal != null && submittedForLegal.Eik.ToLower().Contains(text))
                                                    || (submittedForPerson != null && submittedForPerson.FirstName.ToLower().Contains(text))
                                                    || (submittedForPerson != null && submittedForPerson.LastName.ToLower().Contains(text))
                                                    || status.Name.ToLower().Contains(text)
                                                   )
                                                && (!territoryUnitId.HasValue || buyer.TerritoryUnitId == territoryUnitId.Value)
                                          select new BuyerDTO
                                          {
                                              Id = buyer.Id,
                                              Status = Enum.Parse<BuyerStatusesEnum>(status.Code),
                                              SubmittedForName = submittedForLegal != null
                                                                 ? submittedForLegal.Name + " (" + submittedForLegal.Eik + ")"
                                                                 : submittedForPerson.FirstName + " " + submittedForPerson.LastName,
                                              SubjectNames = buyer.HasUtility.HasValue && buyer.HasUtility.Value
                                                             ? buyer.HasVehicle.HasValue && buyer.HasVehicle.Value
                                                                ? buyer.UtilityName + ", " + buyer.VehicleNumber
                                                                : buyer.UtilityName
                                                             : buyer.VehicleNumber,
                                              ApplicationId = appl.Id,
                                              PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                              BuyerTypeName = buyerType.Name,
                                              BuyerStatusName = status.Name,
                                              Comments = buyer.Comments,
                                              RegistrationDate = buyer.RegistrationDate.Value,
                                              UrorrNumber = buyer.UrrorNum,
                                              RegistrationNumber = buyer.RegistrationNum,
                                              IsActive = buyer.IsActive
                                          };
            return result;
        }

        private void AddOrEditLogBooks(List<LogBookEditDTO> logBooks, int buyerId, bool ignoreLogBookConflicts)
        {
            foreach (LogBookEditDTO logBook in logBooks)
            {
                if (logBook.LogBookId.HasValue) // edit existing log book
                {
                    Db.EditRegisteredBuyerLogBook(logBook, ignoreLogBookConflicts);
                }
                else // add new log book
                {
                    LogBookTypesEnum logBookType = (from lBType in Db.NlogBookTypes
                                                    where lBType.Id == logBook.LogBookTypeId
                                                    select Enum.Parse<LogBookTypesEnum>(lBType.Code)).First();
                    switch (logBookType)
                    {
                        case LogBookTypesEnum.FirstSale:
                            Db.AddFirstSaleLogBook(logBook, buyerId, ignoreLogBookConflicts);
                            break;
                        case LogBookTypesEnum.Admission:
                            Db.AddAdmissionLogBook(logBook, ignoreLogBookConflicts, buyerId: buyerId, legalId: null, personId: null);
                            break;
                        case LogBookTypesEnum.Transportation:
                            Db.AddTransportationLogBook(logBook, ignoreLogBookConflicts, buyerId, legalId: null, personId: null);
                            break;
                    }
                }
            }
        }

        private List<CommonDocumentDTO> GetLicenseDocuments(int buyerId, BuyerLicenseTypesEnum buyerLicenseType)
        {
            List<CommonDocumentDTO> documents = (from license in Db.BuyerLicenses
                                                 join licenseType in Db.NbuyerLicenseTypes on license.BuyerLicenceTypeId equals licenseType.Id
                                                 where license.BuyerId == buyerId && licenseType.Code == buyerLicenseType.ToString() && license.IsActive
                                                 select new CommonDocumentDTO
                                                 {
                                                     Id = license.Id,
                                                     IssueDate = license.IssueDate,
                                                     Issuer = license.DocIssuer,
                                                     Num = license.DocNum,
                                                     ValidFrom = license.DocValidFrom,
                                                     ValidTo = license.DocValidTo,
                                                     IsIndefinite = license.IsUnlimited,
                                                     Comments = license.Comment,
                                                     IsActive = license.IsActive
                                                 }).ToList();

            return documents;
        }

        private void AddLicenseDocument(CommonDocumentDTO document, int buyerId, BuyerLicenseTypesEnum buyerLicenseType)
        {
            DateTime now = DateTime.Now;

            int buyerLicenseTypeId = (from licenseType in Db.NbuyerLicenseTypes
                                      where licenseType.Code == buyerLicenseType.ToString()
                                            && licenseType.ValidFrom <= now
                                            && licenseType.ValidTo > now
                                      select licenseType.Id).Single();

            BuyerLicense dbLicense = new BuyerLicense
            {
                BuyerId = buyerId,
                BuyerLicenceTypeId = buyerLicenseTypeId,
                IssueDate = document.IssueDate.Value,
                DocIssuer = document.Issuer,
                DocNum = document.Num,
                DocValidFrom = document.ValidFrom.Value,
                DocValidTo = document.ValidTo,
                IsUnlimited = document.IsIndefinite.Value,
                Comment = document.Comments
            };

            Db.BuyerLicenses.Add(dbLicense);
        }

        private void AddOrEditLicenseDocument(CommonDocumentDTO document, int buyerId, BuyerLicenseTypesEnum buyerLicenseType)
        {
            if (document.Id.HasValue) // edit
            {
                BuyerLicense dbLicense = (from license in Db.BuyerLicenses
                                          where license.Id == document.Id
                                          select license).First();

                dbLicense.IssueDate = document.IssueDate.Value;
                dbLicense.DocIssuer = document.Issuer;
                dbLicense.DocNum = document.Num;
                dbLicense.DocValidFrom = document.ValidFrom.Value;
                dbLicense.DocValidTo = document.ValidTo;
                dbLicense.IsUnlimited = document.IsIndefinite.Value;
                dbLicense.Comment = document.Comments;
                dbLicense.IsActive = document.IsActive.Value;
            }
            else // add
            {
                AddLicenseDocument(document, buyerId, buyerLicenseType);
            }
        }

        private List<UsageDocumentDTO> GetPremiseUsageDocuments(int buyerId)
        {
            Dictionary<int, bool> ids = (from buyerDoc in Db.BuyerPremiseUsageDocuments
                                         where buyerDoc.BuyerId == buyerId
                                         select new
                                         {
                                             buyerDoc.UsageDocumentId,
                                             buyerDoc.IsActive
                                         }).ToDictionary(x => x.UsageDocumentId, y => y.IsActive);

            List<UsageDocumentDTO> result = usageDocumentsService.GetUsageDocuments(ids.Keys.ToList());

            foreach (UsageDocumentDTO document in result)
            {
                document.IsActive = ids[document.Id.Value];
            }

            return result;
        }

        private void AddPremiseUsageDocuments(BuyerRegister dbBuyer, List<UsageDocumentDTO> usageDocuments)
        {
            foreach (UsageDocumentDTO document in usageDocuments)
            {
                UsageDocument newDocument = Db.AddUsageDocument(document, dbBuyer);

                BuyerPremiseUsageDocument entry = new BuyerPremiseUsageDocument
                {
                    Buyer = dbBuyer,
                    UsageDocument = newDocument,
                    IsActive = document.IsActive.Value
                };

                Db.BuyerPremiseUsageDocuments.Add(entry);
            }
        }

        private void AddOrEditPremiseUsageDocuments(BuyerRegister dbBuyer, List<UsageDocumentDTO> usageDocuments)
        {
            List<BuyerPremiseUsageDocument> currentUsageDocuments = (from usgDoc in Db.BuyerPremiseUsageDocuments
                                                                     where usgDoc.BuyerId == dbBuyer.Id
                                                                     select usgDoc).ToList();

            foreach (BuyerPremiseUsageDocument document in currentUsageDocuments)
            {
                document.IsActive = false;
            }

            if (usageDocuments != null)
            {
                foreach (UsageDocumentDTO document in usageDocuments)
                {
                    if (document.Id != null)
                    {
                        BuyerPremiseUsageDocument dbDoc = currentUsageDocuments.Where(x => x.UsageDocumentId == document.Id).Single();
                        dbDoc.UsageDocument = Db.EditUsageDocument(document, dbBuyer);
                        dbDoc.IsActive = document.IsActive.Value;
                    }
                    else
                    {
                        UsageDocument newDocument = Db.AddUsageDocument(document, dbBuyer);

                        BuyerPremiseUsageDocument entry = new BuyerPremiseUsageDocument
                        {
                            Buyer = dbBuyer,
                            UsageDocument = newDocument
                        };

                        Db.BuyerPremiseUsageDocuments.Add(entry);
                    }
                }
            }
        }

        private List<CancellationHistoryEntryDTO> GetBuyerStatuses(int buyerId)
        {
            List<CancellationHistoryEntryDTO> result = (from status in Db.BuyerRegisterStatuses
                                                        where status.BuyersRegisterId == buyerId
                                                        orderby status.DateOfChange descending
                                                        select new CancellationHistoryEntryDTO
                                                        {
                                                            IsCancelled = status.IsCancelled,
                                                            CancellationReasonId = status.CancellationReasonId,
                                                            DateOfChange = status.DateOfChange,
                                                            IssueOrderNum = status.IssueOrderNum,
                                                            Description = status.Description
                                                        }).ToList();

            return result;
        }

        // other
        private string GenerateUrorrNumber(int territoryUnitId)
        {
            NterritoryUnit territory = (from terr in Db.NterritoryUnits
                                        where terr.Id == territoryUnitId
                                        select terr).First();

            territory.BuyersRegisterSequence++;

            string sequenceNumber = territory.BuyersRegisterSequence.ToString("D6");

            return $"{territory.Code} {sequenceNumber.Substring(0, 1)} {sequenceNumber.Substring(1)}";
        }

        private int GetTypeIdByPageCode(PageCodeEnum pageCode)
        {
            return pageCode switch
            {
                PageCodeEnum.RegFirstSaleBuyer => GetBuyerTypeIdByCode(nameof(BuyerTypesEnum.Buyer)),
                PageCodeEnum.RegFirstSaleCenter => GetBuyerTypeIdByCode(nameof(BuyerTypesEnum.CPP)),
                _ => throw new ArgumentException($"Unexpected application page code {pageCode}")
            };
        }

        private int GetBuyerTypeIdByCode(string code)
        {
            DateTime now = DateTime.Now;

            int typeId = (from type in Db.NbuyerTypes
                          where type.Code == code && type.ValidFrom <= now && type.ValidTo > now
                          select type.Id).Single();

            return typeId;
        }

        private int GetBuyerStatusId(BuyerStatusesEnum status)
        {
            DateTime now = DateTime.Now;

            int id = (from stat in Db.NbuyerStatuses
                      where stat.Code == status.ToString() && stat.ValidFrom <= now && stat.ValidTo > now
                      select stat.Id).First();

            return id;
        }

        private int GetBuyerIdByUrorrOrThrow(bool isOnline, PageCodeEnum pageCode, string urorr, int? buyerId)
        {
            int result;

            if (isOnline)
            {
                bool isFirstSaleCenter = pageCode == PageCodeEnum.ChangeFirstSaleCenter || pageCode == PageCodeEnum.TermFirstSaleCenter;

                result = (from buyer in Db.BuyerRegisters
                          join buyerType in Db.NbuyerTypes on buyer.BuyerTypeId equals buyerType.Id
                          where buyer.UrrorNum == urorr
                                && ((isFirstSaleCenter
                                     && buyerType.Code == nameof(BuyerTypesEnum.CPP))
                                    || buyerType.Code == nameof(BuyerTypesEnum.Buyer))
                          select buyer.Id).FirstOrDefault();

                if (result == default)
                {
                    throw new BuyerDoesNotExistsException($"Buyer with registration number {urorr} does not exist.");
                }
            }
            else
            {
                result = buyerId.Value;
            }

            return result;
        }
    }
}
