namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AnimalLabel", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AnimalLabelType
    {

        private DateTimeType attachmentDateTimeField;

        private TextType typeField;

        public DateTimeType AttachmentDateTime
        {
            get
            {
                return this.attachmentDateTimeField;
            }
            set
            {
                this.attachmentDateTimeField = value;
            }
        }

        public TextType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }
}