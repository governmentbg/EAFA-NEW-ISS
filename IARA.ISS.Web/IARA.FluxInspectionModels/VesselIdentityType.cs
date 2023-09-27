namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselIdentity", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselIdentityType
    {

        private IDType[] vesselIDField;

        private TextType[] vesselNameField;

        private IDType vesselRegistrationCountryIDField;

        private CodeType vesselTypeCodeField;

        [XmlElement("VesselID")]
        public IDType[] VesselID
        {
            get => this.vesselIDField;
            set => this.vesselIDField = value;
        }

        [XmlElement("VesselName")]
        public TextType[] VesselName
        {
            get => this.vesselNameField;
            set => this.vesselNameField = value;
        }

        public IDType VesselRegistrationCountryID
        {
            get => this.vesselRegistrationCountryIDField;
            set => this.vesselRegistrationCountryIDField = value;
        }

        public CodeType VesselTypeCode
        {
            get => this.vesselTypeCodeField;
            set => this.vesselTypeCodeField = value;
        }
    }
}