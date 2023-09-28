namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISDocumentLineDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISDocumentLineDocumentType
    {

        private IDType lineIDField;

        private LineStatusCodeType lineStatusCodeField;

        private CodeType lineStatusReasonCodeField;

        private DateTimeType latestRevisionDateTimeField;

        private IDType idField;

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

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get => this.includedCINoteField;
            set => this.includedCINoteField = value;
        }
    }
}