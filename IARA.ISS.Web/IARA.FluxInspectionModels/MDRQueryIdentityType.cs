namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MDRQueryIdentity", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MDRQueryIdentityType
    {

        private IDType idField;

        private DateTimeType[] validFromDateTimeField;

        private IDType versionIDField;

        private DelimitedPeriodType validityDelimitedPeriodField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("ValidFromDateTime")]
        public DateTimeType[] ValidFromDateTime
        {
            get => this.validFromDateTimeField;
            set => this.validFromDateTimeField = value;
        }

        public IDType VersionID
        {
            get => this.versionIDField;
            set => this.versionIDField = value;
        }

        public DelimitedPeriodType ValidityDelimitedPeriod
        {
            get => this.validityDelimitedPeriodField;
            set => this.validityDelimitedPeriodField = value;
        }
    }
}