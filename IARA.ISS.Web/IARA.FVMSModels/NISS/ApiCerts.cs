using System;
using System.Collections.Generic;

namespace IARA.FVMSModels.ExternalModels
{
    /// <summary>
    /// Данни за удостоверения за риболов на дадено РК от API интерфейс на стара ИСС на ИАРА
    /// </summary>
    public class ApiCerts : Certificate
    {
        public ApiCerts()
        {

        }

        public ApiCerts(Certificate cert)
        {
            this.CFR = cert.CFR;
            this.CurrentPage = cert.CurrentPage;
            this.LicenceCreatedOn = cert.LicenceCreatedOn;
            this.LicenceNumber = cert.LicenceNumber;
            this.LicenceValidFrom = cert.LicenceValidFrom;
            this.LicenceValidTo = cert.LicenceValidTo;
            this.LogBookCreatedOn = cert.LogBookCreatedOn;
            this.LogBookEndPage = cert.LogBookEndPage;
            this.LogBookNumber = cert.LogBookNumber;
            this.LogBookStartDate = cert.LogBookStartDate;
            this.LogBookStartPage = cert.LogBookStartPage;
            this.Name = cert.Name;
            this.NextPage = cert.NextPage;

            if (cert.Permit != null)
                this.Permit = new ApiPerm(cert.Permit);

            if (cert.FishingGears != null)
            {
                this.FishingGears = new List<FishingGear>();

                foreach (var fishingGear in cert.FishingGears)
                {
                    this.FishingGears.Add(new ApiFGear(fishingGear));
                }
            }
        }

        /// <summary>
        /// Init in FVMS
        /// </summary> 
        public long Id { get; set; }

        /// <summary>
        /// Init in FVMS
        /// Време на запис в СНРК
        /// </summary>
        public DateTime StoredDT { get; set; }

        /// <summary>
        /// Init in FVMS
        /// Време на актуализация на данните в СНРК
        /// </summary>
        public DateTime UpdatedDT { get; set; }

        /// <summary>
        /// FVMS - not in use
        /// </summary>
        public int dnevnik_id { get; set; }

        /// <summary>
        /// Init in FVMS
        /// </summary>
        public bool hitted { get; set; }

        /// <summary>
        /// Init in FVMS
        /// </summary>
        public bool isDeleted { get; set; }
    }
}
