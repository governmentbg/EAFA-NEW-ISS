namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SubordinateBusinessType", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SubordinateBusinessTypeType
    {

        private CodeType classificationCodeField;

        private IndicatorType registrationIndicatorField;

        private NumericType preferenceOrderNumericField;

        public CodeType ClassificationCode
        {
            get => this.classificationCodeField;
            set => this.classificationCodeField = value;
        }

        public IndicatorType RegistrationIndicator
        {
            get => this.registrationIndicatorField;
            set => this.registrationIndicatorField = value;
        }

        public NumericType PreferenceOrderNumeric
        {
            get => this.preferenceOrderNumericField;
            set => this.preferenceOrderNumericField = value;
        }
    }
}