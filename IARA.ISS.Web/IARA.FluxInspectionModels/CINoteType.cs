namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CINote", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CINoteType
    {

        private TextType subjectField;

        private CodeType contentCodeField;

        private TextType[] contentField;

        private SubjectCodeType subjectCodeField;

        private IDType idField;

        public TextType Subject
        {
            get
            {
                return this.subjectField;
            }
            set
            {
                this.subjectField = value;
            }
        }

        public CodeType ContentCode
        {
            get
            {
                return this.contentCodeField;
            }
            set
            {
                this.contentCodeField = value;
            }
        }

        [XmlElement("Content")]
        public TextType[] Content
        {
            get
            {
                return this.contentField;
            }
            set
            {
                this.contentField = value;
            }
        }

        public SubjectCodeType SubjectCode
        {
            get
            {
                return this.subjectCodeField;
            }
            set
            {
                this.subjectCodeField = value;
            }
        }

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
    }
}