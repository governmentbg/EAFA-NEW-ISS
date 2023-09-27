namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICreditorFinancialAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICreditorFinancialAccountType
    {

        private IDType iBANIDField;

        private TextType accountNameField;

        private IDType proprietaryIDField;

        private CodeType typeCodeField;

        public IDType IBANID
        {
            get
            {
                return this.iBANIDField;
            }
            set
            {
                this.iBANIDField = value;
            }
        }

        public TextType AccountName
        {
            get
            {
                return this.accountNameField;
            }
            set
            {
                this.accountNameField = value;
            }
        }

        public IDType ProprietaryID
        {
            get
            {
                return this.proprietaryIDField;
            }
            set
            {
                this.proprietaryIDField = value;
            }
        }

        public CodeType TypeCode
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
    }
}