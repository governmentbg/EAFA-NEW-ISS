namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeCountry", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradeCountryType
    {

        private CountryIDType idField;

        private TextType[] nameField;

        private CITradeCountrySubDivisionType[] subordinateCITradeCountrySubDivisionField;

        public CountryIDType ID
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

        [XmlElement("SubordinateCITradeCountrySubDivision")]
        public CITradeCountrySubDivisionType[] SubordinateCITradeCountrySubDivision
        {
            get => this.subordinateCITradeCountrySubDivisionField;
            set => this.subordinateCITradeCountrySubDivisionField = value;
        }
    }
}