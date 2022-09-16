using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class ConstructionLocationType
    {
        public IDType CountryID { get; set; }

        [XmlElement("Description")]
        public TextType[] Description { get; set; }

        public CodeType GeopoliticalRegionCode { get; set; }

        [XmlElement("ID")]
        public IDType[] ID { get; set; }

        [XmlElement("Name")]
        public TextType[] Name { get; set; }

        public CodeType TypeCode { get; set; }

        [XmlElement("SpecifiedContactParty")]
        public ContactPartyType[] SpecifiedContactParty { get; set; }

        public StructuredAddressType PhysicalStructuredAddress { get; set; }
    }
}
