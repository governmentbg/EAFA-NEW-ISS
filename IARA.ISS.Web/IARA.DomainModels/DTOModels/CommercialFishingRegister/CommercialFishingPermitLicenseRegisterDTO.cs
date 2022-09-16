using System;
using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class CommercialFishingPermitLicenseRegisterDTO
    {
        public int Id { get; set; }

        public string RegistrationNumber { get; set; }

        public int PermitId { get; set; }

        public int? DeliveryId { get; set; }

        public int ApplicationId { get; set; }

        public int TypeId { get; set; }

        public CommercialFishingTypesEnum TypeCode { get; set; }

        public string TypeName { get; set; }

        public string TerritoryUnitName { get; set; }

        public string SubmittedForName { get; set; }

        public string QualifiedFisherName { get; set; }

        public DateTime IssueDate { get; set; }

        public bool IsSuspended { get; set; }

        public string SuspensionsInformation { get; set; }

        public bool IsActive { get; set; }

        public PageCodeEnum PageCode { get; set; }

        public List<CommercialFishingLogbookRegisterDTO> Logbooks { get; set; }
    }
}
