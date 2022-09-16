using IARA.Mobile.Application.DTObjects.Nomenclatures;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class PatrolVehicleDto : SelectNomenclatureDto
    {
        public string CallSign { get; set; }
        public int? InstitutionId { get; set; }
        public int? FlagId { get; set; }
        public int? PatrolVehicleTypeId { get; set; }
        public string ExternalMark { get; set; }
    }
}
