using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class AquacultureFacilitiesFilters : BaseRequestModel
    {
        public string RegNum { get; set; }

        public string UrorNum { get; set; }

        public string Name { get; set; }

        public string Eik { get; set; }

        public DateTime? RegistrationDateFrom { get; set; }

        public DateTime? RegistrationDateTo { get; set; }

        public List<int> StatusIds { get; set; }

        public int? TerritoryUnitId { get; set; }

        public List<int> WaterAreaTypeIds { get; set; }

        public int? PopulatedAreaId { get; set; }

        public string Location { get; set; }

        public List<string> WaterSalinityTypes { get; set; }

        public List<string> WaterTemperatureTypes { get; set; }

        public List<string> SystemTypes { get; set; }

        public int? AquaticOrganismId { get; set; }

        public int? PowerSupplyTypeId { get; set; }

        public List<int> InstallationTypeIds { get; set; }

        public decimal? TotalWaterAreaFrom { get; set; }

        public decimal? TotalWaterAreaTo { get; set; }

        public decimal? TotalProductionCapacityFrom { get; set; }

        public decimal? TotalProductionCapacityTo { get; set; }

        public int? PersonId { get; set; }

        public int? LegalId { get; set; }
    }
}
