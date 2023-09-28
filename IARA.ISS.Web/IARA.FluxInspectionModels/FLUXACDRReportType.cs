namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FLUXACDRReport", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FLUXACDRReportType
    {

        private CodeType regionalAreaCodeField;

        private CodeType regionalSpeciesCodeField;

        private CodeType fishingCategoryCodeField;

        private CodeType fAOFishingGearCodeField;

        private CodeType typeCodeField;

        private VesselTransportMeansType specifiedVesselTransportMeansField;

        private ACDRReportedAreaType[] specifiedACDRReportedAreaField;

        public CodeType RegionalAreaCode
        {
            get => this.regionalAreaCodeField;
            set => this.regionalAreaCodeField = value;
        }

        public CodeType RegionalSpeciesCode
        {
            get => this.regionalSpeciesCodeField;
            set => this.regionalSpeciesCodeField = value;
        }

        public CodeType FishingCategoryCode
        {
            get => this.fishingCategoryCodeField;
            set => this.fishingCategoryCodeField = value;
        }

        public CodeType FAOFishingGearCode
        {
            get => this.fAOFishingGearCodeField;
            set => this.fAOFishingGearCodeField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public VesselTransportMeansType SpecifiedVesselTransportMeans
        {
            get => this.specifiedVesselTransportMeansField;
            set => this.specifiedVesselTransportMeansField = value;
        }

        [XmlElement("SpecifiedACDRReportedArea")]
        public ACDRReportedAreaType[] SpecifiedACDRReportedArea
        {
            get => this.specifiedACDRReportedAreaField;
            set => this.specifiedACDRReportedAreaField = value;
        }
    }
}