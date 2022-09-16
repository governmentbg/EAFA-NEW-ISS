using System;
using System.Collections.Generic;
using System.Text;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class CommercialFishingPermitRegisterDTO
    {
        public int Id { get; set; }

        public string RegistrationNumber { get; set; }

        public int? DeliveryId { get; set; }

        public int ApplicationId { get; set; }

        public int TypeId { get; set; }

        public string TypeName { get; set; }

        public CommercialFishingTypesEnum TypeCode { get; set; }

        public string TerritoryUnitName { get; set; }

        public PageCodeEnum PageCode { get; set; }

        public string SubmittedForName { get; set; }

        public string ShipName { get; set; }

        public string QualifiedFisherName { get; set; }

        public DateTime IssueDate { get; set; }

        public bool IsSuspended { get; set; }

        public string SuspensionsInformation { get; set; }

        public bool IsActive { get; set; }

        public List<CommercialFishingPermitLicenseRegisterDTO> Licenses { get; set; }
    }
}
