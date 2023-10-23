using System.Linq;
using System.Collections.Generic;
using IARA.DomainModels.DTOModels.FishingActivityReports;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.CatchSales;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class FishingActivityReportsController : BaseAuditController
    {
        private readonly IFishingActivityReportsService service;

        public FishingActivityReportsController(IPermissionsService permissionsService,
                                                IFishingActivityReportsService service)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingActivityReportsRead)]
        public IActionResult GetAllFishingActivityReports([FromBody] GridRequestModel<FishingActivityReportsFilters> request)
        {
            GridResultModel<FishingActivityReportDTO> result = new(service.GetAllFishingActivityReports(request.Filters), request, false);

            if (result.Records.Any())
            {
                List<string> tripIdentifiers = result.Records.Select(x => x.TripIdentifier).ToList();
                ILookup<string, FishingActivityReportItemDTO> items = service.GetAllFishingActivityReportItems(tripIdentifiers);

                List<int> reportIds = items.SelectMany(x => x.Select(x => x.Id)).ToList();
                List<FishingActivityReportPageDTO> pages = service.GetAllFishingActivityReportPages(reportIds);

                foreach (FishingActivityReportDTO report in result.Records)
                {
                    report.Items = items[report.TripIdentifier].ToList();
                    report.Pages = pages.Where(x => report.Items.Select(x => x.Id).Contains(x.ReportId)).ToList();
                }
            }

            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingActivityReportsRead)]
        public IActionResult GetFishingActivityReportJson([FromQuery] int id)
        {
            string json = service.GetFishingActivityReportJson(id);
            return Ok(json);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingActivityReportsRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }
    }
}
