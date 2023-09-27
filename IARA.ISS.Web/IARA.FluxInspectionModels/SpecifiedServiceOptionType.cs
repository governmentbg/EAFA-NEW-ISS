namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedServiceOption", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedServiceOptionType
    {

        private CodeType typeCodeField;

        private TextType nameField;

        private TextType maximumAvailabilityField;

        private TextType publishedPriceField;

        private TextType descriptionField;

        private LodgingHouseUsageConditionType[] applicableLodgingHouseUsageConditionField;

        private SpecifiedPeriodType[] operatingSpecifiedPeriodField;

        private LodgingHousePictureType[] actualLodgingHousePictureField;

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

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public TextType MaximumAvailability
        {
            get
            {
                return this.maximumAvailabilityField;
            }
            set
            {
                this.maximumAvailabilityField = value;
            }
        }

        public TextType PublishedPrice
        {
            get
            {
                return this.publishedPriceField;
            }
            set
            {
                this.publishedPriceField = value;
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

        [XmlElement("ApplicableLodgingHouseUsageCondition")]
        public LodgingHouseUsageConditionType[] ApplicableLodgingHouseUsageCondition
        {
            get
            {
                return this.applicableLodgingHouseUsageConditionField;
            }
            set
            {
                this.applicableLodgingHouseUsageConditionField = value;
            }
        }

        [XmlElement("OperatingSpecifiedPeriod")]
        public SpecifiedPeriodType[] OperatingSpecifiedPeriod
        {
            get
            {
                return this.operatingSpecifiedPeriodField;
            }
            set
            {
                this.operatingSpecifiedPeriodField = value;
            }
        }

        [XmlElement("ActualLodgingHousePicture")]
        public LodgingHousePictureType[] ActualLodgingHousePicture
        {
            get
            {
                return this.actualLodgingHousePictureField;
            }
            set
            {
                this.actualLodgingHousePictureField = value;
            }
        }
    }
}