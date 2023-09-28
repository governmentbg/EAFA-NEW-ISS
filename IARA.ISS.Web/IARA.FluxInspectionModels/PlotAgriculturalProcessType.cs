namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("PlotAgriculturalProcess", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PlotAgriculturalProcessType
    {

        private CodeType typeCodeField;

        private CodeType subordinateTypeCodeField;

        private TextType descriptionField;

        private DelimitedPeriodType[] occurrenceDelimitedPeriodField;

        private AgriculturalProcessWorkItemType[] performedAgriculturalProcessWorkItemField;

        private AgriculturalProcessReasonType[] specifiedAgriculturalProcessReasonField;

        private CropDataSheetPartyType internalOperatorCropDataSheetPartyField;

        private CropDataSheetPartyType externalOperatorCropDataSheetPartyField;

        private SpecifiedAgriculturalDeviceType[] usedSpecifiedAgriculturalDeviceField;

        private AgriculturalAreaType[] occurrenceAgriculturalAreaField;

        private AgriculturalProcessCropInputType[] usedAgriculturalProcessCropInputField;

        private AgriculturalProduceType[] harvestedAgriculturalProduceField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

        private AgriculturalProcessTargetObjectType[] specifiedAgriculturalProcessTargetObjectField;

        private AgriculturalProcessCropStageType[] specifiedAgriculturalProcessCropStageField;

        private AgriculturalProcessConditionType[] specifiedAgriculturalProcessConditionField;

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

        public CodeType SubordinateTypeCode
        {
            get
            {
                return this.subordinateTypeCodeField;
            }
            set
            {
                this.subordinateTypeCodeField = value;
            }
        }

        public TextType Description
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

        [XmlElement("OccurrenceDelimitedPeriod")]
        public DelimitedPeriodType[] OccurrenceDelimitedPeriod
        {
            get
            {
                return this.occurrenceDelimitedPeriodField;
            }
            set
            {
                this.occurrenceDelimitedPeriodField = value;
            }
        }

        [XmlElement("PerformedAgriculturalProcessWorkItem")]
        public AgriculturalProcessWorkItemType[] PerformedAgriculturalProcessWorkItem
        {
            get
            {
                return this.performedAgriculturalProcessWorkItemField;
            }
            set
            {
                this.performedAgriculturalProcessWorkItemField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalProcessReason")]
        public AgriculturalProcessReasonType[] SpecifiedAgriculturalProcessReason
        {
            get
            {
                return this.specifiedAgriculturalProcessReasonField;
            }
            set
            {
                this.specifiedAgriculturalProcessReasonField = value;
            }
        }

        public CropDataSheetPartyType InternalOperatorCropDataSheetParty
        {
            get
            {
                return this.internalOperatorCropDataSheetPartyField;
            }
            set
            {
                this.internalOperatorCropDataSheetPartyField = value;
            }
        }

        public CropDataSheetPartyType ExternalOperatorCropDataSheetParty
        {
            get
            {
                return this.externalOperatorCropDataSheetPartyField;
            }
            set
            {
                this.externalOperatorCropDataSheetPartyField = value;
            }
        }

        [XmlElement("UsedSpecifiedAgriculturalDevice")]
        public SpecifiedAgriculturalDeviceType[] UsedSpecifiedAgriculturalDevice
        {
            get
            {
                return this.usedSpecifiedAgriculturalDeviceField;
            }
            set
            {
                this.usedSpecifiedAgriculturalDeviceField = value;
            }
        }

        [XmlElement("OccurrenceAgriculturalArea")]
        public AgriculturalAreaType[] OccurrenceAgriculturalArea
        {
            get
            {
                return this.occurrenceAgriculturalAreaField;
            }
            set
            {
                this.occurrenceAgriculturalAreaField = value;
            }
        }

        [XmlElement("UsedAgriculturalProcessCropInput")]
        public AgriculturalProcessCropInputType[] UsedAgriculturalProcessCropInput
        {
            get
            {
                return this.usedAgriculturalProcessCropInputField;
            }
            set
            {
                this.usedAgriculturalProcessCropInputField = value;
            }
        }

        [XmlElement("HarvestedAgriculturalProduce")]
        public AgriculturalProduceType[] HarvestedAgriculturalProduce
        {
            get
            {
                return this.harvestedAgriculturalProduceField;
            }
            set
            {
                this.harvestedAgriculturalProduceField = value;
            }
        }

        [XmlElement("ApplicableTechnicalCharacteristic")]
        public TechnicalCharacteristicType[] ApplicableTechnicalCharacteristic
        {
            get
            {
                return this.applicableTechnicalCharacteristicField;
            }
            set
            {
                this.applicableTechnicalCharacteristicField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalProcessTargetObject")]
        public AgriculturalProcessTargetObjectType[] SpecifiedAgriculturalProcessTargetObject
        {
            get
            {
                return this.specifiedAgriculturalProcessTargetObjectField;
            }
            set
            {
                this.specifiedAgriculturalProcessTargetObjectField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalProcessCropStage")]
        public AgriculturalProcessCropStageType[] SpecifiedAgriculturalProcessCropStage
        {
            get
            {
                return this.specifiedAgriculturalProcessCropStageField;
            }
            set
            {
                this.specifiedAgriculturalProcessCropStageField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalProcessCondition")]
        public AgriculturalProcessConditionType[] SpecifiedAgriculturalProcessCondition
        {
            get
            {
                return this.specifiedAgriculturalProcessConditionField;
            }
            set
            {
                this.specifiedAgriculturalProcessConditionField = value;
            }
        }
    }
}