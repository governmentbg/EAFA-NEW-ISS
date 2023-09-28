namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQRLSupplyChainTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQRLSupplyChainTradeLineItemType
    {

        private CIQDocumentLineDocumentType associatedCIQDocumentLineDocumentField;

        private CIQLSupplyChainTradeAgreementType specifiedCIQLSupplyChainTradeAgreementField;

        private CIQLSupplyChainTradeDeliveryType specifiedCIQLSupplyChainTradeDeliveryField;

        private CITradeProductType specifiedCITradeProductField;

        private CIReferencedProductType[] substituteApplicableCIReferencedProductField;

        private ReferencedLogisticsPackageType physicalReferencedLogisticsPackageField;

        private CIQLSupplyChainTradeSettlementType specifiedCIQLSupplyChainTradeSettlementField;

        public CIQDocumentLineDocumentType AssociatedCIQDocumentLineDocument
        {
            get => this.associatedCIQDocumentLineDocumentField;
            set => this.associatedCIQDocumentLineDocumentField = value;
        }

        public CIQLSupplyChainTradeAgreementType SpecifiedCIQLSupplyChainTradeAgreement
        {
            get => this.specifiedCIQLSupplyChainTradeAgreementField;
            set => this.specifiedCIQLSupplyChainTradeAgreementField = value;
        }

        public CIQLSupplyChainTradeDeliveryType SpecifiedCIQLSupplyChainTradeDelivery
        {
            get => this.specifiedCIQLSupplyChainTradeDeliveryField;
            set => this.specifiedCIQLSupplyChainTradeDeliveryField = value;
        }

        public CITradeProductType SpecifiedCITradeProduct
        {
            get => this.specifiedCITradeProductField;
            set => this.specifiedCITradeProductField = value;
        }

        [XmlElement("SubstituteApplicableCIReferencedProduct")]
        public CIReferencedProductType[] SubstituteApplicableCIReferencedProduct
        {
            get => this.substituteApplicableCIReferencedProductField;
            set => this.substituteApplicableCIReferencedProductField = value;
        }

        public ReferencedLogisticsPackageType PhysicalReferencedLogisticsPackage
        {
            get => this.physicalReferencedLogisticsPackageField;
            set => this.physicalReferencedLogisticsPackageField = value;
        }

        public CIQLSupplyChainTradeSettlementType SpecifiedCIQLSupplyChainTradeSettlement
        {
            get => this.specifiedCIQLSupplyChainTradeSettlementField;
            set => this.specifiedCIQLSupplyChainTradeSettlementField = value;
        }
    }
}