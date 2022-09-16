using System;
using System.Collections.Generic;

namespace IARA.FVMSModels.ExternalModels
{
    /// <summary>
    /// ISS Fgear model / brought from ISS API/
    /// </summary>
    public class ApiFGear : FishingGear
    {
        public ApiFGear()
        {

        }

        public ApiFGear(FishingGear gear)
        {
            this.CFR = gear.CFR;
            this.Name = gear.Name;
            this.Code = gear.Code;
            this.Eye = gear.Eye;
            this.Marks = gear.Marks;
            this.Notes = gear.Notes;
            this.Quantity = gear.Quantity;
        }

        /// <summary>
        /// Init in FVMS
        /// </summary>       
        public long Id { get; set; }
        /// <summary>
        /// Init in FVMS
        /// </summary>
        public DateTime StoredDT { get; set; }
        /// <summary>
        /// Init in FVMS
        /// </summary>
        public DateTime UpdatedDT { get; set; }


        /// <summary>
        /// FVMS - not in use
        /// </summary>
        public string rsr { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// ISS fgear id???
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// ISS Certificate id
        /// </summary>
        public int udo_id { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// ISS FGear id
        /// </summary>
        public int ured_id { get; set; }

        /// <summary>
        /// Init in FVMS
        /// Additinal field for parsing fgear mark field
        /// </summary>
        public List<string> MarkiList { get; set; }

        /// <summary>
        /// Init in FVMS
        /// New ISS integration field        
        /// </summary>
        public string create_date { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// ISS Certificate number
        /// </summary>
        public string nomer { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// ISS Certificate valid to - datetime
        /// </summary>
        public DateTime valid_to { get; set; }

        /// <summary>
        /// Init in FVMS
        /// </summary>
        public bool hitted { get; set; }
        /// <summary>
        /// Init in FVMS
        /// </summary>
        public bool isDeleted { get; set; }

        /// <summary>
        /// Init in FVMS
        /// </summary>
        public ApiCerts ApiCerts { get; set; }
    }
}
