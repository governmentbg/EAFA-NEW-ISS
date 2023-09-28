namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ClientBusinessAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ClientBusinessAccountType
    {

        private IDType idField;

        private IDType numberIDField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType NumberID
        {
            get => this.numberIDField;
            set => this.numberIDField = value;
        }
    }
}