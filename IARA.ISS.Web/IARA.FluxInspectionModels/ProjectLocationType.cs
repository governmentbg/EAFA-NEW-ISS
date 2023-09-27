namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectLocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectLocationType
    {

        private IDType idField;

        private TextType nameField;

        private CodeType typeCodeField;

        private TextType directionsField;

        private TextType descriptionField;

        private IDType districtIDField;

        private SpecifiedGeographicalCoordinateType[] physicalSpecifiedGeographicalCoordinateField;

        private UnstructuredAddressType[] postalUnstructuredAddressField;

        private ProjectLocationType[] subordinateProjectLocationField;

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

        public TextType Directions
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

        [XmlElement("PhysicalSpecifiedGeographicalCoordinate")]
        public SpecifiedGeographicalCoordinateType[] PhysicalSpecifiedGeographicalCoordinate
        {
            get
            {
                return this.physicalSpecifiedGeographicalCoordinateField;
            }
            set
            {
                this.physicalSpecifiedGeographicalCoordinateField = value;
            }
        }

        [XmlElement("PostalUnstructuredAddress")]
        public UnstructuredAddressType[] PostalUnstructuredAddress
        {
            get
            {
                return this.postalUnstructuredAddressField;
            }
            set
            {
                this.postalUnstructuredAddressField = value;
            }
        }

        [XmlElement("SubordinateProjectLocation")]
        public ProjectLocationType[] SubordinateProjectLocation
        {
            get
            {
                return this.subordinateProjectLocationField;
            }
            set
            {
                this.subordinateProjectLocationField = value;
            }
        }
    }
}