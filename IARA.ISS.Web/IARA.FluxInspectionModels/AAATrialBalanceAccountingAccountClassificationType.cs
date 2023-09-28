namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAATrialBalanceAccountingAccountClassification", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAATrialBalanceAccountingAccountClassificationType
    {

        private IDType idField;

        private DateTimeType firstUseDateTimeField;

        private TextType internalReferenceField;

        private CodeType usingCountryCodeField;

        private CurrencyCodeType currencyCodeField;

        private AccountingAccountClassificationCodeType typeCodeField;

        private TextType versionField;

        private TextType revisionField;

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

        public DateTimeType FirstUseDateTime
        {
            get
            {
                return this.firstUseDateTimeField;
            }
            set
            {
                this.firstUseDateTimeField = value;
            }
        }

        public TextType InternalReference
        {
            get
            {
                return this.internalReferenceField;
            }
            set
            {
                this.internalReferenceField = value;
            }
        }

        public CodeType UsingCountryCode
        {
            get
            {
                return this.usingCountryCodeField;
            }
            set
            {
                this.usingCountryCodeField = value;
            }
        }

        public CurrencyCodeType CurrencyCode
        {
            get
            {
                return this.currencyCodeField;
            }
            set
            {
                this.currencyCodeField = value;
            }
        }

        public AccountingAccountClassificationCodeType TypeCode
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

        public TextType Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        public TextType Revision
        {
            get
            {
                return this.revisionField;
            }
            set
            {
                this.revisionField = value;
            }
        }
    }
}