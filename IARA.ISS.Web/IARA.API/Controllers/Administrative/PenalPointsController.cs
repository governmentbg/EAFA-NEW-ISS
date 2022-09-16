using System;
using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.ControlActivity.PenalPoints;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.ControlActivity;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class PenalPointsController : BaseAuditController
    {
        private readonly IPenalPointsService service;
        private readonly IPenalPointsNomenclaturesService nomenclaturesService;
        private readonly ICommercialFishingNomenclaturesService commercialFishingNomenclaturesService;
        private readonly IFileService fileService;

        public PenalPointsController(IPenalPointsService service,
                                       IFileService fileService,
                                       IPermissionsService permissionsService,
                                       IPenalPointsNomenclaturesService nomenclaturesService,
                                       ICommercialFishingNomenclaturesService commercialFishingNomenclaturesService)
            : base(permissionsService)
        {
            this.service = service;
            this.nomenclaturesService = nomenclaturesService;
            this.commercialFishingNomenclaturesService = commercialFishingNomenclaturesService;
            this.fileService = fileService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AwardedPointsRead,
                         Permissions.AwardedPointsReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterReadAll)]
        public IActionResult GetAllPenalPoints([FromBody] GridRequestModel<PenalPointsFilters> request)
        {
            IQueryable<PenalPointsDTO> result = service.GetAllPenalPoints(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AwardedPointsRead)]
        public IActionResult GetPenalPoints([FromQuery] int id)
        {
            PenalPointsEditDTO decree = service.GetPenalPoints(id);
            return Ok(decree);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AwardedPointsAddRecords)]
        public IActionResult AddPenalPoints([FromForm] PenalPointsEditDTO decree)
        {
            int id = service.AddPenalPoints(decree);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AwardedPointsEditRecords)]
        public IActionResult EditPenalPoints([FromForm] PenalPointsEditDTO decree)
        {
            service.EditPenalPoints(decree);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.AwardedPointsDeleteRecords)]
        public IActionResult DeletePenalPoints([FromQuery] int id)
        {
            service.DeletePenalPoints(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.AwardedPointsRestoreRecords)]
        public IActionResult UndoDeletePenalPoints([FromQuery] int id)
        {
            service.UndoDeletePenalPoints(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AwardedPointsRead)]
        public IActionResult GetPenalPointsAuanDecreeData([FromQuery] int decreeId)
        {
            PenalPointsAuanDecreeDataDTO data = service.GetPenalPointsAuanDecreeData(decreeId);
            return Ok(data);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AwardedPointsRead)]
        public IActionResult DownloadFile([FromQuery] int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AwardedPointsRead)]
        public IActionResult GetAllPenalDecrees()
        {
            List<NomenclatureDTO> decrees = nomenclaturesService.GetAllPenalDecrees();
            return Ok(decrees);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AwardedPointsRead)]
        public IActionResult GetAllPenalPointsStatuses()
        {
            List<NomenclatureDTO> statuses = nomenclaturesService.GetAllPenalPoinsStatuses();
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AwardedPointsRead)]
        public IActionResult GetShipPermits([FromQuery] int shipId, [FromQuery] bool onlyPoundNet)
        {
            List<PermitNomenclatureDTO> permits = commercialFishingNomenclaturesService.GetShipPermits(shipId, onlyPoundNet);
            return Ok(permits);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AwardedPointsRead)]
        public IActionResult GetPermitOrders([FromQuery] int ownerId, [FromQuery] bool isFisher, [FromQuery] bool isPermitOwnerPerson)
        {
            List<PenalPointsOrderDTO> orders = service.GetPermitOrders(ownerId, isFisher, isPermitOwnerPerson);
            return Ok(orders);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AwardedPointsRead)]
        public IActionResult GetShipPermitLicenses([FromQuery] int shipId)
        {
            List<PermitNomenclatureDTO> permits = commercialFishingNomenclaturesService.GetShipPermitLicenses(shipId);
            return Ok(permits);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AwardedPointsRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AwardedPointsRead)]
        public IActionResult GetPenalPointsStatusSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetPenalPointsStatusSimpleAudit(id);
            return Ok(audit);
        }
    }
}
