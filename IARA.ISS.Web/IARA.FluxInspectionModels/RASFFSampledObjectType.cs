namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFSampledObject", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFSampledObjectType
    {

        private CodeType samplingLocationTypeCodeField;

        private DateTimeType sampleProductionDateTimeField;

        private RASFFNoteType specifiedRASFFNoteField;

        private RASFFAnalysisType[] conductedRASFFAnalysisField;

        public CodeType SamplingLocationTypeCode
        {
            get => this.samplingLocationTypeCodeField;
            set => this.samplingLocationTypeCodeField = value;
        }

        public DateTimeType SampleProductionDateTime
        {
            get => this.sampleProductionDateTimeField;
            set => this.sampleProductionDateTimeField = value;
        }

        public RASFFNoteType SpecifiedRASFFNote
        {
            get => this.specifiedRASFFNoteField;
            set => this.specifiedRASFFNoteField = value;
        }

        [XmlElement("ConductedRASFFAnalysis")]
        public RASFFAnalysisType[] ConductedRASFFAnalysis
        {
            get => this.conductedRASFFAnalysisField;
            set => this.conductedRASFFAnalysisField = value;
        }
    }
}