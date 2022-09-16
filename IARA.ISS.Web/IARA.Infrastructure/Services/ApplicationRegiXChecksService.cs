using System;
using System.Linq;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ApplicationRegiXChecks;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class ApplicationRegiXChecksService : Service, IApplicationRegiXChecksService
    {
        public ApplicationRegiXChecksService(IARADbContext db)
            : base(db)
        {
        }

        public IQueryable<ApplicationRegixCheckRequestDTO> GetAll(ApplicationRegiXChecksFilters filters)
        {
            IQueryable<ApplicationRegixCheckRequestDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;

                result = GetAllApplicationRegiXChecks(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredApplicationRegiXChecks(filters)
                    : GetFreeTextFilteredApplicationRegiXChecks(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            return result;
        }

        public ApplicationRegixCheckRequestEditDTO Get(int id)
        {
            var result = (from regixCheck in Db.ApplicationRegiXchecks
                          join application in Db.Applications on regixCheck.ApplicationId equals application.Id
                          join applicationType in Db.NapplicationTypes on application.ApplicationTypeId equals applicationType.Id
                          where regixCheck.Id == id
                          select new ApplicationRegixCheckRequestEditDTO
                          {
                              ApplicationId = application.Id.ToString(),
                              ApplicationType = applicationType.Name,
                              WebServiceName = regixCheck.WebServiceName,
                              RemoteAddress = regixCheck.RemoteAddress,
                              RequestDateTime = regixCheck.RequestDateTime,
                              RequestContent = regixCheck.RequestContent,
                              ResponseStatus = regixCheck.ResponseStatus,
                              ResponseDateTime = regixCheck.ResponseDateTime,
                              ResponseContent = regixCheck.ResponseContent,
                              ExpectedResponseContent = regixCheck.ExpectedResponseContent,
                              ErrorLevel = regixCheck.ErrorLevel,
                              ErrorDescription = regixCheck.ErrorDescription,
                              Attempts = regixCheck.Attempts
                          }).First();

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.ApplicationRegiXchecks, id);
            return audit;
        }

        private IQueryable<ApplicationRegixCheckRequestDTO> GetAllApplicationRegiXChecks(bool showInactive)
        {
            var result = from regixCheck in Db.ApplicationRegiXchecks
                         join application in Db.Applications on regixCheck.ApplicationId equals application.Id
                         join applicationType in Db.NapplicationTypes on application.ApplicationTypeId equals applicationType.Id
                         where regixCheck.IsActive == !showInactive
                         orderby regixCheck.RequestDateTime descending
                         select new ApplicationRegixCheckRequestDTO
                         {
                             Id = regixCheck.Id,
                             ApplicationId = application.Id.ToString(),
                             ApplicationType = applicationType.Name,
                             WebServiceName = regixCheck.WebServiceName,
                             RemoteAddress = regixCheck.RemoteAddress,
                             RequestDateTime = regixCheck.RequestDateTime,
                             ResponseStatus = regixCheck.ResponseStatus,
                             ResponseDateTime = regixCheck.ResponseDateTime,
                             ErrorLevel = regixCheck.ErrorLevel,
                             ErrorDescription = regixCheck.ErrorDescription,
                             IsActive = regixCheck.IsActive
                         };

            return result;
        }

        private IQueryable<ApplicationRegixCheckRequestDTO> GetFreeTextFilteredApplicationRegiXChecks(string freeTextSearch, bool showInactive)
        {
            freeTextSearch = freeTextSearch.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(freeTextSearch);

            var result = from regixCheck in Db.ApplicationRegiXchecks
                         join application in Db.Applications on regixCheck.ApplicationId equals application.Id
                         join applicationType in Db.NapplicationTypes on application.ApplicationTypeId equals applicationType.Id
                         where regixCheck.IsActive == !showInactive
                            && (regixCheck.WebServiceName.ToLower().Contains(freeTextSearch)
                                || regixCheck.ResponseStatus.ToLower().Contains(freeTextSearch)
                                || regixCheck.RemoteAddress.ToLower().Contains(freeTextSearch)
                                || regixCheck.ErrorLevel.ToLower().Contains(freeTextSearch)
                                || regixCheck.ErrorDescription.ToLower().Contains(freeTextSearch)
                                || applicationType.Name.ToLower().Contains(freeTextSearch)
                                || application.Id.ToString().Contains(freeTextSearch)
                                || regixCheck.RequestDateTime.Date == searchDate
                                || (regixCheck.ResponseDateTime.HasValue && regixCheck.ResponseDateTime.Value.Date == searchDate))
                         orderby regixCheck.RequestDateTime descending
                         select new ApplicationRegixCheckRequestDTO
                         {
                             Id = regixCheck.Id,
                             ApplicationId = application.Id.ToString(),
                             ApplicationType = applicationType.Name,
                             WebServiceName = regixCheck.WebServiceName,
                             RemoteAddress = regixCheck.RemoteAddress,
                             RequestDateTime = regixCheck.RequestDateTime,
                             ResponseStatus = regixCheck.ResponseStatus,
                             ResponseDateTime = regixCheck.ResponseDateTime,
                             ErrorLevel = regixCheck.ErrorLevel,
                             ErrorDescription = regixCheck.ErrorDescription,
                             IsActive = regixCheck.IsActive
                         };

            return result;
        }

        private IQueryable<ApplicationRegixCheckRequestDTO> GetParametersFilteredApplicationRegiXChecks(ApplicationRegiXChecksFilters filters)
        {
            var query = from regixCheck in Db.ApplicationRegiXchecks
                        join application in Db.Applications on regixCheck.ApplicationId equals application.Id
                        where regixCheck.IsActive == !filters.ShowInactiveRecords
                        select new
                        {
                            regixCheck.Id,
                            regixCheck.ApplicationId,
                            application.ApplicationTypeId,
                            regixCheck.WebServiceName,
                            regixCheck.RemoteAddress,
                            regixCheck.RequestDateTime,
                            regixCheck.ResponseDateTime,
                            regixCheck.ResponseStatus,
                            regixCheck.ErrorLevel,
                            regixCheck.ErrorDescription,
                            regixCheck.IsActive
                        };

            if (!string.IsNullOrEmpty(filters.ApplicationId))
            {
                query = query.Where(x => x.ApplicationId.ToString().Contains(filters.ApplicationId));
            }

            if (filters.ApplicationTypeId.HasValue)
            {
                query = query.Where(x => x.ApplicationTypeId == filters.ApplicationTypeId);
            }

            if (!string.IsNullOrEmpty(filters.WebServiceName))
            {
                query = query.Where(x => x.WebServiceName.ToLower().Contains(filters.WebServiceName.ToLower()));
            }

            if (filters.RequestDateFrom.HasValue)
            {
                query = query.Where(x => x.RequestDateTime.Date >= filters.RequestDateFrom.Value);
            }

            if (filters.RequestDateTo.HasValue)
            {
                query = query.Where(x => x.RequestDateTime.Date <= filters.RequestDateTo.Value);
            }

            if (filters.ResponseDateFrom.HasValue)
            {
                query = query.Where(x => x.ResponseDateTime.HasValue && x.ResponseDateTime.Value.Date >= filters.ResponseDateFrom.Value);
            }

            if (filters.ResponseDateTo.HasValue)
            {
                query = query.Where(x => x.ResponseDateTime.HasValue && x.ResponseDateTime.Value.Date >= filters.ResponseDateTo.Value);
            }

            if (filters.ErrorLevels != null && filters.ErrorLevels.Count > 0)
            {
                query = query.Where(x => filters.ErrorLevels.Contains(x.ErrorLevel));
            }

            var result = from regixCheck in query
                         join application in Db.Applications on regixCheck.ApplicationId equals application.Id
                         join applicationType in Db.NapplicationTypes on application.ApplicationTypeId equals applicationType.Id
                         orderby regixCheck.RequestDateTime descending
                         select new ApplicationRegixCheckRequestDTO
                         {
                             Id = regixCheck.Id,
                             ApplicationId = application.Id.ToString(),
                             ApplicationType = applicationType.Name,
                             WebServiceName = regixCheck.WebServiceName,
                             RemoteAddress = regixCheck.RemoteAddress,
                             RequestDateTime = regixCheck.RequestDateTime,
                             ResponseStatus = regixCheck.ResponseStatus,
                             ResponseDateTime = regixCheck.ResponseDateTime,
                             ErrorLevel = regixCheck.ErrorLevel,
                             ErrorDescription = regixCheck.ErrorDescription,
                             IsActive = regixCheck.IsActive
                         };

            return result;
        }
    }
}
