namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRLSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRLSupplyChainTradeTransactionType
    {

        private CIRLSupplyChainTradeAgreementType applicableCIRLSupplyChainTradeAgreementField;

        private CIRLSupplyChainTradeDeliveryType applicableCIRLSupplyChainTradeDeliveryField;

        private CITradeProductType includedCITradeProductField;

        public CIRLSupplyChainTradeAgreementType ApplicableCIRLSupplyChainTradeAgreement
        {
            get => this.applicableCIRLSupplyChainTradeAgreementField;
            set => this.applicableCIRLSupplyChainTradeAgreementField = value;
        }

        public CIRLSupplyChainTradeDeliveryType ApplicableCIRLSupplyChainTradeDelivery
        {
            get => this.applicableCIRLSupplyChainTradeDeliveryField;
            set => this.applicableCIRLSupplyChainTradeDeliveryField = value;
        }

        public CITradeProductType IncludedCITradeProduct
        {
            get => this.includedCITradeProductField;
            set => this.includedCITradeProductField = value;
        }
    }
}