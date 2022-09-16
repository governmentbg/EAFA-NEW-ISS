using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IARA.FVMSModels.ExternalModels
{
    public class Permit
    {
        /// <summary>
        /// Permit number - 03108281
        /// </summary>
        [JsonPropertyName("nomer")]
        public string Number { get; set; }

        /// <summary>
        /// Permit creation datetime
        /// </summary>
        [JsonPropertyName("create_date")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Permit valid if revoke_flag = 0 or revoke_flg = ""
        /// </summary>
        [JsonPropertyName("revoke_flag")]
        public string revoke_flag { get; set; }

        [JsonIgnore]
        public bool IsRevoked
        {
            get
            {
                return revoke_flag == "0" ? true : !string.IsNullOrEmpty(revoke_flag);
            }
            set
            {
                if (value)
                {
                    revoke_flag = "0";
                }
                else
                {
                    revoke_flag = "";
                }
            }
        }

        /// <summary>
        /// Vessel CFR
        /// </summary>
        [JsonPropertyName("VCFR")]
        public string CFR { get; set; }

        /// <summary>
        /// Initializing in FVMS 
        /// 1. Get only permits from ISS by CFR
        /// 2. Get only license/certs from ISS by CFR
        /// 3. Init Certs with 2. result
        /// </summary>
        [JsonPropertyName("Certs")]
        public ICollection<Certificate> Certificates { get; set; }
    }
}
