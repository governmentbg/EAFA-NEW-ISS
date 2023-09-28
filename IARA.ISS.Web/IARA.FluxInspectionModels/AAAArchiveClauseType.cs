namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAArchiveClause", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAArchiveClauseType
    {

        private IDType idField;

        private DateTimeType versionDateTimeField;

        private TextType contentField;

        private TextType breachExplanationField;

        private IndicatorType breachIndicatorField;

        private IndicatorType wordingNonStandardIndicatorField;

        private IndicatorType parametricStandardWordingIndicatorField;

        private IndicatorType metIndicatorField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public DateTimeType VersionDateTime
        {
            get
            {
                return this.versionDateTimeField;
            }
            set
            {
                this.versionDateTimeField = value;
            }
        }

        public TextType Content
        {
            get
            {
                return this.contentField;
            }
            set
            {
                this.contentField = value;
            }
        }

        public TextType BreachExplanation
        {
            get
            {
                return this.breachExplanationField;
            }
            set
            {
                this.breachExplanationField = value;
            }
        }

        public IndicatorType BreachIndicator
        {
            get
            {
                return this.breachIndicatorField;
            }
            set
            {
                this.breachIndicatorField = value;
            }
        }

        public IndicatorType WordingNonStandardIndicator
        {
            get
            {
                return this.wordingNonStandardIndicatorField;
            }
            set
            {
                this.wordingNonStandardIndicatorField = value;
            }
        }

        public IndicatorType ParametricStandardWordingIndicator
        {
            get
            {
                return this.parametricStandardWordingIndicatorField;
            }
            set
            {
                this.parametricStandardWordingIndicatorField = value;
            }
        }

        public IndicatorType MetIndicator
        {
            get
            {
                return this.metIndicatorField;
            }
            set
            {
                this.metIndicatorField = value;
            }
        }
    }
}