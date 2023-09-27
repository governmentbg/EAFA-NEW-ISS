namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSAddress", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSAddressType
    {

        private CodeType postcodeCodeField;

        private TextType lineOneField;

        private TextType lineTwoField;

        private TextType lineThreeField;

        private TextType lineFourField;

        private TextType lineFiveField;

        private TextType cityNameField;

        private IDType countryIDField;

        private AddressTypeCodeType typeCodeField;

        private TextType countryNameField;

        private IDType countrySubDivisionIDField;

        private TextType countrySubDivisionNameField;

        private IDType cityIDField;

        public CodeType PostcodeCode
        {
            get => this.postcodeCodeField;
            set => this.postcodeCodeField = value;
        }

        public TextType LineOne
        {
            get => this.lineOneField;
            set => this.lineOneField = value;
        }

        public TextType LineTwo
        {
            get => this.lineTwoField;
            set => this.lineTwoField = value;
        }

        public TextType LineThree
        {
            get => this.lineThreeField;
            set => this.lineThreeField = value;
        }

        public TextType LineFour
        {
            get => this.lineFourField;
            set => this.lineFourField = value;
        }

        public TextType LineFive
        {
            get => this.lineFiveField;
            set => this.lineFiveField = value;
        }

        public TextType CityName
        {
            get => this.cityNameField;
            set => this.cityNameField = value;
        }

        public IDType CountryID
        {
            get => this.countryIDField;
            set => this.countryIDField = value;
        }

        public AddressTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType CountryName
        {
            get => this.countryNameField;
            set => this.countryNameField = value;
        }

        public IDType CountrySubDivisionID
        {
            get => this.countrySubDivisionIDField;
            set => this.countrySubDivisionIDField = value;
        }

        public TextType CountrySubDivisionName
        {
            get => this.countrySubDivisionNameField;
            set => this.countrySubDivisionNameField = value;
        }

        public IDType CityID
        {
            get => this.cityIDField;
            set => this.cityIDField = value;
        }
    }
}