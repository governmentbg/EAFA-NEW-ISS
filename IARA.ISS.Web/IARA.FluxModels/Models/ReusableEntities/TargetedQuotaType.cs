using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class TargetedQuotaType
    {
        public CodeType TypeCode { get; set; }

        public CodeType ObjectCode { get; set; }


        [XmlElement("WeightMeasure")]
        public MeasureType[] WeightMeasure { get; set; }


        [XmlElement("SpecifiedQuantity")]
        public QuantityType[] SpecifiedQuantity { get; set; }
    }
}
