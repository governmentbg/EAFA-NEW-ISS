namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRDocumentLineDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRDocumentLineDocumentType
    {

        private IDType uUIDLineIDField;

        private IDType lineIDField;

        private LineStatusCodeType lineStatusCodeField;

        private CodeType lineStatusReasonCodeField;

        private IDType[] subordinateLineIDField;

        private CodeType categoryCodeField;

        private CINoteType[] includedCINoteField;

        public IDType UUIDLineID
        {
            get => this.uUIDLineIDField;
            set => this.uUIDLineIDField = value;
        }

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

        [XmlElement("SubordinateLineID")]
        public IDType[] SubordinateLineID
        {
            get => this.subordinateLineIDField;
            set => this.subordinateLineIDField = value;
        }

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get => this.includedCINoteField;
            set => this.includedCINoteField = value;
        }
    }
}