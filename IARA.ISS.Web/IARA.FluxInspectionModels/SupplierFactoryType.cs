namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SupplierFactory", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SupplierFactoryType
    {

        private TextType nameField;

        private TenderingAddressType postalTenderingAddressField;

        private TenderingContactType[] designatedTenderingContactField;

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

        [XmlElement("DesignatedTenderingContact")]
        public TenderingContactType[] DesignatedTenderingContact
        {
            get => this.designatedTenderingContactField;
            set => this.designatedTenderingContactField = value;
        }
    }
}