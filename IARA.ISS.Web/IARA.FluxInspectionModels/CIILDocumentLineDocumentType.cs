namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIILDocumentLineDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIILDocumentLineDocumentType
    {

        private IDType lineIDField;

        private LineStatusCodeType lineStatusCodeField;

        private CodeType lineStatusReasonCodeField;

        private IDType parentLineIDField;

        private CodeType categoryCodeField;

        private CodeType responseReasonCodeField;

        private CINoteType[] includedCINoteField;

        private CIReferencedDocumentType[] referenceCIReferencedDocumentField;

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

        public IDType ParentLineID
        {
            get => this.parentLineIDField;
            set => this.parentLineIDField = value;
        }

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public CodeType ResponseReasonCode
        {
            get => this.responseReasonCodeField;
            set => this.responseReasonCodeField = value;
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get => this.includedCINoteField;
            set => this.includedCINoteField = value;
        }

        [XmlElement("ReferenceCIReferencedDocument")]
        public CIReferencedDocumentType[] ReferenceCIReferencedDocument
        {
            get => this.referenceCIReferencedDocumentField;
            set => this.referenceCIReferencedDocumentField = value;
        }
    }
}