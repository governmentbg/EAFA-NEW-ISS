namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICHSupplyChainTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICHSupplyChainTradeTransactionType
    {

        private CICHSupplyChainTradeAgreementType applicableCICHSupplyChainTradeAgreementField;

        private CICHCatalogueProductGroupType[] includedCICHCatalogueProductGroupField;

        private CICLSupplyChainTradeLineItemType[] includedCICLSupplyChainTradeLineItemField;

        public CICHSupplyChainTradeAgreementType ApplicableCICHSupplyChainTradeAgreement
        {
            get
            {
                return this.applicableCICHSupplyChainTradeAgreementField;
            }
            set
            {
                this.applicableCICHSupplyChainTradeAgreementField = value;
            }
        }

        [XmlElement("IncludedCICHCatalogueProductGroup")]
        public CICHCatalogueProductGroupType[] IncludedCICHCatalogueProductGroup
        {
            get
            {
                return this.includedCICHCatalogueProductGroupField;
            }
            set
            {
                this.includedCICHCatalogueProductGroupField = value;
            }
        }

        [XmlElement("IncludedCICLSupplyChainTradeLineItem")]
        public CICLSupplyChainTradeLineItemType[] IncludedCICLSupplyChainTradeLineItem
        {
            get
            {
                return this.includedCICLSupplyChainTradeLineItemField;
            }
            set
            {
                this.includedCICLSupplyChainTradeLineItemField = value;
            }
        }
    }
}