namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIOLDocumentLineDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIOLDocumentLineDocumentType
    {

        private IDType lineIDField;

        private IDType uUIDLineIDField;

        private LineStatusCodeType lineStatusCodeField;

        private CodeType lineStatusReasonCodeField;

        private IDType idField;

        private CodeType buyerAssignedCategoryCodeField;

        private CINoteType[] includedCINoteField;

        private CIAcknowledgementDocumentType referenceCIAcknowledgementDocumentField;

        public IDType LineID
        {
            get => this.lineIDField;
            set => this.lineIDField = value;
        }

        public IDType UUIDLineID
        {
            get => this.uUIDLineIDField;
            set => this.uUIDLineIDField = value;
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

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType BuyerAssignedCategoryCode
        {
            get => this.buyerAssignedCategoryCodeField;
            set => this.buyerAssignedCategoryCodeField = value;
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get => this.includedCINoteField;
            set => this.includedCINoteField = value;
        }

        public CIAcknowledgementDocumentType ReferenceCIAcknowledgementDocument
        {
            get => this.referenceCIAcknowledgementDocumentField;
            set => this.referenceCIAcknowledgementDocumentField = value;
        }
    }
}