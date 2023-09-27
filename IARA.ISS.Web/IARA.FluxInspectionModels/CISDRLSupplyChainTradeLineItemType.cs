namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISDRLSupplyChainTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISDRLSupplyChainTradeLineItemType
    {

        private CISDocumentLineDocumentType associatedCISDocumentLineDocumentField;

        private CISSupplyChainTradeAgreementType specifiedCISSupplyChainTradeAgreementField;

        private CISDFLSupplyChainTradeDeliveryType specifiedCISDFLSupplyChainTradeDeliveryField;

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

        public CISDFLSupplyChainTradeDeliveryType SpecifiedCISDFLSupplyChainTradeDelivery
        {
            get => this.specifiedCISDFLSupplyChainTradeDeliveryField;
            set => this.specifiedCISDFLSupplyChainTradeDeliveryField = value;
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