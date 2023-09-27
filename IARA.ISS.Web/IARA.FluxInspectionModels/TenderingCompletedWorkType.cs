namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingCompletedWork", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingCompletedWorkType
    {

        private CodeType subTypeCodeField;

        private AmountType annualAverageAmountField;

        private DateType capacityStandardYearDateField;

        private AmountType organizationCapacityAmountField;

        private TextType descriptionField;

        public CodeType SubTypeCode
        {
            get => this.subTypeCodeField;
            set => this.subTypeCodeField = value;
        }

        public AmountType AnnualAverageAmount
        {
            get => this.annualAverageAmountField;
            set => this.annualAverageAmountField = value;
        }

        public DateType CapacityStandardYearDate
        {
            get => this.capacityStandardYearDateField;
            set => this.capacityStandardYearDateField = value;
        }

        public AmountType OrganizationCapacityAmount
        {
            get => this.organizationCapacityAmountField;
            set => this.organizationCapacityAmountField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}