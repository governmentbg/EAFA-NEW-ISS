namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProduceBatch", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProduceBatchType
    {

        private IDType collectorAssignedIDField;

        private IDType farmerAssignedIDField;

        private MeasureType weightMeasureField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

        public IDType CollectorAssignedID
        {
            get => this.collectorAssignedIDField;
            set => this.collectorAssignedIDField = value;
        }

        public IDType FarmerAssignedID
        {
            get => this.farmerAssignedIDField;
            set => this.farmerAssignedIDField = value;
        }

        public MeasureType WeightMeasure
        {
            get => this.weightMeasureField;
            set => this.weightMeasureField = value;
        }

        [XmlElement("ApplicableTechnicalCharacteristic")]
        public TechnicalCharacteristicType[] ApplicableTechnicalCharacteristic
        {
            get => this.applicableTechnicalCharacteristicField;
            set => this.applicableTechnicalCharacteristicField = value;
        }
    }
}