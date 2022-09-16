namespace IARA.DomainModels.DTOModels.CrossChecks
{
    public class ExecuteCrossCheckDTO
    {
        public string ReportName { get; set; }
        public int ReportId { get; set; }
        public string ReportSql { get; set; }
        public int CheckId { get; set; }
        public string CheckName { get; set; }
    }
}
