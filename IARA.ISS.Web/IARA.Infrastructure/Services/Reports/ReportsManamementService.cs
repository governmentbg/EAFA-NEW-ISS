using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces.Reports;

namespace IARA.Infrastructure.Services.Reports
{
    public class ReportsManamementService : BaseService, IReportsManamementService
    {
        public ReportsManamementService(IARADbContext dbContext)
            : base(dbContext)
        { }

        public int AddGroup(ReportGroupDTO inputGroup)
        {
            ReportGroup newGroup = new ReportGroup()
            {
                Name = inputGroup.Name,
                Description = inputGroup.Description,
                GroupType = inputGroup.GroupType
            };

            this.Db.ReportGroups.Add(newGroup);
            this.Db.SaveChanges();

            return newGroup.Id;
        }

        public int AddNParameter(NReportParameterEditDTO inputNParameter)
        {
            inputNParameter.DateTo = inputNParameter.DateTo.Value.AddDays(1).AddSeconds(-1);

            NreportParameter newNParameter = new NreportParameter()
            {
                Code = inputNParameter.Code,
                DataType = inputNParameter.DataType.ToString(),
                ErrorMessage = inputNParameter.ErrorMessage,
                Description = inputNParameter.Description,
                DefaultValue = inputNParameter.DefaultValue,
                Name = inputNParameter.Name,
                NomenclatureSql = inputNParameter.NomenclatureSQL,
                Pattern = inputNParameter.Pattern,
                ValidFrom = inputNParameter.DateFrom.Value,
                ValidTo = inputNParameter.DateTo.Value
            };

            this.Db.NReportParameters.Add(newNParameter);

            this.Db.SaveChanges();
            return newNParameter.Id;
        }

        public int AddReport(ReportDTO inputReport)
        {
            if (!string.IsNullOrEmpty(inputReport.ReportSQL) && !SQLReportUtils.ValidateDatasourceQuery(inputReport.ReportSQL))
            {
                throw new ArgumentException("Invalid sql query");
            }

            Report newReport = new Report()
            {
                Name = inputReport.Name,
                ReportType = inputReport.ReportType,
                Description = inputReport.Description,
                Code = inputReport.Code,
                IconName = inputReport.IconName,
                LastRunDateTime = inputReport.LastRunDateTime,
                ReportGroupId = inputReport.ReportGroupId.Value,
                IsActive = true,
                ReportSql = inputReport.ReportSQL
            };

            this.Db.Reports.Add(newReport);

            this.ManageParameters(newReport, inputReport.Parameters);
            this.ManageUserPermissions(newReport, inputReport.Users);
            this.ManageRolePermissions(newReport, inputReport.Roles);

            this.Db.SaveChanges();
            return newReport.Id;
        }

        public void DeleteGroup(int id)
        {
            IQueryable<Report> wantedReportsFromDb = from groupFromDb in this.Db.ReportGroups
                                                     join reportFromDb in this.Db.Reports on groupFromDb.Id equals reportFromDb.ReportGroupId into joinedReports
                                                     from joinedReport in joinedReports.DefaultIfEmpty()
                                                     where groupFromDb.Id == id
                                                     select joinedReport;

            if (wantedReportsFromDb.Any(x => x.IsActive))
            {
                foreach (Report report in wantedReportsFromDb)
                {
                    report.IsActive = false;
                }
            }

            this.DeleteRecordWithId(this.Db.ReportGroups, id);
            this.Db.SaveChanges();
        }

        public void DeleteNParameter(int id)
        {
            NreportParameter wantedNParameter = this.GetNReportParameterById(id);

            DateTime now = DateTime.Now;
            wantedNParameter.ValidTo = now;

            this.Db.SaveChanges();
        }

        public void DeleteReport(int id)
        {
            this.DeleteRecordWithId(this.Db.Reports, id);
            this.Db.SaveChanges();
        }

        public void EditGroup(ReportGroupDTO inputGroup)
        {
            ReportGroup wantedGroupFromDb = (from groupFromDb in this.Db.ReportGroups
                                             where groupFromDb.Id == inputGroup.Id
                                             select groupFromDb).First();

            wantedGroupFromDb.Name = inputGroup.Name;
            wantedGroupFromDb.Description = inputGroup.Description;
            wantedGroupFromDb.GroupType = inputGroup.GroupType;

            this.Db.SaveChanges();
        }

