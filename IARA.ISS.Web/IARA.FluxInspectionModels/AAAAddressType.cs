using IARA.FluxInspectionModels;

namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAAddress", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAAddressType
    {

        private IDType idField;

        private CodeType postcodeCodeField;

        private TextType lineOneField;

        private TextType lineTwoField;

        private TextType lineThreeField;

        private TextType cityNameField;

        private IDType countryIDField;

        private TextType citySubDivisionNameField;

        private TextType countryNameField;

        private TextType countrySubDivisionNameField;

        private AddressFormatTypeCodeType formatCodeField;

        private TextType postOfficeBoxField;

        private TextType operationCentreField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

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

        public TextType CitySubDivisionName
        {
            get => this.citySubDivisionNameField;
            set => this.citySubDivisionNameField = value;
        }

        public TextType CountryName
        {
            get => this.countryNameField;
            set => this.countryNameField = value;
        }

        public TextType CountrySubDivisionName
        {
            get => this.countrySubDivisionNameField;
            set => this.countrySubDivisionNameField = value;
        }

        public AddressFormatTypeCodeType FormatCode
        {
            get => this.formatCodeField;
            set => this.formatCodeField = value;
        }

        public TextType PostOfficeBox
        {
            get => this.postOfficeBoxField;
            set => this.postOfficeBoxField = value;
        }

        public TextType OperationCentre
        {
            get => this.operationCentreField;
            set => this.operationCentreField = value;
        }
    }
}
