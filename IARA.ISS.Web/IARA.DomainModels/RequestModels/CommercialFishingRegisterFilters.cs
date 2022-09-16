using System;

namespace IARA.DomainModels.RequestModels
{
    public class CommercialFishingRegisterFilters : BaseRequestModel
    {
        public int? PermitTypeId { get; set; }

        public int? PermitLicenseTypeId { get; set; }

        public string Number { get; set; }

        public DateTime? IssuedOnRangeStartDate { get; set; }

        public DateTime? IssuedOnRangeEndDate { get; set; }

        public int? ShipId { get; set; }

        public string ShipName { get; set; }

        public string ShipCfr { get; set; }

        public string ShipExternalMarking { get; set; }

        public string ShipRegistrationCertificateNumber { get; set; }

        public string PoundNetName { get; set; }

        public string PoundNetNumber { get; set; }

        public int? FishingGearTypeId { get; set; }

        public string FishingGearMarkNumber { get; set; }

        public string FishingGearPingerNumber { get; set; }

        public string SubmittedForName { get; set; }

        public string SubmittedForIdentifier { get; set; }

        public string LogbookNumber { get; set; }

        public int? PermitTerritoryUnitId { get; set; }

        public bool? PermitIsSuspended { get; set; }

        public bool? PermitIsExpired { get; set; }

        public int? PermitLicenseTerritoryUnitId { get; set; }

        public bool? PermitLicenseIsSuspended { get; set; }

        public bool? PermitLicenseIsExpired { get; set; }

        public int? PersonId { get; set; }

        public int? LegalId { get; set; }
    }
}
