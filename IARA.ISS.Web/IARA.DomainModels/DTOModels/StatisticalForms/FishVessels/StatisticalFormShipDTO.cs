using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.StatisticalForms.FishVessels
{
    public class StatisticalFormShipDTO
    {
        public int? ShipId { get; set; }
        public int? ShipYears { get; set; }
        public int? ShipLenghtId { get; set; }
        public int? GrossTonnageId { get; set; }
        public int? FuelTypeId { get; set; }
        public int? FuelConsumption { get; set; }
    }
}
