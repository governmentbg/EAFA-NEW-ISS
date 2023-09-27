namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("StructuredAddress", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class StructuredAddressType
    {

        private IDType idField;

        private CodeType postcodeCodeField;

        private TextType buildingNameField;

        private TextType streetNameField;

        private TextType cityNameField;

        private IDType countryIDField;

        private TextType citySubDivisionNameField;

        private TextType countryNameField;

        private TextType countrySubDivisionNameField;

        private TextType blockNameField;

        private TextType plotIdentificationField;

        private TextType postOfficeBoxField;

        private TextType buildingNumberField;

        private TextType staircaseNumberField;

        private TextType floorIdentificationField;

        private TextType roomIdentificationField;

        private TextType postalAreaField;

        private TextType postcodeField;

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

        public TextType BlockName
        {
            get
            {
                return this.blockNameField;
            }
            set
            {
                this.blockNameField = value;
            }
        }

        public TextType PlotIdentification
        {
            get
            {
                return this.plotIdentificationField;
            }
            set
            {
                this.plotIdentificationField = value;
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

        public TextType StaircaseNumber
        {
            get
            {
                return this.staircaseNumberField;
            }
            set
            {
                this.staircaseNumberField = value;
            }
        }

        public TextType FloorIdentification
        {
            get
            {
                return this.floorIdentificationField;
            }
            set
            {
                this.floorIdentificationField = value;
            }
        }

        public TextType RoomIdentification
        {
            get
            {
                return this.roomIdentificationField;
            }
            set
            {
                this.roomIdentificationField = value;
            }
        }

        public TextType PostalArea
        {
            get
            {
                return this.postalAreaField;
            }
            set
            {
                this.postalAreaField = value;
            }
        }

        public TextType Postcode
        {
            get
            {
                return this.postcodeField;
            }
            set
            {
                this.postcodeField = value;
            }
        }
    }
}