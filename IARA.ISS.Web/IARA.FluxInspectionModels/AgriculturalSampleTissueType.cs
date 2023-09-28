namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalSampleTissue", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalSampleTissueType
    {

        private TextType anatomicalNameField;

        private TextType anatomicalLocationField;

        public TextType AnatomicalName
        {
            get
            {
                return this.anatomicalNameField;
            }
            set
            {
                this.anatomicalNameField = value;
            }
        }

        public TextType AnatomicalLocation
        {
            get
            {
                return this.anatomicalLocationField;
            }
            set
            {
                this.anatomicalLocationField = value;
            }
        }
    }
}