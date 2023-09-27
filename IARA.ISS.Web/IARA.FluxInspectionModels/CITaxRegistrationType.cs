namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITaxRegistration", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITaxRegistrationType
    {

        private IDType idField;

        private CIRegisteredTaxType associatedCIRegisteredTaxField;

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

        public CIRegisteredTaxType AssociatedCIRegisteredTax
        {
            get
            {
                return this.associatedCIRegisteredTaxField;
            }
            set
            {
                this.associatedCIRegisteredTaxField = value;
            }
        }
    }
}