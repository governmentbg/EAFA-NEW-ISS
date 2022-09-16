using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingTickets;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    public class FishingTicketsController : BaseController
    {
        private readonly IFishingAssociationService fishingAssociationService;
        private readonly IFishingTicketService fishingTicketService;
        private readonly IRecreationalFishingService recreationalFishingService;

        public FishingTicketsController(IFishingAssociationService fishingAssociationService,
                                        IFishingTicketService fishingTicketService,
                                        IPermissionsService permissionsService,
                                        IRecreationalFishingService recreationalFishingService)
            : base(permissionsService)
        {
            this.fishingAssociationService = fishingAssociationService;
            this.fishingTicketService = fishingTicketService;
            this.recreationalFishingService = recreationalFishingService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public IActionResult GetTicket(int id)
        {
            if (this.recreationalFishingService.HasUserAccessToTickets(this.CurrentUser.ID, id))
            {
                return this.Ok(this.recreationalFishingService.GetTicket(id));
            }
            return this.NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public IActionResult GetTickets()
        {
            return this.Ok(this.fishingTicketService.GetBasePersonTicketsData(this.CurrentUser.ID));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsAddRecords)]
        public IActionResult AddTickets([FromForm] RecreationalTicketsExtendedDTO ticketDtos)
        {
            if (ticketDtos.UpdateProfileData && ticketDtos.Tickets.Count > 0)
                TryUpdateUserProfileData(ticketDtos.Tickets.First());

            var result = this.recreationalFishingService.AddTickets(ticketDtos.Tickets, payment: null, this.CurrentUser.ID, isOnline: true, null);
            return this.Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsEditRecords)]
        public IActionResult EditTicket([FromForm] RecreationalFishingTicketExtendedDTO ticket)
        {
            if (ticket.UpdateProfileData)
                TryUpdateUserProfileData(ticket);

            if (this.fishingTicketService.HasAccessToTicket(this.CurrentUser.ID, ticket.Id!.Value))
            {
                this.recreationalFishingService.EditTicket(ticket);
                return this.Ok();
            }
            return this.NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public IActionResult GetAllApprovedFishingAssociations()
        {
            return this.Ok(this.fishingAssociationService.GetAllApprovedFishingAssociations());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public IActionResult AllowedUnder14TicketsCount()
        {
            int count = this.fishingTicketService.AllowedUnder14TicketsCount(this.CurrentUser.ID);
            return this.Ok(count);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsRead)]
        public IActionResult IsDuplicateTicket(TicketValidationDTO ticket)
        {
            bool isDuplicate = this.fishingTicketService.IsDuplicateTicket(ticket);
            return this.Ok(isDuplicate);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public IActionResult GetUserPhoto(int photoId, int ticketId, bool isRepresentative)
        {
            DownloadableFileDTO photo = this.fishingTicketService.GetPersonPhoto(photoId, ticketId, isRepresentative);
            return this.File(photo.Bytes, photo.MimeType, photo.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TicketsAddRecords)]
        public IActionResult CalculateTicketValidToDate([FromBody] RecreationalFishingTicketValidToCalculationParamsDTO parameters)
        {
            DateTime result = this.recreationalFishingService.CalculateTicketValidToDate(parameters);
            return this.Ok(new { ValidTo = result });
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TicketsRead)]
        public async Task<IActionResult> Download([FromQuery] int ticketId)
        {
            if (this.fishingTicketService.HasAccessToTicket(this.CurrentUser.ID, ticketId))
            {
                Stream file = await this.recreationalFishingService.DownloadFishingTicket(ticketId);

                return File(file, "application/pdf");
            }

            return NotFound();
        }

        private void TryUpdateUserProfileData(RecreationalFishingTicketDTO ticket)
        {
            if (this.fishingTicketService.HasAccessToUpdateProfileData(this.CurrentUser.ID, ticket.Person.EgnLnc))
            {
                try
                {
                    this.fishingTicketService.UpdateUserProfileData(ticket.Person, ticket.PersonAddressRegistrations, ticket.PersonPhoto, this.CurrentUser.ID);
                }
                catch
                {
                }
            }
        }
    }
}
