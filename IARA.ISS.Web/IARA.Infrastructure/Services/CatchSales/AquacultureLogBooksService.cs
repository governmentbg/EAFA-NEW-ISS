using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces.CatchSales;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.CatchSales
{
    public partial class LogBooksService : Service, ILogBooksService
    {
        public List<LogBookEditDTO> GetAquacultureFacilityLogBooks(int aquacultureFacilityId)
        {
            IQueryable<LogBook> logBooks = from logBook in Db.LogBooks
                                           where logBook.AquacultureFacilityId == aquacultureFacilityId
                                           select logBook;

            List<LogBookEditDTO> results = MapLogBooksToDTO(logBooks).ToList();

            foreach (LogBookEditDTO result in results)
            {
                result.AquaculturePages = GetAquaculturePages(result.LogBookId.Value);
            }

            return results;
        }

        public AquacultureLogBookPageEditDTO GetAquacultureLogBookPage(int id)
        {
            var data = (from page in Db.AquacultureLogBookPages
                        join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                        join aquacultureFacility in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquacultureFacility.Id
                        where page.Id == id
                        select new
                        {
                            Page = new AquacultureLogBookPageEditDTO
                            {
                                Id = page.Id,
                                PageNumber = page.PageNum,
                                LogBookId = page.LogBookId,
                                FillingDate = page.FillingDate,
                                IaraAcceptanceDateTime = page.IaraacceptanceDateTime,
                                AquacultureFacilityName = $"{aquacultureFacility.Name} ({aquacultureFacility.UrorNum})",
                                Status = page.Status
                            },
                            BuyerType = Enum.Parse<LogBookPagePersonTypesEnum>(page.BuyerPersonType),
                            RegisteredBuyerId = page.RegisteredBuyerId,
                            PersonBuyerId = page.PersonBuyerId,
                            LegalBuyerId = page.LegalBuyerId
                        }).First();

            data.Page.Buyer = GetLogBookPagePerson(data.BuyerType,
                                                   data.RegisteredBuyerId,
                                                   data.PersonBuyerId,
                                                   data.LegalBuyerId);

            data.Page.Products = GetAquaculturePageProducts(id);

            data.Page.Files = Db.GetFiles(Db.AquacultureLogBookPageFiles, id);

            return data.Page;

        }

        public AquacultureLogBookPageEditDTO GetNewAquacultureLogBookPage(int logBookId)
        {
            DateTime now = DateTime.Now;

            AquacultureLogBookPageEditDTO aquaculturePage = (from logBook in Db.LogBooks
                                                             join aquacultureFacility in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquacultureFacility.Id
                                                             where logBook.Id == logBookId
                                                             select new AquacultureLogBookPageEditDTO
                                                             {
                                                                 LogBookId = logBook.Id,
                                                                 FillingDate = now,
                                                                 AquacultureFacilityName = $"{aquacultureFacility.Name} ({aquacultureFacility.UrorNum})"
                                                             }).First();

            return aquaculturePage;
        }

        public int AddAquacultureLogBookPage(AquacultureLogBookPageEditDTO page)
        {
            OldLogBookPageStatus oldPage = GetNewLogBookPageStatusAndCheckValidity(page.PageNumber.Value, page.LogBookId.Value, Db.AquacultureLogBookPages);

            if (oldPage != null && oldPage.Status == LogBookPageStatusesEnum.Missing)
            {
                page.Id = oldPage.Id;
                EditAquacultureLogBookPage(page);

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

            HashSet<decimal> alreadySubmittedPageNumbers = GetAlreadySubmittedAquaculturePages(pageIntervalStart, pageIntervalEnd);

            for (long num = lastUsedPageNum + 1; num < page.PageNumber.Value; ++num)
            {
                if (!alreadySubmittedPageNumbers.Contains(num)) // The page is not submitted for any other aquaculture log book
                {
                    AddAquacultureLogBookMissingPage(page.LogBookId.Value, num);
                }
            }

            AquacultureLogBookPage entry = new AquacultureLogBookPage
            {
                LogBookId = page.LogBookId.Value,
                Status = nameof(LogBookPageStatusesEnum.Submitted),
                PageNum = page.PageNumber.Value,
                FillingDate = page.FillingDate.Value,
                IaraacceptanceDateTime = page.IaraAcceptanceDateTime.Value
            };

            SetBuyerPersonAquacultureLogBookPageFields(page.Buyer, entry);
            AddOrEditLogBookPageProducts(page.Products, LogBookTypesEnum.Aquaculture, aquaculturePage: entry);

            if (page.Files != null)
            {
                foreach (FileInfoDTO file in page.Files)
                {
                    Db.AddOrEditFile(entry, entry.AquacultureLogBookPageFiles, file);
                }
            }

            if ((long)page.PageNumber.Value > logBook.LastPageNum)
            {
                logBook.LastPageNum = (int)page.PageNumber.Value;
            }

            Db.AquacultureLogBookPages.Add(entry);
            Db.SaveChanges();

            return entry.Id;
        }

        public void EditAquacultureLogBookPage(AquacultureLogBookPageEditDTO page)
        {
            AquacultureLogBookPage dbEntry = (from aquaculturePage in Db.AquacultureLogBookPages.Include(x => x.AquacultureLogBookPageFiles)
                                              where aquaculturePage.Id == page.Id
                                              select aquaculturePage).First();

            dbEntry.Status = nameof(LogBookPageStatusesEnum.Submitted);
            dbEntry.FillingDate = page.FillingDate.Value;
            dbEntry.IaraacceptanceDateTime = page.IaraAcceptanceDateTime.Value;

            SetBuyerPersonAquacultureLogBookPageFields(page.Buyer, dbEntry);
            AddOrEditLogBookPageProducts(page.Products, LogBookTypesEnum.Aquaculture, aquaculturePage: dbEntry);

            if (page.Files != null)
            {
                foreach (FileInfoDTO file in page.Files)
                {
                    Db.AddOrEditFile(dbEntry, dbEntry.AquacultureLogBookPageFiles, file);
                }
            }

            Db.SaveChanges();
        }

        public SimpleAuditDTO GetAquacultureLogBookPageSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(Db.AquacultureLogBookPages, id);
        }

        private List<AquacultureLogBookPageRegisterDTO> GetAquaculturePages(int logBookId)
        {
            List<AquacultureLogBookPageRegisterDTO> results = (from page in Db.AquacultureLogBookPages
                                                               join buyer in Db.BuyerRegisters on page.RegisteredBuyerId equals buyer.Id into pageBuyer
                                                               from registeredBuyer in pageBuyer.DefaultIfEmpty()
                                                               join registeredBuyerLegal in Db.Legals on registeredBuyer.SubmittedForLegalId equals registeredBuyerLegal.Id into regBuyerLegal
                                                               from registeredBuyerLegal in regBuyerLegal.DefaultIfEmpty()
                                                               join registeredBuyerPerson in Db.Persons on registeredBuyer.SubmittedForPersonId equals registeredBuyerPerson.Id into regBuyerPerson
                                                               from registeredBuyerPerson in regBuyerPerson.DefaultIfEmpty()
                                                               join person in Db.Persons on page.PersonBuyerId equals person.Id into pagePerson
                                                               from buyerPerson in pagePerson.DefaultIfEmpty()
                                                               join legal in Db.Legals on page.LegalBuyerId equals legal.Id into pageLegal
                                                               from buyerLegal in pageLegal.DefaultIfEmpty()
                                                               where page.LogBookId == logBookId
                                                               select new AquacultureLogBookPageRegisterDTO
                                                               {
                                                                   Id = page.Id,
                                                                   LogBookId = page.LogBookId,
                                                                   PageNumber = page.PageNum,
                                                                   FillingDate = page.FillingDate,
                                                                   BuyerName = registeredBuyerLegal != null
                                                                               ? registeredBuyerLegal.Name
                                                                               : registeredBuyerPerson != null
                                                                                    ? registeredBuyerPerson.FirstName + " " + registeredBuyerPerson.LastName
                                                                                    : buyerPerson != null
                                                                                        ? buyerPerson.FirstName + " " + buyerPerson.LastName
                                                                                        : buyerLegal != null
                                                                                            ? buyerLegal.Name
                                                                                            : "",
                                                                   Status = Enum.Parse<LogBookPageStatusesEnum>(page.Status),
                                                                   CancellationReason = page.CancelationReason
                                                               }).ToList();

            List<int> logBookPageIds = results.Select(x => x.Id).ToList();
            List<FishInformationDTO> fishInformation = GetAquacultureLogBookPagesCatchInformation(logBookPageIds);
            ILookup<int, string> pageFishInformation = (from fish in fishInformation
                                                        select new
                                                        {
                                                            fish.LogBookPageId,
                                                            fish.FishData
                                                        }).ToLookup(x => x.LogBookPageId, y => y.FishData);

            foreach (AquacultureLogBookPageRegisterDTO page in results)
            {
                if (pageFishInformation[page.Id].Any())
                {
                    page.ProductionInformation = string.Join(';', pageFishInformation[page.Id]);
                }
            }

            return results;
        }

        private List<AquacultureLogBookPageRegisterDTO> GetAquacultureLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                           CatchesAndSalesAdministrationFilters filters,
                                                                                           List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<AquacultureLogBookPageRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllAquacultureLogBookPagesForTable(logBookIds, permittedLogBookTypes);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredAquacultureLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes)
                    : GetParametersFilteredAquacultureLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            }

            return result;
        }

        private List<AquacultureLogBookPageRegisterDTO> GetAquacultureLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                           CatchesAndSalesPublicFilters filters,
                                                                                           List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<AquacultureLogBookPageRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllAquacultureLogBookPagesForTable(logBookIds, permittedLogBookTypes);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredAquacultureLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes)
                    : GetParametersFilteredAquacultureLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            }

            return result;
        }

        private List<AquacultureLogBookPageRegisterDTO> GetAllAquacultureLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                              List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<AquacultureLogBookPageRegisterHelper> baseQuery = GetAquacultureLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);
            List<AquacultureLogBookPageRegisterDTO> filledPages = FinalizeAquacultureLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<AquacultureLogBookPageRegisterDTO> GetParametersFilteredAquacultureLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                             CatchesAndSalesAdministrationFilters filters,
                                                                                                             List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<AquacultureLogBookPageRegisterHelper> baseQuery = GetAquacultureLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            if (filters.TerritoryUnitId.HasValue && filters.FilterAquacultureLogBookTeritorryUnitId.HasValue && filters.FilterAquacultureLogBookTeritorryUnitId.Value)
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

            List<AquacultureLogBookPageRegisterDTO> filledPages = FinalizeAquacultureLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<AquacultureLogBookPageRegisterDTO> GetFreeTextFilteredAquacultureLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                           CatchesAndSalesAdministrationFilters filters,
                                                                                                           List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<AquacultureLogBookPageRegisterHelper> baseQuery = GetAquacultureLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            if (filters.TerritoryUnitId.HasValue && filters.FilterAquacultureLogBookTeritorryUnitId.HasValue && filters.FilterAquacultureLogBookTeritorryUnitId.Value)
            {
                baseQuery = from filledPage in baseQuery
                            where filledPage.TerritoryUnitId == filters.TerritoryUnitId.Value
                            select filledPage;
            }

            // TODO

            List<AquacultureLogBookPageRegisterDTO> filledPages = FinalizeAquacultureLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<AquacultureLogBookPageRegisterDTO> GetParametersFilteredAquacultureLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                             CatchesAndSalesPublicFilters filters,
                                                                                                             List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<AquacultureLogBookPageRegisterHelper> baseQuery = GetAquacultureLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

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

            List<AquacultureLogBookPageRegisterDTO> filledPages = FinalizeAquacultureLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<AquacultureLogBookPageRegisterDTO> GetFreeTextFilteredAquacultureLogBookPagesForTable(IEnumerable<int> logBookIds,
                                                                                                           CatchesAndSalesPublicFilters filters,
                                                                                                           List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<AquacultureLogBookPageRegisterHelper> baseQuery = GetAquacultureLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            // TODO

            List<AquacultureLogBookPageRegisterDTO> filledPages = FinalizeAquacultureLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private IQueryable<AquacultureLogBookPageRegisterHelper> GetAquacultureLogBookPagesBaseQuery(IEnumerable<int> logBookIds,
                                                                                                     List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<string> permittedLogBookTypeStrings = permittedLogBookTypes.Select(x => x.ToString()).ToList();

            IQueryable<AquacultureLogBookPageRegisterHelper> filledPages = from page in Db.AquacultureLogBookPages
                                                                           join buyer in Db.BuyerRegisters on page.RegisteredBuyerId equals buyer.Id into pageBuyer
                                                                           from registeredBuyer in pageBuyer.DefaultIfEmpty()
                                                                           join registeredBuyerLegal in Db.Legals on registeredBuyer.SubmittedForLegalId equals registeredBuyerLegal.Id into regBuyerLegal
                                                                           from registeredBuyerLegal in regBuyerLegal.DefaultIfEmpty()
                                                                           join registeredBuyerPerson in Db.Persons on registeredBuyer.SubmittedForPersonId equals registeredBuyerPerson.Id into regBuyerPerson
                                                                           from registeredBuyerPerson in regBuyerPerson.DefaultIfEmpty()
                                                                           join person in Db.Persons on page.PersonBuyerId equals person.Id into pagePerson
                                                                           from buyerPerson in pagePerson.DefaultIfEmpty()
                                                                           join legal in Db.Legals on page.LegalBuyerId equals legal.Id into pageLegal
                                                                           from buyerLegal in pageLegal.DefaultIfEmpty()
                                                                           join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                                                                           join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                                                           join aquacultureFacility in Db.AquacultureFacilitiesRegister on logBook.AquacultureFacilityId equals aquacultureFacility.Id
                                                                           join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                                                                           where logBookIds.Contains(page.LogBookId)
                                                                                 && permittedLogBookTypeStrings.Contains(logBookType.Code)
                                                                           orderby page.PageNum descending
                                                                           select new AquacultureLogBookPageRegisterHelper
                                                                           {
                                                                               Id = page.Id,
                                                                               LogBookId = page.LogBookId,
                                                                               PageNumber = page.PageNum,
                                                                               IsLogBookFinished = logBookStatus.Code == nameof(LogBookStatusesEnum.Finished),
                                                                               FillingDate = page.FillingDate,
                                                                               TerritoryUnitId = aquacultureFacility.TerritoryUnitId,
                                                                               BuyerName = registeredBuyerLegal != null
                                                                                               ? registeredBuyerLegal.Name
                                                                                               : registeredBuyerPerson != null
                                                                                                    ? registeredBuyerPerson.FirstName + " " + registeredBuyerPerson.LastName
                                                                                                    : buyerPerson != null
                                                                                                        ? buyerPerson.FirstName + " " + buyerPerson.LastName
                                                                                                        : buyerLegal != null
                                                                                                            ? buyerLegal.Name
                                                                                                            : null,
                                                                               Status = Enum.Parse<LogBookPageStatusesEnum>(page.Status),
                                                                               CancellationReason = page.CancelationReason,
                                                                               IsActive = page.IsActive
                                                                           };

            return filledPages;
        }

        private static List<AquacultureLogBookPageRegisterDTO> FinalizeAquacultureLogBookPagesForTable(IQueryable<AquacultureLogBookPageRegisterHelper> query)
        {
            List<AquacultureLogBookPageRegisterDTO> filledPages = (from page in query
                                                                   select new AquacultureLogBookPageRegisterDTO
                                                                   {
                                                                       Id = page.Id,
                                                                       LogBookId = page.LogBookId,
                                                                       PageNumber = page.PageNumber,
                                                                       IsLogBookFinished = page.IsLogBookFinished,
                                                                       FillingDate = page.FillingDate,
                                                                       BuyerName = page.BuyerName,
                                                                       Status = page.Status,
                                                                       CancellationReason = page.CancellationReason,
                                                                       IsActive = page.IsActive
                                                                   }).ToList();

            return filledPages;
        }

        private List<FishInformationDTO> GetAquacultureLogBookPagesCatchInformation(List<int> logBookPageIds)
        {
            List<FishInformationDTO> results = (from product in Db.LogBookPageProducts
                                                join fish in Db.Nfishes on product.FishId equals fish.Id
                                                where product.AquacultureLogBookPageId.HasValue && logBookPageIds.Contains(product.AquacultureLogBookPageId.Value)
                                                      && product.IsActive
                                                select new FishInformationDTO
                                                {
                                                    LogBookPageId = product.AquacultureLogBookPageId.Value,
                                                    FishData = product.QuantityKg + "kg " + fish.Name // TODO add quantities where fish is the same
                                                }).ToList();

            return results;
        }

        private List<LogBookPageProductDTO> GetAquaculturePageProducts(int pageId)
        {
            IQueryable<LogBookPageProduct> productsQuery = from product in Db.LogBookPageProducts
                                                           where product.AquacultureLogBookPageId == pageId
                                                           select product;

            List<LogBookPageProductDTO> products = GetProductsDTO(productsQuery);

            return products;
        }

        private void SetBuyerPersonAquacultureLogBookPageFields(LogBookPagePersonDTO dto, AquacultureLogBookPage dbEntry)
        {
            dbEntry.BuyerPersonType = dto.PersonType.ToString();

            switch (dto.PersonType)
            {
                case LogBookPagePersonTypesEnum.RegisteredBuyer:
                    dbEntry.RegisteredBuyerId = dto.BuyerId;

                    dbEntry.PersonBuyerId = null;
                    dbEntry.LegalBuyerId = null;
                    break;
                case LogBookPagePersonTypesEnum.Person:
                    Person buyerPerson = Db.AddOrEditPerson(dto.Person);
                    dbEntry.PersonBuyer = buyerPerson;

                    dbEntry.RegisteredBuyerId = null;
                    dbEntry.LegalBuyerId = null;
                    break;
                case LogBookPagePersonTypesEnum.LegalPerson:
                    Legal buyerLegal = Db.AddOrEditLegal(new ApplicationRegisterDataDTO
                    {
                        RecordType = RecordTypesEnum.Register
                    }, dto.PersonLegal);
                    dbEntry.LegalBuyer = buyerLegal;

                    dbEntry.RegisteredBuyerId = null;
                    dbEntry.PersonBuyerId = null;
                    break;
                default:
                    throw new ArgumentException($"Unknown log book page person type: ${dto.PersonType}.");
            }
        }

        private HashSet<decimal> GetAlreadySubmittedAquaculturePages(long startPage, long endPage)
        {
            HashSet<decimal> alreadySubmittedPageNumbers = (from aquaculturePage in Db.AquacultureLogBookPages
                                                            join lb in Db.LogBooks on aquaculturePage.LogBookId equals lb.Id
                                                            where !lb.IsOnline
                                                                  && aquaculturePage.PageNum >= startPage
                                                                  && aquaculturePage.PageNum < endPage
                                                            select Convert.ToDecimal(aquaculturePage.PageNum)).ToHashSet();

            return alreadySubmittedPageNumbers;
        }

        private void AddAquacultureLogBookMissingPage(int logBookId, decimal pageNum)
        {
            AquacultureLogBookPage entry = new AquacultureLogBookPage
            {
                LogBookId = logBookId,
                PageNum = pageNum,
                Status = nameof(LogBookPageStatusesEnum.Missing)
            };

            Db.AquacultureLogBookPages.Add(entry);
        }

        private void DeleteAnnulledAquacultureLogBookPageProducts(int logBookPageId)
        {
            List<LogBookPageProduct> logBookPageProducts = (from lbPageProduct in Db.LogBookPageProducts
                                                            where lbPageProduct.AquacultureLogBookPageId == logBookPageId
                                                                  && lbPageProduct.IsActive
                                                            select lbPageProduct).ToList();

            foreach (LogBookPageProduct logBookPageProduct in logBookPageProducts)
            {
                logBookPageProduct.IsActive = false;
            }
        }
    }

    internal class AquacultureLogBookPageRegisterHelper : AquacultureLogBookPageRegisterDTO
    {
        public int TerritoryUnitId { get; set; }
    }
}
