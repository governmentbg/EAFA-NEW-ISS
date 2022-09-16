using System;
using System.Linq;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ErrorLog;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services.ErrorLogs
{
    public class ErrorLogService : Service, IErrorLogService
    {
        public ErrorLogService(IARADbContext db)
            : base(db)
        {
        }

        public IQueryable<ErrorLogDTO> GetAll(ErrorLogFilters filters)
        {
            IQueryable<ErrorLogDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllErrorLogs();
            }
            else if (filters.HasAnyFilters())
            {
                result = GetParametersFilteredErrorLogs(filters);
            }
            else
            {
                result = GetFreeTextFilteredErrorLogs(filters.FreeTextSearch);
            }
            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }

        private IQueryable<ErrorLogDTO> GetParametersFilteredErrorLogs(ErrorLogFilters filters)
        {
            var query = from errorLog in Db.ErrorLogs
                        join user in Db.Users on errorLog.Username equals user.Username into left
                        from user in left.DefaultIfEmpty()
                        orderby errorLog.LogDate descending
                        select new
                        {
                            userId = user != null ? user.Id : default(int?),
                            errorLog.LogDate,
                            errorLog.Username,
                            errorLog.Severity,
                            errorLog.Client,
                            errorLog.Class,
                            errorLog.Method,
                            errorLog.ExceptionSource,
                            errorLog.Message,
                            errorLog.StackTrace
                        };

            if (filters.UserId.HasValue)
            {
                query = query.Where(user => user.userId.HasValue && user.userId == filters.UserId);
            }

            if (filters.ErrorLogDateFrom.HasValue)
            {
                query = query.Where(errorLog => errorLog.LogDate >= filters.ErrorLogDateFrom);
            }

            if (filters.ErrorLogDateTo.HasValue)
            {
                query = query.Where(errorLog => errorLog.LogDate <= filters.ErrorLogDateTo);
            }

            if (!string.IsNullOrEmpty(filters.Class))
            {
                query = query.Where(errorLog => errorLog.Class.ToLower().Contains(filters.Class.ToLower()));
            }

            if (filters.Severity != null && filters.Severity.Count > 0)
            {
                query = query.Where(errorLog => filters.Severity.Contains(errorLog.Severity));
            }

            var result = from data in query
                         orderby data.LogDate descending
                         select new ErrorLogDTO
                         {
                             LogDate = data.LogDate,
                             Username = data.Username,
                             Severity = data.Severity,
                             Client = data.Client,
                             Class = data.Class,
                             Method = data.Method,
                             ExceptionSource = data.ExceptionSource,
                             Message = data.Message,
                             StackTrace = data.StackTrace
                         };
            return result;
        }

        private IQueryable<ErrorLogDTO> GetFreeTextFilteredErrorLogs(string text)
        {
            text = text.ToLowerInvariant();

            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            var result = from errorLog in Db.ErrorLogs
                         where errorLog.Username.ToLower().Contains(text)
                                 || (searchDate.HasValue && searchDate.Value.Date == errorLog.LogDate.Date)
                                 || errorLog.Message.ToLower().Contains(text)
                                 || errorLog.Method.ToLower().Contains(text)
                                 || errorLog.Severity.ToLower().Contains(text)
                                 || errorLog.Client.ToLower().Contains(text)
                                 || errorLog.Class.ToLower().Contains(text)
                                 || errorLog.ExceptionSource.ToLower().Contains(text)
                                 || errorLog.StackTrace.ToLower().Contains(text)
                         orderby errorLog.LogDate descending
                         select new ErrorLogDTO
                         {
                             LogDate = errorLog.LogDate,
                             Username = errorLog.Username,
                             Severity = errorLog.Severity,
                             Client = errorLog.Client,
                             Class = errorLog.Class,
                             Method = errorLog.Method,
                             ExceptionSource = errorLog.ExceptionSource,
                             Message = errorLog.Message,
                             StackTrace = errorLog.StackTrace
                         };

            return result;
        }

        private IQueryable<ErrorLogDTO> GetAllErrorLogs()
        {
            var result = from errorLog in Db.ErrorLogs
                         orderby errorLog.LogDate descending
                         select new ErrorLogDTO
                         {
                             LogDate = errorLog.LogDate,
                             Username = errorLog.Username,
                             Severity = errorLog.Severity,
                             Client = errorLog.Client,
                             Class = errorLog.Class,
                             Method = errorLog.Method,
                             ExceptionSource = errorLog.ExceptionSource,
                             Message = errorLog.Message,
                             StackTrace = errorLog.StackTrace
                         };

            return result;
        }
    }
}
