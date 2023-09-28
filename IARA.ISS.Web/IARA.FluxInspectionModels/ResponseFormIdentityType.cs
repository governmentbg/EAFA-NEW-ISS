namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ResponseFormIdentity", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ResponseFormIdentityType
    {

        private IDType idField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }
    }
}