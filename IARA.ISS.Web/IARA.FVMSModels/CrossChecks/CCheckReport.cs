using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using IARA.FVMSModels.CrossChecks;

namespace IARA.FVMSModels.GeoZones
{
    public class CCheckReport
    {
        public string Identifier { get; set; }

        [JsonPropertyName("CrDT")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("CCheck")]
        public List<CCheck> CChecks { get; set; }
    }
}
