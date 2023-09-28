namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SingleAgronomicalObservation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SingleAgronomicalObservationType
    {

        private IDType idField;

        private DateTimeType collectionDateTimeField;

        private TextType commentField;

        private CodeType statusCodeField;

        private SpecifiedLocationType occurrenceSpecifiedLocationField;

        private AgronomicalObservationTargetObjectType[] specifiedAgronomicalObservationTargetObjectField;

        private AgriculturalCropType subjectAgriculturalCropField;

        private PartyContactType specifiedPartyContactField;

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

        public DateTimeType CollectionDateTime
        {
            get
            {
                return this.collectionDateTimeField;
            }
            set
            {
                this.collectionDateTimeField = value;
            }
        }

        public TextType Comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
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

        public SpecifiedLocationType OccurrenceSpecifiedLocation
        {
            get
            {
                return this.occurrenceSpecifiedLocationField;
            }
            set
            {
                this.occurrenceSpecifiedLocationField = value;
            }
        }

        [XmlElement("SpecifiedAgronomicalObservationTargetObject")]
        public AgronomicalObservationTargetObjectType[] SpecifiedAgronomicalObservationTargetObject
        {
            get
            {
                return this.specifiedAgronomicalObservationTargetObjectField;
            }
            set
            {
                this.specifiedAgronomicalObservationTargetObjectField = value;
            }
        }

        public AgriculturalCropType SubjectAgriculturalCrop
        {
            get
            {
                return this.subjectAgriculturalCropField;
            }
            set
            {
                this.subjectAgriculturalCropField = value;
            }
        }

        public PartyContactType SpecifiedPartyContact
        {
            get
            {
                return this.specifiedPartyContactField;
            }
            set
            {
                this.specifiedPartyContactField = value;
            }
        }
    }
}