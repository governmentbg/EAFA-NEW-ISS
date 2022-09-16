using System;
using System.Linq;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;

namespace IARA.Infrastructure.Helpers
{
    internal static class CatchSalesHelper
    {
        /// <summary>
        /// Adds Ship log book for a permit license
        /// Throws InvalidLogBookPagesRangeException if page range overlaps with a page range of an existing aquaculture log book.
        /// </summary>
        /// <param name="db">IARADbContext instance</param>
        /// <param name="logBook">DTO model of the log book</param>
        /// <param name="shipId">ID of the ship</param>
        /// <returns></returns>
        public static LogBook AddShipLogBook(this IARADbContext db, CommercialFishingLogBookEditDTO logBook, bool ignoreLogBookConflicts, int? shipId, int? personId, int? legalId)
        {
            if (!ignoreLogBookConflicts && !logBook.IsOnline.Value && IsShipPageRangeInvalid(db, logBook))
            {
                throw new InvalidLogBookLicensePagesRangeException(logBook.LogbookNumber);
            }

            return AddLogBook(db, logBook, shipId: shipId, personId: personId, legalId: legalId);
        }

        /// <summary>
        /// Adds Aquaculture log book.
        /// Throws InvalidLogBookPagesRangeException if page range overlaps with a page range of an existing aquaculture log book.
        /// </summary>
        /// <param name="db">IARADbContext instance</param>
        /// <param name="logBook">DTO model of the log book</param>
        /// <param name="aquacultureId">ID of the aquaculture facility</param>
        /// <returns></returns>
        public static LogBook AddAquacultureLogBook(this IARADbContext db, LogBookEditDTO logBook, int aquacultureId, bool ignoreLogBookConflicts)
        {
            if (!ignoreLogBookConflicts && !logBook.IsOnline.Value && IsPagesRangeInvalid(db, logBook))
            {
                throw new InvalidLogBookPagesRangeException(logBook.LogbookNumber);
            }

            return AddLogBook(db, logBook, aquacultureId: aquacultureId);
        }

        /// <summary>
        /// Adds First sale log book.
        /// Throws InvalidLogBookPagesRangeException if page range overlaps with a page range of an existing aquaculture log book.
        /// </summary>
        /// <param name="db">IARADbContext instance</param>
        /// <param name="logBook">DTO model of the log book</param>
        /// <param name="buyerId">ID of registered buyer or first sale center</param>
        /// <returns></returns>
        public static LogBook AddFirstSaleLogBook(this IARADbContext db, LogBookEditDTO logBook, int buyerId, bool ignoreLogBookConflicts)
        {
            if (!ignoreLogBookConflicts && !logBook.IsOnline.Value && IsPagesRangeInvalid(db, logBook))
            {
                throw new InvalidLogBookPagesRangeException(logBook.LogbookNumber);
            }

            return AddLogBook(db, logBook, buyerId: buyerId);
        }

        /// <summary>
        /// Adds Admission log book.
        /// Throws InvalidLogBookPagesRangeException if page range overlaps with a page range of an existing aquaculture log book.
        /// </summary>
        /// <param name="db">IARADbContext instance</param>
        /// <param name="logBook">DTO model of the log book</param>
        /// <param name="buyerId">ID of registered buyer or first sale center, if buyer/FSC is owner of log book</param>
        /// <param name="legalId">ID of legal, if legal owner of log book</param>
        /// <param name="personId">ID of person, if person owner of log book</param>
        /// <returns></returns>
        public static LogBook AddAdmissionLogBook(this IARADbContext db, LogBookEditDTO logBook, bool ignoreLogBookConflicts, int? buyerId, int? legalId, int? personId)
        {
            if (!ignoreLogBookConflicts && !logBook.IsOnline.Value && IsPagesRangeInvalid(db, logBook))
            {
                throw new InvalidLogBookPagesRangeException(logBook.LogbookNumber);
            }

            return AddLogBook(db, logBook, buyerId: buyerId, legalId: legalId, personId: personId);
        }

