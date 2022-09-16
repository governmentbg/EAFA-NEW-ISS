using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselGeographicalCoordinateType
    {
        public MeasureType AltitudeMeasure { get; set; }

        public MeasureType LatitudeMeasure { get; set; }

        public MeasureType LongitudeMeasure { get; set; }

        public IDType SystemID { get; set; }
    }
}
