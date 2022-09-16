using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Domain.Models
{
    public class ShipUserModel
    {
        public int Id { get; set; }
        public int ShipId { get; set; }
        public InspectedPersonType Type { get; set; }
    }
}
