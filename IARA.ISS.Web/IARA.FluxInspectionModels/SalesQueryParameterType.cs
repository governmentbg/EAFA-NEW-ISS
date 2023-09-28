namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SalesQueryParameter", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SalesQueryParameterType
    {

        private CodeType typeCodeField;

        private CodeType valueCodeField;

        private DateTimeType valueDateTimeField;

        private IDType valueIDField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public CodeType ValueCode
        {
            get => this.valueCodeField;
            set => this.valueCodeField = value;
        }

        public DateTimeType ValueDateTime
        {
            get => this.valueDateTimeField;
            set => this.valueDateTimeField = value;
        }

        public IDType ValueID
        {
            get => this.valueIDField;
            set => this.valueIDField = value;
        }
    }
}