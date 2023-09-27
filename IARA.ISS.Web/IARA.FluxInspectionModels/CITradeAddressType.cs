namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeAddress", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradeAddressType
    {

        private IDType idField;

        private CodeType postcodeCodeField;

        private TextType postOfficeBoxField;

        private TextType buildingNameField;

        private TextType lineOneField;

        private TextType lineTwoField;

        private TextType lineThreeField;

        private TextType lineFourField;

        private TextType lineFiveField;

        private TextType streetNameField;

        private TextType cityNameField;

        private TextType citySubDivisionNameField;

        private CountryIDType countryIDField;

        private TextType[] countryNameField;

        private IDType countrySubDivisionIDField;

        private TextType[] countrySubDivisionNameField;

        private TextType attentionOfField;

        private TextType careOfField;

        private TextType buildingNumberField;

        private TextType additionalStreetNameField;

        private TextType departmentNameField;

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

        public TextType PostOfficeBox
        {
            get
            {
                return this.postOfficeBoxField;
            }
            set
            {
                this.postOfficeBoxField = value;
            }
        }

        public TextType BuildingName
        {
            get
            {
                return this.buildingNameField;
            }
            set
            {
                this.buildingNameField = value;
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

        public TextType StreetName
        {
            get
            {
                return this.streetNameField;
            }
            set
            {
                this.streetNameField = value;
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

        public CountryIDType CountryID
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

        [XmlElement("CountryName")]
        public TextType[] CountryName
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

        public TextType AttentionOf
        {
            get
            {
                return this.attentionOfField;
            }
            set
            {
                this.attentionOfField = value;
            }
        }

        public TextType CareOf
        {
            get
            {
                return this.careOfField;
            }
            set
            {
                this.careOfField = value;
            }
        }

        public TextType BuildingNumber
        {
            get
            {
                return this.buildingNumberField;
            }
            set
            {
                this.buildingNumberField = value;
            }
        }

        public TextType AdditionalStreetName
        {
            get
            {
                return this.additionalStreetNameField;
            }
            set
            {
                this.additionalStreetNameField = value;
            }
        }

        public TextType DepartmentName
        {
            get
            {
                return this.departmentNameField;
            }
            set
            {
                this.departmentNameField = value;
            }
        }
    }
}