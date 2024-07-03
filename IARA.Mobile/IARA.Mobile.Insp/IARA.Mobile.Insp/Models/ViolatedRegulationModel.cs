namespace IARA.Mobile.Insp.Models
{
    public class ViolatedRegulationModel
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
    }
}
