namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CropInputBatch", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CropInputBatchType
    {

        private IDType idField;

        private MeasureType weightMeasureField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
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