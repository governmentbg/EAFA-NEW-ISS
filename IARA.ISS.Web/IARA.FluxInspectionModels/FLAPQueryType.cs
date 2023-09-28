namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FLAPQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FLAPQueryType
    {

        private DateTimeType submittedDateTimeField;

        private CodeType typeCodeField;

        private DelimitedPeriodType specifiedDelimitedPeriodField;

        private FLAPIdentityType[] subjectFLAPIdentityField;

        private VesselIdentityType[] subjectVesselIdentityField;

        private FLUXPartyType submitterFLUXPartyField;

        public DateTimeType SubmittedDateTime
        {
            get => this.submittedDateTimeField;
            set => this.submittedDateTimeField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DelimitedPeriodType SpecifiedDelimitedPeriod
        {
            get => this.specifiedDelimitedPeriodField;
            set => this.specifiedDelimitedPeriodField = value;
        }

        [XmlElement("SubjectFLAPIdentity")]
        public FLAPIdentityType[] SubjectFLAPIdentity
        {
            get => this.subjectFLAPIdentityField;
            set => this.subjectFLAPIdentityField = value;
        }

        [XmlElement("SubjectVesselIdentity")]
        public VesselIdentityType[] SubjectVesselIdentity
        {
            get => this.subjectVesselIdentityField;
            set => this.subjectVesselIdentityField = value;
        }

        public FLUXPartyType SubmitterFLUXParty
        {
            get => this.submitterFLUXPartyField;
            set => this.submitterFLUXPartyField = value;
        }
    }
}