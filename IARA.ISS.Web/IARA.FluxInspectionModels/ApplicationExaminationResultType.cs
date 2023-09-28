namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ApplicationExaminationResult", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ApplicationExaminationResultType
    {

        private NumericType businessAdministrationCapabilityEvaluationPointNumericField;

        private NumericType technicalCapabilityEvaluationPointNumericField;

        private NumericType totalCapabilityEvaluationPointNumericField;

        private TextType disqualificationReasonField;

        private CodeType statusCodeField;

        private CodeType businessTypeCodeField;

        private IndicatorType approvalIndicatorField;

        private TextType conditionField;

        public NumericType BusinessAdministrationCapabilityEvaluationPointNumeric
        {
            get
            {
                return this.businessAdministrationCapabilityEvaluationPointNumericField;
            }
            set
            {
                this.businessAdministrationCapabilityEvaluationPointNumericField = value;
            }
        }

        public NumericType TechnicalCapabilityEvaluationPointNumeric
        {
            get
            {
                return this.technicalCapabilityEvaluationPointNumericField;
            }
            set
            {
                this.technicalCapabilityEvaluationPointNumericField = value;
            }
        }

        public NumericType TotalCapabilityEvaluationPointNumeric
        {
            get
            {
                return this.totalCapabilityEvaluationPointNumericField;
            }
            set
            {
                this.totalCapabilityEvaluationPointNumericField = value;
            }
        }

        public TextType DisqualificationReason
        {
            get
            {
                return this.disqualificationReasonField;
            }
            set
            {
                this.disqualificationReasonField = value;
            }
        }

        public CodeType StatusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        public CodeType BusinessTypeCode
        {
            get
            {
                return this.businessTypeCodeField;
            }
            set
            {
                this.businessTypeCodeField = value;
            }
        }

        public IndicatorType ApprovalIndicator
        {
            get
            {
                return this.approvalIndicatorField;
            }
            set
            {
                this.approvalIndicatorField = value;
            }
        }

        public TextType Condition
        {
            get
            {
                return this.conditionField;
            }
            set
            {
                this.conditionField = value;
            }
        }
    }
}