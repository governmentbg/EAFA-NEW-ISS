using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class FishingGearPingerDto
    {
        public int? Id { get; set; }
        public string Number { get; set; }
        public int StatusId { get; set; }
        public FishingGearPingerStatusesEnum SelectedStatus { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
    }
}
