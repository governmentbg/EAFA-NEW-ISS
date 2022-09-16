using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class RecreationalFishingAdministrationController : BaseAuditController
    {
        private readonly IRecreationalFishingService service;
        private readonly IRecreationalFishingNomenclaturesService nomenclaturesService;
        private readonly IFileService fileService;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IUserService userService;
        private readonly IPersonService personService;

        public RecreationalFishingAdministrationController(IRecreationalFishingService service,
                                                           IRecreationalFishingNomenclaturesService nomenclaturesService,
                                                           IFileService fileService,
                                                           IApplicationsRegisterService applicationsRegisterService,
                                                           IUserService userService,
                                                           IPersonService personService,
                                                           IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
            this.nomenclaturesService = nomenclaturesService;
            this.fileService = fileService;
            this.userService = userService;
            this.personService = personService;
            this.applicationsRegisterService = applicationsRegisterService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public IActionResult GetTicket([FromQuery] int id)
        {
            RecreationalFishingTicketDTO ticket = service.GetTicket(id);
            return Ok(ticket);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetTicketRegixData([FromQuery] int id)
        {
            RegixChecksWrapperDTO<RecreationalFishingTicketBaseRegixDataDTO> result = service.GetTicketRegixData(id);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsAddRecords)]
        public IActionResult AddTickets([FromForm] RecreationalFishingTicketsDTO ticketDtos)
        {
            if (ticketDtos.Tickets.All(x => x.HasUserConfirmed!.Value))
            {
                RecreationalFishingAddTicketsResultDTO result = service.AddTickets(
                    ticketDtos.Tickets,
                    ticketDtos.PaymentData,
                    CurrentUser.ID,
                    isOnline: false,
                    associationId: null
                );
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsAddRecords)]
        public IActionResult AddTicketDuplicate([FromBody] RecreationalFishingTicketDuplicateDTO data)
        {
            int id = service.AddTicketDuplicate(data, CurrentUser.ID);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsEditRecords, Permissions.TicketApplicationsEditRecords)]
        public IActionResult EditTicket([FromForm] RecreationalFishingTicketDTO ticket)
        {
            service.EditTicket(ticket);
            return Ok();
        }

        [HttpPut]
        [CustomAuthorize(Permissions.TicketApplicationsInspectAndCorrectRegiXData)]
        public IActionResult EditTicketRegixData([FromBody] RecreationalFishingTicketBaseRegixDataDTO ticket)
        {
            service.EditTicketRegixData(ticket);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsAddRecords)]
        public IActionResult CalculateTicketValidToDate([FromBody] RecreationalFishingTicketValidToCalculationParamsDTO parameters)
        {
            DateTime result = service.CalculateTicketValidToDate(parameters);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsAddRecords)]
        public IActionResult CheckEgnLncPurchaseAbility([FromBody] RecreationalFishingTicketValidationDTO data)
        {
            RecreationalFishingTicketValidationResultDTO result = service.CheckEgnLncPurchaseAbility(data);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsAddRecords)]
        public IActionResult CheckTicketNumbersAvailability([FromQuery] List<string> ticketNumbers)
        {
            List<bool> result = service.CheckTicketNumbersAvailability(ticketNumbers);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public IActionResult GetPersonData([FromQuery] string egnLnc, [FromQuery] IdentifierTypeEnum idType)
        {
            int? personId = userService.GetPersonIdByEgnLnc(egnLnc, idType);

            if (personId.HasValue)
            {
                RecreationalFishingTicketHolderDTO holder = service.GetTicketHolderData(personId.Value);
                return Ok(holder);
            }

            return Ok(null);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public IActionResult GetPhoto([FromQuery] int fileId)
        {
            string photo = personService.GetPersonPhotoByFileId(fileId);
            return Ok(photo);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public IActionResult GetPersonPhoto([FromQuery] string egnLnc, [FromQuery] IdentifierTypeEnum idType)
        {
            int? personId = userService.GetPersonIdByEgnLnc(egnLnc, idType);

            if (personId.HasValue)
            {
                string photo = personService.GetPersonPhoto(personId.Value);
                return Ok(photo);
            }

            return BadRequest(ErrorResources.msgEgnLnchDoesntExist + ": " + egnLnc);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public async Task<IActionResult> DownloadFishingTicket([FromQuery] int ticketId)
        {
            Stream file = await service.DownloadFishingTicket(ticketId);
            return File(file, "application/pdf");
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public async Task<IActionResult> DownloadTicketDeclaration([FromBody] RecreationalFishingTicketDeclarationParametersDTO parameters)
        {
            Stream file = await service.DownloadTicketDeclaration(parameters, CurrentUser.ID);
            return File(file, "application/pdf");
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public IActionResult GetTicketTypes()
        {
            List<NomenclatureDTO> ticketTypes = nomenclaturesService.GetTicketTypes();
            return Ok(ticketTypes);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public IActionResult GetTicketPeriods()
        {
            List<NomenclatureDTO> ticketPeriods = nomenclaturesService.GetTicketPeriods();
            return Ok(ticketPeriods);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public IActionResult GetTicketPrices()
        {
            List<RecreationalFishingTicketPriceDTO> prices = nomenclaturesService.GetTicketPrices();
            return Ok(prices);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public IActionResult GetAllFishingAssociations()
        {
            List<NomenclatureDTO> associations = nomenclaturesService.GetAllFishingAssociations();
            return Ok(associations);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public IActionResult GetAllTicketApplications([FromBody] GridRequestModel<RecreationalFishingTicketApplicationFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.TicketsApplicationsReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new RecreationalFishingTicketApplicationFilters
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

            IQueryable<RecreationalFishingTicketApplicationDTO> query = service.GetAllTickets(request.Filters);

            GridResultModel<RecreationalFishingTicketApplicationDTO> result = new(query, request, false);
            service.SetTicketsApplicationStatus(result.Records);

            return Ok(result);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.TicketApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            service.DeleteTicket(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.TicketApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            service.UndoDeleteTicket(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsApplicationsReadAll, Permissions.TicketApplicationsRead)]
        public IActionResult GetAllTicketStatuses()
        {
            List<NomenclatureDTO> statuses = applicationsRegisterService.GetApplicationStatuses(ApplicationHierarchyTypesEnum.RecreationalFishingTicket);
            return Ok(statuses);
        }
    }
}
