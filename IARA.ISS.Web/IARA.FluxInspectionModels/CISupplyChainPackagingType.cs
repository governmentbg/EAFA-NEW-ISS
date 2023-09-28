namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISupplyChainPackaging", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISupplyChainPackagingType
    {

        private PackageTypeCodeType typeCodeField;

        private TextType typeField;

        private CodeType conditionCodeField;

        private CodeType[] disposalMethodCodeField;

        private MeasureType[] weightMeasureField;

        private QuantityType maximumStackabilityQuantityField;

        private WeightUnitMeasureType maximumStackabilityWeightMeasureField;

        private TextType[] descriptionField;

        private QuantityType customerFacingTotalUnitQuantityField;

        private QuantityType layerTotalUnitQuantityField;

        private QuantityType contentLayerQuantityField;

        private CodeType additionalInstructionCodeField;

        private CodeType instructionCodeField;

        private IndicatorType additionalInstructionIndicatorField;

        private CISpatialDimensionType linearCISpatialDimensionField;

        private CISpatialDimensionType minimumLinearCISpatialDimensionField;

        private CISpatialDimensionType maximumLinearCISpatialDimensionField;

        private CIPackagingMarkingType[] specifiedCIPackagingMarkingField;

        private CIMaterialGoodsCharacteristicType[] applicableCIMaterialGoodsCharacteristicField;

        private CIDisposalInstructionsType[] applicableCIDisposalInstructionsField;

        private CIReturnableAssetInstructionsType[] applicableCIReturnableAssetInstructionsField;

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

        public CodeType ConditionCode
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

        [XmlElement("DisposalMethodCode")]
        public CodeType[] DisposalMethodCode
        {
            get
            {
                return this.disposalMethodCodeField;
            }
            set
            {
                this.disposalMethodCodeField = value;
            }
        }

        [XmlElement("WeightMeasure")]
        public MeasureType[] WeightMeasure
        {
            get
            {
                return this.weightMeasureField;
            }
            set
            {
                this.weightMeasureField = value;
            }
        }

        public QuantityType MaximumStackabilityQuantity
        {
            get
            {
                return this.maximumStackabilityQuantityField;
            }
            set
            {
                this.maximumStackabilityQuantityField = value;
            }
        }

        public WeightUnitMeasureType MaximumStackabilityWeightMeasure
        {
            get
            {
                return this.maximumStackabilityWeightMeasureField;
            }
            set
            {
                this.maximumStackabilityWeightMeasureField = value;
            }
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

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

        public QuantityType ContentLayerQuantity
        {
            get
            {
                return this.contentLayerQuantityField;
            }
            set
            {
                this.contentLayerQuantityField = value;
            }
        }

        public CodeType AdditionalInstructionCode
        {
            get
            {
                return this.additionalInstructionCodeField;
            }
            set
            {
                this.additionalInstructionCodeField = value;
            }
        }

        public CodeType InstructionCode
        {
            get
            {
                return this.instructionCodeField;
            }
            set
            {
                this.instructionCodeField = value;
            }
        }

        public IndicatorType AdditionalInstructionIndicator
        {
            get
            {
                return this.additionalInstructionIndicatorField;
            }
            set
            {
                this.additionalInstructionIndicatorField = value;
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

        public CISpatialDimensionType MinimumLinearCISpatialDimension
        {
            get
            {
                return this.minimumLinearCISpatialDimensionField;
            }
            set
            {
                this.minimumLinearCISpatialDimensionField = value;
            }
        }

        public CISpatialDimensionType MaximumLinearCISpatialDimension
        {
            get
            {
                return this.maximumLinearCISpatialDimensionField;
            }
            set
            {
                this.maximumLinearCISpatialDimensionField = value;
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