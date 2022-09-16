using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.CatchSales;
using IARA.Interfaces.ControlActivity;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class PenalDecreesController : BaseAuditController
    {
        private readonly IPenalDecreesService service;
        private readonly IPenalDecreesNomenclaturesService nomenclaturesService;
        private readonly IAuanRegisterNomenclaturesService auanNomenclatures;
        private readonly ILogBookNomenclaturesService logBookNomenclatures;
        private readonly IDeliveryService deliveryService;
        private readonly IFileService fileService;

        public PenalDecreesController(IPenalDecreesService service,
                                      IPenalDecreesNomenclaturesService nomenclaturesService,
                                      IFileService fileService,
                                      IPermissionsService permissionsService,
                                      IAuanRegisterNomenclaturesService auanNomenclatures,
                                      ILogBookNomenclaturesService logBookNomenclatures,
                                      IDeliveryService deliveryService)
            : base(permissionsService)
        {
            this.service = service;
            this.nomenclaturesService = nomenclaturesService;
            this.auanNomenclatures = auanNomenclatures;
            this.logBookNomenclatures = logBookNomenclatures;
            this.deliveryService = deliveryService;
            this.fileService = fileService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetAllPenalDecrees([FromBody] GridRequestModel<PenalDecreesFilters> request)
        {
            IQueryable<PenalDecreeDTO> result = service.GetAllPenalDecrees(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetPenalDecree([FromQuery] int id)
        {
            PenalDecreeEditDTO decree = service.GetPenalDecree(id);
            return Ok(decree);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PenalDecreesAddRecords)]
        public async Task<IActionResult> AddPenalDecree([FromForm] PenalDecreeEditDTO decree)
        {
            if (decree.DeliveryData != null)
            {
                if (decree.DeliveryData.DeliveryType!.Value == InspDeliveryTypesEnum.DecreeEDelivery)
                {
                    PenalDecreeAuanDataDTO auan = service.GetPenalDecreeAuanData(decree.AuanId.Value);

                    bool hasEDelivery = auan.InspectedEntity.IsPerson!.Value
                        ? await deliveryService.HasPersonAccessToEDeliveryAsync(auan.InspectedEntity.Person.EgnLnc.EgnLnc)
                        : await deliveryService.HasPersonAccessToEDeliveryAsync(auan.InspectedEntity.Legal.EIK);

                    if (hasEDelivery == false)
                    {
                        return ValidationFailedResult(null, ErrorCode.NoEDeliveryRegistration);
                    }
                }
            }

            int id = service.AddPenalDecree(decree);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PenalDecreesEditRecords)]
        public async Task<IActionResult> EditPenalDecree([FromForm] PenalDecreeEditDTO decree)
        {
            if (decree.DeliveryData != null)
            {
                if (decree.DeliveryData.DeliveryType!.Value == InspDeliveryTypesEnum.DecreeEDelivery)
                {
                    PenalDecreeAuanDataDTO auan = service.GetPenalDecreeAuanData(decree.AuanId.Value);

                    bool hasEDelivery = auan.InspectedEntity.IsPerson!.Value
                        ? await deliveryService.HasPersonAccessToEDeliveryAsync(auan.InspectedEntity.Person.EgnLnc.EgnLnc)
                        : await deliveryService.HasPersonAccessToEDeliveryAsync(auan.InspectedEntity.Legal.EIK);

                    if (hasEDelivery == false)
                    {
                        return ValidationFailedResult(null, ErrorCode.NoEDeliveryRegistration);
                    }
                }
            }

            service.EditPenalDecree(decree);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesEditRecords)]
        public async Task<IActionResult> DownloadPenalDecree([FromQuery] int decreeId)
        {
            DownloadableFileDTO file = await service.GetRegisterFileForDownload(decreeId);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.PenalDecreesDeleteRecords)]
        public IActionResult DeletePenalDecree([FromQuery] int id)
        {
            service.DeletePenalDecree(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.PenalDecreesRestoreRecords)]
        public IActionResult UndoDeletePenalDecree([FromQuery] int id)
        {
            service.UndoDeletePenalDecree(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetPenalDecreeAuanData([FromQuery] int auanId)
        {
            PenalDecreeAuanDataDTO data = service.GetPenalDecreeAuanData(auanId);
            return Ok(data);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult DownloadFile([FromQuery] int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetAllAuans()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetAllAuans();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetInspDeliveryTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetDecreeInspDeliveryTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetPenalDecreeStatusTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetPenalDecreeStatusTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetPenalDecreeAuthorityTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetPenalDecreeAuthorityTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetCourts()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetCourts();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetPenalDecreeTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetPenalDecreeTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetPenalDecreeSanctionTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetPenalDecreeSanctionTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetConfiscationInstitutions()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetConfiscationInstitutions();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetAuanDeliveryTypes()
        {
            List<InspDeliveryTypesNomenclatureDTO> result = auanNomenclatures.GetInspDeliveryTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetAuanDeliveryConfirmationTypes()
        {
            List<InspDeliveryTypesNomenclatureDTO> result = auanNomenclatures.GetInspDeliveryConfirmationTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetConfiscationActions()
        {
            List<AuanConfiscationActionsNomenclatureDTO> result = auanNomenclatures.GetConfiscationActions();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetConfiscatedAppliances()
        {
            List<NomenclatureDTO> result = auanNomenclatures.GetConfiscatedAppliances();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetTurbotSizeGroups()
        {
            List<NomenclatureDTO> result = logBookNomenclatures.GetTurbotSizeGroups();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PenalDecreesRead)]
        public IActionResult GetPenalDecreeStatusSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetPenalDecreeStatusSimpleAudit(id);
            return Ok(audit);
        }
    }
}