        public void EditNParameter(NReportParameterEditDTO inputNParameter)
        {
            NreportParameter wantedNParameter = this.GetNReportParameterById(inputNParameter.Id.Value);

            //2021-10-22 00:00:00
            if (inputNParameter.DateTo.Value.Hour == 0
                && inputNParameter.DateTo.Value.Minute == 0
                && inputNParameter.DateTo.Value.Second == 0
                && inputNParameter.DateTo.Value.Millisecond == 0)
            {
                inputNParameter.DateTo = inputNParameter.DateTo.Value.AddHours(1).AddSeconds(-1);
            }

            wantedNParameter.Code = inputNParameter.Code;
            wantedNParameter.DataType = inputNParameter.DataType.ToString();
            wantedNParameter.ErrorMessage = inputNParameter.ErrorMessage;
            wantedNParameter.Description = inputNParameter.Description;
            wantedNParameter.DefaultValue = inputNParameter.DefaultValue;
            wantedNParameter.Name = inputNParameter.Name;
            wantedNParameter.NomenclatureSql = inputNParameter.NomenclatureSQL;
            wantedNParameter.Pattern = inputNParameter.Pattern;
            wantedNParameter.ValidFrom = inputNParameter.DateFrom.Value;
            wantedNParameter.ValidTo = inputNParameter.DateTo.Value;

            this.Db.SaveChanges();
        }

        public void EditReport(ReportDTO inputReport)
        {
            if (!string.IsNullOrEmpty(inputReport.ReportSQL) && !SQLReportUtils.ValidateDatasourceQuery(inputReport.ReportSQL))
            {
                throw new ArgumentException("Invalid sql query");
            }

            Report wantedReport = (from report in this.Db.Reports
                                   where report.Id == inputReport.Id
                                   select report).First();

            wantedReport.Name = inputReport.Name;
            wantedReport.Description = inputReport.Description;
            wantedReport.IconName = inputReport.IconName;
            wantedReport.Code = inputReport.Code;
            wantedReport.ReportSql = inputReport.ReportSQL;
            wantedReport.ReportType = inputReport.ReportType;

            if (inputReport.ReportGroupId.HasValue)
            {
                wantedReport.ReportGroupId = inputReport.ReportGroupId.Value;
            }

            this.ManageParameters(wantedReport, inputReport.Parameters);
            this.ManageUserPermissions(wantedReport, inputReport.Users);
            this.ManageRolePermissions(wantedReport, inputReport.Roles);

            this.Db.SaveChanges();
        }

