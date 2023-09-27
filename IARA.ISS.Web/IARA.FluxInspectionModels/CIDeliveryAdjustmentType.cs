namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDeliveryAdjustment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDeliveryAdjustmentType
    {

        private AdjustmentReasonCodeType reasonCodeField;

        private TextType[] reasonField;

        private AmountType[] actualAmountField;

        private QuantityType actualQuantityField;

        private DateTimeType actualDateTimeField;

        public AdjustmentReasonCodeType ReasonCode
        {
            get
            {
                return this.reasonCodeField;
            }
            set
            {
                this.reasonCodeField = value;
            }
        }

        [XmlElement("Reason")]
        public TextType[] Reason
        {
            get
            {
                return this.reasonField;
            }
            set
            {
                this.reasonField = value;
            }
        }

        [XmlElement("ActualAmount")]
        public AmountType[] ActualAmount
        {
            get
            {
                return this.actualAmountField;
            }
            set
            {
                this.actualAmountField = value;
            }
        }

        public QuantityType ActualQuantity
        {
            get
            {
                return this.actualQuantityField;
            }
            set
            {
                this.actualQuantityField = value;
            }
        }

        public DateTimeType ActualDateTime
        {
            get
            {
                return this.actualDateTimeField;
            }
            set
            {
                this.actualDateTimeField = value;
            }
        }
    }
}