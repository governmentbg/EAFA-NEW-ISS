namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFConsignmentItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFConsignmentItemType
    {

        private IDType idField;

        private NumericType sequenceNumericField;

        private CodeType productDateTypeCodeField;

        private DateTimeType productDateTimeField;

        private RASFFDocumentType associatedRASFFDocumentField;

        private RASFFCertificateType providedRASFFCertificateField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public NumericType SequenceNumeric
        {
            get => this.sequenceNumericField;
            set => this.sequenceNumericField = value;
        }

        public CodeType ProductDateTypeCode
        {
            get => this.productDateTypeCodeField;
            set => this.productDateTypeCodeField = value;
        }

        public DateTimeType ProductDateTime
        {
            get => this.productDateTimeField;
            set => this.productDateTimeField = value;
        }

        public RASFFDocumentType AssociatedRASFFDocument
        {
            get => this.associatedRASFFDocumentField;
            set => this.associatedRASFFDocumentField = value;
        }

        public RASFFCertificateType ProvidedRASFFCertificate
        {
            get => this.providedRASFFCertificateField;
            set => this.providedRASFFCertificateField = value;
        }
    }
}