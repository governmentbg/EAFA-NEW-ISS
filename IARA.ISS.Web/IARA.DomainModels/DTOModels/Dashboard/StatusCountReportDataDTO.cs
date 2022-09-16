using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.DTOModels.Dashboard
{
    public class StatusCountReportDataDTO
    {
        public List<StatusCountReportDTO> Series { get; set; }

        public List<string> Categories { get; set; }

    }
}
