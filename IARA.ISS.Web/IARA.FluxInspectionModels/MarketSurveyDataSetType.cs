namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MarketSurveyDataSet", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MarketSurveyDataSetType
    {

        private IDType[] idField;

        private IndicatorType rawDataIndicatorField;

        private TextType[] formatDescriptionField;

        private TextType[] adjustmentDescriptionField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IndicatorType RawDataIndicator
        {
            get => this.rawDataIndicatorField;
            set => this.rawDataIndicatorField = value;
        }

        [XmlElement("FormatDescription")]
        public TextType[] FormatDescription
        {
            get => this.formatDescriptionField;
            set => this.formatDescriptionField = value;
        }

        [XmlElement("AdjustmentDescription")]
        public TextType[] AdjustmentDescription
        {
            get => this.adjustmentDescriptionField;
            set => this.adjustmentDescriptionField = value;
        }
    }
}