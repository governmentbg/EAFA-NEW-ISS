namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SalesQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SalesQueryType
    {

        private IDType idField;

        private DateTimeType submittedDateTimeField;

        private CodeType[] typeCodeField;

        private DelimitedPeriodType specifiedDelimitedPeriodField;

        private FLUXPartyType submitterFLUXPartyField;

        private SalesQueryParameterType[] simpleSalesQueryParameterField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public DateTimeType SubmittedDateTime
        {
            get => this.submittedDateTimeField;
            set => this.submittedDateTimeField = value;
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DelimitedPeriodType SpecifiedDelimitedPeriod
        {
            get => this.specifiedDelimitedPeriodField;
            set => this.specifiedDelimitedPeriodField = value;
        }

        public FLUXPartyType SubmitterFLUXParty
        {
            get => this.submitterFLUXPartyField;
            set => this.submitterFLUXPartyField = value;
        }

        [XmlElement("SimpleSalesQueryParameter")]
        public SalesQueryParameterType[] SimpleSalesQueryParameter
        {
            get => this.simpleSalesQueryParameterField;
            set => this.simpleSalesQueryParameterField = value;
        }
    }
}