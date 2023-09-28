namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReceivedWasteMaterial", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReceivedWasteMaterialType
    {

        private DateTimeType receiptDateTimeField;

        private IndicatorType rejectionIndicatorField;

        private TextType rejectionReasonField;

        private MassMeasurementType massMeasurementField;

        private VolumeMeasurementType volumeMeasurementField;

        public DateTimeType ReceiptDateTime
        {
            get => this.receiptDateTimeField;
            set => this.receiptDateTimeField = value;
        }

        public IndicatorType RejectionIndicator
        {
            get => this.rejectionIndicatorField;
            set => this.rejectionIndicatorField = value;
        }

        public TextType RejectionReason
        {
            get => this.rejectionReasonField;
            set => this.rejectionReasonField = value;
        }

        public MassMeasurementType MassMeasurement
        {
            get => this.massMeasurementField;
            set => this.massMeasurementField = value;
        }

        public VolumeMeasurementType VolumeMeasurement
        {
            get => this.volumeMeasurementField;
            set => this.volumeMeasurementField = value;
        }
    }
}