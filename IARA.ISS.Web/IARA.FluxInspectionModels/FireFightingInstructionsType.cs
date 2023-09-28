namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FireFightingInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FireFightingInstructionsType
    {

        private TextType fireAndExplosionHazardHandlingField;

        private TextType flammabilityConditionHandlingField;

        private TextType hazardousCombustionProductHandlingField;

        private TextType specialProcedureField;

        private TextType[] extinguishingMediumItemNameField;

        private TextType flashpointFireProcedureField;

        private TextType autoignitionFireProcedureField;

        private TextType[] unsuitableExtinguishingMediumItemNameField;

        private TextType generalSafetyProcedureField;

        private TextType[] personalProtectiveEquipmentItemNameField;

        public TextType FireAndExplosionHazardHandling
        {
            get => this.fireAndExplosionHazardHandlingField;
            set => this.fireAndExplosionHazardHandlingField = value;
        }

        public TextType FlammabilityConditionHandling
        {
            get => this.flammabilityConditionHandlingField;
            set => this.flammabilityConditionHandlingField = value;
        }

        public TextType HazardousCombustionProductHandling
        {
            get => this.hazardousCombustionProductHandlingField;
            set => this.hazardousCombustionProductHandlingField = value;
        }

        public TextType SpecialProcedure
        {
            get => this.specialProcedureField;
            set => this.specialProcedureField = value;
        }

        [XmlElement("ExtinguishingMediumItemName")]
        public TextType[] ExtinguishingMediumItemName
        {
            get => this.extinguishingMediumItemNameField;
            set => this.extinguishingMediumItemNameField = value;
        }

        public TextType FlashpointFireProcedure
        {
            get => this.flashpointFireProcedureField;
            set => this.flashpointFireProcedureField = value;
        }

        public TextType AutoignitionFireProcedure
        {
            get => this.autoignitionFireProcedureField;
            set => this.autoignitionFireProcedureField = value;
        }

        [XmlElement("UnsuitableExtinguishingMediumItemName")]
        public TextType[] UnsuitableExtinguishingMediumItemName
        {
            get => this.unsuitableExtinguishingMediumItemNameField;
            set => this.unsuitableExtinguishingMediumItemNameField = value;
        }

        public TextType GeneralSafetyProcedure
        {
            get => this.generalSafetyProcedureField;
            set => this.generalSafetyProcedureField = value;
        }

        [XmlElement("PersonalProtectiveEquipmentItemName")]
        public TextType[] PersonalProtectiveEquipmentItemName
        {
            get => this.personalProtectiveEquipmentItemNameField;
            set => this.personalProtectiveEquipmentItemNameField = value;
        }
    }
}