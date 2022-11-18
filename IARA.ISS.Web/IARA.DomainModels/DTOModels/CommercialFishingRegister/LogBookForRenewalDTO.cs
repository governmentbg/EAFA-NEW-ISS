namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class LogBookForRenewalDTO
    {
        public int LogBookPermitLicenseId { get; set; }
        
        public int LogBookId { get; set; }

        public string Number { get; set; }

        public bool IsOnline { get; set; }

        public long StartPageNumber { get; set; }

        public long EndPageNumber { get; set; }

        public long LastUsedPageNumber { get; set; }

        public string StatusName { get; set; }

        public string LogBookTypeName { get; set; }

        public string LastPermitLicenseNumber { get; set; }

        /// <summary>
        /// For UI purposes
        /// </summary>
        public bool IsChecked { get; set; }
    }
}
