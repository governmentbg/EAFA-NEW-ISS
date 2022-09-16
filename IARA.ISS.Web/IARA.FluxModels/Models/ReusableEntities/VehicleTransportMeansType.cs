using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VehicleTransportMeansType
    {
        public IDType ID { get; set; }

        public IDType RegistrationCountryID { get; set; }

        public TextType Name { get; set; }

        public CodeType TypeCode { get; set; }

        public SalesPartyType OwnerSalesParty { get; set; }
    }
}
