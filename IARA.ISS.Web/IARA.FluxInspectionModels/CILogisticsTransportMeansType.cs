namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CILogisticsTransportMeans", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CILogisticsTransportMeansType
    {

        private TransportMeansTypeCodeType typeCodeField;

        private TextType typeField;

        private IDType idField;

        private TextType nameField;

        private LinearUnitMeasureType requiredLaneLengthMeasureField;

        private CITradePartyType ownerCITradePartyField;

        public TransportMeansTypeCodeType TypeCode
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

        public TextType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
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

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public LinearUnitMeasureType RequiredLaneLengthMeasure
        {
            get
            {
                return this.requiredLaneLengthMeasureField;
            }
            set
            {
                this.requiredLaneLengthMeasureField = value;
            }
        }

        public CITradePartyType OwnerCITradeParty
        {
            get
            {
                return this.ownerCITradePartyField;
            }
            set
            {
                this.ownerCITradePartyField = value;
            }
        }
    }
}