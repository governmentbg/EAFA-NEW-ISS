namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAATrialBalanceAccountingCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAATrialBalanceAccountingCharacteristicType
    {

        private CodeType descriptionCodeField;

        private CodeType descriptionValueCodeField;

        public CodeType DescriptionCode
        {
            get
            {
                return this.descriptionCodeField;
            }
            set
            {
                this.descriptionCodeField = value;
            }
        }

        public CodeType DescriptionValueCode
        {
            get
            {
                return this.descriptionValueCodeField;
            }
            set
            {
                this.descriptionValueCodeField = value;
            }
        }
    }
}