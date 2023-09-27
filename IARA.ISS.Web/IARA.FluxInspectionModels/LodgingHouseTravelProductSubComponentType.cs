namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseTravelProductSubComponent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseTravelProductSubComponentType
    {

        private CodeType categoryCodeField;

        private CodeType categoryTypeCodeField;

        private IDType idField;

        private TextType nameField;

        private IDType supplierIDField;

        private TextType supplierNameField;

        private QuantityType applicableUnitQuantityField;

        private TextType descriptionField;

        private TravelProductFeatureType[] distinctiveTravelProductFeatureField;

        private SpecifiedServiceOptionType[] specifiedServiceOptionField;

        private PictureType[] actualPictureField;

        private UsageConditionType[] applicableUsageConditionField;

        private LodgingHouseLocationType physicalLodgingHouseLocationField;

        private ProductAvailabilityType[] specifiedProductAvailabilityField;

        private LodgingHousePictureType[] actualLodgingHousePictureField;

        private LodgingHouseUsageConditionType[] applicableLodgingHouseUsageConditionField;

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public CodeType CategoryTypeCode
        {
            get => this.categoryTypeCodeField;
            set => this.categoryTypeCodeField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public IDType SupplierID
        {
            get => this.supplierIDField;
            set => this.supplierIDField = value;
        }

        public TextType SupplierName
        {
            get => this.supplierNameField;
            set => this.supplierNameField = value;
        }

        public QuantityType ApplicableUnitQuantity
        {
            get => this.applicableUnitQuantityField;
            set => this.applicableUnitQuantityField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("DistinctiveTravelProductFeature")]
        public TravelProductFeatureType[] DistinctiveTravelProductFeature
        {
            get => this.distinctiveTravelProductFeatureField;
            set => this.distinctiveTravelProductFeatureField = value;
        }

        [XmlElement("SpecifiedServiceOption")]
        public SpecifiedServiceOptionType[] SpecifiedServiceOption
        {
            get => this.specifiedServiceOptionField;
            set => this.specifiedServiceOptionField = value;
        }

        [XmlElement("ActualPicture")]
        public PictureType[] ActualPicture
        {
            get => this.actualPictureField;
            set => this.actualPictureField = value;
        }

        [XmlElement("ApplicableUsageCondition")]
        public UsageConditionType[] ApplicableUsageCondition
        {
            get => this.applicableUsageConditionField;
            set => this.applicableUsageConditionField = value;
        }

        public LodgingHouseLocationType PhysicalLodgingHouseLocation
        {
            get => this.physicalLodgingHouseLocationField;
            set => this.physicalLodgingHouseLocationField = value;
        }

        [XmlElement("SpecifiedProductAvailability")]
        public ProductAvailabilityType[] SpecifiedProductAvailability
        {
            get => this.specifiedProductAvailabilityField;
            set => this.specifiedProductAvailabilityField = value;
        }

        [XmlElement("ActualLodgingHousePicture")]
        public LodgingHousePictureType[] ActualLodgingHousePicture
        {
            get => this.actualLodgingHousePictureField;
            set => this.actualLodgingHousePictureField = value;
        }

        [XmlElement("ApplicableLodgingHouseUsageCondition")]
        public LodgingHouseUsageConditionType[] ApplicableLodgingHouseUsageCondition
        {
            get => this.applicableLodgingHouseUsageConditionField;
            set => this.applicableLodgingHouseUsageConditionField = value;
        }
    }
}