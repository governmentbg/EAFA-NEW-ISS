using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class ObservationToolNomenclatureDto : DescrSelectNomenclatureDto
    {
        public ObservationToolOnBoardEnum OnBoard { get; set; }
    }
}
