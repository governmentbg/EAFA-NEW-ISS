namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectNote", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectNoteType
    {

        private IDType idField;

        private TextType nameField;

        private TextType contentField;

        private DateTimeType creationDateTimeField;

        private CodeType contentCodeField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        public DateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }

        public CodeType ContentCode
        {
            get => this.contentCodeField;
            set => this.contentCodeField = value;
        }
    }
}