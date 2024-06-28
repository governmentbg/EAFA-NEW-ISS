using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API
{
    public class LawApiDto : NomenclatureDto
    {
        public string Article { get; set; }
        public string Paragraph { get; set; }
        public string Section { get; set; }
        public string Letter { get; set; }
        public ViolatedRegulationSectionTypesEnum? SectionType { get; set; }
        public int LawSectionId { get; set; }
        public string LawSection { get; set; }
        public string LawText { get; set; }
        public string Comments { get; set; }
    }
}
