namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectArtefactComplexDescription", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectArtefactComplexDescriptionType
    {

        private TextType[] contentField;

        private ProjectArtefactIdentityType registrationProjectArtefactIdentityField;

        private ProjectArtefactDocumentType[] responseProjectArtefactDocumentField;

        [XmlElement("Content")]
        public TextType[] Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        public ProjectArtefactIdentityType RegistrationProjectArtefactIdentity
        {
            get => this.registrationProjectArtefactIdentityField;
            set => this.registrationProjectArtefactIdentityField = value;
        }

        [XmlElement("ResponseProjectArtefactDocument")]
        public ProjectArtefactDocumentType[] ResponseProjectArtefactDocument
        {
            get => this.responseProjectArtefactDocumentField;
            set => this.responseProjectArtefactDocumentField = value;
        }
    }
}