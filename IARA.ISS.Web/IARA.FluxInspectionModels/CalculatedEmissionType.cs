namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CalculatedEmission", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CalculatedEmissionType
    {

        private CodeType typeCodeField;

        private LinearUnitMeasureType affectedDistanceMeasureField;

        private WeightUnitMeasureType[] weightMeasureField;

        private MeasureType[] pollutionMeasureField;

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

        public LinearUnitMeasureType AffectedDistanceMeasure
        {
            get
            {
                return this.affectedDistanceMeasureField;
            }
            set
            {
                this.affectedDistanceMeasureField = value;
            }
        }

        [XmlElement("WeightMeasure")]
        public WeightUnitMeasureType[] WeightMeasure
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

        [XmlElement("PollutionMeasure")]
        public MeasureType[] PollutionMeasure
        {
            get
            {
                return this.pollutionMeasureField;
            }
            set
            {
                this.pollutionMeasureField = value;
            }
        }
    }
}