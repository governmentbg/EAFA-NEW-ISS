namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("GuestRoomFacility", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class GuestRoomFacilityType
    {

        private IDType idField;

        private TextType descriptionField;

        private TextType architecturalStyleField;

        private TextType nameField;

        private DateType constructionDateField;

        private DateType latestRenovationDateField;

        private TextType typeField;

        private LodgingHouseLocationType physicalLodgingHouseLocationField;

        private LodgingHouseUsageConditionType[] specifiedLodgingHouseUsageConditionField;

        private SpecifiedPeriodType[] operatingSpecifiedPeriodField;

        private LodgingHousePictureType[] actualLodgingHousePictureField;

        private LodgingHouseFeatureType[] distinctiveLodgingHouseFeatureField;

        private SupplementalServiceType[] offeredSupplementalServiceField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public TextType ArchitecturalStyle
        {
            get => this.architecturalStyleField;
            set => this.architecturalStyleField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public DateType ConstructionDate
        {
            get => this.constructionDateField;
            set => this.constructionDateField = value;
        }

        public DateType LatestRenovationDate
        {
            get => this.latestRenovationDateField;
            set => this.latestRenovationDateField = value;
        }

        public TextType Type
        {
            get => this.typeField;
            set => this.typeField = value;
        }

        public LodgingHouseLocationType PhysicalLodgingHouseLocation
        {
            get => this.physicalLodgingHouseLocationField;
            set => this.physicalLodgingHouseLocationField = value;
        }

        [XmlElement("SpecifiedLodgingHouseUsageCondition")]
        public LodgingHouseUsageConditionType[] SpecifiedLodgingHouseUsageCondition
        {
            get => this.specifiedLodgingHouseUsageConditionField;
            set => this.specifiedLodgingHouseUsageConditionField = value;
        }

        [XmlElement("OperatingSpecifiedPeriod")]
        public SpecifiedPeriodType[] OperatingSpecifiedPeriod
        {
            get => this.operatingSpecifiedPeriodField;
            set => this.operatingSpecifiedPeriodField = value;
        }

        [XmlElement("ActualLodgingHousePicture")]
        public LodgingHousePictureType[] ActualLodgingHousePicture
        {
            get => this.actualLodgingHousePictureField;
            set => this.actualLodgingHousePictureField = value;
        }

        [XmlElement("DistinctiveLodgingHouseFeature")]
        public LodgingHouseFeatureType[] DistinctiveLodgingHouseFeature
        {
            get => this.distinctiveLodgingHouseFeatureField;
            set => this.distinctiveLodgingHouseFeatureField = value;
        }

        [XmlElement("OfferedSupplementalService")]
        public SupplementalServiceType[] OfferedSupplementalService
        {
            get => this.offeredSupplementalServiceField;
            set => this.offeredSupplementalServiceField = value;
        }
    }
}