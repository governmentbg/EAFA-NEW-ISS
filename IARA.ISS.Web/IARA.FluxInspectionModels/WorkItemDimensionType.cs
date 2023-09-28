namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("WorkItemDimension", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class WorkItemDimensionType
    {

        private IDType idField;

        private MeasureType valueMeasureField;

        private TextType descriptionField;

        private CodeType typeCodeField;

        private CodeType contractualLanguageCodeField;

        private WorkItemDimensionType[] componentWorkItemDimensionField;

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

        public MeasureType ValueMeasure
        {
            get
            {
                return this.valueMeasureField;
            }
            set
            {
                this.valueMeasureField = value;
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

        public CodeType TypeCode
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

        [XmlElement("ComponentWorkItemDimension")]
        public WorkItemDimensionType[] ComponentWorkItemDimension
        {
            get
            {
                return this.componentWorkItemDimensionField;
            }
            set
            {
                this.componentWorkItemDimensionField = value;
            }
        }
    }
}