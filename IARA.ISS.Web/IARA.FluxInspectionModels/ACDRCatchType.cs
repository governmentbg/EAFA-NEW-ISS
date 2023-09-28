namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ACDRCatch", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ACDRCatchType
    {

        private CodeType fAOSpeciesCodeField;

        private QuantityType unitQuantityField;

        private MeasureType weightMeasureField;

        private CodeType usageCodeField;

        public CodeType FAOSpeciesCode
        {
            get
            {
                return this.fAOSpeciesCodeField;
            }
            set
            {
                this.fAOSpeciesCodeField = value;
            }
        }

        public QuantityType UnitQuantity
        {
            get
            {
                return this.unitQuantityField;
            }
            set
            {
                this.unitQuantityField = value;
            }
        }

        public MeasureType WeightMeasure
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

        public CodeType UsageCode
        {
            get
            {
                return this.usageCodeField;
            }
            set
            {
                this.usageCodeField = value;
            }
        }
    }
}