using IARA.Mobile.Application.DTObjects.Nomenclatures;
using System;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class PermitNomenclatureDto : SelectNomenclatureDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
