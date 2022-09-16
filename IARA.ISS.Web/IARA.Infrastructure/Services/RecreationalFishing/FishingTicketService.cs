using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingTickets;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class FishingTicketService : IFishingTicketService
    {
        private readonly IPersonService _personService;
        private readonly IARADbContext Db;
        public FishingTicketService(IARADbContext db, IPersonService personService)
        {
            Db = db;
            _personService = personService;
        }

        public void InactivatePastTickets()
        {
            List<FishingTicket> tickets = (from ticket in Db.FishingTickets
                                           join nTicketStatus in Db.NticketStatuses on ticket.TicketStatusId equals nTicketStatus.Id
                                           where ticket.TicketValidTo < DateTime.Now
                                             && ticket.IsActive
                                             && nTicketStatus.Code != nameof(TicketStatusEnum.EXPIRED)
                                           select ticket).ToList();

            DateTime now = DateTime.Now;
            NticketStatus ticketStatus = Db.NticketStatuses.Where(x => x.Code == nameof(TicketStatusEnum.EXPIRED)
                                                          && x.ValidFrom <= now
                                                          && x.ValidTo > now)
                                                 .First();

            foreach (var ticket in tickets)
            {
                ticket.TicketStatus = ticketStatus;
            }

            Db.SaveChanges();
        }

        public int AllowedUnder14TicketsCount(int currentUserId)
        {
            DateTime now = DateTime.Now;

            EgnLncDTO egnLnch = Db.GetUserEgn(currentUserId);

            int under14TicketCount = (from ticket in Db.FishingTickets
                                      join status in Db.NticketStatuses on ticket.TicketStatusId equals status.Id
                                      join ticketType in Db.NticketTypes on ticket.TicketTypeId equals ticketType.Id
                                      join appl in Db.Applications on ticket.ApplicationId equals appl.Id
                                      join prsn in Db.Persons on appl.SubmittedByPersonId equals prsn.Id
                                      where prsn.EgnLnc == egnLnch.EgnLnc
                                      && prsn.IdentifierType == egnLnch.IdentifierType.ToString()
                                      && ticket.TicketValidTo >= now && ticket.IsActive
                                      && ticketType.Code == nameof(TicketTypeEnum.UNDER14)
                                      && ticket.DuplicateOfTicketId == null
                                      select ticket.Id).Count();

            string maxAllowedUnder14Tickets = Db.NsystemParameters
                .Where(x => x.Code == "MAX_NUMBER_OF_UNDER14_TICKETS" && x.ValidFrom < now && x.ValidTo > now)
                .Select(f => f.ParamValue)
                .First();

            return Math.Max(0, int.Parse(maxAllowedUnder14Tickets) - under14TicketCount);
        }

        public List<FishingTicketDTO> GetBasePersonTicketsData(int currentUserId)
        {
            EgnLncDTO egnLnch = Db.GetUserEgn(currentUserId);

            List<FishingTicketDTO> tickets = (from ticket in Db.FishingTickets
                                              join ticketType in Db.NticketTypes on ticket.TicketTypeId equals ticketType.Id
                                              join ticketPeriod in Db.NticketPeriods on ticket.TicketPeriodId equals ticketPeriod.Id
                                              join ticketStatus in Db.NticketStatuses on ticket.TicketStatusId equals ticketStatus.Id
                                              join application in Db.Applications on ticket.ApplicationId equals application.Id
                                              join applicationStatus in Db.NapplicationStatuses on application.ApplicationStatusId equals applicationStatus.Id
                                              join applicationPayment in Db.ApplicationPayments on application.Id equals applicationPayment.ApplicationId
                                              join paymentStatus in Db.NPaymentStatuses on applicationPayment.PaymentStatusId equals paymentStatus.Id
                                              join status in Db.NticketStatuses on ticket.TicketStatusId equals status.Id
                                              join person in Db.Persons on ticket.PersonId equals person.Id
                                              join representative in Db.Persons on ticket.PersonRepresentativeId equals representative.Id into repr
                                              from representative in repr.DefaultIfEmpty()
                                              where ((person.EgnLnc == egnLnch.EgnLnc && person.IdentifierType == egnLnch.IdentifierType.ToString())
                                                || (representative != null && representative.EgnLnc == egnLnch.EgnLnc && representative.IdentifierType == egnLnch.IdentifierType.ToString()))
                                                    && ticket.IsActive
                                                    && ticket.DuplicateOfTicketId == null
                                              orderby ticket.TicketValidFrom descending
                                              select new FishingTicketDTO
                                              {
                                                  Id = ticket.Id,
                                                  TypeId = ticketType.Id,
                                                  Type = ticketType.Code,
                                                  TypeName = ticketType.Name,
                                                  PeriodId = ticketPeriod.Id,
                                                  PeriodCode = ticketPeriod.Code,
                                                  PeriodName = ticketPeriod.Name,
                                                  Price = ticket.Price,
                                                  ValidFrom = ticket.TicketValidFrom,
                                                  ValidTo = ticket.TicketValidTo,
                                                  StatusCode = ticketStatus.Code,
                                                  StatusName = ticketStatus.Name,
                                                  PersonFullName = $"{person.FirstName} " + (string.IsNullOrEmpty(person.MiddleName) ? $"{person.LastName}" : $"{person.MiddleName} {person.LastName}"),
                                                  PaymentStatus = paymentStatus.Code,
                                                  ApplicationId = application.Id,
                                                  ApplicationStatusCode = applicationStatus.Code,
                                                  ApplicationStatusReason = applicationStatus.Code == nameof(ApplicationStatusesEnum.FILL_BY_APPL) ||
                                                                                   applicationStatus.Code == nameof(ApplicationStatusesEnum.CORR_BY_USR_NEEDED) ? application.StatusReason : null
                                              }).ToList();

            return tickets;
        }

        public DownloadableFileDTO GetPersonPhoto(int photoId, int ticketId, bool isRepresentative)
        {
            if (isRepresentative)
            {
                DownloadableFileDTO result = (from ticket in Db.FishingTickets
                                              join person in Db.Persons on ticket.PersonRepresentativeId equals person.Id
                                              join pFile in Db.PersonFiles on person.Id equals pFile.RecordId
                                              join photo in Db.Files on pFile.FileId equals photo.Id
                                              join fType in Db.NfileTypes on pFile.FileTypeId equals fType.Id
                                              where photo.Id == photoId
                                              && ticket.Id == ticketId
                                                 && pFile.IsActive
                                                 && photo.IsActive
                                                 && fType.Code == nameof(FileTypeEnum.PHOTO)
                                              select new DownloadableFileDTO
                                              {
                                                  MimeType = photo.MimeType,
                                                  Bytes = photo.Content,
                                                  FileName = photo.Name
                                              }).SingleOrDefault();
                return result;
            }
            else
            {
                DownloadableFileDTO result = (from ticket in Db.FishingTickets
                                              join person in Db.Persons on ticket.PersonId equals person.Id
                                              join pFile in Db.PersonFiles on person.Id equals pFile.RecordId
                                              join photo in Db.Files on pFile.FileId equals photo.Id
                                              join fType in Db.NfileTypes on pFile.FileTypeId equals fType.Id
                                              where photo.Id == photoId
                                              && ticket.Id == ticketId
                                                 && pFile.IsActive
                                                 && photo.IsActive
                                                 && fType.Code == nameof(FileTypeEnum.PHOTO)
                                              select new DownloadableFileDTO
                                              {
                                                  MimeType = photo.MimeType,
                                                  Bytes = photo.Content,
                                                  FileName = photo.Name
                                              }).SingleOrDefault();
                return result;
            }

        }
        public bool HasAccessToUpdateProfileData(int userId, EgnLncDTO EgnLnc)
        {
            return (from user in Db.Users
                    join person in Db.Persons on user.PersonId equals person.Id
                    where user.Id == userId && person.EgnLnc == EgnLnc.EgnLnc && person.IdentifierType == EgnLnc.IdentifierType.ToString()
                    select person.Id).Any();
        }

        public bool IsDuplicateTicket(TicketValidationDTO ticket)
        {
            DateTime validTo;
            if (ticket.TypeCode == nameof(TicketTypeEnum.UNDER14))
            {
                DateTime date = ticket.ChildDateOfBirth.Value.AddYears(14);
                validTo = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

            }
            else if (ticket.TypeCode == nameof(TicketTypeEnum.DISABILITY))
            {
                validTo = ticket.TELKIsIndefinite ? DefaultConstants.MAX_VALID_DATE : ticket.TELKValidTo.Value;
            }
            else //NO PERIOD TICKETS
            {
                if (!Enum.TryParse(ticket.PeriodCode, true, out TicketPeriodEnum ticketPeriod))
                {
                    throw new ArgumentException($"Invalid ticket period {ticket.PeriodCode}");
                }

                validTo = this.GetFixedPeriodsValidTo(ticketPeriod, ticket.ValidFrom.Value);
            }

            return (from fticket in Db.FishingTickets
                    join nticketStatus in Db.NticketStatuses on fticket.TicketStatusId equals nticketStatus.Id
                    join nticketType in Db.NticketTypes on fticket.TicketTypeId equals nticketType.Id
                    join nticketPeriod in Db.NticketPeriods on fticket.TicketPeriodId equals nticketPeriod.Id
                    join appl in Db.Applications on fticket.ApplicationId equals appl.Id
                    join prsn in Db.Persons on appl.SubmittedForPersonId equals prsn.Id
                    where fticket.TicketValidTo == validTo &&
                    fticket.IsActive &&
                    appl.IsActive &&
                    nticketStatus.Code != nameof(TicketStatusEnum.CANCELED) &&
                    nticketType.Code == ticket.TypeCode &&
                    nticketPeriod.Code == ticket.PeriodCode &&
                    prsn.EgnLnc == ticket.EgnLnch
                    select fticket.Id).Any();
        }



        public void UpdateUserProfileData(RegixPersonDataDTO personData, List<AddressRegistrationDTO> userAddresses, FileInfoDTO photo, int userId)
        {
            var dbPerson = (from user in Db.Users
                            join person in Db.Persons on user.PersonId equals person.Id
                            where user.Id == userId && person.EgnLnc == personData.EgnLnc.EgnLnc && person.IdentifierType == personData.EgnLnc.IdentifierType.ToString()
                            select new { PersonId = person.Id, GenderId = person.GenderId, BirthDate = person.BirthDate }).First();
            personData.Email = _personService.GetPersonEmail(dbPerson.PersonId);
            personData.Phone = _personService.GetPersonPhoneNumber(dbPerson.PersonId);

            if (personData.Document == null)
            {
                personData.Document = _personService.GetPersonDocument(dbPerson.PersonId);
            }

            if (personData.GenderId == null)
            {
                personData.GenderId = dbPerson.GenderId;
            }

            if (!personData.BirthDate.HasValue)
            {
                personData.BirthDate = dbPerson.BirthDate;
            }
            Db.AddOrEditPerson(personData, userAddresses, dbPerson.PersonId, photo);
            Db.SaveChanges();
        }

        public bool HasAccessToTicket(int userId, int ticketId)
        {
            EgnLncDTO egnLnch = Db.GetUserEgn(userId);
            return (from ticket in Db.FishingTickets
                    join person in Db.Persons on ticket.PersonId equals person.Id
                    join representative in Db.Persons on ticket.PersonRepresentativeId equals representative.Id into repr
                    from representative in repr.DefaultIfEmpty()
                    where ticket.Id == ticketId
                    && ((person.EgnLnc == egnLnch.EgnLnc && person.IdentifierType == egnLnch.IdentifierType.ToString())
                        || (representative != null && representative.EgnLnc == egnLnch.EgnLnc && representative.IdentifierType == egnLnch.IdentifierType.ToString()))
                    select ticket.Id).Any();
        }

        private DateTime DateRound(DateTime date)
        {
            date = date.AddDays(-1);
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }

        private DateTime GetFixedPeriodsValidTo(TicketPeriodEnum period, DateTime validFrom)
        {
            return period switch
            {
                TicketPeriodEnum.ANNUAL => DateRound(validFrom.AddYears(1)),
                TicketPeriodEnum.HALFYEARLY => DateRound(validFrom.AddMonths(6)),
                TicketPeriodEnum.MONTHLY => DateRound(validFrom.AddMonths(1)),
                TicketPeriodEnum.WEEKLY => DateRound(validFrom.AddDays(7)),
                TicketPeriodEnum.NOPERIOD => throw new NotImplementedException(),
                _ => throw new ArgumentException($"Unexpected period {period}"),
            };
        }
    }
}
