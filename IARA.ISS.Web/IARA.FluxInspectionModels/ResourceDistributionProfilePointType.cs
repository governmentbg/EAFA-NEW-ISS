namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ResourceDistributionProfilePoint", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ResourceDistributionProfilePointType
    {

        private TextType nameField;

        private PercentType allocationPercentField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public PercentType AllocationPercent
        {
            get => this.allocationPercentField;
            set => this.allocationPercentField = value;
        }
    }
}