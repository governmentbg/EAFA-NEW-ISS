using System;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class OverlappingLogBookDTO
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public int StatusId { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public long StartPage { get; set; }

        public long EndPage { get; set; }

        /// <summary>
        /// Name of ship when ownerType is `null` and logBookType is `Ship`;
        /// Name and number of registered byer when ownerType is `RegisteredBuyer`;
        /// Name and EGN of person when ownerType is `Person`;
        /// Name and EIK of legal when ownerType is `LegalPerson`;
        /// Name and number of aquaculture facility when ownerType is `null` and logBookType is `Aquaculture`
        /// </summary>
        public string OwnerName { get; set; }

        public int? LogBookPermitLicenseId { get; set; }

        public long? LogBookPermitLicenseStartPage { get; set; }

        public long? LogBookPermitLicenseEndPage { get; set; }

        public DateTime? LogBookPermitLicenseValidFrom { get; set; }

        public DateTime? LogBookPermitLicenseValidTo { get; set; }

        public string LogBookPermitLicenseNumber { get; set; }
    }
}
