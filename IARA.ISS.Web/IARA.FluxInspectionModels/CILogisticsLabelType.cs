namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CILogisticsLabel", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CILogisticsLabelType
    {

        private IDType idField;

        private IDType seriesStartIDField;

        private IDType seriesEndIDField;

        private CodeType layoutTypeCodeField;

        private CodeType sizeCodeField;

        private LabelSectionType[] includedLabelSectionField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType SeriesStartID
        {
            get => this.seriesStartIDField;
            set => this.seriesStartIDField = value;
        }

        public IDType SeriesEndID
        {
            get => this.seriesEndIDField;
            set => this.seriesEndIDField = value;
        }

        public CodeType LayoutTypeCode
        {
            get => this.layoutTypeCodeField;
            set => this.layoutTypeCodeField = value;
        }

        public CodeType SizeCode
        {
            get => this.sizeCodeField;
            set => this.sizeCodeField = value;
        }

        [XmlElement("IncludedLabelSection")]
        public LabelSectionType[] IncludedLabelSection
        {
            get => this.includedLabelSectionField;
            set => this.includedLabelSectionField = value;
        }
    }
}