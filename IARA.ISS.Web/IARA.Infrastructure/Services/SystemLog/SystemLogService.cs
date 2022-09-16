using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.SystemLog;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services.SystemLog
{
    public class SystemLogService : Service, ISystemLogService
    {
        public SystemLogService(IARADbContext db) : base(db)
        {

        }

        public SystemLogViewDTO Get(int id)
        {
            SystemLogViewDTO systemLogView = (from log in Db.AuditLogs
                                              where log.Id == id
                                              select new SystemLogViewDTO
                                              {
                                                  NewValue = log.NewValue,
                                                  OldValue = log.OldValue
                                              }).First();

            return systemLogView;
        }

        public IQueryable<SystemLogDTO> GetAll(SystemLogFilters filters)
        {
            IQueryable<SystemLogDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllSystemLogs();
            }
            else if (filters.HasAnyFilters())
            {
                result = GetParametersFilteredSystemLogs(filters);
            }
            else
            {
                result = GetFreeTextFilteredSystemLogs(filters.FreeTextSearch);
            }

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }

        private IQueryable<SystemLogDTO> GetAllSystemLogs()
        {
            IQueryable<SystemLogDTO> systemLogs = from log in Db.AuditLogs
                                                  join logType in Db.NauditLogActionTypes on log.ActionType.ToLower() equals logType.Code.ToLower() into logTypes
                                                  from logTypeJoined in logTypes.DefaultIfEmpty()
                                                  orderby log.LogDate descending, log.Username
                                                  select new SystemLogDTO
                                                  {
                                                      ActionType = logTypeJoined != null ? logTypeJoined.Name : log.ActionType,
                                                      Username = log.Username,
                                                      LogDate = log.LogDate,
                                                      BrowserInfo = log.BrowserInfo,
                                                      Id = log.Id,
                                                      IPAddress = log.Ipaddress,
                                                      Module = log.SchemaName,
                                                      Action = log.Action,
                                                      Application = log.Application,
                                                      TableName = log.TableName
                                                  };

            return systemLogs;
        }

        private IQueryable<SystemLogDTO> GetParametersFilteredSystemLogs(SystemLogFilters filters)
        {
            var queryLogs = from log in Db.AuditLogs
                            join logType in Db.NauditLogActionTypes on log.ActionType.ToLower() equals logType.Code.ToLower() into logTypes
                            from logTypeJoined in logTypes.DefaultIfEmpty()
                            join user in Db.Users on log.Username equals user.Username into users
                            from userJoined in users.DefaultIfEmpty()
                            select new
                            {
                                actionTypeId = logTypeJoined != null ? logTypeJoined.Id : -1,
                                actionType = logTypeJoined != null ? logTypeJoined.Name : log.ActionType,
                                log.Username,
                                log.LogDate,
                                log.BrowserInfo,
                                log.Id,
                                log.Ipaddress,
                                log.SchemaName,
                                log.Action,
                                log.Application,
                                log.TableName,
                                userId = userJoined != null ? userJoined.Id : -1,
                                log.TableId
                            };

            if (!string.IsNullOrEmpty(filters.TableId) && !string.IsNullOrEmpty(filters.TableName))
            {
                queryLogs = queryLogs.Where(log => log.TableId == filters.TableId && log.TableName == filters.TableName);
            }

            if (filters.UserId.HasValue)
            {
                queryLogs = queryLogs.Where(log => log.userId == filters.UserId);
            }

            if (filters.ActionTypeId.HasValue)
            {
                queryLogs = queryLogs.Where(log => log.actionTypeId == filters.ActionTypeId);
            }

            if (filters.RegisteredDateFrom.HasValue)
            {
                queryLogs = queryLogs.Where(log => log.LogDate >= filters.RegisteredDateFrom);
            }

            if (filters.RegisteredDateTo.HasValue)
            {
                filters.RegisteredDateTo = filters.RegisteredDateTo.Value.AddDays(1).AddSeconds(-1);

                queryLogs = queryLogs.Where(log => log.LogDate <= filters.RegisteredDateTo);
            }

            IQueryable<SystemLogDTO> systemLogs = from log in queryLogs
                                                  orderby log.LogDate descending, log.Username
                                                  select new SystemLogDTO
                                                  {
                                                      ActionType = log.actionType,
                                                      Username = log.Username,
                                                      LogDate = log.LogDate,
                                                      BrowserInfo = log.BrowserInfo,
                                                      Id = log.Id,
                                                      IPAddress = log.Ipaddress,
                                                      Module = log.SchemaName,
                                                      Action = log.Action,
                                                      Application = log.Application,
                                                      TableName = log.TableName
                                                  };

            return systemLogs;
        }

        private IQueryable<SystemLogDTO> GetFreeTextFilteredSystemLogs(string text)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            IQueryable<SystemLogDTO> systemLogs = from log in Db.AuditLogs
                                                  join logType in Db.NauditLogActionTypes on log.ActionType equals logType.Code into logTypes
                                                  from logTypeJoined in logTypes.DefaultIfEmpty()
                                                  where
                                                        (log.Id.ToString().Contains(text) ||
                                                         log.ActionType.ToLower().Contains(text) ||
                                                         logTypeJoined.Name.ToLower().Contains(text) ||
                                                         log.Application.ToLower().Contains(text) ||
                                                         log.SchemaName.ToLower().Contains(text) ||
                                                         log.Action.ToLower().Contains(text) ||
                                                         log.TableName.ToLower().Contains(text) ||
                                                         log.Username.ToLower().Contains(text) ||
                                                         log.Ipaddress.ToLower().Contains(text) ||
                                                         log.BrowserInfo.ToLower().Contains(text) ||
                                                         (searchDate.HasValue && searchDate.Value.Date == log.LogDate.Date))
                                                  orderby log.LogDate descending, log.Username
                                                  select new SystemLogDTO
                                                  {
                                                      ActionType = logTypeJoined != null ? logTypeJoined.Name : log.ActionType,
                                                      Username = log.Username,
                                                      LogDate = log.LogDate,
                                                      BrowserInfo = log.BrowserInfo,
                                                      Id = log.Id,
                                                      IPAddress = log.Ipaddress,
                                                      Module = log.SchemaName,
                                                      Action = log.Action,
                                                      Application = log.Application,
                                                      TableName = log.TableName
                                                  };

            return systemLogs;
        }
    }
}