        /// <summary>
        /// Adds Transportation log book.
        /// Throws InvalidLogBookPagesRangeException if page range overlaps with a page range of an existing aquaculture log book.
        /// </summary>
        /// <param name="db">IARADbContext instance</param>
        /// <param name="logBook">DTO model of the log book</param>
        /// <param name="buyerId">ID of registered buyer or first sale center, if buyer/FSC is owner of log book</param>
        /// <param name="legalId">ID of legal, if legal owner of log book</param>
        /// <param name="personId">ID of person, if person owner of log book</param>
        /// <returns></returns>
        public static LogBook AddTransportationLogBook(this IARADbContext db, LogBookEditDTO logBook, bool ignoreLogBookConflicts, int? buyerId, int? legalId, int? personId)
        {
            if (!ignoreLogBookConflicts && !logBook.IsOnline.Value && IsPagesRangeInvalid(db, logBook))
            {
                throw new InvalidLogBookPagesRangeException(logBook.LogbookNumber);
            }

            return AddLogBook(db, logBook, buyerId: buyerId, legalId: legalId, personId: personId);
        }

        /// <summary>
        /// Edits commercial fishing log book (for ship, person or legal)
        /// Throws InvalidLogBookLicensePagesRangeException if page range overlaps with a page range of an existing aquaculture log book.
        /// </summary>
        /// <param name="db">IARADbContext instance</param>
        /// <param name="logBook">DTO model of the log book</param>
        /// <returns>Editted log book entity</returns>
        public static LogBook EditShipLogBook(this IARADbContext db, CommercialFishingLogBookEditDTO logBook, bool ignoreLogBookConflicts)
        {
            if (!ignoreLogBookConflicts && !logBook.IsOnline.Value && logBook.PermitLicenseIsActive && IsShipPageRangeInvalid(db, logBook))
            {
                throw new InvalidLogBookLicensePagesRangeException(logBook.LogbookNumber);
            }

            return EditLogBook(db, logBook);
        }

        /// <summary>
        /// Edits registered buyer log book - first sale, admission, transportation.
        /// Throws InvalidLogBookPagesRangeException if page range overlaps with a page range of an existing aquaculture log book.
        /// </summary>
        /// <param name="db">IARADbContext instance</param>
        /// <param name="logBook">DTO model of the log book</param>
        /// <returns>Editted log book entity</returns>
        public static LogBook EditRegisteredBuyerLogBook(this IARADbContext db, LogBookEditDTO logBook, bool ignoreLogBookConflicts)
        {
            if (!ignoreLogBookConflicts && IsPagesRangeInvalid(db, logBook) && logBook.LogBookIsActive)
            {
                throw new InvalidLogBookPagesRangeException(logBook.LogbookNumber);
            }

            return EditLogBook(db, logBook);
        }

        /// <summary>
        /// Edits aquaculture log book.
        /// Throws InvalidLogBookPagesRangeException if page range overlaps with a page range of an existing aquaculture log book.
        /// </summary>
        /// <param name="db">IARADbContext instance</param>
        /// <param name="logBook">DTO model of the log book</param>
        /// <returns>Editted log book entity</returns>
        public static LogBook EditAquacultureLogBook(this IARADbContext db, LogBookEditDTO logBook, bool ignoreLogBookConflicts)
        {
            if (!ignoreLogBookConflicts && IsPagesRangeInvalid(db, logBook) && logBook.LogBookIsActive)
            {
                throw new InvalidLogBookPagesRangeException(logBook.LogbookNumber);
            }

            return EditLogBook(db, logBook);
        }

        /// <summary>
        /// Edits log book.
        /// Throws InvalidLogBookPagesRangeException if page range overlaps with a page range of an existing aquaculture log book.
        /// </summary>
        /// <param name="db">IARADbContext instance</param>
        /// <param name="logBook">DTO model of the log book</param>
        /// <returns>Editted log book entity</returns>
        private static LogBook EditLogBook(this IARADbContext db, LogBookEditDTO logBook)
        {
            LogBook dbLogBook = (from lb in db.LogBooks
                                 where lb.Id == logBook.LogBookId.Value
                                 select lb).Single();

            dbLogBook.IssueDate = logBook.IssueDate.Value;
            dbLogBook.FinishDate = logBook.FinishDate;
            dbLogBook.StartPageNum = logBook.StartPageNumber.Value;
            dbLogBook.EndPageNum = logBook.EndPageNumber.Value;
            dbLogBook.Comments = logBook.Comment;
            dbLogBook.Price = logBook.Price;
            dbLogBook.StatusId = logBook.StatusId.Value;
            dbLogBook.IsActive = logBook.LogBookIsActive;

            return dbLogBook;
        }

