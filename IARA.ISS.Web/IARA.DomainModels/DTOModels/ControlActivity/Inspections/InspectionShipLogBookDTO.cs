using System;
using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionShipLogBookDTO
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime IssuedOn { get; set; }
        public long StartPage { get; set; }
        public long EndPage { get; set; }
        public List<NomenclatureDTO> Pages { get; set; }
    }
}
