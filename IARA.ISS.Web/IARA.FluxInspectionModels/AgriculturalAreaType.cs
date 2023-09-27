namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalArea", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalAreaType
    {

        private CodeType typeCodeField;

        private MeasureType actualMeasureField;

        private SpecifiedGeographicalCoordinateType[] boundarySpecifiedGeographicalCoordinateField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

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

        public MeasureType ActualMeasure
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

        [XmlElement("BoundarySpecifiedGeographicalCoordinate")]
        public SpecifiedGeographicalCoordinateType[] BoundarySpecifiedGeographicalCoordinate
        {
            get
            {
                return this.boundarySpecifiedGeographicalCoordinateField;
            }
            set
            {
                this.boundarySpecifiedGeographicalCoordinateField = value;
            }
        }

        [XmlElement("ApplicableTechnicalCharacteristic")]
        public TechnicalCharacteristicType[] ApplicableTechnicalCharacteristic
        {
            get
            {
                return this.applicableTechnicalCharacteristicField;
            }
            set
            {
                this.applicableTechnicalCharacteristicField = value;
            }
        }
    }
}