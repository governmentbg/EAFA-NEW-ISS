namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CILogisticsSeal", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CILogisticsSealType
    {

        private IDType[] idField;

        private IDType maximumIDField;

        private SealTypeCodeType typeCodeField;

        private SealConditionCodeType[] conditionCodeField;

        private SealingPartyRoleCodeType sealingPartyRoleCodeField;

        private TextType sealingPartyRoleField;

        private CITradePartyType issuingCITradePartyField;

        [XmlElement("ID")]
        public IDType[] ID
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

        public IDType MaximumID
        {
            get
            {
                return this.maximumIDField;
            }
            set
            {
                this.maximumIDField = value;
            }
        }

        public SealTypeCodeType TypeCode
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

        [XmlElement("ConditionCode")]
        public SealConditionCodeType[] ConditionCode
        {
            get
            {
                return this.conditionCodeField;
            }
            set
            {
                this.conditionCodeField = value;
            }
        }

        public SealingPartyRoleCodeType SealingPartyRoleCode
        {
            get
            {
                return this.sealingPartyRoleCodeField;
            }
            set
            {
                this.sealingPartyRoleCodeField = value;
            }
        }

        public TextType SealingPartyRole
        {
            get
            {
                return this.sealingPartyRoleField;
            }
            set
            {
                this.sealingPartyRoleField = value;
            }
        }

        public CITradePartyType IssuingCITradeParty
        {
            get
            {
                return this.issuingCITradePartyField;
            }
            set
            {
                this.issuingCITradePartyField = value;
            }
        }
    }
}