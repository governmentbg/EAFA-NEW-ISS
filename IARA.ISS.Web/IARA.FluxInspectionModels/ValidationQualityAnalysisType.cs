namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ValidationQualityAnalysis", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ValidationQualityAnalysisType
    {

        private CodeType levelCodeField;

        private CodeType typeCodeField;

        private TextType[] resultField;

        private IDType idField;

        private TextType descriptionField;

        private TextType[] referencedItemField;

        public CodeType LevelCode
        {
            get
            {
                return this.levelCodeField;
            }
            set
            {
                this.levelCodeField = value;
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

        [XmlElement("Result")]
        public TextType[] Result
        {
            get
            {
                return this.resultField;
            }
            set
            {
                this.resultField = value;
            }
        }

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

        [XmlElement("ReferencedItem")]
        public TextType[] ReferencedItem
        {
            get
            {
                return this.referencedItemField;
            }
            set
            {
                this.referencedItemField = value;
            }
        }
    }
}