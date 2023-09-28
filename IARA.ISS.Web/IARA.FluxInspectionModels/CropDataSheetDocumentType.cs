namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CropDataSheetDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CropDataSheetDocumentType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private DateTimeType creationDateTimeField;

        private IndicatorType copyIndicatorField;

        private NumericType lineCountNumericField;

        private CropDataSheetPartyType issuerCropDataSheetPartyField;

        private CropDataSheetPartyType recipientCropDataSheetPartyField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }

        public IndicatorType CopyIndicator
        {
            get => this.copyIndicatorField;
            set => this.copyIndicatorField = value;
        }

        public NumericType LineCountNumeric
        {
            get => this.lineCountNumericField;
            set => this.lineCountNumericField = value;
        }

        public CropDataSheetPartyType IssuerCropDataSheetParty
        {
            get => this.issuerCropDataSheetPartyField;
            set => this.issuerCropDataSheetPartyField = value;
        }

        public CropDataSheetPartyType RecipientCropDataSheetParty
        {
            get => this.recipientCropDataSheetPartyField;
            set => this.recipientCropDataSheetPartyField = value;
        }
    }
}