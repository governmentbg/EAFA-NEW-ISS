namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("PrimitiveQueryParameter", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PrimitiveQueryParameterType
    {

        private IndicatorType applicableIndicatorField;

        private TextType comparatorField;

        private DateTimeType documentPublicationDateTimeField;

        private TextType scopeField;

        private NumericType realNumberNumericField;

        private NumericType integerNumberNumericField;

        private IndicatorType negationIndicatorField;

        private TextType keywordField;

        private CodeType typeCodeField;

        public IndicatorType ApplicableIndicator
        {
            get => this.applicableIndicatorField;
            set => this.applicableIndicatorField = value;
        }

        public TextType Comparator
        {
            get => this.comparatorField;
            set => this.comparatorField = value;
        }

        public DateTimeType DocumentPublicationDateTime
        {
            get => this.documentPublicationDateTimeField;
            set => this.documentPublicationDateTimeField = value;
        }

        public TextType Scope
        {
            get => this.scopeField;
            set => this.scopeField = value;
        }

        public NumericType RealNumberNumeric
        {
            get => this.realNumberNumericField;
            set => this.realNumberNumericField = value;
        }

        public NumericType IntegerNumberNumeric
        {
            get => this.integerNumberNumericField;
            set => this.integerNumberNumericField = value;
        }

        public IndicatorType NegationIndicator
        {
            get => this.negationIndicatorField;
            set => this.negationIndicatorField = value;
        }

        public TextType Keyword
        {
            get => this.keywordField;
            set => this.keywordField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }
    }
}