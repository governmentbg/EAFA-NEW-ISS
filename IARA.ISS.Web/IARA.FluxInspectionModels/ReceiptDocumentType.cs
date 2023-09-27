namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReceiptDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReceiptDocumentType
    {

        private IDType idField;

        private TextType nameField;

        private DateTimeType receiptDateTimeField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public DateTimeType ReceiptDateTime
        {
            get => this.receiptDateTimeField;
            set => this.receiptDateTimeField = value;
        }
    }
}