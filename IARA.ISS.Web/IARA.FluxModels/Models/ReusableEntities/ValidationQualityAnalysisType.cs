using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class ValidationQualityAnalysisType
    {
        public CodeType LevelCode { get; set; }

        public CodeType TypeCode { get; set; }

        [XmlElement("Result")]
        public TextType[] Result { get; set; }

        public IDType ID { get; set; }

        public TextType Description { get; set; }

        [XmlElement("ReferencedItem")]
        public TextType[] ReferencedItem { get; set; }
    }
}
