namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MSDSHazardousMaterial", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MSDSHazardousMaterialType
    {

        private CodeType[] typeCodeField;

        private TextType[] descriptionField;

        private IndicatorType humanEntryRouteIndicatorField;

        private CodeType exposureMeasureUnitCodeField;

        private TextType informationField;

        private TextType emergencyOverviewInformationField;

        private TextType overviewEntryRouteInformationField;

        private TextType primaryEntryRouteInformationField;

        private TextType[] additionalEntryRouteInformationField;

        private TextType[] overexposureEffectInformationField;

        private TextType targetOrganInformationField;

        private TextType environmentalHazardOverviewInformationField;

        private TextType exposureClassificationJurisdictionInformationField;

        private HazardRatingType[] applicableHazardRatingField;

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public IndicatorType HumanEntryRouteIndicator
        {
            get => this.humanEntryRouteIndicatorField;
            set => this.humanEntryRouteIndicatorField = value;
        }

        public CodeType ExposureMeasureUnitCode
        {
            get => this.exposureMeasureUnitCodeField;
            set => this.exposureMeasureUnitCodeField = value;
        }

        public TextType Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public TextType EmergencyOverviewInformation
        {
            get => this.emergencyOverviewInformationField;
            set => this.emergencyOverviewInformationField = value;
        }

        public TextType OverviewEntryRouteInformation
        {
            get => this.overviewEntryRouteInformationField;
            set => this.overviewEntryRouteInformationField = value;
        }

        public TextType PrimaryEntryRouteInformation
        {
            get => this.primaryEntryRouteInformationField;
            set => this.primaryEntryRouteInformationField = value;
        }

        [XmlElement("AdditionalEntryRouteInformation")]
        public TextType[] AdditionalEntryRouteInformation
        {
            get => this.additionalEntryRouteInformationField;
            set => this.additionalEntryRouteInformationField = value;
        }

        [XmlElement("OverexposureEffectInformation")]
        public TextType[] OverexposureEffectInformation
        {
            get => this.overexposureEffectInformationField;
            set => this.overexposureEffectInformationField = value;
        }

        public TextType TargetOrganInformation
        {
            get => this.targetOrganInformationField;
            set => this.targetOrganInformationField = value;
        }

        public TextType EnvironmentalHazardOverviewInformation
        {
            get => this.environmentalHazardOverviewInformationField;
            set => this.environmentalHazardOverviewInformationField = value;
        }

        public TextType ExposureClassificationJurisdictionInformation
        {
            get => this.exposureClassificationJurisdictionInformationField;
            set => this.exposureClassificationJurisdictionInformationField = value;
        }

        [XmlElement("ApplicableHazardRating")]
        public HazardRatingType[] ApplicableHazardRating
        {
            get => this.applicableHazardRatingField;
            set => this.applicableHazardRatingField = value;
        }
    }
}