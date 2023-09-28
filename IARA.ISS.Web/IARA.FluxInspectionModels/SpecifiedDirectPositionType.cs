namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedDirectPosition", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedDirectPositionType
    {

        private TextType nameField;

        private TextType[] coordinateReferenceDimensionField;

        private TextType[] axisLabelListField;

        private TextType[] uOMLabelListField;

        private NumericType[] countNumericField;

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        [XmlElement("CoordinateReferenceDimension")]
        public TextType[] CoordinateReferenceDimension
        {
            get
            {
                return this.coordinateReferenceDimensionField;
            }
            set
            {
                this.coordinateReferenceDimensionField = value;
            }
        }

        [XmlElement("AxisLabelList")]
        public TextType[] AxisLabelList
        {
            get
            {
                return this.axisLabelListField;
            }
            set
            {
                this.axisLabelListField = value;
            }
        }

        [XmlElement("UOMLabelList")]
        public TextType[] UOMLabelList
        {
            get
            {
                return this.uOMLabelListField;
            }
            set
            {
                this.uOMLabelListField = value;
            }
        }

        [XmlElement("CountNumeric")]
        public NumericType[] CountNumeric
        {
            get
            {
                return this.countNumericField;
            }
            set
            {
                this.countNumericField = value;
            }
        }
    }
}