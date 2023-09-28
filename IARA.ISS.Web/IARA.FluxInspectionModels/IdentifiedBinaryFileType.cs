namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("IdentifiedBinaryFile", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class IdentifiedBinaryFileType
    {

        private IDType[] idField;

        private TextType[] titleField;

        private TextType[] authorNameField;

        private IDType[] versionIDField;

        private TextType fileNameField;

        private IDType[] uRIIDField;

        private MIMECodeType mIMECodeField;

        private CodeType encodingCodeField;

        private CodeType characterSetCodeField;

        private BinaryObjectType includedBinaryObjectField;

        private TextType[] accessField;

        private TextType[] descriptionField;

        private FileSizeUnitMeasureType sizeMeasureField;

        private DelimitedPeriodType[] accessAvailabilityDelimitedPeriodField;

        [XmlElement("ID")]
        public IDType[] ID
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

        [XmlElement("Title")]
        public TextType[] Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        [XmlElement("AuthorName")]
        public TextType[] AuthorName
        {
            get
            {
                return this.authorNameField;
            }
            set
            {
                this.authorNameField = value;
            }
        }

        [XmlElement("VersionID")]
        public IDType[] VersionID
        {
            get
            {
                return this.versionIDField;
            }
            set
            {
                this.versionIDField = value;
            }
        }

        public TextType FileName
        {
            get
            {
                return this.fileNameField;
            }
            set
            {
                this.fileNameField = value;
            }
        }

        [XmlElement("URIID")]
        public IDType[] URIID
        {
            get
            {
                return this.uRIIDField;
            }
            set
            {
                this.uRIIDField = value;
            }
        }

        public MIMECodeType MIMECode
        {
            get
            {
                return this.mIMECodeField;
            }
            set
            {
                this.mIMECodeField = value;
            }
        }

        public CodeType EncodingCode
        {
            get
            {
                return this.encodingCodeField;
            }
            set
            {
                this.encodingCodeField = value;
            }
        }

        public CodeType CharacterSetCode
        {
            get
            {
                return this.characterSetCodeField;
            }
            set
            {
                this.characterSetCodeField = value;
            }
        }

        public BinaryObjectType IncludedBinaryObject
        {
            get
            {
                return this.includedBinaryObjectField;
            }
            set
            {
                this.includedBinaryObjectField = value;
            }
        }

        [XmlElement("Access")]
        public TextType[] Access
        {
            get
            {
                return this.accessField;
            }
            set
            {
                this.accessField = value;
            }
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public FileSizeUnitMeasureType SizeMeasure
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

        [XmlElement("AccessAvailabilityDelimitedPeriod")]
        public DelimitedPeriodType[] AccessAvailabilityDelimitedPeriod
        {
            get
            {
                return this.accessAvailabilityDelimitedPeriodField;
            }
            set
            {
                this.accessAvailabilityDelimitedPeriodField = value;
            }
        }
    }
}