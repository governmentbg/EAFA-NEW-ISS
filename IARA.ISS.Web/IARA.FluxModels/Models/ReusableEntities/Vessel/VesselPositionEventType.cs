using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselPositionEventType
    {
        public DateTimeType ObtainedOccurrenceDateTime { get; set; }


        public CodeType TypeCode { get; set; }


        public MeasureType SpeedValueMeasure { get; set; }


        public MeasureType CourseValueMeasure { get; set; }


        public CodeType ActivityTypeCode { get; set; }


        public VesselGeographicalCoordinateType SpecifiedVesselGeographicalCoordinate { get; set; }
    }
}
