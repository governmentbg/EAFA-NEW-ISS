using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.DTOModels.Dashboard;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Helpers;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using TL.Caching.Interfaces;
using TL.Caching.Models;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService service;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IRecreationalFishingService recreationalFishingService;
        private readonly IMemoryCacheService memoryCache;
        private readonly IUserService userService;

        public DashboardController(IPermissionsService permissionsService,
                                   IApplicationsRegisterService applicationsRegisterService,
                                   IDashboardService dashboardService,
                                   IRecreationalFishingService recreationalFishingService,
                                   IMemoryCacheService memoryCache,
                                   IUserService userService)
            : base(permissionsService)
        {
            this.memoryCache = memoryCache;
            this.applicationsRegisterService = applicationsRegisterService;
            this.recreationalFishingService = recreationalFishingService;
            this.service = dashboardService;
            this.userService = userService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ApplicationsRead,
                         Permissions.QualifiedFishersApplicationsRead,
                         Permissions.ScientificFishingApplicationsRead,
                         Permissions.LegalEntitiesApplicationsRead,
                         Permissions.BuyersApplicationsRead,
                         Permissions.ShipsRegisterApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.FishingCapacityApplicationsRead,
                         Permissions.StatisticalFormsAquaFarmApplicationsRead,
                         Permissions.StatisticalFormsFishVesselsApplicationsRead,
                         Permissions.StatisticalFormsReworkApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            IQueryable<ApplicationRegisterDTO> permits = applicationsRegisterService.GetAllApplications(request.Filters, null, null);
            return PageResult(permits, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsRead,
                         Permissions.ScientificFishingApplicationsRead,
                         Permissions.LegalEntitiesApplicationsRead,
                         Permissions.BuyersApplicationsRead,
                         Permissions.ShipsRegisterApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.FishingCapacityApplicationsRead,
                         Permissions.StatisticalFormsAquaFarmApplicationsRead,
                         Permissions.StatisticalFormsFishVesselsApplicationsRead,
                         Permissions.StatisticalFormsReworkApplicationsRead)]
        public IActionResult GetAllApplicationsByUserId([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            List<string> applicationPermissions = new()
            {
                nameof(Permissions.QualifiedFishersApplicationsRead),
                nameof(Permissions.ScientificFishingApplicationsRead),
                nameof(Permissions.LegalEntitiesApplicationsRead),
                nameof(Permissions.BuyersApplicationsRead),
                nameof(Permissions.ShipsRegisterApplicationsRead),
                nameof(Permissions.CommercialFishingPermitApplicationsRead),
                nameof(Permissions.AquacultureFacilitiesApplicationsRead),
                nameof(Permissions.FishingCapacityApplicationsRead),
                nameof(Permissions.StatisticalFormsAquaFarmApplicationsRead),
                nameof(Permissions.StatisticalFormsFishVesselsApplicationsRead),
                nameof(Permissions.StatisticalFormsReworkApplicationsRead)
            };

            List<string> currentUserApplPermissions = applicationPermissions.Intersect(CurrentUser.Permissions).ToList();

            PageCodeEnum[] currentUserPermittedPageCodes = PermittedCodePages.ALL.Where(x => currentUserApplPermissions.Contains(x.Key)).SelectMany(x => x.Value).ToArray();

            if (request.Filters == null)
            {
                request.Filters = new ApplicationsRegisterFilters
                {
                    ShowInactiveRecords = false
                };
            }

            request.Filters.TerritoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

            if (!request.Filters.TerritoryUnitId.HasValue)
            {
                return Ok(Enumerable.Empty<ApplicationRegisterDTO>());
            }
            else
            {
                IQueryable<ApplicationRegisterDTO> permits = applicationsRegisterService.GetAllApplications(request.Filters, null, currentUserPermittedPageCodes);
                return PageResult(permits, request, false);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketApplicationsRead)]
        public IActionResult GetAllTicketApplications([FromBody] GridRequestModel<RecreationalFishingTicketApplicationFilters> request)
        {
            IQueryable<RecreationalFishingTicketApplicationDTO> query = recreationalFishingService.GetAllTickets(request.Filters);

            GridResultModel<RecreationalFishingTicketApplicationDTO> result = new(query, request, false);
            recreationalFishingService.SetTicketsApplicationStatus(result.Records);

            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ApplicationsRead,
                         Permissions.QualifiedFishersApplicationsRead,
                         Permissions.ScientificFishingApplicationsRead,
                         Permissions.LegalEntitiesApplicationsRead,
                         Permissions.BuyersApplicationsRead,
                         Permissions.ShipsRegisterApplicationsRead,
                         Permissions.TicketApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.FishingCapacityApplicationsRead,
                         Permissions.StatisticalFormsAquaFarmApplicationsRead,
                         Permissions.StatisticalFormsFishVesselsApplicationsRead,
                         Permissions.StatisticalFormsReworkApplicationsRead)]
        public async Task<IActionResult> GetStatusCountReportData([FromQuery] bool shouldFilterByCurrentUser, [FromQuery] bool isTickets)
        {
            StatusCountReportDataDTO result = null;

            if (isTickets && CurrentUser.Permissions.Contains(Permissions.TicketApplicationsRead))
            {
                string key = $"{nameof(GetStatusCountReportData)}{isTickets}";

                var settings = new CachingSettings<IDashboardService, StatusCountReportDataDTO>(key, (service) =>
                {
                    return service.GetStatusCountReportData(null, isTickets, null);
                });

                result = await memoryCache.GetCachedObject(settings);
            }
            else if (CurrentUser.Permissions.Contains(Permissions.ApplicationsRead))
            {
                if (shouldFilterByCurrentUser)
                {
                    result = this.service.GetStatusCountReportData(CurrentUser.ID, isTickets, null);
                }
                else
                {
                    string key = $"{nameof(GetStatusCountReportData)}{isTickets}";

                    var settings = new CachingSettings<IDashboardService, StatusCountReportDataDTO>(key, (service) =>
                    {
                        return service.GetStatusCountReportData(null, isTickets, null);
                    });

                    result = await memoryCache.GetCachedObject(settings);
                }
            }
            else
            {
                List<string> applicationPermissions = new()
                {
                    nameof(Permissions.QualifiedFishersApplicationsRead),
                    nameof(Permissions.ScientificFishingApplicationsRead),
                    nameof(Permissions.LegalEntitiesApplicationsRead),
                    nameof(Permissions.BuyersApplicationsRead),
                    nameof(Permissions.ShipsRegisterApplicationsRead),
                    nameof(Permissions.CommercialFishingPermitApplicationsRead),
                    nameof(Permissions.AquacultureFacilitiesApplicationsRead),
                    nameof(Permissions.FishingCapacityApplicationsRead),
                    nameof(Permissions.StatisticalFormsAquaFarmApplicationsRead),
                    nameof(Permissions.StatisticalFormsFishVesselsApplicationsRead),
                    nameof(Permissions.StatisticalFormsReworkApplicationsRead)
                };

                List<string> currentUserApplPermissions = applicationPermissions.Intersect(CurrentUser.Permissions).ToList();

                PageCodeEnum[] currentUserPermittedPageCodes = PermittedCodePages.ALL.Where(x => currentUserApplPermissions.Contains(x.Key)).SelectMany(x => x.Value).ToArray();
                result = this.service.GetStatusCountReportData(shouldFilterByCurrentUser ? CurrentUser.ID : null, isTickets, currentUserPermittedPageCodes);
            }

            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ApplicationsRead,
                         Permissions.QualifiedFishersApplicationsRead,
                         Permissions.ScientificFishingApplicationsRead,
                         Permissions.LegalEntitiesApplicationsRead,
                         Permissions.BuyersApplicationsRead,
                         Permissions.ShipsRegisterApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.FishingCapacityApplicationsRead,
                         Permissions.StatisticalFormsAquaFarmApplicationsRead,
                         Permissions.StatisticalFormsFishVesselsApplicationsRead,
                         Permissions.StatisticalFormsReworkApplicationsRead)]
        public async Task<IActionResult> GetTypesCountReport([FromQuery] bool shouldFilterByCurrentUser)
        {
            List<TypesCountReportDTO> result = null;

            if (shouldFilterByCurrentUser)
            {
                result = this.service.GetTypesCountReport(CurrentUser.ID);
            }
            else
            {
                var settings = new CachingSettings<IDashboardService, List<TypesCountReportDTO>>(nameof(GetTypesCountReport), (service) =>
                {
                    return service.GetTypesCountReport();
                });

                result = await memoryCache.GetCachedObject(settings);
            }

            if (!CurrentUser.Permissions.Contains(Permissions.ApplicationsRead))
            {
                List<string> applicationPermissions = new()
                {
                    nameof(Permissions.QualifiedFishersApplicationsRead),
                    nameof(Permissions.ScientificFishingApplicationsRead),
                    nameof(Permissions.LegalEntitiesApplicationsRead),
                    nameof(Permissions.BuyersApplicationsRead),
                    nameof(Permissions.ShipsRegisterApplicationsRead),
                    nameof(Permissions.CommercialFishingPermitApplicationsRead),
                    nameof(Permissions.AquacultureFacilitiesApplicationsRead),
                    nameof(Permissions.FishingCapacityApplicationsRead),
                    nameof(Permissions.StatisticalFormsAquaFarmApplicationsRead),
                    nameof(Permissions.StatisticalFormsFishVesselsApplicationsRead),
                    nameof(Permissions.StatisticalFormsReworkApplicationsRead)
                };

                List<string> currentUserApplPermissions = applicationPermissions.Intersect(CurrentUser.Permissions).ToList();

                PageCodeEnum[] currentUserPermittedPageCodes = PermittedCodePages.ALL
                                                                                 .Where(x => currentUserApplPermissions.Contains(x.Key))
                                                                                 .SelectMany(x => x.Value)
                                                                                 .ToArray();

                return Ok(result.Where(x => currentUserPermittedPageCodes.Contains(x.PageCode)));
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketApplicationsRead)]
        public async Task<IActionResult> GetTicketTypesCountReport()
        {
            string key = nameof(GetTicketTypesCountReport);

            var settings = new CachingSettings<IDashboardService, List<TicketTypesCountReportDTO>>(key, (service) =>
            {
                return service.GetTicketTypesCountReport();
            });

            var result = await memoryCache.GetCachedObject(settings);

            return Ok(result);
        }
    }
}
