namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapAccountingAccountBoundary", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapAccountingAccountBoundaryType
    {

        private IDType firstAccountIdentificationIDField;

        private IDType lastAccountIdentificationIDField;

        private IDType firstSubAccountIdentificationIDField;

        private IDType lastSubAccountIdentificationIDField;

        public IDType FirstAccountIdentificationID
        {
            get => this.firstAccountIdentificationIDField;
            set => this.firstAccountIdentificationIDField = value;
        }

        public IDType LastAccountIdentificationID
        {
            get => this.lastAccountIdentificationIDField;
            set => this.lastAccountIdentificationIDField = value;
        }

        public IDType FirstSubAccountIdentificationID
        {
            get => this.firstSubAccountIdentificationIDField;
            set => this.firstSubAccountIdentificationIDField = value;
        }

        public IDType LastSubAccountIdentificationID
        {
            get => this.lastSubAccountIdentificationIDField;
            set => this.lastSubAccountIdentificationIDField = value;
        }
    }
}