using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class PersonReportsController : BaseAuditController
    {
        private readonly IPersonReportsService service;

        public PersonReportsController(IPersonReportsService service,
                                IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PersonsReportRead)]
        public IActionResult GetAllPersonsReport([FromBody] GridRequestModel<PersonsReportFilters> request)
        {
            IQueryable<PersonReportDTO> result = service.GetAllPersonsReport(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PersonsReportRead)]
        public IActionResult GetPeopleHistory([FromBody] IEnumerable<int> validPeopleIds)
        {
            IEnumerable<ReportHistoryDTO> peopleHistory = service.GetPeopleHistory(validPeopleIds);
            return Ok(peopleHistory);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.LegalEntitiesReportRead)]
        public IActionResult GetAllLegalEntitiesReport([FromBody] GridRequestModel<LegalEntitiesReportFilters> request)
        {
            IQueryable<LegalEntityReportDTO> result = service.GetAllLegalEntitiesReport(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.LegalEntitiesReportRead)]
        public IActionResult GetLegalEntitiesHistory([FromBody] IEnumerable<int> ids)
        {
            IEnumerable<ReportHistoryDTO> legalEntitiesHistory = service.GetLegalEntitiesHistory(ids);
            return Ok(legalEntitiesHistory);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PersonsReportRead)]
        public IActionResult GetPersonReport([FromQuery] int id)
        {
            return Ok(service.GetPersonReport(id));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesReportRead)]
        public IActionResult GetLegalEntityReport([FromQuery] int id)
        {
            return Ok(service.GetLegalEntityReport(id));
        }

        // TODO what is this for?
        [HttpGet]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }
    }
}
