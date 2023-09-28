namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SalesEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SalesEventType
    {

        private DateTimeType occurrenceDateTimeField;

        private TextType sellerNameField;

        private TextType buyerNameField;

        private SalesBatchType[] relatedSalesBatchField;

        public DateTimeType OccurrenceDateTime
        {
            get => this.occurrenceDateTimeField;
            set => this.occurrenceDateTimeField = value;
        }

        public TextType SellerName
        {
            get => this.sellerNameField;
            set => this.sellerNameField = value;
        }

        public TextType BuyerName
        {
            get => this.buyerNameField;
            set => this.buyerNameField = value;
        }

        [XmlElement("RelatedSalesBatch")]
        public SalesBatchType[] RelatedSalesBatch
        {
            get => this.relatedSalesBatchField;
            set => this.relatedSalesBatchField = value;
        }
    }
}