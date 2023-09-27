namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TradeProductSupplyChainPackaging", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TradeProductSupplyChainPackagingType
    {

        private QuantityType customerFacingTotalUnitQuantityField;

        private QuantityType[] totalUnitQuantityField;

        private QuantityType layerTotalUnitQuantityField;

        private IndicatorType returnableIndicatorField;

        private PackageTypeCodeType typeCodeField;

        private CISpatialDimensionType linearCISpatialDimensionField;

        private CIPackagingMarkingType[] specifiedCIPackagingMarkingField;

        private CIMaterialGoodsCharacteristicType[] applicableCIMaterialGoodsCharacteristicField;

        private CIDisposalInstructionsType[] applicableCIDisposalInstructionsField;

        private CIReturnableAssetInstructionsType[] applicableCIReturnableAssetInstructionsField;

        public QuantityType CustomerFacingTotalUnitQuantity
        {
            get
            {
                return this.customerFacingTotalUnitQuantityField;
            }
            set
            {
                this.customerFacingTotalUnitQuantityField = value;
            }
        }

        [XmlElement("TotalUnitQuantity")]
        public QuantityType[] TotalUnitQuantity
        {
            get
            {
                return this.totalUnitQuantityField;
            }
            set
            {
                this.totalUnitQuantityField = value;
            }
        }

        public QuantityType LayerTotalUnitQuantity
        {
            get
            {
                return this.layerTotalUnitQuantityField;
            }
            set
            {
                this.layerTotalUnitQuantityField = value;
            }
        }

        public IndicatorType ReturnableIndicator
        {
            get
            {
                return this.returnableIndicatorField;
            }
            set
            {
                this.returnableIndicatorField = value;
            }
        }

        public PackageTypeCodeType TypeCode
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

        [XmlElement("SpecifiedCIPackagingMarking")]
        public CIPackagingMarkingType[] SpecifiedCIPackagingMarking
        {
            get
            {
                return this.specifiedCIPackagingMarkingField;
            }
            set
            {
                this.specifiedCIPackagingMarkingField = value;
            }
        }

        [XmlElement("ApplicableCIMaterialGoodsCharacteristic")]
        public CIMaterialGoodsCharacteristicType[] ApplicableCIMaterialGoodsCharacteristic
        {
            get
            {
                return this.applicableCIMaterialGoodsCharacteristicField;
            }
            set
            {
                this.applicableCIMaterialGoodsCharacteristicField = value;
            }
        }

        [XmlElement("ApplicableCIDisposalInstructions")]
        public CIDisposalInstructionsType[] ApplicableCIDisposalInstructions
        {
            get
            {
                return this.applicableCIDisposalInstructionsField;
            }
            set
            {
                this.applicableCIDisposalInstructionsField = value;
            }
        }

        [XmlElement("ApplicableCIReturnableAssetInstructions")]
        public CIReturnableAssetInstructionsType[] ApplicableCIReturnableAssetInstructions
        {
            get
            {
                return this.applicableCIReturnableAssetInstructionsField;
            }
            set
            {
                this.applicableCIReturnableAssetInstructionsField = value;
            }
        }
    }
}