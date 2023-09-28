namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RecoveryWasteMaterial", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RecoveryWasteMaterialType
    {

        private MeasureType massRatioMeasureField;

        private MeasureType volumeRatioMeasureField;

        private AmountType estimatedValueAmountField;

        private AmountType treatmentCostAmountField;

        public MeasureType MassRatioMeasure
        {
            get => this.massRatioMeasureField;
            set => this.massRatioMeasureField = value;
        }

        public MeasureType VolumeRatioMeasure
        {
            get => this.volumeRatioMeasureField;
            set => this.volumeRatioMeasureField = value;
        }

        public AmountType EstimatedValueAmount
        {
            get => this.estimatedValueAmountField;
            set => this.estimatedValueAmountField = value;
        }

        public AmountType TreatmentCostAmount
        {
            get => this.treatmentCostAmountField;
            set => this.treatmentCostAmountField = value;
        }
    }
}