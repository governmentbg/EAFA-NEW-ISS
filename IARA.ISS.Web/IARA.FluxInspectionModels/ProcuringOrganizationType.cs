namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProcuringOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProcuringOrganizationType
    {

        private TextType nameField;

        private IDType idField;

        private IDType bureauIDField;

        private IDType divisionIDField;

        private TextType internationalNameField;

        private TenderingContactType primaryTenderingContactField;

        private TenderingContactType[] alternateDesignatedTenderingContactField;

        private TenderingAddressType postalTenderingAddressField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType BureauID
        {
            get => this.bureauIDField;
            set => this.bureauIDField = value;
        }

        public IDType DivisionID
        {
            get => this.divisionIDField;
            set => this.divisionIDField = value;
        }

        public TextType InternationalName
        {
            get => this.internationalNameField;
            set => this.internationalNameField = value;
        }

        public TenderingContactType PrimaryTenderingContact
        {
            get => this.primaryTenderingContactField;
            set => this.primaryTenderingContactField = value;
        }

        [XmlElement("AlternateDesignatedTenderingContact")]
        public TenderingContactType[] AlternateDesignatedTenderingContact
        {
            get => this.alternateDesignatedTenderingContactField;
            set => this.alternateDesignatedTenderingContactField = value;
        }

        public TenderingAddressType PostalTenderingAddress
        {
            get => this.postalTenderingAddressField;
            set => this.postalTenderingAddressField = value;
        }
    }
}