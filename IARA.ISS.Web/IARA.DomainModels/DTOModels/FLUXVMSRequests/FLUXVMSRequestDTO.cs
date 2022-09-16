using System;

namespace IARA.DomainModels.DTOModels.FLUXVMSRequests
{
    public class FLUXVMSRequestDTO
    {
        public int Id { get; set; }

        public bool IsOutgoing { get; set; }

        public string WebServiceName { get; set; }

        public string RequestUUID { get; set; }

        public DateTime RequestDateTime { get; set; }

        public string ResponseStatus { get; set; }

        public string ResponseUUID { get; set; }

        public DateTime? ResponseDateTime { get; set; }

        public string ErrorDescription { get; set; }

        public bool IsActive { get; set; }
    }
}
