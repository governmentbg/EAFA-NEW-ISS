namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SampleObservationRequest", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SampleObservationRequestType
    {

        private IDType idField;

        private TextType materialTypeField;

        private TextType generalCharacteristicField;

        private DateTimeType requestedObservationStartDateTimeField;

        private DateTimeType requestedObservationEndDateTimeField;

        private TextType observationTimeFrameField;

        private IndicatorType emergencyObservationIndicatorField;

        private IndicatorType outsourcedObservationIndicatorField;

        private CodeType materialTypeCodeField;

        private LaboratoryObservationReferenceType[] specifiedLaboratoryObservationReferenceField;

        private LaboratoryObservationPartyType authorizationLaboratoryObservationPartyField;

        private LaboratoryObservationNoteType[] attachedLaboratoryObservationNoteField;

        private LaboratoryObservationAnalysisMethodType[] requestedLaboratoryObservationAnalysisMethodField;

        private SampleObservationRequestCharacteristicType[] minimumStandardValueSpecifiedSampleObservationRequestCharacteristicField;

        private SampleObservationRequestCharacteristicType[] maximumStandardValueSpecifiedSampleObservationRequestCharacteristicField;

        private SampleObservationRequestCharacteristicType[] expectedSpecifiedSampleObservationRequestCharacteristicField;

        private LaboratoryObservationInstructionsType[] specifiedLaboratoryObservationInstructionsField;

        private LaboratoryObservationPartyType outsourcedLaboratoryObservationPartyField;

        private ObservationObjectiveParameterType[] applicableObservationObjectiveParameterField;

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

        public DateTimeType RequestedObservationStartDateTime
        {
            get
            {
                return this.requestedObservationStartDateTimeField;
            }
            set
            {
                this.requestedObservationStartDateTimeField = value;
            }
        }

        public DateTimeType RequestedObservationEndDateTime
        {
            get
            {
                return this.requestedObservationEndDateTimeField;
            }
            set
            {
                this.requestedObservationEndDateTimeField = value;
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

        [XmlElement("RequestedLaboratoryObservationAnalysisMethod")]
        public LaboratoryObservationAnalysisMethodType[] RequestedLaboratoryObservationAnalysisMethod
        {
            get
            {
                return this.requestedLaboratoryObservationAnalysisMethodField;
            }
            set
            {
                this.requestedLaboratoryObservationAnalysisMethodField = value;
            }
        }

        [XmlElement("MinimumStandardValueSpecifiedSampleObservationRequestCharacteristic")]
        public SampleObservationRequestCharacteristicType[] MinimumStandardValueSpecifiedSampleObservationRequestCharacteristic
        {
            get
            {
                return this.minimumStandardValueSpecifiedSampleObservationRequestCharacteristicField;
            }
            set
            {
                this.minimumStandardValueSpecifiedSampleObservationRequestCharacteristicField = value;
            }
        }

        [XmlElement("MaximumStandardValueSpecifiedSampleObservationRequestCharacteristic")]
        public SampleObservationRequestCharacteristicType[] MaximumStandardValueSpecifiedSampleObservationRequestCharacteristic
        {
            get
            {
                return this.maximumStandardValueSpecifiedSampleObservationRequestCharacteristicField;
            }
            set
            {
                this.maximumStandardValueSpecifiedSampleObservationRequestCharacteristicField = value;
            }
        }

        [XmlElement("ExpectedSpecifiedSampleObservationRequestCharacteristic")]
        public SampleObservationRequestCharacteristicType[] ExpectedSpecifiedSampleObservationRequestCharacteristic
        {
            get
            {
                return this.expectedSpecifiedSampleObservationRequestCharacteristicField;
            }
            set
            {
                this.expectedSpecifiedSampleObservationRequestCharacteristicField = value;
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

        [XmlElement("ApplicableObservationObjectiveParameter")]
        public ObservationObjectiveParameterType[] ApplicableObservationObjectiveParameter
        {
            get
            {
                return this.applicableObservationObjectiveParameterField;
            }
            set
            {
                this.applicableObservationObjectiveParameterField = value;
            }
        }
    }
}