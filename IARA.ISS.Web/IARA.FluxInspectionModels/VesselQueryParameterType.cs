namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselQueryParameter", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselQueryParameterType
    {

        private CodeType searchTypeCodeField;

        public CodeType SearchTypeCode
        {
            get => this.searchTypeCodeField;
            set => this.searchTypeCodeField = value;
        }
    }
}