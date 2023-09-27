namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("IdentifiedReferencedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class IdentifiedReferencedDocumentType
    {

        private IDType idField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }
    }
}