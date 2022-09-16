using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.RecreationalFishing;
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
    public class RecreationalFishingAssociationsController : BaseAuditController
    {
        private readonly IRecreationalFishingAssociationService service;
        private readonly IFileService fileService;
        private readonly IUserService userService;

        public RecreationalFishingAssociationsController(IRecreationalFishingAssociationService service,
                                                         IFileService fileService,
                                                         IPermissionsService permissionsService,
                                                         IUserService userService)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
            this.userService = userService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AssociationsReadAll, Permissions.AssociationsRead)]
        public IActionResult GetAllAssociations([FromBody] GridRequestModel<RecreationalFishingAssociationsFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.AssociationsReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new RecreationalFishingAssociationsFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.TerritoryUnitId = territoryUnitId.Value;
                }
                else
                {
                    return Ok(Enumerable.Empty<RecreationalFishingTicketApplicationDTO>());
                }
            }

            IQueryable<RecreationalFishingAssociationDTO> associations = service.GetAllAssociations(request.Filters);
            return PageResult(associations, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AssociationsReadAll, Permissions.AssociationsRead)]
        public IActionResult GetAssociation([FromQuery] int id)
        {
            RecreationalFishingAssociationEditDTO association = service.GetAssociation(id);
            return Ok(association);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AssociationsReadAll, Permissions.AssociationsRead)]
        public IActionResult GetPossibleAssociationLegals()
        {
            List<RecreationalFishingPossibleAssociationLegalDTO> result = service.GetPossibleAssociationLegals();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AssociationsReadAll, Permissions.AssociationsRead)]
        public IActionResult GetLegalForAssociation([FromQuery] int id)
        {
            RecreationalFishingAssociationEditDTO association = service.GetLegalForAssociation(id);
            return Ok(association);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AssociationsAddRecords)]
        public IActionResult AddAssociation([FromForm] RecreationalFishingAssociationEditDTO association)
        {
            int id = service.AddAssociation(association);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AssociationsEditRecords)]
        public IActionResult EditAssociation([FromForm] RecreationalFishingAssociationEditDTO association)
        {
            service.EditAssociation(association);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.AssociationsDeleteRecords)]
        public IActionResult DeleteAssociation([FromQuery] int id)
        {
            service.DeleteAssociation(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.AssociationsRestoreRecords)]
        public IActionResult UndoDeleteAssociation([FromQuery] int id)
        {
            service.UndoDeleteAssociation(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AssociationsReadAll, Permissions.AssociationsRead)]
        public IActionResult DownloadAssociationFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AssociationsReadAll, Permissions.AssociationsRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AssociationsReadAll, Permissions.AssociationsRead)]
        public IActionResult GetAllAssociationNomenclatures()
        {
            List<NomenclatureDTO> associations = service.GetAllAssociationNomenclatures();
            return Ok(associations);
        }
    }
}
