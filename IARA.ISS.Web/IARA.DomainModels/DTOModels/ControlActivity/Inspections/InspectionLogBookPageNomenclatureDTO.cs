using System;
using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionLogBookPageNomenclatureDTO : NomenclatureDTO
    {
        public string OriginDeclarationNum { get; set; }

        public DateTime? OriginDeclarationDate { get; set; } //TODO name declaration date

        public decimal? LogPageNum { get; set; }

        public DateTime? LogBookPageDate { get; set; }

        public List<DeclarationLogBookPageFishDTO> LogBookProducts { get; set; }
    }
}
