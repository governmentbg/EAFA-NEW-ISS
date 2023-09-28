namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISSNLSupplyChainTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISSNLSupplyChainTradeLineItemType
    {

        private CISDocumentLineDocumentType associatedCISDocumentLineDocumentField;

        private CISSupplyChainTradeAgreementType specifiedCISSupplyChainTradeAgreementField;

        private CISSNLSupplyChainTradeDeliveryType specifiedCISSNLSupplyChainTradeDeliveryField;

        private CITradeProductType specifiedCITradeProductField;

        private CIReferencedProductType substitutedCIReferencedProductField;

        private CIReferencedProductType[] substituteApplicableCIReferencedProductField;

        public CISDocumentLineDocumentType AssociatedCISDocumentLineDocument
        {
            get => this.associatedCISDocumentLineDocumentField;
            set => this.associatedCISDocumentLineDocumentField = value;
        }

        public CISSupplyChainTradeAgreementType SpecifiedCISSupplyChainTradeAgreement
        {
            get => this.specifiedCISSupplyChainTradeAgreementField;
            set => this.specifiedCISSupplyChainTradeAgreementField = value;
        }

        public CISSNLSupplyChainTradeDeliveryType SpecifiedCISSNLSupplyChainTradeDelivery
        {
            get => this.specifiedCISSNLSupplyChainTradeDeliveryField;
            set => this.specifiedCISSNLSupplyChainTradeDeliveryField = value;
        }

        public CITradeProductType SpecifiedCITradeProduct
        {
            get => this.specifiedCITradeProductField;
            set => this.specifiedCITradeProductField = value;
        }

        public CIReferencedProductType SubstitutedCIReferencedProduct
        {
            get => this.substitutedCIReferencedProductField;
            set => this.substitutedCIReferencedProductField = value;
        }

        [XmlElement("SubstituteApplicableCIReferencedProduct")]
        public CIReferencedProductType[] SubstituteApplicableCIReferencedProduct
        {
            get => this.substituteApplicableCIReferencedProductField;
            set => this.substituteApplicableCIReferencedProductField = value;
        }
    }
}