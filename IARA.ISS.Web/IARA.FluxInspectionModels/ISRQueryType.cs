namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ISRQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ISRQueryType
    {

        private IDType idField;

        private DateTimeType submittedDateTimeField;

        private CodeType typeCodeField;

        private FLUXPartyType submitterFLUXPartyField;

        private DelimitedPeriodType specifiedDelimitedPeriodField;

        private ISRQueryParameterType[] simpleISRQueryParameterField;

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

        public DateTimeType SubmittedDateTime
        {
            get
            {
                return this.submittedDateTimeField;
            }
            set
            {
                this.submittedDateTimeField = value;
            }
        }

        public CodeType TypeCode
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

        public FLUXPartyType SubmitterFLUXParty
        {
            get
            {
                return this.submitterFLUXPartyField;
            }
            set
            {
                this.submitterFLUXPartyField = value;
            }
        }

        public DelimitedPeriodType SpecifiedDelimitedPeriod
        {
            get
            {
                return this.specifiedDelimitedPeriodField;
            }
            set
            {
                this.specifiedDelimitedPeriodField = value;
            }
        }

        [XmlElement("SimpleISRQueryParameter")]
        public ISRQueryParameterType[] SimpleISRQueryParameter
        {
            get
            {
                return this.simpleISRQueryParameterField;
            }
            set
            {
                this.simpleISRQueryParameterField = value;
            }
        }
    }
}