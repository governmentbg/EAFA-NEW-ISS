using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API
{
    public class ObservationToolApiDto : NomenclatureDto
    {
        public ObservationToolOnBoardEnum OnBoard { get; set; }
    }
}
