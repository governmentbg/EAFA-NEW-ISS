namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDHSupplyChainTradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDHSupplyChainTradeAgreementType
    {

        private CITradePartyType sellerCITradePartyField;

        private CITradePartyType buyerCITradePartyField;

        private CITradeDeliveryTermsType applicableCITradeDeliveryTermsField;

        private CIReferencedDocumentType buyerOrderReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] contractReferencedCIReferencedDocumentField;

        private CIReferencedDocumentType[] additionalReferencedCIReferencedDocumentField;

        private ProcuringProjectType specifiedProcuringProjectField;

        private CIReferencedDocumentType sellerOrderReferencedCIReferencedDocumentField;

        private CITradePartyType[] relevantCITradePartyField;

        public CITradePartyType SellerCITradeParty
        {
            get
            {
                return this.sellerCITradePartyField;
            }
            set
            {
                this.sellerCITradePartyField = value;
            }
        }

        public CITradePartyType BuyerCITradeParty
        {
            get
            {
                return this.buyerCITradePartyField;
            }
            set
            {
                this.buyerCITradePartyField = value;
            }
        }

        public CITradeDeliveryTermsType ApplicableCITradeDeliveryTerms
        {
            get
            {
                return this.applicableCITradeDeliveryTermsField;
            }
            set
            {
                this.applicableCITradeDeliveryTermsField = value;
            }
        }

        public CIReferencedDocumentType BuyerOrderReferencedCIReferencedDocument
        {
            get
            {
                return this.buyerOrderReferencedCIReferencedDocumentField;
            }
            set
            {
                this.buyerOrderReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("ContractReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] ContractReferencedCIReferencedDocument
        {
            get
            {
                return this.contractReferencedCIReferencedDocumentField;
            }
            set
            {
                this.contractReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("AdditionalReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] AdditionalReferencedCIReferencedDocument
        {
            get
            {
                return this.additionalReferencedCIReferencedDocumentField;
            }
            set
            {
                this.additionalReferencedCIReferencedDocumentField = value;
            }
        }

        public ProcuringProjectType SpecifiedProcuringProject
        {
            get
            {
                return this.specifiedProcuringProjectField;
            }
            set
            {
                this.specifiedProcuringProjectField = value;
            }
        }

        public CIReferencedDocumentType SellerOrderReferencedCIReferencedDocument
        {
            get
            {
                return this.sellerOrderReferencedCIReferencedDocumentField;
            }
            set
            {
                this.sellerOrderReferencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("RelevantCITradeParty")]
        public CITradePartyType[] RelevantCITradeParty
        {
            get
            {
                return this.relevantCITradePartyField;
            }
            set
            {
                this.relevantCITradePartyField = value;
            }
        }
    }
}