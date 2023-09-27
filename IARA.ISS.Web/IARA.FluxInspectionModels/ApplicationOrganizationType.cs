namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ApplicationOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ApplicationOrganizationType
    {

        private CodeType businessTypeCodeField;

        private TextType nameField;

        private TextType internationalNameField;

        private IDType idField;

        private IndicatorType contactRequiredIndicatorField;

        private BusinessLicenceRegistrationType authorizedBusinessLicenceRegistrationField;

        private TenderingContactType[] designatedTenderingContactField;

        private LocalOrganizationType[] subordinateLocalOrganizationField;

        private TenderingAddressType postalTenderingAddressField;

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

        public TextType InternationalName
        {
            get
            {
                return this.internationalNameField;
            }
            set
            {
                this.internationalNameField = value;
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

        public IndicatorType ContactRequiredIndicator
        {
            get
            {
                return this.contactRequiredIndicatorField;
            }
            set
            {
                this.contactRequiredIndicatorField = value;
            }
        }

        public BusinessLicenceRegistrationType AuthorizedBusinessLicenceRegistration
        {
            get
            {
                return this.authorizedBusinessLicenceRegistrationField;
            }
            set
            {
                this.authorizedBusinessLicenceRegistrationField = value;
            }
        }

        [XmlElement("DesignatedTenderingContact")]
        public TenderingContactType[] DesignatedTenderingContact
        {
            get
            {
                return this.designatedTenderingContactField;
            }
            set
            {
                this.designatedTenderingContactField = value;
            }
        }

        [XmlElement("SubordinateLocalOrganization")]
        public LocalOrganizationType[] SubordinateLocalOrganization
        {
            get
            {
                return this.subordinateLocalOrganizationField;
            }
            set
            {
                this.subordinateLocalOrganizationField = value;
            }
        }

        public TenderingAddressType PostalTenderingAddress
        {
            get
            {
                return this.postalTenderingAddressField;
            }
            set
            {
                this.postalTenderingAddressField = value;
            }
        }
    }
}