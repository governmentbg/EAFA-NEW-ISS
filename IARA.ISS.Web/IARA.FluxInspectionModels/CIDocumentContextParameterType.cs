namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDocumentContextParameter", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDocumentContextParameterType
    {

        private IDType idField;

        private TextType valueField;

        private CIDocumentVersionType specifiedCIDocumentVersionField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }

        public CIDocumentVersionType SpecifiedCIDocumentVersion
        {
            get => this.specifiedCIDocumentVersionField;
            set => this.specifiedCIDocumentVersionField = value;
        }
    }
}