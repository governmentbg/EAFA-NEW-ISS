namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIMaterialGoodsCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIMaterialGoodsCharacteristicType
    {

        private TextType[] descriptionField;

        private CodeType typeCodeField;

        private PercentType proportionalConstituentPercentField;

        private MeasureType absolutePresenceWeightMeasureField;

        private MeasureType absolutePresenceVolumeMeasureField;

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

        public PercentType ProportionalConstituentPercent
        {
            get
            {
                return this.proportionalConstituentPercentField;
            }
            set
            {
                this.proportionalConstituentPercentField = value;
            }
        }

        public MeasureType AbsolutePresenceWeightMeasure
        {
            get
            {
                return this.absolutePresenceWeightMeasureField;
            }
            set
            {
                this.absolutePresenceWeightMeasureField = value;
            }
        }

        public MeasureType AbsolutePresenceVolumeMeasure
        {
            get
            {
                return this.absolutePresenceVolumeMeasureField;
            }
            set
            {
                this.absolutePresenceVolumeMeasureField = value;
            }
        }
    }
}