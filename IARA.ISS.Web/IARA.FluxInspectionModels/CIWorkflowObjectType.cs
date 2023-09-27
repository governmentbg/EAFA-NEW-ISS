namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIWorkflowObject", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIWorkflowObjectType
    {

        private IDType idField;

        private WorkflowStatusCodeType statusCodeField;

        private WorkflowStatusCodeType previousStatusCodeField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public WorkflowStatusCodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public WorkflowStatusCodeType PreviousStatusCode
        {
            get => this.previousStatusCodeField;
            set => this.previousStatusCodeField = value;
        }
    }
}