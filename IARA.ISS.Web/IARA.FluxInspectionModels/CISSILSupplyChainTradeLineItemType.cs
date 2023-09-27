namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISSILSupplyChainTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISSILSupplyChainTradeLineItemType
    {

        private CISDocumentLineDocumentType associatedCISDocumentLineDocumentField;

        private CISSupplyChainTradeAgreementType specifiedCISSupplyChainTradeAgreementField;

        private CISSILSupplyChainTradeDeliveryType specifiedCISSILSupplyChainTradeDeliveryField;

        private CITradeProductType specifiedCITradeProductField;

        private CIReferencedProductType substitutedCIReferencedProductField;

        private CIReferencedProductType[] substituteApplicableCIReferencedProductField;

        private CISSupplyChainTradeSettlementType specifiedCISSupplyChainTradeSettlementField;

        private ReferencedLogisticsPackageType[] physicalReferencedLogisticsPackageField;

        private CITradeProductType requisitionerSpecifiedCITradeProductField;

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

        public CISSILSupplyChainTradeDeliveryType SpecifiedCISSILSupplyChainTradeDelivery
        {
            get => this.specifiedCISSILSupplyChainTradeDeliveryField;
            set => this.specifiedCISSILSupplyChainTradeDeliveryField = value;
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

        public CISSupplyChainTradeSettlementType SpecifiedCISSupplyChainTradeSettlement
        {
            get => this.specifiedCISSupplyChainTradeSettlementField;
            set => this.specifiedCISSupplyChainTradeSettlementField = value;
        }

        [XmlElement("PhysicalReferencedLogisticsPackage")]
        public ReferencedLogisticsPackageType[] PhysicalReferencedLogisticsPackage
        {
            get => this.physicalReferencedLogisticsPackageField;
            set => this.physicalReferencedLogisticsPackageField = value;
        }

        public CITradeProductType RequisitionerSpecifiedCITradeProduct
        {
            get => this.requisitionerSpecifiedCITradeProductField;
            set => this.requisitionerSpecifiedCITradeProductField = value;
        }
    }
}