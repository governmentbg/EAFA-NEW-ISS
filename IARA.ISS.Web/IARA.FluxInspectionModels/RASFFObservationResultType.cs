namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFObservationResult", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFObservationResultType
    {

        private SampleObservationResultCharacteristicType[] specifiedSampleObservationResultCharacteristicField;

        private RASFFPartyType authorizationRASFFPartyField;

        private RASFFNoteType attachedRASFFNoteField;

        [XmlElement("SpecifiedSampleObservationResultCharacteristic")]
        public SampleObservationResultCharacteristicType[] SpecifiedSampleObservationResultCharacteristic
        {
            get => this.specifiedSampleObservationResultCharacteristicField;
            set => this.specifiedSampleObservationResultCharacteristicField = value;
        }

        public RASFFPartyType AuthorizationRASFFParty
        {
            get => this.authorizationRASFFPartyField;
            set => this.authorizationRASFFPartyField = value;
        }

        public RASFFNoteType AttachedRASFFNote
        {
            get => this.attachedRASFFNoteField;
            set => this.attachedRASFFNoteField = value;
        }
    }
}