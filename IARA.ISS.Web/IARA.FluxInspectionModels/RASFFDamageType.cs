namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFDamage", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFDamageType
    {

        private IndicatorType severityIndicatorField;

        private CodeType affectedSubjectCodeField;

        private TextType[] severityJustificationField;

        private NumericType affectedCaseNumericField;

        private TextType[] symptomField;

        private RASFFHealthHazardType observedRASFFHealthHazardField;

        public IndicatorType SeverityIndicator
        {
            get => this.severityIndicatorField;
            set => this.severityIndicatorField = value;
        }

        public CodeType AffectedSubjectCode
        {
            get => this.affectedSubjectCodeField;
            set => this.affectedSubjectCodeField = value;
        }

        [XmlElement("SeverityJustification")]
        public TextType[] SeverityJustification
        {
            get => this.severityJustificationField;
            set => this.severityJustificationField = value;
        }

        public NumericType AffectedCaseNumeric
        {
            get => this.affectedCaseNumericField;
            set => this.affectedCaseNumericField = value;
        }

        [XmlElement("Symptom")]
        public TextType[] Symptom
        {
            get => this.symptomField;
            set => this.symptomField = value;
        }

        public RASFFHealthHazardType ObservedRASFFHealthHazard
        {
            get => this.observedRASFFHealthHazardField;
            set => this.observedRASFFHealthHazardField = value;
        }
    }
}