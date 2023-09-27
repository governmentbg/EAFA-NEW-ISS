namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradePaymentTerms", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradePaymentTermsType
    {

        private PaymentTermsIDType idField;

        private PaymentTermsEventTimeReferenceCodeType fromEventCodeField;

        private MeasureType settlementPeriodMeasureField;

        private TextType[] descriptionField;

        private DateTimeType dueDateDateTimeField;

        private CodeType instructionTypeCodeField;

        private IDType[] paymentMeansIDField;

        private PercentType partialPaymentPercentField;

        private IDType[] directDebitMandateIDField;

        private PaymentTermsTypeCodeType typeCodeField;

        private AmountType[] partialPaymentAmountField;

        private DateTimeType billStartDateTimeField;

        private CITradePaymentPenaltyTermsType applicableCITradePaymentPenaltyTermsField;

        private CITradePaymentDiscountTermsType applicableCITradePaymentDiscountTermsField;

        private CITradePartyType[] payeeCITradePartyField;

        public PaymentTermsIDType ID
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

        public PaymentTermsEventTimeReferenceCodeType FromEventCode
        {
            get
            {
                return this.fromEventCodeField;
            }
            set
            {
                this.fromEventCodeField = value;
            }
        }

        public MeasureType SettlementPeriodMeasure
        {
            get
            {
                return this.settlementPeriodMeasureField;
            }
            set
            {
                this.settlementPeriodMeasureField = value;
            }
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public DateTimeType DueDateDateTime
        {
            get
            {
                return this.dueDateDateTimeField;
            }
            set
            {
                this.dueDateDateTimeField = value;
            }
        }

        public CodeType InstructionTypeCode
        {
            get
            {
                return this.instructionTypeCodeField;
            }
            set
            {
                this.instructionTypeCodeField = value;
            }
        }

        [XmlElement("PaymentMeansID")]
        public IDType[] PaymentMeansID
        {
            get
            {
                return this.paymentMeansIDField;
            }
            set
            {
                this.paymentMeansIDField = value;
            }
        }

        public PercentType PartialPaymentPercent
        {
            get
            {
                return this.partialPaymentPercentField;
            }
            set
            {
                this.partialPaymentPercentField = value;
            }
        }

        [XmlElement("DirectDebitMandateID")]
        public IDType[] DirectDebitMandateID
        {
            get
            {
                return this.directDebitMandateIDField;
            }
            set
            {
                this.directDebitMandateIDField = value;
            }
        }

        public PaymentTermsTypeCodeType TypeCode
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

        [XmlElement("PartialPaymentAmount")]
        public AmountType[] PartialPaymentAmount
        {
            get
            {
                return this.partialPaymentAmountField;
            }
            set
            {
                this.partialPaymentAmountField = value;
            }
        }

        public DateTimeType BillStartDateTime
        {
            get
            {
                return this.billStartDateTimeField;
            }
            set
            {
                this.billStartDateTimeField = value;
            }
        }

        public CITradePaymentPenaltyTermsType ApplicableCITradePaymentPenaltyTerms
        {
            get
            {
                return this.applicableCITradePaymentPenaltyTermsField;
            }
            set
            {
                this.applicableCITradePaymentPenaltyTermsField = value;
            }
        }

        public CITradePaymentDiscountTermsType ApplicableCITradePaymentDiscountTerms
        {
            get
            {
                return this.applicableCITradePaymentDiscountTermsField;
            }
            set
            {
                this.applicableCITradePaymentDiscountTermsField = value;
            }
        }

        [XmlElement("PayeeCITradeParty")]
        public CITradePartyType[] PayeeCITradeParty
        {
            get
            {
                return this.payeeCITradePartyField;
            }
            set
            {
                this.payeeCITradePartyField = value;
            }
        }
    }
}