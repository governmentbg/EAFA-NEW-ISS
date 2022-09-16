using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselStorageCharacteristicType
    {
        [XmlElement("TypeCode")]
        public CodeType[] TypeCode { get; set; }

        public CodeType MethodTypeCode { get; set; }


        [XmlElement("CapacityValueMeasure")]
        public MeasureType[] CapacityValueMeasure { get; set; }


        [XmlElement("TemperatureValueMeasure")]
        public MeasureType[] TemperatureValueMeasure { get; set; }


        public QuantityType UnitValueQuantity { get; set; }


        public IDType ID { get; set; }
    }
}
