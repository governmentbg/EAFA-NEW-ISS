namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalProcessCondition", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalProcessConditionType
    {

        private CodeType typeCodeField;

        private CodeType subordinateTypeCodeField;

        private MeasureType valueMeasureField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public CodeType SubordinateTypeCode
        {
            get => this.subordinateTypeCodeField;
            set => this.subordinateTypeCodeField = value;
        }

        public MeasureType ValueMeasure
        {
            get => this.valueMeasureField;
            set => this.valueMeasureField = value;
        }
    }
}