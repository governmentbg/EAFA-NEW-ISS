using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class DelimitedPeriodType
    {
        public DateTimeType StartDateTime { get; set; }

        public DateTimeType EndDateTime { get; set; }

        public MeasureType DurationMeasure { get; set; }
    }
}
