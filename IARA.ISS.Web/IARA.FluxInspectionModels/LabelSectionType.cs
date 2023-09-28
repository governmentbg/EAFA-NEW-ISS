namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LabelSection", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LabelSectionType
    {

        private IDType idField;

        private CodeType patternCodeField;

        private SectionSegmentType[] includedSectionSegmentField;

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

        public CodeType PatternCode
        {
            get
            {
                return this.patternCodeField;
            }
            set
            {
                this.patternCodeField = value;
            }
        }

        [XmlElement("IncludedSectionSegment")]
        public SectionSegmentType[] IncludedSectionSegment
        {
            get
            {
                return this.includedSectionSegmentField;
            }
            set
            {
                this.includedSectionSegmentField = value;
            }
        }
    }
}