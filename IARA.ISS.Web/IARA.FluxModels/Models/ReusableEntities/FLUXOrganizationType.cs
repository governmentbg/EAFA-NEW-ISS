using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FLUXOrganizationType
    {
        public TextType Name { get; set; }

        [XmlElement("PostalStructuredAddress")]
        public StructuredAddressType[] PostalStructuredAddress { get; set; }
    }
}
