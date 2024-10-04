using IARA.Mobile.Application.DTObjects.Common;
using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionTransportVehicleDto : InspectionEditDto
    {
        public int? VehicleTypeId { get; set; }
        public int? CountryId { get; set; }
        public string TractorLicensePlateNum { get; set; }
        public string TractorBrand { get; set; }
        public string TractorModel { get; set; }
        public string TrailerLicensePlateNum { get; set; }
        public bool? IsSealed { get; set; }
        public int? SealInstitutionId { get; set; }
        public string SealCondition { get; set; }
        public string TransporterComment { get; set; }
        public string InspectionAddress { get; set; }
        public LocationDto InspectionLocation { get; set; }
        public string TransportDestination { get; set; }
        public List<InspectionLogBookPageDto> InspectionLogBookPages { get; set; }

    }
}
