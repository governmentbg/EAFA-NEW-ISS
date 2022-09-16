using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.GridModels;
using IARA.Common.Resources;
using IARA.Common.TempFileUtils;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools;
using IARA.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;
using TL.JasperReports.Integration;
using TL.JasperReports.Integration.Enums;


namespace IARA.Infrastructure.Services.Reports
{
    public class ReportService : Service, IReportService
    {
        private const string CURRENT_USER_PARAMETER_NAME = "CurrentUserID";
        private const string PARAMETER_PREFIX = "@";

        private readonly IJasperReportsClient jasperReportExecutionService;

        private Regex columnNamesExp = new Regex(@"(?:([a-zA-Z_.""]{1,})\s{1,}(?:as|AS)\s{1,}""{0,1}([a-zA-Zа-яА-Я_\s]{1,})""{0,1})");
        private string COUNT_QUERY(string query) => $"SELECT COUNT(*) FROM ({query}) as report";
        private string PAGE_QUERY(string query, int skip, int take, string order) => $"SELECT * FROM ({query}) AS report {(!string.IsNullOrEmpty(order) ? "ORDER BY " + order : "")} OFFSET {skip} LIMIT {take}";

        private IFilesSweeper filesSweeper;

        public ReportService(IARADbContext dbContext, IJasperReportsClient jasperReportExecutionService, IFilesSweeper filesSweeper)
            : base(dbContext)
        {
            this.filesSweeper = filesSweeper;
            this.jasperReportExecutionService = jasperReportExecutionService;
        }

        public BaseGridResultModel<IDictionary<string, object>> ExecutePagedSql(ReportGridRequestDTO request)
        {
            request.SqlQuery = Db.Reports
                                 .Where(x => x.Id == request.ReportId)
                                 .Select(x => x.ReportSql)
                                 .FirstOrDefault();

            request.Parameters = ValidateParameters(request.ReportId, request.Parameters);

            DbConnection connection = this.Db.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            DbCommand command = this.GetQueryCommand(connection, COUNT_QUERY(request.SqlQuery), request.Parameters, request.UserId);

            var matches = columnNamesExp.Matches(request.SqlQuery);

            long totalCount = (long)command.ExecuteScalar();

            string order = "";

            if (request.SortColumns != null && request.SortColumns.Any())
            {
                //Dictionary<string, string> columnNamesDict = matches.Select(x => x.Groups.Select(y => y.Value).ToList())
                //    .ToDictionary(x => x[2].Replace(' ', '_'), x => x[1]);

                //foreach (var column in request.SortColumns)
                //{
                //    if (columnNamesDict.ContainsKey(column.PropertyName))
                //    {
                //        column.PropertyName = columnNamesDict[column.PropertyName];
                //    }
                //}

                order = string.Join(',', request.SortColumns.Select(x => $"report.\"{x.PropertyName.Replace('_', ' ')}\" {x.SortOrder}"));
            }

            string query = PAGE_QUERY(request.SqlQuery, (request.PageNumber - 1) * request.PageSize, request.PageSize, order);
            command = this.GetQueryCommand(connection, query, request.Parameters, request.UserId);

            DbDataReader reader = command.ExecuteReader();

            Dictionary<string, object> record = new Dictionary<string, object>();
            List<IDictionary<string, object>> records = new List<IDictionary<string, object>>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        object value = reader.GetValue(i);

                        record.Add(columnName.Replace(" ", "_"), value?.ToString());
                    }

