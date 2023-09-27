namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ResourceDistributionProfile", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ResourceDistributionProfileType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private ResourceDistributionProfilePointType[] specifiedResourceDistributionProfilePointField;

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

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("SpecifiedResourceDistributionProfilePoint")]
        public ResourceDistributionProfilePointType[] SpecifiedResourceDistributionProfilePoint
        {
            get => this.specifiedResourceDistributionProfilePointField;
            set => this.specifiedResourceDistributionProfilePointField = value;
        }
    }
}