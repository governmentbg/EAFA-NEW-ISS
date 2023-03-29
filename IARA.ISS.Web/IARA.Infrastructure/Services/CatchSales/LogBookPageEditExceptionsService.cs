using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.CatchSales;
using NetTopologySuite.Operation.Valid;

namespace IARA.Infrastructure.Services.CatchSales
{
    public class LogBookPageEditExceptionsService : Service, ILogBookPageEditExceptionsService
    {
        public LogBookPageEditExceptionsService(IARADbContext dbContext)
            : base(dbContext)
        { }

        public IQueryable<LogBookPageEditExceptionRegisterDTO> GetAllLogBookPageEditExceptions(LogBookPageEditExceptionFilters filters)
        {
            IQueryable<LogBookPageEditExceptionRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllLogBookPageEditExceptions(showInactive);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredLogBookPageEditExceptions(filters.FreeTextSearch, filters.ShowInactiveRecords)
                    : GetParametersFilteredLogBookPageEditExceptions(filters);
            }

            return result;
        }

        public LogBookPageEditExceptionEditDTO GetLogBookPageEditException(int id)
        {
            LogBookPageEditExceptionEditDTO result = (from exception in Db.LogBookPageEditExceptions
                                                      where exception.Id == id
                                                      select new LogBookPageEditExceptionEditDTO
                                                      {
                                                          Id = exception.Id,
                                                          UserId = exception.UserId,
                                                          LogBookTypeId = exception.LogBookTypeId,
                                                          LogBookId = exception.LogBookId,
                                                          ExceptionActiveFrom = exception.ExceptionActiveFrom,
                                                          ExceptionActiveTo = exception.ExceptionActiveTo,
                                                          EditPageFrom = exception.EditPageDateFrom,
                                                          EditPageTo = exception.EditPageDateTo
                                                      }).First();

            return result;
        }

        public void AddOrEditLogBookPageEditException(LogBookPageEditExceptionEditDTO model)
        {
            CheckAndThrowIfExceptionCombinationExists(model);
            LogBookPageEditException dbEntry;

            if (model.Id == default) // Add
            {
                dbEntry = (from exception in Db.LogBookPageEditExceptions
                           where exception.UserId == model.UserId
                                 && exception.LogBookTypeId == model.LogBookTypeId
                                 && exception.LogBookId == model.LogBookId
                                 && exception.ExceptionActiveFrom == model.ExceptionActiveFrom.Value
                                 && !exception.IsActive
                           select exception).SingleOrDefault();

                if (dbEntry == null)
                {
                    dbEntry = new LogBookPageEditException();
                }
                else // combination exists but was inactive
                {
                    dbEntry.IsActive = true;
                }
            }
            else // Edit
            {
                dbEntry = (from exception in Db.LogBookPageEditExceptions
                           where exception.Id == model.Id
                           select exception).First();
            }

            dbEntry.UserId = model.UserId;
            dbEntry.LogBookTypeId = model.LogBookTypeId;
            dbEntry.LogBookId = model.LogBookId;
            dbEntry.ExceptionActiveFrom = model.ExceptionActiveFrom.Value;
            dbEntry.ExceptionActiveTo = model.ExceptionActiveTo.Value;
            dbEntry.EditPageDateFrom = model.EditPageFrom.Value;
            dbEntry.EditPageDateTo = model.EditPageTo.Value;

            if (model.Id == default) // Add
            {
                Db.LogBookPageEditExceptions.Add(dbEntry);
            }

            Db.SaveChanges();
        }

        public void DeleteLogBookPageEditException(int id)
        {
            // TODO checks - cannot delete maybe if there is already a page that was added/editted because of this exception ???
            DeleteRecordWithId(Db.LogBookPageEditExceptions, id);
            Db.SaveChanges();
        }

        public void RestoreLogBookPageEditException(int id)
        {
            UndoDeleteRecordWithId(Db.LogBookPageEditExceptions, id);
            Db.SaveChanges();
        }

        // Nomenclatures

