using IARA.Mobile.Application.DTObjects.Nomenclatures;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class FishingGearPingerDto
    {
        public int? Id { get; set; }
        public string Number { get; set; }
        public int StatusId { get; set; }
        public NomenclatureDto SelectedStatus { get; set; }
    }
}
