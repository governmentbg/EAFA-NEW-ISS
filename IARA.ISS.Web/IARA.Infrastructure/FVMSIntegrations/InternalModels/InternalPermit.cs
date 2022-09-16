using System;

namespace IARA.Infrastructure.FVMSIntegrations
{
    internal class InternalPermit
    {
        public string CFR { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Id { get; set; }
        public bool IsSuspended { get; set; }
        public string VesselName { get; set; }
        public string RegistrationNum { get; set; }
    }
}
