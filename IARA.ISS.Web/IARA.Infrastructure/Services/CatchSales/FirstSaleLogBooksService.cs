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
    public partial class LogBooksService : Service, ILogBooksService
    {
        public FirstSaleLogBookPageEditDTO GetFirstSaleLogBookPage(int id)
        {
            var data = (from page in Db.FirstSaleLogBookPages
                        join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                        join originDeclaration in Db.OriginDeclarations on page.OriginDeclarationId equals originDeclaration.Id into od
                        from originDeclaration in od.DefaultIfEmpty()
                        join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id into sp
                        from shipPage in sp.DefaultIfEmpty()
                        join transportationDocument in Db.TransportationLogBookPages on page.TransportationDocumentId equals transportationDocument.Id into td
                        from transportationDocument in td.DefaultIfEmpty()
                        join admissionDocument in Db.AdmissionLogBookPages on page.AdmissionLogBookPageId equals admissionDocument.Id into ad
                        from admissionDocument in ad.DefaultIfEmpty()
                        where page.Id == id
                        select new
                        {
                            Page = new FirstSaleLogBookPageEditDTO
                            {
                                Id = page.Id,
                                PageNumber = page.PageNum,
                                LogBookId = page.LogBookId,
                                LogBookNumber = logBook.LogNum,
                                SaleDate = page.SaleDate,
                                SaleContractDate = page.SaleContractDate,
                                SaleContractNumber = page.SaleContractNumber,
                                SaleLocation = page.SaleLocation,
                                BuyerId = page.BuyerId,
                                Status = page.Status
                            },
                            OriginDeclarationNumber = originDeclaration != null && shipPage != null ? shipPage.PageNum : null,
                            TransportationDocumentNumber = transportationDocument != null ? transportationDocument.PageNum : default(decimal?),
                            AdmissionDocumentNumber = admissionDocument != null ? admissionDocument.PageNum : default(decimal?)
                        }).First();


            if (!string.IsNullOrEmpty(data.OriginDeclarationNumber))
            {
                data.Page.CommonData = GetCommonLogBookPageDataByOriginDeclarationNumber(data.OriginDeclarationNumber);
            }
            else if (data.TransportationDocumentNumber.HasValue) // Ако няма декларация за произход, то може да има номер на документ за превоз
            {
                data.Page.CommonData = GetCommonLogBookPageDataByTransportationDocumentNumber(data.TransportationDocumentNumber.Value);
            }
            else // Ако няма декларация за произход и документ за превоз, то със сигурност трябва да има номер на документ за приемане
            {
                data.Page.CommonData = GetCommonLogBookPageDataByAdmissionDocumentNumber(data.AdmissionDocumentNumber.Value);
            }

            data.Page.ProductsTotalPrice = (from product in Db.LogBookPageProducts
                                            where product.FirstSaleLogBookPageId == id
                                            select product.UnitPrice * product.QuantityKg).Sum().ToString();

            data.Page.Products = GetFirstSalePageProducts(id);

            if (data.Page.CommonData.OriginDeclarationId.HasValue)
            {
                data.Page.OriginalPossibleProducts = GetNewProductsByOriginDeclarationId(data.Page.CommonData.OriginDeclarationId.Value);
            }
            else if (data.Page.CommonData.TransportationDocumentId.HasValue) // ако няма декларация за произход, то може да има документ за транспорт, към който да има собствени продукти
            {
                data.Page.OriginalPossibleProducts = GetNewProductsByTransportationDocument(data.Page.CommonData.TransportationDocumentId.Value);
            }
            else // ако няма декларация за произход и документ за превоз, задължително трябва да има документ за приемане, към който да има собствени продукти
            {
                data.Page.OriginalPossibleProducts = GetNewProductsByAdmissionDocument(data.Page.CommonData.AdmissionDocumentId.Value);
            }

            data.Page.Files = Db.GetFiles(Db.FirstSaleLogBookPageFiles, id);

            return data.Page;
        }

        public FirstSaleLogBookPageEditDTO GetNewFirstSaleLogBookPage(int logBookId,
                                                                      int? originDeclarationId,
                                                                      int? transportationDocumentId,
                                                                      int? admissionDocumentId)
        {
            FirstSaleLogBookPageEditDTO firstSalePage = (from logBook in Db.LogBooks
                                                         join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id
                                                         join legal in Db.Legals on buyer.SubmittedForLegalId equals legal.Id into l
                                                         from legal in l.DefaultIfEmpty()
                                                         join person in Db.Persons on buyer.SubmittedForPersonId equals person.Id into p
                                                         from person in p.DefaultIfEmpty()
                                                         where logBook.Id == logBookId
                                                         select new FirstSaleLogBookPageEditDTO
                                                         {
                                                             LogBookId = logBook.Id,
                                                             LogBookNumber = logBook.LogNum,
                                                             BuyerId = logBook.RegisteredBuyerId,
                                                             BuyerName = legal != null
                                                                        ? $"{legal.Name} ({buyer.RegistrationNum})"
                                                                        : $"{person.FirstName} {person.LastName} ({buyer.RegistrationNum})"
                                                         }).First();

            if (originDeclarationId.HasValue)
            {
                firstSalePage.Products = GetNewProductsByOriginDeclarationId(originDeclarationId.Value);
            }
            else if (transportationDocumentId.HasValue)
            {
                firstSalePage.Products = GetNewProductsByTransportationDocument(transportationDocumentId.Value);
            }
            else
            {
                firstSalePage.Products = GetNewProductsByAdmissionDocument(admissionDocumentId.Value);
            }
            // TODO else throw ??? (if the number is wrong?);

            firstSalePage.OriginalPossibleProducts = firstSalePage.Products;

            return firstSalePage;
        }

        public int AddFirstSaleLogBookPage(FirstSaleLogBookPageEditDTO page)
        {
            OldLogBookPageStatus oldPage = GetNewLogBookPageStatusAndCheckValidity(page.PageNumber.Value, page.LogBookId.Value, Db.FirstSaleLogBookPages);

            if (oldPage != null && oldPage.Status == LogBookPageStatusesEnum.Missing)
            {
                page.Id = oldPage.Id;
                EditFirstSaleLogBookPage(page);

                return page.Id.Value;
            }

            LogBook logBook = (from book in Db.LogBooks
                               where book.Id == page.LogBookId.Value
                               select book).First();

            long lastUsedPageNum;

            if (logBook.LastPageNum != default)
            {
                lastUsedPageNum = logBook.LastPageNum;
            }
            else
            {
                lastUsedPageNum = logBook.StartPageNum - 1;
            }

            long pageIntervalStart = lastUsedPageNum + 1;
            long pageIntervalEnd = (long)page.PageNumber.Value;

            HashSet<decimal> alreadySubmittedPageNumbers = GetAlreadySubmittedFirstSalePages(pageIntervalStart, pageIntervalEnd);

            for (long num = lastUsedPageNum + 1; num < page.PageNumber.Value; ++num)
            {
                if (!alreadySubmittedPageNumbers.Contains(num)) // The page is not submitted for any other first sale log book
                {
                    AddFirstSaleLogBookMissingPage(page.LogBookId.Value, num);
                }
            }

            FirstSaleLogBookPage entry = new FirstSaleLogBookPage
            {
                PageNum = page.PageNumber.Value,
                FluxIdentifier = Guid.NewGuid(),
                BuyerId = page.BuyerId.Value,
                LogBookId = page.LogBookId.Value,
                Status = nameof(LogBookPageStatusesEnum.Submitted),
                OriginDeclarationId = page.CommonData.OriginDeclarationId,
                TransportationDocumentId = page.CommonData.TransportationDocumentId,
                AdmissionLogBookPageId = page.CommonData.AdmissionDocumentId,
                SaleContractDate = page.SaleContractDate,
                SaleContractNumber = page.SaleContractNumber,
                SaleDate = page.SaleDate.Value,
                SaleLocation = page.SaleLocation
            };

            AddOrEditLogBookPageProducts(page.Products, LogBookTypesEnum.FirstSale, firstSalePage: entry);

            if (page.Files != null)
            {
                foreach (FileInfoDTO file in page.Files)
                {
                    Db.AddOrEditFile(entry, entry.FirstSaleLogBookPageFiles, file);
                }
            }

            if ((long)page.PageNumber.Value > logBook.LastPageNum)
            {
                logBook.LastPageNum = (long)page.PageNumber.Value;
            }

            Db.FirstSaleLogBookPages.Add(entry);
            Db.SaveChanges();

            if (salesDomainService.MustSendSalesReport(page.CommonData.ShipId))
            {
                FLUXSalesReportMessageType flux = salesReportMapper.MapFirstSalePageToSalesReport(entry.Id,
                                                                                                  ReportPurposeCodes.Original,
                                                                                                  entry.FluxIdentifier);
                salesDomainService.ReportSalesDocument(flux);
            }

            return entry.Id;
        }

        public void EditFirstSaleLogBookPage(FirstSaleLogBookPageEditDTO page)
        {
            FirstSaleLogBookPage dbEntry = (from firstSalePage in Db.FirstSaleLogBookPages.Include(x => x.FirstSaleLogBookPageFiles)
                                            where firstSalePage.Id == page.Id
                                            select firstSalePage).First();

            dbEntry.Status = nameof(LogBookPageStatusesEnum.Submitted);
            dbEntry.BuyerId = page.BuyerId.Value;
            dbEntry.SaleContractDate = page.SaleContractDate;
            dbEntry.SaleContractNumber = page.SaleContractNumber;
            dbEntry.SaleDate = page.SaleDate.Value;
            dbEntry.SaleLocation = page.SaleLocation;

            dbEntry.OriginDeclarationId = page.CommonData.OriginDeclarationId;
            dbEntry.TransportationDocumentId = page.CommonData.TransportationDocumentId;
            dbEntry.AdmissionLogBookPageId = page.CommonData.AdmissionDocumentId;

            AddOrEditLogBookPageProducts(page.Products, LogBookTypesEnum.FirstSale, firstSalePage: dbEntry);

            if (page.Files != null)
            {
                foreach (FileInfoDTO file in page.Files)
                {
                    Db.AddOrEditFile(dbEntry, dbEntry.FirstSaleLogBookPageFiles, file);
                }
            }

            Db.SaveChanges();

            if (salesDomainService.MustSendSalesReport(page.CommonData.ShipId))
            {
                FLUXSalesReportMessageType flux = salesReportMapper.MapFirstSalePageToSalesReport(dbEntry.Id,
                                                                                                  ReportPurposeCodes.Replace,
                                                                                                  dbEntry.FluxIdentifier);
                salesDomainService.ReportSalesDocument(flux);
            }
        }

        public SimpleAuditDTO GetFirstSaleLogBookPageSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(Db.FirstSaleLogBookPages, id);
        }

        private List<FirstSaleLogBookPageRegisterDTO> GetFirstSalePages(int logBookId)
        {
            List<FirstSaleLogBookPageRegisterDTO> results = (from page in Db.FirstSaleLogBookPages
                                                             join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                                                             join registeredBuyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals registeredBuyer.Id
                                                             join registeredBuyerAppl in Db.Applications on registeredBuyer.ApplicationId equals registeredBuyerAppl.Id
                                                             join legal in Db.Legals on registeredBuyer.SubmittedForLegalId equals legal.Id into subL
                                                             from legal in subL.DefaultIfEmpty()
                                                             join person in Db.Persons on registeredBuyer.SubmittedForPersonId equals person.Id into subP
                                                             from person in subP.DefaultIfEmpty()
                                                             where page.LogBookId == logBookId
                                                             select new FirstSaleLogBookPageRegisterDTO
                                                             {
                                                                 Id = page.Id,
                                                                 PageNumber = page.PageNum,
                                                                 LogBookId = page.LogBookId,
                                                                 SaleDate = page.SaleDate,
                                                                 SaleLocation = page.SaleLocation,
                                                                 BuyerNames = legal != null
                                                                              ? legal.Name + " (" + legal.Eik + ") - " + registeredBuyer.RegistrationNum
                                                                              : person != null
                                                                                  ? person.FirstName + " " + person.LastName + " - " + registeredBuyer.RegistrationNum
                                                                                  : "",
                                                                 Status = Enum.Parse<LogBookPageStatusesEnum>(page.Status),
                                                                 CancellationReason = page.CancelationReason
                                                             }).ToList();

            List<int> logBookPageIds = results.Select(x => x.Id).ToList();
            List<FishInformationDTO> fishInformation = GetFirstSaleLogBookPagesCatchInformation(logBookPageIds);
            ILookup<int, string> pageFishInformation = (from fish in fishInformation
                                                        select new
                                                        {
                                                            fish.LogBookPageId,
                                                            fish.FishData
                                                        }).ToLookup(x => x.LogBookPageId, y => y.FishData);

            foreach (FirstSaleLogBookPageRegisterDTO page in results)
            {
                if (pageFishInformation[page.Id].Any())
                {
                    page.ProductsInformation = string.Join(';', pageFishInformation[page.Id]);
                }
            }

            return results;
        }

        private List<FirstSaleLogBookPageRegisterDTO> GetFirstSaleLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                       CatchesAndSalesAdministrationFilters filters,
                                                                                       List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<FirstSaleLogBookPageRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllFirstSaleLogBookPagesForTable(logBookIds, permittedLogBookTypes);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredFirstSaleLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes)
                    : GetParametersFilteredFirstSaleLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            }

            return result;
        }

        private List<FirstSaleLogBookPageRegisterDTO> GetFirstSaleLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                       CatchesAndSalesPublicFilters filters,
                                                                                       List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<FirstSaleLogBookPageRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllFirstSaleLogBookPagesForTable(logBookIds, permittedLogBookTypes);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredFirstSaleLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes)
                    : GetParametersFilteredFirstSaleLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            }

            return result;
        }

        private List<FirstSaleLogBookPageRegisterDTO> GetAllFirstSaleLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                          List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<FirstSaleLogBookPageRegisterHelper> baseQuery = GetAllFirstSaleLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);
            List<FirstSaleLogBookPageRegisterDTO> filledPages = FinalizeFirstSaleLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<FirstSaleLogBookPageRegisterDTO> GetParametersFilteredFirstSaleLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                         CatchesAndSalesAdministrationFilters filters,
                                                                                                         List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<FirstSaleLogBookPageRegisterHelper> baseQuery = GetAllFirstSaleLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            if (filters.TerritoryUnitId.HasValue && filters.FilterFirstSaleLogBookTeritorryUnitId.HasValue && filters.FilterFirstSaleLogBookTeritorryUnitId.Value)
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

            List<FirstSaleLogBookPageRegisterDTO> filledPages = FinalizeFirstSaleLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<FirstSaleLogBookPageRegisterDTO> GetFreeTextFilteredFirstSaleLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                       CatchesAndSalesPublicFilters filters,
                                                                                                       List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<FirstSaleLogBookPageRegisterHelper> baseQuery = GetAllFirstSaleLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            // TODO
            List<FirstSaleLogBookPageRegisterDTO> filledPages = FinalizeFirstSaleLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<FirstSaleLogBookPageRegisterDTO> GetParametersFilteredFirstSaleLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                         CatchesAndSalesPublicFilters filters,
                                                                                                         List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<FirstSaleLogBookPageRegisterHelper> baseQuery = GetAllFirstSaleLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

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

            List<FirstSaleLogBookPageRegisterDTO> filledPages = FinalizeFirstSaleLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<FirstSaleLogBookPageRegisterDTO> GetFreeTextFilteredFirstSaleLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                       CatchesAndSalesAdministrationFilters filters,
                                                                                                       List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<FirstSaleLogBookPageRegisterHelper> baseQuery = GetAllFirstSaleLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            if (filters.TerritoryUnitId.HasValue && filters.FilterFirstSaleLogBookTeritorryUnitId.HasValue && filters.FilterFirstSaleLogBookTeritorryUnitId.Value)
            {
                baseQuery = from filledPage in baseQuery
                            where filledPage.TerritoryUnitId == filters.TerritoryUnitId.Value
                            select filledPage;
            }

            // TODO
            List<FirstSaleLogBookPageRegisterDTO> filledPages = FinalizeFirstSaleLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private IQueryable<FirstSaleLogBookPageRegisterHelper> GetAllFirstSaleLogBookPagesBaseQuery(IEnumerable<int> logBookIds,
                                                                                                    List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<string> permittedLogBookTypeStrings = permittedLogBookTypes.Select(x => x.ToString()).ToList();

            IQueryable<FirstSaleLogBookPageRegisterHelper> filledPages = from page in Db.FirstSaleLogBookPages
                                                                         join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                                                                         join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                                                         join registeredBuyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals registeredBuyer.Id
                                                                         join registeredBuyerAppl in Db.Applications on registeredBuyer.ApplicationId equals registeredBuyerAppl.Id
                                                                         join legal in Db.Legals on registeredBuyer.SubmittedForLegalId equals legal.Id into subL
                                                                         from legal in subL.DefaultIfEmpty()
                                                                         join person in Db.Persons on registeredBuyer.SubmittedForPersonId equals person.Id into subP
                                                                         from person in subP.DefaultIfEmpty()
                                                                         join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                                                                         where logBookIds.Contains(page.LogBookId)
                                                                               && permittedLogBookTypeStrings.Contains(logBookType.Code)
                                                                         orderby page.PageNum descending
                                                                         select new FirstSaleLogBookPageRegisterHelper
                                                                         {
                                                                             Id = page.Id,
                                                                             PageNumber = page.PageNum,
                                                                             LogBookId = page.LogBookId,
                                                                             IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                                                             TerritoryUnitId = registeredBuyerAppl.TerritoryUnitId.Value,
                                                                             SaleDate = page.SaleDate,
                                                                             SaleLocation = page.SaleLocation,
                                                                             BuyerNames = legal != null
                                                                                          ? legal.Name + " (" + legal.Eik + ") - " + registeredBuyer.RegistrationNum
                                                                                          : person != null
                                                                                            ? person.FirstName + " " + person.LastName + " - " + registeredBuyer.RegistrationNum
                                                                                            : null,
                                                                             Status = Enum.Parse<LogBookPageStatusesEnum>(page.Status),
                                                                             CancellationReason = page.CancelationReason,
                                                                             IsActive = page.IsActive
                                                                         };

            return filledPages;
        }

        private List<FirstSaleLogBookPageRegisterDTO> FinalizeFirstSaleLogBookPagesForTable(IQueryable<FirstSaleLogBookPageRegisterHelper> query)
        {
            List<FirstSaleLogBookPageRegisterDTO> filledPages = (from page in query
                                                                 select new FirstSaleLogBookPageRegisterDTO
                                                                 {
                                                                     Id = page.Id,
                                                                     PageNumber = page.PageNumber,
                                                                     LogBookId = page.LogBookId,
                                                                     IsLogBookFinished = page.IsLogBookFinished,
                                                                     SaleDate = page.SaleDate,
                                                                     SaleLocation = page.SaleLocation,
                                                                     BuyerNames = page.BuyerNames,
                                                                     Status = page.Status,
                                                                     CancellationReason = page.CancellationReason,
                                                                     IsActive = page.IsActive
                                                                 }).ToList();

            return filledPages;
        }

        private List<FishInformationDTO> GetFirstSaleLogBookPagesCatchInformation(List<int> logBookPageIds)
        {
            List<FishInformationDTO> results = (from product in Db.LogBookPageProducts
                                                join fish in Db.Nfishes on product.FishId equals fish.Id
                                                where product.FirstSaleLogBookPageId.HasValue
                                                      && logBookPageIds.Contains(product.FirstSaleLogBookPageId.Value)
                                                      && product.IsActive
                                                select new FishInformationDTO
                                                {
                                                    LogBookPageId = product.FirstSaleLogBookPageId.Value,
                                                    FishData = product.QuantityKg + "kg " + fish.Name // TODO add quantities where fish is the same
                                                }).ToList();

            return results;
        }

        private List<LogBookPageProductDTO> GetFirstSalePageProducts(int pageId)
        {
            IQueryable<LogBookPageProduct> productsQuery = from product in Db.LogBookPageProducts
                                                           where product.FirstSaleLogBookPageId == pageId
                                                           select product;

            List<LogBookPageProductDTO> products = GetProductsDTO(productsQuery);

            return products;
        }

        private HashSet<decimal> GetAlreadySubmittedFirstSalePages(long startPage, long endPage)
        {
            HashSet<decimal> alreadySubmittedPageNumbers = (from firstSalePage in Db.FirstSaleLogBookPages
                                                            join lb in Db.LogBooks on firstSalePage.LogBookId equals lb.Id
                                                            where !lb.IsOnline
                                                                  && Convert.ToDecimal(firstSalePage.PageNum) >= startPage
                                                                  && Convert.ToDecimal(firstSalePage.PageNum) < endPage
                                                            select Convert.ToDecimal(firstSalePage.PageNum)).ToHashSet();

            return alreadySubmittedPageNumbers;
        }

        private void AddFirstSaleLogBookMissingPage(int logBookId, decimal pageNum)
        {
            FirstSaleLogBookPage entry = new FirstSaleLogBookPage
            {
                LogBookId = logBookId,
                PageNum = pageNum,
                Status = nameof(LogBookPageStatusesEnum.Missing)
            };

            Db.FirstSaleLogBookPages.Add(entry);
        }

        private void DeleteAnnulledFirstSalePageProducts(int logBookPageId)
        {
            List<LogBookPageProduct> logBookPageProducts = (from lbPageProduct in Db.LogBookPageProducts
                                                            where lbPageProduct.FirstSaleLogBookPageId == logBookPageId
                                                                  && lbPageProduct.IsActive
                                                            select lbPageProduct).ToList();

            foreach (LogBookPageProduct logBookPageProduct in logBookPageProducts)
            {
                logBookPageProduct.IsActive = false;
            }
        }
    }

    internal class FirstSaleLogBookPageRegisterHelper : FirstSaleLogBookPageRegisterDTO
    {
        public int TerritoryUnitId { get; set; }
    }
}
