namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("WorkItemComplexDescription", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class WorkItemComplexDescriptionType
    {

        private TextType[] abstractField;

        private TextType[] contentField;

        private CodeType contractualLanguageCodeField;

        private SpecificationQueryType[] requestingSpecificationQueryField;

        private SpecificationResponseType[] respondingSpecificationResponseField;

        private WorkItemComplexDescriptionType subsetWorkItemComplexDescriptionField;

        [XmlElement("Abstract")]
        public TextType[] Abstract
        {
            get
            {
                return this.abstractField;
            }
            set
            {
                this.abstractField = value;
            }
        }

        [XmlElement("Content")]
        public TextType[] Content
        {
            get
            {
                return this.contentField;
            }
            set
            {
                this.contentField = value;
            }
        }

        public CodeType ContractualLanguageCode
        {
            get
            {
                return this.contractualLanguageCodeField;
            }
            set
            {
                this.contractualLanguageCodeField = value;
            }
        }

        [XmlElement("RequestingSpecificationQuery")]
        public SpecificationQueryType[] RequestingSpecificationQuery
        {
            get
            {
                return this.requestingSpecificationQueryField;
            }
            set
            {
                this.requestingSpecificationQueryField = value;
            }
        }

        [XmlElement("RespondingSpecificationResponse")]
        public SpecificationResponseType[] RespondingSpecificationResponse
        {
            get
            {
                return this.respondingSpecificationResponseField;
            }
            set
            {
                this.respondingSpecificationResponseField = value;
            }
        }

        public WorkItemComplexDescriptionType SubsetWorkItemComplexDescription
        {
            get
            {
                return this.subsetWorkItemComplexDescriptionField;
            }
            set
            {
                this.subsetWorkItemComplexDescriptionField = value;
            }
        }
    }
}