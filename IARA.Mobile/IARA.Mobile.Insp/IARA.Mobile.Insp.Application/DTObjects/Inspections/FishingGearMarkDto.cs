using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class FishingGearMarkDto
    {
        public int? Id { get; set; }
        public string Number { get; set; }
        public int StatusId { get; set; }
        public FishingGearMarkStatus SelectedStatus { get; set; }
    }
}
