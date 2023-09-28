namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectActualizationLocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectActualizationLocationType
    {

        private IDType idField;

        private TextType nameField;

        private IDType districtIDField;

        private TenderingAddressType postalTenderingAddressField;

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

        public TextType Name
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

        public IDType DistrictID
        {
            get
            {
                return this.districtIDField;
            }
            set
            {
                this.districtIDField = value;
            }
        }

        public TenderingAddressType PostalTenderingAddress
        {
            get
            {
                return this.postalTenderingAddressField;
            }
            set
            {
                this.postalTenderingAddressField = value;
            }
        }
    }
}