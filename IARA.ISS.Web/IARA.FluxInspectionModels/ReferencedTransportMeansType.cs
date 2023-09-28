namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReferencedTransportMeans", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReferencedTransportMeansType
    {

        private TransportMeansTypeCodeType typeCodeField;

        private TextType typeField;

        private IDType[] idField;

        private TextType nameField;

        private IndicatorType driverAccompaniedIndicatorField;

        public TransportMeansTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType Type
        {
            get => this.typeField;
            set => this.typeField = value;
        }

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public IndicatorType DriverAccompaniedIndicator
        {
            get => this.driverAccompaniedIndicatorField;
            set => this.driverAccompaniedIndicatorField = value;
        }
    }
}