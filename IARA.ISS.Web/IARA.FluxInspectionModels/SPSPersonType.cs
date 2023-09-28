namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSPerson", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSPersonType
    {

        private TextType nameField;

        private SPSQualificationType[] attainedSPSQualificationField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlElement("AttainedSPSQualification")]
        public SPSQualificationType[] AttainedSPSQualification
        {
            get => this.attainedSPSQualificationField;
            set => this.attainedSPSQualificationField = value;
        }
    }
}