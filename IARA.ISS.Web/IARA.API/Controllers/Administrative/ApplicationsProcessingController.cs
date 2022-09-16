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

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class ApplicationsProcessingController : BaseAuditController
    {
        private readonly IApplicationsRegisterService service;

        public ApplicationsProcessingController(IApplicationsRegisterService service, IPermissionsService permissions)
            : base(permissions)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            IQueryable<ApplicationRegisterDTO> permits = service.GetAllApplications(request.Filters, null, null);
            return PageResult(permits, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ApplicationsRead,
                         Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll, Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll, Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll, Permissions.QualifiedFishersApplicationsRead,
                         Permissions.LegalEntitiesApplicationsReadAll, Permissions.LegalEntitiesApplicationsRead,
                         Permissions.ScientificFishingApplicationsReadAll, Permissions.ScientificFishingApplicationsRead,
                         Permissions.ShipsRegisterApplicationReadAll, Permissions.ShipsRegisterApplicationsRead,
                         Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead,
                         Permissions.StatisticalFormsAquaFarmApplicationsReadAll, Permissions.StatisticalFormsAquaFarmApplicationsRead,
                         Permissions.StatisticalFormsReworkApplicationsReadAll, Permissions.StatisticalFormsReworkApplicationsRead,
                         Permissions.StatisticalFormsFishVesselsApplicationsReadAll, Permissions.StatisticalFormsFishVesselsApplicationsRead)]
        public IActionResult GetApplicationChangeHistoryRecords([FromBody] IEnumerable<int> applicationIds)
        {
            IEnumerable<ApplicationsChangeHistoryDTO> changeHistoryRecords = service.GetApplicationChangeHistoryRecordsForTable(applicationIds);
            return Ok(changeHistoryRecords);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.ApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            service.DeleteApplication(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.ApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            service.UndoDeleteApplication(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize()]
        public IActionResult GetApplicationStatuses()
        {
            List<NomenclatureDTO> statuses = service.GetApplicationStatuses(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize()]
        public IActionResult GetApplicationTypes()
        {
            List<NomenclatureDTO> types = service.GetApplicationTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize()]
        public IActionResult GetApplicationSources()
        {
            List<NomenclatureDTO> sources = service.GetApplicationSources(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(sources);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ApplicationsRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ApplicationsRead,
                         Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll, Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll, Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll, Permissions.QualifiedFishersApplicationsRead,
                         Permissions.LegalEntitiesApplicationsReadAll, Permissions.LegalEntitiesApplicationsRead,
                         Permissions.ScientificFishingApplicationsReadAll, Permissions.ScientificFishingApplicationsRead,
                         Permissions.ShipsRegisterApplicationReadAll, Permissions.ShipsRegisterApplicationsRead,
                         Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead,
                         Permissions.StatisticalFormsAquaFarmApplicationsReadAll, Permissions.StatisticalFormsAquaFarmApplicationsRead,
                         Permissions.StatisticalFormsReworkApplicationsReadAll, Permissions.StatisticalFormsReworkApplicationsRead,
                         Permissions.StatisticalFormsFishVesselsApplicationsReadAll, Permissions.StatisticalFormsFishVesselsApplicationsRead)]
        public IActionResult GetApplicationHistorySimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetApplicationHistorySimpleAudit(id);
            return Ok(audit);
        }
    }
}