        public List<SystemUserNomenclatureDTO> GetAllUsersNomenclature()
        {
            DateTime now = DateTime.Now;

            List<SystemUserNomenclatureDTO> results = (from user in Db.Users
                                                       join person in Db.Persons on user.PersonId equals person.Id
                                                       join latestPerson in Db.Persons on new
                                                       {
                                                           person.EgnLnc,
                                                           person.IdentifierType
                                                       } equals new
                                                       {
                                                           latestPerson.EgnLnc,
                                                           latestPerson.IdentifierType
                                                       }
                                                       where latestPerson.ValidFrom <= now
                                                             && latestPerson.ValidTo > now
                                                       orderby latestPerson.FirstName, latestPerson.LastName, user.Email
                                                       select new SystemUserNomenclatureDTO
                                                       {
                                                           Value = user.Id,
                                                           DisplayName = $"{latestPerson.FirstName} {latestPerson.LastName} ({user.Email})",
                                                           IsInternalUser = user.IsInternalUser,
                                                           IsActive = user.ValidFrom <= now && user.ValidTo > now
                                                       }).ToList();

            return results;
        }

        public List<LogBookPageEditNomenclatureDTO> GetActiveLogBooksNomenclature(int? logBookPageEditExceptionId)
        {
            HashSet<int> inactiveLogBookIds = (from exception in Db.LogBookPageEditExceptions
                                               join logBook in Db.LogBooks on exception.LogBookId equals logBook.Id
                                               join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                                               where (logBookPageEditExceptionId == null || exception.LogBookId == logBookPageEditExceptionId)
                                                     && (logBookStatus.Code == nameof(LogBookStatusesEnum.SuspLic) || logBookStatus.Code == nameof(LogBookStatusesEnum.Finished))
                                               select logBook.Id).ToHashSet();

            List<LogBookPageEditNomenclatureDTO> results = (from logBook in Db.LogBooks
                                                            join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                                                            join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                                            where inactiveLogBookIds.Contains(logBook.Id)
                                                                  || logBookStatus.Code == nameof(LogBookStatusesEnum.New)
                                                                  || logBookStatus.Code == nameof(LogBookStatusesEnum.Renewed)
                                                            orderby logBookType.Name, logBook.LogNum
                                                            select new LogBookPageEditNomenclatureDTO
                                                            {
                                                                Value = logBook.Id,
                                                                LogBookTypeCode = logBookType.Code,
                                                                DisplayName = $"{logBookType.Name} - {logBook.LogNum}",
                                                                IsActive = (logBookStatus.Code == nameof(LogBookStatusesEnum.New) || logBookStatus.Code == nameof(LogBookStatusesEnum.Renewed))
                                                                           && logBook.IsActive
                                                            }).ToList();

            return results;
        }

