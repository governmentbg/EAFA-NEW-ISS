using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingTickets;

namespace IARA.Interfaces
{
    public interface IFishingTicketService
    {
        DownloadableFileDTO GetPersonPhoto(int photoId, int ticketId, bool isRepresentative);

        List<FishingTicketDTO> GetBasePersonTicketsData(int currentUserId);

        int AllowedUnder14TicketsCount(int currentUserId);

        bool IsDuplicateTicket(TicketValidationDTO ticket);

        bool HasAccessToUpdateProfileData(int userId, EgnLncDTO EgnLnc);

        void UpdateUserProfileData(RegixPersonDataDTO personData, List<AddressRegistrationDTO> userAddresses, FileInfoDTO photo, int userId);

        void InactivatePastTickets();

        bool HasAccessToTicket(int userId, int ticketId);
    }
}
