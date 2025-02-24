using IARA.Mobile.Insp.Application.DTObjects.Inspections;

namespace IARA.Mobile.Insp.Models
{
    public class ViolatedRegulationModel : BaseAssignableModel<ViolatedRegulationModel>
    {
        public int? Id { get; set; }

        public string Article { get; set; }

        public string Paragraph { get; set; }

        public string Section { get; set; }

        public string Letter { get; set; }

        public int? LawSectionId { get; set; }

        public string LawText { get; set; }

        public string Comments { get; set; }
        public string Law { get; set; }

        public static implicit operator AuanViolatedRegulationDto(ViolatedRegulationModel model)
        {
            return new AuanViolatedRegulationDto
            {
                Id = model.Id,
                Article = model.Article,
                Paragraph = model.Paragraph,
                Section = model.Section,
                Letter = model.Letter,
                LawSectionId = model.LawSectionId,
                LawText = model.LawText,
                Comments = model.Comments,
                Law = model.Law
            };
        }
    }
}
