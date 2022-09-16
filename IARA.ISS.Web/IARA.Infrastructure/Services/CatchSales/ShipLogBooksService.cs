using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
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
        public ShipLogBookPageEditDTO GetShipLogBookPage(int id)
        {
            ShipLogBookPageEditDTO shipPage = (from page in Db.ShipLogBookPages
                                               join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                                               join logBookLicense in Db.LogBookPermitLicenses on page.LogBookPermitLicenceId equals logBookLicense.Id
                                               join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookLicense.PermitLicenseRegisterId equals permitLicense.Id
                                               join waterType in Db.NwaterTypes on permitLicense.WaterTypeId equals waterType.Id
                                               join qualifiedFisher in Db.FishermenRegisters on permitLicense.QualifiedFisherId equals qualifiedFisher.Id
                                               join fisherPerson in Db.Persons on qualifiedFisher.PersonId equals fisherPerson.Id
                                               join ship in Db.ShipsRegister on permitLicense.ShipId equals ship.Id
                                               where page.Id == id
                                               select new ShipLogBookPageEditDTO
                                               {
                                                   Id = page.Id,
                                                   PageNumber = page.PageNum.ToString(),
                                                   StatusCode = Enum.Parse<LogBookPageStatusesEnum>(page.Status),
                                                   LogBookId = page.LogBookId,
                                                   FillDate = page.PageFillDate,
                                                   IaraAcceptanceDateTime = page.IaraacceptanceDateTime,
                                                   ShipId = permitLicense.ShipId,
                                                   ShipName = ship.Name,
                                                   LogBookPermitLicenseId = logBookLicense.Id,
                                                   PermitLicenseId = permitLicense.Id,
                                                   PermitLicenseNumber = permitLicense.RegistrationNum,
                                                   PermitLicenseName = $"{fisherPerson.FirstName} {fisherPerson.LastName} ({qualifiedFisher.RegistrationNum})",
                                                   PermitLicenseWaterType = Enum.Parse<WaterTypesEnum>(waterType.Code),
                                                   PermitLicenseWaterTypeName = waterType.Name,
                                                   ArrivalPortId = page.ArrivePortId,
                                                   DeparturePortId = page.DepartPortId,
                                                   FishingGearRegisterId = page.FishingGearRegisterId,
                                                   FishingGearCount = page.FishingGearCount,
                                                   FishingGearHookCount = page.FishingGearHooksCount,
                                                   FishTripStartDateTime = page.FishTripStartDateTime,
                                                   FishTripEndDateTime = page.FishTripEndDateTime,
                                                   PartnerShipId = page.PartnerShipId
                                               }).First();

            shipPage.CatchRecords = GetShipLogBookPageCatchRecords(id);

            shipPage.OriginDeclarationId = (from originDeclaration in Db.OriginDeclarations
                                            where originDeclaration.LogBookPageId == id
                                            select originDeclaration).SingleOrDefault()?.Id;

            if (shipPage.OriginDeclarationId.HasValue)
            {
                shipPage.OriginDeclarationFishes = GetOriginDeclarationFishes(new List<int> { shipPage.OriginDeclarationId.Value })[shipPage.Id.Value].ToList();

                var unloadData = (from originDeclarationFish in Db.OriginDeclarationFish
                                  join unloadType in Db.NcatchFishUnloadTypes on originDeclarationFish.UnloadTypeId equals unloadType.Id
                                  where originDeclarationFish.OriginDeclarationId == shipPage.OriginDeclarationId.Value
                                        && unloadType.Code == nameof(UnloadingTypesEnum.UNL)
                                  select new
                                  {
                                      originDeclarationFish.UnloadPortId,
                                      originDeclarationFish.UnloadDateTime
                                  }).FirstOrDefault();

                if (unloadData != null)
                {
                    shipPage.UnloadPortId = unloadData.UnloadPortId;
                    shipPage.UnloadDateTime = unloadData.UnloadDateTime;
                }
            }

            shipPage.Files = Db.GetFiles(Db.ShipLogBookPageFiles, id);

            return shipPage;
        }

        public List<ShipLogBookPageEditDTO> GetNewShipLogBookPages(long pageNumber, int logBookId)
        {
            GetNewLogBookPageStringStatusAndCheckValidity(pageNumber, logBookId, Db.ShipLogBookPages);

            DateTime now = DateTime.Now;

            List<ShipLogBookPageEditDTO> shipPages = (from logBook in Db.LogBooks
                                                      join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId
                                                      join permitLicense in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicense.PermitLicenseRegisterId equals permitLicense.Id
                                                      join waterType in Db.NwaterTypes on permitLicense.WaterTypeId equals waterType.Id
                                                      join qualifiedFisher in Db.FishermenRegisters on permitLicense.QualifiedFisherId equals qualifiedFisher.Id
                                                      join fisherPerson in Db.Persons on qualifiedFisher.PersonId equals fisherPerson.Id
                                                      join ship in Db.ShipsRegister on logBook.ShipId equals ship.Id
                                                      where logBook.Id == logBookId
                                                            && logBookPermitLicense.StartPageNum <= pageNumber
                                                            && logBookPermitLicense.EndPageNum >= pageNumber
                                                            && logBook.IsActive
                                                      select new ShipLogBookPageEditDTO
                                                      {
                                                          LogBookId = logBook.Id,
                                                          FillDate = now,
                                                          ShipId = logBook.ShipId.Value,
                                                          ShipName = ship.Name,
                                                          LogBookPermitLicenseId = logBookPermitLicense.Id,
                                                          PermitLicenseId = permitLicense.Id,
                                                          PermitLicenseNumber = permitLicense.RegistrationNum,
                                                          PermitLicenseName = $"{fisherPerson.FirstName} {fisherPerson.LastName} ({qualifiedFisher.RegistrationNum})",
                                                          PermitLicenseWaterType = Enum.Parse<WaterTypesEnum>(waterType.Code),
                                                          PermitLicenseWaterTypeName = waterType.Name
                                                      }).ToList();

            return shipPages;
        }

        public List<ShipLogBookPageRegisterDTO> GetShipLogBookPagesAndDeclarations(int logBookId, int? permitLicenseId = null)
        {
            var query = from page in Db.ShipLogBookPages
                        join fishingGearRegister in Db.FishingGearRegisters on page.FishingGearRegisterId equals fishingGearRegister.Id into pageFGR
                        from fishingGearRegister in pageFGR.DefaultIfEmpty()
                        join fishingGear in Db.NfishingGears on fishingGearRegister.FishingGearTypeId equals fishingGear.Id into fG
                        from fishingGear in fG.DefaultIfEmpty()
                        join port in Db.Nports on page.ArrivePortId equals port.Id into pagePort
                        from port in pagePort.DefaultIfEmpty()
                        where page.LogBookId == logBookId
                        select new
                        {
                            Id = page.Id,
                            LogBookId = page.LogBookId,
                            LogBookPermitLicenseId = page.LogBookPermitLicenceId,
                            PageNumber = page.PageNum,
                            OutingStartDate = page.FishTripStartDateTime,
                            PortOfUnloading = port != null ? port.Name : "",
                            FishingGearCode = fishingGear != null ? fishingGear.Code : null,
                            FishingGearName = fishingGear != null ? fishingGear.Name : null,
                            StatusCode = page.Status,
                            CancellationReason = page.CancelationReason,
                            IsActive = page.IsActive
                        };

            if (permitLicenseId.HasValue)
            {
                query = from page in query
                        join logBookPermitLicense in Db.LogBookPermitLicenses on page.LogBookPermitLicenseId equals logBookPermitLicense.Id
                        where logBookPermitLicense.PermitLicenseRegisterId == permitLicenseId
                        group page by new
                        {
                            page.Id,
                            page.LogBookId,
                            page.LogBookPermitLicenseId,
                            page.PageNumber,
                            page.OutingStartDate,
                            page.PortOfUnloading,
                            page.FishingGearCode,
                            page.FishingGearName,
                            page.StatusCode,
                            page.CancellationReason,
                            page.IsActive
                        } into p
                        select p.Key;
            }

            List<ShipLogBookPageRegisterDTO> pages = (from page in query
                                                      select new ShipLogBookPageRegisterDTO
                                                      {
                                                          Id = page.Id,
                                                          LogBookId = page.LogBookId,
                                                          PageNumber = page.PageNumber,
                                                          OutingStartDate = page.OutingStartDate,
                                                          PortOfUnloading = page.PortOfUnloading,
                                                          FishingGear = page.FishingGearCode != null ? $"{page.FishingGearCode} - {page.FishingGearName}" : "",
                                                          Status = Enum.Parse<LogBookPageStatusesEnum>(page.StatusCode),
                                                          CancellationReason = page.CancellationReason,
                                                          IsActive = page.IsActive
                                                      }).ToList();

            // Documents and declarations

            FillShipPagesDocumentAndDeclarations(pages);

            // Unloading information

            List<int> logBookPageIds = pages.Select(x => x.Id).ToList();
            List<FishInformationDTO> unloadingInformation = GetShipLogBookPagesOriginDeclarationInformation(logBookPageIds);
            ILookup<int, string> pageUnloadingInformation = (from unloading in unloadingInformation
                                                             select new
                                                             {
                                                                 unloading.LogBookPageId,
                                                                 unloading.FishData
                                                             }).ToLookup(x => x.LogBookPageId, y => y.FishData);
            foreach (ShipLogBookPageRegisterDTO page in pages)
            {
                page.UnloadingInformation = string.Join(';', pageUnloadingInformation[page.Id]);
            }

            return pages;
        }

        public List<OnBoardCatchRecordFishDTO> GetPreviousTripsOnBoardCatchRecords(int shipId)
        {
            List<OnBoardCatchRecordFishDTO> onBoardCatchRecordFishes = (from logBook in Db.LogBooks
                                                                        join page in Db.ShipLogBookPages on logBook.Id equals page.LogBookId
                                                                        join catchRecord in Db.CatchRecords on page.Id equals catchRecord.LogBookPageId
                                                                        join catchRecordFish in Db.CatchRecordFish on catchRecord.Id equals catchRecordFish.CatchRecordId
                                                                        join catchQuandrant in Db.NcatchZones on catchRecordFish.CatchZoneId equals catchQuandrant.Id
                                                                        where logBook.ShipId == shipId
                                                                              && logBook.IsActive
                                                                              && page.IsActive
                                                                              && catchRecord.IsActive
                                                                              && catchRecordFish.IsActive
                                                                              && catchRecordFish.Quantity - catchRecordFish.UnloadedQuantity > 0
                                                                        select new OnBoardCatchRecordFishDTO
                                                                        {
                                                                            Id = catchRecordFish.Id,
                                                                            ShipLogBookPageId = page.Id,
                                                                            ShipLogBookPageNumber = page.PageNum,
                                                                            TripStartDateTime = page.FishTripStartDateTime.Value,
                                                                            TripEndDateTime = page.FishTripEndDateTime.Value,
                                                                            FishId = catchRecordFish.FishId,
                                                                            QuantityKg = catchRecordFish.Quantity - catchRecordFish.UnloadedQuantity,
                                                                            CatchQuadrantId = catchRecordFish.CatchZoneId,
                                                                            CatchTypeId = catchRecordFish.CatchTypeId,
                                                                            CatchSizeId = catchRecordFish.CatchSizeId,
                                                                            SturgeonWeightKg = catchRecordFish.SturgeonWeightKg,
                                                                            SturgeonSize = catchRecordFish.SturgeonSize,
                                                                            TurbotSizeGroupId = catchRecordFish.TurbotSizeGroupId,
                                                                            TurbotCount = catchRecordFish.TurbotCount,
                                                                            IsContinentalCatch = catchRecordFish.IsContinentalCatch,
                                                                            ThirdCountryCatchZone = catchRecordFish.ThirdCountryCatchZone,
                                                                            IsActive = catchRecordFish.IsActive
                                                                        }).ToList();

            return onBoardCatchRecordFishes;
        }

        public int AddShipLogBookPage(ShipLogBookPageEditDTO page)
        {
            long newPageNum = long.Parse(page.PageNumber);

            OldLogBookPageStatus oldPage = GetNewLogBookPageStringStatusAndCheckValidity(newPageNum, page.LogBookId.Value, Db.ShipLogBookPages);

            if (oldPage != null && oldPage.Status == LogBookPageStatusesEnum.Missing)
            {
                page.Id = oldPage.Id;
                EditShipLogBookPage(page);

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
                lastUsedPageNum = (from logBookPermitLicense in Db.LogBookPermitLicenses
                                   where logBookPermitLicense.Id == page.LogBookPermitLicenseId
                                   select logBookPermitLicense.StartPageNum).First().Value - 1;
            }

            long pageIntervalStart = lastUsedPageNum + 1;
            long pageIntervalEnd = newPageNum;

            HashSet<decimal> alreadySubmittedPageNumbers = GetAlreadySubmittedShipPages(pageIntervalStart, pageIntervalEnd);

            for (long num = pageIntervalStart; num < pageIntervalEnd; ++num)
            {
                if (!alreadySubmittedPageNumbers.Contains(num)) // The page is not submitted for any other ship log book
                {
                    AddShipLogBookMissingPage(page.LogBookId.Value, num.ToString(), page.LogBookPermitLicenseId.Value);
                }
            }

            ShipLogBookPage entry = new ShipLogBookPage
            {
                LogBookId = page.LogBookId.Value,
                LogBookPermitLicenceId = page.LogBookPermitLicenseId.Value,
                PageNum = page.PageNumber.ToString(),
                Status = nameof(LogBookPageStatusesEnum.Submitted),
                DepartPortId = page.DeparturePortId.Value,
                ArrivePortId = page.ArrivalPortId.Value,
                PageFillDate = page.FillDate.Value,
                FishingGearRegisterId = page.FishingGearRegisterId.Value,
                FishingGearCount = page.FishingGearCount.Value,
                FishingGearHooksCount = page.FishingGearHookCount,
                FishTripStartDateTime = page.FishTripStartDateTime.Value,
                FishTripEndDateTime = page.FishTripEndDateTime.Value,
                IaraacceptanceDateTime = page.IaraAcceptanceDateTime,
                PartnerShipId = page.PartnerShipId
            };

            List<CatchRecord> dbCatchRecords = AddOrEditCatchRecords(page.CatchRecords, entry);
            List<CatchRecordFish> dbCatchRecordFishes = dbCatchRecords.SelectMany(x => x.CatchRecordFishes).ToList();
            AddOrEditOriginDeclaration(entry,
                                       page.OriginDeclarationId,
                                       page.OriginDeclarationFishes,
                                       dbCatchRecordFishes,
                                       page.UnloadPortId,
                                       page.UnloadDateTime);

            if (page.Files != null)
            {
                foreach (FileInfoDTO file in page.Files)
                {
                    Db.AddOrEditFile(entry, entry.ShipLogBookPageFiles, file);
                }
            }

            if (newPageNum > logBook.LastPageNum)
            {
                logBook.LastPageNum = newPageNum;
            }

            Db.ShipLogBookPages.Add(entry);
            Db.SaveChanges();

            return entry.Id;
        }

        public void EditShipLogBookPage(ShipLogBookPageEditDTO page)
        {
            ShipLogBookPage dbEntry = (from shipPage in Db.ShipLogBookPages
                                                          .Include(x => x.ShipLogBookPageFiles)
                                       where shipPage.Id == page.Id
                                       select shipPage).First();

            ShipLogBookPageFLUXFieldsDTO fluxRelatedData = new ShipLogBookPageFLUXFieldsDTO
            { 
                PageId = dbEntry.Id
            };

            dbEntry.Status = nameof(LogBookPageStatusesEnum.Submitted);

            if (dbEntry.DepartPortId != page.DeparturePortId.Value)
            {
                dbEntry.DepartPortId = page.DeparturePortId.Value;
                fluxRelatedData.DeparturePortId = dbEntry.DepartPortId;
            }

            if (dbEntry.ArrivePortId != page.ArrivalPortId.Value)
            {
                dbEntry.ArrivePortId = page.ArrivalPortId.Value;
                fluxRelatedData.ArrivalPortId = dbEntry.ArrivePortId;
            }

            if (dbEntry.PageFillDate != page.FillDate.Value)
            {
                dbEntry.PageFillDate = page.FillDate.Value;
                fluxRelatedData.PageFillDate = dbEntry.PageFillDate;
            }

            if (dbEntry.FishingGearRegisterId != page.FishingGearRegisterId.Value)
            {
                dbEntry.FishingGearRegisterId = page.FishingGearRegisterId.Value;
                fluxRelatedData.FishingGearRegisterId = dbEntry.FishingGearRegisterId;
            }

            if (dbEntry.FishingGearCount != page.FishingGearCount.Value)
            {
                dbEntry.FishingGearCount = page.FishingGearCount.Value;
                fluxRelatedData.FishingGearCount = dbEntry.FishingGearCount;
            }

            dbEntry.FishingGearHooksCount = page.FishingGearHookCount;

            if (dbEntry.FishTripStartDateTime != page.FishTripStartDateTime.Value)
            {
                dbEntry.FishTripStartDateTime = page.FishTripStartDateTime.Value;
                fluxRelatedData.FishTripStartDateTime = dbEntry.FishTripStartDateTime;
            }

            if (dbEntry.FishTripEndDateTime != page.FishTripEndDateTime.Value)
            { 
                dbEntry.FishTripEndDateTime = page.FishTripEndDateTime;
                fluxRelatedData.FishTripEndDateTime = dbEntry.FishTripEndDateTime;
            }
            
            dbEntry.IaraacceptanceDateTime = page.IaraAcceptanceDateTime;
            dbEntry.PartnerShipId = page.PartnerShipId;

            List<CatchRecord> dbCatchRecords = AddOrEditCatchRecords(page.CatchRecords, dbEntry);
            List<CatchRecordFish> dbCatchRecordFishes = dbCatchRecords.SelectMany(x => x.CatchRecordFishes).ToList();
            AddOrEditOriginDeclaration(dbEntry,
                                       page.OriginDeclarationId,
                                       page.OriginDeclarationFishes,
                                       dbCatchRecordFishes,
                                       page.UnloadPortId,
                                       page.UnloadDateTime);

            if (page.Files != null)
            {
                foreach (FileInfoDTO file in page.Files)
                {
                    Db.AddOrEditFile(dbEntry, dbEntry.ShipLogBookPageFiles, file);
                }
            }

            Db.SaveChanges();

            // faDomainService.ReportFishingActivities(fluxRelatedData); // TODO uncomment when report is ready
        }

        public SimpleAuditDTO GetShipLogBookPageSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(Db.ShipLogBookPages, id);
        }

        public SimpleAuditDTO GetCatchRecordSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(Db.CatchRecords, id);
        }

        public SimpleAuditDTO GetOriginDeclarationFishSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(Db.OriginDeclarationFish, id);
        }

        private List<ShipLogBookPageRegisterDTO> GetShipLogBookPagesForTable(IEnumerable<int> logBookIds, CatchesAndSalesAdministrationFilters filters, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<ShipLogBookPageRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllShipLogBookPagesForTable(logBookIds, permittedLogBookTypes);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredShipLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes)
                    : GetParametersFilteredShipLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            }

            return result;
        }

        private List<ShipLogBookPageRegisterDTO> GetAllShipLogBookPagesForTable(IEnumerable<int> logBookIds, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<ShipLogBookPageRegisterHelper> baseQuery = GetAllShipLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);
            List<ShipLogBookPageRegisterDTO> filledPages = FinalizeShipLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<ShipLogBookPageRegisterDTO> GetParametersFilteredShipLogBookPagesForTable(IEnumerable<int> logBookIds, CatchesAndSalesAdministrationFilters filters, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<ShipLogBookPageRegisterHelper> baseQuery = GetAllShipLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            if (filters.TerritoryUnitId.HasValue && filters.FilterFishLogBookTeritorryUnitId.HasValue && filters.FilterFishLogBookTeritorryUnitId.Value)
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
                            where filledPage.PageNumber == filters.PageNumber.ToString()
                            select filledPage;
            }

            if (filters.LogBookTypeId != null)
            {
                baseQuery = from filledPage in baseQuery
                            where filledPage.LogBookTypeId == filters.LogBookTypeId
                            select filledPage;
            }

            if (!string.IsNullOrEmpty(filters.LogBookNumber))
            {
                baseQuery = from filledPage in baseQuery
                            where filledPage.LogBookNumber.ToLower().Contains(filters.LogBookNumber.ToLower())
                            select filledPage;
            }

            List<ShipLogBookPageRegisterDTO> filledPages = FinalizeShipLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<ShipLogBookPageRegisterDTO> GetFreeTextFilteredShipLogBookPagesForTable(IEnumerable<int> logBookIds, CatchesAndSalesAdministrationFilters filters, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<ShipLogBookPageRegisterHelper> baseQuery = GetAllShipLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            if (filters.TerritoryUnitId.HasValue && filters.FilterFishLogBookTeritorryUnitId.HasValue && filters.FilterFishLogBookTeritorryUnitId.Value)
            {
                baseQuery = from filledPage in baseQuery
                            where filledPage.TerritoryUnitId == filters.TerritoryUnitId.Value
                            select filledPage;
            }

            // TODO text filters

            List<ShipLogBookPageRegisterDTO> filledPages = FinalizeShipLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<ShipLogBookPageRegisterDTO> GetShipLogBookPagesForTable(IEnumerable<int> logBookIds, CatchesAndSalesPublicFilters filters, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<ShipLogBookPageRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllShipLogBookPagesForTable(logBookIds, permittedLogBookTypes);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredShipLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes)
                    : GetParametersFilteredShipLogBookPagesForTable(logBookIds, filters, permittedLogBookTypes);
            }

            return result;
        }

        private List<ShipLogBookPageRegisterDTO> GetParametersFilteredShipLogBookPagesForTable(IEnumerable<int> logBookIds, CatchesAndSalesPublicFilters filters, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<ShipLogBookPageRegisterHelper> baseQuery = GetAllShipLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            //if (filters.PageNumber != null)
            //{
            //    baseQuery = from filledPage in baseQuery
            //                where filledPage.PageNumber == filters.PageNumber.ToString()
            //                select filledPage;
            //}

            //if (filters.LogBookTypeId != null)
            //{
            //    baseQuery = from filledPage in baseQuery
            //                where filledPage.LogBookTypeId == filters.LogBookTypeId
            //                select filledPage;
            //}

            //if (!string.IsNullOrEmpty(filters.LogBookNumber))
            //{
            //    baseQuery = from filledPage in baseQuery
            //                where filledPage.LogBookNumber.ToLower().Contains(filters.LogBookNumber.ToLower())
            //                select filledPage;
            //}

            List<ShipLogBookPageRegisterDTO> filledPages = FinalizeShipLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private List<ShipLogBookPageRegisterDTO> GetFreeTextFilteredShipLogBookPagesForTable(IEnumerable<int> logBookIds, CatchesAndSalesPublicFilters filters, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            IQueryable<ShipLogBookPageRegisterHelper> baseQuery = GetAllShipLogBookPagesBaseQuery(logBookIds, permittedLogBookTypes);

            // TODO text filters

            List<ShipLogBookPageRegisterDTO> filledPages = FinalizeShipLogBookPagesForTable(baseQuery);
            return filledPages;
        }

        private IQueryable<ShipLogBookPageRegisterHelper> GetAllShipLogBookPagesBaseQuery(IEnumerable<int> logBookIds, List<LogBookTypesEnum> permittedLogBookTypes)
        {
            List<string> permittedLogBookTypeStrings = permittedLogBookTypes.Select(x => x.ToString()).ToList();

            IQueryable<ShipLogBookPageRegisterHelper> result = from page in Db.ShipLogBookPages
                                                               join originDeclaration in Db.OriginDeclarations on page.Id equals originDeclaration.LogBookPageId into pageOD
                                                               from originDeclaration in pageOD.DefaultIfEmpty()
                                                               join port in Db.Nports on page.ArrivePortId equals port.Id into prt
                                                               from port in prt.DefaultIfEmpty()
                                                               join fishingGearRegister in Db.FishingGearRegisters on page.FishingGearRegisterId equals fishingGearRegister.Id into gearReg
                                                               from fishingGearRegister in gearReg.DefaultIfEmpty()
                                                               join fishingGear in Db.NfishingGears on fishingGearRegister.FishingGearTypeId equals fishingGear.Id into fgear
                                                               from fishingGear in fgear.DefaultIfEmpty()
                                                               join fishingGearType in Db.NfishingGearTypes on fishingGear.GearTypeId equals fishingGearType.Id into fgearType
                                                               from fishingGearType in fgearType.DefaultIfEmpty()
                                                               join logBook in Db.LogBooks on page.LogBookId equals logBook.Id
                                                               join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
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
                                                               select new ShipLogBookPageRegisterHelper
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
                                                                   LogBookTypeId = logBook.LogBookTypeId,
                                                                   LogBookNumber = logBook.LogNum,
                                                                   OutingStartDate = page.FishTripStartDateTime.Value,
                                                                   PortOfUnloading = port != null ? port.Name : "",
                                                                   FishingGear = fishingGear != null && fishingGearType != null
                                                                                 ? fishingGear.Code + " - " + fishingGear.Name + " (" + fishingGearType.Name + ")"
                                                                                 : "",
                                                                   StatusCode = page.Status,
                                                                   HasOriginDeclaration = originDeclaration != null,
                                                                   CancellationReason = page.CancelationReason,
                                                                   IsActive = page.IsActive
                                                               };

            return result;
        }

        private List<ShipLogBookPageRegisterDTO> FinalizeShipLogBookPagesForTable(IQueryable<ShipLogBookPageRegisterHelper> query)
        {
            List<ShipLogBookPageRegisterDTO> filledPages = (from p in query
                                                            group p by new
                                                            {
                                                                p.Id,
                                                                p.PageNumber,
                                                                p.LogBookId,
                                                                p.IsLogBookFinished,
                                                                p.OutingStartDate,
                                                                p.PortOfUnloading,
                                                                p.FishingGear,
                                                                p.StatusCode,
                                                                p.HasOriginDeclaration,
                                                                p.CancellationReason,
                                                                p.IsActive
                                                            } into page
                                                            orderby page.Key.Id descending
                                                            select new ShipLogBookPageRegisterDTO
                                                            {
                                                                Id = page.Key.Id,
                                                                PageNumber = page.Key.PageNumber,
                                                                LogBookId = page.Key.LogBookId,
                                                                IsLogBookFinished = page.Key.IsLogBookFinished,
                                                                OutingStartDate = page.Key.OutingStartDate,
                                                                PortOfUnloading = page.Key.PortOfUnloading,
                                                                FishingGear = page.Key.FishingGear,
                                                                Status = Enum.Parse<LogBookPageStatusesEnum>(page.Key.StatusCode),
                                                                HasOriginDeclaration = page.Key.HasOriginDeclaration,
                                                                CancellationReason = page.Key.CancellationReason,
                                                                IsActive = page.Key.IsActive
                                                            }).ToList();

            FillShipPagesDocumentAndDeclarations(filledPages);

            return filledPages;
        }

        private void FillShipPagesDocumentAndDeclarations(List<ShipLogBookPageRegisterDTO> pages)
        {
            List<int> shipPagesIds = pages.Select(x => x.Id).ToList();

            ILookup<int, FirstSaleLogBookPageRegisterDTO> shipFirstSaleDocuments = GetShipFirstSaleDocuments(shipPagesIds);
            ILookup<int, AdmissionLogBookPageRegisterDTO> shipAdmissionDeclarations = GetShipAdmissionDeclarations(shipPagesIds);
            ILookup<int, TransportationLogBookPageRegisterDTO> shipTransporatationDocuments = GetShipTransporatationDocuments(shipPagesIds);

            foreach (ShipLogBookPageRegisterDTO shipPage in pages)
            {
                shipPage.FirstSaleDocuments = shipFirstSaleDocuments[shipPage.Id].ToList();
                shipPage.AdmissionDeclarations = shipAdmissionDeclarations[shipPage.Id].ToList();
                shipPage.TransportationDocuments = shipTransporatationDocuments[shipPage.Id].ToList();
            }
        }

        private List<FishInformationDTO> GetShipLogBookPagesOriginDeclarationInformation(List<int> logBookPageIds)
        {
            List<FishInformationDTO> results = (from originDeclaration in Db.OriginDeclarations
                                                join originDeclarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals originDeclarationFish.OriginDeclarationId
                                                join aquaticOrganism in Db.Nfishes on originDeclarationFish.FishId equals aquaticOrganism.Id
                                                join unloadPort in Db.Nports on originDeclarationFish.UnloadPortId equals unloadPort.Id into fishPort
                                                from unloadPort in fishPort.DefaultIfEmpty()
                                                join transboardPort in Db.Nports on originDeclarationFish.TransboardTargetPortId equals transboardPort.Id into transFishPort
                                                from transboardPort in transFishPort.DefaultIfEmpty()
                                                where logBookPageIds.Contains(originDeclaration.LogBookPageId)
                                                      && originDeclaration.IsActive
                                                      && originDeclarationFish.IsActive
                                                select new FishInformationDTO
                                                {
                                                    LogBookPageId = originDeclaration.LogBookPageId,
                                                    FishData = unloadPort != null
                                                               ? originDeclarationFish.Quantity + "kg " + aquaticOrganism.Name + " (" + unloadPort.Name + ")"
                                                               : originDeclarationFish.Quantity + "kg " + aquaticOrganism.Name + " (" + transboardPort.Name + ")" // TODO add quantities where fish is the same
                                                }).ToList();

            return results;
        }

        private List<CatchRecordDTO> GetShipLogBookPageCatchRecords(int pageId)
        {
            List<CatchRecordDTO> catchRecords = (from catchRecord in Db.CatchRecords
                                                 where catchRecord.LogBookPageId == pageId
                                                 select new CatchRecordDTO
                                                 {
                                                     Id = catchRecord.Id,
                                                     CatchOperationsCount = catchRecord.CatchOperCount,
                                                     Depth = catchRecord.Depth,
                                                     GearEntryTime = catchRecord.GearEntryTime,
                                                     GearExitTime = catchRecord.GearExitTime,
                                                     IsActive = catchRecord.IsActive
                                                 }).ToList();

            List<int> catchRecordIds = catchRecords.Select(x => x.Id.Value).ToList();

            ILookup<int, CatchRecordFishDTO> catchRecordFishes = (from catchRecordFish in Db.CatchRecordFish
                                                                  join catchQuandrant in Db.NcatchZones on catchRecordFish.CatchZoneId equals catchQuandrant.Id
                                                                  join aquaticOrganism in Db.Nfishes on catchRecordFish.FishId equals aquaticOrganism.Id
                                                                  where catchRecordIds.Contains(catchRecordFish.CatchRecordId)
                                                                  select new
                                                                  {
                                                                      catchRecordFish.CatchRecordId,
                                                                      CatchRecordFish = new CatchRecordFishDTO
                                                                      {
                                                                          Id = catchRecordFish.Id,
                                                                          CatchQuadrantId = catchRecordFish.CatchZoneId,
                                                                          CatchQuadrant = catchQuandrant.Gfcmquadrant,
                                                                          CatchZone = catchQuandrant.ZoneNum.ToString(),
                                                                          FishId = catchRecordFish.FishId,
                                                                          FishName = $"{aquaticOrganism.Code} - {aquaticOrganism.Name}",
                                                                          IsContinentalCatch = catchRecordFish.IsContinentalCatch,
                                                                          QuantityKg = catchRecordFish.Quantity,
                                                                          CatchSizeId = catchRecordFish.CatchSizeId,
                                                                          CatchTypeId = catchRecordFish.CatchTypeId,
                                                                          SturgeonSize = catchRecordFish.SturgeonSize,
                                                                          SturgeonGender = catchRecordFish.SturgeonGender != null ? Enum.Parse<SturgeonGendersEnum>(catchRecordFish.SturgeonGender) : default(SturgeonGendersEnum?),
                                                                          SturgeonWeightKg = catchRecordFish.SturgeonWeightKg,
                                                                          ThirdCountryCatchZone = catchRecordFish.ThirdCountryCatchZone,
                                                                          TurbotCount = catchRecordFish.TurbotCount,
                                                                          TurbotSizeGroupId = catchRecordFish.TurbotSizeGroupId,
                                                                          IsActive = catchRecordFish.IsActive
                                                                      }
                                                                  }).ToLookup(x => x.CatchRecordId, y => y.CatchRecordFish);

            foreach (CatchRecordDTO catchRecord in catchRecords)
            {
                catchRecord.CatchRecordFishes = catchRecordFishes[catchRecord.Id.Value].ToList();
                catchRecord.CatchRecordFishesSummary = string.Join(';', catchRecord.CatchRecordFishes.Select(x => $"{x.FishName} {x.QuantityKg}kg ({x.CatchQuadrant})"));
            }

            return catchRecords;
        }

        private ILookup<int, OriginDeclarationFishDTO> GetOriginDeclarationFishes(List<int> originDeclarationIds)
        {
            ILookup<int, OriginDeclarationFishDTO> originDeclarationFishes = (from originDeclarationFish in Db.OriginDeclarationFish
                                                                              join originDeclaration in Db.OriginDeclarations on originDeclarationFish.OriginDeclarationId equals originDeclaration.Id
                                                                              join catchRecordFish in Db.CatchRecordFish on originDeclarationFish.CatchRecordFishId equals catchRecordFish.Id into originCatchRecord
                                                                              from catchRecordFish in originCatchRecord.DefaultIfEmpty()
                                                                              join aquaticOrganism in Db.Nfishes on originDeclarationFish.FishId equals aquaticOrganism.Id
                                                                              join catchQuandrant in Db.NcatchZones on catchRecordFish.CatchZoneId equals catchQuandrant.Id into originCatchZone
                                                                              from catchQuandrant in originCatchZone.DefaultIfEmpty()
                                                                              join presentation in Db.NfishPresentations on originDeclarationFish.CatchFishPresentationId equals presentation.Id
                                                                              join state in Db.NfishFreshnesses on originDeclarationFish.CatchFishFreshnessId equals state.Id into catchState
                                                                              from state in catchState.DefaultIfEmpty()
                                                                              where originDeclarationIds.Contains(originDeclarationFish.OriginDeclarationId)
                                                                              select new
                                                                              {
                                                                                  LogBookPageId = originDeclaration.LogBookPageId,
                                                                                  OriginDeclarationFish = new OriginDeclarationFishDTO
                                                                                  {
                                                                                      Id = originDeclarationFish.Id,
                                                                                      OriginDeclarationId = originDeclarationFish.OriginDeclarationId,
                                                                                      CatchQuadrantId = catchRecordFish != null ? catchRecordFish.CatchZoneId : null,
                                                                                      CatchRecordFishId = catchRecordFish != null ? catchRecordFish.Id : null,
                                                                                      FishId = originDeclarationFish.FishId,
                                                                                      FishName = $"{aquaticOrganism.Code} - {aquaticOrganism.Name}",
                                                                                      CatchQuadrant = catchQuandrant != null ? catchQuandrant.Gfcmquadrant : "",
                                                                                      CatchZone = catchQuandrant != null ? catchQuandrant.ZoneNum.ToString() : "",
                                                                                      CatchFishPresentationId = originDeclarationFish.CatchFishPresentationId,
                                                                                      CatchFishPresentationName = presentation.Name,
                                                                                      CatchFishStateId = originDeclarationFish.CatchFishFreshnessId,
                                                                                      CatchFishStateName = state != null ? state.Name : "",
                                                                                      IsProcessedOnBoard = originDeclarationFish.IsProcessedOnBoard,
                                                                                      QuantityKg = originDeclarationFish.Quantity,
                                                                                      UnloadedProcessedQuantityKg = originDeclarationFish.UnloadedProcessedQuantity,
                                                                                      TransboardShipId = originDeclarationFish.TransboardShipId,
                                                                                      TransboardTargetPortId = originDeclarationFish.TransboardTargetPortId,
                                                                                      TransboradDateTime = originDeclarationFish.TransboardDateTime,
                                                                                      Comments = originDeclarationFish.Comments,
                                                                                      IsActive = originDeclarationFish.IsActive,
                                                                                      IsValid = true
                                                                                  }
                                                                              }).ToLookup(x => x.LogBookPageId, y => y.OriginDeclarationFish);

            return originDeclarationFishes;
        }

        private ILookup<int, FirstSaleLogBookPageRegisterDTO> GetShipFirstSaleDocuments(List<int> shipPagesIds)
        {
            ILookup<int, FirstSaleLogBookPageRegisterDTO> shipFirstSaleDocuments = (from firstSaleDoc in Db.FirstSaleLogBookPages
                                                                                    join originDeclaration in Db.OriginDeclarations on firstSaleDoc.OriginDeclarationId equals originDeclaration.Id
                                                                                    join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                                                                                    join buyer in Db.BuyerRegisters on firstSaleDoc.BuyerId equals buyer.Id
                                                                                    join legal in Db.Legals on buyer.SubmittedForLegalId equals legal.Id into subL
                                                                                    from legal in subL.DefaultIfEmpty()
                                                                                    join person in Db.Persons on buyer.SubmittedForPersonId equals person.Id into subP
                                                                                    from person in subP.DefaultIfEmpty()
                                                                                    where shipPagesIds.Contains(shipPage.Id)
                                                                                          && firstSaleDoc.IsActive
                                                                                          && firstSaleDoc.Status != nameof(LogBookPageStatusesEnum.Canceled)
                                                                                    select new
                                                                                    {
                                                                                        ShipPageId = shipPage.Id,
                                                                                        FirstSaleDocument = new FirstSaleLogBookPageRegisterDTO
                                                                                        {
                                                                                            Id = firstSaleDoc.Id,
                                                                                            PageNumber = firstSaleDoc.PageNum,
                                                                                            LogBookId = firstSaleDoc.LogBookId,
                                                                                            SaleDate = firstSaleDoc.SaleDate,
                                                                                            SaleLocation = firstSaleDoc.SaleLocation,
                                                                                            BuyerNames = legal != null
                                                                                                         ? legal.Name + " (" + legal.Eik + ") - " + buyer.RegistrationNum
                                                                                                         : person.FirstName + " " + person.LastName + " - " + buyer.RegistrationNum,
                                                                                            ProductsInformation = string.Join(';', from product in Db.LogBookPageProducts
                                                                                                                                   join fish in Db.Nfishes on product.FishId equals fish.Id
                                                                                                                                   where product.FirstSaleLogBookPageId == firstSaleDoc.Id
                                                                                                                                         && product.IsActive
                                                                                                                                   select " " + product.QuantityKg + "kg " + fish.Name
                                                                                                      ), // TODO add quantities where fish is the same
                                                                                            Status = Enum.Parse<LogBookPageStatusesEnum>(firstSaleDoc.Status),
                                                                                            CancellationReason = firstSaleDoc.CancelationReason,
                                                                                            IsActive = firstSaleDoc.IsActive
                                                                                        }
                                                                                    }).ToLookup(x => x.ShipPageId, y => y.FirstSaleDocument);

            return shipFirstSaleDocuments;
        }

        private ILookup<int, AdmissionLogBookPageRegisterDTO> GetShipAdmissionDeclarations(List<int> shipPagesIds)
        {
            ILookup<int, AdmissionLogBookPageRegisterDTO> shipAdmissionDeclarations = (from admissionDec in Db.AdmissionLogBookPages
                                                                                       join originDeclaration in Db.OriginDeclarations on admissionDec.OriginDeclarationId equals originDeclaration.Id
                                                                                       join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                                                                                       join logBook in Db.LogBooks on admissionDec.LogBookId equals logBook.Id
                                                                                       join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id into pageBuyer
                                                                                       from acceptingBuyer in pageBuyer.DefaultIfEmpty()
                                                                                       join acceptingBuyerLegal in Db.Legals on acceptingBuyer.SubmittedForLegalId equals acceptingBuyerLegal.Id into aBuyerLegal
                                                                                       from acceptingBuyerLegal in aBuyerLegal.DefaultIfEmpty()
                                                                                       join person in Db.Persons on logBook.PersonId equals person.Id into pagePerson
                                                                                       from acceptingPerson in pagePerson.DefaultIfEmpty()
                                                                                       join legal in Db.Legals on logBook.LegalId equals legal.Id into pageLegal
                                                                                       from acceptingLegal in pageLegal.DefaultIfEmpty()
                                                                                       where shipPagesIds.Contains(shipPage.Id)
                                                                                             && admissionDec.IsActive
                                                                                             && admissionDec.Status != nameof(LogBookPageStatusesEnum.Canceled)
                                                                                       select new
                                                                                       {
                                                                                           ShipPageId = shipPage.Id,
                                                                                           AdmissionDeclaration = new AdmissionLogBookPageRegisterDTO
                                                                                           {
                                                                                               Id = admissionDec.Id,
                                                                                               PageNumber = admissionDec.PageNum,
                                                                                               LogBookId = admissionDec.LogBookId,
                                                                                               HandoverDate = admissionDec.HandoverDate,
                                                                                               StorageLocation = admissionDec.StorageLocation,
                                                                                               AcceptedPersonNames = acceptingBuyer != null && acceptingBuyerLegal != null
                                                                                                                     ? acceptingBuyerLegal.Name
                                                                                                                     : acceptingPerson != null
                                                                                                                        ? acceptingPerson.FirstName + " " + acceptingPerson.LastName
                                                                                                                        : acceptingLegal.Name,
                                                                                               ProductsInformation = string.Join(';', from product in Db.LogBookPageProducts
                                                                                                                                      join fish in Db.Nfishes on product.FishId equals fish.Id
                                                                                                                                      where product.AdmissionLogBookPageId == admissionDec.Id
                                                                                                                                            && product.IsActive
                                                                                                                                      select " " + product.QuantityKg + "kg " + fish.Name
                                                                                                      ), // TODO add quantities where fish is the same
                                                                                               Status = Enum.Parse<LogBookPageStatusesEnum>(admissionDec.Status),
                                                                                               CancellationReason = admissionDec.CancelationReason,
                                                                                               IsActive = admissionDec.IsActive
                                                                                           }
                                                                                       }).ToLookup(x => x.ShipPageId, y => y.AdmissionDeclaration);

            return shipAdmissionDeclarations;
        }

        private ILookup<int, TransportationLogBookPageRegisterDTO> GetShipTransporatationDocuments(List<int> shipPagesIds)
        {
            ILookup<int, TransportationLogBookPageRegisterDTO> shipTransporatationDocuments = (from transportationDoc in Db.TransportationLogBookPages
                                                                                               join originDeclaration in Db.OriginDeclarations on transportationDoc.OriginDeclarationId equals originDeclaration.Id
                                                                                               join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                                                                                               join logBook in Db.LogBooks on transportationDoc.LogBookId equals logBook.Id
                                                                                               join buyer in Db.BuyerRegisters on logBook.RegisteredBuyerId equals buyer.Id into pageBuyer
                                                                                               from receiverBuyer in pageBuyer.DefaultIfEmpty()
                                                                                               join receiverBuyerLegal in Db.Legals on receiverBuyer.SubmittedForLegalId equals receiverBuyerLegal.Id into rBuyerLegal
                                                                                               from receiverBuyerLegal in rBuyerLegal.DefaultIfEmpty()
                                                                                               join person in Db.Persons on logBook.PersonId equals person.Id into pagePerson
                                                                                               from receiverPerson in pagePerson.DefaultIfEmpty()
                                                                                               join legal in Db.Legals on logBook.LegalId equals legal.Id into pageLegal
                                                                                               from receiverLegal in pageLegal.DefaultIfEmpty()
                                                                                               where shipPagesIds.Contains(shipPage.Id)
                                                                                                     && transportationDoc.IsActive
                                                                                                     && transportationDoc.Status != nameof(LogBookPageStatusesEnum.Canceled)
                                                                                               select new
                                                                                               {
                                                                                                   ShipPageId = shipPage.Id,
                                                                                                   TransportationDocument = new TransportationLogBookPageRegisterDTO
                                                                                                   {
                                                                                                       Id = transportationDoc.Id,
                                                                                                       LogBookId = transportationDoc.LogBookId,
                                                                                                       PageNumber = transportationDoc.PageNum,
                                                                                                       LoadingDate = transportationDoc.LoadingDate,
                                                                                                       DeliveryLocation = transportationDoc.DeliveryLocation,
                                                                                                       VehicleNumber = transportationDoc.VehicleNumber,
                                                                                                       RecieverName = receiverBuyer != null && receiverBuyerLegal != null
                                                                                                                      ? receiverBuyerLegal.Name
                                                                                                                      : receiverPerson != null ? receiverPerson.FirstName + " " + receiverPerson.LastName
                                                                                                                      : receiverLegal.Name,
                                                                                                       ProductsInformation = string.Join(';', from product in Db.LogBookPageProducts
                                                                                                                                              join fish in Db.Nfishes on product.FishId equals fish.Id
                                                                                                                                              where product.TransportationLogBookPageId == transportationDoc.Id
                                                                                                                                                    && product.IsActive
                                                                                                                                              select " " + product.QuantityKg + "kg " + fish.Name
                                                                                                           ), // TODO add quantities where fish is the same
                                                                                                       Status = Enum.Parse<LogBookPageStatusesEnum>(transportationDoc.Status),
                                                                                                       CancellationReason = transportationDoc.CancelationReason,
                                                                                                       IsActive = transportationDoc.IsActive
                                                                                                   }
                                                                                               }).ToLookup(x => x.ShipPageId, y => y.TransportationDocument);

            return shipTransporatationDocuments;
        }

        private void AddShipLogBookMissingPage(int logBookId, string pageNum, int logBookPermitLicenseId)
        {
            ShipLogBookPage entry = new ShipLogBookPage
            {
                LogBookId = logBookId,
                LogBookPermitLicenceId = logBookPermitLicenseId,
                PageNum = pageNum,
                Status = nameof(LogBookPageStatusesEnum.Missing)
            };

            Db.ShipLogBookPages.Add(entry);
        }

        private List<CatchRecord> AddOrEditCatchRecords(List<CatchRecordDTO> catchRecords, ShipLogBookPage dbLogBookPage)
        {
            List<CatchRecord> dbCatchRecords = new List<CatchRecord>();

            if (catchRecords != null)
            {
                foreach (CatchRecordDTO catchRecord in catchRecords)
                {
                    if (catchRecord.Id == null) // new entry
                    {
                        CatchRecord dbEntry = new CatchRecord
                        {
                            LogBookPage = dbLogBookPage,
                            CatchOperCount = catchRecord.CatchOperationsCount.Value,
                            Depth = catchRecord.Depth,
                            GearEntryTime = catchRecord.GearEntryTime.Value,
                            GearExitTime = catchRecord.GearExitTime.Value,
                            TransboardFromShipId = catchRecord.TransboardedFromShipId,
                            IsActive = true
                        };

                        dbEntry.CatchRecordFishes = AddOrEditCatchRecordFishes(catchRecord.CatchRecordFishes, dbEntry);

                        Db.CatchRecords.Add(dbEntry);

                        dbCatchRecords.Add(dbEntry);
                    }
                    else
                    {
                        CatchRecord dbEntry = (from cr in Db.CatchRecords.Include(x => x.CatchRecordFishes)
                                               where cr.Id == catchRecord.Id
                                               select cr).FirstOrDefault();

                        if (dbEntry != null)
                        {
                            dbEntry.CatchOperCount = catchRecord.CatchOperationsCount.Value;
                            dbEntry.Depth = catchRecord.Depth;
                            dbEntry.GearEntryTime = catchRecord.GearEntryTime.Value;
                            dbEntry.GearExitTime = catchRecord.GearExitTime.Value;
                            dbEntry.IsActive = catchRecord.IsActive.Value;
                            dbEntry.TransboardFromShipId = catchRecord.TransboardedFromShipId;

                            AddOrEditCatchRecordFishes(catchRecord.CatchRecordFishes, dbEntry);

                            dbCatchRecords.Add(dbEntry);
                        }
                    }
                }
            }
            return dbCatchRecords;
        }

        private void DeleteAnnulledShipLogBookPageCatches(int logBookPageId)
        {
            CatchRecord catchRecord = (from cr in Db.CatchRecords
                                       where cr.LogBookPageId == logBookPageId && cr.IsActive
                                       select cr).FirstOrDefault();

            if (catchRecord != null)
            {
                catchRecord.IsActive = false;
                DeleteShipLogBookPageCatchFishes(catchRecord.Id);
            }
        }

        private void DeleteShipLogBookPageCatchFishes(int catchRecordId)
        {
            List<CatchRecordFish> catchRecordFishes = (from crFish in Db.CatchRecordFish
                                                       where crFish.CatchRecordId == catchRecordId
                                                       select crFish).ToList();

            foreach (CatchRecordFish catchRecordFish in catchRecordFishes)
            {
                catchRecordFish.IsActive = false;
            }
        }

        private List<CatchRecordFish> AddOrEditCatchRecordFishes(List<CatchRecordFishDTO> fishes, CatchRecord dbCatchRecord)
        {
            List<CatchRecordFish> dbCatchRecordFishes = new List<CatchRecordFish>();

            if (fishes != null)
            {
                foreach (CatchRecordFishDTO catchFish in fishes)
                {
                    if (catchFish.Id == null) // new fish
                    {
                        CatchRecordFish dbEntry = new CatchRecordFish
                        {
                            CatchRecord = dbCatchRecord,
                            IsActive = true
                        };

                        MapDTOCatchRecordToDB(catchFish, dbEntry);

                        Db.CatchRecordFish.Add(dbEntry);

                        dbCatchRecordFishes.Add(dbEntry);
                    }
                    else
                    {
                        CatchRecordFish dbEntry = (from crFish in Db.CatchRecordFish
                                                   where crFish.Id == catchFish.Id
                                                   select crFish).FirstOrDefault();

                        if (dbEntry != null)
                        {
                            dbEntry.IsActive = catchFish.IsActive.Value;
                            MapDTOCatchRecordToDB(catchFish, dbEntry);

                            dbCatchRecordFishes.Add(dbEntry);
                        }
                    }
                }
            }

            return dbCatchRecordFishes;
        }

        private void AddOrEditOriginDeclaration(ShipLogBookPage logBookPage,
                                                int? originDeclarationId,
                                                List<OriginDeclarationFishDTO> originDeclarationFishes,
                                                List<CatchRecordFish> dbCatchRecordFishes,
                                                int? unloadPortId = null,
                                                DateTime? unloadDateTime = null)
        {
            if (originDeclarationFishes != null && originDeclarationFishes.Any())
            {
                OriginDeclaration dbEntry;
                if (originDeclarationId == null) // new origin declaration
                {
                    dbEntry = new OriginDeclaration
                    {
                        LogBookPage = logBookPage,
                        IsActive = true
                    };

                    Db.OriginDeclarations.Add(dbEntry);
                }
                else
                {
                    dbEntry = (from originDeclaration in Db.OriginDeclarations
                               where originDeclaration.Id == originDeclarationId
                               select originDeclaration).First();
                }


                foreach (OriginDeclarationFishDTO originDeclarationFish in originDeclarationFishes)
                {
                    CatchRecordFish dbCatchRecordFish = null;

                    if (originDeclarationFish.CatchRecordFishId.HasValue)
                    {
                        bool isCatchRecordInCurrentTrip = dbCatchRecordFishes.Where(x => x.Id == originDeclarationFish.CatchRecordFishId.Value)
                                                                             .Any();
                        if (!isCatchRecordInCurrentTrip)
                        {
                            dbCatchRecordFish = (from catchRecordFish in Db.CatchRecordFish
                                                                           .AsSplitQuery()
                                                                           .Include(x => x.OriginDeclarationFishes)
                                                 where catchRecordFish.Id == originDeclarationFish.CatchRecordFishId
                                                 select catchRecordFish).First();
                        }
                    }

                    if (dbCatchRecordFish == null)
                    {
                        dbCatchRecordFish = dbCatchRecordFishes.OrderBy(x => x.Quantity)
                                                               .Where(x => (!originDeclarationFish.CatchRecordFishId.HasValue || x.Id == originDeclarationFish.CatchRecordFishId)
                                                                           && x.FishId == originDeclarationFish.FishId
                                                                           && x.CatchZoneId == originDeclarationFish.CatchQuadrantId
                                                                           && x.Quantity - x.UnloadedQuantity >= originDeclarationFish.QuantityKg)
                                                               .FirstOrDefault();

                        if (dbCatchRecordFish == null) // in this case there is probably more unloaded quantity reported than caught
                        {
                            List<CatchRecordFish> dbCatchRecordFishesNotUnloaded = dbCatchRecordFishes.Where(x => (!originDeclarationFish.CatchRecordFishId.HasValue
                                                                                                                    || x.Id == originDeclarationFish.CatchRecordFishId)
                                                                                                                   && x.FishId == originDeclarationFish.FishId
                                                                                                                   && x.CatchZoneId == originDeclarationFish.CatchQuadrantId
                                                                                                                   && x.Quantity - x.UnloadedQuantity != 0).ToList();

                            if (dbCatchRecordFishesNotUnloaded.Any())
                            {
                                dbCatchRecordFish = dbCatchRecordFishesNotUnloaded.OrderByDescending(x => x.Quantity - x.UnloadedQuantity).First();
                            }
                            else
                            {

                                dbCatchRecordFish = dbCatchRecordFishes.Where(x => (!originDeclarationFish.CatchRecordFishId.HasValue || x.Id == originDeclarationFish.CatchRecordFishId)
                                                                                   && x.FishId == originDeclarationFish.FishId
                                                                                   && x.CatchZoneId == originDeclarationFish.CatchQuadrantId)
                                                                       .First();
                            }
                        }
                    }

                    if (originDeclarationFish.Id == null) // new fish
                    {
                        OriginDeclarationFish dbOriginFish = new OriginDeclarationFish
                        {
                            OriginDeclaration = dbEntry,
                            IsActive = true
                        };

                        MapDTOOriginDeclarationFishToDB(originDeclarationFish, dbOriginFish, dbCatchRecordFish, unloadPortId, unloadDateTime);
                        Db.OriginDeclarationFish.Add(dbOriginFish);
                    }
                    else // update
                    {
                        OriginDeclarationFish dbOriginFish = (from originFish in Db.OriginDeclarationFish
                                                              where originFish.Id == originDeclarationFish.Id
                                                              select originFish).FirstOrDefault();

                        if (dbOriginFish != null)
                        {
                            dbOriginFish.IsActive = originDeclarationFish.IsActive.Value;
                            MapDTOOriginDeclarationFishToDB(originDeclarationFish, dbOriginFish, dbCatchRecordFish, unloadPortId, unloadDateTime);
                        }
                    }

                    UpdateCatchRecordFishUnloadedQuantity(dbCatchRecordFish, dbCatchRecordFish.OriginDeclarationFishes);
                }
            }
        }

        private void DeleteAnnulledShipLogBookPageOriginDeclaration(int logBookPageId)
        {
            OriginDeclaration originDeclaration = (from originDec in Db.OriginDeclarations
                                                   where originDec.LogBookPageId == logBookPageId
                                                         && originDec.IsActive
                                                   select originDec).FirstOrDefault();
            if (originDeclaration != null)
            {
                originDeclaration.IsActive = false;
                DeleteLogBookPageOriginDeclarationFishes(originDeclaration.Id);
            }
        }

        private void DeleteLogBookPageOriginDeclarationFishes(int originDeclarationId)
        {
            List<OriginDeclarationFish> originDeclarationFishes = (from originDecFish in Db.OriginDeclarationFish
                                                                   where originDecFish.OriginDeclarationId == originDeclarationId
                                                                   select originDecFish).ToList();

            foreach (OriginDeclarationFish originDeclarationFish in originDeclarationFishes)
            {
                originDeclarationFish.IsActive = false;
            }
        }

        private void UpdateCatchRecordFishUnloadedQuantity(CatchRecordFish dbEntry, ICollection<OriginDeclarationFish> originDeclarationFishes)
        {
            dbEntry.UnloadedQuantity = originDeclarationFishes.Where(x => x.IsActive).Sum(x => x.Quantity);
        }

        private HashSet<decimal> GetAlreadySubmittedShipPages(long startPage, long endPage)
        {
            HashSet<decimal> alreadySubmittedPageNumbers = (from shipPage in Db.ShipLogBookPages
                                                            join lb in Db.LogBooks on shipPage.LogBookId equals lb.Id
                                                            where !lb.IsOnline && EF.Functions.Like("^[^-*]+$", shipPage.PageNum)
                                                                  && Convert.ToDecimal(shipPage.PageNum) >= startPage
                                                                  && Convert.ToDecimal(shipPage.PageNum) < endPage
                                                            select Convert.ToDecimal(shipPage.PageNum)).ToHashSet();

            return alreadySubmittedPageNumbers;
        }

        private void MapDTOCatchRecordToDB(CatchRecordFishDTO dto, CatchRecordFish dbEntry)
        {
            dbEntry.FishId = dto.FishId.Value;
            dbEntry.CatchZoneId = dto.CatchQuadrantId.Value;
            dbEntry.CatchTypeId = dto.CatchTypeId.Value;
            dbEntry.CatchSizeId = dto.CatchSizeId.Value;
            dbEntry.IsContinentalCatch = dto.IsContinentalCatch.Value;
            dbEntry.Quantity = dto.QuantityKg.Value;
            dbEntry.ThirdCountryCatchZone = dto.ThirdCountryCatchZone;

            if (dto.SturgeonGender != null)
            {
                dbEntry.SturgeonGender = dto.SturgeonGender.ToString();
                dbEntry.SturgeonSize = dto.SturgeonSize;
                dbEntry.SturgeonWeightKg = dto.SturgeonWeightKg;

                dbEntry.TurbotSizeGroupId = null;
                dbEntry.TurbotCount = null;
            }
            else if (dto.TurbotSizeGroupId != null)
            {
                dbEntry.TurbotSizeGroupId = dto.TurbotSizeGroupId;
                dbEntry.TurbotCount = dto.TurbotCount;

                dbEntry.SturgeonGender = null;
                dbEntry.SturgeonSize = null;
                dbEntry.SturgeonWeightKg = null;
            }
            else
            {
                dbEntry.TurbotSizeGroupId = null;
                dbEntry.TurbotCount = null;
                dbEntry.SturgeonGender = null;
                dbEntry.SturgeonSize = null;
                dbEntry.SturgeonWeightKg = null;
            }
        }

        private void MapDTOOriginDeclarationFishToDB(OriginDeclarationFishDTO dto,
                                                     OriginDeclarationFish dbEntry,
                                                     CatchRecordFish dbCatchRecordFish,
                                                     int? unloadPortId = null,
                                                     DateTime? unloadDateTime = null)
        {
            DateTime now = DateTime.Now;

            if (dbEntry.Id == default)
            {
                dbCatchRecordFish.OriginDeclarationFishes.Add(dbEntry);
            }

            dbEntry.CatchFishPresentationId = dto.CatchFishPresentationId.Value;
            dbEntry.CatchFishFreshnessId = dto.CatchFishStateId.Value;
            dbEntry.Comments = dto.Comments;
            dbEntry.IsProcessedOnBoard = dto.IsProcessedOnBoard.Value;
            dbEntry.Quantity = dto.QuantityKg.Value;
            dbEntry.UnloadedProcessedQuantity = dto.UnloadedProcessedQuantityKg;
            dbEntry.FishId = dto.FishId.Value;

            UnloadingTypesEnum unloadingType;

            if (dto.TransboardTargetPortId.HasValue)
            {
                unloadingType = UnloadingTypesEnum.TRN;

                dbEntry.TransboardDateTime = dto.TransboradDateTime.Value;
                dbEntry.TransboardTargetPortId = dto.TransboardTargetPortId.Value;
                dbEntry.TransboardShipId = dto.TransboardShipId.Value;

                dbEntry.UnloadDateTime = null;
                dbEntry.UnloadPortId = null;
            }
            else
            {
                unloadingType = UnloadingTypesEnum.UNL;

                dbEntry.UnloadDateTime = unloadDateTime;
                dbEntry.UnloadPortId = unloadPortId;

                dbEntry.TransboardDateTime = null;
                dbEntry.TransboardTargetPortId = null;
                dbEntry.TransboardShipId = null;
            }

            dbEntry.UnloadTypeId = (from unloadType in Db.NcatchFishUnloadTypes
                                    where unloadType.Code == unloadingType.ToString()
                                          && unloadType.ValidFrom <= now
                                          && unloadType.ValidTo > now
                                    select unloadType.Id).First();
        }
    }

    public class ShipLogBookPageRegisterHelper : ShipLogBookPageRegisterDTO
    {
        public int TerritoryUnitId { get; set; }

        public string StatusCode { get; set; }

        public int LogBookTypeId { get; set; }

        public string LogBookNumber { get; set; }
    }
}
