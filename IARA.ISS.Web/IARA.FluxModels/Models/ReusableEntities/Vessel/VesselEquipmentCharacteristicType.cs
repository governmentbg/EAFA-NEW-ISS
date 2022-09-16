using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselEquipmentCharacteristicType
    {
        public CodeType TypeCode { get; set; }

        [XmlElement("Description")]
        public TextType[] Description { get; set; }

        public MeasureType ValueMeasure { get; set; }

        [XmlElement("Value")]
        public TextType[] Value { get; set; }

        [XmlElement("ValueCode")]
        public CodeType[] ValueCode { get; set; }

        public DateTimeType ValueDateTime { get; set; }

        public IndicatorType ValueIndicator { get; set; }

        public QuantityType ValueQuantity { get; set; }

        public FLUXBinaryFileType ValueFLUXBinaryFile { get; set; }
    }
}
