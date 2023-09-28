namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAArchiveBundleCollection", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAArchiveBundleCollectionType
    {

        private IDType idField;

        private TextType commentField;

        private IDType agentAssignedArchiveIDField;

        private DateTimeType agentArchiveDeliveryDateTimeField;

        private IDType ownerAssignedArchiveIDField;

        private DateTimeType ownerArchiveDeliveryDateTimeField;

        private AAAArchiveArchiveParameterType lifeCycleAAAArchiveArchiveParameterField;

        private AAAArchiveDocumentType[] includedAAAArchiveDocumentField;

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

        public TextType Comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        public IDType AgentAssignedArchiveID
        {
            get
            {
                return this.agentAssignedArchiveIDField;
            }
            set
            {
                this.agentAssignedArchiveIDField = value;
            }
        }

        public DateTimeType AgentArchiveDeliveryDateTime
        {
            get
            {
                return this.agentArchiveDeliveryDateTimeField;
            }
            set
            {
                this.agentArchiveDeliveryDateTimeField = value;
            }
        }

        public IDType OwnerAssignedArchiveID
        {
            get
            {
                return this.ownerAssignedArchiveIDField;
            }
            set
            {
                this.ownerAssignedArchiveIDField = value;
            }
        }

        public DateTimeType OwnerArchiveDeliveryDateTime
        {
            get
            {
                return this.ownerArchiveDeliveryDateTimeField;
            }
            set
            {
                this.ownerArchiveDeliveryDateTimeField = value;
            }
        }

        public AAAArchiveArchiveParameterType LifeCycleAAAArchiveArchiveParameter
        {
            get
            {
                return this.lifeCycleAAAArchiveArchiveParameterField;
            }
            set
            {
                this.lifeCycleAAAArchiveArchiveParameterField = value;
            }
        }

        [XmlElement("IncludedAAAArchiveDocument")]
        public AAAArchiveDocumentType[] IncludedAAAArchiveDocument
        {
            get
            {
                return this.includedAAAArchiveDocumentField;
            }
            set
            {
                this.includedAAAArchiveDocumentField = value;
            }
        }
    }
}