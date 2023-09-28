namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MSDSDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MSDSDocumentType
    {

        private IDType idField;

        private DateTimeType creationDateTimeField;

        private DateTimeType previousVersionCreationDateTimeField;

        private CodeType proprietaryInformationTypeCodeField;

        private TextType[] additionalInformationRemarksField;

        private TextType revisionField;

        private DocumentPreparerPartyType issuerDocumentPreparerPartyField;

        private ResponsiblePartyType ownerResponsiblePartyField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public DateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }

        public DateTimeType PreviousVersionCreationDateTime
        {
            get => this.previousVersionCreationDateTimeField;
            set => this.previousVersionCreationDateTimeField = value;
        }

        public CodeType ProprietaryInformationTypeCode
        {
            get => this.proprietaryInformationTypeCodeField;
            set => this.proprietaryInformationTypeCodeField = value;
        }

        [XmlElement("AdditionalInformationRemarks")]
        public TextType[] AdditionalInformationRemarks
        {
            get => this.additionalInformationRemarksField;
            set => this.additionalInformationRemarksField = value;
        }

        public TextType Revision
        {
            get => this.revisionField;
            set => this.revisionField = value;
        }

        public DocumentPreparerPartyType IssuerDocumentPreparerParty
        {
            get => this.issuerDocumentPreparerPartyField;
            set => this.issuerDocumentPreparerPartyField = value;
        }

        public ResponsiblePartyType OwnerResponsibleParty
        {
            get => this.ownerResponsiblePartyField;
            set => this.ownerResponsiblePartyField = value;
        }
    }
}