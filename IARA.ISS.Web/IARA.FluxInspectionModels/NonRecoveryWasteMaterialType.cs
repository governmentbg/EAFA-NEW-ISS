namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("NonRecoveryWasteMaterial", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class NonRecoveryWasteMaterialType
    {

        private AmountType treatmentCostAmountField;

        private WasteRecoveryDisposalProcessType intendedDisposalTreatmentWasteRecoveryDisposalProcessField;

        public AmountType TreatmentCostAmount
        {
            get => this.treatmentCostAmountField;
            set => this.treatmentCostAmountField = value;
        }

        public WasteRecoveryDisposalProcessType IntendedDisposalTreatmentWasteRecoveryDisposalProcess
        {
            get => this.intendedDisposalTreatmentWasteRecoveryDisposalProcessField;
            set => this.intendedDisposalTreatmentWasteRecoveryDisposalProcessField = value;
        }
    }
}