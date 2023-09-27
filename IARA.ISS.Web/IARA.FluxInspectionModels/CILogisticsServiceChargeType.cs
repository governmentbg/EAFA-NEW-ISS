namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CILogisticsServiceCharge", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CILogisticsServiceChargeType
    {

        private FreightChargeTypeIDType idField;

        private TextType[] descriptionField;

        private TransportServicePaymentArrangementCodeType paymentArrangementCodeField;

        private FreightChargeTariffClassCodeType tariffClassCodeField;

        private CodeType chargeCategoryCodeField;

        private CodeType serviceCategoryCodeField;

        private AmountType[] disbursementAmountField;

        private AmountType[] appliedAmountField;

        private TextType allowanceChargeField;

        private ChargePayingPartyRoleCodeType payingPartyRoleCodeField;

        private LogisticsChargeCalculationBasisCodeType calculationBasisCodeField;

        private TextType calculationBasisField;

        private CodeType transportPaymentMethodCodeField;

        private CILogisticsLocationType paymentPlaceCILogisticsLocationField;

        private CILogisticsLocationType appliedFromCILogisticsLocationField;

        private CILogisticsLocationType appliedToCILogisticsLocationField;

        private CITradeTaxType[] appliedCITradeTaxField;

        public FreightChargeTypeIDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public TransportServicePaymentArrangementCodeType PaymentArrangementCode
        {
            get => this.paymentArrangementCodeField;
            set => this.paymentArrangementCodeField = value;
        }

        public FreightChargeTariffClassCodeType TariffClassCode
        {
            get => this.tariffClassCodeField;
            set => this.tariffClassCodeField = value;
        }

        public CodeType ChargeCategoryCode
        {
            get => this.chargeCategoryCodeField;
            set => this.chargeCategoryCodeField = value;
        }

        public CodeType ServiceCategoryCode
        {
            get => this.serviceCategoryCodeField;
            set => this.serviceCategoryCodeField = value;
        }

        [XmlElement("DisbursementAmount")]
        public AmountType[] DisbursementAmount
        {
            get => this.disbursementAmountField;
            set => this.disbursementAmountField = value;
        }

        [XmlElement("AppliedAmount")]
        public AmountType[] AppliedAmount
        {
            get => this.appliedAmountField;
            set => this.appliedAmountField = value;
        }

        public TextType AllowanceCharge
        {
            get => this.allowanceChargeField;
            set => this.allowanceChargeField = value;
        }

        public ChargePayingPartyRoleCodeType PayingPartyRoleCode
        {
            get => this.payingPartyRoleCodeField;
            set => this.payingPartyRoleCodeField = value;
        }

        public LogisticsChargeCalculationBasisCodeType CalculationBasisCode
        {
            get => this.calculationBasisCodeField;
            set => this.calculationBasisCodeField = value;
        }

        public TextType CalculationBasis
        {
            get => this.calculationBasisField;
            set => this.calculationBasisField = value;
        }

        public CodeType TransportPaymentMethodCode
        {
            get => this.transportPaymentMethodCodeField;
            set => this.transportPaymentMethodCodeField = value;
        }

        public CILogisticsLocationType PaymentPlaceCILogisticsLocation
        {
            get => this.paymentPlaceCILogisticsLocationField;
            set => this.paymentPlaceCILogisticsLocationField = value;
        }

        public CILogisticsLocationType AppliedFromCILogisticsLocation
        {
            get => this.appliedFromCILogisticsLocationField;
            set => this.appliedFromCILogisticsLocationField = value;
        }

        public CILogisticsLocationType AppliedToCILogisticsLocation
        {
            get => this.appliedToCILogisticsLocationField;
            set => this.appliedToCILogisticsLocationField = value;
        }

        [XmlElement("AppliedCITradeTax")]
        public CITradeTaxType[] AppliedCITradeTax
        {
            get => this.appliedCITradeTaxField;
            set => this.appliedCITradeTaxField = value;
        }
    }
}