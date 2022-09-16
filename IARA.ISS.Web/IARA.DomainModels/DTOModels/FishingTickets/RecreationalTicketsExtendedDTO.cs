using IARA.DomainModels.DTOModels.RecreationalFishing;

namespace IARA.DomainModels.DTOModels.FishingTickets
{
    public class RecreationalTicketsExtendedDTO : RecreationalFishingTicketsDTO
    {
        public bool UpdateProfileData { get; set; }
    }
}
