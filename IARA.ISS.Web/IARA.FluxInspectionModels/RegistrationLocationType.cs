namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RegistrationLocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RegistrationLocationType
    {

        private IDType countryIDField;

        private TextType[] descriptionField;

        private CodeType geopoliticalRegionCodeField;

        private IDType[] idField;

        private TextType[] nameField;

        private CodeType typeCodeField;

        private StructuredAddressType physicalStructuredAddressField;

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

        [XmlElement("Description")]
        public TextType[] Description
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

        public CodeType GeopoliticalRegionCode
        {
            get
            {
                return this.geopoliticalRegionCodeField;
            }
            set
            {
                this.geopoliticalRegionCodeField = value;
            }
        }

        [XmlElement("ID")]
        public IDType[] ID
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

        [XmlElement("Name")]
        public TextType[] Name
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

        public StructuredAddressType PhysicalStructuredAddress
        {
            get
            {
                return this.physicalStructuredAddressField;
            }
            set
            {
                this.physicalStructuredAddressField = value;
            }
        }
    }
}