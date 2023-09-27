namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselDimension", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselDimensionType
    {

        private CodeType typeCodeField;

        private MeasureType valueMeasureField;

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
    }
}