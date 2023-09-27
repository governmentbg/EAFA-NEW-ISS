namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReferencedWasteTransportMovement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReferencedWasteTransportMovementType
    {

        private NumericType sequenceNumericField;

        private IDType notificationIDField;

        public NumericType SequenceNumeric
        {
            get => this.sequenceNumericField;
            set => this.sequenceNumericField = value;
        }

        public IDType NotificationID
        {
            get => this.notificationIDField;
            set => this.notificationIDField = value;
        }
    }
}