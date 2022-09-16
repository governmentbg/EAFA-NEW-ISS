using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class PersonReportDTO
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string EGN { get; set; }

        public string PopulatedArea { get; set; }

        public string Street { get; set; }

        public string Number { get; set; }

        public List<ReportHistoryDTO> History { get; set; }
    }
}
