using System;
using IARA.Mobile.Application.DTObjects.Nomenclatures;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class PermitNomenclatureDto : SelectNomenclatureDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
