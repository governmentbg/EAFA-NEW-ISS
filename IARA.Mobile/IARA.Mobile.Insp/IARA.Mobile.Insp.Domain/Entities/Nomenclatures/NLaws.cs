using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Insp.Domain.Enums;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Nomenclatures
{
    public class NLaws : ICodeNomenclature
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public ViolatedRegulationSectionTypesEnum? SectionType { get; set; }
        public string Article { get; set; }
        public string Paragraph { get; set; }
        public string Section { get; set; }
        public string Letter { get; set; }
        public string LawText { get; set; }
        public int LawSectionId { get; set; }
        public string LawSection { get; set; }
        public string Comments { get; set; }
    }
}
