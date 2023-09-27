namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseUsageCondition", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseUsageConditionType
    {

        private IDType idField;

        private IndicatorType groupApplicableIndicatorField;

        private IndicatorType reservationRequiredIndicatorField;

        private TextType descriptionField;

        private TextType groupAllowedDurationField;

        private TextType genderLimitationField;

        private TextType ageLimitationField;

        private TextType physicalCharacteristicField;

        private TextType appropriateClothingField;

        private TextType maximumOccupancyField;

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

        public IndicatorType GroupApplicableIndicator
        {
            get
            {
                return this.groupApplicableIndicatorField;
            }
            set
            {
                this.groupApplicableIndicatorField = value;
            }
        }

        public IndicatorType ReservationRequiredIndicator
        {
            get
            {
                return this.reservationRequiredIndicatorField;
            }
            set
            {
                this.reservationRequiredIndicatorField = value;
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

        public TextType GroupAllowedDuration
        {
            get
            {
                return this.groupAllowedDurationField;
            }
            set
            {
                this.groupAllowedDurationField = value;
            }
        }

        public TextType GenderLimitation
        {
            get
            {
                return this.genderLimitationField;
            }
            set
            {
                this.genderLimitationField = value;
            }
        }

        public TextType AgeLimitation
        {
            get
            {
                return this.ageLimitationField;
            }
            set
            {
                this.ageLimitationField = value;
            }
        }

        public TextType PhysicalCharacteristic
        {
            get
            {
                return this.physicalCharacteristicField;
            }
            set
            {
                this.physicalCharacteristicField = value;
            }
        }

        public TextType AppropriateClothing
        {
            get
            {
                return this.appropriateClothingField;
            }
            set
            {
                this.appropriateClothingField = value;
            }
        }

        public TextType MaximumOccupancy
        {
            get
            {
                return this.maximumOccupancyField;
            }
            set
            {
                this.maximumOccupancyField = value;
            }
        }
    }
}