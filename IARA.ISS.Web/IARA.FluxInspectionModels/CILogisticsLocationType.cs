namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CILogisticsLocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CILogisticsLocationType
    {

        private IDType idField;

        private TextType[] nameField;

        private LocationFunctionCodeType typeCodeField;

        private TextType[] descriptionField;

        private IDType countrySubDivisionIDField;

        private CITradeAddressType postalCITradeAddressField;

        private CIGeographicalCoordinateType physicalCIGeographicalCoordinateField;

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

        [XmlElement("Name")]
        public TextType[] Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public LocationFunctionCodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
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

        public IDType CountrySubDivisionID
        {
            get
            {
                return this.countrySubDivisionIDField;
            }
            set
            {
                this.countrySubDivisionIDField = value;
            }
        }

        public CITradeAddressType PostalCITradeAddress
        {
            get
            {
                return this.postalCITradeAddressField;
            }
            set
            {
                this.postalCITradeAddressField = value;
            }
        }

        public CIGeographicalCoordinateType PhysicalCIGeographicalCoordinate
        {
            get
            {
                return this.physicalCIGeographicalCoordinateField;
            }
            set
            {
                this.physicalCIGeographicalCoordinateField = value;
            }
        }
    }
}