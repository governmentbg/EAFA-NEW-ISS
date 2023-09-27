namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICrossBorderRegulatoryProcedure", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICrossBorderRegulatoryProcedureType
    {

        private CodeType[] typeCodeField;

        private CodeType[] transactionNatureCodeField;

        private AmountType[] tariffAmountField;

        private AmountType[] nonTariffChargeAmountField;

        private AmountType[] totalChargeAmountField;

        private TextType[] remarkField;

        private CITradeTaxType[] applicableCITradeTaxField;

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
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

        [XmlElement("TransactionNatureCode")]
        public CodeType[] TransactionNatureCode
        {
            get
            {
                return this.transactionNatureCodeField;
            }
            set
            {
                this.transactionNatureCodeField = value;
            }
        }

        [XmlElement("TariffAmount")]
        public AmountType[] TariffAmount
        {
            get
            {
                return this.tariffAmountField;
            }
            set
            {
                this.tariffAmountField = value;
            }
        }

        [XmlElement("NonTariffChargeAmount")]
        public AmountType[] NonTariffChargeAmount
        {
            get
            {
                return this.nonTariffChargeAmountField;
            }
            set
            {
                this.nonTariffChargeAmountField = value;
            }
        }

        [XmlElement("TotalChargeAmount")]
        public AmountType[] TotalChargeAmount
        {
            get
            {
                return this.totalChargeAmountField;
            }
            set
            {
                this.totalChargeAmountField = value;
            }
        }

        [XmlElement("Remark")]
        public TextType[] Remark
        {
            get
            {
                return this.remarkField;
            }
            set
            {
                this.remarkField = value;
            }
        }

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