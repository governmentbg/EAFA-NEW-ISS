namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CILogisticsTransportEquipment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CILogisticsTransportEquipmentType
    {

        private IDType idField;

        private TransportEquipmentCategoryCodeType categoryCodeField;

        private TransportEquipmentSizeTypeCodeType characteristicCodeField;

        private LinearUnitMeasureType loadingLengthMeasureField;

        private TransportEquipmentFullnessCodeType usedCapacityCodeField;

        private IDType carrierAssignedBookingIDField;

        private TextType characteristicField;

        private CISpatialDimensionType linearCISpatialDimensionField;

        private CITradePartyType[] notifiedCITradePartyField;

        private CILogisticsSealType[] affixedCILogisticsSealField;

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

        public TransportEquipmentCategoryCodeType CategoryCode
        {
            get
            {
                return this.categoryCodeField;
            }
            set
            {
                this.categoryCodeField = value;
            }
        }

        public TransportEquipmentSizeTypeCodeType CharacteristicCode
        {
            get
            {
                return this.characteristicCodeField;
            }
            set
            {
                this.characteristicCodeField = value;
            }
        }

        public LinearUnitMeasureType LoadingLengthMeasure
        {
            get
            {
                return this.loadingLengthMeasureField;
            }
            set
            {
                this.loadingLengthMeasureField = value;
            }
        }

        public TransportEquipmentFullnessCodeType UsedCapacityCode
        {
            get
            {
                return this.usedCapacityCodeField;
            }
            set
            {
                this.usedCapacityCodeField = value;
            }
        }

        public IDType CarrierAssignedBookingID
        {
            get
            {
                return this.carrierAssignedBookingIDField;
            }
            set
            {
                this.carrierAssignedBookingIDField = value;
            }
        }

        public TextType Characteristic
        {
            get
            {
                return this.characteristicField;
            }
            set
            {
                this.characteristicField = value;
            }
        }

        public CISpatialDimensionType LinearCISpatialDimension
        {
            get
            {
                return this.linearCISpatialDimensionField;
            }
            set
            {
                this.linearCISpatialDimensionField = value;
            }
        }

        [XmlElement("NotifiedCITradeParty")]
        public CITradePartyType[] NotifiedCITradeParty
        {
            get
            {
                return this.notifiedCITradePartyField;
            }
            set
            {
                this.notifiedCITradePartyField = value;
            }
        }

        [XmlElement("AffixedCILogisticsSeal")]
        public CILogisticsSealType[] AffixedCILogisticsSeal
        {
            get
            {
                return this.affixedCILogisticsSealField;
            }
            set
            {
                this.affixedCILogisticsSealField = value;
            }
        }
    }
}