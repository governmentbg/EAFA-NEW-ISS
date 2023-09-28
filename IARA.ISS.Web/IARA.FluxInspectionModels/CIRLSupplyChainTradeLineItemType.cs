namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRLSupplyChainTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRLSupplyChainTradeLineItemType
    {

        private CIRDocumentLineDocumentType associatedCIRDocumentLineDocumentField;

        private CIRLSupplyChainTradeSettlementType[] specifiedCIRLSupplyChainTradeSettlementField;

        private CIReferencedDocumentType[] referenceCIReferencedDocumentField;

        public CIRDocumentLineDocumentType AssociatedCIRDocumentLineDocument
        {
            get => this.associatedCIRDocumentLineDocumentField;
            set => this.associatedCIRDocumentLineDocumentField = value;
        }

        [XmlElement("SpecifiedCIRLSupplyChainTradeSettlement")]
        public CIRLSupplyChainTradeSettlementType[] SpecifiedCIRLSupplyChainTradeSettlement
        {
            get => this.specifiedCIRLSupplyChainTradeSettlementField;
            set => this.specifiedCIRLSupplyChainTradeSettlementField = value;
        }

        [XmlElement("ReferenceCIReferencedDocument")]
        public CIReferencedDocumentType[] ReferenceCIReferencedDocument
        {
            get => this.referenceCIReferencedDocumentField;
            set => this.referenceCIReferencedDocumentField = value;
        }
    }
}