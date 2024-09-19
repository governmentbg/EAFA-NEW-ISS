using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class AuanViolatedRegulationDto
    {
        public int? Id { get; set; }
        public string Article { get; set; }
        public string Paragraph { get; set; }
        public string Section { get; set; }
        public string Letter { get; set; }
        public ViolatedRegulationSectionTypesEnum? SectionType { get; set; }
        public int? LawSectionId { get; set; }
        public string Law { get; set; }
        public string LawText { get; set; }
        public string Comments { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}