        private static LogBook AddLogBook(
            this IARADbContext db,
            LogBookEditDTO logBook,
            int? shipId = null,
            int? aquacultureId = null,
            int? buyerId = null,
            int? personId = null,
            int? legalId = null)
        {
            LogBook logBookEntry = new LogBook
            {
                LogBookTypeId = logBook.LogBookTypeId,
                LogBookOwnerType = logBook.OwnerType.HasValue ? logBook.OwnerType.Value.ToString() : null,
                ShipId = shipId,
                AquacultureFacilityId = aquacultureId,
                IsOnline = logBook.IsOnline.Value,
                IssueDate = logBook.IssueDate.Value,
                FinishDate = logBook.FinishDate,
                StartPageNum = logBook.StartPageNumber.Value,
                EndPageNum = logBook.EndPageNumber.Value,
                Comments = logBook.Comment,
                Price = logBook.Price,
                StatusId = logBook.StatusId.Value,
                LastPageNum = 0,
                IsActive = true
            };

            switch (logBook.OwnerType)
            {
                case LogBookPagePersonTypesEnum.RegisteredBuyer:
                    {
                        logBookEntry.RegisteredBuyerId = buyerId.Value;
                    }
                    break;
                case LogBookPagePersonTypesEnum.Person:
                    {
                        logBookEntry.PersonId = personId.Value;
                    }
                    break;
                case LogBookPagePersonTypesEnum.LegalPerson:
                    {
                        logBookEntry.LegalId = legalId.Value;
                    }
                    break;
            }

            db.LogBooks.Add(logBookEntry);

            return logBookEntry;
        }

        /// <summary>
        /// Only for paper ship log books. 
        /// For Ship log books and for Person and Legal log books - Searches in LogBookPermitLicenses table for overlapping ranges with the one of the current log book.
        /// </summary>
        /// <param name="db">Instance of db context to work with</param>
        /// <param name="logBook">DTO model with data for the log book to be added/editted</param>
        /// <returns></returns>
        private static bool IsShipPageRangeInvalid(this IARADbContext db, CommercialFishingLogBookEditDTO logBook)
        {
            bool result = (from logBookPermitLicense in db.LogBookPermitLicenses
                           join lb in db.LogBooks on logBookPermitLicense.LogBookId equals lb.Id
                           join status in db.NlogBookStatuses on lb.StatusId equals status.Id
                           where logBookPermitLicense.LogBookId != logBook.LogBookId
                                 && logBookPermitLicense.StartPageNum.HasValue
                                 && logBookPermitLicense.EndPageNum.HasValue
                                 && !(logBook.PermitLicenseEndPageNumber < logBookPermitLicense.StartPageNum || logBookPermitLicense.EndPageNum < logBook.PermitLicenseStartPageNumber)
                                 && lb.LogBookTypeId == logBook.LogBookTypeId
                                 && !lb.IsOnline
                                 && lb.IsActive
                                 && status.Code != nameof(LogBookStatusesEnum.Finished)
                           select lb.Id).Any();

            return result;
        }

        /// <summary>
        /// Only for paper log books. 
        /// For Buyer log books - Searches in LogBooks table for overlapping ranges with the one of the current log book.
        /// </summary>
        /// <param name="db">Instance of db context to work with</param>
        /// <param name="logBook">DTO model with data for the log book to be added/editted</param>
        /// <returns></returns>
        private static bool IsPagesRangeInvalid(this IARADbContext db, LogBookEditDTO logBook)
        {
            bool result = (from lb in db.LogBooks
                           join status in db.NlogBookStatuses on lb.StatusId equals status.Id
                           where lb.Id != logBook.LogBookId
                                 && !(logBook.EndPageNumber < lb.StartPageNum || lb.EndPageNum < logBook.StartPageNumber)
                                 && lb.LogBookTypeId == logBook.LogBookTypeId
                                 && !lb.IsOnline
                                 && lb.IsActive
                                 && status.Code != nameof(LogBookStatusesEnum.Finished)
                           select lb.Id).Any();
            return result;
        }
    }
}
