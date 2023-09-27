namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TravelProductIntermediarySale", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TravelProductIntermediarySaleType
    {

        private IndicatorType guaranteeRequiredIndicatorField;

        private TextType guaranteeField;

        private IndicatorType commissionPayableIndicatorField;

        private PercentType commissionPercentField;

        private AmountType commissionAmountField;

        private CodeType deadlineCodeField;

        private TextType descriptionField;

        public IndicatorType GuaranteeRequiredIndicator
        {
            get => this.guaranteeRequiredIndicatorField;
            set => this.guaranteeRequiredIndicatorField = value;
        }

        public TextType Guarantee
        {
            get => this.guaranteeField;
            set => this.guaranteeField = value;
        }

        public IndicatorType CommissionPayableIndicator
        {
            get => this.commissionPayableIndicatorField;
            set => this.commissionPayableIndicatorField = value;
        }

        public PercentType CommissionPercent
        {
            get => this.commissionPercentField;
            set => this.commissionPercentField = value;
        }

        public AmountType CommissionAmount
        {
            get => this.commissionAmountField;
            set => this.commissionAmountField = value;
        }

        public CodeType DeadlineCode
        {
            get => this.deadlineCodeField;
            set => this.deadlineCodeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}