namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LogisticsLabel", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LogisticsLabelType
    {

        private IDType idField;

        private IDType seriesStartIDField;

        private IDType seriesEndIDField;

        private CodeType layoutTypeCodeField;

        private CodeType sizeCodeField;

        private IndicatorType markingIndicatorField;

        private LabelSectionType[] includedLabelSectionField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public IDType SeriesStartID
        {
            get
            {
                return this.seriesStartIDField;
            }
            set
            {
                this.seriesStartIDField = value;
            }
        }

        public IDType SeriesEndID
        {
            get
            {
                return this.seriesEndIDField;
            }
            set
            {
                this.seriesEndIDField = value;
            }
        }

        public CodeType LayoutTypeCode
        {
            get
            {
                return this.layoutTypeCodeField;
            }
            set
            {
                this.layoutTypeCodeField = value;
            }
        }

        public CodeType SizeCode
        {
            get
            {
                return this.sizeCodeField;
            }
            set
            {
                this.sizeCodeField = value;
            }
        }

        public IndicatorType MarkingIndicator
        {
            get
            {
                return this.markingIndicatorField;
            }
            set
            {
                this.markingIndicatorField = value;
            }
        }

        [XmlElement("IncludedLabelSection")]
        public LabelSectionType[] IncludedLabelSection
        {
            get
            {
                return this.includedLabelSectionField;
            }
            set
            {
                this.includedLabelSectionField = value;
            }
        }
    }
}