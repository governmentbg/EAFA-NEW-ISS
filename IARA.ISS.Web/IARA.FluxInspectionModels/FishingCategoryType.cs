namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FishingCategory", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FishingCategoryType
    {

        private CodeType typeCodeField;

        private CodeType[] fishingMethodCodeField;

        private TextType[] fishingMethodField;

        private CodeType[] fishingAreaCodeField;

        private TextType[] fishingAreaField;

        private TextType specialConditionField;

        private FishingGearType[] authorizedFishingGearField;

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

        [XmlElement("FishingMethodCode")]
        public CodeType[] FishingMethodCode
        {
            get
            {
                return this.fishingMethodCodeField;
            }
            set
            {
                this.fishingMethodCodeField = value;
            }
        }

        [XmlElement("FishingMethod")]
        public TextType[] FishingMethod
        {
            get
            {
                return this.fishingMethodField;
            }
            set
            {
                this.fishingMethodField = value;
            }
        }

        [XmlElement("FishingAreaCode")]
        public CodeType[] FishingAreaCode
        {
            get
            {
                return this.fishingAreaCodeField;
            }
            set
            {
                this.fishingAreaCodeField = value;
            }
        }

        [XmlElement("FishingArea")]
        public TextType[] FishingArea
        {
            get
            {
                return this.fishingAreaField;
            }
            set
            {
                this.fishingAreaField = value;
            }
        }

        public TextType SpecialCondition
        {
            get
            {
                return this.specialConditionField;
            }
            set
            {
                this.specialConditionField = value;
            }
        }

        [XmlElement("AuthorizedFishingGear")]
        public FishingGearType[] AuthorizedFishingGear
        {
            get
            {
                return this.authorizedFishingGearField;
            }
            set
            {
                this.authorizedFishingGearField = value;
            }
        }
    }
}