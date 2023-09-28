namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedMarketplace", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedMarketplaceType
    {

        private IDType idField;

        private TextType nameField;

        private IndicatorType virtualIndicatorField;

        private IDType[] websiteURIIDField;

        private CodeType salesMethodCodeField;

        private AvailablePeriodType[] orderingAvailablePeriodField;

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

        public IndicatorType VirtualIndicator
        {
            get => this.virtualIndicatorField;
            set => this.virtualIndicatorField = value;
        }

        [XmlElement("WebsiteURIID")]
        public IDType[] WebsiteURIID
        {
            get => this.websiteURIIDField;
            set => this.websiteURIIDField = value;
        }

        public CodeType SalesMethodCode
        {
            get => this.salesMethodCodeField;
            set => this.salesMethodCodeField = value;
        }

        [XmlElement("OrderingAvailablePeriod")]
        public AvailablePeriodType[] OrderingAvailablePeriod
        {
            get => this.orderingAvailablePeriodField;
            set => this.orderingAvailablePeriodField = value;
        }
    }
}