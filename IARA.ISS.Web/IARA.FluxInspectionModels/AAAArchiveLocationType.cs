namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAArchiveLocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAArchiveLocationType
    {

        private IDType idField;

        private TextType nameField;

        private TextType[] directionsField;

        private TextType descriptionField;

        private IDType districtIDField;

        private TextType countryNameField;

        private TextType countrySubDivisionNameField;

        private IDType countryIDField;

        private IDType countrySubDivisionIDField;

        private TextType geopoliticalRegionNameField;

        private IDType countrySuperordinateIDField;

        private NumericType uTCOffsetNumericField;

        private TextType typeField;

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

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        [XmlElement("Directions")]
        public TextType[] Directions
        {
            get
            {
                return this.directionsField;
            }
            set
            {
                this.directionsField = value;
            }
        }

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public IDType DistrictID
        {
            get
            {
                return this.districtIDField;
            }
            set
            {
                this.districtIDField = value;
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

        public TextType GeopoliticalRegionName
        {
            get
            {
                return this.geopoliticalRegionNameField;
            }
            set
            {
                this.geopoliticalRegionNameField = value;
            }
        }

        public IDType CountrySuperordinateID
        {
            get
            {
                return this.countrySuperordinateIDField;
            }
            set
            {
                this.countrySuperordinateIDField = value;
            }
        }

        public NumericType UTCOffsetNumeric
        {
            get
            {
                return this.uTCOffsetNumericField;
            }
            set
            {
                this.uTCOffsetNumericField = value;
            }
        }

        public TextType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }
}