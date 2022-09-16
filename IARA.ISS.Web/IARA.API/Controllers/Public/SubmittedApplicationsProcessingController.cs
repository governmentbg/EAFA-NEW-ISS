using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Public
{
    // TODO add security checks
    [AreaRoute(AreaType.Public)]
    public class SubmittedApplicationsProcessingController : BaseController
    {
        private readonly IApplicationsRegisterService service;

        public SubmittedApplicationsProcessingController(IPermissionsService permissionsService,
                                                         IApplicationsRegisterService service)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            IQueryable<ApplicationRegisterDTO> permits = service.GetAllApplications(request.Filters, requesterId: CurrentUser.ID);
            return PageResult(permits, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetApplicationChangeHistoryRecords([FromBody] IEnumerable<int> applicationIds)
        {
            IEnumerable<ApplicationsChangeHistoryDTO> changeHistoryRecords = service.GetApplicationChangeHistoryRecordsForTable(applicationIds);
            return Ok(changeHistoryRecords);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetApplicationStatuses()
        {
            List<NomenclatureDTO> statuses = service.GetApplicationStatuses(new ApplicationHierarchyTypesEnum[] {
                ApplicationHierarchyTypesEnum.Online,
                ApplicationHierarchyTypesEnum.OnPaper
            });
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetApplicationTypes()
        {
            List<NomenclatureDTO> types = service.GetApplicationTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetApplicationSources()
        {
            List<NomenclatureDTO> sources = service.GetApplicationSources(new ApplicationHierarchyTypesEnum[] {
                ApplicationHierarchyTypesEnum.Online,
                ApplicationHierarchyTypesEnum.OnPaper
            });
            return Ok(sources);
        }
    }
}
