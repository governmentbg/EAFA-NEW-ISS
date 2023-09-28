namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalSample", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalSampleType
    {

        private IDType intakeIDField;

        private IDType serialIDField;

        private DateTimeType lIMSRegistrationDateTimeField;

        private DateTimeType samplingDateTimeField;

        private IndicatorType sealedIndicatorField;

        private DateTimeType sealedDateTimeField;

        private TextType packagingTypeField;

        private IDType senderAssignedIDField;

        private DateTimeType receiptDateTimeField;

        private TextType typeNameField;

        private TextType informationField;

        private MeasureType[] sizeMeasureField;

        private MeasureType storageTemperatureMeasureField;

        private DateTimeType microbiologyStartDateTimeField;

        private DateTimeType preservationEndDateTimeField;

        private TextType interpretationResultField;

        private CodeType processingStatusCodeField;

        private IDType samplerAssignedIDField;

        private ReferencedLocationType[] samplingReferencedLocationField;

        private LaboratoryObservationPartyType samplingLaboratoryObservationPartyField;

        private SampleObservationResultType[] specifiedSampleObservationResultField;

        private LaboratoryObservationContractType specifiedLaboratoryObservationContractField;

        private AgriculturalSampledObjectType specifiedAgriculturalSampledObjectField;

        private AgriculturalSampleTypeType specifiedAgriculturalSampleTypeField;

        private LaboratoryObservationReferenceType[] specifiedLaboratoryObservationReferenceField;

        private LaboratoryObservationInstructionsType[] specifiedLaboratoryObservationInstructionsField;

        private SampleObservationRequestType[] specifiedSampleObservationRequestField;

        private ObservationObjectiveParameterType[] interpretationResultObservationObjectiveParameterField;

        private PhotographicPictureType[] attachedPhotographicPictureField;

        private SpecifiedBinaryFileType[] attachedSpecifiedBinaryFileField;

        private ReferencedLogisticsPackageType[] specifiedReferencedLogisticsPackageField;

        private LaboratoryObservationPartyType senderLaboratoryObservationPartyField;

        public IDType IntakeID
        {
            get
            {
                return this.intakeIDField;
            }
            set
            {
                this.intakeIDField = value;
            }
        }

        public IDType SerialID
        {
            get
            {
                return this.serialIDField;
            }
            set
            {
                this.serialIDField = value;
            }
        }

        public DateTimeType LIMSRegistrationDateTime
        {
            get
            {
                return this.lIMSRegistrationDateTimeField;
            }
            set
            {
                this.lIMSRegistrationDateTimeField = value;
            }
        }

        public DateTimeType SamplingDateTime
        {
            get
            {
                return this.samplingDateTimeField;
            }
            set
            {
                this.samplingDateTimeField = value;
            }
        }

        public IndicatorType SealedIndicator
        {
            get
            {
                return this.sealedIndicatorField;
            }
            set
            {
                this.sealedIndicatorField = value;
            }
        }

        public DateTimeType SealedDateTime
        {
            get
            {
                return this.sealedDateTimeField;
            }
            set
            {
                this.sealedDateTimeField = value;
            }
        }

        public TextType PackagingType
        {
            get
            {
                return this.packagingTypeField;
            }
            set
            {
                this.packagingTypeField = value;
            }
        }

        public IDType SenderAssignedID
        {
            get
            {
                return this.senderAssignedIDField;
            }
            set
            {
                this.senderAssignedIDField = value;
            }
        }

        public DateTimeType ReceiptDateTime
        {
            get
            {
                return this.receiptDateTimeField;
            }
            set
            {
                this.receiptDateTimeField = value;
            }
        }

        public TextType TypeName
        {
            get
            {
                return this.typeNameField;
            }
            set
            {
                this.typeNameField = value;
            }
        }

        public TextType Information
        {
            get
            {
                return this.informationField;
            }
            set
            {
                this.informationField = value;
            }
        }

        [XmlElement("SizeMeasure")]
        public MeasureType[] SizeMeasure
        {
            get
            {
                return this.sizeMeasureField;
            }
            set
            {
                this.sizeMeasureField = value;
            }
        }

        public MeasureType StorageTemperatureMeasure
        {
            get
            {
                return this.storageTemperatureMeasureField;
            }
            set
            {
                this.storageTemperatureMeasureField = value;
            }
        }

        public DateTimeType MicrobiologyStartDateTime
        {
            get
            {
                return this.microbiologyStartDateTimeField;
            }
            set
            {
                this.microbiologyStartDateTimeField = value;
            }
        }

        public DateTimeType PreservationEndDateTime
        {
            get
            {
                return this.preservationEndDateTimeField;
            }
            set
            {
                this.preservationEndDateTimeField = value;
            }
        }

        public TextType InterpretationResult
        {
            get
            {
                return this.interpretationResultField;
            }
            set
            {
                this.interpretationResultField = value;
            }
        }

        public CodeType ProcessingStatusCode
        {
            get
            {
                return this.processingStatusCodeField;
            }
            set
            {
                this.processingStatusCodeField = value;
            }
        }

        public IDType SamplerAssignedID
        {
            get
            {
                return this.samplerAssignedIDField;
            }
            set
            {
                this.samplerAssignedIDField = value;
            }
        }

        [XmlElement("SamplingReferencedLocation")]
        public ReferencedLocationType[] SamplingReferencedLocation
        {
            get
            {
                return this.samplingReferencedLocationField;
            }
            set
            {
                this.samplingReferencedLocationField = value;
            }
        }

        public LaboratoryObservationPartyType SamplingLaboratoryObservationParty
        {
            get
            {
                return this.samplingLaboratoryObservationPartyField;
            }
            set
            {
                this.samplingLaboratoryObservationPartyField = value;
            }
        }

        [XmlElement("SpecifiedSampleObservationResult")]
        public SampleObservationResultType[] SpecifiedSampleObservationResult
        {
            get
            {
                return this.specifiedSampleObservationResultField;
            }
            set
            {
                this.specifiedSampleObservationResultField = value;
            }
        }

        public LaboratoryObservationContractType SpecifiedLaboratoryObservationContract
        {
            get
            {
                return this.specifiedLaboratoryObservationContractField;
            }
            set
            {
                this.specifiedLaboratoryObservationContractField = value;
            }
        }

        public AgriculturalSampledObjectType SpecifiedAgriculturalSampledObject
        {
            get
            {
                return this.specifiedAgriculturalSampledObjectField;
            }
            set
            {
                this.specifiedAgriculturalSampledObjectField = value;
            }
        }

        public AgriculturalSampleTypeType SpecifiedAgriculturalSampleType
        {
            get
            {
                return this.specifiedAgriculturalSampleTypeField;
            }
            set
            {
                this.specifiedAgriculturalSampleTypeField = value;
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

        [XmlElement("SpecifiedSampleObservationRequest")]
        public SampleObservationRequestType[] SpecifiedSampleObservationRequest
        {
            get
            {
                return this.specifiedSampleObservationRequestField;
            }
            set
            {
                this.specifiedSampleObservationRequestField = value;
            }
        }

        [XmlElement("InterpretationResultObservationObjectiveParameter")]
        public ObservationObjectiveParameterType[] InterpretationResultObservationObjectiveParameter
        {
            get
            {
                return this.interpretationResultObservationObjectiveParameterField;
            }
            set
            {
                this.interpretationResultObservationObjectiveParameterField = value;
            }
        }

        [XmlElement("AttachedPhotographicPicture")]
        public PhotographicPictureType[] AttachedPhotographicPicture
        {
            get
            {
                return this.attachedPhotographicPictureField;
            }
            set
            {
                this.attachedPhotographicPictureField = value;
            }
        }

        [XmlElement("AttachedSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] AttachedSpecifiedBinaryFile
        {
            get
            {
                return this.attachedSpecifiedBinaryFileField;
            }
            set
            {
                this.attachedSpecifiedBinaryFileField = value;
            }
        }

        [XmlElement("SpecifiedReferencedLogisticsPackage")]
        public ReferencedLogisticsPackageType[] SpecifiedReferencedLogisticsPackage
        {
            get
            {
                return this.specifiedReferencedLogisticsPackageField;
            }
            set
            {
                this.specifiedReferencedLogisticsPackageField = value;
            }
        }

        public LaboratoryObservationPartyType SenderLaboratoryObservationParty
        {
            get
            {
                return this.senderLaboratoryObservationPartyField;
            }
            set
            {
                this.senderLaboratoryObservationPartyField = value;
            }
        }
    }
}