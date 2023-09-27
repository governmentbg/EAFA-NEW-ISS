namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RequestedMarketSurvey", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RequestedMarketSurveyType
    {

        private TextType[] targetGroupDescriptionField;

        private IndicatorType timeSeriesIndicatorField;

        private CITradeCountryType[] targetCITradeCountryField;

        [XmlElement("TargetGroupDescription")]
        public TextType[] TargetGroupDescription
        {
            get => this.targetGroupDescriptionField;
            set => this.targetGroupDescriptionField = value;
        }

        public IndicatorType TimeSeriesIndicator
        {
            get => this.timeSeriesIndicatorField;
            set => this.timeSeriesIndicatorField = value;
        }

        [XmlElement("TargetCITradeCountry")]
        public CITradeCountryType[] TargetCITradeCountry
        {
            get => this.targetCITradeCountryField;
            set => this.targetCITradeCountryField = value;
        }
    }
}