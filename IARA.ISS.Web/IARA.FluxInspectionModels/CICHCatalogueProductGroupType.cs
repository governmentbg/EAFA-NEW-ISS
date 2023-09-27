namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICHCatalogueProductGroup", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICHCatalogueProductGroupType
    {

        private IDType idField;

        private TextType nameField;

        private CICLSupplyChainTradeLineItemType[] includedCICLSupplyChainTradeLineItemField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
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