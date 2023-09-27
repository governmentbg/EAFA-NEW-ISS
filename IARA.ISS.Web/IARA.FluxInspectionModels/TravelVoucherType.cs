namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TravelVoucher", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TravelVoucherType
    {

        private AmountType faceAmountField;

        private CodeType typeCodeField;

        private IDType idField;

        private FormattedDateTimeType issueDateTimeField;

        private TextType issuingCompanyNameField;

        private TextType descriptionField;

        public AmountType FaceAmount
        {
            get => this.faceAmountField;
            set => this.faceAmountField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public FormattedDateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public TextType IssuingCompanyName
        {
            get => this.issuingCompanyNameField;
            set => this.issuingCompanyNameField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}