namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("UnstructuredAddress", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class UnstructuredAddressType
    {

        private IDType idField;

        private CodeType postcodeCodeField;

        private TextType lineOneField;

        private TextType lineTwoField;

        private TextType lineThreeField;

        private TextType lineFourField;

        private TextType lineFiveField;

        private TextType cityNameField;

        private IDType countryIDField;

        private TextType citySubDivisionNameField;

        private TextType countryNameField;

        private TextType[] countrySubDivisionNameField;

        private AddressTypeCodeType typeCodeField;

        private IDType countrySubDivisionIDField;

        private IDType cityIDField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

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

        public TextType LineThree
        {
            get
            {
                return this.lineThreeField;
            }
            set
            {
                this.lineThreeField = value;
            }
        }

        public TextType LineFour
        {
            get
            {
                return this.lineFourField;
            }
            set
            {
                this.lineFourField = value;
            }
        }

        public TextType LineFive
        {
            get
            {
                return this.lineFiveField;
            }
            set
            {
                this.lineFiveField = value;
            }
        }

        public TextType CityName
        {
            get
            {
                return this.cityNameField;
            }
            set
            {
                this.cityNameField = value;
            }
        }

        public IDType CountryID
        {
            get
            {
                return this.countryIDField;
            }
            set
            {
                this.countryIDField = value;
            }
        }

        public TextType CitySubDivisionName
        {
            get
            {
                return this.citySubDivisionNameField;
            }
            set
            {
                this.citySubDivisionNameField = value;
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

        [XmlElement("CountrySubDivisionName")]
        public TextType[] CountrySubDivisionName
        {
            get
            {
                return this.countrySubDivisionNameField;
            }
            set
            {
                this.countrySubDivisionNameField = value;
            }
        }

        public AddressTypeCodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        public IDType CountrySubDivisionID
        {
            get
            {
                return this.countrySubDivisionIDField;
            }
            set
            {
                this.countrySubDivisionIDField = value;
            }
        }

        public IDType CityID
        {
            get
            {
                return this.cityIDField;
            }
            set
            {
                this.cityIDField = value;
            }
        }
    }
}