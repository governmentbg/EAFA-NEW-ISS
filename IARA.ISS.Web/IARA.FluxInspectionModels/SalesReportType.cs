namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SalesReport", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SalesReportType
    {

        private IDType idField;

        private CodeType itemTypeCodeField;

        private SalesDocumentType[] includedSalesDocumentField;

        private ValidationResultDocumentType[] includedValidationResultDocumentField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType ItemTypeCode
        {
            get => this.itemTypeCodeField;
            set => this.itemTypeCodeField = value;
        }

        [XmlElement("IncludedSalesDocument")]
        public SalesDocumentType[] IncludedSalesDocument
        {
            get => this.includedSalesDocumentField;
            set => this.includedSalesDocumentField = value;
        }

        [XmlElement("IncludedValidationResultDocument")]
        public ValidationResultDocumentType[] IncludedValidationResultDocument
        {
            get => this.includedValidationResultDocumentField;
            set => this.includedValidationResultDocumentField = value;
        }
    }
}