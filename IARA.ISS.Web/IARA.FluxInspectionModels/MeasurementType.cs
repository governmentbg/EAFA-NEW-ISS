namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("Measurement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MeasurementType
    {

        private CodeType[] comparisonOperatorCodeField;

        private MeasureType[] actualMeasureField;

        private TextType[] methodField;

        private MeasureType[] conditionMeasureField;

        private CodeType[] typeCodeField;

        private TextType[] descriptionField;

        [XmlElement("ComparisonOperatorCode")]
        public CodeType[] ComparisonOperatorCode
        {
            get
            {
                return this.comparisonOperatorCodeField;
            }
            set
            {
                this.comparisonOperatorCodeField = value;
            }
        }

        [XmlElement("ActualMeasure")]
        public MeasureType[] ActualMeasure
        {
            get
            {
                return this.actualMeasureField;
            }
            set
            {
                this.actualMeasureField = value;
            }
        }

        [XmlElement("Method")]
        public TextType[] Method
        {
            get
            {
                return this.methodField;
            }
            set
            {
                this.methodField = value;
            }
        }

        [XmlElement("ConditionMeasure")]
        public MeasureType[] ConditionMeasure
        {
            get
            {
                return this.conditionMeasureField;
            }
            set
            {
                this.conditionMeasureField = value;
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

        [XmlElement("Description")]
        public TextType[] Description
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
    }
}