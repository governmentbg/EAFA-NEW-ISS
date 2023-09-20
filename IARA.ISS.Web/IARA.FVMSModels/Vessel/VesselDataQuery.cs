using System;

namespace IARA.FVMSModels.Vessel
{
    public class VesselDataQuery
    {
        public string Cfr { get; set; }

        // Ако не е подадено, заявката връща най-актуалните данни
        public DateTime? Date { get; set; }
    }
}
