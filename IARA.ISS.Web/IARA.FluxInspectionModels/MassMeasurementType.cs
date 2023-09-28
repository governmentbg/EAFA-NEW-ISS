namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MassMeasurement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MassMeasurementType
    {

        private MeasureType valueMeasureField;

        private CodeType quantificationTypeCodeField;

        public MeasureType ValueMeasure
        {
            get => this.valueMeasureField;
            set => this.valueMeasureField = value;
        }

        public CodeType QuantificationTypeCode
        {
            get => this.quantificationTypeCodeField;
            set => this.quantificationTypeCodeField = value;
        }
    }
}