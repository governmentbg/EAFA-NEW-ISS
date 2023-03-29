using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.LocalDb;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IFishingTicketsTransaction
    {
        int GetTicketPeriodIdByCode(string periodCode);

        int GetTicketTypeIdByCode(string typeCode);

        string GetTicketTypeCodeById(int id);

        Task<List<TicketTypeDto>> GetAllowedTicketTypes();

        List<TicketPeriodDto> GetTicketPeriods();

        List<TicketTariffDto> GetTicketTariffsByTicketType(string ticketType);

        decimal GetTicketTariff(string ticketType, string ticketPeriod);

        Task<List<AssociationSelectDto>> GetFishingAssiciations();

        Task<AddTicketResponseDto> AddTickets(List<FishingTicketRequestDto> tickets, bool updateProfileData);

        Task<bool> EditTicket(FishingTicketRequestDto ticket);

        Task<bool> IsDuplicateTicket(TicketValidationDTO ticket);

        Task<List<FishingTicketDto>> GetMyTickets();

        Task<List<FishingTicketDto>> GetHomePageTickets();

        Task<FishingTicketResponseDto> GetTicketById(int id);

        Task<FileResponse> GetPersonPhoto(int photoId, int ticketId, bool isRepresentative = false);

        TicketsHomePageDto GetTicketsHomePageMetadata();

        List<UserTicketShortDto> GetListOfActiveTickets();

        int GetActiveTicketsCount();

        Task<DateTime?> CalculateTicketValidToDate(TicketValidToCalculationParamsDto filter);

        Task UpdateUserProfileData(UpdateTicketProfileDataDto data);

        List<FishingTicketDto> GetLocalStoredExpiredTickets();

        List<FishingTicketDto> GetLocalStoredTickets();

        List<TicketTypeDto> GetTicketTypes();
    }
}