        public IQueryable<NReportParameterDTO> GetAllNParameters(ReportParameterDefinitionFilters filters)
        {
            IQueryable<NReportParameterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = this.GetNParameters(showInactive);
            }
            else if (filters.HasAnyFilters())
            {
                result = this.GetFilteredNParameters(filters);
            }
            else
            {
                result = this.GetFreeTextFilteredNParameters(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            return result;
        }

        public NReportParameterEditDTO GetNParameter(int id)
        {
            DateTime now = DateTime.Now;
            NReportParameterEditDTO wantedNParameter = (from nParameter in Db.NReportParameters
                                                        where nParameter.Id == id
                                                        select new NReportParameterEditDTO
                                                        {
                                                            Id = nParameter.Id,
                                                            Code = nParameter.Code,
                                                            DataType = Enum.Parse<ReportParameterTypeEnum>(nParameter.DataType),
                                                            DateFrom = nParameter.ValidFrom,
                                                            DateTo = nParameter.ValidTo,
                                                            DefaultValue = nParameter.DefaultValue,
                                                            Description = nParameter.Description,
                                                            ErrorMessage = nParameter.ErrorMessage,
                                                            Name = nParameter.Name,
                                                            NomenclatureSQL = nParameter.NomenclatureSql,
                                                            Pattern = nParameter.Pattern
                                                        }).First();


            return wantedNParameter;
        }
        public void UndoDeletedGroup(int id)
        {
            this.UndoDeleteRecordWithId(this.Db.ReportGroups, id);
            this.Db.SaveChanges();
        }

        public void UndoDeletedNParameter(int id)
        {
            NreportParameter wantedNParameter = this.GetNReportParameterById(id);

            wantedNParameter.ValidTo = DefaultConstants.MAX_VALID_DATE;

            this.Db.SaveChanges();
        }

        public void UndoDeletedReport(int id)
        {
            this.UndoDeleteRecordWithId(this.Db.Reports, id);
            this.Db.SaveChanges();
        }

        private IQueryable<NReportParameterDTO> GetFilteredNParameters(ReportParameterDefinitionFilters filters)
        {
            DateTime now = DateTime.Now;

            IQueryable<NReportParameterDTO> nParametersQuery = from nParameter in this.Db.NReportParameters
                                                               orderby nParameter.Id descending
                                                               select new NReportParameterDTO
                                                               {
                                                                   Id = nParameter.Id,
                                                                   Code = nParameter.Code,
                                                                   DataType = nParameter.DataType,
                                                                   DateFrom = nParameter.ValidFrom,
                                                                   DateTo = nParameter.ValidTo,
                                                                   DefaultValue = nParameter.DefaultValue,
                                                                   Description = nParameter.Description,
                                                                   ErrorMessage = nParameter.ErrorMessage,
                                                                   Name = nParameter.Name,
                                                                   Pattern = nParameter.Pattern,
                                                                   IsActive = nParameter.ValidFrom <= now && nParameter.ValidTo >= now
                                                               };

            nParametersQuery = nParametersQuery.Where(nParameter => nParameter.IsActive == !filters.ShowInactiveRecords);

            if (filters.ValidFrom.HasValue)
            {
                nParametersQuery = nParametersQuery.Where(nParameter => nParameter.IsActive && nParameter.DateTo >= filters.ValidFrom);
            }
            if (filters.ValidTo.HasValue)
            {
                nParametersQuery = nParametersQuery.Where(nParameter => nParameter.IsActive && nParameter.DateFrom <= filters.ValidTo);
            }

            return nParametersQuery;
        }

        private IQueryable<NReportParameterDTO> GetFreeTextFilteredNParameters(string text, bool showInactive)
        {
            text = text.ToLowerInvariant();
            DateTime? date = DateTimeUtils.TryParseDate(text);
            DateTime now = DateTime.Now;

            IQueryable<NReportParameterDTO> nParametersQuery = from nParameter in this.Db.NReportParameters
                                                               where (nParameter.Code.ToLower().Contains(text) ||
                                                                      nParameter.DataType.ToLower().Contains(text) ||
                                                                      nParameter.DefaultValue.ToLower().Contains(text) ||
                                                                      nParameter.Description.ToLower().Contains(text) ||
                                                                      nParameter.ErrorMessage.ToLower().Contains(text) ||
                                                                      nParameter.Name.ToLower().Contains(text) ||
                                                                      nParameter.Pattern.ToLower().Contains(text) ||
                                                                     (date.HasValue && (date.Value.Date == nParameter.ValidFrom.Date)) ||
                                                                     (date.HasValue && (date.Value.Date == nParameter.ValidTo.Date)))
                                                               orderby nParameter.Id descending
                                                               select new NReportParameterDTO
                                                               {
                                                                   Id = nParameter.Id,
                                                                   Code = nParameter.Code,
                                                                   DataType = nParameter.DataType,
                                                                   DateFrom = nParameter.ValidFrom,
                                                                   DateTo = nParameter.ValidTo,
                                                                   DefaultValue = nParameter.DefaultValue,
                                                                   Description = nParameter.Description,
                                                                   ErrorMessage = nParameter.ErrorMessage,
                                                                   Name = nParameter.Name,
                                                                   Pattern = nParameter.Pattern,
                                                                   IsActive = nParameter.ValidFrom <= now && nParameter.ValidTo >= now
                                                               };

            nParametersQuery = nParametersQuery.Where(nParameter => nParameter.IsActive == !showInactive);

            return nParametersQuery;
        }

        private List<int> GetNomenclatureIds(List<NomenclatureDTO> nomenclatures)
        {
            List<int> nomenclatureIds = new List<int>();
            if (nomenclatures != null)
            {
                foreach (NomenclatureDTO nomenclature in nomenclatures)
                {
                    nomenclatureIds.Add(nomenclature.Value);
                }
            }
            return nomenclatureIds;
        }

        private IQueryable<NReportParameterDTO> GetNParameters(bool showInctive)
        {
            DateTime now = DateTime.Now;

            IQueryable<NReportParameterDTO> nParametersQuery = from nParameter in this.Db.NReportParameters
                                                               orderby nParameter.Id descending
                                                               select new NReportParameterDTO
                                                               {
                                                                   Id = nParameter.Id,
                                                                   Code = nParameter.Code,
                                                                   DateFrom = nParameter.ValidFrom,
                                                                   DateTo = nParameter.ValidTo,
                                                                   DataType = nParameter.DataType,
                                                                   DefaultValue = nParameter.DefaultValue,
                                                                   Description = nParameter.Description,
                                                                   ErrorMessage = nParameter.ErrorMessage,
                                                                   Name = nParameter.Name,
                                                                   Pattern = nParameter.Pattern,
                                                                   IsActive = nParameter.ValidFrom <= now && nParameter.ValidTo >= now
                                                               };


            nParametersQuery = nParametersQuery.Where(nParameter => nParameter.IsActive == !showInctive);

            return nParametersQuery;
        }

        private NreportParameter GetNReportParameterById(int nParameterId)
        {
            return (from nParameter in this.Db.NReportParameters
                    where nParameter.Id == nParameterId
                    select nParameter).First();
        }
        private List<int> GetParameterIds(List<ReportParameterDTO> parameters)
        {
            List<int> listOfParameterIds = new List<int>();
            foreach (ReportParameterDTO parameter in parameters)
            {
                if (parameter.Id.HasValue)
                {
                    listOfParameterIds.Add(parameter.Id.Value);
                }
            }

            return listOfParameterIds;
        }

        private void ManageParameters(Report targetReport, List<ReportParameterDTO> parameters)
        {
            int targetReportId = targetReport.Id;
            List<int> listOfParameterIds = this.GetParameterIds(parameters);

            List<int> dbNParameterIds = (from parameter in this.Db.ReportParameters
                                         join nParameter in this.Db.NReportParameters on parameter.ParameterId equals nParameter.Id
                                         where parameter.ReportId == targetReportId
                                               && parameter.IsActive
                                         select parameter.Id).ToList();

            IEnumerable<int> parametersToDelete = dbNParameterIds.Except(listOfParameterIds);
            //except 0 защото новите записи идват без id
            IEnumerable<int> parametersToEdit = listOfParameterIds.Except(parametersToDelete).Except(new List<int>() { 0 });

            foreach (int id in parametersToDelete)
            {
                ReportParameter wantedReportParameter = (from parameter in this.Db.ReportParameters
                                                         where parameter.Id == id
                                                         select parameter).First();
                wantedReportParameter.IsActive = false;
            }

            foreach (int id in parametersToEdit)
            {
                ReportParameterDTO reportParameterFromDb = parameters.Where(parameter => parameter.Id == id).Single();
                ReportParameter wantedReportParameter = (from parameter in this.Db.ReportParameters
                                                         where parameter.Id == id
                                                         select parameter).First();

                wantedReportParameter.IsMandatory = reportParameterFromDb.IsMandatory;
                wantedReportParameter.IsActive = reportParameterFromDb.IsActive;
                wantedReportParameter.DefaultValue = reportParameterFromDb.DefaultValue;
                wantedReportParameter.ErrorMessage = reportParameterFromDb.ErrorMessage;
                wantedReportParameter.Pattern = reportParameterFromDb.Pattern;

                if (reportParameterFromDb.OrderNumber != null)
                {
                    wantedReportParameter.OrderNum = reportParameterFromDb.OrderNumber.Value;
                }

            }

            foreach (ReportParameterDTO inputNParameter in parameters)
            {
                if (!inputNParameter.Id.HasValue)
                {
                    ReportParameter newParameter = new ReportParameter
                    {
                        IsActive = inputNParameter.IsActive,
                        IsMandatory = inputNParameter.IsMandatory,
                        ParameterId = inputNParameter.ParameterId,
                        Report = targetReport,
                        DefaultValue = inputNParameter.DefaultValue,
                        Pattern = inputNParameter.Pattern,
                        ErrorMessage = inputNParameter.ErrorMessage
                    };

                    short? orderNumber = inputNParameter.OrderNumber;
                    if (orderNumber != null)
                    {
                        newParameter.OrderNum = orderNumber.Value;
                    }

                    this.Db.ReportParameters.Add(newParameter);
                }
            }
        }

        private void ManageRolePermissions(Report targetReport, List<NomenclatureDTO> roles)
        {
            int targetReportId = targetReport.Id;
            List<int> roleIds = this.GetNomenclatureIds(roles);

            List<int> dbReportRolePermissionIds = (from rolePermission in this.Db.ReportRolePermissions
                                                   where rolePermission.ReportId == targetReportId
                                                         && rolePermission.IsActive
                                                   select rolePermission.RoleId).ToList();

            IEnumerable<int> reportRolePermissionsToDelete = dbReportRolePermissionIds.Except(roleIds);
            IEnumerable<int> reportRolePermissionsToAdd = roleIds.Except(dbReportRolePermissionIds);

            foreach (int id in reportRolePermissionsToDelete)
            {
                ReportRolePermission wantedPermission = this.Db.ReportRolePermissions
                                                               .Single(permission => permission.ReportId == targetReportId
                                                                                     && permission.RoleId == id);
                wantedPermission.IsActive = false;
            }

            foreach (int id in reportRolePermissionsToAdd)
            {
                ReportRolePermission wantedPermission = this.Db.ReportRolePermissions
                                                               .SingleOrDefault(permission => permission.ReportId == targetReportId
                                                                                              && permission.RoleId == id);
                if (wantedPermission == null)
                {
                    ReportRolePermission newPermission = new ReportRolePermission()
                    {
                        Report = targetReport,
                        RoleId = id,
                        IsActive = true
                    };

                    this.Db.ReportRolePermissions.Add(newPermission);
                }
                else
                {
                    wantedPermission.IsActive = true;
                }
            }
        }

        private void ManageUserPermissions(Report targetReport, List<NomenclatureDTO> users)
        {
            int targetReportId = targetReport.Id;
            List<int> userIds = this.GetNomenclatureIds(users);

            List<int> dbReportUserPermissionIds = (from reportUserPermission in this.Db.ReportUserPermissions
                                                   where reportUserPermission.ReportId == targetReportId
                                                         && reportUserPermission.IsActive
                                                   select reportUserPermission.UserId).ToList();

            IEnumerable<int> reportUserPermissionIdsToDelete = dbReportUserPermissionIds.Except(userIds);
            IEnumerable<int> reportUserPermissionIdsToAdd = userIds.Except(dbReportUserPermissionIds);

            foreach (int id in reportUserPermissionIdsToDelete)
            {
                ReportUserPermission wantedPermission = this.Db.ReportUserPermissions
                                                               .Single(permission => permission.ReportId == targetReportId
                                                                                     && permission.UserId == id
                                                                                     && permission.IsActive);
                wantedPermission.IsActive = false;
            }

            foreach (int id in reportUserPermissionIdsToAdd)
            {
                ReportUserPermission wantedPermission = this.Db.ReportUserPermissions
                                                               .SingleOrDefault(permission => permission.ReportId == targetReportId
                                                                                              && permission.UserId == id
                                                                                              && !permission.IsActive);
                if (wantedPermission == null)
                {
                    ReportUserPermission newUserPermission = new ReportUserPermission()
                    {
                        Report = targetReport,
                        UserId = id,
                        IsActive = true
                    };

                    this.Db.ReportUserPermissions.Add(newUserPermission);
                }
                else
                {
                    wantedPermission.IsActive = true;
                }
            }
        }
    }
}
