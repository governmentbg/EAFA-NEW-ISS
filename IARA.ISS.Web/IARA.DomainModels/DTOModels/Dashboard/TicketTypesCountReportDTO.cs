using System;
using System.Collections.Generic;
using System.Text;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Dashboard
{
    public class TicketTypesCountReportDTO
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public TicketTypeEnum TicketTypeCode { get; set; }
        public int Count { get; set; }
    }
}
