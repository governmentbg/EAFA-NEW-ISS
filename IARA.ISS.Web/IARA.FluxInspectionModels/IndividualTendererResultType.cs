namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("IndividualTendererResult", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class IndividualTendererResultType
    {

        private AmountType quotedPriceAmountField;

        private QuantityType itemQuotedQuantityField;

        private CodeType resultCodeField;

        private TextType[] descriptionField;

        private IDType tendererIDField;

        private TextType tendererNameField;

        private TenderingDeliverableType[] appliedTenderingDeliverableField;

        public AmountType QuotedPriceAmount
        {
            get => this.quotedPriceAmountField;
            set => this.quotedPriceAmountField = value;
        }

        public QuantityType ItemQuotedQuantity
        {
            get => this.itemQuotedQuantityField;
            set => this.itemQuotedQuantityField = value;
        }

        public CodeType ResultCode
        {
            get => this.resultCodeField;
            set => this.resultCodeField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public IDType TendererID
        {
            get => this.tendererIDField;
            set => this.tendererIDField = value;
        }

        public TextType TendererName
        {
            get => this.tendererNameField;
            set => this.tendererNameField = value;
        }

        [XmlElement("AppliedTenderingDeliverable")]
        public TenderingDeliverableType[] AppliedTenderingDeliverable
        {
            get => this.appliedTenderingDeliverableField;
            set => this.appliedTenderingDeliverableField = value;
        }
    }
}