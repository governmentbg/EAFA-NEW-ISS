using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces.CatchSales;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.CatchSales
{
    partial class LogBooksService : Service, ILogBooksService
    {
        public TransportationLogBookPageEditDTO GetTransportationLogBookPage(int id)
        {
            var transportationPageData = (from page in Db.TransportationLogBookPages
                                          join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                                          where page.Id == id
                                          select new
                                          {
                                              Page = new TransportationLogBookPageEditDTO
                                              {
                                                  Id = page.Id,
                                                  PageNumber = page.PageNum,
                                                  LogBookId = page.LogBookId,
                                                  Status = page.Status,
                                                  VehicleIdentification = page.VehicleNumber,
                                                  LoadingDate = page.LoadingDate,
                                                  LoadingLocation = page.LoadingLocation,
                                                  DeliveryLocation = page.DeliveryLocation
                                              },
                                              ReceiverPersonType = Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType),
                                              ReceiverBuyerId = logBook.RegisteredBuyerId,
                                              ReceiverPersonId = logBook.PersonId,
                                              ReceiverLegalId = logBook.LegalId
                                          }).First();

            TransportationLogBookPageEditDTO transportationPage = transportationPageData.Page;

            transportationPage.CommonData = GetCommonLogBookPageDataByTransportationDocumentNumber(transportationPage.PageNumber.Value);

            transportationPage.Receiver = GetLogBookPagePerson(transportationPageData.ReceiverPersonType,
                                                               transportationPageData.ReceiverBuyerId,
                                                               transportationPageData.ReceiverPersonId,
                                                               transportationPageData.ReceiverLegalId);

            transportationPage.Products = GetTransportationPageProducts(id);

            if (transportationPage.CommonData.OriginDeclarationId.HasValue)
            {
                transportationPage.OriginalPossibleProducts = GetNewProductsByOriginDeclarationId(transportationPage.CommonData.OriginDeclarationId.Value);
            }

            transportationPage.Files = Db.GetFiles(Db.TransportationLogBookPageFiles, id);

            return transportationPage;
        }

        public TransportationLogBookPageEditDTO GetNewTransportationLogBookPage(int logBookId, int? originDeclarationId)
        {
            var transportationPageData = (from logBook in Db.LogBooks
                                          where logBook.Id == logBookId
                                          select new
                                          {
                                              LogBookId = logBook.Id,
                                              ReceiverPersonType = Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType),
                                              ReceiverBuyerId = logBook.RegisteredBuyerId,
                                              ReceiverPersonId = logBook.PersonId,
                                              ReceiverLegalId = logBook.LegalId
                                          }).First();

            TransportationLogBookPageEditDTO transportationPage = new TransportationLogBookPageEditDTO
            {
                LogBookId = transportationPageData.LogBookId
            };

            transportationPage.Receiver = GetLogBookPagePerson(transportationPageData.ReceiverPersonType,
                                                               transportationPageData.ReceiverBuyerId,
                                                               transportationPageData.ReceiverPersonId,
                                                               transportationPageData.ReceiverLegalId);

            if (originDeclarationId.HasValue)
            {
                transportationPage.Products = GetNewProductsByOriginDeclarationId(originDeclarationId.Value);
                transportationPage.OriginalPossibleProducts = transportationPage.Products;
            }

            return transportationPage;
        }

        public int AddTransportationLogBookPage(TransportationLogBookPageEditDTO page)
        {
            OldLogBookPageStatus oldPage = GetNewLogBookPageStatusAndCheckValidity(page.PageNumber.Value,
                                                                                   page.LogBookId.Value,
                                                                                   Db.TransportationLogBookPages);

            if (oldPage != null && oldPage.Status == LogBookPageStatusesEnum.Missing)
            {
                page.Id = oldPage.Id;
                EditTransportationLogBookPage(page);

                return page.Id.Value;
            }

            LogBook logBook = (from book in Db.LogBooks
                               where book.Id == page.LogBookId.Value
                               select book).First();

            int? logBookPermitLicenseId = page.LogBookPermitLicenseId;

            if (!logBookPermitLicenseId.HasValue && logBook.LogBookOwnerType != nameof(LogBookPagePersonTypesEnum.RegisteredBuyer))
            {
                logBookPermitLicenseId = (from lb in Db.LogBooks
                                          join logBookLicense in Db.LogBookPermitLicenses on lb.Id equals logBookLicense.LogBookId
                                          where logBookLicense.LogBookId == logBook.Id
                                                && logBookLicense.StartPageNum.HasValue
                                                && logBookLicense.StartPageNum.Value <= page.PageNumber
                                                && logBookLicense.EndPageNum.HasValue
                                                && logBookLicense.EndPageNum.Value >= page.PageNumber
                                          orderby logBookLicense.LogBookValidTo descending
                                          select logBookLicense.Id).First();
            }

            long lastUsedPageNum;

            if (logBook.LastPageNum != default)
            {
                lastUsedPageNum = logBook.LastPageNum;
            }
            else
            {
                if (logBookPermitLicenseId.HasValue)
                {
                    lastUsedPageNum = (from logBookPermitLicense in Db.LogBookPermitLicenses
                                       where logBookPermitLicense.Id == logBookPermitLicenseId
                                       select logBookPermitLicense.StartPageNum).First().Value - 1;
                }
                else
                {
                    lastUsedPageNum = logBook.StartPageNum;
                }

            }

            long pageIntervalStart = lastUsedPageNum + 1;
            long pageIntervalEnd = (long)page.PageNumber.Value;

            HashSet<decimal> alreadySubmittedPageNumbers = GetAlreadySubmittedTransportationPages(pageIntervalStart, pageIntervalEnd);

            for (long num = lastUsedPageNum + 1; num < page.PageNumber.Value; ++num)
            {
                if (!alreadySubmittedPageNumbers.Contains(num)) // The page is not submitted for any other transportation log book
                {
                    AddTransportationLogBookMissingPage(page.LogBookId.Value, num, logBookPermitLicenseId);
                }
            }

            TransportationLogBookPage entry = new TransportationLogBookPage
            {
                LogBookId = page.LogBookId.Value,
                LogBookPermitLicenseId = logBookPermitLicenseId,
                Status = nameof(LogBookPageStatusesEnum.Submitted),
                PageNum = page.PageNumber.Value,
                FluxIdentifier = Guid.NewGuid(),
                OriginDeclarationId = page.CommonData.OriginDeclarationId,
                DeliveryLocation = page.DeliveryLocation,
                ImportPlace = page.CommonData.PlaceOfImport,
                IsImportNotByShip = page.CommonData.IsImportNotByShip.Value,
                LoadingDate = page.LoadingDate.Value,
                LoadingLocation = page.LoadingLocation,
                VehicleNumber = page.VehicleIdentification
            };

            AddOrEditLogBookPageProducts(page.Products, LogBookTypesEnum.Transportation, transportationPage: entry);

            if (page.Files != null)
            {
                foreach (FileInfoDTO file in page.Files)
                {
                    Db.AddOrEditFile(entry, entry.TransportationLogBookPageFiles, file);
                }
            }

            if ((long)page.PageNumber.Value > logBook.LastPageNum)
            {
                logBook.LastPageNum = (long)page.PageNumber.Value;
            }

            Db.TransportationLogBookPages.Add(entry);
            Db.SaveChanges();

            if (salesDomainService.MustSendSalesReport(page.CommonData.ShipId))
            {
                FLUXSalesReportMessageType flux = salesReportMapper.MapTransportPageToSalesReport(entry.Id,
                                                                                                  ReportPurposeCodes.Original,
                                                                                                  entry.FluxIdentifier);
                salesDomainService.ReportSalesDocument(flux);
            }

            return entry.Id;
        }

        public void EditTransportationLogBookPage(TransportationLogBookPageEditDTO page)
        {
            TransportationLogBookPage dbEntry = (from transportationPage in Db.TransportationLogBookPages
                                                                              .Include(x => x.TransportationLogBookPageFiles)
                                                 where transportationPage.Id == page.Id
                                                 select transportationPage).First();

            dbEntry.Status = nameof(LogBookPageStatusesEnum.Submitted);
            dbEntry.DeliveryLocation = page.DeliveryLocation;
            dbEntry.ImportPlace = page.CommonData.PlaceOfImport;
            dbEntry.IsImportNotByShip = page.CommonData.IsImportNotByShip.Value;
            dbEntry.LoadingDate = page.LoadingDate.Value;
            dbEntry.LoadingLocation = page.LoadingLocation;
            dbEntry.VehicleNumber = page.VehicleIdentification;

            dbEntry.OriginDeclarationId = page.CommonData.OriginDeclarationId;

            AddOrEditLogBookPageProducts(page.Products, LogBookTypesEnum.Transportation, transportationPage: dbEntry);

            if (page.Files != null)
            {
                foreach (FileInfoDTO file in page.Files)
                {
                    Db.AddOrEditFile(dbEntry, dbEntry.TransportationLogBookPageFiles, file);
                }
            }

            Db.SaveChanges();

            if (salesDomainService.MustSendSalesReport(page.CommonData.ShipId))
            {
                FLUXSalesReportMessageType flux = salesReportMapper.MapTransportPageToSalesReport(dbEntry.Id,
                                                                                                  ReportPurposeCodes.Replace,
                                                                                                  dbEntry.FluxIdentifier);
                salesDomainService.ReportSalesDocument(flux);
            }
        }

        public SimpleAuditDTO GetTransportationLogBookPageSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(Db.TransportationLogBookPages, id);
        }

        public List<TransportationLogBookPageRegisterDTO> GetTransportationLogBookPagesAndDeclarations(int logBookId, int? permitLicenseId = null)
        {
            var query = from page in Db.TransportationLogBookPages
                        join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                        join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id into pageBuyer
                        from receiverBuyer in pageBuyer.DefaultIfEmpty()
                        join receiverBuyerLegal in Db.Legals on receiverBuyer.SubmittedForLegalId equals receiverBuyerLegal.Id into buyerLegal
                        from receiverBuyerLegal in buyerLegal.DefaultIfEmpty()
                        join person in Db.Persons on logBook.PersonId equals person.Id into pagePerson
                        from receiverPerson in pagePerson.DefaultIfEmpty()
                        join legal in Db.Legals on logBook.LegalId equals legal.Id into pageLegal
                        from receiverLegal in pageLegal.DefaultIfEmpty()
                        where page.LogBookId == logBookId
                        select new
                        {
                            Id = page.Id,
                            LogBookId = page.LogBookId,
                            PageNumber = page.PageNum,
                            LoadingDate = page.LoadingDate,
                            DeliveryLocation = page.DeliveryLocation,
                            VehicleNumber = page.VehicleNumber,
                            RecieverName = receiverBuyer != null && receiverBuyerLegal != null
                                           ? receiverBuyerLegal.Name
                                           : receiverPerson != null
                                               ? receiverPerson.FirstName + " " + receiverPerson.LastName
                                               : receiverLegal != null
                                                   ? receiverLegal.Name
                                                   : "",
                            StatusCode = page.Status,
                            CancellationReason = page.CancelationReason,
                            IsActive = page.IsActive
                        };

            if (permitLicenseId.HasValue)
            {
                query = from page in query
                        join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                        join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId
                        where logBookPermitLicense.PermitLicenseRegisterId == permitLicenseId
                        select page;
            }

            List<TransportationLogBookPageRegisterDTO> result = (from page in query
                                                                 select new TransportationLogBookPageRegisterDTO
                                                                 {
                                                                     Id = page.Id,
                                                                     LogBookId = page.LogBookId,
                                                                     PageNumber = page.PageNumber,
                                                                     LoadingDate = page.LoadingDate,
                                                                     DeliveryLocation = page.DeliveryLocation,
                                                                     VehicleNumber = page.VehicleNumber,
                                                                     RecieverName = page.RecieverName,
                                                                     Status = Enum.Parse<LogBookPageStatusesEnum>(page.StatusCode),
                                                                     CancellationReason = page.CancellationReason,
                                                                     IsActive = page.IsActive
                                                                 }).ToList();

            List<int> logBookPageIds = result.Select(x => x.Id).ToList();
            List<FishInformationDTO> fishInformation = GetTransportationLogBookPagesCatchInformation(logBookPageIds);
            ILookup<int, string> pageFishInformation = (from fish in fishInformation
                                                        select new
                                                        {
                                                            fish.LogBookPageId,
                                                            fish.FishData
                                                        }).ToLookup(x => x.LogBookPageId, y => y.FishData);

            foreach (TransportationLogBookPageRegisterDTO page in result)
            {
                if (pageFishInformation[page.Id].Any())
                {
                    page.ProductsInformation = string.Join(';', pageFishInformation[page.Id]);
                }
            }

            return result;
        }

        private List<TransportationLogBookPageRegisterDTO> GetTransportationLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                 CatchesAndSalesAdministrationFilters filters,
                                                                                                 List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<TransportationLogBookPageRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllTransportationLogBookPagesForTable(logBookIds, permittedLogBookTypes);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredTransportationLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes)
                    : GetParametersFilteredTransportationLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            }

            return result;
        }

        private List<TransportationLogBookPageRegisterDTO> GetTransportationLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                 CatchesAndSalesPublicFilters filters,
                                                                                                 List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<TransportationLogBookPageRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllTransportationLogBookPagesForTable(logBookIds, permittedLogBookTypes);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredTransportationLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes)
                    : GetParametersFilteredTransportationLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            }

            return result;
        }

        private List<TransportationLogBookPageRegisterDTO> GetAllTransportationLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                    List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<TransportationLogBookPageRegisterHelper> baseQuery = GetTransportationLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);
            List<TransportationLogBookPageRegisterDTO> filledPages = FinalizeTransportationLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<TransportationLogBookPageRegisterDTO> GetParametersFilteredTransportationLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                                   CatchesAndSalesAdministrationFilters filters,
                                                                                                                   List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<TransportationLogBookPageRegisterHelper> baseQuery = GetTransportationLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            if (filters.TerritoryUnitId.HasValue
                && filters.FilterTransportationLogBookTeritorryUnitId.HasValue
                && filters.FilterTransportationLogBookTeritorryUnitId.Value)
            {
                baseQuery = from filledPage in baseQuery
                            where filledPage.TerritoryUnitId == filters.TerritoryUnitId.Value
                            select filledPage;
            }
            else if (filters.TerritoryUnitId.HasValue
                     && (!filters.FilterFishLogBookTeritorryUnitId.HasValue || !filters.FilterFishLogBookTeritorryUnitId.Value)
                     && (!filters.FilterFirstSaleLogBookTeritorryUnitId.HasValue || !filters.FilterFirstSaleLogBookTeritorryUnitId.Value)
                     && (!filters.FilterAdmissionLogBookTeritorryUnitId.HasValue || !filters.FilterAdmissionLogBookTeritorryUnitId.Value)
                     && (!filters.FilterTransportationLogBookTeritorryUnitId.HasValue || !filters.FilterTransportationLogBookTeritorryUnitId.Value)
                     && (!filters.FilterAquacultureLogBookTeritorryUnitId.HasValue || !filters.FilterAquacultureLogBookTeritorryUnitId.Value))
            {
                baseQuery = from filledPage in baseQuery
                            where filledPage.TerritoryUnitId == filters.TerritoryUnitId.Value
                            select filledPage;
            }

            if (filters.PageNumber != null)
            {
                baseQuery = from filledPage in baseQuery
                            where filledPage.PageNumber == filters.PageNumber
                            select filledPage;
            }

            if (filters.LogBookTypeId != null)
            {
                baseQuery = from filledPage in baseQuery
                            join logBook in Db.LogBooks on filledPage.LogBookId equals logBook.Id
                            where logBook.LogBookTypeId == filters.LogBookTypeId
                            select filledPage;
            }

            if (!string.IsNullOrEmpty(filters.LogBookNumber))
            {
                baseQuery = from filledPage in baseQuery
                            join logBook in Db.LogBooks on filledPage.LogBookId equals logBook.Id
                            where logBook.LogNum.ToLower().Contains(filters.LogBookNumber.ToLower())
                            select filledPage;
            }

            if (filters.DocumentNumber.HasValue)
            {
                baseQuery = from filledPage in baseQuery
                            where filledPage.PageNumber == filters.DocumentNumber.Value
                            select filledPage;
            }

            List<TransportationLogBookPageRegisterDTO> filledPages = FinalizeTransportationLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<TransportationLogBookPageRegisterDTO> GetFreeTextFilteredTransportationLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                                 CatchesAndSalesAdministrationFilters filters,
                                                                                                                 List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<TransportationLogBookPageRegisterHelper> baseQuery = GetTransportationLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            if (filters.TerritoryUnitId.HasValue
                && filters.FilterTransportationLogBookTeritorryUnitId.HasValue
                && filters.FilterTransportationLogBookTeritorryUnitId.Value)
            {
                baseQuery = from filledPage in baseQuery
                            where filledPage.TerritoryUnitId == filters.TerritoryUnitId.Value
                            select filledPage;
            }

            // TODO

            List<TransportationLogBookPageRegisterDTO> filledPages = FinalizeTransportationLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<TransportationLogBookPageRegisterDTO> GetParametersFilteredTransportationLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                                  CatchesAndSalesPublicFilters filters,
                                                                                                                  List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<TransportationLogBookPageRegisterHelper> baseQuery = GetTransportationLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            //if (filters.PageNumber != null)
            //{
            //    baseQuery = from filledPage in baseQuery
            //                where filledPage.PageNumber == filters.PageNumber
            //                select filledPage;
            //}

            //if (filters.LogBookTypeId != null)
            //{
            //    baseQuery = from filledPage in baseQuery
            //                join logBook in Db.LogBooks on filledPage.LogBookId equals logBook.Id
            //                where logBook.LogBookTypeId == filters.LogBookTypeId
            //                select filledPage;
            //}

            //if (!string.IsNullOrEmpty(filters.LogBookNumber))
            //{
            //    baseQuery = from filledPage in baseQuery
            //                join logBook in Db.LogBooks on filledPage.LogBookId equals logBook.Id
            //                where logBook.LogNum.ToLower().Contains(filters.LogBookNumber.ToLower())
            //                select filledPage;
            //}

            //if (filters.DocumentNumber.HasValue)
            //{
            //    baseQuery = from filledPage in baseQuery
            //                where filledPage.PageNumber == filters.DocumentNumber.Value
            //                select filledPage;
            //}

            List<TransportationLogBookPageRegisterDTO> filledPages = FinalizeTransportationLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<TransportationLogBookPageRegisterDTO> GetFreeTextFilteredTransportationLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                                 CatchesAndSalesPublicFilters filters,
                                                                                                                 List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<TransportationLogBookPageRegisterHelper> baseQuery = GetTransportationLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            // TODO

            List<TransportationLogBookPageRegisterDTO> filledPages = FinalizeTransportationLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private IQueryable<TransportationLogBookPageRegisterHelper> GetTransportationLogBookPagesBaseQuery(IEnumerable<int> logBookIds,
                                                                                                           List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<string> permittedLogBookTypeStrings = permittedLogBookTypes.Select(x => x.ToString()).ToList();

            IQueryable<TransportationLogBookPageRegisterHelper> filledPages = from page in Db.TransportationLogBookPages
                                                                              join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                                                                              join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                                                              join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id into pageBuyer
                                                                              from receiverBuyer in pageBuyer.DefaultIfEmpty()
                                                                              join receiverBuyerLegal in Db.Legals on receiverBuyer.SubmittedForLegalId equals receiverBuyerLegal.Id into buyerLegal
                                                                              from receiverBuyerLegal in buyerLegal.DefaultIfEmpty()
                                                                              join person in Db.Persons on logBook.PersonId equals person.Id into pagePerson
                                                                              from receiverPerson in pagePerson.DefaultIfEmpty()
                                                                              join legal in Db.Legals on logBook.LegalId equals legal.Id into pageLegal
                                                                              from receiverLegal in pageLegal.DefaultIfEmpty()
                                                                              join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id into regBuyers
                                                                              from registeredBuyer in regBuyers.DefaultIfEmpty()
                                                                              join registeredBuyerAppl in Db.Applications on registeredBuyer.ApplicationId equals registeredBuyerAppl.Id into buyerAppl
                                                                              from registeredBuyerAppl in buyerAppl.DefaultIfEmpty()
                                                                              join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId into lbPermitLicense
                                                                              from logBookPermitLicense in lbPermitLicense.DefaultIfEmpty()
                                                                              join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id into pl
                                                                              from permitLicense in pl.DefaultIfEmpty()
                                                                              join applPermitLicense in Db.Applications on permitLicense.ApplicationId equals applPermitLicense.Id into applPl
                                                                              from applPermitLicense in applPl.DefaultIfEmpty()
                                                                              join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                                                                              where logBookIds.Contains(page.LogBookId)
                                                                                    && permittedLogBookTypeStrings.Contains(logBookType.Code)
                                                                              orderby page.PageNum descending
                                                                              select new TransportationLogBookPageRegisterHelper
                                                                              {
                                                                                  Id = page.Id,
                                                                                  LogBookId = page.LogBookId,
                                                                                  PageNumber = page.PageNum,
                                                                                  IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                                                                  TerritoryUnitId = registeredBuyerAppl != null
                                                                                                    ? registeredBuyerAppl.TerritoryUnitId.Value
                                                                                                    : applPermitLicense != null
                                                                                                        ? applPermitLicense.TerritoryUnitId.Value
                                                                                                        : -1,
                                                                                  LoadingDate = page.LoadingDate,
                                                                                  DeliveryLocation = page.DeliveryLocation,
                                                                                  VehicleNumber = page.VehicleNumber,
                                                                                  RecieverName = receiverBuyer != null && receiverBuyerLegal != null
                                                                                                 ? receiverBuyerLegal.Name
                                                                                                 : receiverPerson != null
                                                                                                    ? receiverPerson.FirstName + " " + receiverPerson.LastName
                                                                                                    : receiverLegal != null
                                                                                                        ? receiverLegal.Name
                                                                                                        : null,
                                                                                  StatusCode = page.Status,
                                                                                  CancellationReason = page.CancelationReason,
                                                                                  IsActive = page.IsActive
                                                                              };

            return filledPages;
        }

        private List<TransportationLogBookPageRegisterDTO> FinalizeTransportationLogBookPagesForTable(IQueryable<TransportationLogBookPageRegisterHelper> query)
        {
            List<TransportationLogBookPageRegisterDTO> filledPages = (from page in query
                                                                      group page by new
                                                                      {
                                                                          page.Id,
                                                                          page.LogBookId,
                                                                          page.PageNumber,
                                                                          page.IsLogBookFinished,
                                                                          page.LoadingDate,
                                                                          page.DeliveryLocation,
                                                                          page.VehicleNumber,
                                                                          page.RecieverName,
                                                                          page.StatusCode,
                                                                          page.CancellationReason,
                                                                          page.IsActive
                                                                      } into p
                                                                      select new TransportationLogBookPageRegisterDTO
                                                                      {
                                                                          Id = p.Key.Id,
                                                                          LogBookId = p.Key.LogBookId,
                                                                          PageNumber = p.Key.PageNumber,
                                                                          IsLogBookFinished = p.Key.IsLogBookFinished,
                                                                          LoadingDate = p.Key.LoadingDate,
                                                                          DeliveryLocation = p.Key.DeliveryLocation,
                                                                          VehicleNumber = p.Key.VehicleNumber,
                                                                          RecieverName = p.Key.RecieverName,
                                                                          Status = Enum.Parse<LogBookPageStatusesEnum>(p.Key.StatusCode),
                                                                          CancellationReason = p.Key.CancellationReason,
                                                                          IsActive = p.Key.IsActive
                                                                      }).ToList();

            List<int> pageIds = filledPages.Select(x => x.Id).ToList();
            HashSet<int> pagesWithOriginProducts = (from product in Db.LogBookPageProducts
                                                    join originProduct in Db.LogBookPageProducts on product.OriginProductId equals originProduct.Id
                                                    where (product.FirstSaleLogBookPageId.HasValue || product.AdmissionLogBookPageId.HasValue)
                                                          && originProduct.TransportationLogBookPageId.HasValue
                                                          && pageIds.Contains(originProduct.TransportationLogBookPageId.Value)
                                                    select originProduct.TransportationLogBookPageId.Value).ToHashSet();

            foreach (TransportationLogBookPageRegisterDTO page in filledPages)
            {
                page.ConsistsOriginProducts = pagesWithOriginProducts.Contains(page.Id);
            }

            return filledPages;
        }

        private List<FishInformationDTO> GetTransportationLogBookPagesCatchInformation(List<int> logBookPageIds)
        {
            List<FishInformationDTO> results = (from product in Db.LogBookPageProducts
                                                join fish in Db.Nfishes on product.FishId equals fish.Id
                                                where product.TransportationLogBookPageId.HasValue
                                                      && logBookPageIds.Contains(product.TransportationLogBookPageId.Value)
                                                      && product.IsActive
                                                select new FishInformationDTO
                                                {
                                                    LogBookPageId = product.TransportationLogBookPageId.Value,
                                                    FishData = product.QuantityKg + "kg " + fish.Name // TODO add quantities where fish is the same
                                                }).ToList();

            return results;
        }

        private List<LogBookPageProductDTO> GetTransportationPageProducts(int pageId)
        {
            IQueryable<LogBookPageProduct> productsQuery = from product in Db.LogBookPageProducts
                                                           where product.TransportationLogBookPageId == pageId
                                                           select product;

            List<LogBookPageProductDTO> products = GetProductsDTO(productsQuery);

            return products;
        }

        private HashSet<decimal> GetAlreadySubmittedTransportationPages(long startPage, long endPage)
        {
            HashSet<decimal> alreadySubmittedPageNumbers = (from transportationPage in Db.TransportationLogBookPages
                                                            join lb in Db.LogBooks on transportationPage.LogBookId equals lb.Id
                                                            where !lb.IsOnline
                                                                  && transportationPage.PageNum >= startPage
                                                                  && transportationPage.PageNum < endPage
                                                            select Convert.ToDecimal(transportationPage.PageNum)).ToHashSet();

            return alreadySubmittedPageNumbers;
        }

        private void AddTransportationLogBookMissingPage(int logBookId, decimal pageNum, int? logBookPermitLicenseId)
        {
            TransportationLogBookPage entry = new TransportationLogBookPage
            {
                LogBookId = logBookId,
                LogBookPermitLicenseId = logBookPermitLicenseId,
                PageNum = pageNum,
                Status = nameof(LogBookPageStatusesEnum.Missing)
            };

            Db.TransportationLogBookPages.Add(entry);
        }

        private void DeleteAnnulledTransportationLogBookPageProducts(int logBookPageId)
        {
            List<LogBookPageProduct> logBookPageProducts = (from lbPageProduct in Db.LogBookPageProducts
                                                            where lbPageProduct.TransportationLogBookPageId == logBookPageId
                                                                  && lbPageProduct.IsActive
                                                            select lbPageProduct).ToList();

            foreach (LogBookPageProduct logBookPageProduct in logBookPageProducts)
            {
                logBookPageProduct.IsActive = false;
            }
        }
    }

    internal class TransportationLogBookPageRegisterHelper : TransportationLogBookPageRegisterDTO
    {
        public int TerritoryUnitId { get; set; }

        public string StatusCode { get; set; }
    }
}
