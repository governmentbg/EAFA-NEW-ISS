using System;
using System.Text.Json.Serialization;

namespace IARA.FVMSModels.Inspections
{
    public class INSRecord
    {
#warning InsId structure missing
        [JsonPropertyName("InsId")]
        public string InspectionId { get; set; }

        [JsonPropertyName("InsData")]
        public byte[] InspectionData { get; set; }

        [JsonPropertyName("TstampCr")]
        public DateTime CreatedOn { get; set; }

#warning Pos structure missing
        public string Pos { get; set; }
    }
}
