namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LivestockAnimalProductInstance", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LivestockAnimalProductInstanceType
    {

        private IDType idField;

        private TextType intendedUseField;

        private TextType[] additionalInformationBasicNoteField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType IntendedUse
        {
            get => this.intendedUseField;
            set => this.intendedUseField = value;
        }

        [XmlArrayItem("Content", IsNullable = false)]
        public TextType[] AdditionalInformationBasicNote
        {
            get => this.additionalInformationBasicNoteField;
            set => this.additionalInformationBasicNoteField = value;
        }
    }
}