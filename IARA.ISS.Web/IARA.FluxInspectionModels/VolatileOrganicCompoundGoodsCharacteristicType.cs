namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VolatileOrganicCompoundGoodsCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VolatileOrganicCompoundGoodsCharacteristicType
    {

        private PercentType constituentPercentField;

        private MeasureType molecularWeightMeasureField;

        private MeasureType volumeMeasureField;

        public PercentType ConstituentPercent
        {
            get => this.constituentPercentField;
            set => this.constituentPercentField = value;
        }

        public MeasureType MolecularWeightMeasure
        {
            get => this.molecularWeightMeasureField;
            set => this.molecularWeightMeasureField = value;
        }

        public MeasureType VolumeMeasure
        {
            get => this.volumeMeasureField;
            set => this.volumeMeasureField = value;
        }
    }
}