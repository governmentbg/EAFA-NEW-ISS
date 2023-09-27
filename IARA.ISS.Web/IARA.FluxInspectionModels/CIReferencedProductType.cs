namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIReferencedProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIReferencedProductType
    {

        private IDType[] idField;

        private IDType[] globalIDField;

        private IDType sellerAssignedIDField;

        private IDType buyerAssignedIDField;

        private IDType manufacturerAssignedIDField;

        private IDType industryAssignedIDField;

        private TextType[] descriptionField;

        private CodeType[] relationshipTypeCodeField;

        private QuantityType[] unitQuantityField;

        [XmlElement("ID")]
        public IDType[] ID
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

        [XmlElement("GlobalID")]
        public IDType[] GlobalID
        {
            get
            {
                return this.globalIDField;
            }
            set
            {
                this.globalIDField = value;
            }
        }

        public IDType SellerAssignedID
        {
            get
            {
                return this.sellerAssignedIDField;
            }
            set
            {
                this.sellerAssignedIDField = value;
            }
        }

        public IDType BuyerAssignedID
        {
            get
            {
                return this.buyerAssignedIDField;
            }
            set
            {
                this.buyerAssignedIDField = value;
            }
        }

        public IDType ManufacturerAssignedID
        {
            get
            {
                return this.manufacturerAssignedIDField;
            }
            set
            {
                this.manufacturerAssignedIDField = value;
            }
        }

        public IDType IndustryAssignedID
        {
            get
            {
                return this.industryAssignedIDField;
            }
            set
            {
                this.industryAssignedIDField = value;
            }
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        [XmlElement("RelationshipTypeCode")]
        public CodeType[] RelationshipTypeCode
        {
            get
            {
                return this.relationshipTypeCodeField;
            }
            set
            {
                this.relationshipTypeCodeField = value;
            }
        }

        [XmlElement("UnitQuantity")]
        public QuantityType[] UnitQuantity
        {
            get
            {
                return this.unitQuantityField;
            }
            set
            {
                this.unitQuantityField = value;
            }
        }
    }
}