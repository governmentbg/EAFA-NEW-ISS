using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Dashboard;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.Dashboard
{
    public class DashboardService : Service, IDashboardService
    {
        private readonly IUserService userService;
        public DashboardService(IARADbContext db, IUserService userService)
            : base(db)
        {
            this.userService = userService;
        }

        public StatusCountReportDataDTO GetStatusCountReportData(int? currentUserId, bool isTickets, PageCodeEnum[] pageCodes)
        {
            List<int> userIdsInTerritoryUnit = currentUserId.HasValue ? userService.GetUserIdsInSameTerritoryUnitAsUser(currentUserId.Value) : null;
            StatusCountReportDataDTO result = new StatusCountReportDataDTO();

            DateTime now = DateTime.Now;

            List<DateTime> last14Days = Enumerable.Range(0, 14)
                                    .Select(day => now.Date.AddDays(-day)).OrderBy(x => x)
                                    .ToList();

            result.Categories = last14Days.Select(x => x.Date.ToShortDateString()).ToList();

            var statusInformation = from history in Db.ApplicationChangeHistories
                                    join status in Db.NapplicationStatuses on history.ApplicationStatusId equals status.Id
                                    join application in Db.Applications on history.ApplicationId equals application.Id
                                    join hiearchyType in Db.NapplicationStatusHierarchyTypes on application.ApplicationStatusHierTypeId equals hiearchyType.Id
                                    where last14Days.Contains(history.ValidFrom.Date)
                                          && ((isTickets == true && hiearchyType.Code == nameof(ApplicationHierarchyTypesEnum.RecreationalFishingTicket))
                                          || (isTickets == false && hiearchyType.Code != nameof(ApplicationHierarchyTypesEnum.RecreationalFishingTicket)))
                                          && application.IsActive
                                    orderby status.OrderNum
                                    select new
                                    {
                                        ApplicationId = history.ApplicationId,
                                        ApplicationTypeId = application.ApplicationTypeId,
                                        AssignedUserId = application.AssignedUserId,
                                        Id = history.ApplicationStatusId,
                                        Name = status.Name,
                                        ValidFrom = history.ValidFrom,
                                        Color = status.Color
                                    };

            if (userIdsInTerritoryUnit != null)
            {
                statusInformation = from statusInfo in statusInformation
                                    where statusInfo.AssignedUserId.HasValue
                                     && userIdsInTerritoryUnit.Contains(statusInfo.AssignedUserId.Value)
                                    select statusInfo;
            }

            if (pageCodes != null && pageCodes.Length > 0)
            {
                string[] codes = pageCodes.Select(x => x.ToString()).ToArray();

                statusInformation = from statusInfo in statusInformation
                                    join applType in Db.NapplicationTypes on statusInfo.ApplicationTypeId equals applType.Id
                                    where codes.Contains(applType.PageCode)
                                    select statusInfo;
            }

            var statusInformationQuery = (from statusInfo in statusInformation
                                          select new
                                          {
                                              ApplicationId = statusInfo.ApplicationId,
                                              Id = statusInfo.Id,
                                              Name = statusInfo.Name,
                                              ValidFrom = statusInfo.ValidFrom,
                                              Color = statusInfo.Color
                                          }
                                         ).ToList();

            var statusesGroup = (from statusInfo in statusInformationQuery
                                 group statusInfo by new { statusInfo.Id, statusInfo.Name, statusInfo.Color } into groupedStatusHistory
                                 select new
                                 {
                                     StatusId = groupedStatusHistory.Key.Id,
                                     StatusName = groupedStatusHistory.Key.Name,
                                     StatusColor = groupedStatusHistory.Key.Color,
                                     Collection = groupedStatusHistory.ToList()
                                 }).ToList();

            result.Series = new List<StatusCountReportDTO>();

            foreach (var statusGroup in statusesGroup)
            {
                var query = (from sGroup in statusGroup.Collection
                             group sGroup by sGroup.ValidFrom.Date into groupedStatusHistory
                             orderby groupedStatusHistory.Key
                             select new
                             {
                                 ValidFrom = groupedStatusHistory.Key.Date,
                                 ApplicationsCount = groupedStatusHistory.Distinct().Count()
                             }).ToList();

                List<DateTime> datesNotInQuery = last14Days.Except(query.Select(x => x.ValidFrom)).ToList();

                foreach (var date in datesNotInQuery)
                {
                    query.Add(new
                    {
                        ValidFrom = date.Date,
                        ApplicationsCount = 0
                    });
                }

                result.Series.Add(new StatusCountReportDTO
                {
                    Name = statusGroup.StatusName,
                    Data = query.OrderBy(x => x.ValidFrom).Select(x => x.ApplicationsCount).ToList(),
                    Color = statusGroup.StatusColor
                });
            }

            return result;
        }

        public List<TypesCountReportDTO> GetTypesCountReport(int? userId = null)
        {
            List<int> userIdsInTerritoryUnit = userId.HasValue ? userService.GetUserIdsInSameTerritoryUnitAsUser(userId.Value) : null;
            DateTime now = DateTime.Now;

            var query = from application in Db.Applications
                        join applHierrType in Db.NapplicationStatusHierarchyTypes on application.ApplicationStatusHierTypeId equals applHierrType.Id
                        join type in Db.NapplicationTypes on application.ApplicationTypeId equals type.Id
                        join status in Db.NapplicationStatuses on application.ApplicationStatusId equals status.Id
                        where applHierrType.Code != nameof(ApplicationHierarchyTypesEnum.RecreationalFishingTicket)
                              && status.Code != nameof(ApplicationStatusesEnum.ADM_ACT_COMPLETION)
                              && status.Code != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                              && application.IsActive
                        select new
                        {
                            Id = application.Id,
                            TypeId = application.ApplicationTypeId,
                            Name = type.Name,
                            AssignedUserId = application.AssignedUserId,
                            PageCode = type.PageCode
                        };

            if (userIdsInTerritoryUnit != null)
            {
                query = from application in query
                        where application.AssignedUserId.HasValue
                            && userIdsInTerritoryUnit.Contains(application.AssignedUserId.Value)
                        select application;
            }

            List<TypesCountReportDTO> queryGroupedByType = (from typeInfo in query
                                                            group typeInfo by new { typeInfo.TypeId, typeInfo.Name, typeInfo.PageCode } into groupedTypes
                                                            select new TypesCountReportDTO
                                                            {
                                                                Name = groupedTypes.Key.Name,
                                                                PageCode = Enum.Parse<PageCodeEnum>(groupedTypes.Key.PageCode),
                                                                Count = groupedTypes.Count()
                                                            }).ToList();
            return queryGroupedByType;
        }

        public List<TicketTypesCountReportDTO> GetTicketTypesCountReport()
        {
            DateTime now = DateTime.Now;

            var query = (from application in Db.Applications
                         join applHierrType in Db.NapplicationStatusHierarchyTypes on application.ApplicationStatusHierTypeId equals applHierrType.Id
                         join ticket in Db.FishingTickets on application.Id equals ticket.ApplicationId
                         join ticketType in Db.NticketTypes on ticket.TicketTypeId equals ticketType.Id
                         where applHierrType.Code == nameof(ApplicationHierarchyTypesEnum.RecreationalFishingTicket)
                               && application.IsActive
                               && ticket.IsActive
                               && application.ApplicationStatus.Code != nameof(ApplicationStatusesEnum.CONFIRMED_ISSUED_TICKET)
                               && application.ApplicationStatus.Code != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                         select new
                         {
                             Id = application.Id,
                             TypeId = ticket.TicketTypeId,
                             TypeName = ticketType.Name,
                             TypeCode = ticketType.Code
                         }).ToList();

            List<TicketTypesCountReportDTO> queryGroupedByType = (from typeInfo in query
                                                                  group typeInfo by new { typeInfo.TypeId, typeInfo.TypeName, typeInfo.TypeCode } into groupedTypes
                                                                  select new TicketTypesCountReportDTO
                                                                  {
                                                                      Name = groupedTypes.Key.TypeName,
                                                                      TicketTypeCode = Enum.Parse<TicketTypeEnum>(groupedTypes.Key.TypeCode),
                                                                      Count = groupedTypes.Count()
                                                                  }).ToList();
            return queryGroupedByType;
        }
        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
