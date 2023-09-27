namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSPackage", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSPackageType
    {

        private CodeType levelCodeField;

        private PackageTypeCodeType typeCodeField;

        private QuantityType itemQuantityField;

        private MeasureType nominalGrossWeightMeasureField;

        private MeasureType nominalGrossVolumeMeasureField;

        private SPSShippingMarksType[] physicalSPSShippingMarksField;

        public CodeType LevelCode
        {
            get => this.levelCodeField;
            set => this.levelCodeField = value;
        }

        public PackageTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public QuantityType ItemQuantity
        {
            get => this.itemQuantityField;
            set => this.itemQuantityField = value;
        }

        public MeasureType NominalGrossWeightMeasure
        {
            get => this.nominalGrossWeightMeasureField;
            set => this.nominalGrossWeightMeasureField = value;
        }

        public MeasureType NominalGrossVolumeMeasure
        {
            get => this.nominalGrossVolumeMeasureField;
            set => this.nominalGrossVolumeMeasureField = value;
        }

        [XmlElement("PhysicalSPSShippingMarks")]
        public SPSShippingMarksType[] PhysicalSPSShippingMarks
        {
            get => this.physicalSPSShippingMarksField;
            set => this.physicalSPSShippingMarksField = value;
        }
    }
}