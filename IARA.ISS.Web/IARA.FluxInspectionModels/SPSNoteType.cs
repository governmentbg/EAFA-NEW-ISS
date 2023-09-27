namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSNote", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSNoteType
    {

        private TextType subjectField;

        private CodeType[] contentCodeField;

        private TextType[] contentField;

        private CodeType subjectCodeField;

        public TextType Subject
        {
            get => this.subjectField;
            set => this.subjectField = value;
        }

        [XmlElement("ContentCode")]
        public CodeType[] ContentCode
        {
            get => this.contentCodeField;
            set => this.contentCodeField = value;
        }

        [XmlElement("Content")]
        public TextType[] Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        public CodeType SubjectCode
        {
            get => this.subjectCodeField;
            set => this.subjectCodeField = value;
        }
    }
}