namespace IARA.DomainModels.DTOModels.FishingActivityReports
{
    public class FishingActivityReportPageDTO
    {
        public int Id { get; set; }

        public int ReportId { get; set; }

        public string PageNumber { get; set; }

        public string PageStatus { get; set; }

        public string GearName { get; set; }

        public string UnloadPort { get; set; }

        public string UnloadedFish { get; set; }

        public bool IsActive { get; set; }
    }
}
