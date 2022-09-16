using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class SpecifiedDirectPositionType
    {
        public TextType Name { get; set; }

        [XmlElement("CoordinateReferenceDimension")]
        public TextType[] CoordinateReferenceDimension { get; set; }

        [XmlElement("AxisLabelList")]
        public TextType[] AxisLabelList { get; set; }

        [XmlElement("UOMLabelList")]
        public TextType[] UOMLabelList { get; set; }

        [XmlElement("CountNumeric")]
        public NumericType[] CountNumeric { get; set; }
    }
}
