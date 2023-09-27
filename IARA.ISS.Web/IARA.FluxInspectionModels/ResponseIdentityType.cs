namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ResponseIdentity", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ResponseIdentityType
    {

        private CodeType contentTypeCodeField;

        private IDType supplementaryIDField;

        public CodeType ContentTypeCode
        {
            get => this.contentTypeCodeField;
            set => this.contentTypeCodeField = value;
        }

        public IDType SupplementaryID
        {
            get => this.supplementaryIDField;
            set => this.supplementaryIDField = value;
        }
    }
}