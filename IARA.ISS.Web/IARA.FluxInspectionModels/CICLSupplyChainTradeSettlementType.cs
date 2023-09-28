namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICLSupplyChainTradeSettlement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICLSupplyChainTradeSettlementType
    {

        private CITradeTaxType[] applicableCITradeTaxField;

        [XmlElement("ApplicableCITradeTax")]
        public CITradeTaxType[] ApplicableCITradeTax
        {
            get
            {
                return this.applicableCITradeTaxField;
            }
            set
            {
                this.applicableCITradeTaxField = value;
            }
        }
    }
}