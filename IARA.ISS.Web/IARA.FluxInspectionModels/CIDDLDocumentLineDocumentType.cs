namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDDLDocumentLineDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDDLDocumentLineDocumentType
    {

        private IDType lineIDField;

        private LineStatusCodeType lineStatusCodeField;

        private CodeType lineStatusReasonCodeField;

        private CINoteType[] includedCINoteField;

        public IDType LineID
        {
            get
            {
                return this.lineIDField;
            }
            set
            {
                this.lineIDField = value;
            }
        }

        public LineStatusCodeType LineStatusCode
        {
            get
            {
                return this.lineStatusCodeField;
            }
            set
            {
                this.lineStatusCodeField = value;
            }
        }

        public CodeType LineStatusReasonCode
        {
            get
            {
                return this.lineStatusReasonCodeField;
            }
            set
            {
                this.lineStatusReasonCodeField = value;
            }
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get
            {
                return this.includedCINoteField;
            }
            set
            {
                this.includedCINoteField = value;
            }
        }
    }
}