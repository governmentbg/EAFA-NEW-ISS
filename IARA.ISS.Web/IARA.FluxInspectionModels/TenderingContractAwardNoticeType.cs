namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingContractAwardNotice", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingContractAwardNoticeType
    {

        private IndicatorType winIndicatorField;

        private AmountType priceAmountField;

        private QuantityType itemQuantityField;

        private ValueType evaluationScoreValueField;

        private TextType lossReasonField;

        private TenderingDeliverableType[] applicableTenderingDeliverableField;

        public IndicatorType WinIndicator
        {
            get => this.winIndicatorField;
            set => this.winIndicatorField = value;
        }

        public AmountType PriceAmount
        {
            get => this.priceAmountField;
            set => this.priceAmountField = value;
        }

        public QuantityType ItemQuantity
        {
            get => this.itemQuantityField;
            set => this.itemQuantityField = value;
        }

        public ValueType EvaluationScoreValue
        {
            get => this.evaluationScoreValueField;
            set => this.evaluationScoreValueField = value;
        }

        public TextType LossReason
        {
            get => this.lossReasonField;
            set => this.lossReasonField = value;
        }

        [XmlElement("ApplicableTenderingDeliverable")]
        public TenderingDeliverableType[] ApplicableTenderingDeliverable
        {
            get => this.applicableTenderingDeliverableField;
            set => this.applicableTenderingDeliverableField = value;
        }
    }
}