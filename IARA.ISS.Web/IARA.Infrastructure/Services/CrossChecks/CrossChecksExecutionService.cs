using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CrossChecks;
using IARA.DomainModels.DTOModels.Reports;
using IARA.EntityModels.Entities;
using IARA.Interfaces.CrossChecks;
using IARA.Interfaces.Reports;
using IARA.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.Services.CrossChecks
{
    public class CrossChecksExecutionService : BaseService, ICrossChecksExecutionService
    {
        private IReportService reportService;
        private IExtendedLogger logger;

        public CrossChecksExecutionService(IARADbContext dbContext, IReportService reportService, IExtendedLogger logger)
            : base(dbContext)
        {
            this.logger = logger;
            this.reportService = reportService;
        }

        public void ExecuteCrossChecks(string execFrequency)
        {
            try
            {
                logger.LogInfo($"Starting cross checks for frequency: {execFrequency}");
                var result = GetCrossChecksForFrequency(execFrequency);

                foreach (var check in result.CrossChecks)
                {
                    try
                    {
                        int totalResultsCount = ExecuteCrossCheck(check, result.UnresolvedStatusId);
                        UpdateCrossCheck(check.CheckId, totalResultsCount);
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(string.Format(ErrorResources.failedCrossCheckExecution, check.CheckName), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        public int ExecuteCrossCheck(int crossCheckId)
        {
            var result = GetCrossCheck(crossCheckId);
            int totalResultsCount = ExecuteCrossCheck(result.CrossCheck, result.UnresolvedStatusId);
            UpdateCrossCheck(result.CrossCheck.CheckId, totalResultsCount);
            return totalResultsCount;
        }

        private (List<ExecuteCrossCheckDTO> CrossChecks, int UnresolvedStatusId) GetCrossChecksForFrequency(string execFrequency)
        {
            List<ExecuteCrossCheckDTO> crossChecks = (from check in Db.CrossChecks
                                                      join report in Db.Reports on check.ReportId equals report.Id
                                                      where check.AutoExecFrequency == execFrequency
                                                         && check.IsActive
                                                         && report.IsActive
                                                      select new ExecuteCrossCheckDTO
                                                      {
                                                          ReportName = report.Name,
                                                          ReportId = report.Id,
                                                          ReportSql = report.ReportSql,
                                                          CheckId = check.Id,
                                                          CheckName = check.CheckTableName
                                                      }).ToList();

            var now = DateTime.Now;

            int unresolvedStatusId = Db.NcheckResolutions
                .Where(x => x.Code == CrossCheckResultResolutionsEnum.Unresolved.ToString()
                         && x.ValidFrom <= now
                         && x.ValidTo > now)
                .Select(x => x.Id)
                .First();

            return (crossChecks, unresolvedStatusId);
        }

        private (ExecuteCrossCheckDTO CrossCheck, int UnresolvedStatusId) GetCrossCheck(int crossCheckId)
        {
            ExecuteCrossCheckDTO crossCheck = (from check in Db.CrossChecks
                                               join report in Db.Reports on check.ReportId equals report.Id
                                               where check.Id == crossCheckId
                                                  && check.IsActive
                                                  && report.IsActive
                                               select new ExecuteCrossCheckDTO
                                               {
                                                   ReportName = report.Name,
                                                   ReportId = report.Id,
                                                   ReportSql = report.ReportSql,
                                                   CheckId = check.Id,
                                                   CheckName = check.CheckTableName
                                               }).First();

            var now = DateTime.Now;

            int unresolvedStatusId = Db.NcheckResolutions
                .Where(x => x.Code == CrossCheckResultResolutionsEnum.Unresolved.ToString()
                         && x.ValidFrom <= now
                         && x.ValidTo > now)
                .Select(x => x.Id)
                .First();

            return (crossCheck, unresolvedStatusId);
        }

        private void UpdateCrossCheck(int crossCheckId, int totalRecordsCount)
        {
            CrossCheck crossCheck = Db.CrossChecks.Where(x => x.Id == crossCheckId).First();
            crossCheck.LastExecRowCount = totalRecordsCount;
            crossCheck.LastExecDateTime = DateTime.Now;
            Db.SaveChanges();
        }

        private int ExecuteCrossCheck(ExecuteCrossCheckDTO check, int unresolvedStatusId)
        {
            var now = DateTime.Now;
            var rawResults = reportService.ExecuteRawSql(check.ReportSql, GetReportParameters(check.ReportId), null).ToList();

            List<BaseCrossCheckResultDTO> results;

            try
            {
                results = rawResults.Select(row => new BaseCrossCheckResultDTO
                {
                    TableId = int.Parse(row["TableID"]?.ToString()),
                    PageCode = row["PageCode"]?.ToString(),
                    ErrorDescription = row["ErrorDescription"]?.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ErrorResources.invalidCrossCheckResult, ex);
            }

            var tableIds = results.Select(x => x.TableId).ToHashSet();
            var pageCodes = results.Select(x => x.PageCode).ToHashSet();

            var existingChecks = Db.CrossCheckResults.Where(x => tableIds.Contains(x.TableId)
                                                                && pageCodes.Contains(x.PageCode)
                                                                && x.CheckId == check.CheckId
                                                                && x.ValidFrom <= now && x.ValidTo > now)
                                                              .Select(x => new
                                                              {
                                                                  x.PageCode,
                                                                  x.TableId
                                                              }).ToList();

            foreach (var row in results)
            {
                bool exists = existingChecks.Where(x => x.TableId == row.TableId && x.PageCode == row.PageCode).Any();

                if (!exists)
                {
                    Db.CrossCheckResults.Add(new CrossCheckResult
                    {
                        CheckId = check.CheckId,
                        PageCode = row.PageCode,
                        TableId = row.TableId,
                        ErrorDescription = row.ErrorDescription,
                        ResolutionId = unresolvedStatusId,
                        ValidFrom = now,
                        ValidTo = DefaultConstants.MAX_VALID_DATE
                    });
                }
            }

            Db.SaveChanges();

            return results.Count;
        }


        private List<ExecutionParamDTO> GetReportParameters(int reportId)
        {
            var reportParameters = from reportParam in Db.ReportParameters
                                   join param in Db.NReportParameters on reportParam.ParameterId equals param.Id
                                   where reportParam.ReportId == reportId
                                   select new ExecutionParamDTO
                                   {
                                       DefaultValue = reportParam.DefaultValue,
                                       IsMandatory = reportParam.IsMandatory,
                                       Name = param.Name,
                                       Pattern = param.Pattern,
                                       Type = Enum.Parse<ReportParameterTypeEnum>(param.DataType),
                                       Value = param.DefaultValue
                                   };

            return reportParameters.ToList();
        }


    }
}
