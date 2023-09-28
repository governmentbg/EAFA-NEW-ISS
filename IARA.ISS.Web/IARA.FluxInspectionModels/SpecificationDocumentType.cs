namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecificationDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecificationDocumentType
    {

        private IDType idField;

        private TextType revisionDescriptionField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType RevisionDescription
        {
            get => this.revisionDescriptionField;
            set => this.revisionDescriptionField = value;
        }
    }
}