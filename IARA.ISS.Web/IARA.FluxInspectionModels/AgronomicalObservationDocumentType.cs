namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgronomicalObservationDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgronomicalObservationDocumentType
    {

        private IDType idField;

        private DateTimeType creationDateTimeField;

        private CodeType statusCodeField;

        private AgronomicalObservationPartyType issuerAgronomicalObservationPartyField;

        private AgronomicalObservationPartyType[] recipientAgronomicalObservationPartyField;

        private AgronomicalObservationPartyType submitterAgronomicalObservationPartyField;

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

        public DateTimeType CreationDateTime
        {
            get
            {
                return this.creationDateTimeField;
            }
            set
            {
                this.creationDateTimeField = value;
            }
        }

        public CodeType StatusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        public AgronomicalObservationPartyType IssuerAgronomicalObservationParty
        {
            get
            {
                return this.issuerAgronomicalObservationPartyField;
            }
            set
            {
                this.issuerAgronomicalObservationPartyField = value;
            }
        }

        [XmlElement("RecipientAgronomicalObservationParty")]
        public AgronomicalObservationPartyType[] RecipientAgronomicalObservationParty
        {
            get
            {
                return this.recipientAgronomicalObservationPartyField;
            }
            set
            {
                this.recipientAgronomicalObservationPartyField = value;
            }
        }

        public AgronomicalObservationPartyType SubmitterAgronomicalObservationParty
        {
            get
            {
                return this.submitterAgronomicalObservationPartyField;
            }
            set
            {
                this.submitterAgronomicalObservationPartyField = value;
            }
        }
    }
}