namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MilkTank", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MilkTankType
    {

        private IDType idField;

        private IDType serialIDField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public IDType SerialID
        {
            get
            {
                return this.serialIDField;
            }
            set
            {
                this.serialIDField = value;
            }
        }
    }
}