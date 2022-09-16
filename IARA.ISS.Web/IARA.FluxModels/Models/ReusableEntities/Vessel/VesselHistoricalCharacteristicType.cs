using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselHistoricalCharacteristicType
    {
        public CodeType TypeCode { get; set; }

        [XmlElement("Description")]
        public TextType[] Description { get; set; }

        public TextType Value { get; set; }

        public IndicatorType ValueIndicator { get; set; }

        public CodeType ValueCode { get; set; }

        public DateTimeType ValueDateTime { get; set; }

        public MeasureType ValueMeasure { get; set; }

        public QuantityType ValueQuantity { get; set; }
    }
}
