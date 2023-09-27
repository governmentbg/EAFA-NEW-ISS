namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReferencedProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReferencedProductType
    {

        private IDType[] idField;

        private IDType[] globalIDField;

        private IDType sellerAssignedIDField;

        private IDType buyerAssignedIDField;

        private IDType[] manufacturerAssignedIDField;

        private IDType[] industryAssignedIDField;

        private TextType[] nameField;

        private TextType[] descriptionField;

        private CodeType[] relationshipTypeCodeField;

        private QuantityType[] unitQuantityField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("GlobalID")]
        public IDType[] GlobalID
        {
            get => this.globalIDField;
            set => this.globalIDField = value;
        }

        public IDType SellerAssignedID
        {
            get => this.sellerAssignedIDField;
            set => this.sellerAssignedIDField = value;
        }

        public IDType BuyerAssignedID
        {
            get => this.buyerAssignedIDField;
            set => this.buyerAssignedIDField = value;
        }

        [XmlElement("ManufacturerAssignedID")]
        public IDType[] ManufacturerAssignedID
        {
            get => this.manufacturerAssignedIDField;
            set => this.manufacturerAssignedIDField = value;
        }

        [XmlElement("IndustryAssignedID")]
        public IDType[] IndustryAssignedID
        {
            get => this.industryAssignedIDField;
            set => this.industryAssignedIDField = value;
        }

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("RelationshipTypeCode")]
        public CodeType[] RelationshipTypeCode
        {
            get => this.relationshipTypeCodeField;
            set => this.relationshipTypeCodeField = value;
        }

        [XmlElement("UnitQuantity")]
        public QuantityType[] UnitQuantity
        {
            get => this.unitQuantityField;
            set => this.unitQuantityField = value;
        }
    }
}