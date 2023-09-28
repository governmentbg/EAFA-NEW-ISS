namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MDRQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MDRQueryType
    {

        private IDType idField;

        private DateTimeType submittedDateTimeField;

        private CodeType typeCodeField;

        private CodeType contractualLanguageCodeField;

        private FLUXPartyType submitterFLUXPartyField;

        private MDRQueryIdentityType subjectMDRQueryIdentityField;

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

        public CodeType ContractualLanguageCode
        {
            get => this.contractualLanguageCodeField;
            set => this.contractualLanguageCodeField = value;
        }

        public FLUXPartyType SubmitterFLUXParty
        {
            get => this.submitterFLUXPartyField;
            set => this.submitterFLUXPartyField = value;
        }

        public MDRQueryIdentityType SubjectMDRQueryIdentity
        {
            get => this.subjectMDRQueryIdentityField;
            set => this.subjectMDRQueryIdentityField = value;
        }
    }
}