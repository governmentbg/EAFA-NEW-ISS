using IARA.Mobile.Application;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.LocalDb;
using IARA.Mobile.Pub.Application.Interfaces.Database;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Application.Transactions.Base;
using IARA.Mobile.Pub.Domain.Entities.FishingTicket;
using IARA.Mobile.Pub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class FishingTicketsTransaction : BaseTransaction, IFishingTicketsTransaction
    {
        private readonly IFishingTicketsSettings fishingTicketsSettings;
        private const int DEFAULT_TICKETS_SHOW_COUNT = 3;

        public FishingTicketsTransaction(IFishingTicketsSettings fishingTicketsSettings, BaseTransactionProvider provider) : base(provider)
        {
            this.fishingTicketsSettings = fishingTicketsSettings;
        }

        public int GetTicketPeriodIdByCode(string periodCode)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NTicketPeriods
                    .Where(x => x.Code == periodCode && x.IsActive).Select(x => x.Id).First();
            }
        }

        public int GetTicketTypeIdByCode(string typeCode)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NTicketTypes
                    .Where(x => x.Code == typeCode && x.IsActive).Select(x => x.Id).First();
            }
        }

        public string GetTicketTypeCodeById(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NTicketTypes
                    .Where(x => x.Id == id && x.IsActive).Select(x => x.Code).First();
            }
        }

        public List<TicketPeriodDto> GetTicketPeriods()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NTicketPeriods
                    .Where(x => x.IsActive)
                    .Select(f => new TicketPeriodDto
                    {
                        Id = f.Id,
                        Code = f.Code,
                        Name = f.Name
                    })
                    .ToList();
            }
        }

        public List<TicketTariffDto> GetTicketTariffsByTicketType(string ticketType)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                string type = $"Ticket_{ticketType.ToLower()}_";
                return context.NTicketTariffs
                    .Where(x => x.Code.StartsWith(type) && x.IsActive)
                    .Select(f => new TicketTariffDto
                    {
                        Id = f.Id,
                        Code = f.Code,
                        Name = f.Name,
                        Price = f.Price
                    }).ToList();
            }
        }

        public decimal GetTicketTariff(string ticketType, string ticketPeriod)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                string type = $"Ticket_{ticketType.ToLower()}_{ticketPeriod.ToLower()}";
                return context.NTicketTariffs
                    .Where(x => x.Code == type && x.IsActive)
                    .Select(f => f.Price).First();
            }
        }

        public async Task<List<TicketTypeDto>> GetAllowedTicketTypes()
        {
            fishingTicketsSettings.AllowedUnder14TicketsCount =
                await RestClient.GetAsync<int>("FishingTickets/AllowedUnder14TicketsCount").GetHttpContent();

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                IEnumerable<TicketTypeDto> result = context.NTicketTypes
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.OrderNo)
                    .Select(x => new TicketTypeDto
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name
                    });

                if (fishingTicketsSettings.AllowedUnder14TicketsCount > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return result.Where(x => x.Code != nameof(TicketTypeEnum.UNDER14)).ToList();
                }
            }
        }

        public List<TicketTypeDto> GetTicketTypes()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NTicketTypes
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.OrderNo)
                    .Select(x => new TicketTypeDto
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name
                    }).ToList();
            }
        }

        public Task<List<AssociationSelectDto>> GetFishingAssiciations()
        {
            return RestClient.GetAsync<List<AssociationSelectDto>>(
                "FishingTickets/GetAllApprovedFishingAssociations"
            ).GetHttpContent();
        }

        public async Task<DateTime?> CalculateTicketValidToDate(TicketValidToCalculationParamsDto filter)
        {
            HttpResult<TicketValidToResponseDto> result = await RestClient.PostAsync<TicketValidToResponseDto>("FishingTickets/CalculateTicketValidToDate", filter);

            if (result.IsSuccessful)
            {
                return result.Content.ValidTo;
            }

            return null;
        }

        public async Task<AddTicketResponseDto> AddTickets(List<FishingTicketRequestDto> tickets, bool updateProfileData)
        {
            HttpResult<AddTicketResponseDto> result = await RestClient.PostAsFormDataAsync<AddTicketResponseDto>("FishingTickets/AddTickets", new { tickets, updateProfileData, hasPaymentData = false });

            if (result.IsSuccessful)
            {
                return result.Content;
            }

            return null;
        }

        public Task<bool> EditTicket(FishingTicketRequestDto ticket)
        {
            return RestClient.PostAsFormDataAsync("FishingTickets/EditTicket", ticket)
                .IsSuccessfulResult();
        }

        public Task<bool> IsDuplicateTicket(TicketValidationDTO ticket)
        {
            return RestClient.PostAsync<bool>("FishingTickets/IsDuplicateTicket", ticket)
                .GetHttpContent();
        }

        public async Task<List<FishingTicketDto>> GetMyTickets()
        {
            if (CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected)
            {
                return GetLocalStoredTickets();
            }

            HttpResult<List<FishingTicketDto>> result = await RestClient.GetAsync<List<FishingTicketDto>>("FishingTickets/GetTickets");

            if (!result.IsSuccessful)
            {
                return GetLocalStoredTickets();
            }
            else if (result.IsSuccessful && result.Content != null && result.Content.Count > 0)
            {
                List<FishingTicketDto> tickets = result.Content;

                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    context.FishingTickets.Clear();
                    context.FishingTickets.AddRange(Mapper.Map<List<FishingTicket>>(tickets));
                }

                return tickets.Where(x => DateTime.Now < x.ValidTo).ToList();
            }

            return null;
        }

        public List<FishingTicketDto> GetLocalStoredTickets()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                DateTime now = DateTime.Now;
                return (from ticket in context.FishingTickets
                        where now < ticket.ValidTo
                        orderby ticket.ValidFrom descending
                        select new FishingTicketDto
                        {
                            Id = ticket.Id,
                            ApplicationId = ticket.ApplicationId,
                            ApplicationStatusCode = ticket.ApplicationStatusCode,
                            ApplicationStatusReason = ticket.ApplicationStatusReason,
                            PaymentStatus = ticket.PaymentStatus,
                            PeriodCode = ticket.PeriodCode,
                            PeriodName = ticket.PeriodName,
                            PeriodId = ticket.PeriodId,
                            PersonFullName = ticket.PersonFullName,
                            Price = ticket.Price,
                            StatusCode = ticket.StatusCode,
                            StatusName = ticket.StatusName,
                            Type = ticket.Type,
                            TypeId = ticket.TypeId,
                            TypeName = ticket.TypeName,
                            ValidTo = ticket.ValidTo,
                            ValidFrom = ticket.ValidFrom,
                            TicketNumber = ticket.TicketNumber,
                            PaymentRequestNum = ticket.PaymentRequestNum
                        }).ToList();
            }
        }

        public List<FishingTicketDto> GetLocalStoredExpiredTickets()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                DateTime now = DateTime.Now;
                return (from ticket in context.FishingTickets
                        where now >= ticket.ValidTo
                        orderby ticket.ValidFrom descending
                        select new FishingTicketDto
                        {
                            Id = ticket.Id,
                            ApplicationId = ticket.ApplicationId,
                            ApplicationStatusCode = ticket.ApplicationStatusCode,
                            ApplicationStatusReason = ticket.ApplicationStatusReason,
                            PaymentStatus = ticket.PaymentStatus,
                            PeriodCode = ticket.PeriodCode,
                            PeriodName = ticket.PeriodName,
                            PeriodId = ticket.PeriodId,
                            PersonFullName = ticket.PersonFullName,
                            Price = ticket.Price,
                            StatusCode = ticket.StatusCode,
                            StatusName = ticket.StatusName,
                            Type = ticket.Type,
                            TypeId = ticket.TypeId,
                            TypeName = ticket.TypeName,
                            ValidTo = ticket.ValidTo,
                            ValidFrom = ticket.ValidFrom,
                            TicketNumber = ticket.TicketNumber,
                            PaymentRequestNum = ticket.PaymentRequestNum,
                        }).ToList();
            }
        }

        public async Task<List<FishingTicketDto>> GetHomePageTickets()
        {
            if (CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected)
            {
                return GetLocalStoredHomePageTickets();
            }

            HttpResult<List<FishingTicketDto>> result = await RestClient.GetAsync<List<FishingTicketDto>>("FishingTickets/GetTickets");

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return null;
            }
            else if (!result.IsSuccessful)
            {
                return GetLocalStoredHomePageTickets();
            }
            else if (result.Content != null && result.Content.Count > 0)
            {
                List<FishingTicketDto> tickets = result.Content;

                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    context.FishingTickets.Clear();
                    context.FishingTickets.AddRange(Mapper.Map<List<FishingTicket>>(tickets));
                }

                if (tickets.Count > DEFAULT_TICKETS_SHOW_COUNT)
                {
                    return tickets.Take(DEFAULT_TICKETS_SHOW_COUNT).ToList();
                }

                return tickets;
            }

            return null;
        }

        public TicketsHomePageDto GetTicketsHomePageMetadata()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                int ticketsCount = context.FishingTickets.Count();

                return new TicketsHomePageDto
                {
                    HasMore = ticketsCount > DEFAULT_TICKETS_SHOW_COUNT,
                    TotalCount = ticketsCount
                };
            }
        }

        public Task<FishingTicketResponseDto> GetTicketById(int id)
        {
            return RestClient.GetAsync<FishingTicketResponseDto>("FishingTickets/GetTicket", new { id })
                .GetHttpContent();
        }

        public async Task<FileResponse> GetPersonPhoto(int photoId, int ticketId, bool isRepresentative = false)
        {
            HttpResult<FileResponse> result = await RestClient.GetAsync<FileResponse>("FishingTickets/GetUserPhoto", new { photoId, ticketId, isRepresentative });

            if (result.IsSuccessful && result.Content != null)
            {
                return result.Content;
            }

            return null;
        }

        public List<UserTicketShortDto> GetListOfActiveTickets()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                DateTime now = DateTimeProvider.Now;

                return (from ticket in context.FishingTickets
                        where ticket.ValidFrom <= now && now <= ticket.ValidTo
                            && (ticket.StatusCode == nameof(TicketStatusEnum.ISSUED) ||
                                    ticket.StatusCode == nameof(TicketStatusEnum.APPROVED) ||
                                    ticket.StatusCode == nameof(TicketStatusEnum.REQUESTED))
                            && (ticket.PaymentStatus == PaymentStatusEnum.PaidOK ||
                                    ticket.PaymentStatus == PaymentStatusEnum.NotNeeded)
                        select new UserTicketShortDto
                        {
                            Id = ticket.Id,
                            PersonFullName = ticket.PersonFullName,
                            TypeName = ticket.TypeName,
                            ValidTo = ticket.ValidTo,
                            ValidFrom = ticket.ValidFrom,
                            TicketNumber = ticket.TicketNumber
                        }).ToList();
            }
        }

        public int GetActiveTicketsCount()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                DateTime now = DateTimeProvider.Now;

                return (
                    from ticket in context.FishingTickets
                    where ticket.ValidFrom <= now && now <= ticket.ValidTo
                        && (ticket.StatusCode == nameof(TicketStatusEnum.ISSUED) ||
                                ticket.StatusCode == nameof(TicketStatusEnum.APPROVED) ||
                                ticket.StatusCode == nameof(TicketStatusEnum.REQUESTED))
                        && (ticket.PaymentStatus == PaymentStatusEnum.PaidOK ||
                                ticket.PaymentStatus == PaymentStatusEnum.NotNeeded)
                    select ticket.Id).Count();
            }
        }

        private List<FishingTicketDto> GetLocalStoredHomePageTickets()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (from ticket in context.FishingTickets
                        orderby ticket.ValidFrom descending
                        select new FishingTicketDto
                        {
                            Id = ticket.Id,
                            ApplicationId = ticket.ApplicationId,
                            ApplicationStatusCode = ticket.ApplicationStatusCode,
                            ApplicationStatusReason = ticket.ApplicationStatusReason,
                            PaymentStatus = ticket.PaymentStatus,
                            PeriodCode = ticket.PeriodCode,
                            PeriodName = ticket.PeriodName,
                            PeriodId = ticket.PeriodId,
                            PersonFullName = ticket.PersonFullName,
                            Price = ticket.Price,
                            StatusCode = ticket.StatusCode,
                            StatusName = ticket.StatusName,
                            Type = ticket.Type,
                            TypeId = ticket.TypeId,
                            TypeName = ticket.TypeName,
                            ValidTo = ticket.ValidTo,
                            ValidFrom = ticket.ValidFrom,
                            TicketNumber = ticket.TicketNumber,
                            PaymentRequestNum = ticket.PaymentRequestNum
                        }).Take(DEFAULT_TICKETS_SHOW_COUNT).ToList();
            }
        }

        public Task UpdateUserProfileData(UpdateTicketProfileDataDto data)
        {
            return RestClient.PostAsFormDataAsync<AddTicketResponseDto>("FishingTickets/UpdateUserProfileData", data);
        }
    }
}
