namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRTSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRTSupplyChainTradeTransactionType
    {

        private CIRDocumentLineDocumentType associatedCIRDocumentLineDocumentField;

        private CIRTSupplyChainTradeAgreementType applicableCIRTSupplyChainTradeAgreementField;

        private CIRTSupplyChainTradeSettlementType applicableCIRTSupplyChainTradeSettlementField;

        private CIRLSupplyChainTradeLineItemType[] includedCIRLSupplyChainTradeLineItemField;

        private CIReferencedDocumentType[] associatedCIReferencedDocumentField;

        private CIRTSupplyChainTradeDeliveryType[] applicableCIRTSupplyChainTradeDeliveryField;

        private CITradeProductType[] includedCITradeProductField;

        private CINoteType[] includedCINoteField;

        public CIRDocumentLineDocumentType AssociatedCIRDocumentLineDocument
        {
            get => this.associatedCIRDocumentLineDocumentField;
            set => this.associatedCIRDocumentLineDocumentField = value;
        }

        public CIRTSupplyChainTradeAgreementType ApplicableCIRTSupplyChainTradeAgreement
        {
            get => this.applicableCIRTSupplyChainTradeAgreementField;
            set => this.applicableCIRTSupplyChainTradeAgreementField = value;
        }

        public CIRTSupplyChainTradeSettlementType ApplicableCIRTSupplyChainTradeSettlement
        {
            get => this.applicableCIRTSupplyChainTradeSettlementField;
            set => this.applicableCIRTSupplyChainTradeSettlementField = value;
        }

        [XmlElement("IncludedCIRLSupplyChainTradeLineItem")]
        public CIRLSupplyChainTradeLineItemType[] IncludedCIRLSupplyChainTradeLineItem
        {
            get => this.includedCIRLSupplyChainTradeLineItemField;
            set => this.includedCIRLSupplyChainTradeLineItemField = value;
        }

        [XmlElement("AssociatedCIReferencedDocument")]
        public CIReferencedDocumentType[] AssociatedCIReferencedDocument
        {
            get => this.associatedCIReferencedDocumentField;
            set => this.associatedCIReferencedDocumentField = value;
        }

        [XmlElement("ApplicableCIRTSupplyChainTradeDelivery")]
        public CIRTSupplyChainTradeDeliveryType[] ApplicableCIRTSupplyChainTradeDelivery
        {
            get => this.applicableCIRTSupplyChainTradeDeliveryField;
            set => this.applicableCIRTSupplyChainTradeDeliveryField = value;
        }

        [XmlElement("IncludedCITradeProduct")]
        public CITradeProductType[] IncludedCITradeProduct
        {
            get => this.includedCITradeProductField;
            set => this.includedCITradeProductField = value;
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get => this.includedCINoteField;
            set => this.includedCINoteField = value;
        }
    }
}