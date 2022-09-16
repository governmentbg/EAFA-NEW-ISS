using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Files;
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

namespace IARA.WebAPI.Controllers.Mobile.Administrative
{
    [AreaRoute(AreaType.MobileAdministrative)]
    public class InspectionsController : BaseController
    {
        private readonly IInspectionsService service;
        private readonly IFileService fileService;

        public InspectionsController(IInspectionsService inspections, IPermissionsService permissionsService, IFileService fileService)
            : base(permissionsService)
        {
            this.service = inspections;
            this.fileService = fileService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead, Permissions.InspectionsReadAll)]
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
            this.service.SafeDelete(id);
            return this.Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = this.fileService.GetFileForDownload(id);
            return this.File(file.Bytes, file.MimeType, file.FileName);
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
        public IActionResult Sign([FromForm] List<FileInfoDTO> files, [FromQuery] int inspectionId)
        {
            this.service.SignInspection(inspectionId, files);
            return this.Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsAddRecords)]
        public IActionResult Add([FromForm] InspectionDraftDTO item)
        {
            int result = this.service.AddRegisterEntry(item, item.InspectionType, this.CurrentUser.ID);
            return this.Ok(result);
        }

        [HttpPut]
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
            int id = this.service.SubmitReport(item, InspectionTypesEnum.OFS, this.CurrentUser.ID);
            return this.Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIBS([FromForm] InspectionAtSeaDTO item)
        {
            int id = this.service.SubmitReport(item, InspectionTypesEnum.IBS, this.CurrentUser.ID);
            return this.Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIBP([FromForm] InspectionTransboardingDTO item)
        {
            int id = this.service.SubmitReport(item, InspectionTypesEnum.IBP, this.CurrentUser.ID);
            return this.Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitITB([FromForm] InspectionTransboardingDTO item)
        {
            int id = this.service.SubmitReport(item, InspectionTypesEnum.ITB, this.CurrentUser.ID);
            return this.Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIVH([FromForm] InspectionTransportVehicleDTO item)
        {
            int id = this.service.SubmitReport(item, InspectionTypesEnum.IVH, this.CurrentUser.ID);
            return this.Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIFS([FromForm] InspectionFirstSaleDTO item)
        {
            int id = this.service.SubmitReport(item, InspectionTypesEnum.IFS, this.CurrentUser.ID);
            return this.Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIAQ([FromForm] InspectionAquacultureDTO item)
        {
            int id = this.service.SubmitReport(item, InspectionTypesEnum.IAQ, this.CurrentUser.ID);
            return this.Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIFP([FromForm] InspectionFisherDTO item)
        {
            int id = this.service.SubmitReport(item, InspectionTypesEnum.IFP, this.CurrentUser.ID);
            return this.Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitCWO([FromForm] InspectionCheckWaterObjectDTO item)
        {
            int id = this.service.SubmitReport(item, InspectionTypesEnum.CWO, this.CurrentUser.ID);
            return this.Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsEditRecords)]
        public IActionResult SubmitIGM([FromForm] InspectionCheckToolMarkDTO item)
        {
            int id = this.service.SubmitReport(item, InspectionTypesEnum.IGM, this.CurrentUser.ID);
            return this.Ok(id);
        }
    }
}
