using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:18")]
    public partial class VesselIdentityType
    {

        [XmlElement("VesselID")]
        public IDType[] VesselID { get; set; }


        [XmlElement("VesselName")]
        public TextType[] VesselName { get; set; }


        public IDType VesselRegistrationCountryID { get; set; }


        public CodeType VesselTypeCode { get; set; }
    }
}
