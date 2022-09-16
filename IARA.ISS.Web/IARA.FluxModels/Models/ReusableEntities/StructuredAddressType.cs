using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class StructuredAddressType
    {
        public IDType ID { get; set; }

        public CodeType PostcodeCode { get; set; }

        public TextType BuildingName { get; set; }

        public TextType StreetName { get; set; }

        public TextType CityName { get; set; }

        public IDType CountryID { get; set; }

        public TextType CitySubDivisionName { get; set; }

        public TextType CountryName { get; set; }

        public TextType CountrySubDivisionName { get; set; }

        public TextType BlockName { get; set; }

        public TextType PlotIdentification { get; set; }

        public TextType PostOfficeBox { get; set; }

        public TextType BuildingNumber { get; set; }

        public TextType StaircaseNumber { get; set; }

        public TextType FloorIdentification { get; set; }

        public TextType RoomIdentification { get; set; }

        public TextType PostalArea { get; set; }

        public TextType Postcode { get; set; }
    }
}
