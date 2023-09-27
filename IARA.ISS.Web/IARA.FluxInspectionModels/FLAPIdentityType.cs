namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FLAPIdentity", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FLAPIdentityType
    {

        private IDType idField;

        private IDType requestIDField;

        private CodeType fADATypeCodeField;

        private CodeType fCTypeCodeField;

        private CodeType[] statusCodeField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType RequestID
        {
            get => this.requestIDField;
            set => this.requestIDField = value;
        }

        public CodeType FADATypeCode
        {
            get => this.fADATypeCodeField;
            set => this.fADATypeCodeField = value;
        }

        public CodeType FCTypeCode
        {
            get => this.fCTypeCodeField;
            set => this.fCTypeCodeField = value;
        }

        [XmlElement("StatusCode")]
        public CodeType[] StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }
    }
}