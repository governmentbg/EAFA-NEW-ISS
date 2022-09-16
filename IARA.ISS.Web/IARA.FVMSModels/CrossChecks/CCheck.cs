using System;

namespace IARA.FVMSModels.CrossChecks
{
    public class CCheck
    {
        public string Identifier { get; set; }
        public string Grid { get; set; }
        public string TrDS { get; set; }
        public string TrDF { get; set; }
        public string ChDS { get; set; }
        public string ChDF { get; set; }
        public string MDReq { get; set; }
        public string ChDesc { get; set; }
        public string Purpose { get; set; }
        public string Precond { get; set; }
        public byte Level { get; set; }
        public NecessityType Necessity { get; set; }
        public string LegelRef { get; set; }
        public string Remarks { get; set; }
        public DateTime DAdd { get; set; }
        public bool CCRes { get; set; }
        public DateTime LUpd { get; set; }
        public Vessel Vessel { get; set; }
    }
}
