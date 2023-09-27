namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselQueryType
    {

        private DateTimeType submittedDateTimeField;

        private CodeType typeCodeField;

        private IDType idField;

        private FLUXPartyType submitterFLUXPartyField;

        private VesselQueryParameterType[] simpleVesselQueryParameterField;

        private LogicalQueryParameterType[] compoundLogicalQueryParameterField;

        private VesselIdentityType subjectVesselIdentityField;

        private DelimitedPeriodType specifiedDelimitedPeriodField;

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

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public FLUXPartyType SubmitterFLUXParty
        {
            get => this.submitterFLUXPartyField;
            set => this.submitterFLUXPartyField = value;
        }

        [XmlElement("SimpleVesselQueryParameter")]
        public VesselQueryParameterType[] SimpleVesselQueryParameter
        {
            get => this.simpleVesselQueryParameterField;
            set => this.simpleVesselQueryParameterField = value;
        }

        [XmlElement("CompoundLogicalQueryParameter")]
        public LogicalQueryParameterType[] CompoundLogicalQueryParameter
        {
            get => this.compoundLogicalQueryParameterField;
            set => this.compoundLogicalQueryParameterField = value;
        }

        public VesselIdentityType SubjectVesselIdentity
        {
            get => this.subjectVesselIdentityField;
            set => this.subjectVesselIdentityField = value;
        }

        public DelimitedPeriodType SpecifiedDelimitedPeriod
        {
            get => this.specifiedDelimitedPeriodField;
            set => this.specifiedDelimitedPeriodField = value;
        }
    }
}