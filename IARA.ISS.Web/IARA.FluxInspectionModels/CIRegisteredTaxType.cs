namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRegisteredTax", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRegisteredTaxType
    {

        private TaxTypeCodeType typeCodeField;

        private CodeType exemptionReasonCodeField;

        private TextType[] exemptionReasonField;

        private CurrencyCodeType currencyCodeField;

        private TextType[] jurisdictionField;

        private TextType[] descriptionField;

        private IndicatorType customsDutyIndicatorField;

        public TaxTypeCodeType TypeCode
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

        public CodeType ExemptionReasonCode
        {
            get
            {
                return this.exemptionReasonCodeField;
            }
            set
            {
                this.exemptionReasonCodeField = value;
            }
        }

        [XmlElement("ExemptionReason")]
        public TextType[] ExemptionReason
        {
            get
            {
                return this.exemptionReasonField;
            }
            set
            {
                this.exemptionReasonField = value;
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

        [XmlElement("Jurisdiction")]
        public TextType[] Jurisdiction
        {
            get
            {
                return this.jurisdictionField;
            }
            set
            {
                this.jurisdictionField = value;
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

        public IndicatorType CustomsDutyIndicator
        {
            get
            {
                return this.customsDutyIndicatorField;
            }
            set
            {
                this.customsDutyIndicatorField = value;
            }
        }
    }
}