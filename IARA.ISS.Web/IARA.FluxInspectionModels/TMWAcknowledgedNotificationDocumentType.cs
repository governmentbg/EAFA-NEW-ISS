namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWAcknowledgedNotificationDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWAcknowledgedNotificationDocumentType
    {

        private IDType idField;

        private DateTimeType receiptDateTimeField;

        private DateTimeType acceptanceDateTimeField;

        private TMWOrganizationType acknowledgingTMWOrganizationField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public DateTimeType ReceiptDateTime
        {
            get => this.receiptDateTimeField;
            set => this.receiptDateTimeField = value;
        }

        public DateTimeType AcceptanceDateTime
        {
            get => this.acceptanceDateTimeField;
            set => this.acceptanceDateTimeField = value;
        }

        public TMWOrganizationType AcknowledgingTMWOrganization
        {
            get => this.acknowledgingTMWOrganizationField;
            set => this.acknowledgingTMWOrganizationField = value;
        }
    }
}