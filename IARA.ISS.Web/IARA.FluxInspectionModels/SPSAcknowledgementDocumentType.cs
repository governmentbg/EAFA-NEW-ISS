namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSAcknowledgementDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSAcknowledgementDocumentType
    {

        private DateTimeType issueDateTimeField;

        private StatusCodeType statusCodeField;

        private TextType[] reasonInformationField;

        private SPSReferencedDocumentType[] referenceSPSReferencedDocumentField;

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public StatusCodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        [XmlElement("ReasonInformation")]
        public TextType[] ReasonInformation
        {
            get => this.reasonInformationField;
            set => this.reasonInformationField = value;
        }

        [XmlElement("ReferenceSPSReferencedDocument")]
        public SPSReferencedDocumentType[] ReferenceSPSReferencedDocument
        {
            get => this.referenceSPSReferencedDocumentField;
            set => this.referenceSPSReferencedDocumentField = value;
        }
    }
}