using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class LawMobileDto : NomenclatureDTO
    {
        public string Article { get; set; }
        public string Paragraph { get; set; }
        public string Section { get; set; }
        public string Letter { get; set; }
        public ViolatedRegulationSectionTypesEnum? SectionType { get; set; }
        public string Law { get; set; }
        public string LawText { get; set; }
    }
}
