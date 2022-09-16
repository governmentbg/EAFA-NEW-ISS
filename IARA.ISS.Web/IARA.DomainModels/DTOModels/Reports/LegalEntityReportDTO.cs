using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class LegalEntityReportDTO
    {
        public int Id { get; set; }

        public string LegalName { get; set; }

        public string Eik { get; set; }

        public string PopulatedArea { get; set; }

        public string Street { get; set; }

        public string Number { get; set; }

        public List<ReportHistoryDTO> History { get; set; }
    }
}