        // Audits

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.LogBookPageEditExceptions, id);
        }

        private IQueryable<LogBookPageEditExceptionRegisterDTO> GetAllLogBookPageEditExceptions(bool showInactive)
        {
            DateTime now = DateTime.Now;

            var result = from exception in Db.LogBookPageEditExceptions
                         join user in Db.Users on exception.UserId equals user.Id into leftUser
                         from user in leftUser.DefaultIfEmpty()
                         join person in Db.Persons on user.PersonId equals person.Id into leftPerson
                         from person in leftPerson.DefaultIfEmpty()
                         join latestPerson in Db.Persons on new
                         {
                             person.EgnLnc,
                             person.IdentifierType
                         } equals new
                         {
                             latestPerson.EgnLnc,
                             latestPerson.IdentifierType
                         } into leftLatestPerson
                         from latestPerson in leftLatestPerson.DefaultIfEmpty()
                         join logBookType in Db.NlogBookTypes on exception.LogBookTypeId equals logBookType.Id into leftLogBookType
                         from logBookType in leftLogBookType.DefaultIfEmpty()
                         join logBook in Db.LogBooks on exception.LogBookId equals logBook.Id into leftLogBook
                         from logBook in leftLogBook.DefaultIfEmpty()
                         where exception.IsActive == !showInactive
                               && (latestPerson == null || (latestPerson.ValidFrom <= now && latestPerson.ValidTo > now))
                         orderby exception.ExceptionActiveTo descending
                         select new LogBookPageEditExceptionRegisterDTO
                         {
                             Id = exception.Id,
                             UserNames = latestPerson != null
                                         ? latestPerson.FirstName + " " + latestPerson.LastName + " (" + user.Email + ")"
                                         : null,
                             LogBookTypeName = logBookType != null
                                               ? logBookType.Name
                                               : null,
                             LogBookNumber = logBook != null
                                             ? logBook.LogNum
                                             : null,
                             IsValidNow = exception.ExceptionActiveFrom <= now && exception.ExceptionActiveTo > now,
                             ExceptionActiveFrom = exception.ExceptionActiveFrom,
                             ExceptionActiveTo = exception.ExceptionActiveTo,
                             EditPageFrom = exception.EditPageDateFrom,
                             EditPageTo = exception.EditPageDateTo,
                             IsActive = exception.IsActive
                         };

            return result;
        }

        private IQueryable<LogBookPageEditExceptionRegisterDTO> GetFreeTextFilteredLogBookPageEditExceptions(string text, bool showInactive)
        {
            DateTime now = DateTime.Now;
            text = text.ToLower();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            var result = from exception in Db.LogBookPageEditExceptions
                         join user in Db.Users on exception.UserId equals user.Id into leftUser
                         from user in leftUser.DefaultIfEmpty()
                         join person in Db.Persons on user.PersonId equals person.Id into leftPerson
                         from person in leftPerson.DefaultIfEmpty()
                         join latestPerson in Db.Persons on new
                         {
                             person.EgnLnc,
                             person.IdentifierType
                         } equals new
                         {
                             latestPerson.EgnLnc,
                             latestPerson.IdentifierType
                         } into leftLatestPerson
                         from latestPerson in leftLatestPerson.DefaultIfEmpty()
                         join logBookType in Db.NlogBookTypes on exception.LogBookTypeId equals logBookType.Id into leftLogBookType
                         from logBookType in leftLogBookType.DefaultIfEmpty()
                         join logBook in Db.LogBooks on exception.LogBookId equals logBook.Id into leftLogBook
                         from logBook in leftLogBook.DefaultIfEmpty()
                         where exception.IsActive == !showInactive
                               && (latestPerson == null || (latestPerson.ValidFrom <= now && latestPerson.ValidTo > now))
                               && ((latestPerson != null && (latestPerson.FirstName + " " + latestPerson.LastName + " (" + user.Email + ")").ToLower().Contains(text))
                                    || (logBookType != null && logBookType.Name.ToLower().Contains(text))
                                    || (logBook != null && logBook.LogNum.ToLower().Contains(text))
                                    || (searchDate.HasValue && exception.ExceptionActiveFrom == searchDate.Value)
                                    || (searchDate.HasValue && exception.ExceptionActiveTo == searchDate.Value)
                                    || (searchDate.HasValue && exception.EditPageDateFrom == searchDate.Value)
                                    || (searchDate.HasValue && exception.EditPageDateTo == searchDate.Value))
                         orderby exception.ExceptionActiveTo descending
                         select new LogBookPageEditExceptionRegisterDTO
                         {
                             Id = exception.Id,
                             UserNames = latestPerson != null
                                         ? latestPerson.FirstName + " " + latestPerson.LastName + " (" + user.Email + ")"
                                         : null,
                             LogBookTypeName = logBookType != null
                                               ? logBookType.Name
                                               : null,
                             LogBookNumber = logBook != null
                                             ? logBook.LogNum
                                             : null,
                             IsValidNow = exception.ExceptionActiveFrom <= now && exception.ExceptionActiveTo > now,
                             ExceptionActiveFrom = exception.ExceptionActiveFrom,
                             ExceptionActiveTo = exception.ExceptionActiveTo,
                             EditPageFrom = exception.EditPageDateFrom,
                             EditPageTo = exception.EditPageDateTo,
                             IsActive = exception.IsActive
                         };

            return result;
        }

        private IQueryable<LogBookPageEditExceptionRegisterDTO> GetParametersFilteredLogBookPageEditExceptions(LogBookPageEditExceptionFilters filters)
        {
            DateTime now = DateTime.Now;

            var result = from exception in Db.LogBookPageEditExceptions
                         join user in Db.Users on exception.UserId equals user.Id into leftUser
                         from user in leftUser.DefaultIfEmpty()
                         join person in Db.Persons on user.PersonId equals person.Id into leftPerson
                         from person in leftPerson.DefaultIfEmpty()
                         join latestPerson in Db.Persons on new
                         {
                             person.EgnLnc,
                             person.IdentifierType
                         } equals new
                         {
                             latestPerson.EgnLnc,
                             latestPerson.IdentifierType
                         } into leftLatestPerson
                         from latestPerson in leftLatestPerson.DefaultIfEmpty()
                         join logBookType in Db.NlogBookTypes on exception.LogBookTypeId equals logBookType.Id into leftLogBookType
                         from logBookType in leftLogBookType.DefaultIfEmpty()
                         join logBook in Db.LogBooks on exception.LogBookId equals logBook.Id into leftLogBook
                         from logBook in leftLogBook.DefaultIfEmpty()
                         where exception.IsActive == !filters.ShowInactiveRecords
                               && (latestPerson == null || (latestPerson.ValidFrom <= now && latestPerson.ValidTo > now))
                               && (!filters.UserId.HasValue || exception.UserId == filters.UserId.Value)
                               && (filters.LogBookTypeIds == null || (exception.LogBookTypeId.HasValue && filters.LogBookTypeIds.Contains(exception.LogBookTypeId.Value)))
                               && (!filters.LogBookId.HasValue || (exception.LogBookId.HasValue && exception.LogBookId == filters.LogBookId))
                               && (!filters.ExceptionActiveDateFrom.HasValue || exception.ExceptionActiveFrom >= filters.ExceptionActiveDateFrom.Value)
                               && (!filters.ExceptionActiveDateTo.HasValue || exception.ExceptionActiveTo <= filters.ExceptionActiveDateTo.Value)
                               && (!filters.EditPageDateFrom.HasValue || exception.EditPageDateFrom >= filters.EditPageDateFrom.Value)
                               && (!filters.EditPageDateTo.HasValue || exception.EditPageDateTo <= filters.EditPageDateTo.Value)
                         orderby exception.ExceptionActiveTo descending
                         select new LogBookPageEditExceptionRegisterDTO
                         {
                             Id = exception.Id,
                             UserNames = latestPerson != null
                                         ? latestPerson.FirstName + " " + latestPerson.LastName + " (" + user.Email + ")"
                                         : null,
                             LogBookTypeName = logBookType != null
                                               ? logBookType.Name
                                               : null,
                             LogBookNumber = logBook != null
                                             ? logBook.LogNum
                                             : null,
                             IsValidNow = exception.ExceptionActiveFrom <= now && exception.ExceptionActiveTo > now,
                             ExceptionActiveFrom = exception.ExceptionActiveFrom,
                             ExceptionActiveTo = exception.ExceptionActiveTo,
                             EditPageFrom = exception.EditPageDateFrom,
                             EditPageTo = exception.EditPageDateTo,
                             IsActive = exception.IsActive
                         };

            return result;
        }

        private void CheckAndThrowIfExceptionCombinationExists(LogBookPageEditExceptionEditDTO model)
        {
            bool combinationExists = (from exception in Db.LogBookPageEditExceptions
                                      where (model.Id == default || exception.Id != model.Id)
                                             && exception.UserId == model.UserId
                                             && exception.LogBookTypeId == model.LogBookTypeId
                                             && exception.LogBookId == model.LogBookId
                                             && exception.ExceptionActiveFrom == model.ExceptionActiveFrom.Value
                                             && exception.IsActive
                                      select 1).Any();

            if (combinationExists)
            {
                throw new LogBookPageEditExceptionCombinationExistsException();
            }
        }
    }
}
