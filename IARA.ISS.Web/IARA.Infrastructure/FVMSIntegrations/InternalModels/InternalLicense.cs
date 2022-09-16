using System;

namespace IARA.Infrastructure.FVMSIntegrations
{
    internal class InternalLicense
    {
        public string CFR { get; set; }
        public long CurrentPage { get; set; }
        public int Id { get; set; }
        public DateTime LicenceCreatedOn { get; set; }
        public string LicenceNumber { get; set; }
        public DateTime LicenceValidFrom { get; set; }
        public DateTime LicenceValidTo { get; set; }
        public DateTime LogBookCreatedOn { get; set; }
        public long LogBookEndPage { get; set; }
        public string LogBookNumber { get; set; }
        public DateTime LogBookStartDate { get; set; }
        public long LogBookStartPage { get; set; }
        public bool NextPage { get; set; }
        public int PermitId { get; set; }
        public string VesselName { get; internal set; }
    }
}
