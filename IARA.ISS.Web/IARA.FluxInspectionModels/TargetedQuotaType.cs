namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TargetedQuota", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TargetedQuotaType
    {

        private CodeType typeCodeField;

        private CodeType objectCodeField;

        private MeasureType[] weightMeasureField;

        private QuantityType[] specifiedQuantityField;

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

        public CodeType ObjectCode
        {
            get
            {
                return this.objectCodeField;
            }
            set
            {
                this.objectCodeField = value;
            }
        }

        [XmlElement("WeightMeasure")]
        public MeasureType[] WeightMeasure
        {
            get
            {
                return this.weightMeasureField;
            }
            set
            {
                this.weightMeasureField = value;
            }
        }

        [XmlElement("SpecifiedQuantity")]
        public QuantityType[] SpecifiedQuantity
        {
            get
            {
                return this.specifiedQuantityField;
            }
            set
            {
                this.specifiedQuantityField = value;
            }
        }
    }
}