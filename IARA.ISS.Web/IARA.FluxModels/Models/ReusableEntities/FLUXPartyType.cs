using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FLUXPartyType
    {
        [XmlElement("ID")]
        public IDType[] ID { get; set; }

        [XmlElement("Name")]
        public TextType[] Name { get; set; }
    }
}
