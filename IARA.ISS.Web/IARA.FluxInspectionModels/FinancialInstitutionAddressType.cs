namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FinancialInstitutionAddress", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FinancialInstitutionAddressType
    {

        private CodeType postcodeCodeField;

        private TextType buildingNumberField;

        private TextType lineOneField;

        private TextType lineTwoField;

        private TextType lineThreeField;

        private TextType lineFourField;

        private TextType lineFiveField;

        private TextType streetNameField;

        private TextType cityNameField;

        private IDType countrySubDivisionIDField;

        private IDType countryIDField;

        private CodeType typeCodeField;

        private TextType departmentNameField;

        private TextType postOfficeBoxField;

        private IDType cityIDField;

        private TextType countrySubDivisionNameField;

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

        public CodeType TypeCode
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

        public TextType CountrySubDivisionName
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