using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselDimensionType
    {
        public CodeType TypeCode { get; set; }

        public MeasureType ValueMeasure { get; set; }
    }
}
