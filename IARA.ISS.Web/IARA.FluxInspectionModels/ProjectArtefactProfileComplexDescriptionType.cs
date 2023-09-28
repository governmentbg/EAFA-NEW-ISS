namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectArtefactProfileComplexDescription", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectArtefactProfileComplexDescriptionType
    {

        private TextType[] contentField;

        private ProjectArtefactProfileComplexDescriptionType[] subsetProjectArtefactProfileComplexDescriptionField;

        private ProjectArtefactIdentityType registrationProjectArtefactIdentityField;

        private SummaryDocumentType responseSummaryDocumentField;

        [XmlElement("Content")]
        public TextType[] Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        [XmlElement("SubsetProjectArtefactProfileComplexDescription")]
        public ProjectArtefactProfileComplexDescriptionType[] SubsetProjectArtefactProfileComplexDescription
        {
            get => this.subsetProjectArtefactProfileComplexDescriptionField;
            set => this.subsetProjectArtefactProfileComplexDescriptionField = value;
        }

        public ProjectArtefactIdentityType RegistrationProjectArtefactIdentity
        {
            get => this.registrationProjectArtefactIdentityField;
            set => this.registrationProjectArtefactIdentityField = value;
        }

        public SummaryDocumentType ResponseSummaryDocument
        {
            get => this.responseSummaryDocumentField;
            set => this.responseSummaryDocumentField = value;
        }
    }
}