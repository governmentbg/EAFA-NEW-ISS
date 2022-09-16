using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.FSM;
using IARA.Interfaces.Reports;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public class RecreationalFishingService : Service, IRecreationalFishingService
    {
        private readonly IApplicationService applicationService;
        private readonly IPersonService personService;
        private readonly IRecreationalFishingAssociationService associationsService;
        private readonly IApplicationStateMachine stateMachine;
        private readonly IRegixApplicationInterfaceService regixApplicationService;
        private readonly IJasperReportExecutionService jasperReportsClient;

        public RecreationalFishingService(IARADbContext db,
                                          IApplicationService applicationService,
                                          IPersonService personService,
                                          IRecreationalFishingAssociationService associationsService,
                                          IApplicationStateMachine stateMachine,
                                          IRegixApplicationInterfaceService regixApplicationService,
                                          IJasperReportExecutionService jasperReportsClient)
            : base(db)
        {
            this.applicationService = applicationService;
            this.personService = personService;
            this.associationsService = associationsService;
            this.stateMachine = stateMachine;
            this.regixApplicationService = regixApplicationService;
            this.jasperReportsClient = jasperReportsClient;
        }

        public RecreationalFishingTicketDTO GetTicket(int id)
        {
            var data = (from ticket in Db.FishingTickets
                        join appl in Db.Applications on ticket.ApplicationId equals appl.Id
                        join member in Db.FishingAssociationMembers on ticket.AssociationMemberId equals member.Id into mem
                        from member in mem.DefaultIfEmpty()
                        where ticket.Id == id
                        select new
                        {
                            ticket.PersonId,
                            ticket.PersonRepresentativeId,
                            ticket.DuplicateOfTicketId,
                            Ticket = new RecreationalFishingTicketDTO
                            {
                                Id = ticket.Id,
                                TicketNum = ticket.IsOnlineTicket ? "E-" + ticket.Id : ticket.TicketNum,
                                ApplicationId = ticket.ApplicationId,
                                TypeId = ticket.TicketTypeId,
                                PeriodId = ticket.TicketPeriodId,
                                Price = ticket.Price,
                                IssuedOn = appl.SubmitDateTime,
                                ValidFrom = ticket.TicketValidFrom,
                                ValidTo = ticket.TicketValidTo,
                                TelkData = new RecreationalFishingTelkDTO
                                {
                                    IsIndefinite = ticket.TelkisIndefinite,
                                    Num = ticket.Telknum,
                                    ValidTo = ticket.TelkvalidTo,
                                },
                                MembershipCard = member != null
                                      ? new RecreationalFishingMembershipCardDTO
                                      {
                                          AssociationId = member.FishingAssociationId,
                                          CardNum = member.MembershipCardNum,
                                          IssueDate = member.MembershipFromDate
                                      }
                                      : null,
                                Comment = ticket.Comment
                            }
                        }).First();

            RecreationalFishingTicketDTO result = data.Ticket;

            if (data.DuplicateOfTicketId.HasValue)
            {
                result.DuplicateOfTicketNum = (from ticket in Db.FishingTickets
                                               where ticket.Id == data.DuplicateOfTicketId.Value
                                               select ticket.TicketNum).First();
            }
            else
            {
                result.TicketDuplicates = GetTicketDuplicates(result.Id.Value);
            }

            result.Person = personService.GetRegixPersonData(data.PersonId);
            result.PersonAddressRegistrations = personService.GetAddressRegistrations(data.PersonId);

            result.PersonPhoto = (from personFile in Db.PersonFiles
                                  join photo in Db.Files on personFile.FileId equals photo.Id
                                  join fileType in Db.NfileTypes on personFile.FileTypeId equals fileType.Id
                                  where fileType.Code == nameof(FileTypeEnum.PHOTO)
                                       && personFile.RecordId == data.PersonId
                                       && personFile.IsActive
                                       && photo.IsActive
                                  select new FileInfoDTO
                                  {
                                      Id = photo.Id
                                  }).SingleOrDefault();

            if (data.PersonRepresentativeId.HasValue)
            {
                result.RepresentativePerson = personService.GetRegixPersonData(data.PersonRepresentativeId.Value);
                result.RepresentativePersonAddressRegistrations = personService.GetAddressRegistrations(data.PersonRepresentativeId.Value);
            }

            result.Files = Db.GetFiles(Db.FishingTicketFiles, result.Id.Value);

            return result;
        }

        public RegixChecksWrapperDTO<RecreationalFishingTicketBaseRegixDataDTO> GetTicketRegixData(int id)
        {
            int applicationId = Db.FishingTickets.Where(x => x.Id == id).Select(x => x.ApplicationId).First();

            var result = new RegixChecksWrapperDTO<RecreationalFishingTicketBaseRegixDataDTO>
            {
                DialogDataModel = GetApplicationRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetTicketChecks(applicationId),
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public RecreationalFishingTicketBaseRegixDataDTO GetApplicationRegixData(int applicationId)
        {
            TicketApplicationDataIds regixDataIds = GetRegixDataIdsByApplicationId(applicationId);

            RecreationalFishingTicketBaseRegixDataDTO regixData = new RecreationalFishingTicketBaseRegixDataDTO
            {
                Id = regixDataIds.Id,
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode,
                Person = personService.GetRegixPersonData(regixDataIds.PersonId),
                PersonAddressRegistrations = personService.GetAddressRegistrations(regixDataIds.PersonId),
            };

            if (!string.IsNullOrEmpty(regixDataIds.TelkNumber))
            {
                regixData.TelkData = new RecreationalFishingTelkDTO
                {
                    Num = regixDataIds.TelkNumber,
                    ValidTo = regixDataIds.TelkValidTo,
                    IsIndefinite = regixDataIds.TelkIsIndefinite
                };
            };

            if (regixDataIds.PersonRepresentativeId.HasValue)
            {
                regixData.RepresentativePerson = personService.GetRegixPersonData(regixDataIds.PersonRepresentativeId.Value);
                regixData.RepresentativePersonAddressRegistrations = personService.GetAddressRegistrations(regixDataIds.PersonRepresentativeId.Value);
            }


            return regixData;
        }

        public IQueryable<RecreationalFishingTicketApplicationDTO> GetAllTickets(RecreationalFishingTicketApplicationFilters filters, int? associationId)
        {
            IQueryable<RecreationalFishingTicketApplicationDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllTickets(showInactive, associationId);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredTickets(filters.FreeTextSearch, filters.ShowInactiveRecords, associationId)
                    : GetParametersFilteredTickets(filters, associationId);
            }

            return result;
        }

        public void SetTicketsApplicationStatus(List<RecreationalFishingTicketApplicationDTO> tickets)
        {
            List<int> ticketIds = tickets.Select(x => x.Id).ToList();

            var statuses = (from ticket in Db.FishingTickets
                            join appl in Db.Applications on ticket.ApplicationId equals appl.Id
                            join applStatus in Db.NapplicationStatuses on appl.ApplicationStatusId equals applStatus.Id
                            where ticketIds.Contains(ticket.Id)
                            select new
                            {
                                TicketId = ticket.Id,
                                Status = Enum.Parse<ApplicationStatusesEnum>(applStatus.Code)
                            }).ToDictionary(x => x.TicketId, y => y.Status);

            foreach (RecreationalFishingTicketApplicationDTO ticket in tickets)
            {
                ticket.ApplicationStatus = statuses[ticket.Id];
            }
        }

        public List<RecreationalFishingTicketCardDTO> GetAllUserTickets(int userId)
        {
            DateTime now = DateTime.Now;

            EgnLncDTO egnLnc = Db.GetUserEgn(userId);

            List<int> personIds = (from person in Db.Persons
                                   where person.EgnLnc == egnLnc.EgnLnc && person.IdentifierType == egnLnc.IdentifierType.ToString()
                                   select person.Id).ToList();

            List<int> ticketIds = (from ticket in Db.FishingTickets
                                   where (personIds.Contains(ticket.PersonId)
                                        || (ticket.PersonRepresentativeId.HasValue && personIds.Contains(ticket.PersonRepresentativeId.Value)))
                                        && ticket.IsActive
                                   select ticket.Id).ToList();

            var tickets = (from ticket in Db.FishingTickets
                           join status in Db.NticketStatuses on ticket.TicketStatusId equals status.Id
                           join application in Db.Applications on ticket.ApplicationId equals application.Id
                           join applicationStatus in Db.NapplicationStatuses on application.ApplicationStatusId equals applicationStatus.Id
                           join applicationPayment in Db.ApplicationPayments on application.Id equals applicationPayment.ApplicationId
                           join paymentStatus in Db.NPaymentStatuses on applicationPayment.PaymentStatusId equals paymentStatus.Id
                           join type in Db.NticketTypes on ticket.TicketTypeId equals type.Id
                           join period in Db.NticketPeriods on ticket.TicketPeriodId equals period.Id
                           join person in Db.Persons on ticket.PersonId equals person.Id
                           where ticketIds.Contains(ticket.Id)
                           orderby ticket.CreatedOn descending
                           select new RecreationalFishingTicketCardDTO
                           {
                               Id = ticket.Id,
                               ApplicationId = ticket.ApplicationId,
                               TicketNum = ticket.IsOnlineTicket ? "E-" + ticket.Id : ticket.TicketNum,
                               Person = $"{person.FirstName} " + (string.IsNullOrEmpty(person.MiddleName) ? $"{person.LastName}"
                                                                                                          : $"{person.MiddleName} {person.LastName}"),
                               TypeId = type.Id,
                               PeriodId = period.Id,
                               Price = ticket.Price,
                               ValidFrom = ticket.TicketValidFrom,
                               ValidTo = ticket.TicketValidTo,
                               PaymentStatus = Enum.Parse<PaymentStatusesEnum>(paymentStatus.Code),
                               ApplicationStatus = Enum.Parse<ApplicationStatusesEnum>(applicationStatus.Code),
                               PercentOfPeriodCompleted = CalculatedPercentOfPeriodCompleted(ticket.TicketValidFrom, ticket.TicketValidTo),
                               IsPaymentProcessing = applicationStatus.Code == nameof(ApplicationStatusesEnum.PAYMENT_PROCESSING),
                               IsExpired = ticket.TicketValidTo <= now,
                               IsCanceled = status.Code == nameof(TicketStatusEnum.CANCELED)
                           }).ToList();

            foreach (RecreationalFishingTicketCardDTO ticket in tickets)
            {
                ticket.IsExpiringSoon = ticket.PercentOfPeriodCompleted >= 80;
            }

            return tickets;
        }

        public List<RecreationalFishingTicketViewDTO> GetRequestedOrActiveTickets(int userId)
        {
            EgnLncDTO egnLnc = Db.GetUserEgn(userId);

            List<int> personIds = (from person in Db.Persons
                                   where person.EgnLnc == egnLnc.EgnLnc && person.IdentifierType == egnLnc.IdentifierType.ToString()
                                   select person.Id).ToList();

            List<int> ticketIds = (from ticket in Db.FishingTickets
                                   where (personIds.Contains(ticket.PersonId)
                                        || (ticket.PersonRepresentativeId.HasValue && personIds.Contains(ticket.PersonRepresentativeId.Value)))
                                        && ticket.IsActive
                                   select ticket.Id).ToList();

            var tickets = (from ticket in Db.FishingTickets
                           join application in Db.Applications on ticket.ApplicationId equals application.Id
                           join applicationPayment in Db.ApplicationPayments on application.Id equals applicationPayment.ApplicationId
                           join paymentStatus in Db.NPaymentStatuses on applicationPayment.PaymentStatusId equals paymentStatus.Id
                           join status in Db.NticketStatuses on ticket.TicketStatusId equals status.Id
                           join person in Db.Persons on ticket.PersonId equals person.Id
                           where ticketIds.Contains(ticket.Id)
                           orderby ticket.CreatedOn descending
                           select new RecreationalFishingTicketViewDTO
                           {
                               ID = ticket.Id,
                               TypeId = ticket.TicketTypeId,
                               PeriodId = ticket.TicketPeriodId,
                               Person = $"{person.FirstName} " + (string.IsNullOrEmpty(person.MiddleName) ? $"{person.LastName}" : $"{person.MiddleName} {person.LastName}"),
                               ValidTo = ticket.TicketValidTo,
                               CancellationReason = status.Code == nameof(TicketStatusEnum.CANCELED) ? application.StatusReason : null,
                               PaymentStatus = Enum.Parse<PaymentStatusesEnum>(paymentStatus.Code)
                           }).ToList();

            return tickets;
        }

        public RecreationalFishingTicketHolderDTO GetTicketHolderData(int personId)
        {
            RegixPersonDataDTO data = personService.GetRegixPersonData(personId);
            List<AddressRegistrationDTO> addresses = personService.GetAddressRegistrations(personId);

            FileInfoDTO pic = (from personFile in Db.PersonFiles
                               join photo in Db.Files on personFile.FileId equals photo.Id
                               join fileType in Db.NfileTypes on personFile.FileTypeId equals fileType.Id
                               where personFile.RecordId == personId
                                    && fileType.Code == nameof(FileTypeEnum.PHOTO)
                                    && personFile.IsActive
                                    && photo.IsActive
                               select new FileInfoDTO
                               {
                                   Id = photo.Id
                               }).SingleOrDefault();

            RecreationalFishingTicketHolderDTO holder = new RecreationalFishingTicketHolderDTO()
            {
                Person = data,
                Addresses = addresses,
                Photo = pic
            };

            return holder;
        }

        public RecreationalFishingAddTicketsResultDTO AddTickets(List<RecreationalFishingTicketDTO> tickets,
                                                                 PaymentDataDTO payment,
                                                                 int currentUserId,
                                                                 bool isOnline,
                                                                 int? associationId)
        {
            RecreationalFishingAddTicketsResultDTO result = new RecreationalFishingAddTicketsResultDTO
            {
                PaidTicketApplicationId = null,
                TicketIds = new List<int>(),
                ChildTicketIds = new List<int>()
            };

            int applicationTypeId = GetRecFishingApplicationTypeId();
            foreach (RecreationalFishingTicketDTO ticket in tickets)
            {
                Application application = AddApplication(applicationTypeId, currentUserId, ticket, isOnline);
                (int? applicationId, int ticketId, TicketTypeEnum type) = AddRequestedTicketEntry(ticket,
                                                                                                  application,
                                                                                                  payment,
                                                                                                  currentUserId,
                                                                                                  isOnline,
                                                                                                  associationId);

                if (applicationId.HasValue)
                {
                    result.PaidTicketApplicationId = applicationId.Value;
                }

                if (type == TicketTypeEnum.UNDER14)
                {
                    result.ChildTicketIds.Add(ticketId);
                }
                else
                {
                    result.TicketIds.Add(ticketId);
                }
            }

            Db.SaveChanges();
            return result;
        }

        public int AddTicketDuplicate(RecreationalFishingTicketDuplicateDTO data, int currentUserId)
        {
            FishingTicket oldTicket = (from ticket in Db.FishingTickets
                                       where ticket.Id == data.TicketId.Value
                                       select ticket).First();

            if (oldTicket.DuplicateOfTicketId.HasValue)
            {
                oldTicket = (from ticket in Db.FishingTickets
                             where ticket.Id == oldTicket.DuplicateOfTicketId.Value
                             select ticket).First();
            }

            Application application = AddTicketDuplicateApplication(data, currentUserId);
            FishingTicket entry = AddTicketDuplicateEntry(data, application, oldTicket, currentUserId);

            oldTicket.TicketValidTo = DateTime.Now;
            Db.SaveChanges();

            AddTicketDuplicatePaymentData(entry, application, data.PaymentData);
            Db.SaveChanges();

            return entry.Id;
        }

        public ApplicationStatusesEnum EditTicket(RecreationalFishingTicketDTO ticket)
        {
            FishingTicket dbTicket = EditTicketRegixFields(ticket, ticket.PersonPhoto);

            if (dbTicket.AssociationMemberId.HasValue)
            {
                dbTicket.AssociationMember = AddOrEditAssociationMember(dbTicket.PersonId, ticket.MembershipCard);
            }

            dbTicket.Comment = ticket.Comment;

            if (ticket.Files != null)
            {
                foreach (FileInfoDTO file in ticket.Files)
                {
                    Db.AddOrEditFile(dbTicket, dbTicket.FishingTicketFiles, file);
                }
            }

            ApplicationStatusesEnum newState = stateMachine.Act(id: dbTicket.ApplicationId, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);
            return newState;
        }

        public ApplicationStatusesEnum EditTicketRegixData(RecreationalFishingTicketBaseRegixDataDTO ticket)
        {
            EditTicketRegixFields(ticket);

            ApplicationStatusesEnum newStatus = stateMachine.Act(id: ticket.ApplicationId.Value, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);
            return newStatus;
        }

        public void CancelTicket(int id, string reason)
        {
            FishingTicket ticket = (from tick in Db.FishingTickets
                                    where tick.Id == id
                                    select tick).First();

            ticket.TicketStatusId = GetTicketStatusId(TicketStatusEnum.CANCELED);

            applicationService.ApplicationAnnulment(ticket.ApplicationId, reason);
            Db.SaveChanges();
        }

        public void DeleteTicket(int id)
        {
            DeleteRecordWithId(Db.FishingTickets, id);
            Db.SaveChanges();
        }

        public void UndoDeleteTicket(int id)
        {
            UndoDeleteRecordWithId(Db.FishingTickets, id);
            Db.SaveChanges();
        }

        public DateTime CalculateTicketValidToDate(RecreationalFishingTicketValidToCalculationParamsDTO parameters)
        {
            TicketTypeEnum type = Enum.Parse<TicketTypeEnum>(Db.NticketTypes.Where(x => x.Id == parameters.TypeId).Select(x => x.Code).First());
            TicketPeriodEnum period = Enum.Parse<TicketPeriodEnum>(Db.NticketPeriods.Where(x => x.Id == parameters.PeriodId).Select(x => x.Code).First());

            return CalculateTicketValidToDate(type, period, parameters.ValidFrom, parameters.BirthDate, parameters.TelkValidTo);
        }

        public RecreationalFishingTicketValidationResultDTO CheckEgnLncPurchaseAbility(RecreationalFishingTicketValidationDTO data)
        {
            TicketTypeEnum type = Enum.Parse<TicketTypeEnum>(Db.NticketTypes.Where(x => x.Id == data.TypeId).Select(x => x.Code).First());
            TicketPeriodEnum period = Enum.Parse<TicketPeriodEnum>(Db.NticketPeriods.Where(x => x.Id == data.PeriodId).Select(x => x.Code).First());

            if (data.IsRepresentative && data.Under14TicketsCount > 0)
            {
                int current = (from ticket in Db.FishingTickets
                               join status in Db.NticketStatuses on ticket.TicketStatusId equals status.Id
                               join representative in Db.Persons on ticket.PersonRepresentativeId equals representative.Id
                               where representative.EgnLnc == data.PersonEgnLnc.EgnLnc
                                    && representative.IdentifierType == data.PersonEgnLnc.IdentifierType.ToString()
                                    && status.Code != nameof(TicketStatusEnum.CANCELED)
                                    && status.Code != nameof(TicketStatusEnum.EXPIRED)
                                    && ticket.IsActive
                               select ticket.Id).Count();

                return new RecreationalFishingTicketValidationResultDTO { CurrentlyActiveUnder14Tickets = current };
            }
            else
            {
                IQueryable<FishingTicket> query = from ticket in Db.FishingTickets
                                                  join status in Db.NticketStatuses on ticket.TicketStatusId equals status.Id
                                                  join person in Db.Persons on ticket.PersonId equals person.Id
                                                  where person.EgnLnc == data.PersonEgnLnc.EgnLnc
                                                      && person.IdentifierType == data.PersonEgnLnc.IdentifierType.ToString()
                                                      && ticket.TicketTypeId == data.TypeId
                                                      && ticket.TicketPeriodId == data.PeriodId
                                                      && status.Code != nameof(TicketStatusEnum.CANCELED)
                                                      && status.Code != nameof(TicketStatusEnum.EXPIRED)
                                                      && ticket.IsActive
                                                  select ticket;

                if (type != TicketTypeEnum.UNDER14)
                {
                    DateTime validTo = CalculateTicketValidToDate(type, period, data.ValidFrom);

                    query = from ticket in query
                            where ticket.TicketValidTo == validTo
                            select ticket;
                }

                if (query.Any())
                {
                    return new RecreationalFishingTicketValidationResultDTO { CannotPurchaseTicket = true };
                }
            }

            return new RecreationalFishingTicketValidationResultDTO();
        }

        public List<bool> CheckTicketNumbersAvailability(List<string> ticketNumbers)
        {
            List<string> existingNums = (from ticket in Db.FishingTickets
                                         where ticketNumbers.Contains(ticket.TicketNum.Trim())
                                         select ticket.TicketNum).ToList();

            List<bool> result = new List<bool>();
            foreach (string ticketNumber in ticketNumbers)
            {
                result.Add(existingNums.Contains(ticketNumber) == false);
            }
            return result;
        }

        public void UpdateUserDataFromTicket(RecreationalFishingUserTicketDataDTO data, int userId)
        {
            int personId = (from user in Db.Users
                            where user.Id == userId
                            select user.PersonId).First();

            Db.AddOrEditPerson(data.Person, data.AddressRegistrations, personId, data.Photo);
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.FishingTickets, id);
        }

        public Task<Stream> DownloadFishingTicket(int ticketId)
        {
            return jasperReportsClient.GetFishingTicket(ticketId);
        }

        public Task<Stream> DownloadTicketDeclaration(RecreationalFishingTicketDeclarationParametersDTO parameters, int currentUserId)
        {
            if (parameters.AssociationId.HasValue)
            {
                parameters.TerritoryUnit = (from assoc in Db.FishingAssociations
                                            join terUnit in Db.NterritoryUnits on assoc.TerritoryUnitId equals terUnit.Id
                                            where assoc.Id == parameters.AssociationId.Value
                                            select terUnit.Name).First();
            }
            else
            {
                parameters.TerritoryUnit = (from user in Db.Users
                                            join userInfo in Db.UserInfos on user.Id equals userInfo.UserId
                                            join terUnit in Db.NterritoryUnits on userInfo.TerritoryUnitId equals terUnit.Id
                                            where user.Id == currentUserId
                                            select terUnit.Name).First();
            }

            parameters.Code = (from tar in Db.Ntariffs
                               where tar.Code == $"Ticket_{parameters.Type.ToString().ToLower()}_{parameters.Period.ToString().ToLower()}"
                               select tar.Description).First();

            AddressRegistrationDTO address = parameters.PersonAddressRegistrations[0];

            parameters.Address = (from cnt in Db.Ncountries
                                  where cnt.Id == address.CountryId
                                  select cnt.Name).First();

            if (address.DistrictId.HasValue)
            {
                string district = (from dis in Db.Ndistricts
                                   where dis.Id == address.DistrictId.Value
                                   select dis.Name).First();

                parameters.Address = $"{parameters.Address}, обл. {district}";
            }

            if (address.MunicipalityId.HasValue)
            {
                string municipality = (from mun in Db.Nmunicipalities
                                       where mun.Id == address.MunicipalityId.Value
                                       select mun.Name).First();

                parameters.Address = $"{parameters.Address}, общ. {municipality}";
            }

            if (address.PopulatedAreaId.HasValue)
            {
                var populatedArea = (from pop in Db.NpopulatedAreas
                                     where pop.Id == address.PopulatedAreaId.Value
                                     select new
                                     {
                                         pop.Name,
                                         pop.AreaType
                                     }).First();

                string type = "";
                switch (populatedArea.AreaType)
                {
                    case 'Г':
                        type = "гр.";
                        break;
                    case 'С':
                        type = "с.";
                        break;
                    case 'М':
                        type = "ман.";
                        break;
                }

                parameters.Address = $"{parameters.Address}, {type} {populatedArea.Name}";
            }

            if (!string.IsNullOrEmpty(address.PostalCode))
            {
                parameters.Address = $"{parameters.Address} {address.PostalCode}";
            }

            if (!string.IsNullOrEmpty(address.Region))
            {
                parameters.Address = $"{parameters.Address}, {address.Region}";
            }

            parameters.Address = $"{parameters.Address}, {address.Street}";

            if (!string.IsNullOrEmpty(address.StreetNum))
            {
                parameters.Address = $"{parameters.Address} №{address.StreetNum}";
            }

            if (!string.IsNullOrEmpty(address.BlockNum))
            {
                parameters.Address = $"{parameters.Address}, бл. {address.BlockNum}";
            }

            if (!string.IsNullOrEmpty(address.EntranceNum))
            {
                parameters.Address = $"{parameters.Address}, вх. {address.EntranceNum}";
            }

            if (!string.IsNullOrEmpty(address.FloorNum))
            {
                parameters.Address = $"{parameters.Address}, ет. {address.FloorNum}";
            }

            if (!string.IsNullOrEmpty(address.ApartmentNum))
            {
                parameters.Address = $"{parameters.Address}, ап. {address.ApartmentNum}";
            }

            return jasperReportsClient.GetFishingTicketDeclaration(parameters);
        }

        // TODO по ЕГН
        public bool HasUserAccessToTickets(int userId, params int[] ticketIds)
        {
            var data = (from ticket in Db.FishingTickets
                        where ticketIds.Contains(ticket.Id)
                        select new
                        {
                            ticket.Id,
                            ticket.CreatedByUserId,
                            ticket.CreatedByFishingAssociationId
                        }).ToList();

            // вземаме всички билети, създадени от потребителя, които не са създадени от сдружение
            // така се подсигуряваме, че човек, ако вече не е част от сдружението, няма да има достъп
            List<int> createdByUserTicketIds = data.Where(x => x.CreatedByUserId == userId && !x.CreatedByFishingAssociationId.HasValue).Select(x => x.Id).ToList();
            if (data.Count == createdByUserTicketIds.Count)
            {
                return true;
            }

            data = data.Where(x => !createdByUserTicketIds.Contains(x.Id)).ToList();

            if (data.All(x => x.CreatedByFishingAssociationId.HasValue))
            {
                List<int> assocIds = associationsService.GetUserFishingAssociations(userId).Select(x => x.Value).ToList();
                return data.All(x => assocIds.Contains(x.CreatedByFishingAssociationId.Value));
            }
            return false;
        }

        public bool HasUserAccessToTicketFile(int userId, int fileId)
        {
            int[] ticketIds = (from tf in Db.FishingTicketFiles
                               where tf.FileId == fileId
                                    && tf.IsActive
                               select tf.RecordId).ToArray();

            return HasUserAccessToTickets(userId, ticketIds);
        }

        public bool HasAssociationAccessToPersonData(int associationId, int personId)
        {
            bool result = (from ticket in Db.FishingTickets
                           where ticket.CreatedByFishingAssociationId == associationId
                                && (ticket.PersonId == personId
                                    || ticket.PersonRepresentativeId == personId)
                           select ticket.Id).Any();

            return result;
        }

        private IQueryable<RecreationalFishingTicketApplicationDTO> GetAllTickets(bool showInactive, int? associationId = null)
        {
            var query = from ticket in Db.FishingTickets
                        select ticket;

            if (associationId.HasValue)
            {
                query = from ticket in Db.FishingTickets
                        where ticket.CreatedByFishingAssociationId == associationId.Value
                        select ticket;
            }

            var tickets = from ticket in query
                          join status in Db.NticketStatuses on ticket.TicketStatusId equals status.Id
                          join type in Db.NticketTypes on ticket.TicketTypeId equals type.Id
                          join period in Db.NticketPeriods on ticket.TicketPeriodId equals period.Id
                          join person in Db.Persons on ticket.PersonId equals person.Id
                          join issuerUser in Db.Users on ticket.CreatedByUserId equals issuerUser.Id
                          join issuerPerson in Db.Persons on issuerUser.PersonId equals issuerPerson.Id
                          where ticket.IsActive == !showInactive
                          orderby ticket.Id descending
                          select new RecreationalFishingTicketApplicationDTO
                          {
                              Id = ticket.Id,
                              ApplicationId = ticket.ApplicationId,
                              TicketNum = ticket.IsOnlineTicket ? "E-" + ticket.Id : ticket.TicketNum,
                              TicketHolderName = person.FirstName + " " + person.LastName,
                              ValidFrom = ticket.TicketValidFrom,
                              ValidTo = ticket.TicketValidTo,
                              TicketTypeId = type.Id,
                              TicketType = type.Name,
                              TicketPeriodId = period.Id,
                              TicketPeriod = period.Name,
                              TicketPrice = ticket.Price,
                              TicketIssuedBy = issuerPerson.FirstName + " " + issuerPerson.LastName,
                              TicketStatusName = status.Name,
                              IsActive = ticket.IsActive
                          };

            return tickets;
        }

        private IQueryable<RecreationalFishingTicketApplicationDTO> GetParametersFilteredTickets(RecreationalFishingTicketApplicationFilters filters, int? associationId = null)
        {
            var query = from ticket in Db.FishingTickets
                        select ticket;

            if (associationId.HasValue)
            {
                query = from ticket in query
                        where ticket.CreatedByFishingAssociationId == associationId.Value
                        select ticket;
            }

            var tickets = from ticket in query
                          join status in Db.NticketStatuses on ticket.TicketStatusId equals status.Id
                          join type in Db.NticketTypes on ticket.TicketTypeId equals type.Id
                          join period in Db.NticketPeriods on ticket.TicketPeriodId equals period.Id
                          join person in Db.Persons on ticket.PersonId equals person.Id
                          join issuerUser in Db.Users on ticket.CreatedByUserId equals issuerUser.Id
                          join issuerPerson in Db.Persons on issuerUser.PersonId equals issuerPerson.Id
                          where ticket.IsActive == !filters.ShowInactiveRecords
                          orderby ticket.Id descending
                          select new
                          {
                              ticket.Id,
                              ticket.ApplicationId,
                              TicketNum = ticket.IsOnlineTicket ? "E-" + ticket.Id : ticket.TicketNum,
                              TicketHolderName = person.FirstName + " " + person.LastName,
                              ticket.TicketValidFrom,
                              ticket.TicketValidTo,
                              TicketTypeId = type.Id,
                              TicketType = type.Name,
                              TicketPeriodId = period.Id,
                              TicketPeriod = period.Name,
                              TicketPrice = ticket.Price,
                              TicketIssuedBy = issuerPerson.FirstName + " " + issuerPerson.LastName,
                              TicketStatusName = status.Name,
                              ticket.IsActive,
                              TicketHolderEGN = person.EgnLnc,
                              IsDuplicate = ticket.DuplicateOfTicketId.HasValue,
                              ticket.CreatedByFishingAssociationId,
                              ticket.PersonId,
                              ticket.PersonRepresentativeId,
                              ticket.AssociationMemberId
                          };

            if (filters.PersonId.HasValue)
            {
                tickets = tickets.Where(ticket => ticket.PersonId == filters.PersonId || ticket.PersonRepresentativeId == filters.PersonId);
            }

            if (!string.IsNullOrEmpty(filters.TicketNum))
            {
                tickets = tickets.Where(x => x.TicketNum.ToLower().Contains(filters.TicketNum.ToLower()));
            }

            if (filters.TypeIds != null && filters.TypeIds.Count > 0)
            {
                tickets = tickets.Where(x => filters.TypeIds.Contains(x.TicketTypeId));
            }

            if (filters.PeriodIds != null && filters.PeriodIds.Count > 0)
            {
                tickets = tickets.Where(x => filters.PeriodIds.Contains(x.TicketPeriodId));
            }

            if (!string.IsNullOrEmpty(filters.TicketHolderEGN))
            {
                tickets = tickets.Where(x => x.TicketHolderEGN == filters.TicketHolderEGN);
            }

            if (filters.ValidFrom.HasValue)
            {
                tickets = tickets.Where(x => x.TicketValidFrom.Date == filters.ValidFrom.Value);
            }

            if (filters.ValidTo.HasValue)
            {
                tickets = tickets.Where(x => x.TicketValidTo.Date == filters.ValidTo.Value);
            }

            if (filters.IsDuplicate.HasValue)
            {
                tickets = tickets.Where(ticket => ticket.IsDuplicate == filters.IsDuplicate.Value);
            }

            if (filters.TerritoryUnitId.HasValue || (filters.StatusIds != null && filters.StatusIds.Count > 0) || (filters.ShowOnlyNotFinished.HasValue && filters.ShowOnlyNotFinished.Value))
            {
                var ticketsQuery = from appl in Db.Applications
                                   join ticket in Db.FishingTickets on appl.Id equals ticket.ApplicationId
                                   select new
                                   {
                                       ticket.Id,
                                       appl.TerritoryUnitId,
                                       appl.ApplicationStatusId
                                   };

                if (filters.TerritoryUnitId.HasValue)
                {
                    ticketsQuery = from ticket in ticketsQuery
                                   where ticket.TerritoryUnitId.HasValue && ticket.TerritoryUnitId.Value == filters.TerritoryUnitId.Value
                                   select ticket;
                }

                if (filters.StatusIds != null && filters.StatusIds.Count > 0)
                {
                    ticketsQuery = from ticket in ticketsQuery
                                   where filters.StatusIds.Contains(ticket.ApplicationStatusId)
                                   select ticket;
                }

                if (filters.ShowOnlyNotFinished.HasValue && filters.ShowOnlyNotFinished.Value)
                {
                    List<int> statusIds = (from applStatus in Db.NapplicationStatuses
                                           where applStatus.Code == nameof(ApplicationStatusesEnum.CONFIRMED_ISSUED_TICKET)
                                                || applStatus.Code == nameof(ApplicationStatusesEnum.CANCELED_APPL)
                                           select applStatus.Id).ToList();

                    ticketsQuery = from ticket in ticketsQuery
                                   where !statusIds.Contains(ticket.ApplicationStatusId)
                                   select ticket;
                }

                HashSet<int> ticketIds = (from ticket in ticketsQuery
                                          select ticket.Id).ToHashSet();

                tickets = tickets.Where(x => ticketIds.Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(filters.TicketHolderName))
            {
                tickets = tickets.Where(x => x.TicketHolderName.ToLower().Contains(filters.TicketHolderName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.TicketIssuerName))
            {
                tickets = tickets.Where(x => x.TicketIssuedBy.ToLower().Contains(filters.TicketIssuerName.ToLower()));
            }

            IQueryable<RecreationalFishingTicketApplicationDTO> result = from ticket in tickets
                                                                         select new RecreationalFishingTicketApplicationDTO
                                                                         {
                                                                             Id = ticket.Id,
                                                                             ApplicationId = ticket.ApplicationId,
                                                                             TicketNum = ticket.TicketNum,
                                                                             TicketHolderName = ticket.TicketHolderName,
                                                                             ValidFrom = ticket.TicketValidFrom,
                                                                             ValidTo = ticket.TicketValidTo,
                                                                             TicketTypeId = ticket.TicketTypeId,
                                                                             TicketType = ticket.TicketType,
                                                                             TicketPeriodId = ticket.TicketPeriodId,
                                                                             TicketPeriod = ticket.TicketPeriod,
                                                                             TicketPrice = ticket.TicketPrice,
                                                                             TicketIssuedBy = ticket.TicketIssuedBy,
                                                                             TicketStatusName = ticket.TicketStatusName,
                                                                             IsActive = ticket.IsActive
                                                                         };

            return result;
        }

        private IQueryable<RecreationalFishingTicketApplicationDTO> GetFreeTextFilteredTickets(string text, bool showInactive, int? associationId = null)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            var query = from ticket in Db.FishingTickets
                        select ticket;

            if (associationId.HasValue)
            {
                query = from ticket in query
                        where ticket.CreatedByFishingAssociationId == associationId.Value
                        select ticket;
            }

            var tickets = from ticket in query
                          join status in Db.NticketStatuses on ticket.TicketStatusId equals status.Id
                          join type in Db.NticketTypes on ticket.TicketTypeId equals type.Id
                          join period in Db.NticketPeriods on ticket.TicketPeriodId equals period.Id
                          join person in Db.Persons on ticket.PersonId equals person.Id
                          join issuerUser in Db.Users on ticket.CreatedByUserId equals issuerUser.Id
                          join issuerPerson in Db.Persons on issuerUser.PersonId equals issuerPerson.Id
                          where ticket.IsActive == !showInactive
                          orderby ticket.Id descending
                          select new RecreationalFishingTicketApplicationDTO
                          {
                              Id = ticket.Id,
                              ApplicationId = ticket.ApplicationId,
                              TicketNum = ticket.IsOnlineTicket ? "E-" + ticket.Id : ticket.TicketNum,
                              TicketHolderName = person.FirstName + " " + person.LastName,
                              ValidFrom = ticket.TicketValidFrom,
                              ValidTo = ticket.TicketValidTo,
                              TicketTypeId = type.Id,
                              TicketType = type.Name,
                              TicketPeriodId = period.Id,
                              TicketPeriod = period.Name,
                              TicketPrice = ticket.Price,
                              TicketIssuedBy = issuerPerson.FirstName + " " + issuerPerson.LastName,
                              TicketStatusName = status.Name,
                              IsActive = ticket.IsActive
                          };

            tickets = from ticket in tickets
                      where (ticket.TicketNum.ToLower().Contains(text)
                            || ticket.TicketHolderName.ToLower().Contains(text)
                            || (searchDate.HasValue && (ticket.ValidFrom.Date == searchDate.Value.Date
                                                    || ticket.ValidTo.Date == searchDate.Value.Date))
                            || ticket.TicketType.ToLower().Contains(text)
                            || ticket.TicketPeriod.ToLower().Contains(text)
                            || ticket.TicketPrice.ToString().Contains(text)
                            || ticket.TicketIssuedBy.ToLower().Contains(text)
                            || ticket.TicketStatusName.ToLower().Contains(text))
                      select ticket;

            return tickets;
        }

        private TicketApplicationDataIds GetRegixDataIdsByApplicationId(int applicationId)
        {
            var data = (from ticket in Db.FishingTickets
                        join application in Db.Applications on ticket.ApplicationId equals application.Id
                        join applType in Db.NapplicationTypes on application.ApplicationTypeId equals applType.Id
                        where application.Id == applicationId
                        select new TicketApplicationDataIds
                        {
                            Id = ticket.Id,
                            PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                            ApplicationId = ticket.ApplicationId,
                            PersonId = ticket.PersonId,
                            PersonRepresentativeId = ticket.PersonRepresentativeId,
                            TelkIsIndefinite = ticket.TelkisIndefinite ?? false,
                            TelkNumber = ticket.Telknum,
                            TelkValidTo = ticket.TelkvalidTo
                        }).First();

            return data;
        }

        private List<RecreationalFishingTicketDuplicateTableDTO> GetTicketDuplicates(int ticketId)
        {
            var result = (from ticket in Db.FishingTickets
                          where ticket.DuplicateOfTicketId.HasValue
                            && ticket.DuplicateOfTicketId.Value == ticketId
                          orderby ticket.CreatedOn descending
                          select new RecreationalFishingTicketDuplicateTableDTO
                          {
                              TicketNum = ticket.TicketNum,
                              IssueDate = ticket.CreatedOn,
                              Price = ticket.Price,
                              IsActive = ticket.IsActive
                          }).ToList();

            return result;
        }

        private Application AddApplication(int applicationTypeId, int currentUserId, RecreationalFishingTicketDTO ticket, bool isOnline)
        {
            Application application = Db.AddApplication(applicationTypeId,
                                                        ApplicationHierarchyTypesEnum.RecreationalFishingTicket,
                                                        currentUserId,
                                                        new ApplicationSubmissionDTO
                                                        {
                                                            SubmittedByPerson = ticket.RepresentativePerson ?? ticket.Person,
                                                            SubmittedByPersonAddresses = ticket.RepresentativePerson != null
                                                                                            ? ticket.RepresentativePersonAddressRegistrations
                                                                                            : ticket.PersonAddressRegistrations,
                                                            SubmittedForPerson = ticket.Person,
                                                            SubmittedForPersonPhoto = ticket.PersonPhoto,
                                                            SubmittedForAddresses = ticket.PersonAddressRegistrations
                                                        });

            if (!isOnline)
            {
                application.TerritoryUnitId = (from userInfo in Db.UserInfos
                                               where userInfo.UserId == currentUserId
                                               select userInfo.TerritoryUnitId).First();
            }

            Db.SaveChanges();

            application.AccessCode = ApplicationHelper.GenerateAccessCode(application.Id);

            List<FileInfoDTO> ticketFiles = ticket.Files;
            FileInfoDTO declarationFile = ticket.DeclarationFile;
            ticket.Files = null;
            ticket.DeclarationFile = null;

            stateMachine.Act(id: application.Id, draftContent: CommonUtils.Serialize(ticket), files: ticketFiles);

            ticket.Files = ticketFiles;
            ticket.DeclarationFile = declarationFile;
            return application;
        }

        private (int?, int, TicketTypeEnum) AddRequestedTicketEntry(RecreationalFishingTicketDTO ticket,
                                                                    Application application,
                                                                    PaymentDataDTO paymentData,
                                                                    int currentUserId,
                                                                    bool isOnline,
                                                                    int? associationId)
        {
            DateTime now = DateTime.Now;
            TicketTypeEnum type = Enum.Parse<TicketTypeEnum>(Db.NticketTypes.Where(x => x.Id == ticket.TypeId).Select(x => x.Code).First());
            TicketPeriodEnum period = Enum.Parse<TicketPeriodEnum>(Db.NticketPeriods.Where(x => x.Id == ticket.PeriodId).Select(x => x.Code).First());

            FishingTicket newTicket = new FishingTicket
            {
                IsOnlineTicket = isOnline,
                Application = application,
                TicketStatusId = GetTicketStatusId(TicketStatusEnum.REQUESTED),
                TicketTypeId = ticket.TypeId.Value,
                TicketPeriodId = ticket.PeriodId.Value,
                Price = ticket.Price.Value,
                TicketValidFrom = ticket.ValidFrom.Value,
                TicketValidTo = CalculateTicketValidToDate(type, period, ticket.ValidFrom.Value, ticket.Person.BirthDate, ticket.TelkData?.IsIndefinite == true ? null : ticket.TelkData?.ValidTo),
                CreatedByUserId = currentUserId,
                Comment = ticket.Comment,
                TelkisIndefinite = ticket.TelkData?.IsIndefinite,
                Telknum = ticket.TelkData?.Num,
                TelkvalidTo = ticket.TelkData?.ValidTo,
                CreatedByFishingAssociationId = associationId,
                HasUserConfirmed = ticket.HasUserConfirmed.Value
            };

            if (!newTicket.IsOnlineTicket)
            {
                newTicket.TicketNum = ticket.TicketNum;
            }

            newTicket.PersonId = application.SubmittedForPersonId.Value;
            if (ticket.RepresentativePerson != null)
            {
                newTicket.PersonRepresentativeId = application.SubmittedByPersonId.Value;
            }

            if (type == TicketTypeEnum.ASSOCIATION
                || type == TicketTypeEnum.BETWEEN14AND18ASSOCIATION
                || type == TicketTypeEnum.ELDERASSOCIATION)
            {
                newTicket.AssociationMember = AddOrEditAssociationMember(newTicket.PersonId, ticket.MembershipCard);
            }

            if (ticket.Files != null)
            {
                foreach (FileInfoDTO file in ticket.Files)
                {
                    Db.AddOrEditFile(newTicket, newTicket.FishingTicketFiles, file);
                }
            }

            if (!newTicket.IsOnlineTicket && type != TicketTypeEnum.UNDER14)
            {
                ticket.DeclarationFile.FileTypeId = (from fileType in Db.NfileTypes
                                                     where fileType.Code == nameof(FileTypeEnum.TICKET_DECLARATION)
                                                     select fileType.Id).First();

                Db.AddOrEditFile(newTicket, newTicket.FishingTicketFiles, ticket.DeclarationFile);
            }

            Db.FishingTickets.Add(newTicket);

            var selectedTariffInfo = (from applTariff in Db.NapplicationTypeTariffs
                                      join tariff in Db.Ntariffs on applTariff.TariffId equals tariff.Id
                                      where applTariff.ApplicationTypeId == application.ApplicationTypeId
                                            && applTariff.ValidFrom <= now
                                            && applTariff.ValidTo > now
                                            && tariff.Code == $"Ticket_{type.ToString().ToLower()}_{period.ToString().ToLower()}"
                                      select new
                                      {
                                          Id = tariff.Id,
                                          Price = tariff.Price
                                      }).FirstOrDefault();

            if (selectedTariffInfo == default)
            {
                throw new ArgumentException($"Can't find tariff for type: {type} and period: {period}!");
            }

            ApplicationPayment payment = Db.AddOrEditApplicationPayment(application,
                                                                        new ApplicationPaymentDTO
                                                                        {
                                                                            PaymentStatus = applicationService.CalculatePaymentStatus(null, selectedTariffInfo.Price)
                                                                        },
                                                                        null);
            Db.AddOrEditApplicationPaymentTariff(payment,
                                                 new PaymentTariffDTO
                                                 {
                                                     Quantity = 1,
                                                     TariffId = selectedTariffInfo.Id,
                                                     UnitPrice = selectedTariffInfo.Price
                                                 },
                                                 null);

            Db.SaveChanges();

            int unpaidPaymentStatusId = (from st in Db.NPaymentStatuses
                                         where st.Code == nameof(PaymentStatusesEnum.Unpaid)
                                            && st.ValidFrom <= now
                                            && st.ValidTo > now
                                         select st.Id).First();

            if (application.PaymentStatusId == unpaidPaymentStatusId && !newTicket.IsOnlineTicket)
            {
                applicationService.EnterOfflineTicketApplicationPaymentData(application.Id, paymentData);
            }

            PaymentStatusesEnum paymentStatusCode = (from payStatus in Db.NPaymentStatuses
                                                     where payStatus.Id == payment.PaymentStatusId
                                                     select Enum.Parse<PaymentStatusesEnum>(payStatus.Code)).First();

            if (paymentStatusCode == PaymentStatusesEnum.NotNeeded)
            {
                stateMachine.Act(newTicket.ApplicationId, ApplicationStatusesEnum.EXT_CHK_STARTED);
                return (null, newTicket.Id, type);
            }

            stateMachine.Act(newTicket.ApplicationId);
            return (application.Id, newTicket.Id, type);
        }

        private FishingAssociationMember AddOrEditAssociationMember(int personId, RecreationalFishingMembershipCardDTO membershipCard)
        {
            FishingAssociationMember member = (from assocMem in Db.FishingAssociationMembers
                                               where assocMem.PersonId == personId
                                                    && assocMem.FishingAssociationId == membershipCard.AssociationId.Value
                                               select assocMem).SingleOrDefault();

            if (member != null)
            {
                member.IsActive = true;
                member.MembershipCardNum = membershipCard.CardNum;
                member.MembershipFromDate = membershipCard.IssueDate.Value;
            }
            else
            {
                member = new FishingAssociationMember
                {
                    PersonId = personId,
                    FishingAssociationId = membershipCard.AssociationId.Value,
                    MembershipCardNum = membershipCard.CardNum,
                    MembershipFromDate = membershipCard.IssueDate.Value
                };

                Db.FishingAssociationMembers.Add(member);
            }
            return member;
        }

        private Application AddTicketDuplicateApplication(RecreationalFishingTicketDuplicateDTO data, int currentUserId)
        {
            DateTime now = DateTime.Now;

            Application oldApplication = (from ticket in Db.FishingTickets
                                          join appl in Db.Applications on ticket.ApplicationId equals appl.Id
                                          where ticket.Id == data.TicketId.Value
                                          select appl).First();

            int applicationTypeId = (from type in Db.NapplicationTypes
                                     where type.PageCode == nameof(PageCodeEnum.RecFishDup)
                                         && type.ValidFrom <= now
                                         && type.ValidTo > now
                                     select type.Id).First();

            int applicationStatusHierTypeId = (from hier in Db.NapplicationStatusHierarchyTypes
                                               where hier.Code == nameof(ApplicationHierarchyTypesEnum.RecreationalFishingTicket)
                                                    && hier.ValidFrom <= now
                                                    && hier.ValidTo > now
                                               select hier.Id).First();

            int applicationStatusId = (from status in Db.NapplicationStatuses
                                       where status.Code == nameof(ApplicationStatusesEnum.WAIT_PAYMENT_DATA)
                                            && status.ValidFrom <= now
                                            && status.ValidTo > now
                                       select status.Id).First();

            int paymentStatusId = (from status in Db.NPaymentStatuses
                                   where status.Code == nameof(PaymentStatusesEnum.Unpaid)
                                        && status.ValidFrom <= now
                                        && status.ValidTo > now
                                   select status.Id).First();

            Application application = new Application
            {
                ApplicationTypeId = applicationTypeId,
                ApplicationStatusHierTypeId = applicationStatusHierTypeId,
                ApplicationStatusId = applicationStatusId,
                PaymentStatusId = paymentStatusId,
                SubmitDateTime = now,
                SubmittedByUserId = currentUserId,
                SubmittedByPersonId = oldApplication.SubmittedByPersonId,
                SubmittedForPersonId = oldApplication.SubmittedForPersonId
            };

            Db.Applications.Add(application);
            Db.AddApplicationChangeHistory(application, now);

            var tariffData = (from applTariff in Db.NapplicationTypeTariffs
                              join tariff in Db.Ntariffs on applTariff.TariffId equals tariff.Id
                              where applTariff.ApplicationTypeId == application.ApplicationTypeId
                                    && applTariff.ValidFrom <= now
                                    && applTariff.ValidTo > now
                                    && tariff.Code == nameof(TariffCodesEnum.Ticket_duplicate)
                              select new
                              {
                                  tariff.Id,
                                  tariff.Price
                              }).First();

            ApplicationPayment payment = Db.AddOrEditApplicationPayment(application,
                                                                        new ApplicationPaymentDTO
                                                                        {
                                                                            PaymentStatus = PaymentStatusesEnum.Unpaid
                                                                        },
                                                                        null);
            Db.AddOrEditApplicationPaymentTariff(payment,
                                                 new PaymentTariffDTO
                                                 {
                                                     Quantity = 1,
                                                     TariffId = tariffData.Id,
                                                     UnitPrice = tariffData.Price
                                                 },
                                                 null);

            return application;
        }

        private FishingTicket AddTicketDuplicateEntry(RecreationalFishingTicketDuplicateDTO data, Application application, FishingTicket oldTicket, int currentUserId)
        {
            FishingTicket applEntry = new FishingTicket
            {
                Application = application,
                DuplicateOfTicketId = oldTicket.Id,
                PersonId = oldTicket.PersonId,
                PersonRepresentativeId = oldTicket.PersonRepresentativeId,
                TicketNum = data.TicketNum,
                TicketStatusId = GetTicketStatusId(TicketStatusEnum.REQUESTED),
                TicketTypeId = oldTicket.TicketTypeId,
                TicketPeriodId = oldTicket.TicketPeriodId,
                Price = data.Price.Value,
                TicketValidFrom = oldTicket.TicketValidFrom,
                TicketValidTo = oldTicket.TicketValidTo,
                CreatedByUserId = currentUserId,
                Comment = oldTicket.Comment,
                TelkisIndefinite = oldTicket.TelkisIndefinite,
                Telknum = oldTicket.Telknum,
                TelkvalidTo = oldTicket.TelkvalidTo,
                AssociationMemberId = oldTicket.AssociationMemberId,
                CreatedByFishingAssociationId = data.CreatedByAssociationId,
                IsOnlineTicket = false,
                HasUserConfirmed = oldTicket.HasUserConfirmed
            };

            var ticketFiles = (from ticketFile in Db.FishingTicketFiles
                               where ticketFile.RecordId == oldTicket.Id
                                  && ticketFile.IsActive
                               select new { ticketFile.FileId, ticketFile.FileTypeId }).ToList();

            foreach (var ticketFile in ticketFiles)
            {
                FishingTicketFile file = new FishingTicketFile
                {
                    Record = applEntry,
                    FileId = ticketFile.FileId,
                    FileTypeId = ticketFile.FileTypeId,
                };

                applEntry.FishingTicketFiles.Add(file);
            }

            Db.FishingTickets.Add(applEntry);

            return applEntry;
        }

        private void AddTicketDuplicatePaymentData(FishingTicket ticket, Application application, PaymentDataDTO payment)
        {
            applicationService.EnterOfflineTicketApplicationPaymentData(ticket.ApplicationId, payment);

            application.PaymentStatusId = Db.NPaymentStatuses.Where(x => x.Code == nameof(PaymentStatusesEnum.PaidOK)).Select(x => x.Id).First();

            ticket.TicketStatusId = GetTicketStatusId(TicketStatusEnum.APPROVED);
        }

        private FishingTicket EditTicketRegixFields(RecreationalFishingTicketBaseRegixDataDTO ticket, FileInfoDTO photo = null)
        {
            FishingTicket dbTicket = (from tick in Db.FishingTickets
                                            .AsSplitQuery()
                                            .Include(x => x.FishingTicketFiles)
                                      where tick.Id == ticket.Id
                                      select tick).First();

            dbTicket.Person = Db.AddOrEditPerson(ticket.Person, ticket.PersonAddressRegistrations, dbTicket.PersonId, photo);

            if (dbTicket.PersonRepresentativeId.HasValue)
            {
                dbTicket.PersonRepresentative = Db.AddOrEditPerson(ticket.RepresentativePerson, ticket.RepresentativePersonAddressRegistrations, dbTicket.PersonRepresentativeId);
            }

            dbTicket.TelkisIndefinite = ticket.TelkData?.IsIndefinite;
            dbTicket.Telknum = ticket.TelkData?.Num;
            dbTicket.TelkvalidTo = ticket.TelkData?.ValidTo;

            return dbTicket;
        }

        private int GetRecFishingApplicationTypeId()
        {
            DateTime now = DateTime.Now;

            int result = (from type in Db.NapplicationTypes
                          where type.PageCode == nameof(PageCodeEnum.RecFish)
                               && type.ValidFrom <= now && type.ValidTo > now
                          select type.Id).First();

            return result;
        }

        private static DateTime CalculateTicketValidToDate(TicketTypeEnum type, TicketPeriodEnum period, DateTime validFrom, DateTime? birthDate = null, DateTime? telkValidTo = null)
        {
            return period switch
            {
                TicketPeriodEnum.ANNUAL => validFrom.AddYears(1).Date.AddSeconds(-1),
                TicketPeriodEnum.HALFYEARLY => validFrom.AddMonths(6).Date.AddSeconds(-1),
                TicketPeriodEnum.MONTHLY => validFrom.AddMonths(1).Date.AddSeconds(-1),
                TicketPeriodEnum.WEEKLY => validFrom.AddDays(7).Date.AddSeconds(-1),
                TicketPeriodEnum.UNTIL14 => birthDate.HasValue ? birthDate.Value.AddYears(14).Date.AddSeconds(-1) : DefaultConstants.MAX_VALID_DATE,
                TicketPeriodEnum.DISABILITY => telkValidTo.HasValue ? telkValidTo.Value.AddDays(1).Date.AddSeconds(-1) : DefaultConstants.MAX_VALID_DATE,
                TicketPeriodEnum.NOPERIOD => DefaultConstants.MAX_VALID_DATE,
                _ => throw new ArgumentException($"Invalid ticket period {period} for type {type}"),
            };
        }

        private static int CalculatedPercentOfPeriodCompleted(DateTime validFrom, DateTime validTo)
        {
            DateTime now = DateTime.Now;

            double totalDays = (validTo - validFrom).TotalDays;
            double daysPassed = (now - validFrom).TotalDays;

            int result = (int)(daysPassed / totalDays * 100);
            return result;
        }

        private int GetTicketStatusId(TicketStatusEnum status)
        {
            int id = (from st in Db.NticketStatuses
                      where st.Code == status.ToString()
                      select st.Id).First();

            return id;
        }
    }
}
