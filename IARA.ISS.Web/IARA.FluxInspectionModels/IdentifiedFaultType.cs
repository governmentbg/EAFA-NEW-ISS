namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("IdentifiedFault", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class IdentifiedFaultType
    {

        private TextType[] descriptionField;

        private CodeType[] identificationCodeField;

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("IdentificationCode")]
        public CodeType[] IdentificationCode
        {
            get => this.identificationCodeField;
            set => this.identificationCodeField = value;
        }
    }
}