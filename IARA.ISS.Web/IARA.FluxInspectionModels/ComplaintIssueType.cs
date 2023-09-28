namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ComplaintIssue", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ComplaintIssueType
    {

        private IDType idField;

        private CodeType[] typeCodeField;

        private CodeType[] severityCodeField;

        private TextType descriptionField;

        private IndicatorType closedIndicatorField;

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

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        [XmlElement("SeverityCode")]
        public CodeType[] SeverityCode
        {
            get
            {
                return this.severityCodeField;
            }
            set
            {
                this.severityCodeField = value;
            }
        }

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public IndicatorType ClosedIndicator
        {
            get
            {
                return this.closedIndicatorField;
            }
            set
            {
                this.closedIndicatorField = value;
            }
        }
    }
}