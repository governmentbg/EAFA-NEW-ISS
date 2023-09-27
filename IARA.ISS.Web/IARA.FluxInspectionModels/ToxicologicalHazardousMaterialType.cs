namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ToxicologicalHazardousMaterial", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ToxicologicalHazardousMaterialType
    {

        private TextType reproductiveToxinNameField;

        private TextType descriptionField;

        private TextType[] entryRouteDescriptionField;

        private TextType biologicalSeverityDescriptionField;

        public TextType ReproductiveToxinName
        {
            get => this.reproductiveToxinNameField;
            set => this.reproductiveToxinNameField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("EntryRouteDescription")]
        public TextType[] EntryRouteDescription
        {
            get => this.entryRouteDescriptionField;
            set => this.entryRouteDescriptionField = value;
        }

        public TextType BiologicalSeverityDescription
        {
            get => this.biologicalSeverityDescriptionField;
            set => this.biologicalSeverityDescriptionField = value;
        }
    }
}