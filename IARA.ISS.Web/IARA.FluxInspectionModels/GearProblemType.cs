namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("GearProblem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class GearProblemType
    {

        private CodeType typeCodeField;

        private QuantityType affectedQuantityField;

        private CodeType[] recoveryMeasureCodeField;

        private FLUXLocationType[] specifiedFLUXLocationField;

        private FishingGearType[] relatedFishingGearField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public QuantityType AffectedQuantity
        {
            get => this.affectedQuantityField;
            set => this.affectedQuantityField = value;
        }

        [XmlElement("RecoveryMeasureCode")]
        public CodeType[] RecoveryMeasureCode
        {
            get => this.recoveryMeasureCodeField;
            set => this.recoveryMeasureCodeField = value;
        }

        [XmlElement("SpecifiedFLUXLocation")]
        public FLUXLocationType[] SpecifiedFLUXLocation
        {
            get => this.specifiedFLUXLocationField;
            set => this.specifiedFLUXLocationField = value;
        }

        [XmlElement("RelatedFishingGear")]
        public FishingGearType[] RelatedFishingGear
        {
            get => this.relatedFishingGearField;
            set => this.relatedFishingGearField = value;
        }
    }
}