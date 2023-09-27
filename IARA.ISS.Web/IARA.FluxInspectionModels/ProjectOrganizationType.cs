namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectOrganizationType
    {

        private CodeType businessTypeCodeField;

        private CodeType legalClassificationCodeField;

        private IDType taxRegistrationIDField;

        private TextType nameField;

        private IDType idField;

        private TextType descriptionField;

        private IDType districtIDField;

        private ProjectContactType[] designatedProjectContactField;

        private ProjectOrganizationType[] subordinateProjectOrganizationField;

        private UnstructuredAddressType[] postalUnstructuredAddressField;

        private ProjectLocationType[] physicalProjectLocationField;

        public CodeType BusinessTypeCode
        {
            get
            {
                return this.businessTypeCodeField;
            }
            set
            {
                this.businessTypeCodeField = value;
            }
        }

        public CodeType LegalClassificationCode
        {
            get
            {
                return this.legalClassificationCodeField;
            }
            set
            {
                this.legalClassificationCodeField = value;
            }
        }

        public IDType TaxRegistrationID
        {
            get
            {
                return this.taxRegistrationIDField;
            }
            set
            {
                this.taxRegistrationIDField = value;
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

        [XmlElement("DesignatedProjectContact")]
        public ProjectContactType[] DesignatedProjectContact
        {
            get
            {
                return this.designatedProjectContactField;
            }
            set
            {
                this.designatedProjectContactField = value;
            }
        }

        [XmlElement("SubordinateProjectOrganization")]
        public ProjectOrganizationType[] SubordinateProjectOrganization
        {
            get
            {
                return this.subordinateProjectOrganizationField;
            }
            set
            {
                this.subordinateProjectOrganizationField = value;
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

        [XmlElement("PhysicalProjectLocation")]
        public ProjectLocationType[] PhysicalProjectLocation
        {
            get
            {
                return this.physicalProjectLocationField;
            }
            set
            {
                this.physicalProjectLocationField = value;
            }
        }
    }
}