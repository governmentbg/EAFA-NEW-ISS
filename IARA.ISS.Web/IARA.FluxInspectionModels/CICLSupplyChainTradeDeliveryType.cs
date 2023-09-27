namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICLSupplyChainTradeDelivery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICLSupplyChainTradeDeliveryType
    {

        private CIHandlingInstructionsType handlingCIHandlingInstructionsField;

        private CITransportDangerousGoodsType applicableCITransportDangerousGoodsField;

        private CISupplyChainPackagingType[] includedCISupplyChainPackagingField;

        public CIHandlingInstructionsType HandlingCIHandlingInstructions
        {
            get
            {
                return this.handlingCIHandlingInstructionsField;
            }
            set
            {
                this.handlingCIHandlingInstructionsField = value;
            }
        }

        public CITransportDangerousGoodsType ApplicableCITransportDangerousGoods
        {
            get
            {
                return this.applicableCITransportDangerousGoodsField;
            }
            set
            {
                this.applicableCITransportDangerousGoodsField = value;
            }
        }

        [XmlElement("IncludedCISupplyChainPackaging")]
        public CISupplyChainPackagingType[] IncludedCISupplyChainPackaging
        {
            get
            {
                return this.includedCISupplyChainPackagingField;
            }
            set
            {
                this.includedCISupplyChainPackagingField = value;
            }
        }
    }
}