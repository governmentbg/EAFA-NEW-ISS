namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LaboratoryObservationContract", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LaboratoryObservationContractType
    {

        private IDType idField;

        private IDType observerAssignedIDField;

        private CodeType typeCodeField;

        private CodeType[] observationTypeCodeField;

        private TextType[] observationNameField;

        private TextType[] observationCharacteristicDescriptionField;

        private TextType submissionReasonDescriptionField;

        private DateTimeType issueDateTimeField;

        private DateType observationEndDateField;

        private DateType lastItemDeliveryDateField;

        private TextType observationInformationField;

        private IDType projectIDField;

        private DateTimeType observationReportPublicationDateTimeField;

        private DateTimeType requestedObservationReportPublicationDateTimeField;

        private MeasureType standardObservationLeadTimeMeasureField;

        private MeasureType emergencyObservationLeadTimeMeasureField;

        private TextType[] emergencyObservationConditionInformationField;

        private IndicatorType gMORegulationIndicatorField;

        private LaboratoryObservationNoteType specifiedLaboratoryObservationNoteField;

        private LaboratoryObservationPartyType orderingLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType observingLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType invoiceeLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType contractSenderLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType issuerLaboratoryObservationPartyField;

        private LaboratoryObservationReferenceType[] specifiedLaboratoryObservationReferenceField;

        private LaboratoryObservationInstructionsType[] specifiedLaboratoryObservationInstructionsField;

        private LaboratoryObservationReferenceType[] orderSpecifiedLaboratoryObservationReferenceField;

        private LaboratoryObservationAnalysisMethodType[] agreedAssociatedLaboratoryObservationAnalysisMethodField;

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

        public IDType ObserverAssignedID
        {
            get
            {
                return this.observerAssignedIDField;
            }
            set
            {
                this.observerAssignedIDField = value;
            }
        }

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        [XmlElement("ObservationTypeCode")]
        public CodeType[] ObservationTypeCode
        {
            get
            {
                return this.observationTypeCodeField;
            }
            set
            {
                this.observationTypeCodeField = value;
            }
        }

        [XmlElement("ObservationName")]
        public TextType[] ObservationName
        {
            get
            {
                return this.observationNameField;
            }
            set
            {
                this.observationNameField = value;
            }
        }

        [XmlElement("ObservationCharacteristicDescription")]
        public TextType[] ObservationCharacteristicDescription
        {
            get
            {
                return this.observationCharacteristicDescriptionField;
            }
            set
            {
                this.observationCharacteristicDescriptionField = value;
            }
        }

        public TextType SubmissionReasonDescription
        {
            get
            {
                return this.submissionReasonDescriptionField;
            }
            set
            {
                this.submissionReasonDescriptionField = value;
            }
        }

        public DateTimeType IssueDateTime
        {
            get
            {
                return this.issueDateTimeField;
            }
            set
            {
                this.issueDateTimeField = value;
            }
        }

        public DateType ObservationEndDate
        {
            get
            {
                return this.observationEndDateField;
            }
            set
            {
                this.observationEndDateField = value;
            }
        }

        public DateType LastItemDeliveryDate
        {
            get
            {
                return this.lastItemDeliveryDateField;
            }
            set
            {
                this.lastItemDeliveryDateField = value;
            }
        }

        public TextType ObservationInformation
        {
            get
            {
                return this.observationInformationField;
            }
            set
            {
                this.observationInformationField = value;
            }
        }

        public IDType ProjectID
        {
            get
            {
                return this.projectIDField;
            }
            set
            {
                this.projectIDField = value;
            }
        }

        public DateTimeType ObservationReportPublicationDateTime
        {
            get
            {
                return this.observationReportPublicationDateTimeField;
            }
            set
            {
                this.observationReportPublicationDateTimeField = value;
            }
        }

        public DateTimeType RequestedObservationReportPublicationDateTime
        {
            get
            {
                return this.requestedObservationReportPublicationDateTimeField;
            }
            set
            {
                this.requestedObservationReportPublicationDateTimeField = value;
            }
        }

        public MeasureType StandardObservationLeadTimeMeasure
        {
            get
            {
                return this.standardObservationLeadTimeMeasureField;
            }
            set
            {
                this.standardObservationLeadTimeMeasureField = value;
            }
        }

        public MeasureType EmergencyObservationLeadTimeMeasure
        {
            get
            {
                return this.emergencyObservationLeadTimeMeasureField;
            }
            set
            {
                this.emergencyObservationLeadTimeMeasureField = value;
            }
        }

        [XmlElement("EmergencyObservationConditionInformation")]
        public TextType[] EmergencyObservationConditionInformation
        {
            get
            {
                return this.emergencyObservationConditionInformationField;
            }
            set
            {
                this.emergencyObservationConditionInformationField = value;
            }
        }

        public IndicatorType GMORegulationIndicator
        {
            get
            {
                return this.gMORegulationIndicatorField;
            }
            set
            {
                this.gMORegulationIndicatorField = value;
            }
        }

        public LaboratoryObservationNoteType SpecifiedLaboratoryObservationNote
        {
            get
            {
                return this.specifiedLaboratoryObservationNoteField;
            }
            set
            {
                this.specifiedLaboratoryObservationNoteField = value;
            }
        }

        public LaboratoryObservationPartyType OrderingLaboratoryObservationParty
        {
            get
            {
                return this.orderingLaboratoryObservationPartyField;
            }
            set
            {
                this.orderingLaboratoryObservationPartyField = value;
            }
        }

        public LaboratoryObservationPartyType ObservingLaboratoryObservationParty
        {
            get
            {
                return this.observingLaboratoryObservationPartyField;
            }
            set
            {
                this.observingLaboratoryObservationPartyField = value;
            }
        }

        public LaboratoryObservationPartyType InvoiceeLaboratoryObservationParty
        {
            get
            {
                return this.invoiceeLaboratoryObservationPartyField;
            }
            set
            {
                this.invoiceeLaboratoryObservationPartyField = value;
            }
        }

        public LaboratoryObservationPartyType ContractSenderLaboratoryObservationParty
        {
            get
            {
                return this.contractSenderLaboratoryObservationPartyField;
            }
            set
            {
                this.contractSenderLaboratoryObservationPartyField = value;
            }
        }

        public LaboratoryObservationPartyType IssuerLaboratoryObservationParty
        {
            get
            {
                return this.issuerLaboratoryObservationPartyField;
            }
            set
            {
                this.issuerLaboratoryObservationPartyField = value;
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

        [XmlElement("OrderSpecifiedLaboratoryObservationReference")]
        public LaboratoryObservationReferenceType[] OrderSpecifiedLaboratoryObservationReference
        {
            get
            {
                return this.orderSpecifiedLaboratoryObservationReferenceField;
            }
            set
            {
                this.orderSpecifiedLaboratoryObservationReferenceField = value;
            }
        }

        [XmlElement("AgreedAssociatedLaboratoryObservationAnalysisMethod")]
        public LaboratoryObservationAnalysisMethodType[] AgreedAssociatedLaboratoryObservationAnalysisMethod
        {
            get
            {
                return this.agreedAssociatedLaboratoryObservationAnalysisMethodField;
            }
            set
            {
                this.agreedAssociatedLaboratoryObservationAnalysisMethodField = value;
            }
        }
    }
}