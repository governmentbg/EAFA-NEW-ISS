using System.Linq;
using IARA.DomainModels.RequestModels;
using IARA.Infrastructure;
using IARA.Security;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using IARA.DomainModels.DTOModels.PrintConfigurations;
using IARA.Interfaces;
using IARA.WebHelpers;
using IARA.Security.Permissions;
using IARA.Interfaces.Nomenclatures;
using IARA.Common.Exceptions;
using IARA.WebHelpers.Enums;
using IARA.Logging.Abstractions.Interfaces;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class PrintConfigurationController : BaseAuditController
    {
        private readonly IPrintConfigurationsService service;
        private readonly ICommonNomenclaturesService commonNomenclaturesService;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IExtendedLogger logger;

        public PrintConfigurationController(IPermissionsService permissionsService, 
                                            IPrintConfigurationsService service,
                                            ICommonNomenclaturesService commonNomenclaturesService,
                                            IApplicationsRegisterService applicationsRegisterService,
                                            IExtendedLogger logger) 
            : base(permissionsService)
        {
            this.service = service;
            this.commonNomenclaturesService = commonNomenclaturesService;
            this.applicationsRegisterService = applicationsRegisterService;
            this.logger = logger;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PrintConfigurationsRead)]
        public IActionResult GetAllPrintConfigurations([FromBody] GridRequestModel<PrintConfigurationFilters> request)
        {
            IQueryable<PrintConfigurationDTO> configurations = service.GetAllPrintConfigurations(request.Filters);
            return PageResult(configurations, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PrintConfigurationsRead)]
        public IActionResult GetPrintConfiguration([FromQuery] int id)
        {
            PrintConfigurationEditDTO result = service.GetPrintConfiguration(id);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PrintConfigurationsAddRecords)]
        public IActionResult AddPrintConfiguration([FromBody] PrintConfigurationEditDTO model)
        {
            try
            {
                PrintConfigurationEditDTO result = service.AddOrEditPringConfiguration(model);
                return Ok(result.Id);
            }
            catch(PrintConfigurationAlreadyExistsException ex)
            {
                logger.LogException(ex, "PrintConfigurationController", "AddPrintConfiguration");
                return ValidationFailedResult(errorCode: ErrorCode.PrintConfigurationAlreadyExists);
            }
        }

        [HttpPut]
        [CustomAuthorize(Permissions.PrintConfigurationsEditRecords)]
        public IActionResult EditPrintConfiguration([FromBody] PrintConfigurationEditDTO model)
        {
            service.AddOrEditPringConfiguration(model);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.PrintConfigurationsDeleteRecords)]
        public IActionResult DeletePrintConfiguration([FromQuery] int id)
        {
            service.DeletePrintConfiguration(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.PrintConfigurationsRestoreRecords)]
        public IActionResult UndoDeletePrintConfiguration([FromQuery] int id)
        {
            service.UndoDeletePrintConfiguration(id);
            return Ok();
        }

        // Nomenclatures

        [HttpGet]
        [CustomAuthorize(Permissions.PrintConfigurationsRead)]
        public IActionResult GetApplicationTypes()
        {
            return Ok(commonNomenclaturesService.GetApplicationTypes());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PrintConfigurationsRead)]
        public IActionResult GetUsersNomenclature()
        {
            return Ok(applicationsRegisterService.GetUsersNomenclature());
        }

        // Simple audit

        [HttpGet]
        [CustomAuthorize(Permissions.PrintConfigurationsRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }
    }
}
