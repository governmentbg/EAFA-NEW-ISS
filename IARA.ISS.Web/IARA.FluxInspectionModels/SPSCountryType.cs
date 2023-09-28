namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSCountry", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSCountryType
    {

        private IDType idField;

        private TextType[] nameField;

        private SPSCountrySubDivisionType[] subordinateSPSCountrySubDivisionField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlElement("SubordinateSPSCountrySubDivision")]
        public SPSCountrySubDivisionType[] SubordinateSPSCountrySubDivision
        {
            get => this.subordinateSPSCountrySubDivisionField;
            set => this.subordinateSPSCountrySubDivisionField = value;
        }
    }
}