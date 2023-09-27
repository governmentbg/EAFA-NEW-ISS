namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CompletedWasteRecoveryDisposalProcess", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CompletedWasteRecoveryDisposalProcessType
    {

        private CodeType[] typeCodeField;

        private TextType descriptionField;

        private DateTimeType endDateTimeField;

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public DateTimeType EndDateTime
        {
            get => this.endDateTimeField;
            set => this.endDateTimeField = value;
        }
    }
}