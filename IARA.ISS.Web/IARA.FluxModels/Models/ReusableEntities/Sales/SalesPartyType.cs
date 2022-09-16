using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class SalesPartyType
    {
        public IDType ID { get; set; }

        public TextType Name { get; set; }

        public CodeType TypeCode { get; set; }

        public IDType CountryID { get; set; }

        [XmlElement("RoleCode")]
        public CodeType[] RoleCode { get; set; }

        [XmlElement("SpecifiedStructuredAddress")]
        public StructuredAddressType[] SpecifiedStructuredAddress { get; set; }


        public FLUXOrganizationType SpecifiedFLUXOrganization { get; set; }
    }
}
