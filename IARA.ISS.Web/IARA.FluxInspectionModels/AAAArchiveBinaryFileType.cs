namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAArchiveBinaryFile", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAArchiveBinaryFileType
    {

        private IDType idField;

        private TextType titleField;

        private TextType authorNameField;

        private IDType versionIDField;

        private TextType fileNameField;

        private BinaryObjectType includedBinaryObjectField;

        private TextType accessField;

        private TextType descriptionField;

        private MeasureType sizeMeasureField;

        private AAAPeriodType[] accessAvailabilityAAAPeriodField;

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

        public TextType Title
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

        public TextType AuthorName
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

        public IDType VersionID
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

        public TextType Access
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

        public TextType Description
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

        public MeasureType SizeMeasure
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

        [XmlElement("AccessAvailabilityAAAPeriod")]
        public AAAPeriodType[] AccessAvailabilityAAAPeriod
        {
            get
            {
                return this.accessAvailabilityAAAPeriodField;
            }
            set
            {
                this.accessAvailabilityAAAPeriodField = value;
            }
        }
    }
}