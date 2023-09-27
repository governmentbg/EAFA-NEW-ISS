namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDLSupplyChainTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDLSupplyChainTradeLineItemType
    {

        private CIDDLDocumentLineDocumentType associatedCIDDLDocumentLineDocumentField;

        private CIDDLSupplyChainTradeAgreementType specifiedCIDDLSupplyChainTradeAgreementField;

        private CIDDLSupplyChainTradeDeliveryType specifiedCIDDLSupplyChainTradeDeliveryField;

        private CIDDLSupplyChainTradeSettlementType specifiedCIDDLSupplyChainTradeSettlementField;

        private CITradeProductType specifiedCITradeProductField;

        private ReferencedLogisticsPackageType[] physicalReferencedLogisticsPackageField;

        private CIReferencedProductType[] substitutedCIReferencedProductField;

        public CIDDLDocumentLineDocumentType AssociatedCIDDLDocumentLineDocument
        {
            get
            {
                return this.associatedCIDDLDocumentLineDocumentField;
            }
            set
            {
                this.associatedCIDDLDocumentLineDocumentField = value;
            }
        }

        public CIDDLSupplyChainTradeAgreementType SpecifiedCIDDLSupplyChainTradeAgreement
        {
            get
            {
                return this.specifiedCIDDLSupplyChainTradeAgreementField;
            }
            set
            {
                this.specifiedCIDDLSupplyChainTradeAgreementField = value;
            }
        }

        public CIDDLSupplyChainTradeDeliveryType SpecifiedCIDDLSupplyChainTradeDelivery
        {
            get
            {
                return this.specifiedCIDDLSupplyChainTradeDeliveryField;
            }
            set
            {
                this.specifiedCIDDLSupplyChainTradeDeliveryField = value;
            }
        }

        public CIDDLSupplyChainTradeSettlementType SpecifiedCIDDLSupplyChainTradeSettlement
        {
            get
            {
                return this.specifiedCIDDLSupplyChainTradeSettlementField;
            }
            set
            {
                this.specifiedCIDDLSupplyChainTradeSettlementField = value;
            }
        }

        public CITradeProductType SpecifiedCITradeProduct
        {
            get
            {
                return this.specifiedCITradeProductField;
            }
            set
            {
                this.specifiedCITradeProductField = value;
            }
        }

        [XmlElement("PhysicalReferencedLogisticsPackage")]
        public ReferencedLogisticsPackageType[] PhysicalReferencedLogisticsPackage
        {
            get
            {
                return this.physicalReferencedLogisticsPackageField;
            }
            set
            {
                this.physicalReferencedLogisticsPackageField = value;
            }
        }

        [XmlElement("SubstitutedCIReferencedProduct")]
        public CIReferencedProductType[] SubstitutedCIReferencedProduct
        {
            get
            {
                return this.substitutedCIReferencedProductField;
            }
            set
            {
                this.substitutedCIReferencedProductField = value;
            }
        }
    }
}