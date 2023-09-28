namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIILBSubordinateTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIILBSubordinateTradeLineItemType
    {

        private IDType[] idField;

        private CodeType categoryCodeField;

        private CodeType responseReasonCodeField;

        private ReferencedProductType specifiedReferencedProductField;

        private CITradeProductType[] applicableCITradeProductField;

        private CIILBSupplyChainTradeAgreementType specifiedCIILBSupplyChainTradeAgreementField;

        private CIILBSupplyChainTradeDeliveryType specifiedCIILBSupplyChainTradeDeliveryField;

        private CIILBSupplyChainTradeSettlementType specifiedCIILBSupplyChainTradeSettlementField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public CodeType ResponseReasonCode
        {
            get => this.responseReasonCodeField;
            set => this.responseReasonCodeField = value;
        }

        public ReferencedProductType SpecifiedReferencedProduct
        {
            get => this.specifiedReferencedProductField;
            set => this.specifiedReferencedProductField = value;
        }

        [XmlElement("ApplicableCITradeProduct")]
        public CITradeProductType[] ApplicableCITradeProduct
        {
            get => this.applicableCITradeProductField;
            set => this.applicableCITradeProductField = value;
        }

        public CIILBSupplyChainTradeAgreementType SpecifiedCIILBSupplyChainTradeAgreement
        {
            get => this.specifiedCIILBSupplyChainTradeAgreementField;
            set => this.specifiedCIILBSupplyChainTradeAgreementField = value;
        }

        public CIILBSupplyChainTradeDeliveryType SpecifiedCIILBSupplyChainTradeDelivery
        {
            get => this.specifiedCIILBSupplyChainTradeDeliveryField;
            set => this.specifiedCIILBSupplyChainTradeDeliveryField = value;
        }

        public CIILBSupplyChainTradeSettlementType SpecifiedCIILBSupplyChainTradeSettlement
        {
            get => this.specifiedCIILBSupplyChainTradeSettlementField;
            set => this.specifiedCIILBSupplyChainTradeSettlementField = value;
        }
    }
}