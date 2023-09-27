namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingLocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingLocationType
    {

        private IDType idField;

        private TextType nameField;

        private TenderingAddressType postalTenderingAddressField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TenderingAddressType PostalTenderingAddress
        {
            get => this.postalTenderingAddressField;
            set => this.postalTenderingAddressField = value;
        }
    }
}