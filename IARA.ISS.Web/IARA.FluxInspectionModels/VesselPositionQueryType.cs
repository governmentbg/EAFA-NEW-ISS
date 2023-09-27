namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselPositionQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselPositionQueryType
    {

        private IDType idField;

        private DateTimeType submittedDateTimeField;

        private CodeType typeCodeField;

        private FLUXPartyType submitterFLUXPartyField;

        private DelimitedPeriodType specifiedDelimitedPeriodField;

        private FLUXLocationType[] specifiedFLUXLocationField;

        private VesselIdentityType subjectVesselIdentityField;

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

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public FLUXPartyType SubmitterFLUXParty
        {
            get => this.submitterFLUXPartyField;
            set => this.submitterFLUXPartyField = value;
        }

        public DelimitedPeriodType SpecifiedDelimitedPeriod
        {
            get => this.specifiedDelimitedPeriodField;
            set => this.specifiedDelimitedPeriodField = value;
        }

        [XmlElement("SpecifiedFLUXLocation")]
        public FLUXLocationType[] SpecifiedFLUXLocation
        {
            get => this.specifiedFLUXLocationField;
            set => this.specifiedFLUXLocationField = value;
        }

        public VesselIdentityType SubjectVesselIdentity
        {
            get => this.subjectVesselIdentityField;
            set => this.subjectVesselIdentityField = value;
        }
    }
}