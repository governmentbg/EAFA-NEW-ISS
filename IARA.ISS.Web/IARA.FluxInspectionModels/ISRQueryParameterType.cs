namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ISRQueryParameter", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ISRQueryParameterType
    {

        private CodeType typeCodeField;

        private CodeType valueCodeField;

        private DateTimeType valueDateTimeField;

        private IDType valueIDField;

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

        public CodeType ValueCode
        {
            get
            {
                return this.valueCodeField;
            }
            set
            {
                this.valueCodeField = value;
            }
        }

        public DateTimeType ValueDateTime
        {
            get
            {
                return this.valueDateTimeField;
            }
            set
            {
                this.valueDateTimeField = value;
            }
        }

        public IDType ValueID
        {
            get
            {
                return this.valueIDField;
            }
            set
            {
                this.valueIDField = value;
            }
        }
    }
}