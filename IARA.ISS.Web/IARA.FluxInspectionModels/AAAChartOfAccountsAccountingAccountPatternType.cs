using IARA.FluxInspectionModels;

namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAChartOfAccountsAccountingAccountPattern", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAChartOfAccountsAccountingAccountPatternType
    {

        private IDType idField;

        private TextType nameField;

        private TextType templateField;

        private DateTimeType openingDateTimeField;

        private DateTimeType closingDateTimeField;

        private CountryIDType countryIDField;

        private AAAAccountingAccountDimensionType[] specifiedAAAAccountingAccountDimensionField;

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

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public TextType Template
        {
            get
            {
                return this.templateField;
            }
            set
            {
                this.templateField = value;
            }
        }

        public DateTimeType OpeningDateTime
        {
            get
            {
                return this.openingDateTimeField;
            }
            set
            {
                this.openingDateTimeField = value;
            }
        }

        public DateTimeType ClosingDateTime
        {
            get
            {
                return this.closingDateTimeField;
            }
            set
            {
                this.closingDateTimeField = value;
            }
        }

        public CountryIDType CountryID
        {
            get
            {
                return this.countryIDField;
            }
            set
            {
                this.countryIDField = value;
            }
        }

        [XmlElement("SpecifiedAAAAccountingAccountDimension")]
        public AAAAccountingAccountDimensionType[] SpecifiedAAAAccountingAccountDimension
        {
            get
            {
                return this.specifiedAAAAccountingAccountDimensionField;
            }
            set
            {
                this.specifiedAAAAccountingAccountDimensionField = value;
            }
        }
    }
}