namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("DocumentPreparerParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class DocumentPreparerPartyType
    {

        private TextType nameField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }
    }
}