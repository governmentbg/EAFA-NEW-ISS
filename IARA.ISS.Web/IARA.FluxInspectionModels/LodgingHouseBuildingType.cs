namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseBuilding", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseBuildingType
    {

        private IDType[] idField;

        private TextType[] formalNameField;

        private TextType familiarNameField;

        private TextType architecturalStyleField;

        private DateType originalConstructionDateField;

        private DateType latestRenovationDateField;

        private TextType descriptionField;

        private LodgingHousePictureType[] actualLodgingHousePictureField;

        private LodgingHouseFeatureType[] distinctiveLodgingHouseFeatureField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("FormalName")]
        public TextType[] FormalName
        {
            get => this.formalNameField;
            set => this.formalNameField = value;
        }

        public TextType FamiliarName
        {
            get => this.familiarNameField;
            set => this.familiarNameField = value;
        }

        public TextType ArchitecturalStyle
        {
            get => this.architecturalStyleField;
            set => this.architecturalStyleField = value;
        }

        public DateType OriginalConstructionDate
        {
            get => this.originalConstructionDateField;
            set => this.originalConstructionDateField = value;
        }

        public DateType LatestRenovationDate
        {
            get => this.latestRenovationDateField;
            set => this.latestRenovationDateField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
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
    }
}