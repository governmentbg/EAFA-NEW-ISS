using System;

namespace IARA.DomainModels.DTOModels.FLUXVMSRequests
{
    public class FluxFlapRequestDTO
    {
        public int Id { get; set; }

        public bool IsOutgoing { get; set; }

        public string Ship { get; set; }

        public string RequestUuid { get; set; }

        public DateTime RequestDate { get; set; }

        public string ResponseUuid { get; set; }

        public DateTime? ResponseDate { get; set; }
    }
}
