namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFCountry", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFCountryType
    {

        private IDType idField;

        private TextType[] nameField;

        private RASFFCountrySubDivisionType subordinateRASFFCountrySubDivisionField;

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

        public RASFFCountrySubDivisionType SubordinateRASFFCountrySubDivision
        {
            get => this.subordinateRASFFCountrySubDivisionField;
            set => this.subordinateRASFFCountrySubDivisionField = value;
        }
    }
}