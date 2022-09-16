using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Common;
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

namespace IARA.Web.Controllers.Public
{
    [AreaRoute(AreaType.Public)]
    public class RecreationalFishingPublicController : BaseController
    {
        private readonly IRecreationalFishingService service;
        private readonly IRecreationalFishingNomenclaturesService nomenclaturesService;
        private readonly IRecreationalFishingAssociationService associationsService;
        private readonly IFileService fileService;
        private readonly IUserService userService;
        private readonly IPersonService personService;

        public RecreationalFishingPublicController(IRecreationalFishingService service,
                                                   IRecreationalFishingNomenclaturesService nomenclaturesService,
                                                   IRecreationalFishingAssociationService associationsService,
                                                   IFileService fileService,
                                                   IUserService userService,
                                                   IPersonService personService,
                                                   IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
            this.nomenclaturesService = nomenclaturesService;
            this.associationsService = associationsService;
            this.fileService = fileService;
            this.userService = userService;
            this.personService = personService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead,
                         Permissions.TicketApplicationsRead,
                         Permissions.AssociationsTicketsRead,
                         Permissions.AssociationsTicketApplicationsRead)]
        public IActionResult GetTicket([FromQuery] int id)
        {
            if (service.HasUserAccessToTickets(CurrentUser.ID, id))
            {
                RecreationalFishingTicketDTO ticket = service.GetTicket(id);
                return Ok(ticket);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsAddRecords, Permissions.AssociationsTicketsAddRecords)]
        public IActionResult AddTickets([FromForm] RecreationalFishingTicketsDTO ticketDtos)
        {
            RecreationalFishingAddTicketsResultDTO result = service.AddTickets(
                ticketDtos.Tickets,
                ticketDtos.PaymentData,
                CurrentUser.ID,
                isOnline: !ticketDtos.AssociationId.HasValue,
                ticketDtos.AssociationId
            );
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsAddRecords, Permissions.AssociationsTicketsAddRecords)]
        public IActionResult AddTicketDuplicate([FromBody] RecreationalFishingTicketDuplicateDTO data)
        {
            int id = service.AddTicketDuplicate(data, CurrentUser.ID);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsEditRecords,
                         Permissions.TicketApplicationsEditRecords,
                         Permissions.AssociationsTicketApplicationsEditRecords)]
        public IActionResult EditTicket([FromForm] RecreationalFishingTicketDTO ticket)
        {
            if (service.HasUserAccessToTickets(CurrentUser.ID, ticket.Id!.Value))
            {
                service.EditTicket(ticket);
                return Ok();
            }
            return NotFound();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.TicketsEditRecords,
                         Permissions.TicketApplicationsEditRecords,
                         Permissions.AssociationsTicketApplicationsEditRecords)]
        public IActionResult CancelTicket([FromQuery] int id, [FromBody] ReasonDTO reason)
        {
            if (service.HasUserAccessToTickets(CurrentUser.ID, id))
            {
                service.CancelTicket(id, reason.Reason);
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsAddRecords, Permissions.AssociationsTicketsAddRecords)]
        public IActionResult CalculateTicketValidToDate([FromBody] RecreationalFishingTicketValidToCalculationParamsDTO parameters)
        {
            DateTime result = service.CalculateTicketValidToDate(parameters);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsAddRecords, Permissions.AssociationsTicketsAddRecords)]
        public IActionResult CheckEgnLncPurchaseAbility([FromBody] RecreationalFishingTicketValidationDTO data)
        {
            RecreationalFishingTicketValidationResultDTO result = service.CheckEgnLncPurchaseAbility(data);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsAddRecords, Permissions.AssociationsTicketsAddRecords)]
        public IActionResult CheckTicketNumbersAvailability([FromQuery] List<string> ticketNumbers)
        {
            List<bool> result = service.CheckTicketNumbersAvailability(ticketNumbers);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsAddRecords, Permissions.TicketsEditRecords)]
        public IActionResult UpdateUserDataFromTicket([FromForm] RecreationalFishingUserTicketDataDTO userData)
        {
            service.UpdateUserDataFromTicket(userData, CurrentUser.ID);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public IActionResult GetAllUserTickets()
        {
            List<RecreationalFishingTicketCardDTO> tickets = service.GetAllUserTickets(CurrentUser.ID);
            return Ok(tickets);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public IActionResult GetUserRequestedOrActiveTickets()
        {
            List<RecreationalFishingTicketViewDTO> tickets = service.GetRequestedOrActiveTickets(CurrentUser.ID);
            return Ok(tickets);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public IActionResult GetUserPersonData()
        {
            int personId = userService.GetUserPersonId(CurrentUser.ID);
            RecreationalFishingTicketHolderDTO holder = service.GetTicketHolderData(personId);
            return Ok(holder);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AssociationsTicketsRead, Permissions.AssociationsTicketApplicationsRead)]
        public IActionResult GetUserAssociations()
        {
            List<NomenclatureDTO> assocs = associationsService.GetUserFishingAssociations(CurrentUser.ID);
            return Ok(assocs);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public IActionResult GetUserPhoto()
        {
            int personId = userService.GetUserPersonId(CurrentUser.ID);
            string photo = personService.GetPersonPhoto(personId);
            return Ok(photo);
        }


        [HttpGet]
        [CustomAuthorize(Permissions.AssociationsTicketsRead, Permissions.AssociationsTicketApplicationsRead)]
        public IActionResult GetPersonData([FromQuery] string egnLnc, [FromQuery] IdentifierTypeEnum idType, [FromQuery] int associationId)
        {
            int? personId = userService.GetPersonIdByEgnLnc(egnLnc, idType);

            if (personId.HasValue && service.HasAssociationAccessToPersonData(associationId, personId.Value))
            {
                RecreationalFishingTicketHolderDTO holder = service.GetTicketHolderData(personId.Value);
                return Ok(holder);
            }

            return Ok(null);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead,
                         Permissions.TicketApplicationsRead,
                         Permissions.AssociationsTicketsRead,
                         Permissions.AssociationsTicketApplicationsRead)]
        public IActionResult GetPhoto([FromQuery] int fileId)
        {
            string photo = personService.GetPersonPhotoByFileId(fileId);
            return Ok(photo);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AssociationsTicketsRead, Permissions.AssociationsTicketApplicationsRead)]
        public IActionResult GetPersonPhoto([FromQuery] string egnLnc, [FromQuery] IdentifierTypeEnum idType, [FromQuery] int associationId)
        {
            int? personId = userService.GetPersonIdByEgnLnc(egnLnc, idType);

            if (personId.HasValue && service.HasAssociationAccessToPersonData(associationId, personId.Value))
            {
                string photo = personService.GetPersonPhoto(personId.Value);
                return Ok(photo);
            }

            return Ok(null);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead,
                         Permissions.TicketApplicationsRead,
                         Permissions.AssociationsTicketsRead,
                         Permissions.AssociationsTicketApplicationsRead)]
        public IActionResult DownloadFile(int id)
        {
            if (service.HasUserAccessToTicketFile(CurrentUser.ID, id))
            {
                DownloadableFileDTO file = fileService.GetFileForDownload(id);
                return File(file.Bytes, file.MimeType, file.FileName);
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead, Permissions.AssociationsTicketsRead)]
        public async Task<IActionResult> DownloadFishingTicket([FromQuery] int ticketId)
        {
            if (service.HasUserAccessToTickets(CurrentUser.ID, ticketId))
            {
                Stream file = await service.DownloadFishingTicket(ticketId);
                return File(file, "application/pdf");
            }

            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AssociationsTicketsRead)]
        public async Task<IActionResult> DownloadTicketDeclaration([FromBody] RecreationalFishingTicketDeclarationParametersDTO parameters)
        {
            Stream file = await service.DownloadTicketDeclaration(parameters, CurrentUser.ID);
            return File(file, "application/pdf");
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead,
                         Permissions.TicketApplicationsRead,
                         Permissions.AssociationsTicketsRead,
                         Permissions.AssociationsTicketApplicationsRead)]
        public IActionResult GetTicketTypes()
        {
            List<NomenclatureDTO> ticketTypes = nomenclaturesService.GetTicketTypes();
            return Ok(ticketTypes);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead,
                         Permissions.TicketApplicationsRead,
                         Permissions.AssociationsTicketsRead,
                         Permissions.AssociationsTicketApplicationsRead)]
        public IActionResult GetTicketPeriods()
        {
            List<NomenclatureDTO> ticketPeriods = nomenclaturesService.GetTicketPeriods();
            return Ok(ticketPeriods);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead,
                         Permissions.TicketApplicationsRead,
                         Permissions.AssociationsTicketsRead,
                         Permissions.AssociationsTicketApplicationsRead)]
        public IActionResult GetTicketPrices()
        {
            List<RecreationalFishingTicketPriceDTO> prices = nomenclaturesService.GetTicketPrices();
            return Ok(prices);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead,
                         Permissions.TicketApplicationsRead,
                         Permissions.AssociationsTicketsRead,
                         Permissions.AssociationsTicketApplicationsRead)]
        public IActionResult GetAllFishingAssociations()
        {
            List<NomenclatureDTO> associations = nomenclaturesService.GetAllFishingAssociations();
            return Ok(associations);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketApplicationsRead, Permissions.AssociationsTicketApplicationsRead)]
        public IActionResult GetAllAssociationTicketApplications([FromQuery] int associationId, [FromBody] GridRequestModel<RecreationalFishingTicketApplicationFilters> request)
        {
            IQueryable<RecreationalFishingTicketApplicationDTO> query = service.GetAllTickets(request.Filters, associationId);

            GridResultModel<RecreationalFishingTicketApplicationDTO> result = new(query, request, false);
            service.SetTicketsApplicationStatus(result.Records);

            return Ok(result);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.TicketApplicationsDeleteRecords, Permissions.AssociationsTicketApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            if (service.HasUserAccessToTickets(CurrentUser.ID, id))
            {
                service.DeleteTicket(id);
                return Ok();
            }
            return NotFound();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.TicketApplicationsRestoreRecords, Permissions.AssociationsTicketApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            if (service.HasUserAccessToTickets(CurrentUser.ID, id))
            {
                service.UndoDeleteTicket(id);
                return Ok();
            }
            return NotFound();
        }
    }
}
