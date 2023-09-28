namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AnimalPassportDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AnimalPassportDocumentType
    {

        private DateTimeType issueDateTimeField;

        private IDType idField;

        private IDType versionIDField;

        private SpecifiedPartyType nationalAuthorityIssuerSpecifiedPartyField;

        private SpecifiedPartyType officeIssuerSpecifiedPartyField;

        public DateTimeType IssueDateTime
        {
            get
            {
                return this.issueDateTimeField;
            }
            set
            {
                this.issueDateTimeField = value;
            }
        }

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

        public IDType VersionID
        {
            get
            {
                return this.versionIDField;
            }
            set
            {
                this.versionIDField = value;
            }
        }

        public SpecifiedPartyType NationalAuthorityIssuerSpecifiedParty
        {
            get
            {
                return this.nationalAuthorityIssuerSpecifiedPartyField;
            }
            set
            {
                this.nationalAuthorityIssuerSpecifiedPartyField = value;
            }
        }

        public SpecifiedPartyType OfficeIssuerSpecifiedParty
        {
            get
            {
                return this.officeIssuerSpecifiedPartyField;
            }
            set
            {
                this.officeIssuerSpecifiedPartyField = value;
            }
        }
    }
}