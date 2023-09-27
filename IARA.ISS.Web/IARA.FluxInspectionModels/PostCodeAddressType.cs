namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("PostCodeAddress", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PostCodeAddressType
    {

        private CodeType postcodeCodeField;

        private IDType countryIDField;

        public CodeType PostcodeCode
        {
            get => this.postcodeCodeField;
            set => this.postcodeCodeField = value;
        }

        public IDType CountryID
        {
            get => this.countryIDField;
            set => this.countryIDField = value;
        }
    }
}