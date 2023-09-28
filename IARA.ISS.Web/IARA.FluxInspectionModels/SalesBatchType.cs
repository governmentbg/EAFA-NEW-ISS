namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SalesBatch", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SalesBatchType
    {

        private IDType[] idField;

        private AAPProductType[] specifiedAAPProductField;

        private AmountType[] totalSalesPriceField;

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

        [XmlElement("SpecifiedAAPProduct")]
        public AAPProductType[] SpecifiedAAPProduct
        {
            get
            {
                return this.specifiedAAPProductField;
            }
            set
            {
                this.specifiedAAPProductField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("ChargeAmount", IsNullable = false)]
        public AmountType[] TotalSalesPrice
        {
            get
            {
                return this.totalSalesPriceField;
            }
            set
            {
                this.totalSalesPriceField = value;
            }
        }
    }
}