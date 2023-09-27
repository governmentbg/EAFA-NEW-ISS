namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIPackagingMarking", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIPackagingMarkingType
    {

        private PackagingMarkingCodeType[] typeCodeField;

        private TextType[] contentField;

        private DateTimeType contentDateTimeField;

        private AmountType[] contentAmountField;

        private CodeType[] barcodeTypeCodeField;

        private CodeType[] contentCodeField;

        private AutomaticDataCaptureMethodCodeType[] automaticDataCaptureMethodTypeCodeField;

        [XmlElement("TypeCode")]
        public PackagingMarkingCodeType[] TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        [XmlElement("Content")]
        public TextType[] Content
        {
            get
            {
                return this.contentField;
            }
            set
            {
                this.contentField = value;
            }
        }

        public DateTimeType ContentDateTime
        {
            get
            {
                return this.contentDateTimeField;
            }
            set
            {
                this.contentDateTimeField = value;
            }
        }

        [XmlElement("ContentAmount")]
        public AmountType[] ContentAmount
        {
            get
            {
                return this.contentAmountField;
            }
            set
            {
                this.contentAmountField = value;
            }
        }

        [XmlElement("BarcodeTypeCode")]
        public CodeType[] BarcodeTypeCode
        {
            get
            {
                return this.barcodeTypeCodeField;
            }
            set
            {
                this.barcodeTypeCodeField = value;
            }
        }

        [XmlElement("ContentCode")]
        public CodeType[] ContentCode
        {
            get
            {
                return this.contentCodeField;
            }
            set
            {
                this.contentCodeField = value;
            }
        }

        [XmlElement("AutomaticDataCaptureMethodTypeCode")]
        public AutomaticDataCaptureMethodCodeType[] AutomaticDataCaptureMethodTypeCode
        {
            get
            {
                return this.automaticDataCaptureMethodTypeCodeField;
            }
            set
            {
                this.automaticDataCaptureMethodTypeCodeField = value;
            }
        }
    }
}