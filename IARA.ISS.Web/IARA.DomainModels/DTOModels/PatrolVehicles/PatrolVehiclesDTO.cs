using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.DTOModels.Vehicles
{
    public class PatrolVehiclesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? FlagCountryId { get; set; }
        public string FlagCountry { get; set; }
        public string ExternalMark { get; set; }
        public int PatrolVehicleTypeId { get; set; }
        public string PatrolVehicleType { get; set; }
        public string Cfr { get; set; }
        public string Uvi { get; set; }
        public string IrcscallSign { get; set; }
        public string Mmsi { get; set; }
        public int? VesselTypeId { get; set; }
        public string VesselType { get; set; }
        public int? InstitutionId { get; set; }
        public string Institution { get; set; }
        public bool IsActive { get; set; }
    }
}
