namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SampledObjectIllness", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SampledObjectIllnessType
    {

        private DateTimeType startDateTimeField;

        private TextType symptomField;

        private TextType distributionLevelField;

        private TextType affectedPartField;

        private IndicatorType increasingIndicatorField;

        public DateTimeType StartDateTime
        {
            get
            {
                return this.startDateTimeField;
            }
            set
            {
                this.startDateTimeField = value;
            }
        }

        public TextType Symptom
        {
            get
            {
                return this.symptomField;
            }
            set
            {
                this.symptomField = value;
            }
        }

        public TextType DistributionLevel
        {
            get
            {
                return this.distributionLevelField;
            }
            set
            {
                this.distributionLevelField = value;
            }
        }

        public TextType AffectedPart
        {
            get
            {
                return this.affectedPartField;
            }
            set
            {
                this.affectedPartField = value;
            }
        }

        public IndicatorType IncreasingIndicator
        {
            get
            {
                return this.increasingIndicatorField;
            }
            set
            {
                this.increasingIndicatorField = value;
            }
        }
    }
}