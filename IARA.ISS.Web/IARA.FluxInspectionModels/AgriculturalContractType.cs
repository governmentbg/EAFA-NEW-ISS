namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalContract", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalContractType
    {

        private IDType idField;

        private IDType[] thirdPartyIssuedIDField;

        private DateTimeType[] signedDateTimeField;

        private SpecifiedPartyType buyerSpecifiedPartyField;

        private SpecifiedPartyType sellerSpecifiedPartyField;

        private SpecifiedPartyType[] additionalIdentifiedSpecifiedPartyField;

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

        [XmlElement("ThirdPartyIssuedID")]
        public IDType[] ThirdPartyIssuedID
        {
            get
            {
                return this.thirdPartyIssuedIDField;
            }
            set
            {
                this.thirdPartyIssuedIDField = value;
            }
        }

        [XmlElement("SignedDateTime")]
        public DateTimeType[] SignedDateTime
        {
            get
            {
                return this.signedDateTimeField;
            }
            set
            {
                this.signedDateTimeField = value;
            }
        }

        public SpecifiedPartyType BuyerSpecifiedParty
        {
            get
            {
                return this.buyerSpecifiedPartyField;
            }
            set
            {
                this.buyerSpecifiedPartyField = value;
            }
        }

        public SpecifiedPartyType SellerSpecifiedParty
        {
            get
            {
                return this.sellerSpecifiedPartyField;
            }
            set
            {
                this.sellerSpecifiedPartyField = value;
            }
        }

        [XmlElement("AdditionalIdentifiedSpecifiedParty")]
        public SpecifiedPartyType[] AdditionalIdentifiedSpecifiedParty
        {
            get
            {
                return this.additionalIdentifiedSpecifiedPartyField;
            }
            set
            {
                this.additionalIdentifiedSpecifiedPartyField = value;
            }
        }
    }
}