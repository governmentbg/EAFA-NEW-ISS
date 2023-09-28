namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReservationRequirement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReservationRequirementType
    {

        private TextType[] acceptableCreditCardNamePaymentInformationField;

        private TextType[] lodgingHouseCovenantRuleField;

        private TextType descriptionField;

        private LodgingHouseContractType actualLodgingHouseContractField;

        [XmlElement("AcceptableCreditCardNamePaymentInformation")]
        public TextType[] AcceptableCreditCardNamePaymentInformation
        {
            get => this.acceptableCreditCardNamePaymentInformationField;
            set => this.acceptableCreditCardNamePaymentInformationField = value;
        }

        [XmlElement("LodgingHouseCovenantRule")]
        public TextType[] LodgingHouseCovenantRule
        {
            get => this.lodgingHouseCovenantRuleField;
            set => this.lodgingHouseCovenantRuleField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public LodgingHouseContractType ActualLodgingHouseContract
        {
            get => this.actualLodgingHouseContractField;
            set => this.actualLodgingHouseContractField = value;
        }
    }
}