using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Dashboard
{
    public class StatusCountReportDTO
    {
        public string Name { get; set; }

        public List<int> Data { get; set; }

        public string Color { get; set; }
    }
}

