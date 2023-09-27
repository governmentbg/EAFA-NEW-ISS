namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ContractPerformanceMeasurement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ContractPerformanceMeasurementType
    {

        private PercentType penaltyCalculationPercentField;

        private PercentType bonusCalculationPercentField;

        private AmountType basisAmountField;

        private AmountType actualAmountField;

        private AmountType calculatedAmountField;

        public PercentType PenaltyCalculationPercent
        {
            get => this.penaltyCalculationPercentField;
            set => this.penaltyCalculationPercentField = value;
        }

        public PercentType BonusCalculationPercent
        {
            get => this.bonusCalculationPercentField;
            set => this.bonusCalculationPercentField = value;
        }

        public AmountType BasisAmount
        {
            get => this.basisAmountField;
            set => this.basisAmountField = value;
        }

        public AmountType ActualAmount
        {
            get => this.actualAmountField;
            set => this.actualAmountField = value;
        }

        public AmountType CalculatedAmount
        {
            get => this.calculatedAmountField;
            set => this.calculatedAmountField = value;
        }
    }
}