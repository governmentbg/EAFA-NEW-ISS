using System;

namespace IARA.DomainModels.DTOModels.ControlActivity.AuanRegister
{
    public class AuanLawSectionDTO : AuanViolatedRegulationDTO
    {
        public int? LawId { get; set; }

        public string LawText { get; set; }

        public bool IsChecked { get; set; }
    }
}
