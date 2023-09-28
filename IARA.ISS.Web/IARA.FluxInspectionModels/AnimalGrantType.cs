namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AnimalGrant", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AnimalGrantType
    {

        private DateTimeType allocationDateTimeField;

        private TextType applicationTypeField;

        private SpecifiedPartyType awardingSpecifiedPartyField;

        private SpecifiedPartyType receiverSpecifiedPartyField;

        public DateTimeType AllocationDateTime
        {
            get
            {
                return this.allocationDateTimeField;
            }
            set
            {
                this.allocationDateTimeField = value;
            }
        }

        public TextType ApplicationType
        {
            get
            {
                return this.applicationTypeField;
            }
            set
            {
                this.applicationTypeField = value;
            }
        }

        public SpecifiedPartyType AwardingSpecifiedParty
        {
            get
            {
                return this.awardingSpecifiedPartyField;
            }
            set
            {
                this.awardingSpecifiedPartyField = value;
            }
        }

        public SpecifiedPartyType ReceiverSpecifiedParty
        {
            get
            {
                return this.receiverSpecifiedPartyField;
            }
            set
            {
                this.receiverSpecifiedPartyField = value;
            }
        }
    }
}