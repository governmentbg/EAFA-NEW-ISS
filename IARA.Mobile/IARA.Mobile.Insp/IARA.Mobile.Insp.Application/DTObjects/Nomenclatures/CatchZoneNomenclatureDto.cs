using System.Drawing;
using IARA.Mobile.Application.DTObjects.Nomenclatures;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class CatchZoneNomenclatureDto : SelectNomenclatureDto
    {
        public RectangleF Block { get; set; }
    }
}
