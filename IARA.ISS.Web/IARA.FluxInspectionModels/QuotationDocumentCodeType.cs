namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class QuotationDocumentCodeType
    {

        private string listIDField;

        private QuotationDocumentCodeListAgencyIDContentType listAgencyIDField;

        private bool listAgencyIDFieldSpecified;

        private string listVersionIDField;

        private string listURIField;

        private DocumentNameCodeQuotationContentType valueField;

        public QuotationDocumentCodeType()
        {
            this.listIDField = "1001_DocumentTypeCode_Quotation";
            this.listAgencyIDField = QuotationDocumentCodeListAgencyIDContentType.Item6;
            this.listVersionIDField = "D22A";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get => this.listIDField;
            set => this.listIDField = value;
        }

        [XmlAttribute]
        public QuotationDocumentCodeListAgencyIDContentType listAgencyID
        {
            get => this.listAgencyIDField;
            set => this.listAgencyIDField = value;
        }

        [XmlIgnore()]
        public bool listAgencyIDSpecified
        {
            get => this.listAgencyIDFieldSpecified;
            set => this.listAgencyIDFieldSpecified = value;
        }

        [XmlAttribute(DataType = "token")]
        public string listVersionID
        {
            get => this.listVersionIDField;
            set => this.listVersionIDField = value;
        }

        [XmlAttribute(DataType = "anyURI")]
        public string listURI
        {
            get => this.listURIField;
            set => this.listURIField = value;
        }

        [XmlText]
        public DocumentNameCodeQuotationContentType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}