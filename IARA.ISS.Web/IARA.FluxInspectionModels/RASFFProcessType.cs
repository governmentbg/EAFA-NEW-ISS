namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFProcess", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFProcessType
    {

        private IDType idField;

        private TextType[] descriptionField;

        private CodeType typeCodeField;

        private DateTimeType entryIntoForceStartDateTimeField;

        private CodeType categoryCodeField;

        private TextType resourceIdentificationField;

        private IDType uRIIDField;

        private CodeType resourceTypeCodeField;

        private AvailablePeriodType completionAvailablePeriodField;

        private RASFFCountryType operationRASFFCountryField;

        private RASFFNoteType[] additionalInformationRASFFNoteField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DateTimeType EntryIntoForceStartDateTime
        {
            get => this.entryIntoForceStartDateTimeField;
            set => this.entryIntoForceStartDateTimeField = value;
        }

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public TextType ResourceIdentification
        {
            get => this.resourceIdentificationField;
            set => this.resourceIdentificationField = value;
        }

        public IDType URIID
        {
            get => this.uRIIDField;
            set => this.uRIIDField = value;
        }

        public CodeType ResourceTypeCode
        {
            get => this.resourceTypeCodeField;
            set => this.resourceTypeCodeField = value;
        }

        public AvailablePeriodType CompletionAvailablePeriod
        {
            get => this.completionAvailablePeriodField;
            set => this.completionAvailablePeriodField = value;
        }

        public RASFFCountryType OperationRASFFCountry
        {
            get => this.operationRASFFCountryField;
            set => this.operationRASFFCountryField = value;
        }

        [XmlElement("AdditionalInformationRASFFNote")]
        public RASFFNoteType[] AdditionalInformationRASFFNote
        {
            get => this.additionalInformationRASFFNoteField;
            set => this.additionalInformationRASFFNoteField = value;
        }
    }
}