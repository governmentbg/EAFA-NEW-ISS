using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CrossChecks;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;

namespace IARA.Infrastructure.Services
{
    public class CrossChecksAdministrationService : Service, ICrossChecksAdministrationService
    {
        private readonly IPermissionsService permissionsService;

        public CrossChecksAdministrationService(IARADbContext dbContext, IPermissionsService permissionsService)
            : base(dbContext)
        {
            this.permissionsService = permissionsService;
        }

        // Cross checks
        public IQueryable<CrossCheckDTO> GetAllCrossChecks(CrossChecksFilters filters)
        {
            IQueryable<CrossCheckDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllCrossChecks(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredCrossChecks(filters)
                    : GetFreeTextFilteredCrossChecks(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            return result;
        }

        public CrossCheckEditDTO GetCrossCheck(int id)
        {
            CrossCheckEditDTO result = (from check in Db.CrossChecks
                                        join report in Db.Reports on check.ReportId equals report.Id
                                        where check.Id == id
                                        select new CrossCheckEditDTO
                                        {
                                            Id = check.Id,
                                            Code = report.Code,
                                            Name = report.Name,
                                            ReportGroupId = report.ReportGroupId,
                                            ErrorLevel = check.ErrorLevel,
                                            DataSource = check.DataSource,
                                            DataSourceColumns = check.DataSourceColumns,
                                            CheckSource = check.CheckSource,
                                            CheckSourceColumns = check.CheckSourceColumns,
                                            CheckTableName = check.CheckTableName,
                                            Purpose = check.Puprose,
                                            AutoExecFrequency = Enum.Parse<CrossChecksAutoExecFrequencyEnum>(check.AutoExecFrequency),
                                            ReportSQL = report.ReportSql
                                        }).First();

            result.Users = GetCrossCheckUsers(id);
            result.Roles = GetCrossCheckRoles(id);

            return result;
        }

        public int AddCrossCheck(CrossCheckEditDTO crossCheck)
        {
            Report report = AddReport(crossCheck);

            CrossCheck entry = new CrossCheck
            {
                Report = report,
                ErrorLevel = crossCheck.ErrorLevel.Value,
                DataSource = crossCheck.DataSource,
                DataSourceColumns = crossCheck.DataSourceColumns,
                CheckSource = crossCheck.CheckSource,
                CheckSourceColumns = crossCheck.CheckSourceColumns,
                CheckTableName = crossCheck.CheckTableName,
                Puprose = crossCheck.Purpose,
                AutoExecFrequency = crossCheck.AutoExecFrequency.ToString()
            };

            AddOrEditCrossCheckReportGroup(report, crossCheck.ReportGroupId, crossCheck.ReportGroupName);

            AddCrossCheckUsers(report, crossCheck.Users);
            AddCrossCheckRoles(report, crossCheck.Roles);

            Db.CrossChecks.Add(entry);
            Db.SaveChanges();

            return entry.Id;
        }

        public void EditCrossCheck(CrossCheckEditDTO crossCheck)
        {
            CrossCheck dbCrossCheck = (from cc in Db.CrossChecks
                                       where cc.Id == crossCheck.Id
                                       select cc).First();

            Report dbReport = (from report in Db.Reports
                               where report.Id == dbCrossCheck.ReportId
                               select report).First();

            dbReport.Code = crossCheck.Code;
            dbReport.Name = crossCheck.Name;
            dbReport.ReportSql = crossCheck.ReportSQL;
            dbCrossCheck.ErrorLevel = crossCheck.ErrorLevel.Value;
            dbCrossCheck.DataSource = crossCheck.DataSource;
            dbCrossCheck.DataSourceColumns = crossCheck.DataSourceColumns;
            dbCrossCheck.CheckSource = crossCheck.CheckSource;
            dbCrossCheck.CheckSourceColumns = crossCheck.CheckSourceColumns;
            dbCrossCheck.CheckTableName = crossCheck.CheckTableName;
            dbCrossCheck.Puprose = crossCheck.Purpose;
            dbCrossCheck.AutoExecFrequency = crossCheck.AutoExecFrequency.ToString();

            AddOrEditCrossCheckReportGroup(dbReport, crossCheck.ReportGroupId, crossCheck.ReportGroupName);

            EditCrossCheckUsers(dbReport, crossCheck.Users);
            EditCrossCheckRoles(dbReport, crossCheck.Roles);

            Db.SaveChanges();
        }

        public void DeleteCrossCheck(int id)
        {
            DeleteRecordWithId(Db.CrossChecks, id);
            Db.SaveChanges();
        }

        public void UndoDeleteCrossCheck(int id)
        {
            UndoDeleteRecordWithId(Db.CrossChecks, id);
            Db.SaveChanges();
        }

        // Cross check results
        public IQueryable<CrossCheckResultDTO> GetAllCrossCheckResults(CrossCheckResultsFilters filters)
        {
            IQueryable<CrossCheckResultDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllCrossCheckResults(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredCrossCheckResults(filters)
                    : GetFreeTextFilteredCrossCheckResults(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            return result;
        }

        public void DeleteCrossCheckResult(int id)
        {
            DeleteRecordWithId(Db.CrossCheckResults, id);
            Db.SaveChanges();
        }

        public void UndoDeleteCrossCheckResult(int id)
        {
            UndoDeleteRecordWithId(Db.CrossCheckResults, id);
            Db.SaveChanges();
        }

        public void AssignCrossCheckResult(int checkResultId, int userId)
        {
            CrossCheckResult result = (from res in Db.CrossCheckResults
                                       where res.Id == checkResultId
                                       select res).First();

            result.AssignedUserId = userId;
            Db.SaveChanges();
        }

        public CrossCheckResolutionEditDTO GetCrossCheckResolution(int checkResultId)
        {
            CrossCheckResult result = (from res in Db.CrossCheckResults
                                       where res.Id == checkResultId
                                       select res).First();

            CrossCheckResolutionEditDTO resolution = new CrossCheckResolutionEditDTO
            {
                CheckResultId = result.Id,
                ResolutionId = result.ResolutionId,
                ResolutionDate = result.ValidTo == DefaultConstants.MAX_VALID_DATE ? default(DateTime?) : result.ValidTo,
                ResolutionDetails = result.ResolutionDetails
            };

            return resolution;
        }

        public void EditCrossCheckResolution(CrossCheckResolutionEditDTO resolution)
        {
            CrossCheckResult result = (from res in Db.CrossCheckResults
                                       where res.Id == resolution.CheckResultId.Value
                                       select res).First();

            result.ResolutionId = resolution.ResolutionId.Value;
            result.ValidTo = resolution.ResolutionDate.Value;
            result.ResolutionDetails = resolution.ResolutionDetails;

            Db.SaveChanges();
        }

        public bool HasUserAccessToCrossCheckResolution(int checkResultId, int userId)
        {
            // ако има право да назначава потребители към дадена кръстосана проверка, то има право да я вижда
            if (permissionsService.HasUserPermissionsAny(userId, Permissions.CrossCheckResultsAssign))
            {
                return true;
            }

            // ако няма право да назначава, но има право да разрешава кръстосани проверки, 
            // то проверката трябва да е назначена на съответния потребител
            if (permissionsService.HasUserPermissionsAny(userId, Permissions.CrossCheckResultsResolve))
            {
                int? assignedUserId = (from res in Db.CrossCheckResults
                                       where res.Id == checkResultId
                                       select res.AssignedUserId).First();

                if (assignedUserId.HasValue)
                {
                    return assignedUserId.Value == userId;
                }
            }

            return false;
        }

        public SimpleAuditDTO GetCrossCheckResultSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.CrossCheckResults, id);
            return audit;
        }

        // Nomenclatures
        public List<NomenclatureDTO> GetAllReportGroups()
        {
            List<NomenclatureDTO> result = (from gr in Db.ReportGroups
                                            where gr.GroupType == nameof(ReportTypesEnum.Cross)
                                            orderby gr.OrderNum
                                            select new NomenclatureDTO
                                            {
                                                Value = gr.Id,
                                                DisplayName = gr.Name,
                                                IsActive = gr.IsActive
                                            }).ToList();

            return result;
        }

        public List<NomenclatureDTO> GetCheckResolutionTypes()
        {
            List<NomenclatureDTO> result = GetCodeNomenclature(Db.NcheckResolutions);
            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.CrossChecks, id);
        }

        // Cross checks
        private IQueryable<CrossCheckDTO> GetAllCrossChecks(bool showInactive)
        {
            IQueryable<CrossCheckDTO> result = from check in Db.CrossChecks
                                               join report in Db.Reports on check.ReportId equals report.Id
                                               join reportGroup in Db.ReportGroups on report.ReportGroupId equals reportGroup.Id
                                               where showInactive == !check.IsActive
                                               orderby check.Id descending
                                               select new CrossCheckDTO
                                               {
                                                   Id = check.Id,
                                                   Code = report.Code,
                                                   Name = report.Name,
                                                   GroupName = reportGroup.Name,
                                                   Purpose = check.Puprose,
                                                   ErrorLevel = check.ErrorLevel,
                                                   DataSource = check.DataSource,
                                                   DataSourceColumns = check.DataSourceColumns,
                                                   CheckSource = check.CheckSource,
                                                   CheckSourceColumns = check.CheckSourceColumns,
                                                   CheckTableName = check.CheckTableName,
                                                   AutoExecFrequency = Enum.Parse<CrossChecksAutoExecFrequencyEnum>(check.AutoExecFrequency),
                                                   IsActive = check.IsActive
                                               };
            return result;
        }

        private IQueryable<CrossCheckDTO> GetParametersFilteredCrossChecks(CrossChecksFilters filters)
        {
            var crossChecks = from check in Db.CrossChecks
                              join report in Db.Reports on check.ReportId equals report.Id
                              join reportGroup in Db.ReportGroups on report.ReportGroupId equals reportGroup.Id
                              where check.IsActive == !filters.ShowInactiveRecords
                              orderby check.Id descending
                              select new
                              {
                                  check.Id,
                                  check.ReportId,
                                  report.Code,
                                  report.Name,
                                  GroupName = reportGroup.Name,
                                  check.Puprose,
                                  check.ErrorLevel,
                                  check.DataSource,
                                  check.DataSourceColumns,
                                  check.CheckSource,
                                  check.CheckSourceColumns,
                                  check.CheckTableName,
                                  check.AutoExecFrequency,
                                  check.IsActive
                              };

            if (!string.IsNullOrEmpty(filters.Name))
            {
                crossChecks = crossChecks.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.CheckedTable))
            {
                crossChecks = crossChecks.Where(x => x.CheckTableName.ToLower().Contains(filters.CheckedTable.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.DataSource))
            {
                crossChecks = crossChecks.Where(x => x.DataSource.ToLower().Contains(filters.DataSource.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.ReportGroupName))
            {
                crossChecks = crossChecks.Where(x => x.GroupName.ToLower().Contains(filters.ReportGroupName));
            }

            if (filters.ErrorLevels != null && filters.ErrorLevels.Count > 0)
            {
                crossChecks = crossChecks.Where(x => filters.ErrorLevels.Contains(x.ErrorLevel));
            }

            if (filters.AutoExecFrequencyCodes != null && filters.AutoExecFrequencyCodes.Count > 0)
            {
                crossChecks = crossChecks.Where(x => filters.AutoExecFrequencyCodes.Contains(x.AutoExecFrequency));
            }

            IQueryable<CrossCheckDTO> result = from check in crossChecks
                                               join report in Db.Reports on check.ReportId equals report.Id
                                               join reportGroup in Db.ReportGroups on report.ReportGroupId equals reportGroup.Id
                                               select new CrossCheckDTO
                                               {
                                                   Id = check.Id,
                                                   Code = report.Code,
                                                   Name = report.Name,
                                                   GroupName = reportGroup.Name,
                                                   Purpose = check.Puprose,
                                                   ErrorLevel = check.ErrorLevel,
                                                   DataSource = check.DataSource,
                                                   DataSourceColumns = check.DataSourceColumns,
                                                   CheckSource = check.CheckSource,
                                                   CheckSourceColumns = check.CheckSourceColumns,
                                                   CheckTableName = check.CheckTableName,
                                                   AutoExecFrequency = Enum.Parse<CrossChecksAutoExecFrequencyEnum>(check.AutoExecFrequency),
                                                   IsActive = check.IsActive
                                               };

            return result;
        }

        private IQueryable<CrossCheckDTO> GetFreeTextFilteredCrossChecks(string text, bool showInactive)
        {
            text = text.ToLower();

            IQueryable<CrossCheckDTO> result = from check in Db.CrossChecks
                                               join report in Db.Reports on check.ReportId equals report.Id
                                               join reportGroup in Db.ReportGroups on report.ReportGroupId equals reportGroup.Id
                                               where showInactive == !check.IsActive
                                                  && (report.Code.ToLower().Contains(text)
                                                    || report.Name.ToLower().Contains(text)
                                                    || reportGroup.Name.ToLower().Contains(text)
                                                    || check.Puprose.ToLower().Contains(text)
                                                    || check.ErrorLevel.ToString().Contains(text)
                                                    || check.DataSource.ToLower().Contains(text)
                                                    || check.DataSourceColumns.ToLower().Contains(text)
                                                    || check.CheckSource.ToLower().Contains(text)
                                                    || check.CheckSourceColumns.ToLower().Contains(text)
                                                    || check.CheckTableName.ToLower().Contains(text))
                                               orderby check.Id descending
                                               select new CrossCheckDTO
                                               {
                                                   Id = check.Id,
                                                   Code = report.Code,
                                                   Name = report.Name,
                                                   GroupName = reportGroup.Name,
                                                   Purpose = check.Puprose,
                                                   ErrorLevel = check.ErrorLevel,
                                                   DataSource = check.DataSource,
                                                   DataSourceColumns = check.DataSourceColumns,
                                                   CheckSource = check.CheckSource,
                                                   CheckSourceColumns = check.CheckSourceColumns,
                                                   CheckTableName = check.CheckTableName,
                                                   AutoExecFrequency = Enum.Parse<CrossChecksAutoExecFrequencyEnum>(check.AutoExecFrequency),
                                                   IsActive = check.IsActive
                                               };
            return result;
        }

        private Report AddReport(CrossCheckEditDTO crossCheck)
        {
            Report report = new Report
            {
                ReportType = nameof(ReportTypesEnum.Cross),
                Code = crossCheck.Code,
                Name = crossCheck.Name,
                ReportSql = crossCheck.ReportSQL
            };

            return report;
        }

        private List<NomenclatureDTO> GetCrossCheckUsers(int crossCheckId)
        {
            int reportId = (from cc in Db.CrossChecks
                            where cc.Id == crossCheckId
                            select cc.ReportId).First();

            List<NomenclatureDTO> result = (from repUser in Db.ReportUserPermissions
                                            join user in Db.Users on repUser.UserId equals user.Id
                                            join person in Db.Persons on user.PersonId equals person.Id
                                            where repUser.ReportId == reportId
                                                && repUser.IsActive
                                            select new NomenclatureDTO
                                            {
                                                Value = user.Id,
                                                DisplayName = person.FirstName + " " + person.LastName + "(" + user.Username + ")",
                                                IsActive = true
                                            }).ToList();

            return result;
        }

        private List<NomenclatureDTO> GetCrossCheckRoles(int crossCheckId)
        {
            int reportId = (from cc in Db.CrossChecks
                            where cc.Id == crossCheckId
                            select cc.ReportId).First();

            List<NomenclatureDTO> result = (from repRole in Db.ReportRolePermissions
                                            join role in Db.Roles on repRole.RoleId equals role.Id
                                            where repRole.ReportId == reportId
                                                && repRole.IsActive
                                            select new NomenclatureDTO
                                            {
                                                Value = role.Id,
                                                DisplayName = role.Name,
                                                IsActive = true
                                            }).ToList();

            return result;
        }

        private void AddCrossCheckUsers(Report report, List<NomenclatureDTO> users)
        {
            if (users != null)
            {
                foreach (NomenclatureDTO user in users)
                {
                    ReportUserPermission repUser = new ReportUserPermission
                    {
                        Report = report,
                        UserId = user.Value
                    };

                    Db.ReportUserPermissions.Add(repUser);
                }
            }
        }

        private void AddCrossCheckRoles(Report report, List<NomenclatureDTO> roles)
        {
            if (roles != null)
            {
                foreach (NomenclatureDTO role in roles)
                {
                    ReportRolePermission repRole = new ReportRolePermission
                    {
                        Report = report,
                        RoleId = role.Value
                    };

                    Db.ReportRolePermissions.Add(repRole);
                }
            }
        }

        private void EditCrossCheckUsers(Report report, List<NomenclatureDTO> users)
        {
            List<ReportUserPermission> currentRepUsers = (from repUser in Db.ReportUserPermissions
                                                          where repUser.ReportId == report.Id
                                                          select repUser).ToList();

            if (users != null)
            {
                List<int> newUserIds = users.Where(x => x.IsActive).Select(x => x.Value).ToList();

                List<int> currentUserIds = currentRepUsers.Where(x => x.IsActive).Select(x => x.UserId).ToList();
                List<int> userIdsToAdd = newUserIds.Where(x => !currentUserIds.Contains(x)).ToList();
                List<int> userIdsToRemove = currentUserIds.Where(x => !newUserIds.Contains(x)).ToList();

                foreach (int userId in userIdsToAdd)
                {
                    ReportUserPermission repUser = currentRepUsers.Where(x => x.UserId == userId).SingleOrDefault();
                    if (repUser != null)
                    {
                        repUser.IsActive = true;
                    }
                    else
                    {
                        ReportUserPermission entry = new ReportUserPermission
                        {
                            Report = report,
                            UserId = userId
                        };
                        Db.ReportUserPermissions.Add(entry);
                    }
                }

                foreach (int userId in userIdsToRemove)
                {
                    ReportUserPermission repUser = currentRepUsers.Where(x => x.UserId == userId).Single();
                    repUser.IsActive = false;
                }
            }
            else
            {
                foreach (ReportUserPermission repUser in currentRepUsers)
                {
                    repUser.IsActive = false;
                }
            }
        }

        private void EditCrossCheckRoles(Report report, List<NomenclatureDTO> roles)
        {
            List<ReportRolePermission> currentRepRoles = (from repRole in Db.ReportRolePermissions
                                                          where repRole.ReportId == report.Id
                                                          select repRole).ToList();

            if (roles != null)
            {
                List<int> newRoleIds = roles.Where(x => x.IsActive).Select(x => x.Value).ToList();

                List<int> currentRoleIds = currentRepRoles.Where(x => x.IsActive).Select(x => x.RoleId).ToList();
                List<int> roleIdsToAdd = newRoleIds.Where(x => !currentRoleIds.Contains(x)).ToList();
                List<int> roleIdsToRemove = currentRoleIds.Where(x => !newRoleIds.Contains(x)).ToList();

                foreach (int roleId in roleIdsToAdd)
                {
                    ReportRolePermission repRole = currentRepRoles.Where(x => x.RoleId == roleId).SingleOrDefault();
                    if (repRole != null)
                    {
                        repRole.IsActive = true;
                    }
                    else
                    {
                        ReportRolePermission entry = new ReportRolePermission
                        {
                            Report = report,
                            RoleId = roleId
                        };
                        Db.ReportRolePermissions.Add(entry);
                    }
                }

                foreach (int roleId in roleIdsToRemove)
                {
                    ReportRolePermission repRole = currentRepRoles.Where(x => x.RoleId == roleId).Single();
                    repRole.IsActive = false;
                }
            }
            else
            {
                foreach (ReportRolePermission repRole in currentRepRoles)
                {
                    repRole.IsActive = false;
                }
            }
        }

        private void AddOrEditCrossCheckReportGroup(Report report, int? reportGroupId, string reportGroupName)
        {
            if (reportGroupId.HasValue)
            {
                report.ReportGroupId = reportGroupId.Value;
            }
            else if (!string.IsNullOrEmpty(reportGroupName))
            {
                ReportGroup group = (from gr in Db.ReportGroups
                                     where gr.Name == reportGroupName
                                        && gr.GroupType == nameof(ReportTypesEnum.Cross)
                                     select gr).SingleOrDefault();

                if (group != null)
                {
                    report.ReportGroup = group;
                }
                else
                {
                    report.ReportGroup = new ReportGroup
                    {
                        GroupType = nameof(ReportTypesEnum.Cross),
                        Name = reportGroupName,
                        Description = "crosscheck",
                        OrderNum = 9999
                    };
                }
            }
        }

        // Cross check results
        private IQueryable<CrossCheckResultDTO> GetAllCrossCheckResults(bool showInactive)
        {
            IQueryable<CrossCheckResultDTO> result = from res in Db.CrossCheckResults
                                                     join check in Db.CrossChecks on res.CheckId equals check.Id
                                                     join report in Db.Reports on check.ReportId equals report.Id
                                                     join user in Db.Users on res.AssignedUserId equals user.Id into us
                                                     from user in us.DefaultIfEmpty()
                                                     join person in Db.Persons on user.PersonId equals person.Id into per
                                                     from person in per.DefaultIfEmpty()
                                                     join resolution in Db.NcheckResolutions on res.ResolutionId equals resolution.Id
                                                     where res.IsActive == !showInactive
                                                     orderby res.Id descending
                                                     select new CrossCheckResultDTO
                                                     {
                                                         Id = res.Id,
                                                         CheckCode = report.Code,
                                                         CheckName = report.Name,
                                                         PageCode = res.PageCode,
                                                         TableId = res.TableId,
                                                         ErrorDescription = res.ErrorDescription,
                                                         ValidFrom = res.ValidFrom,
                                                         ValidTo = res.ValidTo == DefaultConstants.MAX_VALID_DATE ? default(DateTime?) : res.ValidTo,
                                                         AssignedUserId = res.AssignedUserId,
                                                         AssignedUser = person != null
                                                            ? person.FirstName + " " + person.LastName + (user != null ? " (" + user.Username + ")" : "")
                                                            : null,
                                                         Resolution = resolution.Name,
                                                         IsActive = res.IsActive
                                                     };

            return result;
        }

        private IQueryable<CrossCheckResultDTO> GetParametersFilteredCrossCheckResults(CrossCheckResultsFilters filters)
        {
            var query = from res in Db.CrossCheckResults
                        join check in Db.CrossChecks on res.CheckId equals check.Id
                        join report in Db.Reports on check.ReportId equals report.Id
                        join user in Db.Users on res.AssignedUserId equals user.Id into us
                        from user in us.DefaultIfEmpty()
                        join person in Db.Persons on user.PersonId equals person.Id into per
                        from person in per.DefaultIfEmpty()
                        join resolution in Db.NcheckResolutions on res.ResolutionId equals resolution.Id
                        where res.IsActive == !filters.ShowInactiveRecords
                        orderby res.Id descending
                        select new
                        {
                            res.Id,
                            CheckCode = report.Code,
                            CheckName = report.Name,
                            res.PageCode,
                            res.TableId,
                            res.ErrorDescription,
                            res.ValidFrom,
                            ValidTo = res.ValidTo == DefaultConstants.MAX_VALID_DATE ? default(DateTime?) : res.ValidTo,
                            res.AssignedUserId,
                            AssignedUser = person != null
                               ? person.FirstName + " " + person.LastName + (user != null ? " (" + user.Username + ")" : "")
                               : null,
                            res.ResolutionId,
                            Resolution = resolution.Name,
                            res.IsActive
                        };

            if (!string.IsNullOrEmpty(filters.CheckCode))
            {
                query = query.Where(x => x.CheckCode.ToLower().Contains(filters.CheckCode.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.CheckName))
            {
                query = query.Where(x => x.CheckName.ToLower().Contains(filters.CheckName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.CheckTableName))
            {
                query = query.Where(x => x.PageCode.ToLower().Contains(filters.CheckTableName.ToLower()));
            }

            if (filters.ResolutionIds != null && filters.ResolutionIds.Count > 0)
            {
                query = query.Where(x => filters.ResolutionIds.Contains(x.ResolutionId));
            }

            if (!string.IsNullOrEmpty(filters.TableId))
            {
                query = query.Where(x => x.TableId.ToString().Contains(filters.TableId));
            }

            if (!string.IsNullOrEmpty(filters.ErrorDescription))
            {
                query = query.Where(x => x.ErrorDescription.ToLower().Contains(filters.ErrorDescription.ToLower()));
            }

            if (filters.ValidFrom.HasValue)
            {
                query = query.Where(x => x.ValidFrom >= filters.ValidFrom.Value);
            }

            if (filters.ValidTo.HasValue)
            {
                query = query.Where(x => x.ValidTo <= filters.ValidTo.Value);
            }

            if (filters.AssignedUserId.HasValue)
            {
                query = query.Where(x => x.AssignedUserId.HasValue && x.AssignedUserId.Value == filters.AssignedUserId.Value);
            }

            IQueryable<CrossCheckResultDTO> result = from res in query
                                                     select new CrossCheckResultDTO
                                                     {
                                                         Id = res.Id,
                                                         CheckCode = res.CheckCode,
                                                         CheckName = res.CheckName,
                                                         PageCode = res.PageCode,
                                                         TableId = res.TableId,
                                                         ErrorDescription = res.ErrorDescription,
                                                         ValidFrom = res.ValidFrom,
                                                         ValidTo = res.ValidTo,
                                                         AssignedUserId = res.AssignedUserId,
                                                         AssignedUser = res.AssignedUser,
                                                         Resolution = res.Resolution,
                                                         IsActive = res.IsActive
                                                     };

            return result;
        }

        private IQueryable<CrossCheckResultDTO> GetFreeTextFilteredCrossCheckResults(string text, bool showInactive)
        {
            text = text.ToLower();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            IQueryable<CrossCheckResultDTO> result = from res in Db.CrossCheckResults
                                                     join check in Db.CrossChecks on res.CheckId equals check.Id
                                                     join report in Db.Reports on check.ReportId equals report.Id
                                                     join user in Db.Users on res.AssignedUserId equals user.Id into us
                                                     from user in us.DefaultIfEmpty()
                                                     join person in Db.Persons on user.PersonId equals person.Id into per
                                                     from person in per.DefaultIfEmpty()
                                                     join resolution in Db.NcheckResolutions on res.ResolutionId equals resolution.Id
                                                     where res.IsActive == !showInactive
                                                        && (report.Code.ToLower().Contains(text)
                                                         || report.Name.ToLower().Contains(text)
                                                         || res.PageCode.ToLower().Contains(text)
                                                         || res.ErrorDescription.ToLower().Contains(text)
                                                         || res.CheckId.ToString().Contains(text)
                                                         || res.TableId.ToString().Contains(text)
                                                         || (searchDate.HasValue && searchDate.Value == res.ValidFrom.Date)
                                                         || (searchDate.HasValue && searchDate.Value == res.ValidTo.Date)
                                                         || (person != null
                                                                ? person.FirstName + " " + person.LastName + (user != null ? " (" + user.Username + ")" : "")
                                                                : "").Contains(text)
                                                         || resolution.Name.ToLower().Contains(text))
                                                     orderby res.Id descending
                                                     select new CrossCheckResultDTO
                                                     {
                                                         Id = res.Id,
                                                         CheckCode = report.Code,
                                                         CheckName = report.Name,
                                                         PageCode = res.PageCode,
                                                         TableId = res.TableId,
                                                         ErrorDescription = res.ErrorDescription,
                                                         ValidFrom = res.ValidFrom,
                                                         ValidTo = res.ValidTo == DefaultConstants.MAX_VALID_DATE ? default(DateTime?) : res.ValidTo,
                                                         AssignedUserId = res.AssignedUserId,
                                                         AssignedUser = person != null
                                                            ? person.FirstName + " " + person.LastName + (user != null ? " (" + user.Username + ")" : "")
                                                            : null,
                                                         Resolution = resolution.Name,
                                                         IsActive = res.IsActive
                                                     };

            return result;
        }
    }
}
