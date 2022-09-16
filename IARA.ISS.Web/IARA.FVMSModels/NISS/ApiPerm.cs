using System;
using System.Collections.Generic;

namespace IARA.FVMSModels.ExternalModels
{
    /// <summary>
    /// ISS Fishing Permits
    /// 
    /// Checked with new ISS Integration documentation - 24.09.2020
    /// </summary>
    public class ApiPerm : Permit
    {
        public ApiPerm() { }

        public ApiPerm(Permit permit)
        {
            this.CFR = permit.CFR;
            this.CreatedOn = permit.CreatedOn;
            this.Number = permit.Number;

            if (permit.Certificates != null)
            {
                this.Certificates = new List<Certificate>();

                foreach (var certificate in permit.Certificates)
                {
                    this.Certificates.Add(new ApiCerts(certificate));
                }
            }
        }

        /// <summary>
        /// Initializing in FVMS
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Initializing in FVMS
        /// </summary>
        public DateTime StoredDT { get; set; }

        /// <summary>
        /// Initializing in FVMS
        /// </summary>
        public DateTime UpdatedDT { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// ISS DB Id - sequence number
        /// ИД в БД на ИСС
        /// </summary>
        public int rsrid { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// Type of permit
        /// </summary>
        public int rsr_type { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// Type of permit
        /// </summary>
        public int sto_type { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// Permit validity
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// ISS DB Id of owner
        /// </summary>
        public string tituliar_id { get; set; }

        /// <summary>    
        /// FVMS - not in use
        /// ISS DB Id of user
        /// int type
        /// </summary>
        public string ribar_id { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// ISS DB ship Id
        /// int type
        /// </summary>
        public string ship_id { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// int type
        /// </summary>
        public string ship_doc_id { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// ISS DB Id dalqn if exist
        /// int type
        /// </summary>
        public string dalian_id { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// int type
        /// </summary>
        public string dalian_doc_doc_id { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// float type
        /// </summary>        
        public string dalian_tax { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// object type
        /// </summary>
        public string dalian_tax_date { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// int type
        /// </summary>
        public string stopanstvo_id { get; set; }

        /// <summary>
        /// FVMS - not in use
        ///  int type
        /// </summary>
        public string sto_doc_id { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// int type
        /// </summary>
        public string zveno_id { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// </summary>
        public string ctrl_code { get; set; }

        /// <summary>
        /// FVMS - Not in use 
        /// object type
        /// </summary>
        public string revoke_date { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// object type
        /// </summary>
        public string revoke_reason { get; set; }


        /// <summary>
        /// Initializing in FVMS
        /// </summary>
        public bool hitted { get; set; }

        /// <summary>
        /// Initializing in FVMS
        /// </summary>
        public bool isDeleted { get; set; }
    }
}
