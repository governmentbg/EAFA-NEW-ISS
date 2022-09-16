using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Mobile.Reports;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Mobile;
using IARA.Interfaces.Reports;

namespace IARA.Infrastructure.Services.Mobile
{
    public class MobileReportService : Service, IMobileReportService
    {
        private readonly IReportService reportService;

        public MobileReportService(IARADbContext dbContext, IReportService reportService)
            : base(dbContext)
        {
            this.reportService = reportService;
        }

        public List<MobileReportNodeDTO> GetReports(int userId, bool isAdministrator)
        {
            List<int> userRoleIds = !isAdministrator ? (
                from userRole in this.Db.UserRoles
                where userRole.UserId == userId
                select userRole.RoleId
            ).ToList() : null;

            var reports = (
                from report in this.Db.Reports
                join reportUser in this.Db.ReportUserPermissions on report.Id equals reportUser.ReportId into ru
                from reportUser in ru.DefaultIfEmpty()
                join reportRole in this.Db.ReportRolePermissions on report.Id equals reportRole.ReportId into rr
                from reportRole in rr.DefaultIfEmpty()
                where report.IsActive
                    && report.ReportType != nameof(ReportTypesEnum.Cross)
                    && (isAdministrator
                        || (reportUser != null && reportUser.IsActive && reportUser.UserId == userId)
                        || (reportRole != null && reportRole.IsActive && userRoleIds.Contains(reportRole.RoleId)))
                orderby report.OrderNum
                select new
                {
                    report.Id,
                    report.IconName,
                    report.Name,
                    report.ReportGroupId
                }
            ).ToList();

            List<int> reportGroupIds = reports
                .Select(f => f.ReportGroupId)
                .Distinct()
                .ToList();

            List<MobileReportNodeDTO> reportGroups = (
                from reportGroup in this.Db.ReportGroups
                where reportGroup.IsActive
                    && reportGroupIds.Contains(reportGroup.Id)
                select new MobileReportNodeDTO
                {
                    Id = reportGroup.Id,
                    Name = reportGroup.Name,
                }
            ).ToList();

            List<MobileReportNodeDTO> result = new List<MobileReportNodeDTO>();

            foreach (MobileReportNodeDTO reportGroup in reportGroups)
            {
                reportGroup.Children = reports.Where(f => f.ReportGroupId == reportGroup.Id)
                    .Select(f => new MobileReportNodeDTO
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Icon = f.IconName == null ? null : MaterialIconConstants.MaterialIcons[f.IconName]
                    })
                    .ToList();

                if (reportGroup.Children.Count > 0)
                {
                    result.Add(reportGroup);
                }
            }

            return result;
        }

        public MobileReportDTO GetReport(int id, int userId, bool isAdministrator)
        {
            if (!isAdministrator)
            {
                this.ThrowIfUserCannotAccessReport(id, userId);
            }

            MobileReportDTO report = (
                from rep in this.Db.Reports
                where rep.Id == id
                select new MobileReportDTO
                {
                    Id = rep.Id,
                    Icon = rep.IconName,
                    Name = rep.Name,
                    ReportType = Enum.Parse<ReportTypesEnum>(rep.ReportType)
                }
            ).Single();

            report.Parameters = this.GetParameters(id, userId);

            return report;
        }

        public MobileReportDTO GetReportByCode(string code, int userId, bool isAdministrator)
        {
            MobileReportDTO report = (
                from rep in this.Db.Reports
                where rep.Code == code
                select new MobileReportDTO
                {
                    Id = rep.Id,
                    Icon = rep.IconName,
                    Name = rep.Name,
                    ReportType = Enum.Parse<ReportTypesEnum>(rep.ReportType)
                }
            ).Single();

            if (!isAdministrator)
            {
                this.ThrowIfUserCannotAccessReport(report.Id, userId);
            }

            report.Parameters = this.GetParameters(report.Id, userId);

            return report;
        }

        public string GetReportSQL(int id, int userId, bool isAdministrator)
        {
            if (!isAdministrator)
            {
                this.ThrowIfUserCannotAccessReport(id, userId);
            }

            string reportSQL = (
                from rep in this.Db.Reports
                where rep.Id == id
                select rep.ReportSql
            ).Single();

            return reportSQL;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }

        private List<MobileReportParameterDTO> GetParameters(int id, int userId)
        {
            return this.reportService.GetExecuteReportParameters(id, userId)
                .ConvertAll(f => new MobileReportParameterDTO
                {
                    DataType = f.DataType,
                    DefaultValue = f.DefaultValue,
                    ErrorMessage = f.ErrorMessage,
                    Id = f.Id,
                    Code = f.Code,
                    IsMandatory = f.IsMandatory,
                    Name = f.Description,
                    Pattern = f.Pattern,
                    Nomenclatures = f.ParameterTypeNomenclatures,
                });
        }

        private void ThrowIfUserCannotAccessReport(int reportId, int userId)
        {
            bool foundUser = (
                from reportUser in this.Db.ReportUserPermissions
                where reportUser.UserId == userId
                    && reportUser.ReportId == reportId
                select reportUser
            ).Any();

            if (foundUser)
            {
                return;
            }

            List<int> userRoleIds = (
                from userRole in this.Db.UserRoles
                where userRole.UserId == userId
                select userRole.RoleId
            ).ToList();

            bool foundRole = (
                from reportRole in this.Db.ReportRolePermissions
                where userRoleIds.Contains(reportRole.RoleId)
                    && reportRole.ReportId == reportId
                select reportRole
            ).Any();

            if (foundRole)
            {
                return;
            }

            throw new UnauthorizedAccessException("You cannot access the specified report.");
        }
    }
}
