namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CreditorFinancialInstitution", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CreditorFinancialInstitutionType
    {

        private IDType bICIDField;

        private IDType cHIPSUniversalIDField;

        private IDType newZealandNCCIDField;

        private IDType irishNSCIDField;

        private IDType uKSortCodeIDField;

        private IDType cHIPSParticipantIDField;

        private IDType swissBCIDField;

        private IDType fedwireRoutingNumberIDField;

        private IDType portugueseNCCIDField;

        private IDType russianCentralBankIDField;

        private IDType italianDomesticIDField;

        private IDType austrianBankleitzahlIDField;

        private IDType canadianPaymentsAssociationIDField;

        private IDType sICIDField;

        private IDType germanBankleitzahlIDField;

        private IDType spanishDomesticInterbankingIDField;

        private IDType southAfricanNCCIDField;

        private IDType hongKongBankIDField;

        private IDType australianBSBIDField;

        private IDType indianFinancialSystemIDField;

        private IDType hellenicBankIDField;

        private IDType polishNationalClearingIDField;

        private TextType nameField;

        private TextType clearingSystemNameField;

        private IDType[] sortCodeIDField;

        private IDType japanFinancialInstitutionCommonIDField;

        private FinancialInstitutionAddressType locationFinancialInstitutionAddressField;

        private BranchFinancialInstitutionType subDivisionBranchFinancialInstitutionField;

        public IDType BICID
        {
            get => this.bICIDField;
            set => this.bICIDField = value;
        }

        public IDType CHIPSUniversalID
        {
            get => this.cHIPSUniversalIDField;
            set => this.cHIPSUniversalIDField = value;
        }

        public IDType NewZealandNCCID
        {
            get => this.newZealandNCCIDField;
            set => this.newZealandNCCIDField = value;
        }

        public IDType IrishNSCID
        {
            get => this.irishNSCIDField;
            set => this.irishNSCIDField = value;
        }

        public IDType UKSortCodeID
        {
            get => this.uKSortCodeIDField;
            set => this.uKSortCodeIDField = value;
        }

        public IDType CHIPSParticipantID
        {
            get => this.cHIPSParticipantIDField;
            set => this.cHIPSParticipantIDField = value;
        }

        public IDType SwissBCID
        {
            get => this.swissBCIDField;
            set => this.swissBCIDField = value;
        }

        public IDType FedwireRoutingNumberID
        {
            get => this.fedwireRoutingNumberIDField;
            set => this.fedwireRoutingNumberIDField = value;
        }

        public IDType PortugueseNCCID
        {
            get => this.portugueseNCCIDField;
            set => this.portugueseNCCIDField = value;
        }

        public IDType RussianCentralBankID
        {
            get => this.russianCentralBankIDField;
            set => this.russianCentralBankIDField = value;
        }

        public IDType ItalianDomesticID
        {
            get => this.italianDomesticIDField;
            set => this.italianDomesticIDField = value;
        }

        public IDType AustrianBankleitzahlID
        {
            get => this.austrianBankleitzahlIDField;
            set => this.austrianBankleitzahlIDField = value;
        }

        public IDType CanadianPaymentsAssociationID
        {
            get => this.canadianPaymentsAssociationIDField;
            set => this.canadianPaymentsAssociationIDField = value;
        }

        public IDType SICID
        {
            get => this.sICIDField;
            set => this.sICIDField = value;
        }

        public IDType GermanBankleitzahlID
        {
            get => this.germanBankleitzahlIDField;
            set => this.germanBankleitzahlIDField = value;
        }

        public IDType SpanishDomesticInterbankingID
        {
            get => this.spanishDomesticInterbankingIDField;
            set => this.spanishDomesticInterbankingIDField = value;
        }

        public IDType SouthAfricanNCCID
        {
            get => this.southAfricanNCCIDField;
            set => this.southAfricanNCCIDField = value;
        }

        public IDType HongKongBankID
        {
            get => this.hongKongBankIDField;
            set => this.hongKongBankIDField = value;
        }

        public IDType AustralianBSBID
        {
            get => this.australianBSBIDField;
            set => this.australianBSBIDField = value;
        }

        public IDType IndianFinancialSystemID
        {
            get => this.indianFinancialSystemIDField;
            set => this.indianFinancialSystemIDField = value;
        }

        public IDType HellenicBankID
        {
            get => this.hellenicBankIDField;
            set => this.hellenicBankIDField = value;
        }

        public IDType PolishNationalClearingID
        {
            get => this.polishNationalClearingIDField;
            set => this.polishNationalClearingIDField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType ClearingSystemName
        {
            get => this.clearingSystemNameField;
            set => this.clearingSystemNameField = value;
        }

        [XmlElement("SortCodeID")]
        public IDType[] SortCodeID
        {
            get => this.sortCodeIDField;
            set => this.sortCodeIDField = value;
        }

        public IDType JapanFinancialInstitutionCommonID
        {
            get => this.japanFinancialInstitutionCommonIDField;
            set => this.japanFinancialInstitutionCommonIDField = value;
        }

        public FinancialInstitutionAddressType LocationFinancialInstitutionAddress
        {
            get => this.locationFinancialInstitutionAddressField;
            set => this.locationFinancialInstitutionAddressField = value;
        }

        public BranchFinancialInstitutionType SubDivisionBranchFinancialInstitution
        {
            get => this.subDivisionBranchFinancialInstitutionField;
            set => this.subDivisionBranchFinancialInstitutionField = value;
        }
    }
}