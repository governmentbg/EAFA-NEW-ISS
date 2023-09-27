namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISCRLSupplyChainTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISCRLSupplyChainTradeLineItemType
    {

        private IDType[] additionalIDField;

        private IDType[] barcodeIDField;

        private IDType idField;

        private CodeType typeCodeField;

        private CodeType typeExtensionCodeField;

        private CISCRLSupplyChainTradeDeliveryType specifiedCISCRLSupplyChainTradeDeliveryField;

        private CIDocumentLineDocumentType associatedCIDocumentLineDocumentField;

        private CISSupplyChainTradeAgreementType specifiedCISSupplyChainTradeAgreementField;

        private CITradeProductType[] specifiedCITradeProductField;

        private CISLogisticsPackageType[] referencedCISLogisticsPackageField;

        [XmlElement("AdditionalID")]
        public IDType[] AdditionalID
        {
            get => this.additionalIDField;
            set => this.additionalIDField = value;
        }

        [XmlElement("BarcodeID")]
        public IDType[] BarcodeID
        {
            get => this.barcodeIDField;
            set => this.barcodeIDField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public CodeType TypeExtensionCode
        {
            get => this.typeExtensionCodeField;
            set => this.typeExtensionCodeField = value;
        }

        public CISCRLSupplyChainTradeDeliveryType SpecifiedCISCRLSupplyChainTradeDelivery
        {
            get => this.specifiedCISCRLSupplyChainTradeDeliveryField;
            set => this.specifiedCISCRLSupplyChainTradeDeliveryField = value;
        }

        public CIDocumentLineDocumentType AssociatedCIDocumentLineDocument
        {
            get => this.associatedCIDocumentLineDocumentField;
            set => this.associatedCIDocumentLineDocumentField = value;
        }

        public CISSupplyChainTradeAgreementType SpecifiedCISSupplyChainTradeAgreement
        {
            get => this.specifiedCISSupplyChainTradeAgreementField;
            set => this.specifiedCISSupplyChainTradeAgreementField = value;
        }

        [XmlElement("SpecifiedCITradeProduct")]
        public CITradeProductType[] SpecifiedCITradeProduct
        {
            get => this.specifiedCITradeProductField;
            set => this.specifiedCITradeProductField = value;
        }

        [XmlElement("ReferencedCISLogisticsPackage")]
        public CISLogisticsPackageType[] ReferencedCISLogisticsPackage
        {
            get => this.referencedCISLogisticsPackageField;
            set => this.referencedCISLogisticsPackageField = value;
        }
    }
}