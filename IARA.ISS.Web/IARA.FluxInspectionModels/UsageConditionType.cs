namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("UsageCondition", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class UsageConditionType
    {

        private TextType[] ageLimitationField;

        private IndicatorType[] applicableIndicatorField;

        private TextType appropriateClothingField;

        private TextType descriptionField;

        private TextType durationField;

        private TextType genderLimitationField;

        private IDType[] idField;

        private TextType[] occupancyField;

        private TextType[] physicalCharacteristicField;

        private IndicatorType requiredIndicatorField;

        [XmlElement("AgeLimitation")]
        public TextType[] AgeLimitation
        {
            get => this.ageLimitationField;
            set => this.ageLimitationField = value;
        }

        [XmlElement("ApplicableIndicator")]
        public IndicatorType[] ApplicableIndicator
        {
            get => this.applicableIndicatorField;
            set => this.applicableIndicatorField = value;
        }

        public TextType AppropriateClothing
        {
            get => this.appropriateClothingField;
            set => this.appropriateClothingField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public TextType Duration
        {
            get => this.durationField;
            set => this.durationField = value;
        }

        public TextType GenderLimitation
        {
            get => this.genderLimitationField;
            set => this.genderLimitationField = value;
        }

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Occupancy")]
        public TextType[] Occupancy
        {
            get => this.occupancyField;
            set => this.occupancyField = value;
        }

        [XmlElement("PhysicalCharacteristic")]
        public TextType[] PhysicalCharacteristic
        {
            get => this.physicalCharacteristicField;
            set => this.physicalCharacteristicField = value;
        }

        public IndicatorType RequiredIndicator
        {
            get => this.requiredIndicatorField;
            set => this.requiredIndicatorField = value;
        }
    }
}