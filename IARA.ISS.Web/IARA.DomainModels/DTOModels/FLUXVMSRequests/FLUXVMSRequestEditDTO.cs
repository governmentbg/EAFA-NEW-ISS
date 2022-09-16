using System;

namespace IARA.DomainModels.DTOModels.FLUXVMSRequests
{
    public class FLUXVMSRequestEditDTO
    {
        public bool IsOutgoing { get; set; }

        public string DomainName { get; set; }

        public string WebServiceName { get; set; }

        public string RequestUUID { get; set; }

        public DateTime RequestDateTime { get; set; }

        public string RequestContent { get; set; }

        public string ResponseStatus { get; set; }

        public string ResponseUUID { get; set; }

        public DateTime? ResponseDateTime { get; set; }

        public string ResponseContent { get; set; }

        public string ErrorDescription { get; set; }
    }
}
