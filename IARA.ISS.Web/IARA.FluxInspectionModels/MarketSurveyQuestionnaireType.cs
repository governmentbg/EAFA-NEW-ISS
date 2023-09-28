namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MarketSurveyQuestionnaire", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MarketSurveyQuestionnaireType
    {

        private IDType[] idField;

        private TextType[] descriptionField;

        private QuestionnaireSectionType[] specifiedQuestionnaireSectionField;

        [XmlElement("ID")]
        public IDType[] ID
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

        [XmlElement("SpecifiedQuestionnaireSection")]
        public QuestionnaireSectionType[] SpecifiedQuestionnaireSection
        {
            get => this.specifiedQuestionnaireSectionField;
            set => this.specifiedQuestionnaireSectionField = value;
        }
    }
}