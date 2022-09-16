using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FLUXLocationType
    {
        public CodeType TypeCode { get; set; }

        public IDType CountryID { get; set; }

        public IDType ID { get; set; }

        public CodeType GeopoliticalRegionCode { get; set; }

        [XmlElement("Name")]
        public TextType[] Name { get; set; }

        public IDType SovereignRightsCountryID { get; set; }

        public IDType JurisdictionCountryID { get; set; }

        public CodeType RegionalFisheriesManagementOrganizationCode { get; set; }

        public FLUXGeographicalCoordinateType SpecifiedPhysicalFLUXGeographicalCoordinate { get; set; }

        [XmlElement("PostalStructuredAddress")]
        public StructuredAddressType[] PostalStructuredAddress { get; set; }

        public StructuredAddressType PhysicalStructuredAddress { get; set; }

        [XmlElement("BoundarySpecifiedPolygon")]
        public SpecifiedPolygonType[] BoundarySpecifiedPolygon { get; set; }

        [XmlElement("ApplicableFLUXCharacteristic")]
        public FLUXCharacteristicType[] ApplicableFLUXCharacteristic { get; set; }
    }
}
