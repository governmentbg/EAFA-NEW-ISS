namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedPreference", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedPreferenceType
    {

        private NumericType priorityRankingNumericField;

        private IndicatorType preferredIndicatorField;

        private SpecifiedPeriodType unavailableSpecifiedPeriodField;

        private SpecifiedPeriodType availableSpecifiedPeriodField;

        public NumericType PriorityRankingNumeric
        {
            get => this.priorityRankingNumericField;
            set => this.priorityRankingNumericField = value;
        }

        public IndicatorType PreferredIndicator
        {
            get => this.preferredIndicatorField;
            set => this.preferredIndicatorField = value;
        }

        public SpecifiedPeriodType UnavailableSpecifiedPeriod
        {
            get => this.unavailableSpecifiedPeriodField;
            set => this.unavailableSpecifiedPeriodField = value;
        }

        public SpecifiedPeriodType AvailableSpecifiedPeriod
        {
            get => this.availableSpecifiedPeriodField;
            set => this.availableSpecifiedPeriodField = value;
        }
    }
}