using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.DTOModels.CatchQuotas
{
    public class UnloadPortDTO
    {
        public int? CatchQuotaId { get; set; }
        public int? PortId { get; set; }
        public bool? IsActive { get; set; }
    }
}
