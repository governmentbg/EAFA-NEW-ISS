namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProductAvailability", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProductAvailabilityType
    {

        private CodeType statusCodeField;

        private QuantityType saleUnitQuantityField;

        private TextType descriptionField;

        private AvailablePeriodType[] applicableAvailablePeriodField;

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public QuantityType SaleUnitQuantity
        {
            get => this.saleUnitQuantityField;
            set => this.saleUnitQuantityField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("ApplicableAvailablePeriod")]
        public AvailablePeriodType[] ApplicableAvailablePeriod
        {
            get => this.applicableAvailablePeriodField;
            set => this.applicableAvailablePeriodField = value;
        }
    }
}