namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("PersonNote", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PersonNoteType
    {

        private TextType subjectField;

        private CodeType contentCodeField;

        private TextType contentField;

        private CodeType subjectCodeField;

        private IDType idField;

        private TextType nameField;

        private DateTimeType creationDateTimeField;

        public TextType Subject
        {
            get => this.subjectField;
            set => this.subjectField = value;
        }

        public CodeType ContentCode
        {
            get => this.contentCodeField;
            set => this.contentCodeField = value;
        }

        public TextType Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        public CodeType SubjectCode
        {
            get => this.subjectCodeField;
            set => this.subjectCodeField = value;
        }

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

        public DateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }
    }
}