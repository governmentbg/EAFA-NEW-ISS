using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.ControlActivity;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    [RequestFormLimits(ValueCountLimit = 5000)]
    public class InspectionsController : BaseAuditController
    {
        private readonly IInspectionsService service;
        private readonly IFileService fileService;

        public InspectionsController(IInspectionsService service,
                                     IFileService fileService,
                                     IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetShipPersonnel([FromQuery] int shipId)
        {
            List<InspectionShipSubjectNomenclatureDTO> result = this.service.GetShipPersonnel(shipId);

            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetCurrentInspector()
        {
            InspectorDTO result = this.service.GetInspectorByUserId(this.CurrentUser.ID);

            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetInspectors()
        {
            List<NomenclatureDTO> result = this.service.GetInspectors().ToList();

            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetInspector([FromQuery] int id)
        {
            InspectorDTO result = this.service.GetInspector(id);

            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPatrolVehicles([FromQuery] bool isWaterVehicle)
        {
            List<NomenclatureDTO> result = this.service.GetPatrolVehicles(isWaterVehicle).ToList();

            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPatrolVehicle([FromQuery] int id)
        {
            VesselDTO result = this.service.GetPatrolVehicle(id);

            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetShip(int id)
        {
            VesselDTO result = this.service.GetShip(id);

            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetCheckTypesForInspection([FromQuery] InspectionTypesEnum inspectionType)
        {
            List<InspectionCheckTypeNomenclatureDTO> result = this.service.GetCheckTypesForInspectionType(inspectionType);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetShipPermitLicenses([FromQuery] int shipId)
        {
            List<InspectionPermitLicenseDTO> result = this.service.GetShipPermitLicenses(shipId);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPoundNetPermitLicenses([FromQuery] int poundNetId)
        {
            List<InspectionPermitLicenseDTO> result = this.service.GetPoundNetPermitLicenses(poundNetId);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetShipLogBooks([FromQuery] int shipId)
        {
            List<InspectionShipLogBookDTO> result = this.service.GetShipLogBooks(shipId);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetShipFishingGears([FromQuery] int shipId)
        {
            List<FishingGearDTO> result = this.service.GetPermittedFishingGears(shipId, InspectionSubjectEnum.Ship);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPoundNetFishingGears([FromQuery] int poundNetId)
        {
            List<FishingGearDTO> result = this.service.GetPermittedFishingGears(poundNetId, InspectionSubjectEnum.Poundnet);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetBuyers()
        {
            List<InspectedBuyerNomenclatureDTO> result = this.service.GetBuyers();
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetAquacultures()
        {
            List<NomenclatureDTO> result = this.service.GetAquacultures();
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetDeclarationLogBookPages([FromQuery] DeclarationLogBookTypeEnum type, [FromQuery] int shipId)
        {
            List<DeclarationLogBookPageDTO> result = this.service.GetDeclarationLogBookPages(type, shipId);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetAquacultureOwner([FromQuery] int aquacultureId)
        {
            InspectionSubjectPersonnelDTO result = this.service.GetAquacultureOwner(aquacultureId);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = this.fileService.GetFileForDownload(id);
            return this.File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetInspectionAUANs([FromBody] List<int> inspectionIds)
        {
            List<AuanRegisterDTO> result = service.GetInspectionAUANs(inspectionIds);

            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead,
                         Permissions.InspectionsReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterReadAll)]
        public IActionResult GetAll([FromBody] GridRequestModel<InspectionsFilters> request)
        {
            IQueryable<InspectionDTO> result;

            if (this.CurrentUser.Permissions.Contains(Permissions.InspectionsReadAll))
            {
                result = this.service.GetAll(request.Filters, null);
            }
            else
            {
                result = this.service.GetAll(request.Filters, this.CurrentUser.ID);
            }

            return this.PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult Get([FromQuery] int id)
        {
            InspectionEditDTO result = this.service.GetRegisterEntry(id);
            return this.Ok(result);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.InspectionsDeleteRecords)]
        public IActionResult Delete([FromQuery] int id)
        {
            this.service.Delete(id);
            return this.Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.InspectionsRestoreRecords)]
        public IActionResult UndoDelete([FromQuery] int id)
        {
            this.service.Undelete(id);
            return this.Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public async Task<IActionResult> DownloadReport([FromQuery] int inspectionId)
        {
            byte[] file = await this.service.DownloadInspection(inspectionId);

            return this.File(file, "application/pdf");
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult Sign([FromForm] InspectionSignDTO dto, [FromQuery] int inspectionId)
        {
            this.service.SignInspection(inspectionId, dto.Files);
            return this.Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsAddRecords)]
        public IActionResult Add([FromForm] InspectionDraftDTO item)
        {
            int result = this.service.AddRegisterEntry(item, item.InspectionType, this.CurrentUser.ID);
            return this.Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult Edit([FromForm] InspectionDraftDTO item)
        {
            try
            {
                this.service.EditRegisterEntry(item, item.InspectionType);
                return this.Ok(0); // Do not remove the 0, it is required
            }
            catch (ArgumentException ex) when (ex.Message == "Submitted")
            {
                return this.ValidationFailedResult(null, ErrorCode.AlreadySubmitted);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitOFS([FromForm] InspectionObservationAtSeaDTO item)
        {
            try
            {
                int id = this.service.SubmitReport(item, InspectionTypesEnum.OFS, this.CurrentUser.ID);
                return this.Ok(id);
            }
            catch (ArgumentException ex) when (ex.Message == "NotInspector")
            {
                return this.ValidationFailedResult(null, ErrorCode.NotInspector);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIBS([FromForm] InspectionAtSeaDTO item)
        {
            try
            {
                int id = this.service.SubmitReport(item, InspectionTypesEnum.IBS, this.CurrentUser.ID);
                return this.Ok(id);
            }
            catch (ArgumentException ex) when (ex.Message == "NotInspector")
            {
                return this.ValidationFailedResult(null, ErrorCode.NotInspector);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIBP([FromForm] InspectionTransboardingDTO item)
        {
            try
            {
                int id = this.service.SubmitReport(item, InspectionTypesEnum.IBP, this.CurrentUser.ID);
                return this.Ok(id);
            }
            catch (ArgumentException ex) when (ex.Message == "NotInspector")
            {
                return this.ValidationFailedResult(null, ErrorCode.NotInspector);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitITB([FromForm] InspectionTransboardingDTO item)
        {
            try
            {
                int id = this.service.SubmitReport(item, InspectionTypesEnum.ITB, this.CurrentUser.ID);
                return this.Ok(id);
            }
            catch (ArgumentException ex) when (ex.Message == "NotInspector")
            {
                return this.ValidationFailedResult(null, ErrorCode.NotInspector);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIVH([FromForm] InspectionTransportVehicleDTO item)
        {
            try
            {
                int id = this.service.SubmitReport(item, InspectionTypesEnum.IVH, this.CurrentUser.ID);
                return this.Ok(id);
            }
            catch (ArgumentException ex) when (ex.Message == "NotInspector")
            {
                return this.ValidationFailedResult(null, ErrorCode.NotInspector);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIFS([FromForm] InspectionFirstSaleDTO item)
        {
            try
            {
                int id = this.service.SubmitReport(item, InspectionTypesEnum.IFS, this.CurrentUser.ID);
                return this.Ok(id);
            }
            catch (ArgumentException ex) when (ex.Message == "NotInspector")
            {
                return this.ValidationFailedResult(null, ErrorCode.NotInspector);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIAQ([FromForm] InspectionAquacultureDTO item)
        {
            try
            {
                int id = this.service.SubmitReport(item, InspectionTypesEnum.IAQ, this.CurrentUser.ID);
                return this.Ok(id);
            }
            catch (ArgumentException ex) when (ex.Message == "NotInspector")
            {
                return this.ValidationFailedResult(null, ErrorCode.NotInspector);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIFP([FromForm] InspectionFisherDTO item)
        {
            try
            {
                int id = this.service.SubmitReport(item, InspectionTypesEnum.IFP, this.CurrentUser.ID);
                return this.Ok(id);
            }
            catch (ArgumentException ex) when (ex.Message == "NotInspector")
            {
                return this.ValidationFailedResult(null, ErrorCode.NotInspector);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitCWO([FromForm] InspectionCheckWaterObjectDTO item)
        {
            try
            {
                int id = this.service.SubmitReport(item, InspectionTypesEnum.CWO, this.CurrentUser.ID);
                return this.Ok(id);
            }
            catch (ArgumentException ex) when (ex.Message == "NotInspector")
            {
                return this.ValidationFailedResult(null, ErrorCode.NotInspector);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIGM([FromForm] InspectionCheckToolMarkDTO item)
        {
            try
            {
                int id = this.service.SubmitReport(item, InspectionTypesEnum.IGM, this.CurrentUser.ID);
                return this.Ok(id);
            }
            catch (ArgumentException ex) when (ex.Message == "NotInspector")
            {
                return this.ValidationFailedResult(null, ErrorCode.NotInspector);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFluxXmlOFS([FromBody] int id)
        {
            return DownloadFlux((InspectionObservationAtSeaDTO)service.GetRegisterEntry(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFluxXmlIBS([FromBody] int id)
        {
            return DownloadFlux((InspectionAtSeaDTO)service.GetRegisterEntry(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFluxXmlIBP([FromBody] int id)
        {
            return DownloadFlux((InspectionTransboardingDTO)service.GetRegisterEntry(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFluxXmlITB([FromBody] int id)
        {
            return DownloadFlux((InspectionTransboardingDTO)service.GetRegisterEntry(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFluxXmlIVH([FromBody] int id)
        {
            return DownloadFlux((InspectionTransportVehicleDTO)service.GetRegisterEntry(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFluxXmlIFS([FromBody] int id)
        {
            return DownloadFlux((InspectionFirstSaleDTO)service.GetRegisterEntry(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFluxXmlIAQ([FromBody] int id)
        {
            return DownloadFlux((InspectionAquacultureDTO)service.GetRegisterEntry(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFluxXmlIFP([FromBody] int id)
        {
            return DownloadFlux((InspectionFisherDTO)service.GetRegisterEntry(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFluxXmlCWO([FromBody] int id)
        {
            return DownloadFlux((InspectionCheckWaterObjectDTO)service.GetRegisterEntry(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFluxXmlIGM([FromBody] int id)
        {
            return DownloadFlux((InspectionCheckToolMarkDTO)service.GetRegisterEntry(id));
        }

        [HttpGet]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            return this.Ok(this.service.GetSimpleAudit(id));
        }

        private IActionResult DownloadFlux<T>(T item)
            where T : InspectionEditDTO
        {
            XmlSerializer serializer = new(typeof(T));
            using StringWriter sww = new();
            using XmlWriter writer = XmlWriter.Create(sww);

            serializer.Serialize(writer, item);

            string xml = sww.ToString();
            return File(Encoding.UTF8.GetBytes(xml), "application/xml", $"igm_{item.ReportNum}");
        }
    }
}
