namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICLSupplyChainTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICLSupplyChainTradeLineItemType
    {

        private CICLDocumentLineDocumentType associatedCICLDocumentLineDocumentField;

        private CICLSupplyChainTradeAgreementType specifiedCICLSupplyChainTradeAgreementField;

        private CICLSupplyChainTradeDeliveryType specifiedCICLSupplyChainTradeDeliveryField;

        private CITradeTaxType[] specifiedCICLSupplyChainTradeSettlementField;

        private CatalogueTradeProductType specifiedCatalogueTradeProductField;

        private CIReferencedProductType[] substituteApplicableCIReferencedProductField;

        private CIReferencedProductType[] accessoryApplicableCIReferencedProductField;

        private CIReferencedProductType[] componentApplicableCIReferencedProductField;

        private CIReferencedProductType[] complementaryApplicableCIReferencedProductField;

        private CIReferencedProductType[] additionalApplicableCIReferencedProductField;

        private CIReferencedProductType[] requiredApplicableCIReferencedProductField;

        public CICLDocumentLineDocumentType AssociatedCICLDocumentLineDocument
        {
            get
            {
                return this.associatedCICLDocumentLineDocumentField;
            }
            set
            {
                this.associatedCICLDocumentLineDocumentField = value;
            }
        }

        public CICLSupplyChainTradeAgreementType SpecifiedCICLSupplyChainTradeAgreement
        {
            get
            {
                return this.specifiedCICLSupplyChainTradeAgreementField;
            }
            set
            {
                this.specifiedCICLSupplyChainTradeAgreementField = value;
            }
        }

        public CICLSupplyChainTradeDeliveryType SpecifiedCICLSupplyChainTradeDelivery
        {
            get
            {
                return this.specifiedCICLSupplyChainTradeDeliveryField;
            }
            set
            {
                this.specifiedCICLSupplyChainTradeDeliveryField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("ApplicableCITradeTax", IsNullable = false)]
        public CITradeTaxType[] SpecifiedCICLSupplyChainTradeSettlement
        {
            get
            {
                return this.specifiedCICLSupplyChainTradeSettlementField;
            }
            set
            {
                this.specifiedCICLSupplyChainTradeSettlementField = value;
            }
        }

        public CatalogueTradeProductType SpecifiedCatalogueTradeProduct
        {
            get
            {
                return this.specifiedCatalogueTradeProductField;
            }
            set
            {
                this.specifiedCatalogueTradeProductField = value;
            }
        }

        [XmlElement("SubstituteApplicableCIReferencedProduct")]
        public CIReferencedProductType[] SubstituteApplicableCIReferencedProduct
        {
            get
            {
                return this.substituteApplicableCIReferencedProductField;
            }
            set
            {
                this.substituteApplicableCIReferencedProductField = value;
            }
        }

        [XmlElement("AccessoryApplicableCIReferencedProduct")]
        public CIReferencedProductType[] AccessoryApplicableCIReferencedProduct
        {
            get
            {
                return this.accessoryApplicableCIReferencedProductField;
            }
            set
            {
                this.accessoryApplicableCIReferencedProductField = value;
            }
        }

        [XmlElement("ComponentApplicableCIReferencedProduct")]
        public CIReferencedProductType[] ComponentApplicableCIReferencedProduct
        {
            get
            {
                return this.componentApplicableCIReferencedProductField;
            }
            set
            {
                this.componentApplicableCIReferencedProductField = value;
            }
        }

        [XmlElement("ComplementaryApplicableCIReferencedProduct")]
        public CIReferencedProductType[] ComplementaryApplicableCIReferencedProduct
        {
            get
            {
                return this.complementaryApplicableCIReferencedProductField;
            }
            set
            {
                this.complementaryApplicableCIReferencedProductField = value;
            }
        }

        [XmlElement("AdditionalApplicableCIReferencedProduct")]
        public CIReferencedProductType[] AdditionalApplicableCIReferencedProduct
        {
            get
            {
                return this.additionalApplicableCIReferencedProductField;
            }
            set
            {
                this.additionalApplicableCIReferencedProductField = value;
            }
        }

        [XmlElement("RequiredApplicableCIReferencedProduct")]
        public CIReferencedProductType[] RequiredApplicableCIReferencedProduct
        {
            get
            {
                return this.requiredApplicableCIReferencedProductField;
            }
            set
            {
                this.requiredApplicableCIReferencedProductField = value;
            }
        }
    }
}