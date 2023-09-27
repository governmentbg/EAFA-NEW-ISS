namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTTradeTransaction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTTradeTransactionType
    {

        private IDType idField;

        private IDType typeIDField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType TypeID
        {
            get => this.typeIDField;
            set => this.typeIDField = value;
        }
    }
}