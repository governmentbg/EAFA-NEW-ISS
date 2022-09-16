using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.Inspectors;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
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
    public class InspectorsController : BaseAuditController
    {
        private readonly IInspectorsService service;
        public InspectorsController(IInspectorsService service,
                                    IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectorsRead)]
        public IActionResult GetAllRegistered([FromBody] GridRequestModel<InspectorsFilters> request)
        {
            IQueryable<InspectorsRegisterDTO> inspectors = service.GetAll(request.Filters, true);
            return PageResult(inspectors, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectorsRead)]
        public IActionResult GetAllUnregistered([FromBody] GridRequestModel<InspectorsFilters> request)
        {
            IQueryable<InspectorsRegisterDTO> inspectors = service.GetAll(request.Filters, false);
            return PageResult(inspectors, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectorsAddRecords)]
        public IActionResult AddInspector([FromBody] InspectorsRegisterEditDTO inspector)
        {
            try
            {
                int id = service.AddInspector(inspector);
                return Ok(id);
            }
            catch (InspectorAlreadyExistsException)
            {
                return ValidationFailedResult(null, ErrorCode.InspectorAlreadyExists);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectorsAddRecords)]
        public IActionResult AddUnregisteredInspector([FromBody] UnregisteredPersonEditDTO inspector)
        {
            try
            {
                int id = service.AddUnregisteredInspector(inspector);
                return Ok(id);
            }
            catch (InspectorAlreadyExistsException)
            {
                return ValidationFailedResult(null, ErrorCode.InspectorAlreadyExists);
            }
        }

        [HttpPut]
        [CustomAuthorize(Permissions.InspectorsEditRecords)]
        public IActionResult EditInspector([FromBody] InspectorsRegisterEditDTO inspector)
        {
            try
            {
                service.EditInspector(inspector);
                return Ok();
            }
            catch (InspectorAlreadyExistsException)
            {
                return ValidationFailedResult(null, ErrorCode.InspectorAlreadyExists);
            }
        }

        [HttpPut]
        [CustomAuthorize(Permissions.InspectorsEditRecords)]
        public IActionResult EditUnregisteredInspector([FromBody] UnregisteredPersonEditDTO inspector)
        {
            try
            {
                service.EditUnregisteredInspector(inspector);
                return Ok();
            }
            catch (InspectorAlreadyExistsException)
            {
                return ValidationFailedResult(null, ErrorCode.InspectorAlreadyExists);
            }
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.InspectorsDeleteRecords)]
        public IActionResult DeleteInspector([FromQuery] int id)
        {
            service.DeleteInspector(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.InspectorsRestoreRecords)]
        public IActionResult UndoDeleteInspector([FromQuery] int id)
        {
            service.UndoDeleteInspector(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectorsRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }
    }
}
