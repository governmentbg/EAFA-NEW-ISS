namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("BusinessSupportRoomFacility", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class BusinessSupportRoomFacilityType
    {

        private IDType idField;

        private TextType descriptionField;

        private TextType architecturalStyleField;

        private TextType nameField;

        private DateType constructionDateField;

        private DateType latestRenovationDateField;

        private LodgingHouseLocationType physicalLodgingHouseLocationField;

        private LodgingHouseUsageConditionType[] specifiedLodgingHouseUsageConditionField;

        private SpecifiedPeriodType[] operatingSpecifiedPeriodField;

        private LodgingHousePictureType[] actualLodgingHousePictureField;

        private LodgingHouseFeatureType[] distinctiveLodgingHouseFeatureField;

        private SupplementalServiceType[] offeredSupplementalServiceField;

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

        public TextType ArchitecturalStyle
        {
            get
            {
                return this.architecturalStyleField;
            }
            set
            {
                this.architecturalStyleField = value;
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

        public DateType ConstructionDate
        {
            get
            {
                return this.constructionDateField;
            }
            set
            {
                this.constructionDateField = value;
            }
        }

        public DateType LatestRenovationDate
        {
            get
            {
                return this.latestRenovationDateField;
            }
            set
            {
                this.latestRenovationDateField = value;
            }
        }

        public LodgingHouseLocationType PhysicalLodgingHouseLocation
        {
            get
            {
                return this.physicalLodgingHouseLocationField;
            }
            set
            {
                this.physicalLodgingHouseLocationField = value;
            }
        }

        [XmlElement("SpecifiedLodgingHouseUsageCondition")]
        public LodgingHouseUsageConditionType[] SpecifiedLodgingHouseUsageCondition
        {
            get
            {
                return this.specifiedLodgingHouseUsageConditionField;
            }
            set
            {
                this.specifiedLodgingHouseUsageConditionField = value;
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

        [XmlElement("DistinctiveLodgingHouseFeature")]
        public LodgingHouseFeatureType[] DistinctiveLodgingHouseFeature
        {
            get
            {
                return this.distinctiveLodgingHouseFeatureField;
            }
            set
            {
                this.distinctiveLodgingHouseFeatureField = value;
            }
        }

        [XmlElement("OfferedSupplementalService")]
        public SupplementalServiceType[] OfferedSupplementalService
        {
            get
            {
                return this.offeredSupplementalServiceField;
            }
            set
            {
                this.offeredSupplementalServiceField = value;
            }
        }
    }
}