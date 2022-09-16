using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.RequestModels
{
    public class PatrolVehiclesFilters : BaseRequestModel
    {
        public string Name { get; set; }
        public int? FlagCountryId { get; set; }
        public string ExternalMark { get; set; }
        public int? PatrolVehicleTypeId { get; set; }
        public string CFR { get; set; }
        public string UVI { get; set; }
        public string IRCSCallSign { get; set; }
        public string MMSI { get; set; }
        public int? VesselTypeId { get; set; }
        public int? InstitutionId { get; set; }
    }
}