                    records.Add(record);
                    record = new Dictionary<string, object>();
                }
            }

            reader.Close();

            return new BaseGridResultModel<IDictionary<string, object>>
            {
                Records = records,
                TotalRecordsCount = totalCount
            };
        }

        private DbDataReader BuildCommandAndOpenReader(string sqlQuery, List<ExecutionParamDTO> parameters, int? userID)
        {
            DbConnection connection = this.Db.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            DbCommand command = this.GetQueryCommand(connection, sqlQuery, parameters, userID);

            DbDataReader reader = command.ExecuteReader();
            return reader;
        }

        public IEnumerable<IDictionary<string, object>> ExecuteRawSql(string sqlQuery, List<ExecutionParamDTO> parameters, int? userID)
        {
            using DbDataReader reader = BuildCommandAndOpenReader(sqlQuery, parameters, userID);

            IDictionary<string, object> record = new Dictionary<string, object>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        object value = reader.GetValue(i);

                        record.TryAdd(columnName.Replace(" ", "_"), value);
                    }

                    yield return record;
                    record = new Dictionary<string, object>();
                }
            }
            else
            {
                yield break;
            }

            reader.Close();
        }

        public Stream GenerateCSV(int reportId, List<ExecutionParamDTO> parameters, int? userId, out string reportName)
        {
            parameters = ValidateParameters(reportId, parameters);

            var reportData = this.Db.Reports
                .Where(x => x.Id == reportId)
                .Select(x => new
                {
                    x.Name,
                    x.ReportSql
                }).FirstOrDefault();

            reportName = reportData.Name;

            TempFileStream fileStream = new TempFileStream(TempFileBuilder.GenerateUniqueFileName());

            filesSweeper.AddFileForRemoval(fileStream);

            var enumerableResult = this.ExecuteRawSql(reportData.ReportSql, parameters, userId);

            return new ExcelExporter(fileStream, "").ExportXlsx(enumerableResult);
        }

        public List<ReportNodeDTO> GetAllReportNodes()
        {
            List<ReportNodeDTO> reportsHierarchy = new List<ReportNodeDTO>();

            var reports = (from report in Db.Reports
                           where report.ReportType != ReportTypesEnum.Cross.ToString()
                           select new
                           {
                               Id = report.Id,
                               Name = report.Name,
                               //IconCode = x.IconName,
                               IconName = report.IconName,
                               IsActive = report.IsActive,
                               ReportGroupId = report.ReportGroupId
                           })
                          .ToLookup(x => x.ReportGroupId, x => new ReportNodeDTO
                          {
                              Id = x.Id,
                              Name = x.Name,
                              IconName = x.IconName,
                              IsActive = x.IsActive
                          });

            List<ReportNodeDTO> reportGroups = Db.ReportGroups
                .Where(x => x.GroupType != ReportTypesEnum.Cross.ToString())
                .Select(x => new ReportNodeDTO
                {
                    IsActive = x.IsActive,
                    Name = x.Name,
                    Id = x.Id
                }).ToList();

            var emptyList = new List<ReportNodeDTO>();

            foreach (var reportGroup in reportGroups)
            {
                List<ReportNodeDTO> reportsForGroup = reports[reportGroup.Id].ToList();

                if (reportsForGroup.Any())
                {
                    reportGroup.Children = reportsForGroup;

                }
                else
                {
                    reportGroup.Children = emptyList;
                }

                reportsHierarchy.Add(reportGroup);
            }

            return reportsHierarchy;
        }

        public HashSet<ReportSchema> GetColumnNames(int reportId, List<ExecutionParamDTO> parameters, int? userID)
        {
            string sqlQuery = (from report in this.Db.Reports
                               where report.Id == reportId
                               select report.ReportSql).First();

            return this.GetColumnNames(sqlQuery, parameters, userID);
        }

        public HashSet<ReportSchema> GetColumnNames(string sqlQuery, List<ExecutionParamDTO> parameters, int? userID)
        {
            HashSet<ReportSchema> propertyNames = new HashSet<ReportSchema>();

            using DbDataReader reader = BuildCommandAndOpenReader(sqlQuery, parameters, userID);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                propertyNames.Add(new ReportSchema
                {
                    PropertyName = columnName.Replace(" ", "_"),
                    PropertyDisplayName = columnName
                });
            }

            reader.Close();

            return propertyNames;
        }

        public ExecuteReportDTO GetExecuteReport(int id, int? userId)
        {
            ExecuteReportDTO wantedReport = (from report in this.Db.Reports
                                             where report.Id == id
                                             select new ExecuteReportDTO
                                             {
                                                 Id = report.Id,
                                                 Name = report.Name,
                                                 ReportGroupId = report.ReportGroupId,
                                                 SqlQuery = report.ReportSql,
                                                 ReportType = Enum.Parse<ReportTypesEnum>(report.ReportType)
                                             }).First();

            wantedReport.Parameters = this.GetExecuteReportParameters(id, userId);
            return wantedReport;
        }

        public List<ReportParameterExecuteDTO> GetExecuteReportParameters(int reportId, int? userId)
        {
            List<ReportParameterExecuteDTO> listOfWantedParameters = (from parameter in this.Db.ReportParameters
                                                                      join nParameter in this.Db.NReportParameters on parameter.ParameterId equals nParameter.Id
                                                                      where parameter.ReportId == reportId
                                                                      && parameter.IsActive == true
                                                                      orderby parameter.OrderNum
                                                                      select new ReportParameterExecuteDTO
                                                                      {
                                                                          Id = parameter.Id,
                                                                          ParameterId = nParameter.Id,
                                                                          Name = nParameter.Name,
                                                                          Code = nParameter.Code,
                                                                          Description = nParameter.Description,
                                                                          DataType = Enum.Parse<ReportParameterTypeEnum>(nParameter.DataType),
                                                                          DefaultValue = parameter.DefaultValue,
                                                                          IsActive = parameter.IsActive,
                                                                          IsMandatory = parameter.IsMandatory,
                                                                          ErrorMessage = parameter.ErrorMessage,
                                                                          Pattern = parameter.Pattern,
                                                                          NomenclatureSQL = nParameter.NomenclatureSql
                                                                      }).ToList();

            foreach (ReportParameterExecuteDTO parameter in listOfWantedParameters)
            {
                if (!string.IsNullOrEmpty(parameter.NomenclatureSQL))
                {
                    List<IDictionary<string, object>> queryResult = this.ExecuteRawSql(parameter.NomenclatureSQL, new List<ExecutionParamDTO>(), userId).ToList();
                    List<string> keys = queryResult.First().Keys.ToList();
                    parameter.ParameterTypeNomenclatures = (from item in queryResult
                                                            select new NomenclatureDTO
                                                            {
                                                                DisplayName = item[keys[1]].ToString(),
                                                                Value = int.Parse(item[keys[0]].ToString())
                                                            }).ToList();
                }
            }

            return listOfWantedParameters;
        }

        public ReportGroupDTO GetGroup(int id)
        {
            ReportGroupDTO wantedGroup = (from groupFromDb in this.Db.ReportGroups
                                          where groupFromDb.Id == id
                                          select new ReportGroupDTO
                                          {
                                              Id = groupFromDb.Id,
                                              Description = groupFromDb.Description,
                                              Name = groupFromDb.Name,
                                              GroupType = groupFromDb.GroupType
                                          }).First();

            return wantedGroup;
        }

        public ReportDTO GetReport(int id)
        {
            ReportDTO wantedReport = (from report in this.Db.Reports
                                      join lastUser in this.Db.Users on report.LastRunUserId equals lastUser.Id into users
                                      from user in users.DefaultIfEmpty()
                                      where report.Id == id
                                      select new ReportDTO
                                      {
                                          Id = report.Id,
                                          Name = report.Name,
                                          Code = report.Code,
                                          Description = report.Description,
                                          ReportType = report.ReportType,
                                          IconName = report.IconName,
                                          LastRunUsername = report.LastRunUserId != null
                                                                ? user.Username
                                                                : null,
                                          LastRunDateTime = report.LastRunDateTime,
                                          LastRunDurationSec = report.LastRunDurationSec,
                                          ReportGroupId = report.ReportGroupId,
                                          ReportSQL = report.ReportSql
                                      }).First();

            wantedReport.Parameters = this.GetReportParameters(id);
            wantedReport.Users = this.GetReportUserPermissions(id);
            wantedReport.Roles = this.GetReportRolePermissions(id);

            return wantedReport;
        }

        public List<ReportNodeDTO> GetReportNodes(int? userId)
        {
            List<ReportNodeDTO> reportsHierarchy = new List<ReportNodeDTO>();

            HashSet<int> uniqueRoleIds = GetUserRoleIds(userId);

            var reports = (from report in Db.Reports
                           join reportRole in Db.ReportRolePermissions on report.Id equals reportRole.ReportId into left
                           from reportRole in left.DefaultIfEmpty()
                           join reportUser in Db.ReportUserPermissions on report.Id equals reportUser.UserId into left1
                           from reportUser in left1.DefaultIfEmpty()
                           where report.IsActive
                           && report.ReportType != ReportTypesEnum.Cross.ToString()
                           && ((reportRole != null && reportRole.IsActive && uniqueRoleIds.Contains(reportRole.RoleId))
                            || (userId != null && userId.HasValue && reportUser != null && reportUser.IsActive && reportUser.UserId == userId.Value))
                           select new
                           {
                               Id = report.Id,
                               Name = report.Name,
                               //IconCode = x.IconName,
                               IconName = report.IconName,
                               IsActive = report.IsActive,
                               ReportGroupId = report.ReportGroupId
                           })
                           .Distinct()
                           .ToLookup(x => x.ReportGroupId, x => new ReportNodeDTO
                           {
                               Id = x.Id,
                               Name = x.Name,
                               IconName = x.IconName,
                               IsActive = x.IsActive
                           });

            List<ReportNodeDTO> reportGroups = Db.ReportGroups
                .Where(x => x.IsActive && x.GroupType != ReportTypesEnum.Cross.ToString())
                .Select(x => new ReportNodeDTO
                {
                    IsActive = x.IsActive,
                    Name = x.Name,
                    Id = x.Id
                }).ToList();

            foreach (var reportGroup in reportGroups)
            {
                List<ReportNodeDTO> reportsForGroup = reports[reportGroup.Id].ToList();

                if (reportsForGroup.Any())
                {
                    reportGroup.Children = reportsForGroup;
                    reportsHierarchy.Add(reportGroup);
                }
            }

            return reportsHierarchy;
        }

        public SimpleAuditDTO GetReportGroupsSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(this.Db.ReportGroups, id);
        }

        public SimpleAuditDTO GetReportParametersSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(this.Db.ReportParameters, id);
        }

        public SimpleAuditDTO GetParametersSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(this.Db.NReportParameters, id);
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(this.Db.Reports, id);
        }

        public List<TableNodeDTO> GetTableNodes()
        {
            List<TableNodeDTO> listOfTableNodes = (from groupOfSchemas in (from entity in this.Db.Model.GetEntityTypes()
                                                                           select new
                                                                           {
                                                                               Name = entity.GetTableName(),
                                                                               DisplayName = entity.GetTableName(),
                                                                               Schema = entity.GetSchema(),
                                                                               Children = (from property in entity.GetProperties()
                                                                                           select new TableNodeDTO
                                                                                           {
                                                                                               Name = property.Name,
                                                                                               DisplayName = property.GetColumnName()
                                                                                           }).ToList()
                                                                           })
                                                   group groupOfSchemas by groupOfSchemas.Schema into groupedSchemas
                                                   select new TableNodeDTO
                                                   {
                                                       Name = groupedSchemas.Key,
                                                       DisplayName = groupedSchemas.Key,
                                                       Children = (from table in groupedSchemas
                                                                   select new TableNodeDTO
                                                                   {
                                                                       Name = table.Name,
                                                                       DisplayName = table.DisplayName,
                                                                       Children = table.Children
                                                                   }).ToList()
                                                   }).ToList();

            return listOfTableNodes;
        }

        public bool HasUserAccessToReport(int reportId, int? userId)
        {
            HashSet<int> uniqueRoleIds = GetUserRoleIds(userId);

            bool hasAccess = (from report in Db.Reports
                              join reportRole in Db.ReportRolePermissions on report.Id equals reportRole.ReportId into left
                              from reportRole in left.DefaultIfEmpty()
                              join reportUser in Db.ReportUserPermissions on report.Id equals reportUser.UserId into left1
                              from reportUser in left1.DefaultIfEmpty()
                              where report.Id == reportId && report.IsActive
                              && report.ReportType != ReportTypesEnum.Cross.ToString()
                              && ((reportRole != null && reportRole.IsActive && uniqueRoleIds.Contains(reportRole.RoleId))
                               || (userId != null && userId.HasValue && reportUser != null && reportUser.IsActive && reportUser.UserId == userId.Value))
                              select report.Id).Any();

            return hasAccess;
        }

        public Task<Stream> RunReport(int id, List<ExecutionParamDTO> parameters, int? userId, out string reportName)
        {
            parameters = ValidateParameters(id, parameters);

            var dbReport = (from report in this.Db.Reports
                            where report.Id == id
                            select new
                            {
                                report.Code,
                                report.Name
                            }).First();

            reportName = dbReport.Name;

            var userParameter = parameters.Where(x => x.Name == CURRENT_USER_PARAMETER_NAME).FirstOrDefault();

            if (userParameter != null)
            {
                userParameter.Value = userId != null && userId.HasValue ? userId.Value.ToString() : null;
            }

            return this.jasperReportExecutionService.RunReport($"{DefaultConstants.BASE_REPORTS_PATH}/{dbReport.Code}",
                OutputFormats.pdf,
                parameters.ToDictionary(f => f.Name, f => f.Value)
            );
        }

        private List<ExecutionParamDTO> ValidateParameters(int reportId, List<ExecutionParamDTO> parameters)
        {
            List<ExecutionParamDTO> dbParameters = this.GetReportParameters(reportId)
                .Select(x => new ExecutionParamDTO
                {
                    Name = x.ParameterName,
                    DefaultValue = x.DefaultValue,
                    IsMandatory = x.IsMandatory,
                    Type = x.DataType.Value,
                    Pattern = x.Pattern
                }).ToList();

            var dbParameterNames = dbParameters.Select(x => x.Name).ToList();
            var parameterNames = parameters.Select(x => x.Name).ToList();
            int count = dbParameterNames.Except(parameterNames).Count();
            count += parameterNames.Except(dbParameterNames).Count();

            if (count > 0)
            {
                throw new ArgumentException("Неправилен брой параметри");
            }

            foreach (var dbParameter in dbParameters)
            {
                dbParameter.Value = parameters.First(x => x.Name == dbParameter.Name).Value;

                if (string.IsNullOrEmpty(dbParameter.Value))
                {
                    dbParameter.Value = dbParameter.DefaultValue;
                }
            }

            var emptyParameters = dbParameters.Where(x => x.IsMandatory && string.IsNullOrEmpty(x.Value)).ToList();

            if (emptyParameters.Any())
            {
                List<string> errors = new List<string>();
                foreach (var parameter in emptyParameters)
                {
                    string error = $"{string.Format(ErrorResources.msgRequired, parameter.Name)}";
                    errors.Add(error);
                }

                throw new ArgumentException(string.Join(Environment.NewLine, errors));
            }

            return dbParameters;
        }

        private object ConvertFromParamType(string paramValue, ReportParameterTypeEnum paramType)
        {
            object value = paramValue;

            if (paramValue != null && paramValue != string.Empty)
            {
                switch (paramType)
                {
                    case ReportParameterTypeEnum.Int:
                        return Convert.ToInt32(paramValue);
                    case ReportParameterTypeEnum.Decimal:
                        return Convert.ToDecimal(paramValue);
                    case ReportParameterTypeEnum.Date:
                    case ReportParameterTypeEnum.Time:
                    case ReportParameterTypeEnum.DateTime:
                        return DateTime.Parse(paramValue);
                    case ReportParameterTypeEnum.Nomenclature:
                    case ReportParameterTypeEnum.String:
                    default:
                        return paramValue;
                }
            }

            return value;
        }

        private DbParameter GetParameter(DbCommand command, string name, object value)
        {
            DbParameter param = command.CreateParameter();

            param.ParameterName = name;
            param.Value = value;

            return param;
        }

        private DbCommand GetQueryCommand(DbConnection connection, string sqlQuery, List<ExecutionParamDTO> parameters, int? userID)
        {
            if (!SQLReportUtils.ValidateDatasourceQuery(sqlQuery))
            {
                throw new ArgumentException("Invalid sql query");
            }

            List<DbParameter> dbParameters = new List<DbParameter>();

            if (sqlQuery.Contains($"{PARAMETER_PREFIX}{CURRENT_USER_PARAMETER_NAME}"))
            {
                parameters.Add(new ExecutionParamDTO
                {
                    Name = CURRENT_USER_PARAMETER_NAME,
                    Value = userID != null && userID.HasValue ? userID.ToString() : null
                });
            }

            DbCommand command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            command.CommandType = CommandType.Text;

            if (parameters != null)
            {
                foreach (ExecutionParamDTO param in parameters.Where(x => sqlQuery.Contains($"{PARAMETER_PREFIX}{x.Name}")))
                {
                    DbParameter paramerter;
                    if (string.IsNullOrEmpty(param.Value))
                    {
                        param.Value = null;
                        paramerter = this.GetParameter(command, param.Name, DBNull.Value);
                    }
                    else
                    {
                        paramerter = this.GetParameter(command, param.Name, this.ConvertFromParamType(param.Value, param.Type));
                    }

                    command.Parameters.Add(paramerter);
                }
            }

            return command;
        }

        private List<ReportParameterDTO> GetReportParameters(int reportId)
        {
            List<ReportParameterDTO> listOfWantedParameters = (from parameter in this.Db.ReportParameters
                                                               join nParameter in this.Db.NReportParameters on parameter.ParameterId equals nParameter.Id
                                                               where parameter.ReportId == reportId && parameter.IsActive
                                                               orderby parameter.OrderNum
                                                               select new ReportParameterDTO
                                                               {
                                                                   Id = parameter.Id,
                                                                   ParameterId = parameter.ParameterId,
                                                                   ParameterName = nParameter.Name,
                                                                   Code = nParameter.Code,
                                                                   DataType = Enum.Parse<ReportParameterTypeEnum>(nParameter.DataType),
                                                                   DefaultValue = parameter.DefaultValue,
                                                                   IsActive = parameter.IsActive,
                                                                   IsMandatory = parameter.IsMandatory,
                                                                   ErrorMessage = parameter.ErrorMessage,
                                                                   OrderNumber = parameter.OrderNum,
                                                                   Pattern = parameter.Pattern
                                                               }).ToList();
            return listOfWantedParameters;
        }

        private List<NomenclatureDTO> GetReportRolePermissions(int targetReportId)
        {
            DateTime now = DateTime.Now;

            return (from rolePermission in this.Db.ReportRolePermissions
                    join role in this.Db.Roles on rolePermission.RoleId equals role.Id
                    where rolePermission.ReportId == targetReportId
                          && rolePermission.IsActive
                          && role.ValidFrom <= now && role.ValidTo >= now
                    orderby role.Name
                    select new NomenclatureDTO
                    {
                        DisplayName = role.Name,
                        Value = role.Id
                    }).ToList();
        }

        private HashSet<int> GetUserRoleIds(int? userId = null)
        {
            HashSet<int> uniqueRoleIds = new HashSet<int>();
            uniqueRoleIds.Add(DefaultConstants.PUBLIC_ROLE_ID);

            if (userId != null && userId.HasValue)
            {
                var now = DateTime.Now;

                var userRoleIds = (from user in Db.Users
                                   join userRole in Db.UserRoles on user.Id equals userRole.UserId
                                   join role in Db.Roles on userRole.RoleId equals role.Id
                                   //join rolePermission in Db.RolePermissions on role.Id equals rolePermission.RoleId
                                   //join permission in Db.Npermissions on rolePermission.PermissionId equals permission.Id
                                   where userRole.UserId == userId && userRole.IsActive && userRole.AccessValidFrom <= now && userRole.AccessValidTo >= now
                                   select role.Id).Distinct().ToList();

                userRoleIds.AddRange(uniqueRoleIds);
                uniqueRoleIds = userRoleIds.ToHashSet();
            }

            return uniqueRoleIds;
        }

        private List<NomenclatureDTO> GetReportUserPermissions(int targetReportId)
        {
            return (from userPermission in this.Db.ReportUserPermissions
                    join user in this.Db.Users on userPermission.UserId equals user.Id
                    join person in this.Db.Persons on user.PersonId equals person.Id
                    where userPermission.ReportId == targetReportId
                          && userPermission.IsActive
                    select new NomenclatureDTO
                    {
                        DisplayName = $"{person.FirstName} {person.LastName} ({user.Username})",
                        Value = user.Id
                    }).ToList();
        }

        private Dictionary<string, string> GetSchemaCodes()
        {
            Dictionary<string, string> schemaCodeDictionary = new Dictionary<string, string>();
            var schemaCodes = from nomenclatureGroup in this.Db.NnomenclatureGroups
                              select new
                              {
                                  Code = nomenclatureGroup.Code,
                                  Name = nomenclatureGroup.Name
                              };

            foreach (var nomenclatureGroup in schemaCodes)
            {
                schemaCodeDictionary.Add(nomenclatureGroup.Code, nomenclatureGroup.Name);
            }

            return schemaCodeDictionary;
        }

        private HashSet<int> GetUserRoleIds(int userId)
        {
            DateTime now = DateTime.Now;
            List<int> userRoleIds = (from userRole in this.Db.UserRoles
                                     where userRole.UserId == userId && userRole.AccessValidFrom <= now && userRole.AccessValidTo > now
                                     select userRole.RoleId).ToList();

            List<int> userLegalRoleIds = (from userLegal in this.Db.UserLegals
                                          where userLegal.UserId == userId && userLegal.AccessValidFrom <= now && userLegal.AccessValidTo > now
                                          select userLegal.RoleId).ToList();

            HashSet<int> roleIds = userRoleIds.Concat(userLegalRoleIds).ToHashSet();

            return roleIds;
        }
    }
}
