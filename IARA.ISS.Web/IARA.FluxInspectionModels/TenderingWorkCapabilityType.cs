namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingWorkCapability", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingWorkCapabilityType
    {

        private IDType licenceIDField;

        private CodeType typeCodeField;

        private TenderingCompletedWorkType[] experienceTenderingCompletedWorkField;

        private TenderingTechnicalCapabilityType supportedTenderingTechnicalCapabilityField;

        private TenderingDeliverableType[] supportedTenderingDeliverableField;

        public IDType LicenceID
        {
            get => this.licenceIDField;
            set => this.licenceIDField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("ExperienceTenderingCompletedWork")]
        public TenderingCompletedWorkType[] ExperienceTenderingCompletedWork
        {
            get => this.experienceTenderingCompletedWorkField;
            set => this.experienceTenderingCompletedWorkField = value;
        }

        public TenderingTechnicalCapabilityType SupportedTenderingTechnicalCapability
        {
            get => this.supportedTenderingTechnicalCapabilityField;
            set => this.supportedTenderingTechnicalCapabilityField = value;
        }

        [XmlElement("SupportedTenderingDeliverable")]
        public TenderingDeliverableType[] SupportedTenderingDeliverable
        {
            get => this.supportedTenderingDeliverableField;
            set => this.supportedTenderingDeliverableField = value;
        }
    }
}