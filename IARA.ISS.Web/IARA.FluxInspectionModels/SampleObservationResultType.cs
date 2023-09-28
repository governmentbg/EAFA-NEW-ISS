namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SampleObservationResult", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SampleObservationResultType
    {

        private IDType idField;

        private TextType materialTypeField;

        private TextType generalCharacteristicField;

        private DateTimeType actualObservationStartDateTimeField;

        private DateTimeType actualObservationEndDateTimeField;

        private TextType observationTimeFrameField;

        private TextType observationDiscontinuationReasonField;

        private IndicatorType emergencyObservationIndicatorField;

        private IndicatorType outsourcedObservationIndicatorField;

        private CodeType observationDiscontinuationReasonCodeField;

        private CodeType materialTypeCodeField;

        private LaboratoryObservationReferenceType[] specifiedLaboratoryObservationReferenceField;

        private LaboratoryObservationPartyType authorizationLaboratoryObservationPartyField;

        private LaboratoryObservationNoteType[] attachedLaboratoryObservationNoteField;

        private LaboratoryObservationAnalysisMethodType[] usedLaboratoryObservationAnalysisMethodField;

        private SampleObservationResultCharacteristicType[] minimumStandardValueSpecifiedSampleObservationResultCharacteristicField;

        private SampleObservationResultCharacteristicType[] maximumStandardValueSpecifiedSampleObservationResultCharacteristicField;

        private SampleObservationResultCharacteristicType[] observedValueSpecifiedSampleObservationResultCharacteristicField;

        private SampleObservationResultCharacteristicType[] expectedValueSpecifiedSampleObservationResultCharacteristicField;

        private LaboratoryObservationInstructionsType[] specifiedLaboratoryObservationInstructionsField;

        private LaboratoryObservationReferenceType[] laboratoryAnalysisRequestSpecifiedLaboratoryObservationReferenceField;

        private LaboratoryObservationPartyType outsourcedLaboratoryObservationPartyField;

        private ObservationObjectiveParameterType[] interpretationResultApplicableObservationObjectiveParameterField;

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

        public TextType MaterialType
        {
            get
            {
                return this.materialTypeField;
            }
            set
            {
                this.materialTypeField = value;
            }
        }

        public TextType GeneralCharacteristic
        {
            get
            {
                return this.generalCharacteristicField;
            }
            set
            {
                this.generalCharacteristicField = value;
            }
        }

        public DateTimeType ActualObservationStartDateTime
        {
            get
            {
                return this.actualObservationStartDateTimeField;
            }
            set
            {
                this.actualObservationStartDateTimeField = value;
            }
        }

        public DateTimeType ActualObservationEndDateTime
        {
            get
            {
                return this.actualObservationEndDateTimeField;
            }
            set
            {
                this.actualObservationEndDateTimeField = value;
            }
        }

        public TextType ObservationTimeFrame
        {
            get
            {
                return this.observationTimeFrameField;
            }
            set
            {
                this.observationTimeFrameField = value;
            }
        }

        public TextType ObservationDiscontinuationReason
        {
            get
            {
                return this.observationDiscontinuationReasonField;
            }
            set
            {
                this.observationDiscontinuationReasonField = value;
            }
        }

        public IndicatorType EmergencyObservationIndicator
        {
            get
            {
                return this.emergencyObservationIndicatorField;
            }
            set
            {
                this.emergencyObservationIndicatorField = value;
            }
        }

        public IndicatorType OutsourcedObservationIndicator
        {
            get
            {
                return this.outsourcedObservationIndicatorField;
            }
            set
            {
                this.outsourcedObservationIndicatorField = value;
            }
        }

        public CodeType ObservationDiscontinuationReasonCode
        {
            get
            {
                return this.observationDiscontinuationReasonCodeField;
            }
            set
            {
                this.observationDiscontinuationReasonCodeField = value;
            }
        }

        public CodeType MaterialTypeCode
        {
            get
            {
                return this.materialTypeCodeField;
            }
            set
            {
                this.materialTypeCodeField = value;
            }
        }

        [XmlElement("SpecifiedLaboratoryObservationReference")]
        public LaboratoryObservationReferenceType[] SpecifiedLaboratoryObservationReference
        {
            get
            {
                return this.specifiedLaboratoryObservationReferenceField;
            }
            set
            {
                this.specifiedLaboratoryObservationReferenceField = value;
            }
        }

        public LaboratoryObservationPartyType AuthorizationLaboratoryObservationParty
        {
            get
            {
                return this.authorizationLaboratoryObservationPartyField;
            }
            set
            {
                this.authorizationLaboratoryObservationPartyField = value;
            }
        }

        [XmlElement("AttachedLaboratoryObservationNote")]
        public LaboratoryObservationNoteType[] AttachedLaboratoryObservationNote
        {
            get
            {
                return this.attachedLaboratoryObservationNoteField;
            }
            set
            {
                this.attachedLaboratoryObservationNoteField = value;
            }
        }

        [XmlElement("UsedLaboratoryObservationAnalysisMethod")]
        public LaboratoryObservationAnalysisMethodType[] UsedLaboratoryObservationAnalysisMethod
        {
            get
            {
                return this.usedLaboratoryObservationAnalysisMethodField;
            }
            set
            {
                this.usedLaboratoryObservationAnalysisMethodField = value;
            }
        }

        [XmlElement("MinimumStandardValueSpecifiedSampleObservationResultCharacteristic")]
        public SampleObservationResultCharacteristicType[] MinimumStandardValueSpecifiedSampleObservationResultCharacteristic
        {
            get
            {
                return this.minimumStandardValueSpecifiedSampleObservationResultCharacteristicField;
            }
            set
            {
                this.minimumStandardValueSpecifiedSampleObservationResultCharacteristicField = value;
            }
        }

        [XmlElement("MaximumStandardValueSpecifiedSampleObservationResultCharacteristic")]
        public SampleObservationResultCharacteristicType[] MaximumStandardValueSpecifiedSampleObservationResultCharacteristic
        {
            get
            {
                return this.maximumStandardValueSpecifiedSampleObservationResultCharacteristicField;
            }
            set
            {
                this.maximumStandardValueSpecifiedSampleObservationResultCharacteristicField = value;
            }
        }

        [XmlElement("ObservedValueSpecifiedSampleObservationResultCharacteristic")]
        public SampleObservationResultCharacteristicType[] ObservedValueSpecifiedSampleObservationResultCharacteristic
        {
            get
            {
                return this.observedValueSpecifiedSampleObservationResultCharacteristicField;
            }
            set
            {
                this.observedValueSpecifiedSampleObservationResultCharacteristicField = value;
            }
        }

        [XmlElement("ExpectedValueSpecifiedSampleObservationResultCharacteristic")]
        public SampleObservationResultCharacteristicType[] ExpectedValueSpecifiedSampleObservationResultCharacteristic
        {
            get
            {
                return this.expectedValueSpecifiedSampleObservationResultCharacteristicField;
            }
            set
            {
                this.expectedValueSpecifiedSampleObservationResultCharacteristicField = value;
            }
        }

        [XmlElement("SpecifiedLaboratoryObservationInstructions")]
        public LaboratoryObservationInstructionsType[] SpecifiedLaboratoryObservationInstructions
        {
            get
            {
                return this.specifiedLaboratoryObservationInstructionsField;
            }
            set
            {
                this.specifiedLaboratoryObservationInstructionsField = value;
            }
        }

        [XmlElement("LaboratoryAnalysisRequestSpecifiedLaboratoryObservationReference")]
        public LaboratoryObservationReferenceType[] LaboratoryAnalysisRequestSpecifiedLaboratoryObservationReference
        {
            get
            {
                return this.laboratoryAnalysisRequestSpecifiedLaboratoryObservationReferenceField;
            }
            set
            {
                this.laboratoryAnalysisRequestSpecifiedLaboratoryObservationReferenceField = value;
            }
        }

        public LaboratoryObservationPartyType OutsourcedLaboratoryObservationParty
        {
            get
            {
                return this.outsourcedLaboratoryObservationPartyField;
            }
            set
            {
                this.outsourcedLaboratoryObservationPartyField = value;
            }
        }

        [XmlElement("InterpretationResultApplicableObservationObjectiveParameter")]
        public ObservationObjectiveParameterType[] InterpretationResultApplicableObservationObjectiveParameter
        {
            get
            {
                return this.interpretationResultApplicableObservationObjectiveParameterField;
            }
            set
            {
                this.interpretationResultApplicableObservationObjectiveParameterField = value;
            }
        }
    }
}