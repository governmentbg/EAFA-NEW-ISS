namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class PossibleLogBooksForPageDTO
    {
        public int LogBookId { get; set; }

        public string LogBookNumber { get; set; }

        public int? LogBookPermitLicenseId { get; set; }

        public string BuyerNumber { get; set; }

        public string BuyerUrorr { get; set; }

        public string BuyerName { get; set; }

        public string PersonName { get; set; }

        public string LegalName { get; set; }

        public string PermitLicenseNumber { get; set; } 

        public string QualifiedFisherName { get; set; }

        public string QualifiedFisherNumber { get; set; }
    }
}
