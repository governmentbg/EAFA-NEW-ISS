using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IARA.FVMSModels.ExternalModels
{
    public class Certificate
    {
        /// <summary>
        /// Vessel name  - БУМЕРАНГ
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Vessel CFR - BGR002031477
        /// </summary>
        [JsonPropertyName("cfr")]
        public string CFR { get; set; }

        /// <summary>
        /// Д03108381-025-001
        /// </summary>
        [JsonPropertyName("dnevnik_nomer")]
        public string LogBookNumber { get; set; }

        /// <summary>
        /// Valid from 
        /// </summary>
        [JsonPropertyName("dnevnik_start_date")]
        public DateTime LogBookStartDate { get; set; }

        /// <summary>
        /// Used in integration with old ISS, and in FVMS process of creation page 
        /// </summary>
        [JsonPropertyName("dnevnik_start_page")]
        public long LogBookStartPage { get; set; }

        /// <summary>
        /// Used in integration with old ISS, and in FVMS process of creation page 
        /// </summary> 
        [JsonPropertyName("dnevnik_end_page")]
        public long LogBookEndPage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("dnevnik_create_date")]
        public DateTime LogBookCreatedOn { get; set; }

        /// <summary>
        /// 03108381-025
        /// </summary>
        [JsonPropertyName("udo_nomer")]
        public string LicenceNumber { get; set; }

        /// <summary>
        /// Valid from
        /// </summary>
        [JsonPropertyName("udo_valid_from")]
        public DateTime LicenceValidFrom { get; set; }

        /// <summary>
        /// Valid to
        /// </summary>
        [JsonPropertyName("udo_valid_to")]
        public DateTime LicenceValidTo { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        [JsonPropertyName("udo_create_date")]
        public DateTime LicenceCreatedOn { get; set; }

        /// <summary>
        /// Used in integration with old ISS, and in FVMS process of creation page 
        /// </summary>
        [JsonPropertyName("current_page")]
        public long CurrentPage { get; set; }

        /// <summary>
        /// Used in integration with old ISS, and in FVMS process of creation page 
        /// </summary>
        [JsonPropertyName("next_page")]
        public bool NextPage { get; set; }

        /// <summary>
        /// Initializing in FVMS      
        /// 1. Get only license/certs from ISS by CFR
        /// 2. Get only gears by cfr
        /// 3. Init Certs with 2. result
        /// </summary>
        [JsonPropertyName("Gears")]
        public ICollection<FishingGear> FishingGears { get; set; }

        /// <summary>
        /// Init in FVMS
        /// </summary>
        [JsonPropertyName("ApiPerm")]
        public Permit Permit { get; set; }
    }
}
