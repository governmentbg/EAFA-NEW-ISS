namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CILegalOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CILegalOrganizationType
    {

        private CodeType legalClassificationCodeField;

        private TextType nameField;

        private IDType idField;

        private TextType tradingBusinessNameField;

        private CITradeAddressType postalCITradeAddressField;

        private CILegalRegistrationType[] authorizedCILegalRegistrationField;

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

        public TextType TradingBusinessName
        {
            get
            {
                return this.tradingBusinessNameField;
            }
            set
            {
                this.tradingBusinessNameField = value;
            }
        }

        public CITradeAddressType PostalCITradeAddress
        {
            get
            {
                return this.postalCITradeAddressField;
            }
            set
            {
                this.postalCITradeAddressField = value;
            }
        }

        [XmlElement("AuthorizedCILegalRegistration")]
        public CILegalRegistrationType[] AuthorizedCILegalRegistration
        {
            get
            {
                return this.authorizedCILegalRegistrationField;
            }
            set
            {
                this.authorizedCILegalRegistrationField = value;
            }
        }
    }
}