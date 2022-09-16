using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
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
    public class AuanRegisterController : BaseAuditController
    {
        private readonly IAuanRegisterService service;
        private readonly IAuanRegisterNomenclaturesService nomenclaturesService;
        private readonly ILogBookNomenclaturesService logBookNomenclatures;
        private readonly IDeliveryService deliveryService;
        private readonly IFileService fileService;

        public AuanRegisterController(IAuanRegisterService service,
                                      IFileService fileService,
                                      IDeliveryService deliveryService,
                                      IPermissionsService permissionsService,
                                      IAuanRegisterNomenclaturesService nomenclaturesService,
                                      ILogBookNomenclaturesService logBookNomenclatures)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
            this.deliveryService = deliveryService;
            this.nomenclaturesService = nomenclaturesService;
            this.logBookNomenclatures = logBookNomenclatures;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult GetAllAuans([FromBody] GridRequestModel<AuanRegisterFilters> request)
        {
            IQueryable<AuanRegisterDTO> result = service.GetAllAuans(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult GetAuan([FromQuery] int id)
        {
            AuanRegisterEditDTO auan = service.GetAuan(id);
            return Ok(auan);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AuanRegisterAddRecords)]
        public async Task<IActionResult> AddAuan([FromForm] AuanRegisterEditDTO auan)
        {
            if (auan.DeliveryData.DeliveryType!.Value == InspDeliveryTypesEnum.EDelivery 
                || (auan.DeliveryData.IsEDeliveryRequested.HasValue && auan.DeliveryData.IsEDeliveryRequested.Value == true))
            {
                bool hasEDelivery = auan.InspectedEntity.IsPerson!.Value
                    ? await deliveryService.HasPersonAccessToEDeliveryAsync(auan.InspectedEntity.Person.EgnLnc.EgnLnc)
                    : await deliveryService.HasPersonAccessToEDeliveryAsync(auan.InspectedEntity.Legal.EIK);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(null, ErrorCode.NoEDeliveryRegistration);
                }
            }

            int id = service.AddAuan(auan);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AuanRegisterEditRecords)]
        public async Task<IActionResult> EditAuan([FromForm] AuanRegisterEditDTO auan)
        {
            if (auan.DeliveryData.DeliveryType!.Value == InspDeliveryTypesEnum.EDelivery
                || (auan.DeliveryData.IsEDeliveryRequested.HasValue && auan.DeliveryData.IsEDeliveryRequested.Value == true))
            {
                bool hasEDelivery = auan.InspectedEntity.IsPerson!.Value
                    ? await deliveryService.HasPersonAccessToEDeliveryAsync(auan.InspectedEntity.Person.EgnLnc.EgnLnc)
                    : await deliveryService.HasPersonAccessToEDeliveryAsync(auan.InspectedEntity.Legal.EIK);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(null, ErrorCode.NoEDeliveryRegistration);
                }
            }

            service.EditAuan(auan);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public async Task<IActionResult> DownloadAuan([FromQuery] int auanId)
        {
            byte[] file = await service.DownloadAuan(auanId);
            return File(file, "application/pdf");
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.AuanRegisterDeleteRecords)]
        public IActionResult DeleteAuan([FromQuery] int id)
        {
            service.DeleteAuan(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.AuanRegisterRestoreRecords)]
        public IActionResult UndoDeleteAuan([FromQuery] int id)
        {
            service.UndoDeleteAuan(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult GetAuanReportData([FromQuery] int inspectionId)
        {
            AuanReportDataDTO data = service.GetAuanReportDataFromInspection(inspectionId);
            return Ok(data);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult DownloadFile([FromQuery] int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult GetAllDrafters()
        {
            List<NomenclatureDTO> drafters = service.GetAllDrafters();
            return Ok(drafters);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult GetAllInspectionReports()
        {
            List<NomenclatureDTO> reports = nomenclaturesService.GetAllInspectionReports();
            return Ok(reports);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult GetConfiscationActions()
        {
            List<AuanConfiscationActionsNomenclatureDTO> result = nomenclaturesService.GetConfiscationActions();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult GetAuanDeliveryTypes()
        {
            List<InspDeliveryTypesNomenclatureDTO> result = nomenclaturesService.GetInspDeliveryTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult GetAuanDeliveryConfirmationTypes()
        {
            List<InspDeliveryTypesNomenclatureDTO> result = nomenclaturesService.GetInspDeliveryConfirmationTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult GetAuanStatuses()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetAuanStatuses();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult GetConfiscatedAppliances()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetConfiscatedAppliances();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public IActionResult GetTurbotSizeGroups()
        {
            List<NomenclatureDTO> result = logBookNomenclatures.GetTurbotSizeGroups();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AuanRegisterRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }
    }
}
