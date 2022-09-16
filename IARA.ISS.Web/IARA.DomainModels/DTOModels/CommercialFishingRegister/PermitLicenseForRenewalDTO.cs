using System;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class PermitLicenseForRenewalDTO
    {
        public int Id { get; set; }

        public string RegistrationNumber { get; set; }

        public string HolderNames { get; set; }

        public string Captain { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public string FishingGears { get; set; }

        public string AuqticOrganisms { get; set; }

        public bool IsChecked { get; set; }
    }
}
