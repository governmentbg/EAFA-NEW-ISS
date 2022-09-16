using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.AquacultureFacilities
{
    public class AquacultureFacilityDTO
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public int RegNum { get; set; }

        public string UrorNum { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }

        public string TerritoryUnit { get; set; }

        public AquacultureStatusEnum Status { get; set; }

        public string StatusName { get; set; }

        public int? DeliveryId { get; set; }

        public bool IsActive { get; set; }
    }
}
