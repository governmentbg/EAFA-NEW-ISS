namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICreditorFinancialInstitution", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICreditorFinancialInstitutionType
    {

        private IDType bICIDField;

        private TextType clearingSystemNameField;

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

        private IDType japanFinancialInstitutionCommonIDField;

        private FinancialInstitutionAddressType locationFinancialInstitutionAddressField;

        private BranchFinancialInstitutionType subDivisionBranchFinancialInstitutionField;

        public IDType BICID
        {
            get
            {
                return this.bICIDField;
            }
            set
            {
                this.bICIDField = value;
            }
        }

        public TextType ClearingSystemName
        {
            get
            {
                return this.clearingSystemNameField;
            }
            set
            {
                this.clearingSystemNameField = value;
            }
        }

        public IDType CHIPSUniversalID
        {
            get
            {
                return this.cHIPSUniversalIDField;
            }
            set
            {
                this.cHIPSUniversalIDField = value;
            }
        }

        public IDType NewZealandNCCID
        {
            get
            {
                return this.newZealandNCCIDField;
            }
            set
            {
                this.newZealandNCCIDField = value;
            }
        }

        public IDType IrishNSCID
        {
            get
            {
                return this.irishNSCIDField;
            }
            set
            {
                this.irishNSCIDField = value;
            }
        }

        public IDType UKSortCodeID
        {
            get
            {
                return this.uKSortCodeIDField;
            }
            set
            {
                this.uKSortCodeIDField = value;
            }
        }

        public IDType CHIPSParticipantID
        {
            get
            {
                return this.cHIPSParticipantIDField;
            }
            set
            {
                this.cHIPSParticipantIDField = value;
            }
        }

        public IDType SwissBCID
        {
            get
            {
                return this.swissBCIDField;
            }
            set
            {
                this.swissBCIDField = value;
            }
        }

        public IDType FedwireRoutingNumberID
        {
            get
            {
                return this.fedwireRoutingNumberIDField;
            }
            set
            {
                this.fedwireRoutingNumberIDField = value;
            }
        }

        public IDType PortugueseNCCID
        {
            get
            {
                return this.portugueseNCCIDField;
            }
            set
            {
                this.portugueseNCCIDField = value;
            }
        }

        public IDType RussianCentralBankID
        {
            get
            {
                return this.russianCentralBankIDField;
            }
            set
            {
                this.russianCentralBankIDField = value;
            }
        }

        public IDType ItalianDomesticID
        {
            get
            {
                return this.italianDomesticIDField;
            }
            set
            {
                this.italianDomesticIDField = value;
            }
        }

        public IDType AustrianBankleitzahlID
        {
            get
            {
                return this.austrianBankleitzahlIDField;
            }
            set
            {
                this.austrianBankleitzahlIDField = value;
            }
        }

        public IDType CanadianPaymentsAssociationID
        {
            get
            {
                return this.canadianPaymentsAssociationIDField;
            }
            set
            {
                this.canadianPaymentsAssociationIDField = value;
            }
        }

        public IDType SICID
        {
            get
            {
                return this.sICIDField;
            }
            set
            {
                this.sICIDField = value;
            }
        }

        public IDType GermanBankleitzahlID
        {
            get
            {
                return this.germanBankleitzahlIDField;
            }
            set
            {
                this.germanBankleitzahlIDField = value;
            }
        }

        public IDType SpanishDomesticInterbankingID
        {
            get
            {
                return this.spanishDomesticInterbankingIDField;
            }
            set
            {
                this.spanishDomesticInterbankingIDField = value;
            }
        }

        public IDType SouthAfricanNCCID
        {
            get
            {
                return this.southAfricanNCCIDField;
            }
            set
            {
                this.southAfricanNCCIDField = value;
            }
        }

        public IDType HongKongBankID
        {
            get
            {
                return this.hongKongBankIDField;
            }
            set
            {
                this.hongKongBankIDField = value;
            }
        }

        public IDType AustralianBSBID
        {
            get
            {
                return this.australianBSBIDField;
            }
            set
            {
                this.australianBSBIDField = value;
            }
        }

        public IDType IndianFinancialSystemID
        {
            get
            {
                return this.indianFinancialSystemIDField;
            }
            set
            {
                this.indianFinancialSystemIDField = value;
            }
        }

        public IDType HellenicBankID
        {
            get
            {
                return this.hellenicBankIDField;
            }
            set
            {
                this.hellenicBankIDField = value;
            }
        }

        public IDType PolishNationalClearingID
        {
            get
            {
                return this.polishNationalClearingIDField;
            }
            set
            {
                this.polishNationalClearingIDField = value;
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

        public IDType JapanFinancialInstitutionCommonID
        {
            get
            {
                return this.japanFinancialInstitutionCommonIDField;
            }
            set
            {
                this.japanFinancialInstitutionCommonIDField = value;
            }
        }

        public FinancialInstitutionAddressType LocationFinancialInstitutionAddress
        {
            get
            {
                return this.locationFinancialInstitutionAddressField;
            }
            set
            {
                this.locationFinancialInstitutionAddressField = value;
            }
        }

        public BranchFinancialInstitutionType SubDivisionBranchFinancialInstitution
        {
            get
            {
                return this.subDivisionBranchFinancialInstitutionField;
            }
            set
            {
                this.subDivisionBranchFinancialInstitutionField = value;
            }
        }
    }
}