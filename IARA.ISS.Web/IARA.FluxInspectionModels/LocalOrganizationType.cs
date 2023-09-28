namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LocalOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LocalOrganizationType
    {

        private TextType nameField;

        private IDType idField;

        private IDType districtIDField;

        private BusinessLicenceRegistrationType authorizedBusinessLicenceRegistrationField;

        private TenderingContactType primaryTenderingContactField;

        private TenderingAddressType postalTenderingAddressField;

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

        public TenderingContactType PrimaryTenderingContact
        {
            get
            {
                return this.primaryTenderingContactField;
            }
            set
            {
                this.primaryTenderingContactField = value;
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