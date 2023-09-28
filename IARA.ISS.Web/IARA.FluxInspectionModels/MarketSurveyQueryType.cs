namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MarketSurveyQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MarketSurveyQueryType
    {

        private IDType[] idField;

        private CodeType[] typeCodeField;

        private TextType[] multiChoiceAnswerField;

        private TextType[] contentField;

        private SubjectClassificationType specifiedSubjectClassificationField;

        private QueryResponseType[] submittedQueryResponseField;

        private QuestionnaireSectionType referencedQuestionnaireSectionField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("MultiChoiceAnswer")]
        public TextType[] MultiChoiceAnswer
        {
            get => this.multiChoiceAnswerField;
            set => this.multiChoiceAnswerField = value;
        }

        [XmlElement("Content")]
        public TextType[] Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        public SubjectClassificationType SpecifiedSubjectClassification
        {
            get => this.specifiedSubjectClassificationField;
            set => this.specifiedSubjectClassificationField = value;
        }

        [XmlElement("SubmittedQueryResponse")]
        public QueryResponseType[] SubmittedQueryResponse
        {
            get => this.submittedQueryResponseField;
            set => this.submittedQueryResponseField = value;
        }

        public QuestionnaireSectionType ReferencedQuestionnaireSection
        {
            get => this.referencedQuestionnaireSectionField;
            set => this.referencedQuestionnaireSectionField = value;
        }
    }
}