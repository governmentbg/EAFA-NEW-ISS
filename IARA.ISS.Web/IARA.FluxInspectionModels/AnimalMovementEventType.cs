namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AnimalMovementEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AnimalMovementEventType
    {

        private DateTimeType occurrenceDateTimeField;

        private SpecifiedClassificationType nationalApplicableSpecifiedClassificationField;

        private SpecifiedClassificationType unifiedApplicableSpecifiedClassificationField;

        public DateTimeType OccurrenceDateTime
        {
            get => this.occurrenceDateTimeField;
            set => this.occurrenceDateTimeField = value;
        }

        public SpecifiedClassificationType NationalApplicableSpecifiedClassification
        {
            get => this.nationalApplicableSpecifiedClassificationField;
            set => this.nationalApplicableSpecifiedClassificationField = value;
        }

        public SpecifiedClassificationType UnifiedApplicableSpecifiedClassification
        {
            get => this.unifiedApplicableSpecifiedClassificationField;
            set => this.unifiedApplicableSpecifiedClassificationField = value;
        }
    }
}