using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class ShipPersonnelDto : SelectNomenclatureDto
    {
        public int? EntryId { get; set; }
        public InspectedPersonType Type { get; set; }
    }
}
