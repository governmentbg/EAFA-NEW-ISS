namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICHExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICHExchangedDocumentType
    {

        private IDType idField;

        private TextType[] nameField;

        private DocumentCodeType typeCodeField;

        private DateTimeType issueDateTimeField;

        private IndicatorType copyIndicatorField;

        private IDType[] languageIDField;

        private NumericType lineCountNumericField;

        private MessageFunctionCodeType purposeCodeField;

        private DateTimeType revisionDateTimeField;

        private IDType globalIDField;

        private IDType revisionIDField;

        private IDType previousRevisionIDField;

        private CINoteType[] includedCINoteField;

        private CISpecifiedPeriodType effectiveCISpecifiedPeriodField;

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

        [XmlElement("Name")]
        public TextType[] Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public DocumentCodeType TypeCode
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

        public IndicatorType CopyIndicator
        {
            get
            {
                return this.copyIndicatorField;
            }
            set
            {
                this.copyIndicatorField = value;
            }
        }

        [XmlElement("LanguageID")]
        public IDType[] LanguageID
        {
            get
            {
                return this.languageIDField;
            }
            set
            {
                this.languageIDField = value;
            }
        }

        public NumericType LineCountNumeric
        {
            get
            {
                return this.lineCountNumericField;
            }
            set
            {
                this.lineCountNumericField = value;
            }
        }

        public MessageFunctionCodeType PurposeCode
        {
            get
            {
                return this.purposeCodeField;
            }
            set
            {
                this.purposeCodeField = value;
            }
        }

        public DateTimeType RevisionDateTime
        {
            get
            {
                return this.revisionDateTimeField;
            }
            set
            {
                this.revisionDateTimeField = value;
            }
        }

        public IDType GlobalID
        {
            get
            {
                return this.globalIDField;
            }
            set
            {
                this.globalIDField = value;
            }
        }

        public IDType RevisionID
        {
            get
            {
                return this.revisionIDField;
            }
            set
            {
                this.revisionIDField = value;
            }
        }

        public IDType PreviousRevisionID
        {
            get
            {
                return this.previousRevisionIDField;
            }
            set
            {
                this.previousRevisionIDField = value;
            }
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get
            {
                return this.includedCINoteField;
            }
            set
            {
                this.includedCINoteField = value;
            }
        }

        public CISpecifiedPeriodType EffectiveCISpecifiedPeriod
        {
            get
            {
                return this.effectiveCISpecifiedPeriodField;
            }
            set
            {
                this.effectiveCISpecifiedPeriodField = value;
            }
        }
    }
}