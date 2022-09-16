using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FLUXGeographicalCoordinateType
    {
        public MeasureType LongitudeMeasure { get; set; }

        public MeasureType LatitudeMeasure { get; set; }


        public MeasureType AltitudeMeasure { get; set; }


        public IDType SystemID { get; set; }
    }
}
