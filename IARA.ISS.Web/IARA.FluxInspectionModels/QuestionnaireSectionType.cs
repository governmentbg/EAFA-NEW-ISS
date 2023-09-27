namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("QuestionnaireSection", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class QuestionnaireSectionType
    {

        private IDType[] idField;

        private TextType[] descriptionField;

        private SubjectClassificationType[] specifiedSubjectClassificationField;

        private CIReferencedDocumentType briefingReferencedCIReferencedDocumentField;

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

        [XmlElement("SpecifiedSubjectClassification")]
        public SubjectClassificationType[] SpecifiedSubjectClassification
        {
            get => this.specifiedSubjectClassificationField;
            set => this.specifiedSubjectClassificationField = value;
        }

        public CIReferencedDocumentType BriefingReferencedCIReferencedDocument
        {
            get => this.briefingReferencedCIReferencedDocumentField;
            set => this.briefingReferencedCIReferencedDocumentField = value;
        }
    }
}