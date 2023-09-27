namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CancelledDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CancelledDocumentType
    {

        private IDType idField;

        private CodeType cancellationReasonCodeField;

        private TextType cancellationReasonField;

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

        public CodeType CancellationReasonCode
        {
            get
            {
                return this.cancellationReasonCodeField;
            }
            set
            {
                this.cancellationReasonCodeField = value;
            }
        }

        public TextType CancellationReason
        {
            get
            {
                return this.cancellationReasonField;
            }
            set
            {
                this.cancellationReasonField = value;
            }
        }
    }
}