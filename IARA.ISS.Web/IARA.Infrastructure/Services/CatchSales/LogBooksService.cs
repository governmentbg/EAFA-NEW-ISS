using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.EntityModels.Interfaces;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Interfaces;
using IARA.Interfaces.CatchSales;
using IARA.Interfaces.Flux;
using IARA.Interfaces.Legals;
using IARA.Security.Permissions;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.CatchSales
{
    public partial class LogBooksService : Service, ILogBooksService
    {
        private const char ONLINE_PAGE_SEPARATOR = '-';

        private readonly ILegalService legalService;
        private readonly IPersonService personService;
        private readonly ISalesReportMapper salesReportMapper;
        private readonly ISalesDomainService salesDomainService;
        private readonly IFishingActivitiesDomainService faDomainService;

        public LogBooksService(IARADbContext dbContext,
                               ILegalService legalService,
                               IPersonService personService,
                               ISalesReportMapper salesReportMapper,
                               ISalesDomainService salesDomainService,
                               IFishingActivitiesDomainService faDomainService)
            : base(dbContext)
        {
            this.legalService = legalService;
            this.personService = personService;
            this.salesReportMapper = salesReportMapper;
            this.salesDomainService = salesDomainService;
            this.faDomainService = faDomainService;
        }

        public IQueryable<LogBookRegisterDTO> GetAllLogBooks(CatchesAndSalesAdministrationFilters filters, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<LogBookRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllLogBooks(showInactive, permittedLogBookTypes);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredLogBooks(filters, permittedLogBookTypes)
                    : GetParametersFilteredLogBooks(filters, permittedLogBookTypes);
            }

            return result;
        }

        public LogBookPagesDTO GetLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                       CatchesAndSalesAdministrationFilters filters,
                                                       List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<ShipLogBookPageRegisterDTO> shipPages = GetShipLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            List<FirstSaleLogBookPageRegisterDTO> firstSalePages = GetFirstSaleLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            List<AdmissionLogBookPageRegisterDTO> admissionPages = GetAdmissionLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            List<TransportationLogBookPageRegisterDTO> transportationPages = GetTransportationLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            List<AquacultureLogBookPageRegisterDTO> aquaculturePages = GetAquacultureLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);

            return new LogBookPagesDTO
            {
                ShipPages = shipPages,
                FirstSalePages = firstSalePages,
                AdmissionPages = admissionPages,
                TransportationPages = transportationPages,
                AquaculturePages = aquaculturePages,
                UnloadingInformation = GetShipLogBookPagesOriginDeclarationInformation(shipPages.Select(x => x.Id).ToList()),
                FirstSaleProductInformation = GetFirstSaleLogBookPagesCatchInformation(firstSalePages.Select(x => x.Id).ToList()),
                AdmissionProductInformation = GetAdmissionLogBookPagesCatchInformation(admissionPages.Select(x => x.Id).ToList()),
                TransportationProductInformation = GetTransportationLogBookPagesCatchInformation(transportationPages.Select(x => x.Id).ToList()),
                AquacultureProductInformation = GetAquacultureLogBookPagesCatchInformation(aquaculturePages.Select(x => x.Id).ToList())
            };
        }

        public IQueryable<LogBookRegisterDTO> GetAllLogBooks(CatchesAndSalesPublicFilters filters,
                                                             List<LogBookTypesEnum> permittedLogBookTypes,
                                                             int userId)
        {
            IQueryable<LogBookRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllPublicLogBooks(showInactive: false, userId, permittedLogBookTypes); // За публичното приложение не гледаме showInactive, показваме само активни
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredLogBooks(filters, userId, permittedLogBookTypes)
                    : GetParametersFilteredLogBooks(filters, userId, permittedLogBookTypes);
            }

            return result;
        }

        public LogBookPagesDTO GetLogBookPagesForTable(IEnumerable<int> logBookIds, CatchesAndSalesPublicFilters filters, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<ShipLogBookPageRegisterDTO> shipPages = GetShipLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            List<FirstSaleLogBookPageRegisterDTO> firstSalePages = GetFirstSaleLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            List<AdmissionLogBookPageRegisterDTO> admissionPages = GetAdmissionLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            List<TransportationLogBookPageRegisterDTO> transportationPages = GetTransportationLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            List<AquacultureLogBookPageRegisterDTO> aquaculturePages = GetAquacultureLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);

            return new LogBookPagesDTO
            {
                ShipPages = shipPages,
                FirstSalePages = firstSalePages,
                AdmissionPages = admissionPages,
                TransportationPages = transportationPages,
                AquaculturePages = aquaculturePages,
                UnloadingInformation = GetShipLogBookPagesOriginDeclarationInformation(shipPages.Select(x => x.Id).ToList()),
                FirstSaleProductInformation = GetFirstSaleLogBookPagesCatchInformation(firstSalePages.Select(x => x.Id).ToList()),
                AdmissionProductInformation = GetAdmissionLogBookPagesCatchInformation(admissionPages.Select(x => x.Id).ToList()),
                TransportationProductInformation = GetTransportationLogBookPagesCatchInformation(transportationPages.Select(x => x.Id).ToList()),
                AquacultureProductInformation = GetAquacultureLogBookPagesCatchInformation(aquaculturePages.Select(x => x.Id).ToList())
            };
        }

        public List<CommercialFishingLogBookEditDTO> GetPermitLicenseLogBooks(int permitLicenseId)
        {
            DateTime now = DateTime.Now;

            List<CommercialFishingLogBookEditDTO> results = (from logBookLicense in Db.LogBookPermitLicenses
                                                             join logBook in Db.LogBooks on logBookLicense.LogBookId equals logBook.Id
                                                             join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookLicense.PermitLicenseRegisterId equals permitLicense.Id
                                                             where logBookLicense.PermitLicenseRegisterId == permitLicenseId
                                                             orderby logBookLicense.LogBookValidTo descending
                                                             select new CommercialFishingLogBookEditDTO
                                                             {
                                                                 LogBookLicenseId = logBookLicense.Id,
                                                                 LastLogBookLicenseId = logBookLicense.Id,
                                                                 LogBookId = logBookLicense.LogBookId,
                                                                 LogbookNumber = logBook.LogNum,
                                                                 LogBookTypeId = logBook.LogBookTypeId,
                                                                 IsOnline = logBook.IsOnline,
                                                                 StatusId = logBook.StatusId,
                                                                 IssueDate = logBook.IssueDate,
                                                                 FinishDate = logBook.FinishDate,
                                                                 StartPageNumber = logBook.StartPageNum,
                                                                 EndPageNumber = logBook.EndPageNum,
                                                                 Comment = logBook.Comments,
                                                                 PermitLicenseStartPageNumber = logBookLicense.StartPageNum,
                                                                 PermitLicenseEndPageNumber = logBookLicense.EndPageNum,
                                                                 Price = logBook.Price,
                                                                 LogBookIsActive = logBook.IsActive,
                                                                 PermitLicenseIsActive = logBookLicense.IsActive,
                                                                 IsActive = logBookLicense.IsActive,
                                                                 OwnerType = logBook.LogBookOwnerType != null
                                                                             ? Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType)
                                                                             : null,
                                                                 PermitLicenseRegistrationNumber = permitLicense.RegistrationNum,
                                                                 LogBookLicenseValidForm = logBookLicense.LogBookValidFrom,
                                                                 LogBookLicenseValidTo = logBookLicense.LogBookValidTo
                                                             }).ToList();

            foreach (CommercialFishingLogBookEditDTO result in results)
            {
                LogBookTypesEnum logBookType = GetLogBookType(result.LogBookTypeId);

                switch (logBookType)
                {
                    case LogBookTypesEnum.Ship:
                        result.ShipPagesAndDeclarations = GetShipLogBookPagesAndDeclarations(result.LogBookId.Value, permitLicenseId);
                        break;
                    case LogBookTypesEnum.Admission:
                        result.AdmissionPagesAndDeclarations = GetAdmissionLogBookPagesAndDeclarations(result.LogBookId.Value, permitLicenseId);
                        break;
                    case LogBookTypesEnum.Transportation:
                        result.TransportationPagesAndDeclarations = GetTransportationLogBookPagesAndDeclarations(result.LogBookId.Value, permitLicenseId);
                        break;
                }
            }

            return results;
        }

        public CommercialFishingLogBookEditDTO GetCommercialFishingLogBook(int id)
        {
            CommercialFishingLogBookEditDTO result = (from logBook in Db.LogBooks
                                                      join logBookLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookLicense.LogBookId
                                                      join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookLicense.PermitLicenseRegisterId equals permitLicense.Id
                                                      orderby logBookLicense.LogBookValidTo descending
                                                      where logBook.Id == id
                                                      select new CommercialFishingLogBookEditDTO
                                                      {
                                                          LogBookLicenseId = logBookLicense.Id,
                                                          LastLogBookLicenseId = logBookLicense.Id,
                                                          LogBookId = logBookLicense.LogBookId,
                                                          LogbookNumber = logBook.LogNum,
                                                          LogBookTypeId = logBook.LogBookTypeId,
                                                          StatusId = logBook.StatusId,
                                                          IssueDate = logBook.IssueDate,
                                                          FinishDate = logBook.FinishDate,
                                                          IsOnline = logBook.IsOnline,
                                                          StartPageNumber = logBook.StartPageNum,
                                                          EndPageNumber = logBook.EndPageNum,
                                                          PermitLicenseStartPageNumber = logBookLicense.StartPageNum,
                                                          PermitLicenseEndPageNumber = logBookLicense.EndPageNum,
                                                          OwnerType = logBook.LogBookOwnerType != null
                                                                      ? Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType)
                                                                      : null,
                                                          Price = logBook.Price,
                                                          Comment = logBook.Comments,
                                                          LogBookIsActive = logBook.IsActive,
                                                          PermitLicenseIsActive = logBookLicense.IsActive,
                                                          IsActive = logBookLicense.IsActive,
                                                          PermitLicenseRegistrationNumber = permitLicense.RegistrationNum,
                                                          LogBookLicenseValidForm = logBookLicense.LogBookValidFrom,
                                                          LogBookLicenseValidTo = logBookLicense.LogBookValidTo
                                                      }).First();

            LogBookTypesEnum logBookType = GetLogBookType(result.LogBookTypeId);

            switch (logBookType)
            {
                case LogBookTypesEnum.Ship:
                    result.ShipPagesAndDeclarations = GetShipLogBookPagesAndDeclarations(result.LogBookId.Value);
                    break;
                case LogBookTypesEnum.Admission:
                    result.AdmissionPagesAndDeclarations = GetAdmissionLogBookPagesAndDeclarations(result.LogBookId.Value);
                    break;
                case LogBookTypesEnum.Transportation:
                    result.TransportationPagesAndDeclarations = GetTransportationLogBookPagesAndDeclarations(result.LogBookId.Value);
                    break;
            }

            return result;
        }

        public LogBookEditDTO GetLogBook(int id)
        {
            IQueryable<LogBook> query = from logBook in Db.LogBooks
                                        where logBook.Id == id
                                        select logBook;

            LogBookEditDTO result = MapLogBooksToDTO(query).First();

            LogBookTypesEnum logBookType = GetLogBookType(result.LogBookTypeId);

            switch (logBookType)
            {
                case LogBookTypesEnum.FirstSale:
                    result.FirstSalePages = GetFirstSalePages(result.LogBookId.Value);
                    break;
                case LogBookTypesEnum.Admission:
                    result.AdmissionPagesAndDeclarations = GetAdmissionLogBookPagesAndDeclarations(result.LogBookId.Value);
                    break;
                case LogBookTypesEnum.Transportation:
                    result.TransportationPagesAndDeclarations = GetTransportationLogBookPagesAndDeclarations(result.LogBookId.Value);
                    break;
                case LogBookTypesEnum.Aquaculture:
                    result.AquaculturePages = GetAquaculturePages(result.LogBookId.Value);
                    break;
            }

            return result;
        }

        public List<LogBookEditDTO> GetBuyerLogBooks(int buyerId)
        {
            IQueryable<LogBook> logBooks = from logBook in Db.LogBooks
                                           where logBook.RegisteredBuyerId == buyerId
                                           select logBook;

            List<LogBookEditDTO> results = MapLogBooksToDTO(logBooks).ToList();

            foreach (LogBookEditDTO result in results)
            {
                LogBookTypesEnum logBookType = GetLogBookType(result.LogBookTypeId);

                switch (logBookType)
                {
                    case LogBookTypesEnum.FirstSale:
                        result.FirstSalePages = GetFirstSalePages(result.LogBookId.Value);
                        break;
                    case LogBookTypesEnum.Admission:
                        result.AdmissionPagesAndDeclarations = GetAdmissionLogBookPagesAndDeclarations(result.LogBookId.Value);
                        break;
                    case LogBookTypesEnum.Transportation:
                        result.TransportationPagesAndDeclarations = GetTransportationLogBookPagesAndDeclarations(result.LogBookId.Value);
                        break;
                }
            }

            return results;
        }

        public List<RangeOverlappingLogBooksDTO> GetOverlappedLogBooks(List<OverlappingLogBooksParameters> ranges)
        {
            List<RangeOverlappingLogBooksDTO> results = new List<RangeOverlappingLogBooksDTO>();

            List<int> logBookIds = ranges.Where(x => x.LogBookId.HasValue).Select(x => x.LogBookId.Value).ToList();
            Dictionary<int, string> logBookNumber = (from logBook in Db.LogBooks
                                                     where logBookIds.Contains(logBook.Id)
                                                     select new
                                                     {
                                                         logBook.Id,
                                                         logBook.LogNum
                                                     }).ToDictionary(x => x.Id, y => y.LogNum);

            foreach (OverlappingLogBooksParameters range in ranges)
            {
                List<OverlappingLogBookDTO> result;

                var query = from logBook in Db.LogBooks
                            join ship in Db.ShipsRegister on logBook.ShipId equals ship.Id into logBookShip
                            from ship in logBookShip.DefaultIfEmpty()
                            join aquaculture in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquaculture.Id into logBookAquaculture
                            from aquaculture in logBookAquaculture.DefaultIfEmpty()
                            join registeredBuyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals registeredBuyer.Id into logBookBuyer
                            from registeredBuyer in logBookBuyer.DefaultIfEmpty()
                            join registeredBuyerLegal in Db.Legals on registeredBuyer.SubmittedForLegalId equals registeredBuyerLegal.Id into logBookRegLegal
                            from registeredBuyerLegal in logBookRegLegal.DefaultIfEmpty()
                            join registeredBuyerPerson in Db.Persons on registeredBuyer.SubmittedForPersonId equals registeredBuyerPerson.Id into logBookRegPerson
                            from registeredBuyerPerson in logBookRegPerson.DefaultIfEmpty()
                            join person in Db.Persons on logBook.PersonId equals person.Id into logBookPerson
                            from person in logBookPerson.DefaultIfEmpty()
                            join legal in Db.Legals on logBook.LegalId equals legal.Id into logBookLegal
                            from legal in logBookLegal.DefaultIfEmpty()
                            join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                            where logBook.LogBookTypeId == range.TypeId
                                  && (!range.LogBookId.HasValue || logBook.Id != range.LogBookId.Value)
                                  && !logBook.IsOnline
                                  && logBook.IsActive
                                  && logBookStatus.Code != nameof(LogBookStatusesEnum.Finished)
                            select new
                            {
                                Id = logBook.Id,
                                Number = logBook.LogNum,
                                IssueDate = logBook.IssueDate,
                                FinishDate = logBook.FinishDate,
                                StartPage = logBook.StartPageNum,
                                EndPage = logBook.EndPageNum,
                                StatusId = logBook.StatusId,
                                OwnerName = ship != null
                                              ? ship.Cfr != null ? $"{ship.Name} | {ship.Cfr} | {ship.ExternalMark}" : $"{ship.Name} | {ship.ExternalMark}"
                                              : aquaculture != null
                                                ? $"{aquaculture.Name} ({aquaculture.UrorNum})"
                                                : registeredBuyerLegal != null
                                                    ? $"{registeredBuyerLegal.Name} ({registeredBuyer.UrrorNum})"
                                                    : registeredBuyerPerson != null
                                                    ? $"{registeredBuyerPerson.FirstName} {registeredBuyerPerson.LastName} ({registeredBuyer.UrrorNum})"
                                                    : person != null
                                                        ? $"{person.FirstName} {person.LastName}"
                                                        : legal != null
                                                            ? $"{legal.Name} ({legal.Eik})"
                                                            : "***Липсва***"
                            };

                LogBookTypesEnum logBookType = GetLogBookType(range.TypeId.Value);

                if (logBookType == LogBookTypesEnum.Ship
                    || range.OwnerType == LogBookPagePersonTypesEnum.Person
                    || range.OwnerType == LogBookPagePersonTypesEnum.LegalPerson) // search in db.LogBookPermitLicenses
                {
                    result = (from logBook in query
                              join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId
                              join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id
                              where logBookPermitLicense.StartPageNum.HasValue
                                    && logBookPermitLicense.EndPageNum.HasValue
                                    && !(range.EndPage < logBookPermitLicense.StartPageNum || logBookPermitLicense.EndPageNum < range.StartPage)
                              orderby logBook.IssueDate descending
                              select new OverlappingLogBookDTO
                              {
                                  Id = logBook.Id,
                                  Number = logBook.Number,
                                  IssueDate = logBook.IssueDate,
                                  FinishDate = logBook.FinishDate,
                                  StartPage = logBook.StartPage,
                                  EndPage = logBook.EndPage,
                                  StatusId = logBook.StatusId,
                                  OwnerName = logBook.OwnerName,
                                  LogBookPermitLicenseId = logBookPermitLicense.Id,
                                  LogBookPermitLicenseStartPage = logBookPermitLicense.StartPageNum,
                                  LogBookPermitLicenseEndPage = logBookPermitLicense.EndPageNum,
                                  LogBookPermitLicenseValidFrom = logBookPermitLicense.LogBookValidFrom,
                                  LogBookPermitLicenseValidTo = logBookPermitLicense.LogBookValidTo,
                                  LogBookPermitLicenseNumber = permitLicense.RegistrationNum
                              }).ToList();
                }
                else // search in db.LogBooks
                {
                    result = (from logBook in query
                              where !(range.EndPage < logBook.StartPage || logBook.EndPage < range.StartPage)
                              orderby logBook.IssueDate descending
                              select new OverlappingLogBookDTO
                              {
                                  Id = logBook.Id,
                                  Number = logBook.Number,
                                  IssueDate = logBook.IssueDate,
                                  FinishDate = logBook.FinishDate,
                                  StartPage = logBook.StartPage,
                                  EndPage = logBook.EndPage,
                                  StatusId = logBook.StatusId,
                                  OwnerName = logBook.OwnerName
                              }).ToList();
                }

                results.Add(new RangeOverlappingLogBooksDTO
                {
                    StartPage = range.StartPage.Value,
                    EndPage = range.EndPage.Value,
                    LogBookNumber = range.LogBookId.HasValue ? logBookNumber[range.LogBookId.Value] : null,
                    OverlappingLogBooks = result
                });
            }

            return results;
        }

        public BasicLogBookPageDocumentDataDTO GetLogBookPageDocumentData(BasicLogBookPageDocumentParameters parameters)
        {
            BasicLogBookPageDocumentDataDTO result = new BasicLogBookPageDocumentDataDTO
            {
                ShipLogBookPageId = parameters.ShipLogBookPageId.Value,
                DocumentNumber = parameters.DocumentNumber.Value,
                SourceData = GetCommonLogBookPageDataByShipLogBookPage(parameters.ShipLogBookPageId.Value)
            };

            LogBookPagePersonTypesEnum ownerType = (from lb in Db.LogBooks
                                                    where lb.Id == parameters.LogBookId.Value
                                                    select Enum.Parse<LogBookPagePersonTypesEnum>(lb.LogBookOwnerType)).First();

            switch (parameters.DocumentType)
            {
                case LogBookPageDocumentTypesEnum.FirstSaleDocument:
                    {
                        if (ownerType == LogBookPagePersonTypesEnum.RegisteredBuyer)
                        {
                            var logBookData = (from lb in Db.LogBooks
                                               where lb.Id == parameters.LogBookId.Value
                                               select new
                                               {
                                                   LogBookNumber = lb.LogNum,
                                                   RegisteredBuyerId = lb.RegisteredBuyerId
                                               }).First();

                            OldLogBookPageStatus pageStatus = GetNewLogBookPageStatusAndCheckValidity(parameters.DocumentNumber.Value,
                                                                                                      parameters.LogBookId.Value,
                                                                                                      Db.FirstSaleLogBookPages);

                            if (pageStatus != null && pageStatus.Status == LogBookPageStatusesEnum.Missing)
                            {
                                result.PageStatus = pageStatus.Status;
                            }

                            DocumentLogBookData lbData = GetDocumentLogBookData(parameters, ownerType);

                            if (lbData == null)
                            {
                                throw new RecordNotFoundException("Cannot find log book for this log book owner and page number");
                            }

                            result.LogBookId = parameters.LogBookId.Value;
                            result.LogBookNumber = logBookData.LogBookNumber;
                            result.RegisteredBuyerId = logBookData.RegisteredBuyerId.Value;

                            SetCommonLogBookPageDocumentData(lbData, result);
                        }
                    }
                    break;
                case LogBookPageDocumentTypesEnum.AdmissionDocument:
                    {
                        DocumentLogBookData logBookData = GetDocumentLogBookData(parameters, ownerType);

                        if (logBookData == null)
                        {
                            throw new RecordNotFoundException("Cannot find log book for this log book owner and page number");
                        }

                        OldLogBookPageStatus pageStatus = GetNewLogBookPageStatusAndCheckValidity(parameters.DocumentNumber.Value,
                                                                                                  logBookData.LogBookId,
                                                                                                  Db.AdmissionLogBookPages);

                        if (pageStatus != null && pageStatus.Status == LogBookPageStatusesEnum.Missing)
                        {
                            result.PageStatus = pageStatus.Status;
                        }

                        SetCommonLogBookPageDocumentData(logBookData, result);
                    }
                    break;
                case LogBookPageDocumentTypesEnum.TransportationDocument:
                    {
                        DocumentLogBookData logBookData = GetDocumentLogBookData(parameters, ownerType);

                        if (logBookData == null)
                        {
                            throw new RecordNotFoundException("Cannot find log book for this log book owner and page number");
                        }

                        OldLogBookPageStatus pageStatus = GetNewLogBookPageStatusAndCheckValidity(parameters.DocumentNumber.Value,
                                                                                                  logBookData.LogBookId,
                                                                                                  Db.TransportationLogBookPages);

                        if (pageStatus != null && pageStatus.Status == LogBookPageStatusesEnum.Missing)
                        {
                            result.PageStatus = pageStatus.Status;
                        }

                        SetCommonLogBookPageDocumentData(logBookData, result);
                    }
                    break;
            }

            return result;
        }

        public List<LogBookNomenclatureDTO> GetLogBookPageDocumentOwnerData(decimal documentNumber, LogBookPageDocumentTypesEnum documentType)
        {
            LogBookTypesEnum logBookType;
            switch (documentType)
            {
                case LogBookPageDocumentTypesEnum.FirstSaleDocument:
                    logBookType = LogBookTypesEnum.FirstSale;
                    break;
                case LogBookPageDocumentTypesEnum.AdmissionDocument:
                    logBookType = LogBookTypesEnum.Admission;
                    break;
                case LogBookPageDocumentTypesEnum.TransportationDocument:
                    logBookType = LogBookTypesEnum.Transportation;
                    break;
                default:
                    throw new ArgumentException($"No log book type found for document type: ${documentType.ToString()}");
            }

            HashSet<int> possiblePermitLicenseLogBookIds = (from logBook in Db.LogBooks
                                                            join lbType in Db.NlogBookTypes on logBook.LogBookTypeId equals lbType.Id
                                                            join lbStatus in Db.NlogBookStatuses on logBook.StatusId equals lbStatus.Id
                                                            join lbPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals lbPermitLicense.LogBookId
                                                            where lbType.Code == logBookType.ToString()
                                                                  && lbPermitLicense.StartPageNum.HasValue
                                                                  && lbPermitLicense.EndPageNum.HasValue
                                                                  && (decimal)lbPermitLicense.StartPageNum <= documentNumber
                                                                  && (decimal)lbPermitLicense.EndPageNum >= documentNumber
                                                                  && lbStatus.Code != nameof(LogBookStatusesEnum.Finished)
                                                                  && logBook.IsActive
                                                            select logBook.Id).ToHashSet();

            HashSet<int> possibleNoPermitLicenseLogBookIds = (from logBook in Db.LogBooks
                                                              join lbType in Db.NlogBookTypes on logBook.LogBookTypeId equals lbType.Id
                                                              join lbStatus in Db.NlogBookStatuses on logBook.StatusId equals lbStatus.Id
                                                              where lbType.Code == logBookType.ToString()
                                                                    && (decimal)logBook.StartPageNum <= documentNumber
                                                                    && (decimal)logBook.EndPageNum >= documentNumber
                                                                    && lbStatus.Code != nameof(LogBookStatusesEnum.Finished)
                                                                    && logBook.IsActive
                                                              select logBook.Id).ToHashSet();

            HashSet<int> possibleLogBookIds = possiblePermitLicenseLogBookIds.Concat(possibleNoPermitLicenseLogBookIds).ToHashSet();

            List<LogBookNomenclatureDTO> possibleLogBooks = (from logBook in Db.LogBooks
                                                             join person in Db.Persons on logBook.PersonId equals person.Id into lbPerson
                                                             from person in lbPerson.DefaultIfEmpty()
                                                             join legal in Db.Legals on logBook.LegalId equals legal.Id into lbLegal
                                                             from legal in lbLegal.DefaultIfEmpty()
                                                             join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id into lbBuyer
                                                             from buyer in lbBuyer.DefaultIfEmpty()
                                                             join personBuyer in Db.Persons on buyer.SubmittedForPersonId equals personBuyer.Id into pBuyer
                                                             from personBuyer in pBuyer.DefaultIfEmpty()
                                                             join legalBuyer in Db.Legals on buyer.SubmittedForLegalId equals legalBuyer.Id into lBuyer
                                                             from legalBuyer in lBuyer.DefaultIfEmpty()
                                                             join ship in Db.ShipsRegister on logBook.ShipId equals ship.Id into lbShip
                                                             from ship in lbShip.DefaultIfEmpty()
                                                             join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId into lbPermitLicense
                                                             from logBookPermitLicense in lbPermitLicense.DefaultIfEmpty()
                                                             join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id into pl
                                                             from permitLicense in pl.DefaultIfEmpty()
                                                             where possibleLogBookIds.Contains(logBook.Id)
                                                             select new LogBookNomenclatureDTO
                                                             {
                                                                 Value = logBook.Id,
                                                                 DisplayName = logBook.LogNum,
                                                                 OwnerName = person != null
                                                                             ? $"{person.FirstName} {person.LastName}"
                                                                             : legal != null
                                                                                ? $"{legal.Name} - {legal.Eik}"
                                                                                : personBuyer != null
                                                                                    ? $"{personBuyer.FirstName} {personBuyer.LastName} ({buyer.UrrorNum})"
                                                                                    : legalBuyer != null
                                                                                        ? $"{legalBuyer.Name} - {legalBuyer.Eik} ({buyer.UrrorNum})"
                                                                                        : ship != null
                                                                                            ? $"{ship.Name} | {ship.ExternalMark} | {ship.Cfr}"
                                                                                            : "",
                                                                 OwnerType = logBook.LogBookOwnerType != null
                                                                             ? Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType)
                                                                             : null,
                                                                 LogBookPermitLicenseId = logBookPermitLicense != null
                                                                                          ? logBookPermitLicense.Id
                                                                                          : null,
                                                                 PermitLicenseNumber = logBookPermitLicense != null && permitLicense != null
                                                                                       ? permitLicense.RegistrationNum
                                                                                       : null,
                                                                 IsActive = true
                                                             }).ToList();

            if (!possibleLogBooks.Any())
            {
                throw new RecordNotFoundException("Cannot find log book for this log book owner and page number");
            }

            return possibleLogBooks;
        }

        private DocumentLogBookData GetDocumentLogBookData(BasicLogBookPageDocumentParameters parameters, LogBookPagePersonTypesEnum ownerType)
        {
            DocumentLogBookData logBookData = null;

            switch (ownerType)
            {
                case LogBookPagePersonTypesEnum.Person:
                    {
                        logBookData = (from logBook in Db.LogBooks
                                       where logBook.Id == parameters.LogBookId
                                       select new DocumentLogBookData
                                       {
                                           LogBookId = logBook.Id,
                                           LogBookNumber = logBook.LogNum,
                                           PersonId = logBook.PersonId.Value,
                                           OwnerType = Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType)
                                       }).FirstOrDefault();
                    }
                    break;
                case LogBookPagePersonTypesEnum.LegalPerson:
                    {
                        logBookData = (from logBook in Db.LogBooks
                                       where logBook.Id == parameters.LogBookId
                                       select new DocumentLogBookData
                                       {
                                           LogBookId = logBook.Id,
                                           LogBookNumber = logBook.LogNum,
                                           LegalId = logBook.LegalId.Value,
                                           OwnerType = Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType)
                                       }).FirstOrDefault();
                    }
                    break;
                case LogBookPagePersonTypesEnum.RegisteredBuyer:
                    {
                        logBookData = (from logBook in Db.LogBooks
                                       where logBook.Id == parameters.LogBookId
                                       select new DocumentLogBookData
                                       {
                                           LogBookId = logBook.Id,
                                           LogBookNumber = logBook.LogNum,
                                           RegisteredBuyerId = logBook.RegisteredBuyerId.Value,
                                           OwnerType = Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType)
                                       }).FirstOrDefault();
                    }
                    break;
            }

            return logBookData;
        }

        private void SetCommonLogBookPageDocumentData(DocumentLogBookData logBookData, BasicLogBookPageDocumentDataDTO result)
        {
            result.LogBookId = logBookData.LogBookId;
            result.LogBookNumber = logBookData.LogBookNumber;
            result.OwnerType = logBookData.OwnerType;
            result.PersonData = GetLogBookPagePerson(logBookData.OwnerType, logBookData.RegisteredBuyerId, logBookData.PersonId, logBookData.LegalId);
        }

        public bool CheckDocumentPageToAddExistance(decimal pageToAdd, int logBookId, LogBookTypesEnum logBookType)
        {
            bool result;

            switch (logBookType)
            {
                case LogBookTypesEnum.FirstSale:
                    {
                        result = (from logBook in Db.LogBooks
                                  where logBook.Id == logBookId
                                        && logBook.StartPageNum <= pageToAdd
                                        && logBook.EndPageNum >= pageToAdd
                                  select logBook.Id).Any();
                    }
                    break;
                case LogBookTypesEnum.Admission:
                    {
                        result = CheckAdmissionOrTransportationPageRange(logBookId, pageToAdd);
                    }
                    break;
                case LogBookTypesEnum.Transportation:
                    {
                        result = CheckAdmissionOrTransportationPageRange(logBookId, pageToAdd);
                    }
                    break;
                default: throw new ArgumentException($"Not a valid logBookType {logBookType.ToString()}");
            }

            return result;
        }

        private bool CheckAdmissionOrTransportationPageRange(int logBookId, decimal pageToAdd)
        {
            bool result = (from logBook in Db.LogBooks
                           join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId into lbPl
                           from logBookPermitLicense in lbPl.DefaultIfEmpty()
                           where logBook.Id == logBookId
                                 && (
                                      ((logBook.LogBookOwnerType == nameof(LogBookPagePersonTypesEnum.Person) || logBook.LogBookOwnerType == nameof(LogBookPagePersonTypesEnum.LegalPerson))
                                       && logBookPermitLicense != null
                                       && logBookPermitLicense.StartPageNum.HasValue
                                       && logBookPermitLicense.EndPageNum.HasValue
                                       && logBookPermitLicense.StartPageNum.Value <= pageToAdd
                                       && logBookPermitLicense.EndPageNum.Value >= pageToAdd)
                                      || (logBook.StartPageNum <= pageToAdd && logBook.EndPageNum >= pageToAdd)
                                    )
                           select logBook.Id).Any();

            return result;
        }

        public LogBookPageStatusesEnum? CheckDocumentPageToAddStatus(decimal pageToAdd, LogBookTypesEnum logBookType)
        {
            LogBookPageStatusesEnum? result = null;
            string logBookPageStatusCode = null;

            switch (logBookType)
            {
                case LogBookTypesEnum.FirstSale:
                    {
                        logBookPageStatusCode = (from page in Db.FirstSaleLogBookPages
                                                 where page.PageNum == pageToAdd
                                                 select page.Status).FirstOrDefault();
                    }
                    break;
                case LogBookTypesEnum.Admission:
                    {
                        logBookPageStatusCode = (from page in Db.AdmissionLogBookPages
                                                 where page.PageNum == pageToAdd
                                                 select page.Status).FirstOrDefault();

                    }
                    break;
                case LogBookTypesEnum.Transportation:
                    {
                        logBookPageStatusCode = (from page in Db.TransportationLogBookPages
                                                 where page.PageNum == pageToAdd
                                                 select page.Status).FirstOrDefault();

                    }
                    break;
                default: throw new ArgumentException($"Not a valid logBookType: {logBookType.ToString()}");
            }

            if (!string.IsNullOrEmpty(logBookPageStatusCode))
            {
                result = Enum.Parse<LogBookPageStatusesEnum>(logBookPageStatusCode);
            }

            return result;
        }

        public CommonLogBookPageDataDTO GetCommonLogBookPageDataDTO(CommonLogBookPageDataParameters parameters)
        {
            CommonLogBookPageDataDTO result = null;

            switch (parameters.LogBookType)
            {
                case LogBookTypesEnum.FirstSale:
                    {
                        if (!string.IsNullOrEmpty(parameters.OriginDeclarationNumber))
                        {
                            result = GetCommonLogBookPageDataByOriginDeclarationNumber(parameters.OriginDeclarationNumber);
                        }
                        else if (parameters.TransportationDocumentNumber.HasValue) // Ако няма номер на декларация за произход, то може да има номер на документ за превоз
                        {
                            result = GetCommonLogBookPageDataByTransportationDocumentNumber(parameters.TransportationDocumentNumber.Value);
                        }
                        else // Ако няма номер на декларация за произход и номер на документ за превоз, то трябва задължително да има номер на документ за приемане
                        {
                            result = GetCommonLogBookPageDataByAdmissionDocumentNumber(parameters.AdmissionDocumentNumber.Value);
                        }

                        result.PossibleLogBooks = (from logBook in Db.LogBooks
                                                   join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id
                                                   join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                                                   where logBook.Id == parameters.LogBookId
                                                         && logBook.StartPageNum <= parameters.PageNumberToAdd
                                                         && logBook.EndPageNum >= parameters.PageNumberToAdd
                                                   select new PossibleLogBooksForPageDTO
                                                   {
                                                       LogBookId = logBook.Id,
                                                       LogBookNumber = logBook.LogNum,
                                                       BuyerNumber = buyer.RegistrationNum,
                                                       BuyerUrorr = buyer.UrrorNum,
                                                       BuyerName = buyer.UtilityName != null
                                                                   ? buyer.UtilityName
                                                                   : buyer.VehicleNumber != null
                                                                       ? buyer.VehicleNumber
                                                                       : ""
                                                   }).ToList();
                    }
                    break;
                case LogBookTypesEnum.Admission:
                    {
                        if (!string.IsNullOrEmpty(parameters.OriginDeclarationNumber))
                        {
                            result = GetCommonLogBookPageDataByOriginDeclarationNumber(parameters.OriginDeclarationNumber);
                        }
                        else // Ако няма номер на декларация за произход, то трябва задължително да има номер на документ за превоз
                        {
                            result = GetCommonLogBookPageDataByTransportationDocumentNumber(parameters.TransportationDocumentNumber.Value);
                        }

                        result.PossibleLogBooks = GetPossibleLogBooksForPage(parameters.PageNumberToAdd.Value, parameters.LogBookId.Value);
                    }
                    break;
                case LogBookTypesEnum.Transportation:
                    {
                        if (!string.IsNullOrEmpty(parameters.OriginDeclarationNumber))
                        {
                            result = GetCommonLogBookPageDataByOriginDeclarationNumber(parameters.OriginDeclarationNumber);
                        }

                        result.PossibleLogBooks = GetPossibleLogBooksForPage(parameters.PageNumberToAdd.Value, parameters.LogBookId.Value);
                    }
                    break;
            }

            return result;
        }

        private List<PossibleLogBooksForPageDTO> GetPossibleLogBooksForPage(decimal pageToAdd, int logBookId)
        {
            return (from logBook in Db.LogBooks
                    join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId into lbPl
                    from logBookPermitLicense in lbPl.DefaultIfEmpty()
                    join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id into pl
                    from permitLicense in pl.DefaultIfEmpty()
                    join qualifiedFisher in Db.FishermenRegisters on permitLicense.QualifiedFisherId equals qualifiedFisher.Id into qf
                    from qualifiedFisher in qf.DefaultIfEmpty()
                    join qualifiedFisherPerson in Db.Persons on qualifiedFisher.PersonId equals qualifiedFisherPerson.Id into qfPerson
                    from qualifiedFisherPerson in qfPerson.DefaultIfEmpty()
                    join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id into lbBuyer
                    from buyer in lbBuyer.DefaultIfEmpty()
                    join person in Db.Persons on logBook.PersonId equals person.Id into lbPerson
                    from person in lbPerson.DefaultIfEmpty()
                    join legal in Db.Legals on logBook.LegalId equals legal.Id into lbLegal
                    from legal in lbLegal.DefaultIfEmpty()
                    join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                    where logBook.Id == logBookId
                          && (
                              (logBookPermitLicense == null
                                && logBook.StartPageNum <= pageToAdd
                                && logBook.EndPageNum >= pageToAdd)
                              ||
                              (logBookPermitLicense != null
                                && logBookPermitLicense.StartPageNum.HasValue
                                && logBookPermitLicense.EndPageNum.HasValue
                                && logBookPermitLicense.StartPageNum <= pageToAdd
                                && logBookPermitLicense.EndPageNum >= pageToAdd)
                             // ||
                             //(logBookPermitLicense != null
                             // && (!logBookPermitLicense.StartPageNum.HasValue || !logBookPermitLicense.EndPageNum.HasValue)
                             // && logBook.StartPageNum <= pageToAdd
                             // && logBook.EndPageNum >= pageToAdd)
                             )
                    select new PossibleLogBooksForPageDTO
                    {
                        LogBookId = logBook.Id,
                        LogBookNumber = logBook.LogNum,
                        LogBookPermitLicenseId = logBookPermitLicense.Id,
                        BuyerNumber = buyer != null ? buyer.RegistrationNum : null,
                        BuyerUrorr = buyer != null ? buyer.UrrorNum : null,
                        BuyerName = buyer != null
                                    ? buyer.UtilityName != null
                                        ? buyer.UtilityName
                                        : buyer.VehicleNumber != null
                                            ? buyer.VehicleNumber
                                            : ""
                                    : null,
                        PersonName = person != null ? person.FirstName + " " + person.LastName : null,
                        LegalName = legal != null ? legal.Name + " (" + legal.Eik + ")" : null,
                        PermitLicenseNumber = permitLicense != null ? permitLicense.RegistrationNum : null,
                        QualifiedFisherName = permitLicense != null && qualifiedFisher != null && qualifiedFisherPerson != null
                                              ? qualifiedFisherPerson.FirstName + " " + qualifiedFisherPerson.LastName
                                              : null,
                        QualifiedFisherNumber = permitLicense != null && qualifiedFisher != null
                                                ? qualifiedFisher.RegistrationNum
                                                : null
                    }).ToList();
        }

        public List<LogBookPageProductDTO> GetPossibleProducts(int shipPageId)
        {
            int originDeclatarionId = (from shipPage in Db.ShipLogBookPages
                                       join originDeclaration in Db.OriginDeclarations on shipPage.Id equals originDeclaration.LogBookPageId
                                       where shipPage.Id == shipPageId
                                             && originDeclaration.IsActive
                                       select originDeclaration.Id).FirstOrDefault();

            List<LogBookPageProductDTO> products;

            if (originDeclatarionId == default)
            {
                products = new List<LogBookPageProductDTO>();
            }
            else
            {
                products = GetNewProductsByOriginDeclarationId(originDeclatarionId);
            }

            return products;
        }

        public void AnnulLogBookPage(LogBookPageCancellationReasonDTO reasonData)
        {
            LogBookTypesEnum logBookType = (from logBook in Db.LogBooks
                                            join lbType in Db.NlogBookTypes on logBook.LogBookTypeId equals lbType.Id
                                            where logBook.Id == reasonData.LogBookId
                                            select Enum.Parse<LogBookTypesEnum>(lbType.Code)).First();

            int? fluxReportPageId = null;
            Guid? fluxReportPageIdentifier = null;

            switch (logBookType)
            {
                case LogBookTypesEnum.Ship:
                    {
                        ShipLogBookPage shipLogBookPage = (from logBookPage in Db.ShipLogBookPages
                                                           where logBookPage.PageNum == reasonData.LogBookPageNumber
                                                           select logBookPage).Single();

                        shipLogBookPage.Status = nameof(LogBookPageStatusesEnum.Canceled);
                        shipLogBookPage.CancelationReason = reasonData.Reason;
                        DeleteAnnulledShipLogBookPageOriginDeclaration(shipLogBookPage.Id);
                        DeleteAnnulledShipLogBookPageCatches(shipLogBookPage.Id);
                    }
                    break;
                case LogBookTypesEnum.FirstSale:
                    {
                        long logBookPageNumber = long.Parse(reasonData.LogBookPageNumber);
                        FirstSaleLogBookPage firstSaleLogBookPage = (from logBookPage in Db.FirstSaleLogBookPages
                                                                     where logBookPage.PageNum == logBookPageNumber
                                                                     select logBookPage).Single();

                        firstSaleLogBookPage.Status = nameof(LogBookPageStatusesEnum.Canceled);
                        firstSaleLogBookPage.CancelationReason = reasonData.Reason;
                        DeleteAnnulledFirstSalePageProducts(firstSaleLogBookPage.Id);

                        fluxReportPageId = firstSaleLogBookPage.Id;
                        fluxReportPageIdentifier = firstSaleLogBookPage.FluxIdentifier;
                    }
                    break;
                case LogBookTypesEnum.Admission:
                    {
                        long logBookPageNumber = long.Parse(reasonData.LogBookPageNumber);
                        AdmissionLogBookPage admissionLogBookPage = (from logBookPage in Db.AdmissionLogBookPages
                                                                     where logBookPage.PageNum == logBookPageNumber
                                                                     select logBookPage).Single();

                        admissionLogBookPage.Status = nameof(LogBookPageStatusesEnum.Canceled);
                        admissionLogBookPage.CancelationReason = reasonData.Reason;
                        DeleteAnnulledAdmissionLogBookPageProducts(admissionLogBookPage.Id);

                        fluxReportPageId = admissionLogBookPage.Id;
                        fluxReportPageIdentifier = admissionLogBookPage.FluxIdentifier;
                    }
                    break;
                case LogBookTypesEnum.Transportation:
                    {
                        long logBookPageNumber = long.Parse(reasonData.LogBookPageNumber);
                        TransportationLogBookPage transportationLogBookPage = (from logBookPage in Db.TransportationLogBookPages
                                                                               where logBookPage.PageNum == logBookPageNumber
                                                                               select logBookPage).Single();

                        transportationLogBookPage.Status = nameof(LogBookPageStatusesEnum.Canceled);
                        transportationLogBookPage.CancelationReason = reasonData.Reason;
                        DeleteAnnulledTransportationLogBookPageProducts(transportationLogBookPage.Id);

                        fluxReportPageId = transportationLogBookPage.Id;
                        fluxReportPageIdentifier = transportationLogBookPage.FluxIdentifier;
                    }
                    break;
                case LogBookTypesEnum.Aquaculture:
                    {
                        long logBookPageNumber = long.Parse(reasonData.LogBookPageNumber);
                        AquacultureLogBookPage aquacultureLogBookPage = (from logBookPage in Db.AquacultureLogBookPages
                                                                         where logBookPage.PageNum == logBookPageNumber
                                                                         select logBookPage).Single();

                        aquacultureLogBookPage.Status = nameof(LogBookPageStatusesEnum.Canceled);
                        aquacultureLogBookPage.CancelationReason = reasonData.Reason;
                        DeleteAnnulledAquacultureLogBookPageProducts(aquacultureLogBookPage.Id);
                    }
                    break;
            }

            Db.SaveChanges();

            if (fluxReportPageId.HasValue && fluxReportPageIdentifier.HasValue)
            {
                SendFluxSalesReportDelete(logBookType, fluxReportPageId.Value, fluxReportPageIdentifier.Value);
            }
        }

        // Checks
        public bool CheckOriginDeclarationExistance(string number)
        {
            bool result = (from shipPage in Db.ShipLogBookPages
                           where shipPage.PageNum == number
                                 && shipPage.Status == nameof(LogBookPageStatusesEnum.Submitted)
                           select shipPage.Id).Any();

            return result;
        }

        public bool CheckTransportationDocumentExistance(decimal number)
        {
            bool result = (from transportationPage in Db.TransportationLogBookPages
                           where transportationPage.PageNum == number
                                 && transportationPage.Status == nameof(LogBookPageStatusesEnum.Submitted)
                           select transportationPage.Id).Any();

            return result;
        }

        public bool CheckAdmissionDocumentExistance(decimal number)
        {
            bool result = (from admissionPage in Db.AdmissionLogBookPages
                           where admissionPage.PageNum == number
                                 && admissionPage.Status == nameof(LogBookPageStatusesEnum.Submitted)
                           select admissionPage.Id).Any();

            return result;
        }

        // Simple audits
        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.LogBooks, id);
        }

        public SimpleAuditDTO GetLogBookPageProductAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.LogBookPageProducts, id);
        }

        // Helpers
        private IQueryable<LogBookRegisterDTO> GetAllLogBooks(bool showInactive, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<LogBookRegisterHelper> baseQuery = GetAllLogBooksBaseQuery(showInactive, permittedLogBookTypes);
            IQueryable<LogBookRegisterDTO> result = FinalizeGetLogBooks(baseQuery);

            return result;
        }

        private IQueryable<LogBookRegisterHelper> GetAllLogBooksBaseQuery(bool showInactive, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<string> permittedLogBookTypeStrings = permittedLogBookTypes.Select(x => x.ToString()).ToList();

            IQueryable<LogBookRegisterHelper> query = from logBook in Db.LogBooks
                                                      join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                                      join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                                                      join ship in Db.ShipsRegister on logBook.ShipId equals ship.Id into logBookShip
                                                      from ship in logBookShip.DefaultIfEmpty()
                                                      join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id into regBuyers
                                                      from registeredBuyer in regBuyers.DefaultIfEmpty()
                                                      join personBuyer in Db.Persons on registeredBuyer.SubmittedForPersonId equals personBuyer.Id into regPersonBuyers
                                                      from registeredPersonBuyer in regPersonBuyers.DefaultIfEmpty()
                                                      join legalBuyer in Db.Legals on registeredBuyer.SubmittedForLegalId equals legalBuyer.Id into regLegalBuyers
                                                      from registeredLegalBuyer in regLegalBuyers.DefaultIfEmpty()
                                                      join person in Db.Persons on logBook.PersonId equals person.Id into logBookP
                                                      from logBookPerson in logBookP.DefaultIfEmpty()
                                                      join legal in Db.Legals on logBook.LegalId equals legal.Id into logBookL
                                                      from logBookLegal in logBookL.DefaultIfEmpty()
                                                      join aquaculture in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquaculture.Id into aquaFacil
                                                      from aquacultureFacility in aquaFacil.DefaultIfEmpty()
                                                      join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId into lbPermitLicense
                                                      from logBookPermitLicense in lbPermitLicense.DefaultIfEmpty()
                                                      join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id into pl
                                                      from permitLicense in pl.DefaultIfEmpty()
                                                      join applPermitLicense in Db.Applications on permitLicense.ApplicationId equals applPermitLicense.Id into applPl
                                                      from applPermitLicense in applPl.DefaultIfEmpty()
                                                      where logBook.IsActive == !showInactive && permittedLogBookTypeStrings.Contains(logBookType.Code)
                                                      select new LogBookRegisterHelper
                                                      {
                                                          Id = logBook.Id,
                                                          Number = logBook.LogNum,
                                                          TypeId = logBook.LogBookTypeId,
                                                          StatusId = logBook.StatusId,
                                                          StatusName = logBookStatus.Name,
                                                          IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                                          LogBookTypeCode = logBookType.Code,
                                                          Type = logBookType.Name,
                                                          LogBookOwnerTypeCode = logBook.LogBookOwnerType,
                                                          ShipId = ship != null ? ship.Id : default(int?),
                                                          AquacultureId = aquacultureFacility != null ? aquacultureFacility.Id : default(int?),
                                                          ShipName = ship != null ? ship.Name + ": " + ship.Cfr : null,
                                                          AquacultureFacilityName = aquacultureFacility != null
                                                                                    ? aquacultureFacility.Name + " (" + aquacultureFacility.UrorNum + ")"
                                                                                    : null,
                                                          RegisteredBuyerId = registeredBuyer != null ? registeredBuyer.Id : default(int?),
                                                          RegisteredPersonBuyerName = registeredPersonBuyer != null
                                                                                      ? registeredPersonBuyer.FirstName + " " + registeredPersonBuyer.LastName + " (" + registeredBuyer.RegistrationNum + ")"
                                                                                      : null,
                                                          RegisteredLegalBuyerName = registeredLegalBuyer != null
                                                                                     ? registeredLegalBuyer.Name + " (" + registeredBuyer.RegistrationNum + ")"
                                                                                     : null,
                                                          LogBookPersonName = logBookPerson != null
                                                                              ? logBookPerson.FirstName + " " + logBookPerson.LastName
                                                                              : null,
                                                          LogBookLegalName = logBookLegal != null ? logBookLegal.Name : null,
                                                          TeritorryUnitId = aquacultureFacility != null
                                                                            ? aquacultureFacility.TerritoryUnitId
                                                                               : registeredBuyer != null
                                                                                   ? registeredBuyer.TerritoryUnitId.Value
                                                                                   : applPermitLicense != null
                                                                                       ? applPermitLicense.TerritoryUnitId.Value
                                                                                       : -1,
                                                          OwnerEgnEik = logBookPerson != null
                                                                         ? logBookPerson.EgnLnc
                                                                         : logBookLegal != null
                                                                            ? logBookLegal.Eik
                                                                            : registeredLegalBuyer != null
                                                                                ? registeredLegalBuyer.Eik
                                                                                : registeredPersonBuyer != null
                                                                                    ? registeredPersonBuyer.EgnLnc
                                                                                    : null,
                                                          IssueDate = logBook.IssueDate,
                                                          FinishDate = logBook.FinishDate,
                                                          IsOnline = logBook.IsOnline,
                                                          StartPageNum = logBook.StartPageNum,
                                                          EndPageNum = logBook.EndPageNum,
                                                          OwnerRegisteredByerId = logBook.RegisteredBuyerId,
                                                          OwnerUnregisteredBuyerPersonId = logBook.PersonId,
                                                          OwnerUnregisteredBuyerLegalId = logBook.LegalId,
                                                          LogBookPermitLicenseStartPage = logBookPermitLicense != null
                                                                                          ? logBookPermitLicense.StartPageNum
                                                                                          : default,
                                                          LogBookPermitLicenseEndPage = logBookPermitLicense != null
                                                                                        ? logBookPermitLicense.EndPageNum
                                                                                        : default
                                                      };

            return query;
        }

        private IQueryable<LogBookRegisterDTO> GetParametersFilteredLogBooks(CatchesAndSalesAdministrationFilters filters, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<LogBookRegisterHelper> baseQuery = GetAllLogBooksBaseQuery(filters.ShowInactiveRecords, permittedLogBookTypes);

            if (filters.PageNumber.HasValue)
            {
                baseQuery = from logBook in baseQuery
                            where !logBook.IsOnline
                               && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                    && logBook.LogBookPermitLicenseEndPage.HasValue
                                    && logBook.LogBookPermitLicenseStartPage.Value <= filters.PageNumber.Value
                                    && logBook.LogBookPermitLicenseEndPage.Value >= filters.PageNumber.Value)
                                   || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                       && !logBook.LogBookPermitLicenseEndPage.HasValue
                                       && !string.IsNullOrEmpty(logBook.LogBookOwnerTypeCode)
                                       && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                       && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                       && logBook.StartPageNum <= filters.PageNumber.Value
                                       && logBook.EndPageNum >= filters.PageNumber.Value))
                            select logBook;
            }

            if (filters.OnlinePageNumber != null)
            {
                List<string> logBookNumberParts = filters.OnlinePageNumber.Split(ONLINE_PAGE_SEPARATOR).ToList();
                if (logBookNumberParts.Count > 1)
                {
                    long number;
                    bool isNumberCastSucc = long.TryParse(logBookNumberParts.Last(), out number);

                    if (isNumberCastSucc)
                    {
                        int charsToSubsctract = filters.OnlinePageNumber.Length - number.ToString("D5").Length - 1;
                        string logBookNumber = filters.OnlinePageNumber.Substring(0, charsToSubsctract);

                        baseQuery = from logBook in baseQuery
                                    where logBook.Number == logBookNumber
                                          && logBook.IsOnline
                                          && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                               && logBook.LogBookPermitLicenseEndPage.HasValue
                                               && logBook.LogBookPermitLicenseStartPage.Value <= number
                                              && logBook.LogBookPermitLicenseEndPage.Value >= number)
                                               || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                                   && !logBook.LogBookPermitLicenseEndPage.HasValue
                                                   && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                                   && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                                   && logBook.StartPageNum <= number
                                                  && logBook.EndPageNum >= number))
                                    select logBook;
                    }
                }
                else
                {
                    long number;
                    bool isNumberCastSucc = long.TryParse(filters.OnlinePageNumber, out number);
                    if (isNumberCastSucc)
                    {
                        baseQuery = from logBook in baseQuery
                                    where logBook.IsOnline
                                          && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                               && logBook.LogBookPermitLicenseEndPage.HasValue
                                               && logBook.LogBookPermitLicenseStartPage.Value <= number
                                               && logBook.LogBookPermitLicenseEndPage.Value >= number)
                                                || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                                   && !logBook.LogBookPermitLicenseEndPage.HasValue
                                                   && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                                   && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                                   && logBook.StartPageNum <= number
                                                   && logBook.EndPageNum >= number))
                                    select logBook;
                    }
                }
            }

            if (filters.TerritoryUnitId.HasValue)
            {
                if (!filters.FilterFishLogBookTeritorryUnitId.HasValue
                     && !filters.FilterFirstSaleLogBookTeritorryUnitId.HasValue
                     && !filters.FilterAdmissionLogBookTeritorryUnitId.HasValue
                     && !filters.FilterTransportationLogBookTeritorryUnitId.HasValue
                     && !filters.FilterAquacultureLogBookTeritorryUnitId.HasValue)
                {
                    baseQuery = from logBook in baseQuery
                                where logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                                select logBook;
                }
                else if (filters.FilterFishLogBookTeritorryUnitId.HasValue && !filters.FilterFishLogBookTeritorryUnitId.Value
                         && filters.FilterFirstSaleLogBookTeritorryUnitId.HasValue && !filters.FilterFirstSaleLogBookTeritorryUnitId.Value
                         && filters.FilterAdmissionLogBookTeritorryUnitId.HasValue && !filters.FilterAdmissionLogBookTeritorryUnitId.Value
                         && filters.FilterTransportationLogBookTeritorryUnitId.HasValue && !filters.FilterTransportationLogBookTeritorryUnitId.Value
                         && filters.FilterAquacultureLogBookTeritorryUnitId.HasValue && !filters.FilterAquacultureLogBookTeritorryUnitId.Value)
                {
                    baseQuery = from logBook in baseQuery
                                where logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                                select logBook;
                }
                else
                {
                    if (filters.FilterFishLogBookTeritorryUnitId.HasValue && filters.FilterFishLogBookTeritorryUnitId.Value)
                    {
                        baseQuery = from logBook in baseQuery
                                    where logBook.LogBookTypeCode != nameof(LogBookTypesEnum.Ship)
                                          || logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                                    select logBook;
                    }

                    if (filters.FilterFirstSaleLogBookTeritorryUnitId.HasValue && filters.FilterFirstSaleLogBookTeritorryUnitId.Value)
                    {
                        baseQuery = from logBook in baseQuery
                                    where logBook.LogBookTypeCode != nameof(LogBookTypesEnum.FirstSale)
                                          || logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                                    select logBook;
                    }

                    if (filters.FilterAdmissionLogBookTeritorryUnitId.HasValue && filters.FilterAdmissionLogBookTeritorryUnitId.Value)
                    {
                        baseQuery = from logBook in baseQuery
                                    where logBook.LogBookTypeCode != nameof(LogBookTypesEnum.Admission)
                                          || logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                                    select logBook;
                    }

                    if (filters.FilterTransportationLogBookTeritorryUnitId.HasValue && filters.FilterTransportationLogBookTeritorryUnitId.Value)
                    {
                        baseQuery = from logBook in baseQuery
                                    where logBook.LogBookTypeCode != nameof(LogBookTypesEnum.Transportation)
                                          || logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                                    select logBook;
                    }

                    if (filters.FilterAquacultureLogBookTeritorryUnitId.HasValue && filters.FilterAquacultureLogBookTeritorryUnitId.Value)
                    {
                        baseQuery = from logBook in baseQuery
                                    where logBook.LogBookTypeCode != nameof(LogBookTypesEnum.Aquaculture)
                                          || logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                                    select logBook;
                    }
                }
            }

            if (filters.LogBookTypeId != null)
            {
                baseQuery = from logBook in baseQuery
                            where logBook.TypeId == filters.LogBookTypeId
                            select logBook;
            }

            if (!string.IsNullOrEmpty(filters.LogBookNumber))
            {
                baseQuery = from logBook in baseQuery
                            where logBook.Number.ToLower().Contains(filters.LogBookNumber.ToLower())
                            select logBook;
            }

            if (filters.ShipId.HasValue)
            {
                baseQuery = from logBook in baseQuery
                            where logBook.ShipId.HasValue && logBook.ShipId.Value == filters.ShipId.Value
                            select logBook;
            }

            if (filters.AquacultureId.HasValue)
            {
                baseQuery = from logBook in baseQuery
                            where logBook.AquacultureId.HasValue && logBook.AquacultureId.Value == filters.AquacultureId.Value
                            select logBook;
            }

            if (filters.RegisteredBuyerId.HasValue)
            {
                baseQuery = from logBook in baseQuery
                            where logBook.RegisteredBuyerId.HasValue && logBook.RegisteredBuyerId.Value == filters.RegisteredBuyerId.Value
                            select logBook;
            }

            if (!string.IsNullOrEmpty(filters.OwnerEngEik))
            {
                baseQuery = from logBook in baseQuery
                            where logBook.OwnerEgnEik != null && logBook.OwnerEgnEik.ToLower() == filters.OwnerEngEik.ToLower()
                            select logBook;
            }

            if (filters.LogBookStatusIds != null && filters.LogBookStatusIds.Count > 0)
            {
                baseQuery = from logBook in baseQuery
                            where filters.LogBookStatusIds.Contains(logBook.StatusId)
                            select logBook;
            }

            if (filters.LogBookValidityStartDate.HasValue)
            {
                baseQuery = from logBook in baseQuery
                            where logBook.IssueDate <= filters.LogBookValidityStartDate.Value
                            select logBook;
            }

            if (filters.LogBookValidityEndDate.HasValue)
            {
                baseQuery = from logBook in baseQuery
                            where !logBook.FinishDate.HasValue || logBook.FinishDate.Value > filters.LogBookValidityEndDate.Value
                            select logBook;
            }

            if (filters.DocumentNumber.HasValue)
            {
                baseQuery = from logBook in baseQuery
                            join firstSalePage in Db.FirstSaleLogBookPages on logBook.Id equals firstSalePage.LogBookId into lbFirstSale
                            from firstSalePage in lbFirstSale.DefaultIfEmpty()
                            join admissionPage in Db.AdmissionLogBookPages on logBook.Id equals admissionPage.LogBookId into lbAdmission
                            from admissionPage in lbAdmission.DefaultIfEmpty()
                            join transportationPage in Db.TransportationLogBookPages on logBook.Id equals transportationPage.LogBookId into lbTransportation
                            from transportationPage in lbTransportation.DefaultIfEmpty()
                            where (firstSalePage != null && firstSalePage.PageNum == filters.DocumentNumber.Value)
                                    || (admissionPage != null && admissionPage.PageNum == filters.DocumentNumber.Value)
                                    || (transportationPage != null && transportationPage.PageNum == filters.DocumentNumber.Value)
                            select logBook;
            }

            IQueryable<LogBookRegisterDTO> result = FinalizeGetLogBooks(baseQuery);

            return result;
        }

        private IQueryable<LogBookRegisterDTO> GetFreeTextFilteredLogBooks(CatchesAndSalesAdministrationFilters filters, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            string text = filters.FreeTextSearch.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);
            decimal pageNum;
            bool pageNumParse = decimal.TryParse(text, out pageNum);

            IQueryable<LogBookRegisterHelper> baseQuery = GetAllLogBooksBaseQuery(filters.ShowInactiveRecords, permittedLogBookTypes);

            baseQuery = from entry in baseQuery
                        where entry.Number.ToLower().Contains(text)
                               || (searchDate.HasValue && entry.IssueDate <= searchDate.Value && entry.FinishDate >= searchDate.Value)
                               || (entry.OwnerEgnEik != null && entry.OwnerEgnEik == text)
                               || (entry.ShipName != null && entry.ShipName.ToLower().Contains(text))
                               || (entry.AquacultureFacilityName != null && entry.AquacultureFacilityName.ToLower().Contains(text))
                               || (entry.RegisteredPersonBuyerName != null && entry.RegisteredPersonBuyerName.ToLower().Contains(text))
                               || (entry.RegisteredLegalBuyerName != null && entry.RegisteredLegalBuyerName.ToLower().Contains(text))
                               || (entry.LogBookPersonName != null && entry.LogBookPersonName.ToLower().Contains(text))
                               || (entry.LogBookLegalName != null && entry.LogBookLegalName.ToLower().Contains(text))
                               || (pageNumParse && entry.StartPageNum <= pageNum && entry.EndPageNum > pageNum)
                               || entry.StatusName.ToLower().Contains(text)
                        select entry;

            if (filters.FilterFishLogBookTeritorryUnitId.HasValue && filters.FilterFishLogBookTeritorryUnitId.Value)
            {
                baseQuery = from logBook in baseQuery
                            where logBook.LogBookTypeCode != nameof(LogBookTypesEnum.Ship)
                              || logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                            select logBook;
            }

            if (filters.FilterFirstSaleLogBookTeritorryUnitId.HasValue && filters.FilterFirstSaleLogBookTeritorryUnitId.Value)
            {
                baseQuery = from logBook in baseQuery
                            where logBook.LogBookTypeCode != nameof(LogBookTypesEnum.FirstSale)
                              || logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                            select logBook;
            }

            if (filters.FilterAdmissionLogBookTeritorryUnitId.HasValue && filters.FilterAdmissionLogBookTeritorryUnitId.Value)
            {
                baseQuery = from logBook in baseQuery
                            where logBook.LogBookTypeCode != nameof(LogBookTypesEnum.Admission)
                              || logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                            select logBook;
            }

            if (filters.FilterTransportationLogBookTeritorryUnitId.HasValue && filters.FilterTransportationLogBookTeritorryUnitId.Value)
            {
                baseQuery = from logBook in baseQuery
                            where logBook.LogBookTypeCode != nameof(LogBookTypesEnum.Transportation)
                              || logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                            select logBook;
            }

            if (filters.FilterAquacultureLogBookTeritorryUnitId.HasValue && filters.FilterAquacultureLogBookTeritorryUnitId.Value)
            {
                baseQuery = from logBook in baseQuery
                            where logBook.LogBookTypeCode != nameof(LogBookTypesEnum.Aquaculture)
                              || logBook.TeritorryUnitId == filters.TerritoryUnitId.Value
                            select logBook;
            }

            IQueryable<LogBookRegisterDTO> result = FinalizeGetLogBooks(baseQuery);

            return result;
        }

        private IQueryable<LogBookRegisterDTO> FinalizeGetLogBooks(IQueryable<LogBookRegisterHelper> baseQuery)
        {
            IQueryable<LogBookRegisterDTO> result = from logBook in baseQuery
                                                    group logBook by new
                                                    {
                                                        logBook.Id,
                                                        logBook.Number,
                                                        logBook.TypeId,
                                                        logBook.LogBookTypeCode,
                                                        logBook.Type,
                                                        logBook.StatusName,
                                                        logBook.IsLogBookFinished,
                                                        logBook.ShipName,
                                                        logBook.AquacultureFacilityName,
                                                        logBook.RegisteredPersonBuyerName,
                                                        logBook.RegisteredLegalBuyerName,
                                                        logBook.LogBookPersonName,
                                                        logBook.LogBookLegalName,
                                                        logBook.IssueDate,
                                                        logBook.FinishDate,
                                                        logBook.IsOnline,
                                                        logBook.StartPageNum,
                                                        logBook.EndPageNum,
                                                        logBook.OwnerRegisteredByerId,
                                                        logBook.OwnerUnregisteredBuyerPersonId,
                                                        logBook.OwnerUnregisteredBuyerLegalId
                                                    } into lb
                                                    orderby lb.Key.IssueDate descending
                                                    select new LogBookRegisterDTO
                                                    {
                                                        Id = lb.Key.Id,
                                                        Number = lb.Key.Number,
                                                        TypeCode = Enum.Parse<LogBookTypesEnum>(lb.Key.LogBookTypeCode),
                                                        Type = lb.Key.Type,
                                                        StatusName = lb.Key.StatusName,
                                                        IsLogBookFinished = lb.Key.IsLogBookFinished,
                                                        IsOnline = lb.Key.IsOnline,
                                                        Name = lb.Key.ShipName != null
                                                               ? lb.Key.ShipName
                                                               : lb.Key.AquacultureFacilityName != null
                                                                 ? lb.Key.AquacultureFacilityName
                                                                 : lb.Key.RegisteredPersonBuyerName != null
                                                                     ? lb.Key.RegisteredPersonBuyerName
                                                                     : lb.Key.RegisteredLegalBuyerName != null
                                                                         ? lb.Key.RegisteredLegalBuyerName
                                                                         : lb.Key.LogBookPersonName != null
                                                                             ? lb.Key.LogBookPersonName
                                                                             : lb.Key.LogBookLegalName != null
                                                                                 ? lb.Key.LogBookLegalName
                                                                                 : "",
                                                        IssueDate = lb.Key.IssueDate,
                                                        FinishDate = lb.Key.FinishDate,
                                                        OwnerType = lb.Key.OwnerRegisteredByerId != null
                                                                    ? LogBookPagePersonTypesEnum.RegisteredBuyer
                                                                    : lb.Key.OwnerUnregisteredBuyerPersonId != null
                                                                        ? LogBookPagePersonTypesEnum.Person
                                                                        : lb.Key.OwnerUnregisteredBuyerLegalId != null
                                                                            ? LogBookPagePersonTypesEnum.LegalPerson
                                                                            : null
                                                    };

            return result;
        }

        private IQueryable<LogBookRegisterDTO> GetAllPublicLogBooks(bool showInactive, int userId, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            PermittedPublicLogBookBaseQueries baseQueries = GetAllPublicLogBooksBaseQuery(showInactive, userId, permittedLogBookTypes);
            IQueryable<LogBookRegisterDTO> result = FinalizeGetPublicLogBooks(baseQueries);

            return result;
        }

        private PermittedPublicLogBookBaseQueries GetAllPublicLogBooksBaseQuery(bool showInactive,
                                                                                int userId,
                                                                                List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<string> logBookReadPermissions = new List<string>();
            PermittedPublicLogBookBaseQueries result = new PermittedPublicLogBookBaseQueries();

            foreach (LogBookTypesEnum logBookType in permittedLogBookTypes)
            {
                switch (logBookType)
                {
                    case LogBookTypesEnum.Ship: logBookReadPermissions.Add(Permissions.FishLogBookRead); break;
                    case LogBookTypesEnum.FirstSale: logBookReadPermissions.Add(Permissions.FirstSaleLogBookRead); break;
                    case LogBookTypesEnum.Admission: logBookReadPermissions.Add(Permissions.AdmissionLogBookRead); break;
                    case LogBookTypesEnum.Transportation: logBookReadPermissions.Add(Permissions.TransportationLogBookRead); break;
                    case LogBookTypesEnum.Aquaculture: logBookReadPermissions.Add(Permissions.AquacultureLogBookRead); break;
                    default: throw new ArgumentException($"Unknown log book type: {logBookType.ToString()}");
                }
            }

            ILookup<string, string> legalPermissions = (from legal in Db.Legals
                                                        join userLegal in Db.UserLegals on legal.Id equals userLegal.LegalId
                                                        join rolePermission in Db.RolePermissions on userLegal.RoleId equals rolePermission.RoleId
                                                        join permission in Db.Npermissions on rolePermission.PermissionId equals permission.Id
                                                        where userLegal.UserId == userId
                                                              && logBookReadPermissions.Contains(permission.Name)
                                                              && userLegal.IsActive
                                                              && legal.RecordType == nameof(RecordTypesEnum.Register)
                                                        select new
                                                        {
                                                            legal.Eik,
                                                            PermissionCode = permission.Name
                                                        }).ToLookup(x => x.PermissionCode, y => y.Eik);

            HashSet<string> shipLogBookPermittedLegals = new HashSet<string>(legalPermissions[Permissions.FishLogBookRead]);
            HashSet<string> firstSaleLogBookPermittedLegals = new HashSet<string>(legalPermissions[Permissions.FirstSaleLogBookRead]);
            HashSet<string> admissionLogBookPermittedLegals = new HashSet<string>(legalPermissions[Permissions.AdmissionLogBookRead]);
            HashSet<string> transportationLogBookPermittedLegals = new HashSet<string>(legalPermissions[Permissions.TransportationLogBookRead]);
            HashSet<string> aquacultureLogBookPermittedLegals = new HashSet<string>(legalPermissions[Permissions.AquacultureLogBookRead]);

            EgnLncDTO userIdentifier = (from user in Db.Users
                                        join person in Db.Persons on user.PersonId equals person.Id
                                        where user.Id == userId
                                        select new EgnLncDTO
                                        {
                                            EgnLnc = person.EgnLnc,
                                            IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                                        }).First();

            HashSet<int> userPersonIds = (from person in Db.Persons
                                          where person.EgnLnc == userIdentifier.EgnLnc
                                                && person.IdentifierType == userIdentifier.IdentifierType.ToString()
                                          select person.Id).ToHashSet();

            result.PersonalLogBooks = from logBook in Db.LogBooks
                                      join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                      join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id

                                      join ship in Db.ShipsRegister on logBook.ShipId equals ship.Id into logBookShip
                                      from ship in logBookShip.DefaultIfEmpty()
                                      join aquacultureOwner in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquacultureOwner.Id into lbAqua
                                      from aquacultureOwner in lbAqua.DefaultIfEmpty()

                                      join personOwner in Db.Persons on logBook.PersonId equals personOwner.Id into lbPerson
                                      from personOwner in lbPerson.DefaultIfEmpty()

                                      join buyerOwner in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyerOwner.Id into lbBuyer
                                      from buyerOwner in lbBuyer.DefaultIfEmpty()
                                      join buyerPerson in Db.Persons on buyerOwner.SubmittedForPersonId equals buyerPerson.Id into lbBuyerPerson
                                      from buyerPerson in lbBuyerPerson.DefaultIfEmpty()
                                      join buyerLegal in Db.Legals on buyerOwner.SubmittedForLegalId equals buyerLegal.Id into lbBuyerLegal
                                      from buyerLegal in lbBuyerLegal.DefaultIfEmpty()
                                      join legalOwner in Db.Legals on logBook.LegalId equals legalOwner.Id into lbLegal
                                      from legalOwner in lbLegal.DefaultIfEmpty()
                                      join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId into lbLicense
                                      from logBookPermitLicense in lbLicense.DefaultIfEmpty()
                                      where logBook.IsActive == !showInactive
                                            && logBook.PersonId.HasValue
                                            && userPersonIds.Contains(logBook.PersonId.Value)
                                      select new LogBookRegisterHelper
                                      {
                                          Id = logBook.Id,
                                          Number = logBook.LogNum,
                                          TypeId = logBook.LogBookTypeId,
                                          StatusId = logBook.StatusId,
                                          StatusName = logBookStatus.Name,
                                          IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                          LogBookTypeCode = logBookType.Code,
                                          Type = logBookType.Name,
                                          LogBookOwnerTypeCode = logBook.LogBookOwnerType,
                                          ShipId = ship != null ? ship.Id : null,
                                          AquacultureId = aquacultureOwner != null ? (int?)aquacultureOwner.Id : null,
                                          ShipName = ship != null ? ship.Name : null,
                                          ShipExtMark = ship != null ? ship.ExternalMark : null,

                                          AquacultureFacilityName = aquacultureOwner != null
                                                                       ? aquacultureOwner.Name + " (" + aquacultureOwner.UrorNum + ")"
                                                                       : null,
                                          RegisteredBuyerId = buyerOwner != null ? buyerOwner.Id : null,
                                          RegisteredPersonBuyerName = buyerPerson != null
                                                                         ? buyerPerson.FirstName + " " + buyerPerson.LastName + " (" + buyerOwner.RegistrationNum + ")"
                                                                         : null,
                                          RegisteredLegalBuyerName = buyerLegal.Name,
                                          LogBookPersonName = personOwner != null
                                                                 ? personOwner.FirstName + " " + personOwner.LastName
                                                                 : null,
                                          LogBookLegalName = legalOwner != null ? legalOwner.Name : null,
                                          OwnerEgnEik = personOwner != null
                                                           ? personOwner.EgnLnc
                                                           : legalOwner != null
                                                             ? legalOwner.Eik
                                                             : buyerLegal != null
                                                               ? buyerLegal.Eik
                                                               : buyerPerson != null
                                                                   ? buyerPerson.EgnLnc
                                                                   : null,

                                          IssueDate = logBook.IssueDate,
                                          FinishDate = logBook.FinishDate,
                                          IsOnline = logBook.IsOnline,
                                          StartPageNum = logBook.StartPageNum,
                                          EndPageNum = logBook.EndPageNum,
                                          OwnerRegisteredByerId = logBook.RegisteredBuyerId,
                                          OwnerUnregisteredBuyerPersonId = logBook.PersonId,
                                          OwnerUnregisteredBuyerLegalId = logBook.LegalId,
                                          LogBookPermitLicenseStartPage = logBookPermitLicense != null
                                                                                    ? logBookPermitLicense.StartPageNum
                                                                                    : default,
                                          LogBookPermitLicenseEndPage = logBookPermitLicense != null
                                                                                  ? logBookPermitLicense.EndPageNum
                                                                                  : default
                                      };

            result.LegalLogBooks = from logBook in Db.LogBooks
                                   join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                   join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id

                                   join ship in Db.ShipsRegister on logBook.ShipId equals ship.Id into logBookShip
                                   from ship in logBookShip.DefaultIfEmpty()
                                   join aquacultureOwner in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquacultureOwner.Id into lbAqua
                                   from aquacultureOwner in lbAqua.DefaultIfEmpty()

                                   join legalOwner in Db.Legals on logBook.LegalId equals legalOwner.Id

                                   join buyerOwner in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyerOwner.Id into lbBuyer
                                   from buyerOwner in lbBuyer.DefaultIfEmpty()
                                   join buyerPerson in Db.Persons on buyerOwner.SubmittedForPersonId equals buyerPerson.Id into lbBuyerPerson
                                   from buyerPerson in lbBuyerPerson.DefaultIfEmpty()
                                   join buyerLegal in Db.Legals on buyerOwner.SubmittedForLegalId equals buyerLegal.Id into lbBuyerLegal
                                   from buyerLegal in lbBuyerLegal.DefaultIfEmpty()
                                   join personOwner in Db.Persons on logBook.PersonId equals personOwner.Id into lbPerson
                                   from personOwner in lbPerson.DefaultIfEmpty()
                                   join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId into lbLicense
                                   from logBookPermitLicense in lbLicense.DefaultIfEmpty()
                                   where logBook.IsActive == !showInactive
                                         && ((logBookType.Code == nameof(LogBookTypesEnum.Admission) && admissionLogBookPermittedLegals.Contains(legalOwner.Eik))
                                              || (logBookType.Code == nameof(LogBookTypesEnum.Transportation) && transportationLogBookPermittedLegals.Contains(legalOwner.Eik)))
                                   select new LogBookRegisterHelper
                                   {
                                       Id = logBook.Id,
                                       Number = logBook.LogNum,
                                       TypeId = logBook.LogBookTypeId,
                                       StatusId = logBook.StatusId,
                                       StatusName = logBookStatus.Name,
                                       IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                       LogBookTypeCode = logBookType.Code,
                                       Type = logBookType.Name,
                                       LogBookOwnerTypeCode = logBook.LogBookOwnerType,
                                       ShipId = ship != null ? ship.Id : null,
                                       AquacultureId = aquacultureOwner != null ? (int?)aquacultureOwner.Id : null,
                                       ShipName = ship != null ? ship.Name : null,
                                       ShipExtMark = ship != null ? ship.ExternalMark : null,

                                       AquacultureFacilityName = aquacultureOwner != null
                                                                    ? aquacultureOwner.Name + " (" + aquacultureOwner.UrorNum + ")"
                                                                    : null,
                                       RegisteredBuyerId = buyerOwner != null ? buyerOwner.Id : null,
                                       RegisteredPersonBuyerName = buyerPerson != null
                                                                      ? buyerPerson.FirstName + " " + buyerPerson.LastName + " (" + buyerOwner.RegistrationNum + ")"
                                                                      : null,
                                       RegisteredLegalBuyerName = buyerLegal != null
                                                                     ? buyerLegal.Name + " (" + buyerOwner.RegistrationNum + ")"
                                                                     : null,
                                       LogBookPersonName = personOwner != null
                                                              ? personOwner.FirstName + " " + personOwner.LastName
                                                              : null,
                                       LogBookLegalName = legalOwner != null ? legalOwner.Name : null,
                                       OwnerEgnEik = personOwner != null
                                                        ? personOwner.EgnLnc
                                                        : legalOwner != null
                                                          ? legalOwner.Eik
                                                          : buyerLegal != null
                                                            ? buyerLegal.Eik
                                                            : buyerPerson != null
                                                                ? buyerPerson.EgnLnc
                                                                : null,

                                       IssueDate = logBook.IssueDate,
                                       FinishDate = logBook.FinishDate,
                                       IsOnline = logBook.IsOnline,
                                       StartPageNum = logBook.StartPageNum,
                                       EndPageNum = logBook.EndPageNum,
                                       OwnerRegisteredByerId = logBook.RegisteredBuyerId,
                                       OwnerUnregisteredBuyerPersonId = logBook.PersonId,
                                       OwnerUnregisteredBuyerLegalId = logBook.LegalId,
                                       LogBookPermitLicenseStartPage = logBookPermitLicense != null
                                                                                    ? logBookPermitLicense.StartPageNum
                                                                                    : default,
                                       LogBookPermitLicenseEndPage = logBookPermitLicense != null
                                                                                  ? logBookPermitLicense.EndPageNum
                                                                                  : default
                                   };

            result.BuyerLegalLogBooks = from logBook in Db.LogBooks
                                        join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                        join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id

                                        join ship in Db.ShipsRegister on logBook.ShipId equals ship.Id into logBookShip
                                        from ship in logBookShip.DefaultIfEmpty()
                                        join aquacultureOwner in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquacultureOwner.Id into lbAqua
                                        from aquacultureOwner in lbAqua.DefaultIfEmpty()

                                        join buyerOwner in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyerOwner.Id
                                        join buyerLegal in Db.Legals on buyerOwner.SubmittedForLegalId equals buyerLegal.Id

                                        join buyerPerson in Db.Persons on buyerOwner.SubmittedForPersonId equals buyerPerson.Id into lbBuyerPerson
                                        from buyerPerson in lbBuyerPerson.DefaultIfEmpty()
                                        join personOwner in Db.Persons on logBook.PersonId equals personOwner.Id into lbPerson
                                        from personOwner in lbPerson.DefaultIfEmpty()
                                        join legalOwner in Db.Legals on logBook.LegalId equals legalOwner.Id into lbLegal
                                        from legalOwner in lbLegal.DefaultIfEmpty()
                                        join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId into lbLicense
                                        from logBookPermitLicense in lbLicense.DefaultIfEmpty()
                                        where logBook.IsActive == !showInactive
                                              && ((logBookType.Code == nameof(LogBookTypesEnum.FirstSale) && firstSaleLogBookPermittedLegals.Contains(buyerLegal.Eik))
                                                   || (logBookType.Code == nameof(LogBookTypesEnum.Admission) && admissionLogBookPermittedLegals.Contains(buyerLegal.Eik))
                                                   || (logBookType.Code == nameof(LogBookTypesEnum.Transportation) && transportationLogBookPermittedLegals.Contains(buyerLegal.Eik))
                                                 )
                                        select new LogBookRegisterHelper
                                        {
                                            Id = logBook.Id,
                                            Number = logBook.LogNum,
                                            TypeId = logBook.LogBookTypeId,
                                            StatusId = logBook.StatusId,
                                            StatusName = logBookStatus.Name,
                                            IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                            LogBookTypeCode = logBookType.Code,
                                            Type = logBookType.Name,
                                            LogBookOwnerTypeCode = logBook.LogBookOwnerType,
                                            ShipId = ship != null ? ship.Id : default,
                                            AquacultureId = aquacultureOwner != null ? (int?)aquacultureOwner.Id : default,
                                            ShipName = ship != null ? ship.Name : null,

                                            ShipExtMark = ship != null ? ship.ExternalMark : null,

                                            AquacultureFacilityName = aquacultureOwner != null
                                                                         ? aquacultureOwner.Name + " (" + aquacultureOwner.UrorNum + ")"
                                                                         : null,
                                            RegisteredBuyerId = buyerOwner != null ? buyerOwner.Id : default(int?),
                                            RegisteredPersonBuyerName = buyerPerson != null
                                                                           ? buyerPerson.FirstName + " " + buyerPerson.LastName + " (" + buyerOwner.RegistrationNum + ")"
                                                                           : null,
                                            RegisteredLegalBuyerName = buyerLegal != null
                                                                          ? buyerLegal.Name + " (" + buyerOwner.RegistrationNum + ")"
                                                                          : null,
                                            LogBookPersonName = personOwner != null
                                                                   ? personOwner.FirstName + " " + personOwner.LastName
                                                                   : null,
                                            LogBookLegalName = legalOwner != null ? legalOwner.Name : null,
                                            OwnerEgnEik = personOwner != null
                                                             ? personOwner.EgnLnc
                                                             : legalOwner != null
                                                               ? legalOwner.Eik
                                                               : buyerLegal != null
                                                                 ? buyerLegal.Eik
                                                                 : buyerPerson != null
                                                                     ? buyerPerson.EgnLnc
                                                                     : null,

                                            IssueDate = logBook.IssueDate,
                                            FinishDate = logBook.FinishDate,
                                            IsOnline = logBook.IsOnline,
                                            StartPageNum = logBook.StartPageNum,
                                            EndPageNum = logBook.EndPageNum,
                                            OwnerRegisteredByerId = logBook.RegisteredBuyerId,
                                            OwnerUnregisteredBuyerPersonId = logBook.PersonId,
                                            OwnerUnregisteredBuyerLegalId = logBook.LegalId,
                                            LogBookPermitLicenseStartPage = logBookPermitLicense != null
                                                                                         ? logBookPermitLicense.StartPageNum
                                                                                         : default,
                                            LogBookPermitLicenseEndPage = logBookPermitLicense != null
                                                                                       ? logBookPermitLicense.EndPageNum
                                                                                       : default
                                        };

            result.PersonBuyerLogBooks = from logBook in Db.LogBooks
                                         join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                         join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id

                                         join ship in Db.ShipsRegister on logBook.ShipId equals ship.Id into logBookShip
                                         from ship in logBookShip.DefaultIfEmpty()
                                         join aquacultureOwner in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquacultureOwner.Id into lbAqua
                                         from aquacultureOwner in lbAqua.DefaultIfEmpty()

                                         join buyerOwner in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyerOwner.Id

                                         join buyerPerson in Db.Persons on buyerOwner.SubmittedForPersonId equals buyerPerson.Id into lbBuyerPerson
                                         from buyerPerson in lbBuyerPerson.DefaultIfEmpty()
                                         join buyerLegal in Db.Legals on buyerOwner.SubmittedForLegalId equals buyerLegal.Id into lbBuyerLegal
                                         from buyerLegal in lbBuyerLegal.DefaultIfEmpty()
                                         join personOwner in Db.Persons on logBook.PersonId equals personOwner.Id into lbPerson
                                         from personOwner in lbPerson.DefaultIfEmpty()
                                         join legalOwner in Db.Legals on logBook.LegalId equals legalOwner.Id into lbLegal
                                         from legalOwner in lbLegal.DefaultIfEmpty()
                                         join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId into lbLicense
                                         from logBookPermitLicense in lbLicense.DefaultIfEmpty()
                                         where logBook.IsActive == !showInactive
                                               && buyerOwner.SubmittedForPersonId.HasValue
                                               && userPersonIds.Contains(buyerOwner.SubmittedForPersonId.Value)
                                         select new LogBookRegisterHelper
                                         {
                                             Id = logBook.Id,
                                             Number = logBook.LogNum,
                                             TypeId = logBook.LogBookTypeId,
                                             StatusId = logBook.StatusId,
                                             StatusName = logBookStatus.Name,
                                             IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                             LogBookTypeCode = logBookType.Code,
                                             Type = logBookType.Name,
                                             LogBookOwnerTypeCode = logBook.LogBookOwnerType,
                                             ShipId = ship != null ? ship.Id : default,
                                             AquacultureId = aquacultureOwner != null ? (int?)aquacultureOwner.Id : default,
                                             ShipName = ship != null ? ship.Name : null,

                                             ShipExtMark = ship != null ? ship.ExternalMark : null,

                                             AquacultureFacilityName = aquacultureOwner != null
                                                                    ? aquacultureOwner.Name + " (" + aquacultureOwner.UrorNum + ")"
                                                                    : null,
                                             RegisteredBuyerId = buyerOwner != null ? buyerOwner.Id : default(int?),
                                             RegisteredPersonBuyerName = buyerPerson != null
                                                                      ? buyerPerson.FirstName + " " + buyerPerson.LastName + " (" + buyerOwner.RegistrationNum + ")"
                                                                      : null,
                                             RegisteredLegalBuyerName = buyerLegal != null
                                                                     ? buyerLegal.Name + " (" + buyerOwner.RegistrationNum + ")"
                                                                     : null,
                                             LogBookPersonName = personOwner != null
                                                              ? personOwner.FirstName + " " + personOwner.LastName
                                                              : null,
                                             LogBookLegalName = legalOwner != null ? legalOwner.Name : null,
                                             OwnerEgnEik = personOwner != null
                                                        ? personOwner.EgnLnc
                                                        : legalOwner != null
                                                          ? legalOwner.Eik
                                                          : buyerLegal != null
                                                            ? buyerLegal.Eik
                                                            : buyerPerson != null
                                                                ? buyerPerson.EgnLnc
                                                                : null,
                                             IssueDate = logBook.IssueDate,
                                             FinishDate = logBook.FinishDate,
                                             IsOnline = logBook.IsOnline,
                                             StartPageNum = logBook.StartPageNum,
                                             EndPageNum = logBook.EndPageNum,
                                             OwnerRegisteredByerId = logBook.RegisteredBuyerId,
                                             OwnerUnregisteredBuyerPersonId = logBook.PersonId,
                                             OwnerUnregisteredBuyerLegalId = logBook.LegalId,
                                             LogBookPermitLicenseStartPage = logBookPermitLicense != null
                                                                                    ? logBookPermitLicense.StartPageNum
                                                                                    : default,
                                             LogBookPermitLicenseEndPage = logBookPermitLicense != null
                                                                                  ? logBookPermitLicense.EndPageNum
                                                                                  : default
                                         };

            if (aquacultureLogBookPermittedLegals.Count > 0)
            {
                result.AquacultureLegalLogBooks = from logBook in Db.LogBooks
                                                  join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                                  join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id

                                                  join ship in Db.ShipsRegister on logBook.ShipId equals ship.Id into logBookShip
                                                  from ship in logBookShip.DefaultIfEmpty()

                                                  join aquacultureOwner in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquacultureOwner.Id
                                                  join aquacultureLegalOwner in Db.Legals on aquacultureOwner.SubmittedForLegalId equals aquacultureLegalOwner.Id

                                                  join buyerOwner in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyerOwner.Id into lbBuyer
                                                  from buyerOwner in lbBuyer.DefaultIfEmpty()
                                                  join buyerPerson in Db.Persons on buyerOwner.SubmittedForPersonId equals buyerPerson.Id into lbBuyerPerson
                                                  from buyerPerson in lbBuyerPerson.DefaultIfEmpty()
                                                  join buyerLegal in Db.Legals on buyerOwner.SubmittedForLegalId equals buyerLegal.Id into lbBuyerLegal
                                                  from buyerLegal in lbBuyerLegal.DefaultIfEmpty()
                                                  join personOwner in Db.Persons on logBook.PersonId equals personOwner.Id into lbPerson
                                                  from personOwner in lbPerson.DefaultIfEmpty()
                                                  join legalOwner in Db.Legals on logBook.LegalId equals legalOwner.Id into lbLegal
                                                  from legalOwner in lbLegal.DefaultIfEmpty()
                                                  join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId into lbLicense
                                                  from logBookPermitLicense in lbLicense.DefaultIfEmpty()
                                                  where logBook.IsActive == !showInactive
                                                        && logBookType.Code == nameof(LogBookTypesEnum.Aquaculture)
                                                        && aquacultureLogBookPermittedLegals.Contains(aquacultureLegalOwner.Eik)
                                                  select new LogBookRegisterHelper
                                                  {
                                                      Id = logBook.Id,
                                                      Number = logBook.LogNum,
                                                      TypeId = logBook.LogBookTypeId,
                                                      StatusId = logBook.StatusId,
                                                      StatusName = logBookStatus.Name,
                                                      IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                                      LogBookTypeCode = logBookType.Code,
                                                      Type = logBookType.Name,
                                                      LogBookOwnerTypeCode = logBook.LogBookOwnerType,
                                                      ShipId = ship != null ? ship.Id : default,
                                                      AquacultureId = aquacultureOwner != null ? (int?)aquacultureOwner.Id : default,
                                                      ShipName = ship != null ? ship.Name : null,
                                                      ShipExtMark = ship != null ? ship.ExternalMark : null,

                                                      AquacultureFacilityName = aquacultureOwner != null
                                                                             ? aquacultureOwner.Name + " (" + aquacultureOwner.UrorNum + ")"
                                                                             : null,
                                                      RegisteredBuyerId = buyerOwner != null ? buyerOwner.Id : default(int?),
                                                      RegisteredPersonBuyerName = buyerPerson != null
                                                                               ? buyerPerson.FirstName + " " + buyerPerson.LastName + " (" + buyerOwner.RegistrationNum + ")"
                                                                               : null,
                                                      RegisteredLegalBuyerName = buyerLegal != null
                                                                              ? buyerLegal.Name + " (" + buyerOwner.RegistrationNum + ")"
                                                                              : null,
                                                      LogBookPersonName = personOwner != null
                                                                       ? personOwner.FirstName + " " + personOwner.LastName
                                                                       : null,
                                                      LogBookLegalName = legalOwner != null ? legalOwner.Name : null,
                                                      OwnerEgnEik = personOwner != null
                                                                 ? personOwner.EgnLnc
                                                                 : legalOwner != null
                                                                   ? legalOwner.Eik
                                                                   : buyerLegal != null
                                                                     ? buyerLegal.Eik
                                                                     : buyerPerson != null
                                                                         ? buyerPerson.EgnLnc
                                                                         : null,

                                                      IssueDate = logBook.IssueDate,
                                                      FinishDate = logBook.FinishDate,
                                                      IsOnline = logBook.IsOnline,
                                                      StartPageNum = logBook.StartPageNum,
                                                      EndPageNum = logBook.EndPageNum,
                                                      OwnerRegisteredByerId = logBook.RegisteredBuyerId,
                                                      OwnerUnregisteredBuyerPersonId = logBook.PersonId,
                                                      OwnerUnregisteredBuyerLegalId = logBook.LegalId,
                                                      LogBookPermitLicenseStartPage = logBookPermitLicense != null
                                                                                             ? logBookPermitLicense.StartPageNum
                                                                                             : default,
                                                      LogBookPermitLicenseEndPage = logBookPermitLicense != null
                                                                                           ? logBookPermitLicense.EndPageNum
                                                                                           : default
                                                  };
            }

            result.AquaculturePersonaLogBooks = from logBook in Db.LogBooks
                                                join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                                join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id

                                                join ship in Db.ShipsRegister on logBook.ShipId equals ship.Id into logBookShip
                                                from ship in logBookShip.DefaultIfEmpty()

                                                join aquacultureOwner in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquacultureOwner.Id

                                                join buyerOwner in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyerOwner.Id into lbBuyer
                                                from buyerOwner in lbBuyer.DefaultIfEmpty()
                                                join buyerPerson in Db.Persons on buyerOwner.SubmittedForPersonId equals buyerPerson.Id into lbBuyerPerson
                                                from buyerPerson in lbBuyerPerson.DefaultIfEmpty()
                                                join buyerLegal in Db.Legals on buyerOwner.SubmittedForLegalId equals buyerLegal.Id into lbBuyerLegal
                                                from buyerLegal in lbBuyerLegal.DefaultIfEmpty()
                                                join personOwner in Db.Persons on logBook.PersonId equals personOwner.Id into lbPerson
                                                from personOwner in lbPerson.DefaultIfEmpty()
                                                join legalOwner in Db.Legals on logBook.LegalId equals legalOwner.Id into lbLegal
                                                from legalOwner in lbLegal.DefaultIfEmpty()
                                                join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId into lbLicense
                                                from logBookPermitLicense in lbLicense.DefaultIfEmpty()
                                                where logBook.IsActive == !showInactive
                                                      && aquacultureOwner.SubmittedForPersonId.HasValue
                                                      && userPersonIds.Contains(aquacultureOwner.SubmittedForPersonId.Value)
                                                select new LogBookRegisterHelper
                                                {
                                                    Id = logBook.Id,
                                                    Number = logBook.LogNum,
                                                    TypeId = logBook.LogBookTypeId,
                                                    StatusId = logBook.StatusId,
                                                    StatusName = logBookStatus.Name,
                                                    IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                                    LogBookTypeCode = logBookType.Code,
                                                    Type = logBookType.Name,
                                                    LogBookOwnerTypeCode = logBook.LogBookOwnerType,
                                                    ShipId = ship != null ? ship.Id : default,
                                                    AquacultureId = aquacultureOwner != null ? (int?)aquacultureOwner.Id : default,
                                                    ShipName = ship != null ? ship.Name : null,
                                                    ShipExtMark = ship != null ? ship.ExternalMark : null,

                                                    AquacultureFacilityName = aquacultureOwner != null
                                                                    ? aquacultureOwner.Name + " (" + aquacultureOwner.UrorNum + ")"
                                                                    : null,
                                                    RegisteredBuyerId = buyerOwner != null ? buyerOwner.Id : default(int?),
                                                    RegisteredPersonBuyerName = buyerPerson != null
                                                                      ? buyerPerson.FirstName + " " + buyerPerson.LastName + " (" + buyerOwner.RegistrationNum + ")"
                                                                      : null,
                                                    RegisteredLegalBuyerName = buyerLegal != null
                                                                     ? buyerLegal.Name + " (" + buyerOwner.RegistrationNum + ")"
                                                                     : null,
                                                    LogBookPersonName = personOwner != null
                                                              ? personOwner.FirstName + " " + personOwner.LastName
                                                              : null,
                                                    LogBookLegalName = legalOwner != null ? legalOwner.Name : null,
                                                    OwnerEgnEik = personOwner != null
                                                        ? personOwner.EgnLnc
                                                        : legalOwner != null
                                                          ? legalOwner.Eik
                                                          : buyerLegal != null
                                                            ? buyerLegal.Eik
                                                            : buyerPerson != null
                                                                ? buyerPerson.EgnLnc
                                                                : null,

                                                    IssueDate = logBook.IssueDate,
                                                    FinishDate = logBook.FinishDate,
                                                    IsOnline = logBook.IsOnline,
                                                    StartPageNum = logBook.StartPageNum,
                                                    EndPageNum = logBook.EndPageNum,
                                                    OwnerRegisteredByerId = logBook.RegisteredBuyerId,
                                                    OwnerUnregisteredBuyerPersonId = logBook.PersonId,
                                                    OwnerUnregisteredBuyerLegalId = logBook.LegalId,
                                                    LogBookPermitLicenseStartPage = logBookPermitLicense != null
                                                                                    ? logBookPermitLicense.StartPageNum
                                                                                    : default,
                                                    LogBookPermitLicenseEndPage = logBookPermitLicense != null
                                                                                  ? logBookPermitLicense.EndPageNum
                                                                                  : default
                                                };

            if (shipLogBookPermittedLegals.Count > 0)
            {
                result.ShipLegalLogBooks = from logBook in Db.LogBooks
                                           join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                           join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id

                                           join aquacultureOwner in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquacultureOwner.Id into lbAqua
                                           from aquacultureOwner in lbAqua.DefaultIfEmpty()

                                           join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId
                                           join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id
                                           join permitLicenseLegalOwner in Db.Legals on permitLicense.SubmittedForLegalId equals permitLicenseLegalOwner.Id
                                           join ship in Db.ShipsRegister on permitLicense.ShipId equals ship.Id

                                           join buyerOwner in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyerOwner.Id into lbBuyer
                                           from buyerOwner in lbBuyer.DefaultIfEmpty()
                                           join buyerPerson in Db.Persons on buyerOwner.SubmittedForPersonId equals buyerPerson.Id into lbBuyerPerson
                                           from buyerPerson in lbBuyerPerson.DefaultIfEmpty()
                                           join buyerLegal in Db.Legals on buyerOwner.SubmittedForLegalId equals buyerLegal.Id into lbBuyerLegal
                                           from buyerLegal in lbBuyerLegal.DefaultIfEmpty()
                                           join personOwner in Db.Persons on logBook.PersonId equals personOwner.Id into lbPerson
                                           from personOwner in lbPerson.DefaultIfEmpty()
                                           join legalOwner in Db.Legals on logBook.LegalId equals legalOwner.Id into lbLegal
                                           from legalOwner in lbLegal.DefaultIfEmpty()
                                           where logBook.IsActive == !showInactive
                                                   && logBookType.Code == nameof(LogBookTypesEnum.Ship)
                                                   && shipLogBookPermittedLegals.Contains(permitLicenseLegalOwner.Eik)
                                           select new LogBookRegisterHelper
                                           {
                                               Id = logBook.Id,
                                               Number = logBook.LogNum,
                                               TypeId = logBook.LogBookTypeId,
                                               StatusId = logBook.StatusId,
                                               StatusName = logBookStatus.Name,
                                               IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                               LogBookTypeCode = logBookType.Code,
                                               Type = logBookType.Name,
                                               LogBookOwnerTypeCode = logBook.LogBookOwnerType,
                                               ShipId = ship != null ? ship.Id : default,
                                               AquacultureId = aquacultureOwner != null ? (int?)aquacultureOwner.Id : default,
                                               ShipName = ship != null ? ship.Name : null,
                                               ShipExtMark = ship != null ? ship.ExternalMark : null,

                                               AquacultureFacilityName = aquacultureOwner != null
                                                                        ? aquacultureOwner.Name + " (" + aquacultureOwner.UrorNum + ")"
                                                                        : null,
                                               RegisteredBuyerId = buyerOwner != null ? buyerOwner.Id : default(int?),
                                               RegisteredPersonBuyerName = buyerPerson != null
                                                                          ? buyerPerson.FirstName + " " + buyerPerson.LastName + " (" + buyerOwner.RegistrationNum + ")"
                                                                          : null,
                                               RegisteredLegalBuyerName = buyerLegal != null
                                                                         ? buyerLegal.Name + " (" + buyerOwner.RegistrationNum + ")"
                                                                         : null,
                                               LogBookPersonName = personOwner != null
                                                                  ? personOwner.FirstName + " " + personOwner.LastName
                                                                  : null,
                                               LogBookLegalName = legalOwner != null ? legalOwner.Name : null,
                                               OwnerEgnEik = personOwner != null
                                                            ? personOwner.EgnLnc
                                                            : legalOwner != null
                                                              ? legalOwner.Eik
                                                              : buyerLegal != null
                                                                ? buyerLegal.Eik
                                                                : buyerPerson != null
                                                                    ? buyerPerson.EgnLnc
                                                                    : null,

                                               IssueDate = logBook.IssueDate,
                                               FinishDate = logBook.FinishDate,
                                               IsOnline = logBook.IsOnline,
                                               StartPageNum = logBook.StartPageNum,
                                               EndPageNum = logBook.EndPageNum,
                                               OwnerRegisteredByerId = logBook.RegisteredBuyerId,
                                               OwnerUnregisteredBuyerPersonId = logBook.PersonId,
                                               OwnerUnregisteredBuyerLegalId = logBook.LegalId,
                                               LogBookPermitLicenseStartPage = logBookPermitLicense.StartPageNum,
                                               LogBookPermitLicenseEndPage = logBookPermitLicense.EndPageNum
                                           };
            }


            result.ShipPersonLogBooks = from logBook in Db.LogBooks
                                        join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                        join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id

                                        join aquacultureOwner in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquacultureOwner.Id into lbAqua
                                        from aquacultureOwner in lbAqua.DefaultIfEmpty()

                                        join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId
                                        join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id
                                        join ship in Db.ShipsRegister on permitLicense.ShipId equals ship.Id

                                        join buyerOwner in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyerOwner.Id into lbBuyer
                                        from buyerOwner in lbBuyer.DefaultIfEmpty()
                                        join buyerPerson in Db.Persons on buyerOwner.SubmittedForPersonId equals buyerPerson.Id into lbBuyerPerson
                                        from buyerPerson in lbBuyerPerson.DefaultIfEmpty()
                                        join buyerLegal in Db.Legals on buyerOwner.SubmittedForLegalId equals buyerLegal.Id into lbBuyerLegal
                                        from buyerLegal in lbBuyerLegal.DefaultIfEmpty()
                                        join personOwner in Db.Persons on logBook.PersonId equals personOwner.Id into lbPerson
                                        from personOwner in lbPerson.DefaultIfEmpty()
                                        join legalOwner in Db.Legals on logBook.LegalId equals legalOwner.Id into lbLegal
                                        from legalOwner in lbLegal.DefaultIfEmpty()
                                        where logBook.IsActive == !showInactive
                                                && permitLicense.SubmittedForPersonId.HasValue
                                                && userPersonIds.Contains(permitLicense.SubmittedForPersonId.Value)
                                        select new LogBookRegisterHelper
                                        {
                                            Id = logBook.Id,
                                            Number = logBook.LogNum,
                                            TypeId = logBook.LogBookTypeId,
                                            StatusId = logBook.StatusId,
                                            StatusName = logBookStatus.Name,
                                            IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                            LogBookTypeCode = logBookType.Code,
                                            Type = logBookType.Name,
                                            LogBookOwnerTypeCode = logBook.LogBookOwnerType,
                                            ShipId = ship != null ? ship.Id : default,
                                            AquacultureId = aquacultureOwner != null ? (int?)aquacultureOwner.Id : default,
                                            ShipName = ship != null ? ship.Name : null,
                                            ShipExtMark = ship != null ? ship.ExternalMark : null,

                                            AquacultureFacilityName = aquacultureOwner != null
                                                                    ? aquacultureOwner.Name + " (" + aquacultureOwner.UrorNum + ")"
                                                                    : null,
                                            RegisteredBuyerId = buyerOwner != null ? buyerOwner.Id : default(int?),
                                            RegisteredPersonBuyerName = buyerPerson != null
                                                                      ? buyerPerson.FirstName + " " + buyerPerson.LastName + " (" + buyerOwner.RegistrationNum + ")"
                                                                      : null,
                                            RegisteredLegalBuyerName = buyerLegal != null
                                                                     ? buyerLegal.Name + " (" + buyerOwner.RegistrationNum + ")"
                                                                     : null,
                                            LogBookPersonName = personOwner != null
                                                              ? personOwner.FirstName + " " + personOwner.LastName
                                                              : null,
                                            LogBookLegalName = legalOwner != null ? legalOwner.Name : null,
                                            OwnerEgnEik = personOwner != null
                                                        ? personOwner.EgnLnc
                                                        : legalOwner != null
                                                          ? legalOwner.Eik
                                                          : buyerLegal != null
                                                            ? buyerLegal.Eik
                                                            : buyerPerson != null
                                                                ? buyerPerson.EgnLnc
                                                                : null,

                                            IssueDate = logBook.IssueDate,
                                            FinishDate = logBook.FinishDate,
                                            IsOnline = logBook.IsOnline,
                                            StartPageNum = logBook.StartPageNum,
                                            EndPageNum = logBook.EndPageNum,
                                            OwnerRegisteredByerId = logBook.RegisteredBuyerId,
                                            OwnerUnregisteredBuyerPersonId = logBook.PersonId,
                                            OwnerUnregisteredBuyerLegalId = logBook.LegalId,
                                            LogBookPermitLicenseStartPage = logBookPermitLicense.StartPageNum,
                                            LogBookPermitLicenseEndPage = logBookPermitLicense.EndPageNum
                                        };

            return result;
        }

        private IQueryable<LogBookRegisterDTO> GetFreeTextFilteredLogBooks(CatchesAndSalesPublicFilters filters, int userId, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            PermittedPublicLogBookBaseQueries baseQueries = GetAllPublicLogBooksBaseQuery(filters.ShowInactiveRecords, userId, permittedLogBookTypes);

            string text = filters.FreeTextSearch.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);
            decimal pageNum;
            bool pageNumParse = decimal.TryParse(text, out pageNum);

            baseQueries.PersonalLogBooks = GetFreeTextFilteredPersonalLogBooks(baseQueries.PersonalLogBooks, text, pageNumParse ? pageNum : null, searchDate);
            baseQueries.LegalLogBooks = GetFreeTextFilteredLegalLogBooks(baseQueries.LegalLogBooks, text, pageNumParse ? pageNum : null, searchDate);
            baseQueries.PersonBuyerLogBooks = GetFreeTextFilteredPersonBuyerLogBooks(baseQueries.PersonBuyerLogBooks, text, pageNumParse ? pageNum : null, searchDate);
            baseQueries.BuyerLegalLogBooks = GetFreeTextFilteredBuyerLegalLogBooks(baseQueries.BuyerLegalLogBooks, text, pageNumParse ? pageNum : null, searchDate);
            baseQueries.AquaculturePersonaLogBooks = GetFreeTextFilteredAquaculturePersonaLogBooks(baseQueries.AquaculturePersonaLogBooks, text, pageNumParse ? pageNum : null, searchDate);

            if (baseQueries.AquacultureLegalLogBooks != null)
            {
                baseQueries.AquacultureLegalLogBooks = GetFreeTextFilteredAquacultureLegalLogBooks(baseQueries.AquacultureLegalLogBooks, text, pageNumParse ? pageNum : null, searchDate);
            }

            baseQueries.ShipPersonLogBooks = GetFreeTextFilteredShipPersonLogBooks(baseQueries.ShipPersonLogBooks, text, pageNumParse ? pageNum : null, searchDate);

            if (baseQueries.ShipLegalLogBooks != null)
            {
                baseQueries.ShipLegalLogBooks = GetFreeTextFilteredShipLegalLogBooks(baseQueries.ShipLegalLogBooks, text, pageNumParse ? pageNum : null, searchDate);
            }

            IQueryable<LogBookRegisterDTO> result = FinalizeGetPublicLogBooks(baseQueries);

            return result;
        }

        private IQueryable<LogBookRegisterHelper> GetFreeTextFilteredPersonalLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                      string text,
                                                                                      decimal? pageNum,
                                                                                      DateTime? searchDate)
        {
            return from logBook in query
                   where (logBook.ShipName != null && (logBook.ShipName + " (" + logBook.ShipExtMark + ")").ToLower().Contains(text))
                         || (logBook.ShipExtMark != null && logBook.ShipExtMark.ToLower().Contains(text))
                         || (logBook.AquacultureFacilityName != null && logBook.AquacultureFacilityName.ToLower().Contains(text))
                         || (logBook.RegisteredPersonBuyerName != null && logBook.RegisteredPersonBuyerName.ToLower().Contains(text))
                         || (logBook.RegisteredLegalBuyerName != null && logBook.RegisteredLegalBuyerName.ToLower().Contains(text))
                         || (logBook.LogBookPersonName != null && logBook.LogBookPersonName.ToLower().Contains(text))
                         || (logBook.LogBookLegalName != null && logBook.LogBookLegalName.ToLower().Contains(text))
                         || logBook.Number.ToLower().Contains(text)
                         || (pageNum.HasValue && logBook.StartPageNum <= pageNum.Value && logBook.EndPageNum >= pageNum.Value)
                         || logBook.StatusName.ToLower().Contains(text)
                         || (searchDate.HasValue && logBook.IssueDate <= searchDate.Value && logBook.FinishDate >= searchDate.Value)
                   select logBook;
        }

        private IQueryable<LogBookRegisterHelper> GetFreeTextFilteredLegalLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                   string text,
                                                                                   decimal? pageNum,
                                                                                   DateTime? searchDate)
        {
            return from logBook in query
                   where (logBook.ShipName != null && (logBook.ShipName + " (" + logBook.ShipExtMark + ")").ToLower().Contains(text))
                            || (logBook.ShipExtMark != null && logBook.ShipExtMark.ToLower().Contains(text))
                            || (logBook.AquacultureFacilityName != null && logBook.AquacultureFacilityName.ToLower().Contains(text))
                            || (logBook.RegisteredPersonBuyerName != null && logBook.RegisteredPersonBuyerName.ToLower().Contains(text))
                            || (logBook.RegisteredLegalBuyerName != null && logBook.RegisteredLegalBuyerName.ToLower().Contains(text))
                            || (logBook.LogBookPersonName != null && logBook.LogBookPersonName.ToLower().Contains(text))
                            || (logBook.LogBookLegalName != null && logBook.LogBookLegalName.ToLower().Contains(text))
                            || logBook.Number.ToLower().Contains(text)
                            || (pageNum.HasValue && logBook.StartPageNum <= pageNum.Value && logBook.EndPageNum >= pageNum.Value)
                            || logBook.StatusName.ToLower().Contains(text)
                            || (searchDate.HasValue && logBook.IssueDate <= searchDate.Value && logBook.FinishDate >= searchDate.Value)
                   select logBook;
        }

        private IQueryable<LogBookRegisterHelper> GetFreeTextFilteredPersonBuyerLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                         string text,
                                                                                         decimal? pageNum,
                                                                                         DateTime? searchDate)
        {
            return from logBook in query
                   where (logBook.ShipName != null && (logBook.ShipName + " (" + logBook.ShipExtMark + ")").ToLower().Contains(text))
                            || (logBook.ShipExtMark != null && logBook.ShipExtMark.ToLower().Contains(text))
                            || (logBook.AquacultureFacilityName != null && logBook.AquacultureFacilityName.ToLower().Contains(text))
                            || (logBook.RegisteredPersonBuyerName != null && logBook.RegisteredPersonBuyerName.ToLower().Contains(text))
                            || (logBook.RegisteredLegalBuyerName != null && logBook.RegisteredLegalBuyerName.ToLower().Contains(text))
                            || (logBook.LogBookPersonName != null && logBook.LogBookPersonName.ToLower().Contains(text))
                            || (logBook.LogBookLegalName != null && logBook.LogBookLegalName.ToLower().Contains(text))
                            || logBook.Number.ToLower().Contains(text)
                            || (pageNum.HasValue && logBook.StartPageNum <= pageNum.Value && logBook.EndPageNum >= pageNum.Value)
                            || logBook.StatusName.ToLower().Contains(text)
                   select logBook;
        }

        private IQueryable<LogBookRegisterHelper> GetFreeTextFilteredBuyerLegalLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                        string text,
                                                                                        decimal? pageNum,
                                                                                        DateTime? searchDate)
        {
            return from logBook in query
                   where (logBook.ShipName != null && (logBook.ShipName + " (" + logBook.ShipExtMark + ")").ToLower().Contains(text))
                            || (logBook.ShipExtMark != null && logBook.ShipExtMark.ToLower().Contains(text))
                            || (logBook.AquacultureFacilityName != null && logBook.AquacultureFacilityName.ToLower().Contains(text))
                            || (logBook.RegisteredPersonBuyerName != null && logBook.RegisteredPersonBuyerName.ToLower().Contains(text))
                            || (logBook.RegisteredLegalBuyerName != null && logBook.RegisteredLegalBuyerName.ToLower().Contains(text))
                            || (logBook.LogBookPersonName != null && logBook.LogBookPersonName.ToLower().Contains(text))
                            || (logBook.LogBookLegalName != null && logBook.LogBookLegalName.ToLower().Contains(text))
                            || logBook.Number.ToLower().Contains(text)
                            || (pageNum.HasValue && logBook.StartPageNum <= pageNum.Value && logBook.EndPageNum >= pageNum.Value)
                            || logBook.StatusName.ToLower().Contains(text)
                            || (searchDate.HasValue && logBook.IssueDate <= searchDate.Value && logBook.FinishDate >= searchDate.Value)
                   select logBook;
        }

        private IQueryable<LogBookRegisterHelper> GetFreeTextFilteredAquaculturePersonaLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                                string text,
                                                                                                decimal? pageNum,
                                                                                                DateTime? searchDate)
        {
            return from logBook in query
                   where (logBook.ShipName != null && (logBook.ShipName + " (" + logBook.ShipExtMark + ")").ToLower().Contains(text))
                            || (logBook.ShipExtMark != null && logBook.ShipExtMark.ToLower().Contains(text))
                            || (logBook.AquacultureFacilityName != null && logBook.AquacultureFacilityName.ToLower().Contains(text))
                            || (logBook.RegisteredPersonBuyerName != null && logBook.RegisteredPersonBuyerName.ToLower().Contains(text))
                            || (logBook.RegisteredLegalBuyerName != null && logBook.RegisteredLegalBuyerName.ToLower().Contains(text))
                            || (logBook.LogBookPersonName != null && logBook.LogBookPersonName.ToLower().Contains(text))
                            || (logBook.LogBookLegalName != null && logBook.LogBookLegalName.ToLower().Contains(text))
                            || logBook.Number.ToLower().Contains(text)
                            || (pageNum.HasValue && logBook.StartPageNum <= pageNum.Value && logBook.EndPageNum >= pageNum.Value)
                            || logBook.StatusName.ToLower().Contains(text)
                            || (searchDate.HasValue && logBook.IssueDate <= searchDate.Value && logBook.FinishDate >= searchDate.Value)
                   select logBook;
        }

        private IQueryable<LogBookRegisterHelper> GetFreeTextFilteredAquacultureLegalLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                              string text,
                                                                                              decimal? pageNum,
                                                                                              DateTime? searchDate)
        {
            return from logBook in query
                   where (logBook.ShipName != null && (logBook.ShipName + " (" + logBook.ShipExtMark + ")").ToLower().Contains(text))
                          || (logBook.ShipExtMark != null && logBook.ShipExtMark.ToLower().Contains(text))
                          || (logBook.AquacultureFacilityName != null && logBook.AquacultureFacilityName.ToLower().Contains(text))
                          || (logBook.RegisteredPersonBuyerName != null && logBook.RegisteredPersonBuyerName.ToLower().Contains(text))
                          || (logBook.RegisteredLegalBuyerName != null && logBook.RegisteredLegalBuyerName.ToLower().Contains(text))
                          || (logBook.LogBookPersonName != null && logBook.LogBookPersonName.ToLower().Contains(text))
                          || (logBook.LogBookLegalName != null && logBook.LogBookLegalName.ToLower().Contains(text))
                          || logBook.Number.ToLower().Contains(text)
                          || (pageNum.HasValue && logBook.StartPageNum <= pageNum.Value && logBook.EndPageNum >= pageNum.Value)
                          || logBook.StatusName.ToLower().Contains(text)
                          || (searchDate.HasValue && logBook.IssueDate <= searchDate.Value && logBook.FinishDate >= searchDate.Value)
                   select logBook;
        }

        private IQueryable<LogBookRegisterHelper> GetFreeTextFilteredShipPersonLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                        string text,
                                                                                        decimal? pageNum,
                                                                                        DateTime? searchDate)
        {
            return from logBook in query
                   where (logBook.ShipName != null && (logBook.ShipName + " (" + logBook.ShipExtMark + ")").ToLower().Contains(text))
                          || (logBook.ShipExtMark != null && logBook.ShipExtMark.ToLower().Contains(text))
                          || (logBook.AquacultureFacilityName != null && logBook.AquacultureFacilityName.ToLower().Contains(text))
                          || (logBook.RegisteredPersonBuyerName != null && logBook.RegisteredPersonBuyerName.ToLower().Contains(text))
                          || (logBook.RegisteredLegalBuyerName != null && logBook.RegisteredLegalBuyerName.ToLower().Contains(text))
                          || (logBook.LogBookPersonName != null && logBook.LogBookPersonName.ToLower().Contains(text))
                          || (logBook.LogBookLegalName != null && logBook.LogBookLegalName.ToLower().Contains(text))
                          || logBook.Number.ToLower().Contains(text)
                          || (pageNum.HasValue && logBook.StartPageNum <= pageNum.Value && logBook.EndPageNum >= pageNum.Value)
                          || logBook.StatusName.ToLower().Contains(text)
                          || (searchDate.HasValue && logBook.IssueDate <= searchDate.Value && logBook.FinishDate >= searchDate.Value)
                   select logBook;
        }

        private IQueryable<LogBookRegisterHelper> GetFreeTextFilteredShipLegalLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                       string text,
                                                                                       decimal? pageNum,
                                                                                       DateTime? searchDate)
        {
            return from logBook in query
                   where (logBook.ShipName != null && (logBook.ShipName + " (" + logBook.ShipExtMark + ")").ToLower().Contains(text))
                             || (logBook.ShipExtMark != null && logBook.ShipExtMark.ToLower().Contains(text))
                             || (logBook.AquacultureFacilityName != null && logBook.AquacultureFacilityName.ToLower().Contains(text))
                             || (logBook.RegisteredPersonBuyerName != null && logBook.RegisteredPersonBuyerName.ToLower().Contains(text))
                             || (logBook.RegisteredLegalBuyerName != null && logBook.RegisteredLegalBuyerName.ToLower().Contains(text))
                             || (logBook.LogBookPersonName != null && logBook.LogBookPersonName.ToLower().Contains(text))
                             || (logBook.LogBookLegalName != null && logBook.LogBookLegalName.ToLower().Contains(text))
                             || logBook.Number.ToLower().Contains(text)
                             || (pageNum.HasValue && logBook.StartPageNum <= pageNum.Value && logBook.EndPageNum >= pageNum.Value)
                             || logBook.StatusName.ToLower().Contains(text)
                             || (searchDate.HasValue && logBook.IssueDate <= searchDate.Value && logBook.FinishDate >= searchDate.Value)
                   select logBook;
        }


        private IQueryable<LogBookRegisterDTO> GetParametersFilteredLogBooks(CatchesAndSalesPublicFilters filters, int userId, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            PermittedPublicLogBookBaseQueries baseQueries = GetAllPublicLogBooksBaseQuery(filters.ShowInactiveRecords, userId, permittedLogBookTypes);

            bool isNumberCastSucc = false;
            long number = default;
            string logBookNumber = null;

            if (!string.IsNullOrEmpty(filters.OnlinePageNumber))
            {
                List<string> logBookNumberParts = filters.OnlinePageNumber.Split(ONLINE_PAGE_SEPARATOR).ToList();

                if (logBookNumberParts.Count > 1)
                {
                    isNumberCastSucc = long.TryParse(logBookNumberParts.Last(), out number);

                    if (isNumberCastSucc)
                    {
                        int charsToSubsctract = filters.OnlinePageNumber.Length - number.ToString("D5").Length - 1;
                        if (charsToSubsctract > 0)
                        {
                            logBookNumber = filters.OnlinePageNumber.Substring(0, charsToSubsctract);
                        }
                    }
                }
                else
                {
                    isNumberCastSucc = long.TryParse(filters.OnlinePageNumber, out number);
                }
            }

            baseQueries.LegalLogBooks = GetParametersFilteredLegalLogBooks(baseQueries.LegalLogBooks, filters, logBookNumber, isNumberCastSucc? number : null);
            baseQueries.PersonalLogBooks = GetParametersFilteredPersonalLogBooks(baseQueries.PersonalLogBooks, filters, logBookNumber, isNumberCastSucc ? number : null);
            baseQueries.PersonBuyerLogBooks = GetParametersFilteredPersonBuyerLogBooks(baseQueries.PersonBuyerLogBooks, filters, logBookNumber, isNumberCastSucc ? number : null);
            baseQueries.BuyerLegalLogBooks = GetParametersFilteredBuyerLegalLogBooks(baseQueries.BuyerLegalLogBooks, filters, logBookNumber, isNumberCastSucc ? number : null);

            if (!filters.DocumentNumber.HasValue)
            {
                baseQueries.AquaculturePersonaLogBooks = GetParametersFilteredAquaculturePersonaLogBooks(baseQueries.AquaculturePersonaLogBooks, filters, logBookNumber, isNumberCastSucc ? number : null);
            }
            else
            {
                baseQueries.AquaculturePersonaLogBooks = null;
            }

            if (baseQueries.AquacultureLegalLogBooks != null && !filters.DocumentNumber.HasValue)
            {
                baseQueries.AquacultureLegalLogBooks = GetParametersFilteredAquacultureLegalLogBooks(baseQueries.AquacultureLegalLogBooks, filters, logBookNumber, isNumberCastSucc ? number : null);
            }
            else
            {
                baseQueries.AquacultureLegalLogBooks = null;
            }

            if (!filters.DocumentNumber.HasValue)
            {
                baseQueries.ShipPersonLogBooks = GetParametersFilteredShipPersonLogBooks(baseQueries.ShipPersonLogBooks, filters, logBookNumber, isNumberCastSucc ? number : null);

            }
            else
            {
                baseQueries.ShipPersonLogBooks = null;
            }

            if (baseQueries.ShipLegalLogBooks != null && !filters.DocumentNumber.HasValue)
            {
                baseQueries.ShipLegalLogBooks = GetParametersFilteredShipLegalLogBooks(baseQueries.ShipLegalLogBooks, filters, logBookNumber, isNumberCastSucc ? number : null);
            }
            else
            {
                baseQueries.ShipLegalLogBooks = null;
            }

            IQueryable<LogBookRegisterDTO> result = FinalizeGetPublicLogBooks(baseQueries);

            return result;
        }

        private IQueryable<LogBookRegisterHelper> GetParametersFilteredLegalLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                     CatchesAndSalesPublicFilters filters,
                                                                                     string logBookNumber,
                                                                                     long? onlinePageNumber)
        {
            return from logBook in query
                   join admissionPage in Db.AdmissionLogBookPages on logBook.Id equals admissionPage.LogBookId into lbAdmission
                   from admissionPage in lbAdmission.DefaultIfEmpty()
                   join transportationPage in Db.TransportationLogBookPages on logBook.Id equals transportationPage.LogBookId into lbTransportation
                   from transportationPage in lbTransportation.DefaultIfEmpty()
                   where (!filters.LogBookTypeId.HasValue || logBook.TypeId == filters.LogBookTypeId.Value)
                          && (!filters.LogBookValidityStartDate.HasValue || logBook.IssueDate <= filters.LogBookValidityStartDate.Value)
                          && (!filters.LogBookValidityEndDate.HasValue || logBook.FinishDate >= filters.LogBookValidityEndDate.Value)
                          && (filters.LogBookStatusIds == null || filters.LogBookStatusIds.Contains(logBook.StatusId))
                          && (string.IsNullOrEmpty(filters.LogBookNumber) || logBook.Number.Contains(filters.LogBookNumber))
                          && (!filters.DocumentNumber.HasValue
                               || (admissionPage != null && admissionPage.PageNum == filters.DocumentNumber.Value)
                               || (transportationPage != null && transportationPage.PageNum == filters.DocumentNumber.Value))
                         && (!filters.PageNumber.HasValue
                               || (!logBook.IsOnline
                                   && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                           && logBook.LogBookPermitLicenseEndPage.HasValue
                                           && logBook.LogBookPermitLicenseStartPage.Value <= filters.PageNumber.Value
                                           && logBook.LogBookPermitLicenseEndPage.Value >= filters.PageNumber.Value)
                                       || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                           && !logBook.LogBookPermitLicenseEndPage.HasValue
                                           && !string.IsNullOrEmpty(logBook.LogBookOwnerTypeCode)
                                           && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                           && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                           && logBook.StartPageNum <= filters.PageNumber.Value
                                           && logBook.EndPageNum >= filters.PageNumber.Value))))
                        && (string.IsNullOrEmpty(filters.OnlinePageNumber)
                             || ((string.IsNullOrEmpty(logBookNumber) || logBook.Number == logBookNumber)
                                   && logBook.IsOnline
                                   && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                           && logBook.LogBookPermitLicenseEndPage.HasValue
                                           && logBook.LogBookPermitLicenseStartPage.Value <= onlinePageNumber
                                           && logBook.LogBookPermitLicenseEndPage.Value >= onlinePageNumber)
                                        || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                               && !logBook.LogBookPermitLicenseEndPage.HasValue
                                               && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                               && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                               && logBook.StartPageNum <= onlinePageNumber
                                               && logBook.EndPageNum >= onlinePageNumber))
                                       ))
                   group logBook by new
                   {
                       logBook.Id,
                       logBook.Number,
                       logBook.TypeId,
                       logBook.StatusId,
                       logBook.StatusName,
                       logBook.IsLogBookFinished,
                       logBook.LogBookTypeCode,
                       logBook.Type,
                       logBook.LogBookOwnerTypeCode,
                       logBook.ShipId,
                       logBook.AquacultureId,
                       logBook.ShipName,
                       logBook.ShipExtMark,
                       logBook.AquacultureFacilityName,
                       logBook.RegisteredBuyerId,
                       logBook.RegisteredPersonBuyerName,
                       logBook.RegisteredLegalBuyerName,
                       logBook.LogBookPersonName,
                       logBook.LogBookLegalName,
                       logBook.OwnerEgnEik,
                       logBook.IssueDate,
                       logBook.FinishDate,
                       logBook.IsOnline,
                       logBook.StartPageNum,
                       logBook.EndPageNum,
                       logBook.OwnerRegisteredByerId,
                       logBook.OwnerUnregisteredBuyerPersonId,
                       logBook.OwnerUnregisteredBuyerLegalId
                   } into lb
                   select new LogBookRegisterHelper
                   {
                       Id = lb.Key.Id,
                       Number = lb.Key.Number,
                       TypeId = lb.Key.TypeId,
                       StatusId = lb.Key.StatusId,
                       StatusName = lb.Key.StatusName,
                       IsLogBookFinished = lb.Key.IsLogBookFinished,
                       LogBookTypeCode = lb.Key.LogBookTypeCode,
                       Type = lb.Key.Type,
                       LogBookOwnerTypeCode = lb.Key.LogBookOwnerTypeCode,
                       ShipId = lb.Key.ShipId,
                       AquacultureId = lb.Key.AquacultureId,
                       ShipName = lb.Key.ShipName,
                       ShipExtMark = lb.Key.ShipExtMark,
                       AquacultureFacilityName = lb.Key.AquacultureFacilityName,
                       RegisteredBuyerId = lb.Key.RegisteredBuyerId,
                       RegisteredPersonBuyerName = lb.Key.RegisteredPersonBuyerName,
                       RegisteredLegalBuyerName = lb.Key.RegisteredLegalBuyerName,
                       LogBookPersonName = lb.Key.LogBookPersonName,
                       LogBookLegalName = lb.Key.LogBookLegalName,
                       OwnerEgnEik = lb.Key.OwnerEgnEik,
                       IssueDate = lb.Key.IssueDate,
                       FinishDate = lb.Key.FinishDate,
                       IsOnline = lb.Key.IsOnline,
                       StartPageNum = lb.Key.StartPageNum,
                       EndPageNum = lb.Key.EndPageNum,
                       OwnerRegisteredByerId = lb.Key.OwnerRegisteredByerId,
                       OwnerUnregisteredBuyerPersonId = lb.Key.OwnerUnregisteredBuyerPersonId,
                       OwnerUnregisteredBuyerLegalId = lb.Key.OwnerUnregisteredBuyerLegalId,
                       LogBookPermitLicenseStartPage = default,
                       LogBookPermitLicenseEndPage = default
                   };
        }

        private IQueryable<LogBookRegisterHelper> GetParametersFilteredPersonalLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                        CatchesAndSalesPublicFilters filters,
                                                                                        string logBookNumber,
                                                                                        long? onlinePageNumber)
        { 
            return from logBook in query
                   join admissionPage in Db.AdmissionLogBookPages on logBook.Id equals admissionPage.LogBookId into lbAdmission
                   from admissionPage in lbAdmission.DefaultIfEmpty()
                   join transportationPage in Db.TransportationLogBookPages on logBook.Id equals transportationPage.LogBookId into lbTransportation
                   from transportationPage in lbTransportation.DefaultIfEmpty()
                   where (!filters.LogBookTypeId.HasValue || logBook.TypeId == filters.LogBookTypeId.Value)
                           && (!filters.LogBookValidityStartDate.HasValue || logBook.IssueDate <= filters.LogBookValidityStartDate.Value)
                           && (!filters.LogBookValidityEndDate.HasValue || logBook.FinishDate >= filters.LogBookValidityEndDate.Value)
                           && (filters.LogBookStatusIds == null || filters.LogBookStatusIds.Contains(logBook.StatusId))
                           && (string.IsNullOrEmpty(filters.LogBookNumber) || logBook.Number.Contains(filters.LogBookNumber))
                           && (!filters.DocumentNumber.HasValue
                            || (admissionPage != null && admissionPage.PageNum == filters.DocumentNumber.Value)
                            || (transportationPage != null && transportationPage.PageNum == filters.DocumentNumber.Value))
                           && (!filters.PageNumber.HasValue
                            || (!logBook.IsOnline
                                && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                        && logBook.LogBookPermitLicenseEndPage.HasValue
                                        && logBook.LogBookPermitLicenseStartPage.Value <= filters.PageNumber.Value
                                        && logBook.LogBookPermitLicenseEndPage.Value >= filters.PageNumber.Value)
                                    || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                        && !logBook.LogBookPermitLicenseEndPage.HasValue
                                        && !string.IsNullOrEmpty(logBook.LogBookOwnerTypeCode)
                                        && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                        && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                        && logBook.StartPageNum <= filters.PageNumber.Value
                                        && logBook.EndPageNum >= filters.PageNumber.Value))))
                          && (string.IsNullOrEmpty(filters.OnlinePageNumber)
                              || ((string.IsNullOrEmpty(logBookNumber) || logBook.Number == logBookNumber)
                                    && logBook.IsOnline
                                    && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                            && logBook.LogBookPermitLicenseEndPage.HasValue
                                            && logBook.LogBookPermitLicenseStartPage.Value <= onlinePageNumber
                                            && logBook.LogBookPermitLicenseEndPage.Value >= onlinePageNumber)
                                         || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                                && !logBook.LogBookPermitLicenseEndPage.HasValue
                                                && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                                && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                                && logBook.StartPageNum <= onlinePageNumber
                                                && logBook.EndPageNum >= onlinePageNumber))
                                        ))
                   group logBook by new
                   {
                       logBook.Id,
                       logBook.Number,
                       logBook.TypeId,
                       logBook.StatusId,
                       logBook.StatusName,
                       logBook.IsLogBookFinished,
                       logBook.LogBookTypeCode,
                       logBook.Type,
                       logBook.LogBookOwnerTypeCode,
                       logBook.ShipId,
                       logBook.AquacultureId,
                       logBook.ShipName,
                       logBook.ShipExtMark,
                       logBook.AquacultureFacilityName,
                       logBook.RegisteredBuyerId,
                       logBook.RegisteredPersonBuyerName,
                       logBook.RegisteredLegalBuyerName,
                       logBook.LogBookPersonName,
                       logBook.LogBookLegalName,
                       logBook.OwnerEgnEik,
                       logBook.IssueDate,
                       logBook.FinishDate,
                       logBook.IsOnline,
                       logBook.StartPageNum,
                       logBook.EndPageNum,
                       logBook.OwnerRegisteredByerId,
                       logBook.OwnerUnregisteredBuyerPersonId,
                       logBook.OwnerUnregisteredBuyerLegalId
                   } into lb
                   select new LogBookRegisterHelper
                   {
                       Id = lb.Key.Id,
                       Number = lb.Key.Number,
                       TypeId = lb.Key.TypeId,
                       StatusId = lb.Key.StatusId,
                       StatusName = lb.Key.StatusName,
                       IsLogBookFinished = lb.Key.IsLogBookFinished,
                       LogBookTypeCode = lb.Key.LogBookTypeCode,
                       Type = lb.Key.Type,
                       LogBookOwnerTypeCode = lb.Key.LogBookOwnerTypeCode,
                       ShipId = lb.Key.ShipId,
                       AquacultureId = lb.Key.AquacultureId,
                       ShipName = lb.Key.ShipName,
                       ShipExtMark = lb.Key.ShipExtMark,
                       AquacultureFacilityName = lb.Key.AquacultureFacilityName,
                       RegisteredBuyerId = lb.Key.RegisteredBuyerId,
                       RegisteredPersonBuyerName = lb.Key.RegisteredPersonBuyerName,
                       RegisteredLegalBuyerName = lb.Key.RegisteredLegalBuyerName,
                       LogBookPersonName = lb.Key.LogBookPersonName,
                       LogBookLegalName = lb.Key.LogBookLegalName,
                       OwnerEgnEik = lb.Key.OwnerEgnEik,
                       IssueDate = lb.Key.IssueDate,
                       FinishDate = lb.Key.FinishDate,
                       IsOnline = lb.Key.IsOnline,
                       StartPageNum = lb.Key.StartPageNum,
                       EndPageNum = lb.Key.EndPageNum,
                       OwnerRegisteredByerId = lb.Key.OwnerRegisteredByerId,
                       OwnerUnregisteredBuyerPersonId = lb.Key.OwnerUnregisteredBuyerPersonId,
                       OwnerUnregisteredBuyerLegalId = lb.Key.OwnerUnregisteredBuyerLegalId,
                       LogBookPermitLicenseStartPage = default,
                       LogBookPermitLicenseEndPage = default
                   };
        }

        private IQueryable<LogBookRegisterHelper> GetParametersFilteredPersonBuyerLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                           CatchesAndSalesPublicFilters filters,
                                                                                           string logBookNumber,
                                                                                           long? onlinePageNumber)
        {
            return from logBook in query
                   join firstSalePage in Db.FirstSaleLogBookPages on logBook.Id equals firstSalePage.LogBookId into lbFirstSale
                   from firstSalePage in lbFirstSale.DefaultIfEmpty()
                   join admissionPage in Db.AdmissionLogBookPages on logBook.Id equals admissionPage.LogBookId into lbAdmission
                   from admissionPage in lbAdmission.DefaultIfEmpty()
                   join transportationPage in Db.TransportationLogBookPages on logBook.Id equals transportationPage.LogBookId into lbTransportation
                   from transportationPage in lbTransportation.DefaultIfEmpty()
                   where (!filters.LogBookTypeId.HasValue || logBook.TypeId == filters.LogBookTypeId.Value)
                           && (!filters.LogBookValidityStartDate.HasValue || logBook.IssueDate <= filters.LogBookValidityStartDate.Value)
                           && (!filters.LogBookValidityEndDate.HasValue || logBook.FinishDate >= filters.LogBookValidityEndDate.Value)
                           && (filters.LogBookStatusIds == null || filters.LogBookStatusIds.Contains(logBook.StatusId))
                           && (string.IsNullOrEmpty(filters.LogBookNumber) || logBook.Number.Contains(filters.LogBookNumber))
                           && (!filters.DocumentNumber.HasValue
                                 || (firstSalePage != null && firstSalePage.PageNum == filters.DocumentNumber.Value)
                                 || (admissionPage != null && admissionPage.PageNum == filters.DocumentNumber.Value)
                                 || (transportationPage != null && transportationPage.PageNum == filters.DocumentNumber.Value))
                           && (!filters.PageNumber.HasValue
                                 || (!logBook.IsOnline
                                     && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                             && logBook.LogBookPermitLicenseEndPage.HasValue
                                             && logBook.LogBookPermitLicenseStartPage.Value <= filters.PageNumber.Value
                                             && logBook.LogBookPermitLicenseEndPage.Value >= filters.PageNumber.Value)
                                         || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                             && !logBook.LogBookPermitLicenseEndPage.HasValue
                                             && !string.IsNullOrEmpty(logBook.LogBookOwnerTypeCode)
                                             && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                             && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                             && logBook.StartPageNum <= filters.PageNumber.Value
                                             && logBook.EndPageNum >= filters.PageNumber.Value))))
                         && (string.IsNullOrEmpty(filters.OnlinePageNumber)
                               || ((string.IsNullOrEmpty(logBookNumber) || logBook.Number == logBookNumber)
                                     && logBook.IsOnline
                                     && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                             && logBook.LogBookPermitLicenseEndPage.HasValue
                                             && logBook.LogBookPermitLicenseStartPage.Value <= onlinePageNumber
                                             && logBook.LogBookPermitLicenseEndPage.Value >= onlinePageNumber)
                                          || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                                 && !logBook.LogBookPermitLicenseEndPage.HasValue
                                                 && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                                 && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                                 && logBook.StartPageNum <= onlinePageNumber
                                                 && logBook.EndPageNum >= onlinePageNumber))
                                         ))
                   group logBook by new
                   {
                       logBook.Id,
                       logBook.Number,
                       logBook.TypeId,
                       logBook.StatusId,
                       logBook.StatusName,
                       logBook.IsLogBookFinished,
                       logBook.LogBookTypeCode,
                       logBook.Type,
                       logBook.LogBookOwnerTypeCode,
                       logBook.ShipId,
                       logBook.AquacultureId,
                       logBook.ShipName,
                       logBook.ShipExtMark,
                       logBook.AquacultureFacilityName,
                       logBook.RegisteredBuyerId,
                       logBook.RegisteredPersonBuyerName,
                       logBook.RegisteredLegalBuyerName,
                       logBook.LogBookPersonName,
                       logBook.LogBookLegalName,
                       logBook.OwnerEgnEik,
                       logBook.IssueDate,
                       logBook.FinishDate,
                       logBook.IsOnline,
                       logBook.StartPageNum,
                       logBook.EndPageNum,
                       logBook.OwnerRegisteredByerId,
                       logBook.OwnerUnregisteredBuyerPersonId,
                       logBook.OwnerUnregisteredBuyerLegalId
                   } into lb
                   select new LogBookRegisterHelper
                   {
                       Id = lb.Key.Id,
                       Number = lb.Key.Number,
                       TypeId = lb.Key.TypeId,
                       StatusId = lb.Key.StatusId,
                       StatusName = lb.Key.StatusName,
                       IsLogBookFinished = lb.Key.IsLogBookFinished,
                       LogBookTypeCode = lb.Key.LogBookTypeCode,
                       Type = lb.Key.Type,
                       LogBookOwnerTypeCode = lb.Key.LogBookOwnerTypeCode,
                       ShipId = lb.Key.ShipId,
                       AquacultureId = lb.Key.AquacultureId,
                       ShipName = lb.Key.ShipName,
                       ShipExtMark = lb.Key.ShipExtMark,
                       AquacultureFacilityName = lb.Key.AquacultureFacilityName,
                       RegisteredBuyerId = lb.Key.RegisteredBuyerId,
                       RegisteredPersonBuyerName = lb.Key.RegisteredPersonBuyerName,
                       RegisteredLegalBuyerName = lb.Key.RegisteredLegalBuyerName,
                       LogBookPersonName = lb.Key.LogBookPersonName,
                       LogBookLegalName = lb.Key.LogBookLegalName,
                       OwnerEgnEik = lb.Key.OwnerEgnEik,
                       IssueDate = lb.Key.IssueDate,
                       FinishDate = lb.Key.FinishDate,
                       IsOnline = lb.Key.IsOnline,
                       StartPageNum = lb.Key.StartPageNum,
                       EndPageNum = lb.Key.EndPageNum,
                       OwnerRegisteredByerId = lb.Key.OwnerRegisteredByerId,
                       OwnerUnregisteredBuyerPersonId = lb.Key.OwnerUnregisteredBuyerPersonId,
                       OwnerUnregisteredBuyerLegalId = lb.Key.OwnerUnregisteredBuyerLegalId,
                       LogBookPermitLicenseStartPage = default,
                       LogBookPermitLicenseEndPage = default
                   };
        }

        private IQueryable<LogBookRegisterHelper> GetParametersFilteredBuyerLegalLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                          CatchesAndSalesPublicFilters filters,
                                                                                          string logBookNumber,
                                                                                          long? onlinePageNumber)
        {
            return from logBook in query
                   join firstSalePage in Db.FirstSaleLogBookPages on logBook.Id equals firstSalePage.LogBookId into lbFirstSale
                   from firstSalePage in lbFirstSale.DefaultIfEmpty()
                   join admissionPage in Db.AdmissionLogBookPages on logBook.Id equals admissionPage.LogBookId into lbAdmission
                   from admissionPage in lbAdmission.DefaultIfEmpty()
                   join transportationPage in Db.TransportationLogBookPages on logBook.Id equals transportationPage.LogBookId into lbTransportation
                   from transportationPage in lbTransportation.DefaultIfEmpty()
                   where (!filters.LogBookTypeId.HasValue || logBook.TypeId == filters.LogBookTypeId.Value)
                            && (!filters.LogBookValidityStartDate.HasValue || logBook.IssueDate <= filters.LogBookValidityStartDate.Value)
                            && (!filters.LogBookValidityEndDate.HasValue || logBook.FinishDate >= filters.LogBookValidityEndDate.Value)
                            && (filters.LogBookStatusIds == null || filters.LogBookStatusIds.Contains(logBook.StatusId))
                            && (string.IsNullOrEmpty(filters.LogBookNumber) || logBook.Number.Contains(filters.LogBookNumber))
                            && (!filters.DocumentNumber.HasValue
                                  || (firstSalePage != null && firstSalePage.PageNum == filters.DocumentNumber.Value)
                                  || (admissionPage != null && admissionPage.PageNum == filters.DocumentNumber.Value)
                                  || (transportationPage != null && transportationPage.PageNum == filters.DocumentNumber.Value))
                           && (!filters.PageNumber.HasValue
                                  || (!logBook.IsOnline
                                      && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                              && logBook.LogBookPermitLicenseEndPage.HasValue
                                              && logBook.LogBookPermitLicenseStartPage.Value <= filters.PageNumber.Value
                                              && logBook.LogBookPermitLicenseEndPage.Value >= filters.PageNumber.Value)
                                          || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                              && !logBook.LogBookPermitLicenseEndPage.HasValue
                                              && !string.IsNullOrEmpty(logBook.LogBookOwnerTypeCode)
                                              && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                              && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                              && logBook.StartPageNum <= filters.PageNumber.Value
                                              && logBook.EndPageNum >= filters.PageNumber.Value))))
                         && (string.IsNullOrEmpty(filters.OnlinePageNumber)
                                || ((string.IsNullOrEmpty(logBookNumber) || logBook.Number == logBookNumber)
                                      && logBook.IsOnline
                                      && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                              && logBook.LogBookPermitLicenseEndPage.HasValue
                                              && logBook.LogBookPermitLicenseStartPage.Value <= onlinePageNumber
                                              && logBook.LogBookPermitLicenseEndPage.Value >= onlinePageNumber)
                                           || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                                  && !logBook.LogBookPermitLicenseEndPage.HasValue
                                                  && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                                  && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                                  && logBook.StartPageNum <= onlinePageNumber
                                                  && logBook.EndPageNum >= onlinePageNumber))
                                          ))
                   group logBook by new
                   {
                       logBook.Id,
                       logBook.Number,
                       logBook.TypeId,
                       logBook.StatusId,
                       logBook.StatusName,
                       logBook.IsLogBookFinished,
                       logBook.LogBookTypeCode,
                       logBook.Type,
                       logBook.LogBookOwnerTypeCode,
                       logBook.ShipId,
                       logBook.AquacultureId,
                       logBook.ShipName,
                       logBook.ShipExtMark,
                       logBook.AquacultureFacilityName,
                       logBook.RegisteredBuyerId,
                       logBook.RegisteredPersonBuyerName,
                       logBook.RegisteredLegalBuyerName,
                       logBook.LogBookPersonName,
                       logBook.LogBookLegalName,
                       logBook.OwnerEgnEik,
                       logBook.IssueDate,
                       logBook.FinishDate,
                       logBook.IsOnline,
                       logBook.StartPageNum,
                       logBook.EndPageNum,
                       logBook.OwnerRegisteredByerId,
                       logBook.OwnerUnregisteredBuyerPersonId,
                       logBook.OwnerUnregisteredBuyerLegalId
                   } into lb
                   select new LogBookRegisterHelper
                   {
                       Id = lb.Key.Id,
                       Number = lb.Key.Number,
                       TypeId = lb.Key.TypeId,
                       StatusId = lb.Key.StatusId,
                       StatusName = lb.Key.StatusName,
                       IsLogBookFinished = lb.Key.IsLogBookFinished,
                       LogBookTypeCode = lb.Key.LogBookTypeCode,
                       Type = lb.Key.Type,
                       LogBookOwnerTypeCode = lb.Key.LogBookOwnerTypeCode,
                       ShipId = lb.Key.ShipId,
                       AquacultureId = lb.Key.AquacultureId,
                       ShipName = lb.Key.ShipName,
                       ShipExtMark = lb.Key.ShipExtMark,
                       AquacultureFacilityName = lb.Key.AquacultureFacilityName,
                       RegisteredBuyerId = lb.Key.RegisteredBuyerId,
                       RegisteredPersonBuyerName = lb.Key.RegisteredPersonBuyerName,
                       RegisteredLegalBuyerName = lb.Key.RegisteredLegalBuyerName,
                       LogBookPersonName = lb.Key.LogBookPersonName,
                       LogBookLegalName = lb.Key.LogBookLegalName,
                       OwnerEgnEik = lb.Key.OwnerEgnEik,
                       IssueDate = lb.Key.IssueDate,
                       FinishDate = lb.Key.FinishDate,
                       IsOnline = lb.Key.IsOnline,
                       StartPageNum = lb.Key.StartPageNum,
                       EndPageNum = lb.Key.EndPageNum,
                       OwnerRegisteredByerId = lb.Key.OwnerRegisteredByerId,
                       OwnerUnregisteredBuyerPersonId = lb.Key.OwnerUnregisteredBuyerPersonId,
                       OwnerUnregisteredBuyerLegalId = lb.Key.OwnerUnregisteredBuyerLegalId,
                       LogBookPermitLicenseStartPage = default,
                       LogBookPermitLicenseEndPage = default
                   };
        }

        private IQueryable<LogBookRegisterHelper> GetParametersFilteredAquaculturePersonaLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                                  CatchesAndSalesPublicFilters filters,
                                                                                                  string logBookNumber,
                                                                                                  long? onlinePageNumber)
        {
            return from logBook in query
                   where (!filters.LogBookTypeId.HasValue || logBook.TypeId == filters.LogBookTypeId.Value)
                           && (!filters.LogBookValidityStartDate.HasValue || logBook.IssueDate <= filters.LogBookValidityStartDate.Value)
                           && (!filters.LogBookValidityEndDate.HasValue || logBook.FinishDate >= filters.LogBookValidityEndDate.Value)
                           && (filters.LogBookStatusIds == null || filters.LogBookStatusIds.Contains(logBook.StatusId))
                           && (string.IsNullOrEmpty(filters.LogBookNumber) || logBook.Number.Contains(filters.LogBookNumber))
                           && (!filters.PageNumber.HasValue
                              || (!logBook.IsOnline
                                  && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                          && logBook.LogBookPermitLicenseEndPage.HasValue
                                          && logBook.LogBookPermitLicenseStartPage.Value <= filters.PageNumber.Value
                                          && logBook.LogBookPermitLicenseEndPage.Value >= filters.PageNumber.Value)
                                      || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                          && !logBook.LogBookPermitLicenseEndPage.HasValue
                                          && !string.IsNullOrEmpty(logBook.LogBookOwnerTypeCode)
                                          && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                          && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                          && logBook.StartPageNum <= filters.PageNumber.Value
                                          && logBook.EndPageNum >= filters.PageNumber.Value))))
                           && (string.IsNullOrEmpty(filters.OnlinePageNumber)
                                || ((string.IsNullOrEmpty(logBookNumber) || logBook.Number == logBookNumber)
                                      && logBook.IsOnline
                                      && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                              && logBook.LogBookPermitLicenseEndPage.HasValue
                                              && logBook.LogBookPermitLicenseStartPage.Value <= onlinePageNumber
                                              && logBook.LogBookPermitLicenseEndPage.Value >= onlinePageNumber)
                                           || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                                  && !logBook.LogBookPermitLicenseEndPage.HasValue
                                                  && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                                  && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                                  && logBook.StartPageNum <= onlinePageNumber
                                                  && logBook.EndPageNum >= onlinePageNumber))
                                          ))
                   select new LogBookRegisterHelper
                   {
                       Id = logBook.Id,
                       Number = logBook.Number,
                       TypeId = logBook.TypeId,
                       StatusId = logBook.StatusId,
                       StatusName = logBook.StatusName,
                       IsLogBookFinished = logBook.IsLogBookFinished,
                       LogBookTypeCode = logBook.LogBookTypeCode,
                       Type = logBook.Type,
                       LogBookOwnerTypeCode = logBook.LogBookOwnerTypeCode,
                       ShipId = logBook.ShipId,
                       AquacultureId = logBook.AquacultureId,
                       ShipName = logBook.ShipName,
                       ShipExtMark = logBook.ShipExtMark,
                       IssueDate = logBook.IssueDate,
                       FinishDate = logBook.FinishDate,
                       IsOnline = logBook.IsOnline,
                       StartPageNum = logBook.StartPageNum,
                       EndPageNum = logBook.EndPageNum,
                       AquacultureFacilityName = logBook.AquacultureFacilityName,
                       RegisteredBuyerId = logBook.RegisteredBuyerId,
                       RegisteredPersonBuyerName = logBook.RegisteredPersonBuyerName,
                       RegisteredLegalBuyerName = logBook.RegisteredLegalBuyerName,
                       LogBookPersonName = logBook.LogBookPersonName,
                       LogBookLegalName = logBook.LogBookLegalName,
                       OwnerEgnEik = logBook.OwnerEgnEik,
                       OwnerRegisteredByerId = logBook.OwnerRegisteredByerId,
                       OwnerUnregisteredBuyerPersonId = logBook.OwnerUnregisteredBuyerPersonId,
                       OwnerUnregisteredBuyerLegalId = logBook.OwnerUnregisteredBuyerLegalId,
                       LogBookPermitLicenseStartPage = default,
                       LogBookPermitLicenseEndPage = default
                   };
        }

        private IQueryable<LogBookRegisterHelper> GetParametersFilteredAquacultureLegalLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                                CatchesAndSalesPublicFilters filters,
                                                                                                string logBookNumber,
                                                                                                long? onlinePageNumber)
        {
            return from logBook in query
                   where (!filters.LogBookTypeId.HasValue || logBook.TypeId == filters.LogBookTypeId.Value)
                           && (!filters.LogBookValidityStartDate.HasValue || logBook.IssueDate <= filters.LogBookValidityStartDate.Value)
                           && (!filters.LogBookValidityEndDate.HasValue || logBook.FinishDate >= filters.LogBookValidityEndDate.Value)
                           && (filters.LogBookStatusIds == null || filters.LogBookStatusIds.Contains(logBook.StatusId))
                           && (string.IsNullOrEmpty(filters.LogBookNumber) || logBook.Number.Contains(filters.LogBookNumber))
                           && (!filters.PageNumber.HasValue
                                || (!logBook.IsOnline
                                    && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                            && logBook.LogBookPermitLicenseEndPage.HasValue
                                            && logBook.LogBookPermitLicenseStartPage.Value <= filters.PageNumber.Value
                                            && logBook.LogBookPermitLicenseEndPage.Value >= filters.PageNumber.Value)
                                        || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                            && !logBook.LogBookPermitLicenseEndPage.HasValue
                                            && !string.IsNullOrEmpty(logBook.LogBookOwnerTypeCode)
                                            && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                            && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                            && logBook.StartPageNum <= filters.PageNumber.Value
                                            && logBook.EndPageNum >= filters.PageNumber.Value))))
                          && (string.IsNullOrEmpty(filters.OnlinePageNumber)
                              || ((string.IsNullOrEmpty(logBookNumber) || logBook.Number == logBookNumber)
                                    && logBook.IsOnline
                                    && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                            && logBook.LogBookPermitLicenseEndPage.HasValue
                                            && logBook.LogBookPermitLicenseStartPage.Value <= onlinePageNumber
                                            && logBook.LogBookPermitLicenseEndPage.Value >= onlinePageNumber)
                                         || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                                && !logBook.LogBookPermitLicenseEndPage.HasValue
                                                && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                                && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                                && logBook.StartPageNum <= onlinePageNumber
                                                && logBook.EndPageNum >= onlinePageNumber))
                                        ))
                   select new LogBookRegisterHelper
                   {
                       Id = logBook.Id,
                       Number = logBook.Number,
                       TypeId = logBook.TypeId,
                       StatusId = logBook.StatusId,
                       StatusName = logBook.StatusName,
                       IsLogBookFinished = logBook.IsLogBookFinished,
                       LogBookTypeCode = logBook.LogBookTypeCode,
                       Type = logBook.Type,
                       LogBookOwnerTypeCode = logBook.LogBookOwnerTypeCode,
                       ShipId = logBook.ShipId,
                       AquacultureId = logBook.AquacultureId,
                       ShipName = logBook.ShipName,
                       ShipExtMark = logBook.ShipExtMark,
                       IssueDate = logBook.IssueDate,
                       FinishDate = logBook.FinishDate,
                       IsOnline = logBook.IsOnline,
                       StartPageNum = logBook.StartPageNum,
                       EndPageNum = logBook.EndPageNum,
                       AquacultureFacilityName = logBook.AquacultureFacilityName,
                       RegisteredBuyerId = logBook.RegisteredBuyerId,
                       RegisteredPersonBuyerName = logBook.RegisteredPersonBuyerName,
                       RegisteredLegalBuyerName = logBook.RegisteredLegalBuyerName,
                       LogBookPersonName = logBook.LogBookPersonName,
                       LogBookLegalName = logBook.LogBookLegalName,
                       OwnerEgnEik = logBook.OwnerEgnEik,
                       OwnerRegisteredByerId = logBook.OwnerRegisteredByerId,
                       OwnerUnregisteredBuyerPersonId = logBook.OwnerUnregisteredBuyerPersonId,
                       OwnerUnregisteredBuyerLegalId = logBook.OwnerUnregisteredBuyerLegalId,
                       LogBookPermitLicenseStartPage = default,
                       LogBookPermitLicenseEndPage = default
                   };
        }

        private IQueryable<LogBookRegisterHelper> GetParametersFilteredShipPersonLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                          CatchesAndSalesPublicFilters filters,
                                                                                          string logBookNumber,
                                                                                          long? onlinePageNumber)
        {
            return from logBook in query
                   where (!filters.LogBookTypeId.HasValue || logBook.TypeId == filters.LogBookTypeId.Value)
                          && (!filters.LogBookValidityStartDate.HasValue || logBook.IssueDate <= filters.LogBookValidityStartDate.Value)
                          && (!filters.LogBookValidityEndDate.HasValue || logBook.FinishDate >= filters.LogBookValidityEndDate.Value)
                          && (filters.LogBookStatusIds == null || filters.LogBookStatusIds.Contains(logBook.StatusId))
                          && (string.IsNullOrEmpty(filters.LogBookNumber) || logBook.Number.Contains(filters.LogBookNumber))
                          && (!filters.PageNumber.HasValue
                              || (!logBook.IsOnline
                                  && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                          && logBook.LogBookPermitLicenseEndPage.HasValue
                                          && logBook.LogBookPermitLicenseStartPage.Value <= filters.PageNumber.Value
                                          && logBook.LogBookPermitLicenseEndPage.Value >= filters.PageNumber.Value)
                                      || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                          && !logBook.LogBookPermitLicenseEndPage.HasValue
                                          && !string.IsNullOrEmpty(logBook.LogBookOwnerTypeCode)
                                          && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                          && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                          && logBook.StartPageNum <= filters.PageNumber.Value
                                          && logBook.EndPageNum >= filters.PageNumber.Value))))
                          && (string.IsNullOrEmpty(filters.OnlinePageNumber)
                                || ((string.IsNullOrEmpty(logBookNumber) || logBook.Number == logBookNumber)
                                      && logBook.IsOnline
                                      && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                              && logBook.LogBookPermitLicenseEndPage.HasValue
                                              && logBook.LogBookPermitLicenseStartPage.Value <= onlinePageNumber
                                              && logBook.LogBookPermitLicenseEndPage.Value >= onlinePageNumber)
                                           || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                                  && !logBook.LogBookPermitLicenseEndPage.HasValue
                                                  && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                                  && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                                  && logBook.StartPageNum <= onlinePageNumber
                                                  && logBook.EndPageNum >= onlinePageNumber))
                                          ))
                   select logBook;
        }

        private IQueryable<LogBookRegisterHelper> GetParametersFilteredShipLegalLogBooks(IQueryable<LogBookRegisterHelper> query,
                                                                                         CatchesAndSalesPublicFilters filters,
                                                                                         string logBookNumber,
                                                                                         long? onlinePageNumber)
        {
            return from logBook in query
                   where (!filters.LogBookTypeId.HasValue || logBook.TypeId == filters.LogBookTypeId.Value)
                          && (!filters.LogBookValidityStartDate.HasValue || logBook.IssueDate <= filters.LogBookValidityStartDate.Value)
                          && (!filters.LogBookValidityEndDate.HasValue || logBook.FinishDate >= filters.LogBookValidityEndDate.Value)
                          && (filters.LogBookStatusIds == null || filters.LogBookStatusIds.Contains(logBook.StatusId))
                          && (string.IsNullOrEmpty(filters.LogBookNumber) || logBook.Number.Contains(filters.LogBookNumber))
                          && (!filters.PageNumber.HasValue
                               || (!logBook.IsOnline
                                   && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                           && logBook.LogBookPermitLicenseEndPage.HasValue
                                           && logBook.LogBookPermitLicenseStartPage.Value <= filters.PageNumber.Value
                                           && logBook.LogBookPermitLicenseEndPage.Value >= filters.PageNumber.Value)
                                       || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                           && !logBook.LogBookPermitLicenseEndPage.HasValue
                                           && !string.IsNullOrEmpty(logBook.LogBookOwnerTypeCode)
                                           && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                           && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                           && logBook.StartPageNum <= filters.PageNumber.Value
                                           && logBook.EndPageNum >= filters.PageNumber.Value))))
                          && (string.IsNullOrEmpty(filters.OnlinePageNumber)
                                 || ((string.IsNullOrEmpty(logBookNumber) || logBook.Number == logBookNumber)
                                       && logBook.IsOnline
                                       && ((logBook.LogBookPermitLicenseStartPage.HasValue
                                               && logBook.LogBookPermitLicenseEndPage.HasValue
                                               && logBook.LogBookPermitLicenseStartPage.Value <= onlinePageNumber
                                               && logBook.LogBookPermitLicenseEndPage.Value >= onlinePageNumber)
                                            || (!logBook.LogBookPermitLicenseStartPage.HasValue
                                                   && !logBook.LogBookPermitLicenseEndPage.HasValue
                                                   && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.Person)
                                                   && logBook.LogBookOwnerTypeCode != nameof(LogBookPagePersonTypesEnum.LegalPerson)
                                                   && logBook.StartPageNum <= onlinePageNumber
                                                   && logBook.EndPageNum >= onlinePageNumber))
                                           ))
                   select logBook;
        }

        private IQueryable<LogBookRegisterDTO> FinalizeGetPublicLogBooks(PermittedPublicLogBookBaseQueries baseQueries)
        {

            IQueryable<LogBookRegisterHelper> baseQuery = baseQueries.PersonalLogBooks.Union(baseQueries.LegalLogBooks)
                                                                                      .Union(baseQueries.PersonBuyerLogBooks)
                                                                                      .Union(baseQueries.BuyerLegalLogBooks);

            if (baseQueries.AquaculturePersonaLogBooks != null)
            {
                baseQuery = baseQuery.Union(baseQueries.AquaculturePersonaLogBooks);
            }

            if (baseQueries.AquacultureLegalLogBooks != null)
            {
                baseQuery = baseQuery.Union(baseQueries.AquacultureLegalLogBooks);
            }

            if (baseQueries.ShipLegalLogBooks != null)
            {
                baseQueries.ShipLegalLogBooks = from logBook in baseQueries.ShipLegalLogBooks
                                                group logBook by new
                                                {
                                                    logBook.Id,
                                                    logBook.Number,
                                                    logBook.TypeId,
                                                    logBook.StatusId,
                                                    logBook.StatusName,
                                                    logBook.IsLogBookFinished,
                                                    logBook.LogBookOwnerTypeCode,
                                                    logBook.LogBookTypeCode,
                                                    logBook.Type,
                                                    logBook.ShipId,
                                                    logBook.ShipName,
                                                    logBook.ShipExtMark,
                                                    logBook.AquacultureId,
                                                    logBook.IssueDate,
                                                    logBook.FinishDate,
                                                    logBook.IsOnline,
                                                    logBook.StartPageNum,
                                                    logBook.EndPageNum,
                                                    logBook.AquacultureFacilityName,
                                                    logBook.RegisteredBuyerId,
                                                    logBook.RegisteredPersonBuyerName,
                                                    logBook.RegisteredLegalBuyerName,
                                                    logBook.LogBookPersonName,
                                                    logBook.LogBookLegalName,
                                                    logBook.OwnerEgnEik,
                                                    logBook.OwnerRegisteredByerId,
                                                    logBook.OwnerUnregisteredBuyerPersonId,
                                                    logBook.OwnerUnregisteredBuyerLegalId
                                                } into lb
                                                select new LogBookRegisterHelper
                                                {
                                                    Id = lb.Key.Id,
                                                    Number = lb.Key.Number,
                                                    TypeId = lb.Key.TypeId,
                                                    StatusId = lb.Key.StatusId,
                                                    StatusName = lb.Key.StatusName,
                                                    IsLogBookFinished = lb.Key.IsLogBookFinished,
                                                    LogBookTypeCode = lb.Key.LogBookTypeCode,
                                                    Type = lb.Key.Type,
                                                    LogBookOwnerTypeCode = lb.Key.LogBookOwnerTypeCode,
                                                    ShipId = lb.Key.ShipId,
                                                    AquacultureId = lb.Key.AquacultureId,
                                                    ShipName = lb.Key.ShipName,
                                                    ShipExtMark = lb.Key.ShipExtMark,
                                                    IssueDate = lb.Key.IssueDate,
                                                    FinishDate = lb.Key.FinishDate,
                                                    IsOnline = lb.Key.IsOnline,
                                                    StartPageNum = lb.Key.StartPageNum,
                                                    EndPageNum = lb.Key.EndPageNum,
                                                    AquacultureFacilityName = lb.Key.AquacultureFacilityName,
                                                    RegisteredBuyerId = lb.Key.RegisteredBuyerId,
                                                    RegisteredPersonBuyerName = lb.Key.RegisteredPersonBuyerName,
                                                    RegisteredLegalBuyerName = lb.Key.RegisteredLegalBuyerName,
                                                    LogBookPersonName = lb.Key.LogBookPersonName,
                                                    LogBookLegalName = lb.Key.LogBookLegalName,
                                                    OwnerEgnEik = lb.Key.OwnerEgnEik,
                                                    OwnerRegisteredByerId = lb.Key.OwnerRegisteredByerId,
                                                    OwnerUnregisteredBuyerPersonId = lb.Key.OwnerUnregisteredBuyerPersonId,
                                                    OwnerUnregisteredBuyerLegalId = lb.Key.OwnerUnregisteredBuyerLegalId,
                                                    LogBookPermitLicenseStartPage = default,
                                                    LogBookPermitLicenseEndPage = default
                                                };

                baseQuery = baseQuery.Union(baseQueries.ShipLegalLogBooks);
            }

            if (baseQueries.ShipPersonLogBooks != null)
            {
                baseQueries.ShipPersonLogBooks = from logBook in baseQueries.ShipPersonLogBooks
                                                 group logBook by new
                                                 {
                                                     logBook.Id,
                                                     logBook.Number,
                                                     logBook.TypeId,
                                                     logBook.StatusId,
                                                     logBook.StatusName,
                                                     logBook.IsLogBookFinished,
                                                     logBook.LogBookOwnerTypeCode,
                                                     logBook.LogBookTypeCode,
                                                     logBook.Type,
                                                     logBook.ShipId,
                                                     logBook.ShipName,
                                                     logBook.ShipExtMark,
                                                     logBook.AquacultureId,
                                                     logBook.IssueDate,
                                                     logBook.FinishDate,
                                                     logBook.IsOnline,
                                                     logBook.StartPageNum,
                                                     logBook.EndPageNum,
                                                     logBook.AquacultureFacilityName,
                                                     logBook.RegisteredBuyerId,
                                                     logBook.RegisteredPersonBuyerName,
                                                     logBook.RegisteredLegalBuyerName,
                                                     logBook.LogBookPersonName,
                                                     logBook.LogBookLegalName,
                                                     logBook.OwnerEgnEik,
                                                     logBook.OwnerRegisteredByerId,
                                                     logBook.OwnerUnregisteredBuyerPersonId,
                                                     logBook.OwnerUnregisteredBuyerLegalId
                                                 } into lb
                                                 select new LogBookRegisterHelper
                                                 {
                                                     Id = lb.Key.Id,
                                                     Number = lb.Key.Number,
                                                     TypeId = lb.Key.TypeId,
                                                     StatusId = lb.Key.StatusId,
                                                     StatusName = lb.Key.StatusName,
                                                     IsLogBookFinished = lb.Key.IsLogBookFinished,
                                                     LogBookTypeCode = lb.Key.LogBookTypeCode,
                                                     Type = lb.Key.Type,
                                                     LogBookOwnerTypeCode = lb.Key.LogBookOwnerTypeCode,
                                                     ShipId = lb.Key.ShipId,
                                                     AquacultureId = lb.Key.AquacultureId,
                                                     ShipName = lb.Key.ShipName,
                                                     ShipExtMark = lb.Key.ShipExtMark,
                                                     IssueDate = lb.Key.IssueDate,
                                                     FinishDate = lb.Key.FinishDate,
                                                     IsOnline = lb.Key.IsOnline,
                                                     StartPageNum = lb.Key.StartPageNum,
                                                     EndPageNum = lb.Key.EndPageNum,
                                                     AquacultureFacilityName = lb.Key.AquacultureFacilityName,
                                                     RegisteredBuyerId = lb.Key.RegisteredBuyerId,
                                                     RegisteredPersonBuyerName = lb.Key.RegisteredPersonBuyerName,
                                                     RegisteredLegalBuyerName = lb.Key.RegisteredLegalBuyerName,
                                                     LogBookPersonName = lb.Key.LogBookPersonName,
                                                     LogBookLegalName = lb.Key.LogBookLegalName,
                                                     OwnerEgnEik = lb.Key.OwnerEgnEik,
                                                     OwnerRegisteredByerId = lb.Key.OwnerRegisteredByerId,
                                                     OwnerUnregisteredBuyerPersonId = lb.Key.OwnerUnregisteredBuyerPersonId,
                                                     OwnerUnregisteredBuyerLegalId = lb.Key.OwnerUnregisteredBuyerLegalId,
                                                     LogBookPermitLicenseStartPage = default,
                                                     LogBookPermitLicenseEndPage = default
                                                 };

                baseQuery = baseQuery.Union(baseQueries.ShipPersonLogBooks);
            }


            IQueryable<LogBookRegisterDTO> result = from logBook in baseQuery.ToList().AsQueryable()
                                                    select new LogBookRegisterDTO
                                                    {
                                                        Id = logBook.Id,
                                                        Number = logBook.Number,
                                                        TypeCode = Enum.Parse<LogBookTypesEnum>(logBook.LogBookTypeCode),
                                                        Type = logBook.Type,
                                                        StatusName = logBook.StatusName,
                                                        IsLogBookFinished = logBook.IsLogBookFinished,
                                                        IsOnline = logBook.IsOnline,
                                                        IssueDate = logBook.IssueDate,
                                                        FinishDate = logBook.FinishDate,
                                                        OwnerType = logBook.OwnerRegisteredByerId != null
                                                                    ? LogBookPagePersonTypesEnum.RegisteredBuyer
                                                                    : logBook.OwnerUnregisteredBuyerPersonId != null
                                                                        ? LogBookPagePersonTypesEnum.Person
                                                                        : logBook.OwnerUnregisteredBuyerLegalId != null
                                                                            ? LogBookPagePersonTypesEnum.LegalPerson
                                                                            : null,
                                                        Name = logBook.LogBookPersonName != null
                                                                ? logBook.LogBookPersonName
                                                                : logBook.LogBookLegalName != null
                                                                    ? logBook.LogBookLegalName
                                                                    : logBook.ShipName != null
                                                                       ? logBook.ShipName + " (" + logBook.ShipExtMark + ")"
                                                                       : logBook.AquacultureFacilityName != null
                                                                         ? logBook.AquacultureFacilityName
                                                                         : logBook.RegisteredPersonBuyerName != null
                                                                             ? logBook.RegisteredPersonBuyerName
                                                                             : logBook.RegisteredLegalBuyerName != null
                                                                                 ? logBook.RegisteredLegalBuyerName
                                                                                 : ""
                                                    };

            return result;
        }

        private CommonLogBookPageDataDTO GetCommonLogBookPageDataByAdmissionDocumentNumber(decimal admissionDocumentNumber)
        {
            CommonLogBookPageDataDTO result;

            var documentData = (from logBookPage in Db.AdmissionLogBookPages
                                join originDeclaration in Db.OriginDeclarations on logBookPage.OriginDeclarationId equals originDeclaration.Id into od
                                from originDeclaration in od.DefaultIfEmpty()
                                join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id into sp
                                from shipPage in sp.DefaultIfEmpty()
                                join transportationDocument in Db.TransportationLogBookPages on logBookPage.TransportationDocumentId equals transportationDocument.Id into admissionTransportation
                                from transportationDocument in admissionTransportation.DefaultIfEmpty()
                                where logBookPage.PageNum == admissionDocumentNumber
                                select new
                                {
                                    OriginDeclarationNumber = originDeclaration != null && shipPage != null ? shipPage.PageNum : null,
                                    TransportationDocumentNumber = transportationDocument != null ? transportationDocument.PageNum : default(decimal?),
                                    AdmissionDocumentId = logBookPage.Id,
                                    AdmissionDocumentHandoverDate = logBookPage.HandoverDate
                                }).First();

            if (!string.IsNullOrEmpty(documentData.OriginDeclarationNumber)) // Най-вероятно е да има декларация за произход, ако е придобита от кораб
            {
                result = GetCommonLogBookPageDataByOriginDeclarationNumber(documentData.OriginDeclarationNumber);
            }
            else // Ако не е налична декларация за произход, то със сигурност трябва да има документ за транспорт (който също няма да има декларация за произход, тоест няма да е внос с кораб)
            {
                result = GetCommonLogBookPageDataByTransportationDocumentNumber(documentData.TransportationDocumentNumber.Value);
            }

            result.AdmissionDocumentId = documentData.AdmissionDocumentId;
            result.AdmissionDocumentNumber = admissionDocumentNumber;
            result.AdmissionHandoverDate = documentData.AdmissionDocumentHandoverDate;

            return result;
        }

        private CommonLogBookPageDataDTO GetCommonLogBookPageDataByTransportationDocumentNumber(decimal transportationDocumentNumber)
        {
            CommonLogBookPageDataDTO result = null;

            var documentData = (from logBookPage in Db.TransportationLogBookPages
                                join originDeclaration in Db.OriginDeclarations on logBookPage.OriginDeclarationId equals originDeclaration.Id into od
                                from originDeclaration in od.DefaultIfEmpty()
                                join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id into sp
                                from shipPage in sp.DefaultIfEmpty()
                                where logBookPage.PageNum == transportationDocumentNumber
                                select new
                                {
                                    OriginDeclarationNumber = originDeclaration != null && shipPage != null ? shipPage.PageNum : null,
                                    TransportationDocumentId = logBookPage.Id,
                                    TransportationLoadingDate = logBookPage.LoadingDate
                                }).SingleOrDefault();

            if (documentData != null && !string.IsNullOrEmpty(documentData.OriginDeclarationNumber))
            {
                result = GetCommonLogBookPageDataByOriginDeclarationNumber(documentData.OriginDeclarationNumber);
                result.TransportationDocumentId = documentData.TransportationDocumentId;
                result.TransportationDocumentNumber = transportationDocumentNumber;
                result.TransportationDocumentDate = documentData.TransportationLoadingDate;
            }
            else
            {
                result = (from logBookPage in Db.TransportationLogBookPages
                          join originDeclaration in Db.OriginDeclarations on logBookPage.OriginDeclarationId equals originDeclaration.Id into od
                          from originDeclaration in od.DefaultIfEmpty()
                          join shipLogBookPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipLogBookPage.Id into shipPages
                          from shipLogBookPage in shipPages.DefaultIfEmpty()
                          join shipLogBook in Db.LogBooks on shipLogBookPage.LogBookId equals shipLogBook.Id into shipLogBooks
                          from shipLogBook in shipLogBooks.DefaultIfEmpty()
                          join logBookPermitLicense in Db.LogBookPermitLicenses on shipLogBook.Id equals logBookPermitLicense.LogBookId into logBookLicenses
                          from logBookPermitLicense in logBookLicenses.DefaultIfEmpty()
                          join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id into licenses
                          from permitLicense in licenses.DefaultIfEmpty()
                          join captain in Db.FishermenRegisters on permitLicense.QualifiedFisherId equals captain.Id into captains
                          from captain in captains.DefaultIfEmpty()
                          join captainPerson in Db.Persons on captain.PersonId equals captainPerson.Id into captainPersons
                          from captainPerson in captainPersons.DefaultIfEmpty()
                          join vendorPerson in Db.Persons on permitLicense.SubmittedForPersonId equals vendorPerson.Id into vp
                          from vendorPerson in vp.DefaultIfEmpty()
                          join vendorLegal in Db.Legals on permitLicense.SubmittedForLegalId equals vendorLegal.Id into vl
                          from vendorLegal in vl.DefaultIfEmpty()
                          where logBookPage.PageNum == transportationDocumentNumber
                          select new CommonLogBookPageDataDTO
                          {
                              ShipId = shipLogBook != null ? shipLogBook.ShipId : default,
                              TransportationDocumentId = logBookPage.Id,
                              TransportationDocumentNumber = logBookPage.PageNum,
                              TransportationDocumentDate = logBookPage.LoadingDate,
                              OriginDeclarationId = logBookPage.OriginDeclarationId,
                              OriginDeclarationNumber = shipLogBookPage != null ? shipLogBookPage.PageNum : null,
                              OriginDeclarationDate = shipLogBookPage != null ? shipLogBookPage.PageFillDate : default,
                              CaptainName = captainPerson != null ? $"{captainPerson.FirstName} {captainPerson.LastName} ({captainPerson.EgnLnc})" : null,
                              UnloadingInformation = shipLogBookPage != null
                                                     ? string.Join(';', from originDeclaration in Db.OriginDeclarations
                                                                        join originDeclarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals originDeclarationFish.OriginDeclarationId
                                                                        join port in Db.Nports on originDeclarationFish.UnloadPortId equals port.Id
                                                                        where originDeclaration.LogBookPageId == shipLogBookPage.Id
                                                                              && originDeclaration.IsActive
                                                                              && originDeclarationFish.IsActive
                                                                        select " " + port.Name + " (" + originDeclarationFish.UnloadDateTime + ")"
                                                                  )
                                                     : null,
                              VendorName = vendorPerson != null
                                      ? $"{vendorPerson.FirstName} {vendorPerson.LastName} ({vendorPerson.EgnLnc})"
                                      : vendorLegal != null
                                        ? $"{vendorLegal.Name} ({vendorLegal.Eik})"
                                        : null,
                              IsImportNotByShip = logBookPage.IsImportNotByShip,
                              PlaceOfImport = logBookPage.ImportPlace
                          }).First();
            }

            return result;
        }

        private CommonLogBookPageDataDTO GetCommonLogBookPageDataByOriginDeclarationNumber(string originDeclarationNumber)
        {
            CommonLogBookPageDataDTO result = (from logBookPage in Db.ShipLogBookPages
                                               join logBook in Db.LogBooks on logBookPage.LogBookId equals logBook.Id
                                               join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId
                                               join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id
                                               join captain in Db.FishermenRegisters on permitLicense.QualifiedFisherId equals captain.Id
                                               join captainPerson in Db.Persons on captain.PersonId equals captainPerson.Id
                                               join vendorPerson in Db.Persons on permitLicense.SubmittedForPersonId equals vendorPerson.Id into vp
                                               from vendorPerson in vp.DefaultIfEmpty()
                                               join vendorLegal in Db.Legals on permitLicense.SubmittedForLegalId equals vendorLegal.Id into vl
                                               from vendorLegal in vl.DefaultIfEmpty()
                                               join originDeclaration in Db.OriginDeclarations on logBookPage.Id equals originDeclaration.LogBookPageId into originDec
                                               from originDeclaration in originDec.DefaultIfEmpty()
                                               where logBookPage.PageNum == originDeclarationNumber
                                               select new CommonLogBookPageDataDTO
                                               {
                                                   ShipId = logBook.ShipId.Value,
                                                   OriginDeclarationId = originDeclaration != null ? originDeclaration.Id : default(int?),
                                                   OriginDeclarationNumber = logBookPage.PageNum,
                                                   OriginDeclarationDate = logBookPage.PageFillDate,
                                                   CaptainName = $"{captainPerson.FirstName} {captainPerson.LastName} ({captainPerson.EgnLnc})",
                                                   UnloadingInformation = string.Join(';', from originDeclaration in Db.OriginDeclarations
                                                                                           join originDeclarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals originDeclarationFish.OriginDeclarationId
                                                                                           join port in Db.Nports on originDeclarationFish.UnloadPortId equals port.Id
                                                                                           where originDeclaration.LogBookPageId == logBookPage.Id
                                                                                                 && originDeclaration.IsActive
                                                                                                 && originDeclarationFish.IsActive
                                                                                           select " " + port.Name + " (" + originDeclarationFish.UnloadDateTime + ")"
                                                                                     ),
                                                   VendorName = vendorPerson != null
                                                                 ? $"{vendorPerson.FirstName} {vendorPerson.LastName} ({vendorPerson.EgnLnc})"
                                                                 : $"{vendorLegal.Name} ({vendorLegal.Eik})",
                                                   IsImportNotByShip = false
                                               }).First();

            return result;
        }

        private CommonLogBookPageDataDTO GetCommonLogBookPageDataByShipLogBookPage(int shipLogBookPageId)
        {
            CommonLogBookPageDataDTO result = (from logBookPage in Db.ShipLogBookPages
                                               join logBook in Db.LogBooks on logBookPage.LogBookId equals logBook.Id
                                               join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId
                                               join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id
                                               join captain in Db.FishermenRegisters on permitLicense.QualifiedFisherId equals captain.Id
                                               join captainPerson in Db.Persons on captain.PersonId equals captainPerson.Id
                                               join vendorPerson in Db.Persons on permitLicense.SubmittedForPersonId equals vendorPerson.Id into vp
                                               from vendorPerson in vp.DefaultIfEmpty()
                                               join vendorLegal in Db.Legals on permitLicense.SubmittedForLegalId equals vendorLegal.Id into vl
                                               from vendorLegal in vl.DefaultIfEmpty()
                                               join originDeclaration in Db.OriginDeclarations on logBookPage.Id equals originDeclaration.LogBookPageId into originDec
                                               from originDeclaration in originDec.DefaultIfEmpty()
                                               where logBookPage.Id == shipLogBookPageId
                                               select new CommonLogBookPageDataDTO
                                               {
                                                   ShipId = logBook.ShipId.Value,
                                                   OriginDeclarationId = originDeclaration != null ? originDeclaration.Id : default(int?),
                                                   OriginDeclarationNumber = logBookPage.PageNum,
                                                   OriginDeclarationDate = logBookPage.PageFillDate,
                                                   CaptainName = $"{captainPerson.FirstName} {captainPerson.LastName} ({captainPerson.EgnLnc})",
                                                   UnloadingInformation = string.Join(';', from originDeclaration in Db.OriginDeclarations
                                                                                           join originDeclarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals originDeclarationFish.OriginDeclarationId
                                                                                           join port in Db.Nports on originDeclarationFish.UnloadPortId equals port.Id
                                                                                           where originDeclaration.LogBookPageId == logBookPage.Id
                                                                                                 && originDeclaration.IsActive
                                                                                                 && originDeclarationFish.IsActive
                                                                                           select " " + port.Name + " (" + originDeclarationFish.UnloadDateTime + ")"
                                                                                     ),
                                                   VendorName = vendorPerson != null
                                                                 ? $"{vendorPerson.FirstName} {vendorPerson.LastName} ({vendorPerson.EgnLnc})"
                                                                 : $"{vendorLegal.Name} ({vendorLegal.Eik})",
                                                   IsImportNotByShip = false
                                               }).First();

            return result;
        }

        private List<LogBookPageProductDTO> GetProductsDTO(IQueryable<LogBookPageProduct> productsQuery)
        {
            List<LogBookPageProductDTO> products = (from product in productsQuery
                                                    join aquaticOrganism in Db.Nfishes on product.FishId equals aquaticOrganism.Id
                                                    join freshness in Db.NfishFreshnesses on product.ProductFreshnessId equals freshness.Id into productFreshness
                                                    from freshness in productFreshness.DefaultIfEmpty()
                                                    join presentation in Db.NfishPresentations on product.ProductPresentationId equals presentation.Id
                                                    join purpose in Db.NfishSalePurposes on product.ProductPurposeId equals purpose.Id
                                                    select new LogBookPageProductDTO
                                                    {
                                                        Id = product.Id,
                                                        OriginProductId = product.OriginProductId,
                                                        OriginDeclarationFishId = product.OriginDeclarationFishId,
                                                        FishId = product.FishId,
                                                        FishSizeCategoryId = product.FishSizeCategoryId,
                                                        CatchLocation = product.CatchLocation,
                                                        MinSize = product.MinSize,
                                                        ProductFreshnessId = product.ProductFreshnessId,
                                                        ProductPresentationId = product.ProductPresentationId,
                                                        ProductPurposeId = product.ProductPurposeId,
                                                        QuantityKg = product.QuantityKg,
                                                        UnitPrice = product.UnitPrice,
                                                        AverageUnitWeightKg = product.AverageUnitWeightKg,
                                                        UnitCount = product.UnitCount,
                                                        TurbotSizeGroupId = product.TurbotSizeGroupId,
                                                        LogBookType = product.FirstSaleLogBookPageId.HasValue
                                                                      ? LogBookTypesEnum.FirstSale
                                                                      : product.AdmissionLogBookPageId.HasValue
                                                                        ? LogBookTypesEnum.Admission
                                                                        : product.TransportationLogBookPageId.HasValue
                                                                            ? LogBookTypesEnum.Transportation
                                                                            : product.AquacultureLogBookPageId.HasValue
                                                                                ? LogBookTypesEnum.Aquaculture
                                                                                : default(LogBookTypesEnum?),
                                                        IsActive = product.IsActive
                                                    }).ToList();

            return products;
        }

        private List<LogBookPageProductDTO> GetNewProductsByOriginDeclarationId(int originDeclarationId)
        {
            List<SoldFishHelper> usedFishQuantitiesGrouped;

            List<SoldFishHelper> soldFishQuantitiesGrouped = GetAlreadySoldFishes(originDeclarationId);
            List<SoldFishHelper> admissionedFishQuantitiesGrouped = GetAlreadyAdmissionedProducts(originDeclarationId);
            List<SoldFishHelper> transportedFishQuantitiesGrouped = GetAlreadyTransportatedProducts(originDeclarationId);

            usedFishQuantitiesGrouped = soldFishQuantitiesGrouped.Concat(admissionedFishQuantitiesGrouped)
                                                                 .Concat(transportedFishQuantitiesGrouped)
                                                                 .ToList();

            List<LogBookPageProductDTO> products = (from originDeclarationFish in Db.OriginDeclarationFish
                                                    join catchRecordFish in Db.CatchRecordFish on originDeclarationFish.CatchRecordFishId equals catchRecordFish.Id into originCatchRecord
                                                    from catchRecordFish in originCatchRecord.DefaultIfEmpty()
                                                    join quadrant in Db.NcatchZones on catchRecordFish.CatchZoneId equals quadrant.Id into catchQuadrant
                                                    from quadrant in catchQuadrant.DefaultIfEmpty()
                                                    join unloadingType in Db.NcatchFishUnloadTypes on originDeclarationFish.UnloadTypeId equals unloadingType.Id
                                                    where originDeclarationFish.OriginDeclarationId == originDeclarationId
                                                          && originDeclarationFish.IsActive
                                                          && (catchRecordFish == null || catchRecordFish.IsActive)
                                                          && unloadingType.Code == nameof(UnloadingTypesEnum.UNL)
                                                    select new LogBookPageProductDTO
                                                    {
                                                        OriginDeclarationFishId = originDeclarationFish.Id,
                                                        FishId = originDeclarationFish.FishId,
                                                        CatchLocation = quadrant != null ? $"{quadrant.Gfcmquadrant} ({quadrant.ZoneNum})" : "",
                                                        QuantityKg = originDeclarationFish.Quantity,
                                                        ProductFreshnessId = originDeclarationFish.CatchFishFreshnessId,
                                                        ProductPresentationId = originDeclarationFish.CatchFishPresentationId,
                                                        HasMissingProperties = true,
                                                        IsActive = true
                                                    }).ToList();

            usedFishQuantitiesGrouped = GroupUsedFishQuantitiesByOriginDeclarationFish(usedFishQuantitiesGrouped);

            products = (from product in products
                        join sold in usedFishQuantitiesGrouped on product.OriginDeclarationFishId equals sold.OriginDeclarationFishId into soldProduct
                        from sold in soldProduct.DefaultIfEmpty()
                        select new LogBookPageProductDTO
                        {
                            OriginDeclarationFishId = product.OriginDeclarationFishId,
                            FishId = product.FishId,
                            CatchLocation = product.CatchLocation,
                            QuantityKg = sold != null ? product.QuantityKg - sold.Quantity : product.QuantityKg,
                            ProductFreshnessId = product.ProductFreshnessId,
                            ProductPresentationId = product.ProductPresentationId,
                            HasMissingProperties = product.HasMissingProperties,
                            IsActive = product.IsActive
                        }).Where(x => x.QuantityKg > 0).ToList();

            return products;
        }

        private List<LogBookPageProductDTO> GetNewProductsByTransportationDocument(int transportationDocumentId)
        {
            List<SoldFishHelper> usedFishQuantitiesGrouped = new List<SoldFishHelper>();

            List<SoldFishHelper> soldFishQuantitiesGrouped = GetAlreadySoldTransportationProducts(transportationDocumentId);
            List<SoldFishHelper> admissionedFishQuantitiesGrouped = GetAlreadyAdmissionedTransportedProducts(transportationDocumentId);

            usedFishQuantitiesGrouped = soldFishQuantitiesGrouped.Concat(admissionedFishQuantitiesGrouped).ToList();

            usedFishQuantitiesGrouped = GroupUsedFishQuantitiesByOriginDeclarationFish(usedFishQuantitiesGrouped);

            List<LogBookPageProductDTO> products = (from transportationDoc in Db.TransportationLogBookPages
                                                    join product in Db.LogBookPageProducts on transportationDoc.Id equals product.TransportationLogBookPageId
                                                    join fish in Db.Nfishes on product.FishId equals fish.Id
                                                    where transportationDoc.Id == transportationDocumentId
                                                          && product.IsActive
                                                    select new LogBookPageProductDTO
                                                    {
                                                        OriginProductId = product.Id,
                                                        OriginDeclarationFishId = product.OriginDeclarationFishId,
                                                        FishId = product.FishId,
                                                        CatchLocation = product.CatchLocation,
                                                        QuantityKg = product.QuantityKg,
                                                        UnitPrice = product.UnitPrice,
                                                        ProductFreshnessId = product.ProductFreshnessId,
                                                        ProductPresentationId = product.ProductPresentationId,
                                                        ProductPurposeId = product.ProductPurposeId,
                                                        TurbotSizeGroupId = product.TurbotSizeGroupId,
                                                        UnitCount = product.UnitCount,
                                                        AverageUnitWeightKg = product.AverageUnitWeightKg,
                                                        FishSizeCategoryId = product.FishSizeCategoryId,
                                                        HasMissingProperties = true,
                                                        IsActive = true
                                                    }).ToList();

            products = (from product in products
                        join sold in usedFishQuantitiesGrouped on product.OriginDeclarationFishId equals sold.OriginDeclarationFishId into soldProduct
                        from sold in soldProduct.DefaultIfEmpty()
                        select new LogBookPageProductDTO
                        {
                            OriginProductId = product.OriginProductId,
                            OriginDeclarationFishId = product.OriginDeclarationFishId,
                            FishId = product.FishId,
                            CatchLocation = product.CatchLocation,
                            QuantityKg = sold != null ? product.QuantityKg - sold.Quantity : product.QuantityKg,
                            UnitPrice = product.UnitPrice,
                            ProductFreshnessId = product.ProductFreshnessId,
                            ProductPresentationId = product.ProductPresentationId,
                            ProductPurposeId = product.ProductPurposeId,
                            TurbotSizeGroupId = product.TurbotSizeGroupId,
                            UnitCount = product.UnitCount,
                            AverageUnitWeightKg = product.AverageUnitWeightKg,
                            HasMissingProperties = product.HasMissingProperties,
                            FishSizeCategoryId = product.FishSizeCategoryId,
                            IsActive = product.IsActive
                        }).Where(x => x.QuantityKg > 0).ToList();

            return products;
        }

        private List<LogBookPageProductDTO> GetNewProductsByAdmissionDocument(int admissionDocumentId)
        {
            List<SoldFishHelper> usedFishQuantitiesGrouped = new List<SoldFishHelper>();

            usedFishQuantitiesGrouped = GetAlreadSoldAdmissionedProducts(admissionDocumentId);

            usedFishQuantitiesGrouped = GroupUsedFishQuantitiesByOriginDeclarationFish(usedFishQuantitiesGrouped);

            List<LogBookPageProductDTO> products = (from admissionDoc in Db.AdmissionLogBookPages
                                                    join product in Db.LogBookPageProducts on admissionDoc.Id equals product.AdmissionLogBookPageId
                                                    join fish in Db.Nfishes on product.FishId equals fish.Id
                                                    where admissionDoc.Id == admissionDocumentId
                                                          && product.IsActive
                                                    select new LogBookPageProductDTO
                                                    {
                                                        OriginProductId = product.Id,
                                                        OriginDeclarationFishId = product.OriginDeclarationFishId,
                                                        FishId = product.FishId,
                                                        CatchLocation = product.CatchLocation,
                                                        QuantityKg = product.QuantityKg,
                                                        UnitPrice = product.UnitPrice,
                                                        ProductFreshnessId = product.ProductFreshnessId,
                                                        ProductPresentationId = product.ProductPresentationId,
                                                        ProductPurposeId = product.ProductPurposeId,
                                                        TurbotSizeGroupId = product.TurbotSizeGroupId,
                                                        UnitCount = product.UnitCount,
                                                        AverageUnitWeightKg = product.AverageUnitWeightKg,
                                                        MinSize = product.MinSize,
                                                        FishSizeCategoryId = product.FishSizeCategoryId,
                                                        HasMissingProperties = true,
                                                        IsActive = true
                                                    }).ToList();

            products = (from product in products
                        join sold in usedFishQuantitiesGrouped on product.OriginDeclarationFishId equals sold.OriginDeclarationFishId into soldProduct
                        from sold in soldProduct.DefaultIfEmpty()
                        select new LogBookPageProductDTO
                        {
                            OriginProductId = product.OriginProductId,
                            OriginDeclarationFishId = product.OriginDeclarationFishId,
                            FishId = product.FishId,
                            CatchLocation = product.CatchLocation,
                            QuantityKg = sold != null ? product.QuantityKg - sold.Quantity : product.QuantityKg,
                            UnitPrice = product.UnitPrice,
                            ProductFreshnessId = product.ProductFreshnessId,
                            ProductPresentationId = product.ProductPresentationId,
                            ProductPurposeId = product.ProductPurposeId,
                            TurbotSizeGroupId = product.TurbotSizeGroupId,
                            UnitCount = product.UnitCount,
                            AverageUnitWeightKg = product.AverageUnitWeightKg,
                            FishSizeCategoryId = product.FishSizeCategoryId,
                            MinSize = product.MinSize,
                            HasMissingProperties = product.HasMissingProperties,
                            IsActive = product.IsActive
                        }).Where(x => x.QuantityKg > 0).ToList();

            return products;
        }

        private List<SoldFishHelper> GroupUsedFishQuantitiesByOriginDeclarationFish(List<SoldFishHelper> usedFishQuantitiesGrouped)
        {
            usedFishQuantitiesGrouped = (from usedFish in usedFishQuantitiesGrouped
                                         group usedFish by usedFish.OriginDeclarationFishId into uf
                                         select new SoldFishHelper
                                         {
                                             FishId = uf.First().FishId,
                                             OriginDeclarationFishId = uf.Key,
                                             Quantity = uf.Sum(x => x.Quantity)
                                         }).ToList();

            return usedFishQuantitiesGrouped;
        }

        private List<SoldFishHelper> GetAlreadySoldFishes(int originDeclarationId)
        {
            IQueryable<SoldFishHelper> firstSaleFishesQuantities = from product in Db.LogBookPageProducts
                                                                   join originDeclarationFish in Db.OriginDeclarationFish on product.OriginDeclarationFishId equals originDeclarationFish.Id
                                                                   where product.FirstSaleLogBookPageId.HasValue
                                                                         && product.IsActive
                                                                         && originDeclarationFish.OriginDeclarationId == originDeclarationId
                                                                   select new SoldFishHelper
                                                                   {
                                                                       FishId = product.FishId,
                                                                       OriginDeclarationFishId = originDeclarationFish.Id,
                                                                       Quantity = product.QuantityKg
                                                                   };

            List<SoldFishHelper> soldFishQuantitiesGrouped = GetFishQuantitiesGrouped(firstSaleFishesQuantities);

            return soldFishQuantitiesGrouped;
        }

        private List<SoldFishHelper> GetAlreadySoldTransportationProducts(int transportationDocumentId)
        {
            IQueryable<SoldFishHelper> transportationProductsQuantities = from product in Db.LogBookPageProducts
                                                                          join originProduct in Db.LogBookPageProducts on product.OriginProductId equals originProduct.Id
                                                                          where product.FirstSaleLogBookPageId.HasValue
                                                                                && originProduct.TransportationLogBookPageId == transportationDocumentId
                                                                                && originProduct.IsActive
                                                                                && product.IsActive
                                                                          select new SoldFishHelper
                                                                          {
                                                                              FishId = originProduct.FishId,
                                                                              OriginDeclarationFishId = originProduct.OriginDeclarationFishId,
                                                                              Quantity = product.QuantityKg
                                                                          };

            List<SoldFishHelper> soldFishQuantitiesGrouped = GetFishQuantitiesGrouped(transportationProductsQuantities);

            return soldFishQuantitiesGrouped;
        }

        private List<SoldFishHelper> GetAlreadSoldAdmissionedProducts(int admissionDocumentId)
        {
            IQueryable<SoldFishHelper> admissionedProductsQuantities = from product in Db.LogBookPageProducts
                                                                       join originProduct in Db.LogBookPageProducts on product.OriginProductId equals originProduct.Id
                                                                       where product.FirstSaleLogBookPageId.HasValue
                                                                             && originProduct.AdmissionLogBookPageId == admissionDocumentId
                                                                             && originProduct.IsActive
                                                                             && product.IsActive
                                                                       select new SoldFishHelper
                                                                       {
                                                                           FishId = originProduct.FishId,
                                                                           OriginDeclarationFishId = originProduct.OriginDeclarationFishId,
                                                                           Quantity = product.QuantityKg
                                                                       };

            List<SoldFishHelper> soldFishQuantitiesGrouped = GetFishQuantitiesGrouped(admissionedProductsQuantities);

            return soldFishQuantitiesGrouped;
        }

        private List<SoldFishHelper> GetAlreadyAdmissionedTransportedProducts(int transportationDocumentId)
        {
            IQueryable<SoldFishHelper> admissionedProductsQuantities = from product in Db.LogBookPageProducts
                                                                       join originProduct in Db.LogBookPageProducts on product.OriginProductId equals originProduct.Id
                                                                       where product.AdmissionLogBookPageId.HasValue
                                                                             && originProduct.TransportationLogBookPageId == transportationDocumentId
                                                                             && originProduct.IsActive
                                                                             && product.IsActive
                                                                       select new SoldFishHelper
                                                                       {
                                                                           FishId = originProduct.FishId,
                                                                           OriginDeclarationFishId = originProduct.OriginDeclarationFishId,
                                                                           Quantity = product.QuantityKg
                                                                       };

            List<SoldFishHelper> soldFishQuantitiesGrouped = GetFishQuantitiesGrouped(admissionedProductsQuantities);

            return soldFishQuantitiesGrouped;
        }

        private List<SoldFishHelper> GetAlreadyAdmissionedProducts(int originDeclarationId)
        {
            IQueryable<SoldFishHelper> admissionedFishQuantities = from product in Db.LogBookPageProducts
                                                                   join originDeclarationFish in Db.OriginDeclarationFish on product.OriginDeclarationFishId equals originDeclarationFish.Id
                                                                   where product.AdmissionLogBookPageId.HasValue
                                                                         && product.IsActive
                                                                         && originDeclarationFish.OriginDeclarationId == originDeclarationId
                                                                   select new SoldFishHelper
                                                                   {
                                                                       FishId = product.FishId,
                                                                       OriginDeclarationFishId = originDeclarationFish.Id,
                                                                       Quantity = product.QuantityKg
                                                                   };

            List<SoldFishHelper> admissionedFishQuantitiesGrouped = GetFishQuantitiesGrouped(admissionedFishQuantities);

            return admissionedFishQuantitiesGrouped;
        }

        private List<SoldFishHelper> GetAlreadyTransportatedProducts(int originDeclarationId)
        {
            IQueryable<SoldFishHelper> transportedFishQuantities = from product in Db.LogBookPageProducts
                                                                   join originDeclarationFish in Db.OriginDeclarationFish on product.OriginDeclarationFishId equals originDeclarationFish.Id
                                                                   where product.TransportationLogBookPageId.HasValue
                                                                         && product.IsActive
                                                                         && originDeclarationFish.OriginDeclarationId == originDeclarationId
                                                                   select new SoldFishHelper
                                                                   {
                                                                       FishId = product.FishId,
                                                                       OriginDeclarationFishId = originDeclarationFish.Id,
                                                                       Quantity = product.QuantityKg
                                                                   };

            List<SoldFishHelper> transportedFishQuantitiesGrouped = GetFishQuantitiesGrouped(transportedFishQuantities);

            return transportedFishQuantitiesGrouped;
        }

        private List<SoldFishHelper> GetFishQuantitiesGrouped(IQueryable<SoldFishHelper> productsQuantities)
        {

            List<SoldFishHelper> soldFishQuantitiesGrouped = (from product in productsQuantities
                                                              group product by new
                                                              {
                                                                  product.FishId,
                                                                  product.OriginDeclarationFishId
                                                              } into grouped
                                                              select new SoldFishHelper
                                                              {
                                                                  FishId = grouped.Key.FishId,
                                                                  OriginDeclarationFishId = grouped.Key.OriginDeclarationFishId,
                                                                  Quantity = grouped.Sum(x => x.Quantity)
                                                              }).ToList();

            return soldFishQuantitiesGrouped;
        }

        private LogBookPagePersonDTO GetLogBookPagePerson(LogBookPagePersonTypesEnum personType, int? buyerId, int? personId, int? legalId)
        {
            LogBookPagePersonDTO logBookPagePerson = new LogBookPagePersonDTO
            {
                PersonType = personType,
                BuyerId = buyerId,
                Person = personId != null ? personService.GetRegixPersonData(personId.Value) : null,
                PersonLegal = legalId != null ? legalService.GetRegixLegalData(legalId.Value) : null,
                Addresses = personId != null
                            ? personService.GetAddressRegistrations(personId.Value)
                            : legalId != null
                                ? legalService.GetAddressRegistrations(legalId.Value)
                                : null
            };

            return logBookPagePerson;
        }

        /// <summary>
        /// Adds or updates each product form the given list according to the logBookType.
        /// According to the logBookType exactly one of the ids must be passed as a parameter.
        /// </summary>
        /// <param name="products">List with the products to add/update</param>
        /// <param name="logBookType">The type of the log book for which the products are</param>
        /// <param name="firstSalePage">Identificator for page when logBookType is `FirstSale`</param>
        /// <param name="admissionPage">Identificator for page when logBookType is `Admission`</param>
        /// <param name="transportationPage">Identificator for page when logBookType is `Transportation`</param>
        /// <param name="aquaculturePage">Identificator for page when logBookType is `Aquaculture`</param>
        private void AddOrEditLogBookPageProducts(List<LogBookPageProductDTO> products,
                                                  LogBookTypesEnum logBookType,
                                                  FirstSaleLogBookPage firstSalePage = null,
                                                  AdmissionLogBookPage admissionPage = null,
                                                  TransportationLogBookPage transportationPage = null,
                                                  AquacultureLogBookPage aquaculturePage = null)
        {
            List<LogBookPageProduct> dbEntries;

            if (products.Any(x => x.Id.HasValue))
            {
                List<int> logBookProductIds = products.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToList();

                dbEntries = (from logBookProduct in Db.LogBookPageProducts
                             where logBookProductIds.Contains(logBookProduct.Id)
                             select logBookProduct).ToList();
            }
            else
            {
                dbEntries = new List<LogBookPageProduct>();
            }

            foreach (LogBookPageProductDTO product in products)
            {
                if (product.Id.HasValue)
                {
                    LogBookPageProduct dbEntry = dbEntries.Where(x => x.Id == product.Id.Value).Single();

                    if (product.LogBookType != LogBookTypesEnum.Aquaculture)
                    {
                        dbEntry.ProductFreshnessId = product.ProductFreshnessId.Value;
                        dbEntry.CatchLocation = product.CatchLocation;
                        dbEntry.MinSize = product.MinSize.Value;
                        dbEntry.AverageUnitWeightKg = product.AverageUnitWeightKg;
                    }
                    else
                    {
                        dbEntry.AverageUnitWeightKg = product.AverageUnitWeightKg.Value;
                    }

                    dbEntry.ProductPresentationId = product.ProductPresentationId.Value;
                    dbEntry.ProductPurposeId = product.ProductPurposeId.Value;
                    dbEntry.QuantityKg = product.QuantityKg.Value;
                    dbEntry.UnitPrice = product.UnitPrice.Value;
                    dbEntry.UnitCount = product.UnitCount;
                    dbEntry.TurbotSizeGroupId = product.TurbotSizeGroupId;
                    dbEntry.FishSizeCategoryId = product.FishSizeCategoryId;
                    dbEntry.IsActive = product.IsActive.Value;
                }
                else
                {
                    LogBookPageProduct entry = new LogBookPageProduct
                    {
                        FishId = product.FishId.Value,
                        FishSizeCategoryId = product.FishSizeCategoryId,
                        OriginDeclarationFishId = product.OriginDeclarationFishId,
                        OriginProductId = product.OriginProductId,
                        ProductPresentationId = product.ProductPresentationId.Value,
                        ProductPurposeId = product.ProductPurposeId.Value,
                        QuantityKg = product.QuantityKg.Value,
                        UnitPrice = product.UnitPrice.Value,
                        UnitCount = product.UnitCount,
                        TurbotSizeGroupId = product.TurbotSizeGroupId
                    };

                    if (product.LogBookType != LogBookTypesEnum.Aquaculture)
                    {
                        entry.ProductFreshnessId = product.ProductFreshnessId.Value;
                        entry.CatchLocation = product.CatchLocation;
                        entry.MinSize = product.MinSize.Value;
                        entry.AverageUnitWeightKg = product.AverageUnitWeightKg;
                    }
                    else
                    {
                        entry.AverageUnitWeightKg = product.AverageUnitWeightKg.Value;
                    }

                    switch (logBookType)
                    {
                        case LogBookTypesEnum.FirstSale:
                            entry.FirstSaleLogBookPage = firstSalePage;
                            break;
                        case LogBookTypesEnum.Admission:
                            entry.AdmissionLogBookPage = admissionPage;
                            break;
                        case LogBookTypesEnum.Transportation:
                            entry.TransportationLogBookPage = transportationPage;
                            break;
                        case LogBookTypesEnum.Aquaculture:
                            entry.AquacultureLogBookPage = aquaculturePage;
                            break;
                        default:
                            throw new ArgumentException($"no products functionality for ${logBookType.ToString()} is available");
                    }

                    Db.LogBookPageProducts.Add(entry);
                }
            }

            List<int> productIds = products.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToList();
            List<LogBookPageProduct> dbEntriesToDelete = dbEntries.Where(x => !productIds.Contains(x.Id)).ToList();

            foreach (LogBookPageProduct product in dbEntriesToDelete)
            {
                product.IsActive = false;
            }
        }

        private LogBookTypesEnum GetLogBookType(int logBookTypeId)
        {
            LogBookTypesEnum logBookType = (from lbType in Db.NlogBookTypes
                                            where lbType.Id == logBookTypeId
                                            select Enum.Parse<LogBookTypesEnum>(lbType.Code)).First();

            return logBookType;
        }

        private static IQueryable<LogBookEditDTO> MapLogBooksToDTO(IQueryable<LogBook> logBooks)
        {
            IQueryable<LogBookEditDTO> results = from logBook in logBooks
                                                 select new LogBookEditDTO
                                                 {
                                                     LogBookId = logBook.Id,
                                                     IsOnline = logBook.IsOnline,
                                                     StatusId = logBook.StatusId,
                                                     OwnerType = logBook.LogBookOwnerType != null
                                                                 ? Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType)
                                                                 : null,
                                                     LogbookNumber = logBook.LogNum,
                                                     LogBookTypeId = logBook.LogBookTypeId,
                                                     IssueDate = logBook.IssueDate,
                                                     FinishDate = logBook.FinishDate,
                                                     StartPageNumber = logBook.StartPageNum,
                                                     EndPageNumber = logBook.EndPageNum,
                                                     Comment = logBook.Comments,
                                                     Price = logBook.Price,
                                                     LogBookIsActive = logBook.IsActive,
                                                     IsActive = logBook.IsActive
                                                 };

            return results;
        }

        private OldLogBookPageStatus GetNewLogBookPageStatusAndCheckValidity<T>(decimal pageNumber, int logBookId, DbSet<T> pages)
            where T : class, ILogBookPageDecimalEntity
        {
            bool isLogBookOwnerPersonOrLegal = (from logBook in Db.LogBooks
                                                where logBook.Id == logBookId
                                                      && (logBook.PersonId.HasValue || logBook.LegalId.HasValue)
                                                select logBook.Id).Any();

            // Ако дневникът е за ФЛ/ЮЛ, то той е към титуляр на удостоверение и трябва за него да има запис в LogBookPermitLicense таблицата
            if (isLogBookOwnerPersonOrLegal)
            {
                ThrowIfPageNumberNotInLogBookPermitLicense(logBookId, pageNumber);
            }
            else
            {
                ThrowIfPageNumberNotInLogBook(logBookId, pageNumber);
            }

            OldLogBookPageStatus oldPage = GetOldLogBookPageStatusOrThrowIfAlreadyExists(logBookId, pageNumber, pages);
            return oldPage;
        }

        private OldLogBookPageStatus GetNewLogBookPageStringStatusAndCheckValidity<T>(decimal pageNumber, int logBookId, DbSet<T> pages)
            where T : class, ILogBookPageStringEntity
        {
            ThrowIfPageNumberNotInLogBookPermitLicense(logBookId, pageNumber);

            OldLogBookPageStatus oldPage = GetOldLogBookPageStringStatusOrThrowIfAlreadyExists(logBookId, pageNumber, pages);
            return oldPage;
        }

        private void ThrowIfPageNumberNotInLogBook(int logBookId, decimal pageNumber)
        {
            bool isPageInLogbook = (from logbook in Db.LogBooks
                                    where logbook.Id == logBookId
                                        && logbook.StartPageNum <= pageNumber
                                        && logbook.EndPageNum >= pageNumber
                                    select logbook.Id).Any();

            if (!isPageInLogbook)
            {
                throw new PageNumberNotInLogbookException();
            }
        }

        private void ThrowIfPageNumberNotInLogBookPermitLicense(int logBookId, decimal pageNumber)
        {
            bool isPageInLogBookPermitLicense = (from logBook in Db.LogBooks
                                                 join logBookLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookLicense.LogBookId
                                                 where logBook.Id == logBookId
                                                       && logBookLicense.StartPageNum.HasValue
                                                       && logBookLicense.StartPageNum.Value <= pageNumber
                                                       && logBookLicense.EndPageNum.HasValue
                                                       && logBookLicense.EndPageNum.Value >= pageNumber
                                                 select logBookLicense.Id).Any();

            if (!isPageInLogBookPermitLicense)
            {
                throw new PageNumberNotInLogBookLicenseException();
            }
        }

        private static OldLogBookPageStatus GetOldLogBookPageStatusOrThrowIfAlreadyExists<T>(int logBookId, decimal pageNumber, DbSet<T> pages)
            where T : class, ILogBookPageDecimalEntity
        {
            OldLogBookPageStatus oldPage = (from pg in pages
                                            where pg.LogBookId == logBookId
                                                 && pg.PageNum == pageNumber
                                            select new OldLogBookPageStatus
                                            {
                                                Id = pg.Id,
                                                Status = Enum.Parse<LogBookPageStatusesEnum>(pg.Status)
                                            }).SingleOrDefault();

            if (oldPage != null)
            {
                if (oldPage.Status != LogBookPageStatusesEnum.Missing)
                {
                    throw new LogBookPageAlreadySubmittedException();
                }

                return oldPage;
            }

            return null;
        }

        private static OldLogBookPageStatus GetOldLogBookPageStringStatusOrThrowIfAlreadyExists<T>(int logBookId, decimal pageNumber, DbSet<T> pages)
            where T : class, ILogBookPageStringEntity
        {
            OldLogBookPageStatus oldPage = (from pg in pages
                                            where pg.LogBookId == logBookId
                                                 && pg.PageNum == pageNumber.ToString()
                                            select new OldLogBookPageStatus
                                            {
                                                Id = pg.Id,
                                                Status = Enum.Parse<LogBookPageStatusesEnum>(pg.Status)
                                            }).SingleOrDefault();

            if (oldPage != null)
            {
                if (oldPage.Status != LogBookPageStatusesEnum.Missing)
                {
                    throw new LogBookPageAlreadySubmittedException();
                }

                return oldPage;
            }

            return null;
        }

        private void SendFluxSalesReportDelete(LogBookTypesEnum logbookType, int pageId, Guid fluxIdentifier)
        {
            decimal? shipLength = null;

            switch (logbookType)
            {
                case LogBookTypesEnum.FirstSale:
                    shipLength = (from page in Db.FirstSaleLogBookPages
                                  join originDeclaration in Db.OriginDeclarations on page.OriginDeclarationId equals originDeclaration.Id
                                  join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                                  join shipLogbook in Db.LogBooks on shipPage.LogBookId equals shipLogbook.Id
                                  join ship in Db.ShipsRegister on shipLogbook.ShipId equals ship.Id
                                  where page.Id == pageId
                                  select ship.TotalLength).FirstOrDefault();
                    break;
                case LogBookTypesEnum.Admission:
                    shipLength = (from page in Db.AdmissionLogBookPages
                                  join originDeclaration in Db.OriginDeclarations on page.OriginDeclarationId equals originDeclaration.Id
                                  join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                                  join shipLogbook in Db.LogBooks on shipPage.LogBookId equals shipLogbook.Id
                                  join ship in Db.ShipsRegister on shipLogbook.ShipId equals ship.Id
                                  where page.Id == pageId
                                  select ship.TotalLength).FirstOrDefault();
                    break;
                case LogBookTypesEnum.Transportation:
                    shipLength = (from page in Db.TransportationLogBookPages
                                  join originDeclaration in Db.OriginDeclarations on page.OriginDeclarationId equals originDeclaration.Id
                                  join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                                  join shipLogbook in Db.LogBooks on shipPage.LogBookId equals shipLogbook.Id
                                  join ship in Db.ShipsRegister on shipLogbook.ShipId equals ship.Id
                                  where page.Id == pageId
                                  select ship.TotalLength).FirstOrDefault();
                    break;
            }

            if (salesDomainService.MustSendSalesReport(length: shipLength))
            {
                FLUXSalesReportMessageType flux = salesReportMapper.MapFirstSalePageToSalesReport(pageId,
                                                                                                  ReportPurposeCodes.Delete,
                                                                                                  fluxIdentifier);
                salesDomainService.ReportSalesDocument(flux);
            }
        }
    }

    internal class OldLogBookPageStatus
    {
        public int Id { get; set; }
        public LogBookPageStatusesEnum Status { get; set; }
    }

    internal class PermittedPublicLogBookBaseQueries
    {
        /// <summary>
        /// Log books owned by a person (the logged in user) - for admission or transportation
        /// </summary>
        public IQueryable<LogBookRegisterHelper> PersonalLogBooks { get; set; }

        /// <summary>
        /// Log books owned by a legal for which the logged in user is authorized to fill data - for admission or transportation
        /// </summary>
        public IQueryable<LogBookRegisterHelper> LegalLogBooks { get; set; }

        /// <summary>
        /// Log books owned by a registered buyer Legal for with the logged in user is authorized to fill data - 
        /// for first sale, admission or transportation
        /// </summary>
        public IQueryable<LogBookRegisterHelper> BuyerLegalLogBooks { get; set; }

        /// <summary>
        /// Log books owned by a registered buyer person (the logged in user) - for first sale, admission or transportation
        /// </summary>
        public IQueryable<LogBookRegisterHelper> PersonBuyerLogBooks { get; set; }

        /// <summary>
        /// Log books owned by a legal of an aquaculture facility for which the logged in user is authorized to fill data - for aquaculture log books
        /// </summary>
        public IQueryable<LogBookRegisterHelper> AquacultureLegalLogBooks { get; set; }

        /// <summary>
        /// Log books owned by a person (the logged in user) of an aquaculture facility - for aquaculture log books
        /// </summary>
        public IQueryable<LogBookRegisterHelper> AquaculturePersonaLogBooks { get; set; }

        /// <summary>
        /// Log books owned by a legal submitter of a permit lincese of a ship for which the logged in user is authorized to fill data - for fish log books
        /// </summary>
        public IQueryable<LogBookRegisterHelper> ShipLegalLogBooks { get; set; }

        /// <summary>
        /// Log books owned by a person (the logged in user) submitter of a permit license of a ship - for fish log books
        /// </summary>
        public IQueryable<LogBookRegisterHelper> ShipPersonLogBooks { get; set; }
    }

    internal class LogBookRegisterHelper : LogBookRegisterDTO
    {
        public int TypeId { get; set; }

        public int? OwnerRegisteredByerId { get; set; }

        public int? OwnerUnregisteredBuyerPersonId { get; set; }

        public int? OwnerUnregisteredBuyerLegalId { get; set; }

        public int TeritorryUnitId { get; set; }

        public int? ShipId { get; set; }

        public int? AquacultureId { get; set; }

        public int? RegisteredBuyerId { get; set; }

        public string OwnerEgnEik { get; set; }

        public int StatusId { get; set; }

        public string LogBookTypeCode { get; set; }

        public string ShipName { get; set; }

        public string ShipExtMark { get; set; }

        public string AquacultureFacilityName { get; set; }

        public string RegisteredPersonBuyerName { get; set; }

        public string RegisteredLegalBuyerName { get; set; }

        public string LogBookPersonName { get; set; }

        public string LogBookLegalName { get; set; }

        public long? LogBookPermitLicenseStartPage { get; set; }

        public long? LogBookPermitLicenseEndPage { get; set; }

        public string LogBookOwnerTypeCode { get; set; }
    }

    internal class SoldFishHelper
    {
        public int FishId { get; set; }
        public int? OriginDeclarationFishId { get; set; }
        public decimal Quantity { get; set; }
    }

    internal class DocumentLogBookData
    {
        public int LogBookId { get; set; }
        public string LogBookNumber { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
        public int? RegisteredBuyerId { get; set; }
        public LogBookPagePersonTypesEnum OwnerType { get; set; }
    }
}
