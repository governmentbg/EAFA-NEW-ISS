namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselTransportCharter", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselTransportCharterType
    {

        private CodeType typeCodeField;

        private DelimitedPeriodType[] applicableDelimitedPeriodField;

        private ContactPartyType[] specifiedContactPartyField;

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

        [XmlElement("ApplicableDelimitedPeriod")]
        public DelimitedPeriodType[] ApplicableDelimitedPeriod
        {
            get
            {
                return this.applicableDelimitedPeriodField;
            }
            set
            {
                this.applicableDelimitedPeriodField = value;
            }
        }

        [XmlElement("SpecifiedContactParty")]
        public ContactPartyType[] SpecifiedContactParty
        {
            get
            {
                return this.specifiedContactPartyField;
            }
            set
            {
                this.specifiedContactPartyField = value;
            }
        }
    }
}