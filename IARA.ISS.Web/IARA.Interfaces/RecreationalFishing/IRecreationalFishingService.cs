using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IRecreationalFishingService : IService
    {
        RecreationalFishingTicketDTO GetTicket(int id);

        RegixChecksWrapperDTO<RecreationalFishingTicketBaseRegixDataDTO> GetTicketRegixData(int id);

        RecreationalFishingTicketBaseRegixDataDTO GetApplicationRegixData(int applicationId);

        IQueryable<RecreationalFishingTicketApplicationDTO> GetAllTickets(RecreationalFishingTicketApplicationFilters filters, int? associationId = null);

        void SetTicketsApplicationStatus(List<RecreationalFishingTicketApplicationDTO> tickets);

        List<RecreationalFishingTicketCardDTO> GetAllUserTickets(int userId);

        List<RecreationalFishingTicketViewDTO> GetRequestedOrActiveTickets(int userId);

        RecreationalFishingTicketHolderDTO GetTicketHolderData(int personId);

        RecreationalFishingAddTicketsResultDTO AddTickets(List<RecreationalFishingTicketDTO> tickets, PaymentDataDTO payment, int currentUserId, bool isOnline, int? associationId);

        int AddTicketDuplicate(RecreationalFishingTicketDuplicateDTO data, int currentUserId);

        ApplicationStatusesEnum EditTicket(RecreationalFishingTicketDTO ticket);

        ApplicationStatusesEnum EditTicketRegixData(RecreationalFishingTicketBaseRegixDataDTO ticket);

        void CancelTicket(int id, string reason);

        void DeleteTicket(int id);

        void UndoDeleteTicket(int id);

        DateTime CalculateTicketValidToDate(RecreationalFishingTicketValidToCalculationParamsDTO parameters);

        RecreationalFishingTicketValidationResultDTO CheckEgnLncPurchaseAbility(RecreationalFishingTicketValidationDTO data);

        List<bool> CheckTicketNumbersAvailability(List<string> ticketNumbers);

        void UpdateUserDataFromTicket(RecreationalFishingUserTicketDataDTO data, int userId);

        Task<Stream> DownloadFishingTicket(int ticketId);

        Task<Stream> DownloadTicketDeclaration(RecreationalFishingTicketDeclarationParametersDTO parameters, int currentUserId);

        bool HasUserAccessToTickets(int userId, params int[] ticketIds);

        bool HasUserAccessToTicketFile(int userId, int fileId);

        bool HasAssociationAccessToPersonData(int associationId, int personId);
    }
}
