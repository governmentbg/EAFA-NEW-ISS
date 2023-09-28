namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIQDocumentLineDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIQDocumentLineDocumentType
    {

        private IDType lineIDField;

        private LineStatusCodeType lineStatusCodeField;

        private CodeType lineStatusReasonCodeField;

        private DateTimeType latestRevisionDateTimeField;

        private CINoteType[] includedCINoteField;

        public IDType LineID
        {
            get => this.lineIDField;
            set => this.lineIDField = value;
        }

        public LineStatusCodeType LineStatusCode
        {
            get => this.lineStatusCodeField;
            set => this.lineStatusCodeField = value;
        }

        public CodeType LineStatusReasonCode
        {
            get => this.lineStatusReasonCodeField;
            set => this.lineStatusReasonCodeField = value;
        }

        public DateTimeType LatestRevisionDateTime
        {
            get => this.latestRevisionDateTimeField;
            set => this.latestRevisionDateTimeField = value;
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get => this.includedCINoteField;
            set => this.includedCINoteField = value;
        }
    }
}