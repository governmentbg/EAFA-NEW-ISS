namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingAddress", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingAddressType
    {

        private CodeType postcodeCodeField;

        private TextType lineOneField;

        private TextType lineTwoField;

        private TextType countryNameField;

        public CodeType PostcodeCode
        {
            get
            {
                return this.postcodeCodeField;
            }
            set
            {
                this.postcodeCodeField = value;
            }
        }

        public TextType LineOne
        {
            get
            {
                return this.lineOneField;
            }
            set
            {
                this.lineOneField = value;
            }
        }

        public TextType LineTwo
        {
            get
            {
                return this.lineTwoField;
            }
            set
            {
                this.lineTwoField = value;
            }
        }

        public TextType CountryName
        {
            get
            {
                return this.countryNameField;
            }
            set
            {
                this.countryNameField = value;
            }
        }
    }
}