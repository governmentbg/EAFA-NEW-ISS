namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class LogBookPageDto
    {
        public int Id { get; set; }
        public int LogBookId { get; set; }
        public string PageNum { get; set; }
        public string Status { get; set; }
        public string DisplayValue => PageNum + " (" + Status + ")";
    }
}
