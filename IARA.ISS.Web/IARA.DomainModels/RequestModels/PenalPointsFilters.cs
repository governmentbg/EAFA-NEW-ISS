using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.RequestModels
{
    public class PenalPointsFilters : BaseRequestModel
    {
        public string PenalDecreeNum { get; set; }

        public string DecreeNum { get; set; }

        public string PermitNum { get; set; }

        public string PermitLicenseNum { get; set; }

        public DateTime? DecreeDateFrom { get; set; }

        public DateTime? DecreeDateTo { get; set; }

        public DateTime? PenalDecreeDateFrom { get; set; }

        public DateTime? PenalDecreeDateTo { get; set; }

        public int? ShipId { get; set; }

        public string ShipName { get; set; }

        public string ShipCfr { get; set; }

        public string ShipExternalMarking { get; set; }

        public string ShipRegistrationCertificateNumber { get; set; }

        public string PermitOwnerName { get; set; }

        public string PermitOwnerIdentifier { get; set; }

        public string CaptainName { get; set; }

        public string CaptainIdentifier { get; set; }

        public bool? isIncreasePoints { get; set; }

        public PointsTypeEnum? PointsType { get; set; }

        public int? PersonId { get; set; }

        public int? LegalId { get; set; }

    }
}
