namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("InspectionPerson", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class InspectionPersonType
    {

        private TextType nameField;

        private SpecifiedQualificationType attainedSpecifiedQualificationField;

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public SpecifiedQualificationType AttainedSpecifiedQualification
        {
            get
            {
                return this.attainedSpecifiedQualificationField;
            }
            set
            {
                this.attainedSpecifiedQualificationField = value;
            }
        }
    }
}