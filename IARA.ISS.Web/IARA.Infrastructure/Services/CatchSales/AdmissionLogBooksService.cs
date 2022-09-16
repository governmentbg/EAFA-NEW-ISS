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
        public AdmissionLogBookPageEditDTO GetAdmissionLogBookPage(int id)
        {
            var data = (from page in Db.AdmissionLogBookPages
                        join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                        join originDeclaration in Db.OriginDeclarations on page.OriginDeclarationId equals originDeclaration.Id into od
                        from originDeclaration in od.DefaultIfEmpty()
                        join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id into sp
                        from shipPage in sp.DefaultIfEmpty()
                        join transportationDocument in Db.TransportationLogBookPages on page.TransportationDocumentId equals transportationDocument.Id into td
                        from transportationDocument in td.DefaultIfEmpty()
                        where page.Id == id
                        select new
                        {
                            Page = new AdmissionLogBookPageEditDTO
                            {
                                Id = page.Id,
                                PageNumber = page.PageNum,
                                LogBookId = page.LogBookId,
                                HandoverDate = page.HandoverDate,
                                StorageLocation = page.StorageLocation,
                                Status = page.Status
                            },
                            OriginDeclarationNumber = originDeclaration != null && shipPage != null ? shipPage.PageNum : null,
                            TransportationDocumentNumber = transportationDocument != null ? transportationDocument.PageNum : default(decimal?),
                            AcceptingPersonType = Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType),
                            AcceptingPersonId = logBook.PersonId,
                            AcceptingBuyerId = logBook.RegisteredBuyerId,
                            AcceptingLegalId = logBook.LegalId
                        }).First();

            if (!string.IsNullOrEmpty(data.OriginDeclarationNumber))
            {
                data.Page.CommonData = GetCommonLogBookPageDataByOriginDeclarationNumber(data.OriginDeclarationNumber);
            }
            else // Ако няма декларация за произход, то със сигурност трябва да има номер на документ за превоз
            {
                data.Page.CommonData = GetCommonLogBookPageDataByTransportationDocumentNumber(data.TransportationDocumentNumber.Value);
            }

            data.Page.AcceptingPerson = GetLogBookPagePerson(data.AcceptingPersonType,
                                                             data.AcceptingBuyerId,
                                                             data.AcceptingPersonId,
                                                             data.AcceptingLegalId);

            data.Page.Products = GetAdmissionPageProducts(id);

            if (data.Page.CommonData.OriginDeclarationId.HasValue)
            {
                data.Page.OriginalPossibleProducts = GetNewProductsByOriginDeclarationId(data.Page.CommonData.OriginDeclarationId.Value);
            }
            else // ако няма декларация за произход, задължително трябва да има документ за транспорт, към който да има собствени продукти
            {
                data.Page.OriginalPossibleProducts = GetNewProductsByTransportationDocument(data.Page.CommonData.TransportationDocumentId.Value);
            }

            data.Page.Files = Db.GetFiles(Db.AdmissionLogBookPageFiles, id);

            return data.Page;
        }

        public AdmissionLogBookPageEditDTO GetNewAdmissionLogBookPage(int logBookId, int? originDeclarationId, int? transportationDocumentId)
        {
            var admissionPageData = (from logBook in Db.LogBooks
                                     where logBook.Id == logBookId
                                     select new
                                     {
                                         LogBookId = logBook.Id,
                                         AcceptingPersonType = Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType),
                                         AcceptingPersonId = logBook.PersonId,
                                         AcceptingBuyerId = logBook.RegisteredBuyerId,
                                         AcceptingLegalId = logBook.LegalId
                                     }).First();

            AdmissionLogBookPageEditDTO admissionPage = new AdmissionLogBookPageEditDTO
            {
                LogBookId = admissionPageData.LogBookId
            };

            admissionPage.AcceptingPerson = GetLogBookPagePerson(admissionPageData.AcceptingPersonType,
                                                                 admissionPageData.AcceptingBuyerId,
                                                                 admissionPageData.AcceptingPersonId,
                                                                 admissionPageData.AcceptingLegalId);

            if (originDeclarationId.HasValue)
            {
                admissionPage.Products = GetNewProductsByOriginDeclarationId(originDeclarationId.Value);
            }
            else
            {
                admissionPage.Products = GetNewProductsByTransportationDocument(transportationDocumentId.Value);
            }

            admissionPage.OriginalPossibleProducts = admissionPage.Products;

            return admissionPage;
        }

        public int AddAdmissionLogBookPage(AdmissionLogBookPageEditDTO page)
        {
            OldLogBookPageStatus oldPage = GetNewLogBookPageStatusAndCheckValidity(page.PageNumber.Value, page.LogBookId.Value, Db.AdmissionLogBookPages);

            if (oldPage != null && oldPage.Status == LogBookPageStatusesEnum.Missing)
            {
                page.Id = oldPage.Id;
                EditAdmissionLogBookPage(page);

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

            HashSet<decimal> alreadySubmittedPageNumbers = GetAlreadySubmittedAdmissionPages(pageIntervalStart, pageIntervalEnd);

            for (long num = lastUsedPageNum + 1; num < page.PageNumber.Value; ++num)
            {
                if (!alreadySubmittedPageNumbers.Contains(num)) // The page is not submitted for any other admission log book
                {
                    AddAdmissionLogBookMissingPage(page.LogBookId.Value, num, logBookPermitLicenseId);
                }
            }

            AdmissionLogBookPage entry = new AdmissionLogBookPage
            {
                LogBookId = page.LogBookId.Value,
                LogBookPermitLicenseId = logBookPermitLicenseId,
                Status = nameof(LogBookPageStatusesEnum.Submitted),
                PageNum = page.PageNumber.Value,
                FluxIdentifier = Guid.NewGuid(),
                OriginDeclarationId = page.CommonData.OriginDeclarationId,
                TransportationDocumentId = page.CommonData.TransportationDocumentId,
                HandoverDate = page.HandoverDate.Value,
                StorageLocation = page.StorageLocation
            };

            AddOrEditLogBookPageProducts(page.Products, LogBookTypesEnum.Admission, admissionPage: entry);

            if (page.Files != null)
            {
                foreach (FileInfoDTO file in page.Files)
                {
                    Db.AddOrEditFile(entry, entry.AdmissionLogBookPageFiles, file);
                }
            }

            if ((long)page.PageNumber.Value > logBook.LastPageNum)
            {
                logBook.LastPageNum = (long)page.PageNumber.Value;
            }

            Db.AdmissionLogBookPages.Add(entry);
            Db.SaveChanges();

            if (salesDomainService.MustSendSalesReport(page.CommonData.ShipId))
            {
                FLUXSalesReportMessageType flux = salesReportMapper.MapAdmissionPageToSalesReport(entry.Id,
                                                                                                  ReportPurposeCodes.Original,
                                                                                                  entry.FluxIdentifier);
                salesDomainService.ReportSalesDocument(flux);
            }

            return entry.Id;
        }

        public void EditAdmissionLogBookPage(AdmissionLogBookPageEditDTO page)
        {
            AdmissionLogBookPage dbEntry = (from admissionPage in Db.AdmissionLogBookPages
                                                .Include(x => x.AdmissionLogBookPageFiles)
                                            where admissionPage.Id == page.Id
                                            select admissionPage).First();

            dbEntry.Status = nameof(LogBookPageStatusesEnum.Submitted);
            dbEntry.HandoverDate = page.HandoverDate.Value;
            dbEntry.StorageLocation = page.StorageLocation;

            dbEntry.OriginDeclarationId = page.CommonData.OriginDeclarationId;
            dbEntry.TransportationDocumentId = page.CommonData.TransportationDocumentId;

            AddOrEditLogBookPageProducts(page.Products, LogBookTypesEnum.Admission, admissionPage: dbEntry);

            if (page.Files != null)
            {
                foreach (FileInfoDTO file in page.Files)
                {
                    Db.AddOrEditFile(dbEntry, dbEntry.AdmissionLogBookPageFiles, file);
                }
            }

            Db.SaveChanges();

            if (salesDomainService.MustSendSalesReport(page.CommonData.ShipId))
            {
                FLUXSalesReportMessageType flux = salesReportMapper.MapAdmissionPageToSalesReport(dbEntry.Id,
                                                                                                  ReportPurposeCodes.Replace,
                                                                                                  dbEntry.FluxIdentifier);
                salesDomainService.ReportSalesDocument(flux);
            }
        }

        public SimpleAuditDTO GetAdmissionLogBookPageSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(Db.AdmissionLogBookPages, id);
        }

        public List<AdmissionLogBookPageRegisterDTO> GetAdmissionLogBookPagesAndDeclarations(int logBookId, int? permitLicenseId = null)
        {
            var query = from page in Db.AdmissionLogBookPages
                        join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                        join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id into pageBuyer
                        from acceptingBuyer in pageBuyer.DefaultIfEmpty()
                        join acceptingBuyerLegal in Db.Legals on acceptingBuyer.SubmittedForLegalId equals acceptingBuyerLegal.Id into buyerLegal
                        from acceptingBuyerLegal in buyerLegal.DefaultIfEmpty()
                        join acceptingBuyerPerson in Db.Persons on acceptingBuyer.SubmittedForPersonId equals acceptingBuyerPerson.Id into buyerPerson
                        from acceptingBuyerPerson in buyerPerson.DefaultIfEmpty()
                        join person in Db.Persons on logBook.PersonId equals person.Id into pagePerson
                        from acceptingPerson in pagePerson.DefaultIfEmpty()
                        join legal in Db.Legals on logBook.LegalId equals legal.Id into pageLegal
                        from acceptingLegal in pageLegal.DefaultIfEmpty()
                        where page.LogBookId == logBookId
                        select new
                        {
                            Id = page.Id,
                            PageNumber = page.PageNum,
                            LogBookId = page.LogBookId,
                            HandoverDate = page.HandoverDate,
                            StorageLocation = page.StorageLocation,
                            AcceptedPersonNames = acceptingBuyer != null && acceptingBuyerLegal != null
                                                  ? acceptingBuyerLegal.Name
                                                  : acceptingBuyerPerson != null
                                                    ? acceptingBuyerPerson.FirstName + " " + acceptingBuyerPerson.LastName
                                                       : acceptingPerson != null
                                                            ? acceptingPerson.FirstName + " " + acceptingPerson.LastName
                                                            : acceptingLegal != null
                                                                ? acceptingLegal.Name
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

            List<AdmissionLogBookPageRegisterDTO> results = (from page in query
                                                             select new AdmissionLogBookPageRegisterDTO
                                                             {
                                                                 Id = page.Id,
                                                                 PageNumber = page.PageNumber,
                                                                 LogBookId = page.LogBookId,
                                                                 HandoverDate = page.HandoverDate,
                                                                 StorageLocation = page.StorageLocation,
                                                                 AcceptedPersonNames = page.AcceptedPersonNames,
                                                                 Status = Enum.Parse<LogBookPageStatusesEnum>(page.StatusCode),
                                                                 CancellationReason = page.CancellationReason,
                                                                 IsActive = page.IsActive
                                                             }).ToList();

            List<int> logBookPageIds = results.Select(x => x.Id).ToList();
            List<FishInformationDTO> fishInformation = GetAdmissionLogBookPagesCatchInformation(logBookPageIds);
            ILookup<int, string> pageFishInformation = (from fish in fishInformation
                                                        select new
                                                        {
                                                            fish.LogBookPageId,
                                                            fish.FishData
                                                        }).ToLookup(x => x.LogBookPageId, y => y.FishData);

            foreach (AdmissionLogBookPageRegisterDTO page in results)
            {
                if (pageFishInformation[page.Id].Any())
                {
                    page.ProductsInformation = string.Join(';', pageFishInformation[page.Id]);
                }
            }

            return results;
        }

        private List<AdmissionLogBookPageRegisterDTO> GetAdmissionLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                       CatchesAndSalesAdministrationFilters filters,
                                                                                       List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<AdmissionLogBookPageRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllAdmissionLogBookPagesForTable(logBookIds, permittedLogBookTypes);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredAdmissionLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes)
                    : GetParametersFilteredAdmissionLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            }

            return result;
        }

        private List<AdmissionLogBookPageRegisterDTO> GetAdmissionLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                       CatchesAndSalesPublicFilters filters,
                                                                                       List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<AdmissionLogBookPageRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllAdmissionLogBookPagesForTable(logBookIds, permittedLogBookTypes);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredAdmissionLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes)
                    : GetParametersFilteredAdmissionLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            }

            return result;
        }

        private List<AdmissionLogBookPageRegisterDTO> GetAllAdmissionLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                          List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<AdmissionLogBookPageRegisterHelper> baseQuery = GetAllAdmissionFilteredLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);
            List<AdmissionLogBookPageRegisterDTO> filledPages = FinalizeAdmissionLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<AdmissionLogBookPageRegisterDTO> GetParametersFilteredAdmissionLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                         CatchesAndSalesAdministrationFilters filters,
                                                                                                         List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<AdmissionLogBookPageRegisterHelper> baseQuery = GetAllAdmissionFilteredLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            if (filters.TerritoryUnitId.HasValue && filters.FilterAdmissionLogBookTeritorryUnitId.HasValue && filters.FilterAdmissionLogBookTeritorryUnitId.Value)
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

            List<AdmissionLogBookPageRegisterDTO> filledPages = FinalizeAdmissionLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<AdmissionLogBookPageRegisterDTO> GetFreeTextFilteredAdmissionLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                       CatchesAndSalesAdministrationFilters filters,
                                                                                                       List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<AdmissionLogBookPageRegisterHelper> baseQuery = GetAllAdmissionFilteredLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            if (filters.TerritoryUnitId.HasValue && filters.FilterAdmissionLogBookTeritorryUnitId.HasValue && filters.FilterAdmissionLogBookTeritorryUnitId.Value)
            {
                baseQuery = from filledPage in baseQuery
                            where filledPage.TerritoryUnitId == filters.TerritoryUnitId.Value
                            select filledPage;
            }

            // TODO
            List<AdmissionLogBookPageRegisterDTO> filledPages = FinalizeAdmissionLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<AdmissionLogBookPageRegisterDTO> GetParametersFilteredAdmissionLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                         CatchesAndSalesPublicFilters filters,
                                                                                                         List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<AdmissionLogBookPageRegisterHelper> baseQuery = GetAllAdmissionFilteredLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

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

            List<AdmissionLogBookPageRegisterDTO> filledPages = FinalizeAdmissionLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<AdmissionLogBookPageRegisterDTO> GetFreeTextFilteredAdmissionLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                       CatchesAndSalesPublicFilters filters,
                                                                                                       List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<AdmissionLogBookPageRegisterHelper> baseQuery = GetAllAdmissionFilteredLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            // TODO
            List<AdmissionLogBookPageRegisterDTO> filledPages = FinalizeAdmissionLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private IQueryable<AdmissionLogBookPageRegisterHelper> GetAllAdmissionFilteredLogBookPagesBaseQuery(IEnumerable<int> logBookIds,
                                                                                                            List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<string> permittedLogBookTypeStrings = permittedLogBookTypes.Select(x => x.ToString()).ToList();

            IQueryable<AdmissionLogBookPageRegisterHelper> filledPages = from page in Db.AdmissionLogBookPages
                                                                         join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                                                                         join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                                                         join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id into pageBuyer
                                                                         from acceptingBuyer in pageBuyer.DefaultIfEmpty()
                                                                         join acceptingBuyerLegal in Db.Legals on acceptingBuyer.SubmittedForLegalId equals acceptingBuyerLegal.Id into buyerLegal
                                                                         from acceptingBuyerLegal in buyerLegal.DefaultIfEmpty()
                                                                         join acceptingBuyerPerson in Db.Persons on acceptingBuyer.SubmittedForPersonId equals acceptingBuyerPerson.Id into buyerPerson
                                                                         from acceptingBuyerPerson in buyerPerson.DefaultIfEmpty()
                                                                         join person in Db.Persons on logBook.PersonId equals person.Id into pagePerson
                                                                         from acceptingPerson in pagePerson.DefaultIfEmpty()
                                                                         join legal in Db.Legals on logBook.LegalId equals legal.Id into pageLegal
                                                                         from acceptingLegal in pageLegal.DefaultIfEmpty()
                                                                         join registeredBuyerAppl in Db.Applications on acceptingBuyer.ApplicationId equals registeredBuyerAppl.Id into buyerAppl
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
                                                                         select new AdmissionLogBookPageRegisterHelper
                                                                         {
                                                                             Id = page.Id,
                                                                             PageNumber = page.PageNum,
                                                                             LogBookId = page.LogBookId,
                                                                             IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                                                             TerritoryUnitId = registeredBuyerAppl != null
                                                                                               ? registeredBuyerAppl.TerritoryUnitId.Value
                                                                                               : applPermitLicense != null
                                                                                                    ? applPermitLicense.TerritoryUnitId.Value
                                                                                                    : -1,
                                                                             HandoverDate = page.HandoverDate,
                                                                             StorageLocation = page.StorageLocation,
                                                                             AcceptedPersonNames = acceptingBuyer != null && acceptingBuyerLegal != null
                                                                                                   ? acceptingBuyerLegal.Name
                                                                                                   : acceptingBuyer != null && acceptingBuyerPerson != null
                                                                                                        ? acceptingBuyerPerson.FirstName + " " + acceptingBuyerPerson.LastName
                                                                                                        : acceptingPerson != null
                                                                                                            ? acceptingPerson.FirstName + " " + acceptingPerson.LastName
                                                                                                            : acceptingLegal != null
                                                                                                                ? acceptingLegal.Name
                                                                                                                : null,
                                                                             StatusCode = page.Status,
                                                                             CancellationReason = page.CancelationReason,
                                                                             IsActive = page.IsActive
                                                                         };

            return filledPages;
        }

        private List<AdmissionLogBookPageRegisterDTO> FinalizeAdmissionLogBookPagesForTable(IQueryable<AdmissionLogBookPageRegisterHelper> query)
        {
            List<AdmissionLogBookPageRegisterDTO> filledPages = (from page in query
                                                                 group page by new
                                                                 {
                                                                     page.Id,
                                                                     page.PageNumber,
                                                                     page.LogBookId,
                                                                     page.IsLogBookFinished,
                                                                     page.HandoverDate,
                                                                     page.StorageLocation,
                                                                     page.AcceptedPersonNames,
                                                                     page.StatusCode,
                                                                     page.CancellationReason,
                                                                     page.IsActive
                                                                 } into p
                                                                 select new AdmissionLogBookPageRegisterDTO
                                                                 {
                                                                     Id = p.Key.Id,
                                                                     PageNumber = p.Key.PageNumber,
                                                                     LogBookId = p.Key.LogBookId,
                                                                     IsLogBookFinished = p.Key.IsLogBookFinished,
                                                                     HandoverDate = p.Key.HandoverDate,
                                                                     StorageLocation = p.Key.StorageLocation,
                                                                     AcceptedPersonNames = p.Key.AcceptedPersonNames,
                                                                     Status = Enum.Parse<LogBookPageStatusesEnum>(p.Key.StatusCode),
                                                                     CancellationReason = p.Key.CancellationReason,
                                                                     IsActive = p.Key.IsActive
                                                                 }).ToList();

            List<int> pageIds = filledPages.Select(x => x.Id).ToList();
            HashSet<int> pagesWithOriginProducts = (from product in Db.LogBookPageProducts
                                                    join originProduct in Db.LogBookPageProducts on product.OriginProductId equals originProduct.Id
                                                    where product.FirstSaleLogBookPageId.HasValue
                                                          && originProduct.AdmissionLogBookPageId.HasValue
                                                          && pageIds.Contains(originProduct.AdmissionLogBookPageId.Value)
                                                    select originProduct.AdmissionLogBookPageId.Value).ToHashSet();

            foreach (AdmissionLogBookPageRegisterDTO page in filledPages)
            {
                page.ConsistsOriginProducts = pagesWithOriginProducts.Contains(page.Id);
            }

            return filledPages;
        }

        private List<FishInformationDTO> GetAdmissionLogBookPagesCatchInformation(List<int> logBookPageIds)
        {
            List<FishInformationDTO> results = (from product in Db.LogBookPageProducts
                                                join fish in Db.Nfishes on product.FishId equals fish.Id
                                                where product.AdmissionLogBookPageId.HasValue && logBookPageIds.Contains(product.AdmissionLogBookPageId.Value)
                                                      && product.IsActive
                                                select new FishInformationDTO
                                                {
                                                    LogBookPageId = product.AdmissionLogBookPageId.Value,
                                                    FishData = product.QuantityKg + "kg " + fish.Name
                                                }).ToList();

            return results;
        }

        private List<LogBookPageProductDTO> GetAdmissionPageProducts(int pageId)
        {
            IQueryable<LogBookPageProduct> productsQuery = from product in Db.LogBookPageProducts
                                                           where product.AdmissionLogBookPageId == pageId
                                                           select product;

            List<LogBookPageProductDTO> products = GetProductsDTO(productsQuery);

            return products;
        }

        private HashSet<decimal> GetAlreadySubmittedAdmissionPages(long startPage, long endPage)
        {
            HashSet<decimal> alreadySubmittedPageNumbers = (from admissionPage in Db.AdmissionLogBookPages
                                                            join lb in Db.LogBooks on admissionPage.LogBookId equals lb.Id
                                                            where !lb.IsOnline
                                                                  && admissionPage.PageNum >= startPage
                                                                  && admissionPage.PageNum < endPage
                                                            select Convert.ToDecimal(admissionPage.PageNum)).ToHashSet();

            return alreadySubmittedPageNumbers;
        }

        private void AddAdmissionLogBookMissingPage(int logBookId, decimal pageNum, int? logBookPermitLicenseId)
        {
            AdmissionLogBookPage entry = new AdmissionLogBookPage
            {
                LogBookId = logBookId,
                LogBookPermitLicenseId = logBookPermitLicenseId,
                PageNum = pageNum,
                Status = nameof(LogBookPageStatusesEnum.Missing)
            };

            Db.AdmissionLogBookPages.Add(entry);
        }

        private void DeleteAnnulledAdmissionLogBookPageProducts(int logBookPageId)
        {
            List<LogBookPageProduct> logBookPageProducts = (from lbPageProduct in Db.LogBookPageProducts
                                                            where lbPageProduct.AdmissionLogBookPageId == logBookPageId
                                                                  && lbPageProduct.IsActive
                                                            select lbPageProduct).ToList();

            foreach (LogBookPageProduct logBookPageProduct in logBookPageProducts)
            {
                logBookPageProduct.IsActive = false;
            }
        }
    }

    internal class AdmissionLogBookPageRegisterHelper : AdmissionLogBookPageRegisterDTO
    {
        public int TerritoryUnitId { get; set; }

        public string StatusCode { get; set; }
    }
}
