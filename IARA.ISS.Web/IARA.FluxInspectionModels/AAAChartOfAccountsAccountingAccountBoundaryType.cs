namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAChartOfAccountsAccountingAccountBoundary", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAChartOfAccountsAccountingAccountBoundaryType
    {

        private IDType firstAccountIdentificationIDField;

        private IDType lastAccountIdentificationIDField;

        private IDType firstSubAccountIdentificationIDField;

        private IDType lastSubAccountIdentificationIDField;

        public IDType FirstAccountIdentificationID
        {
            get
            {
                return this.firstAccountIdentificationIDField;
            }
            set
            {
                this.firstAccountIdentificationIDField = value;
            }
        }

        public IDType LastAccountIdentificationID
        {
            get
            {
                return this.lastAccountIdentificationIDField;
            }
            set
            {
                this.lastAccountIdentificationIDField = value;
            }
        }

        public IDType FirstSubAccountIdentificationID
        {
            get
            {
                return this.firstSubAccountIdentificationIDField;
            }
            set
            {
                this.firstSubAccountIdentificationIDField = value;
            }
        }

        public IDType LastSubAccountIdentificationID
        {
            get
            {
                return this.lastSubAccountIdentificationIDField;
            }
            set
            {
                this.lastSubAccountIdentificationIDField = value;
            }
        }
    }
}