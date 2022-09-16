using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IARA.FVMSModels.GeoZones
{
    public class GeoZoneReport
    {
        public Guid Identifier { get; set; }

        /// <summary>
        /// Дата/час на създаване на репорта
        /// </summary>
        [JsonPropertyName("CrDT")]
        public DateTime CreatedOn { get; set; }

        public List<GeoZoneData> GeoZones { get; set; }

    }
}
