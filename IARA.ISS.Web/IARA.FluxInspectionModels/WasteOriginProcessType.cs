namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("WasteOriginProcess", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class WasteOriginProcessType
    {

        private TextType descriptionField;

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}