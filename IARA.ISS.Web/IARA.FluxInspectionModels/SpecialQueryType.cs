namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecialQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecialQueryType
    {

        private IDType idField;

        private FormattedDateTimeType submittedDateTimeField;

        private TextType[] submittingPersonNameField;

        private TextType[] subjectField;

        private TextType[] contentField;

        private FormattedDateTimeType latestResponseDateTimeField;

        private QueryResponseType[] submittedQueryResponseField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public FormattedDateTimeType SubmittedDateTime
        {
            get => this.submittedDateTimeField;
            set => this.submittedDateTimeField = value;
        }

        [XmlElement("SubmittingPersonName")]
        public TextType[] SubmittingPersonName
        {
            get => this.submittingPersonNameField;
            set => this.submittingPersonNameField = value;
        }

        [XmlElement("Subject")]
        public TextType[] Subject
        {
            get => this.subjectField;
            set => this.subjectField = value;
        }

        [XmlElement("Content")]
        public TextType[] Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        public FormattedDateTimeType LatestResponseDateTime
        {
            get => this.latestResponseDateTimeField;
            set => this.latestResponseDateTimeField = value;
        }

        [XmlElement("SubmittedQueryResponse")]
        public QueryResponseType[] SubmittedQueryResponse
        {
            get => this.submittedQueryResponseField;
            set => this.submittedQueryResponseField = value;
        }
    }
}