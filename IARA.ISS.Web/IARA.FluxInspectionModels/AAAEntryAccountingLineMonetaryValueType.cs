namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAEntryAccountingLineMonetaryValue", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAEntryAccountingLineMonetaryValueType
    {

        private AmountType localAccountingCurrencyAmountField;

        private AmountType voucherCurrencyAmountField;

        private AmountType alternateCurrencyAmountField;

        private AlternateCurrencyAmountTypeCodeType alternateCurrencyAmountTypeCodeField;

        private AccountingDebitCreditStatusCodeType debitCreditCodeField;

        private IDType matchingIDField;

        private IDType tickingIDField;

        private IDType applicationNumberIDField;

        private IDType achievedWorkCategoryIDField;

        private IDType distributionKeyIDField;

        private AccountingPerquisiteCodeType perquisiteCodeField;

        private RefundMethodCodeType refundMethodCodeField;

        private IDType remunerationTypeIDField;

        private DateTimeType matchingDateTimeField;

        private AAAEntryAccountingAccountType[] bookingAAAEntryAccountingAccountField;

        public AmountType LocalAccountingCurrencyAmount
        {
            get
            {
                return this.localAccountingCurrencyAmountField;
            }
            set
            {
                this.localAccountingCurrencyAmountField = value;
            }
        }

        public AmountType VoucherCurrencyAmount
        {
            get
            {
                return this.voucherCurrencyAmountField;
            }
            set
            {
                this.voucherCurrencyAmountField = value;
            }
        }

        public AmountType AlternateCurrencyAmount
        {
            get
            {
                return this.alternateCurrencyAmountField;
            }
            set
            {
                this.alternateCurrencyAmountField = value;
            }
        }

        public AlternateCurrencyAmountTypeCodeType AlternateCurrencyAmountTypeCode
        {
            get
            {
                return this.alternateCurrencyAmountTypeCodeField;
            }
            set
            {
                this.alternateCurrencyAmountTypeCodeField = value;
            }
        }

        public AccountingDebitCreditStatusCodeType DebitCreditCode
        {
            get
            {
                return this.debitCreditCodeField;
            }
            set
            {
                this.debitCreditCodeField = value;
            }
        }

        public IDType MatchingID
        {
            get
            {
                return this.matchingIDField;
            }
            set
            {
                this.matchingIDField = value;
            }
        }

        public IDType TickingID
        {
            get
            {
                return this.tickingIDField;
            }
            set
            {
                this.tickingIDField = value;
            }
        }

        public IDType ApplicationNumberID
        {
            get
            {
                return this.applicationNumberIDField;
            }
            set
            {
                this.applicationNumberIDField = value;
            }
        }

        public IDType AchievedWorkCategoryID
        {
            get
            {
                return this.achievedWorkCategoryIDField;
            }
            set
            {
                this.achievedWorkCategoryIDField = value;
            }
        }

        public IDType DistributionKeyID
        {
            get
            {
                return this.distributionKeyIDField;
            }
            set
            {
                this.distributionKeyIDField = value;
            }
        }

        public AccountingPerquisiteCodeType PerquisiteCode
        {
            get
            {
                return this.perquisiteCodeField;
            }
            set
            {
                this.perquisiteCodeField = value;
            }
        }

        public RefundMethodCodeType RefundMethodCode
        {
            get
            {
                return this.refundMethodCodeField;
            }
            set
            {
                this.refundMethodCodeField = value;
            }
        }

        public IDType RemunerationTypeID
        {
            get
            {
                return this.remunerationTypeIDField;
            }
            set
            {
                this.remunerationTypeIDField = value;
            }
        }

        public DateTimeType MatchingDateTime
        {
            get
            {
                return this.matchingDateTimeField;
            }
            set
            {
                this.matchingDateTimeField = value;
            }
        }

        [XmlElement("BookingAAAEntryAccountingAccount")]
        public AAAEntryAccountingAccountType[] BookingAAAEntryAccountingAccount
        {
            get
            {
                return this.bookingAAAEntryAccountingAccountField;
            }
            set
            {
                this.bookingAAAEntryAccountingAccountField = value;
            }
        }
    }
}