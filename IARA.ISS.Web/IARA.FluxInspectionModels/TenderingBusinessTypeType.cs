namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingBusinessType", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingBusinessTypeType
    {

        private CodeType classificationCodeField;

        private SubordinateBusinessTypeType[] availableSubordinateBusinessTypeField;

        private ProjectActualizationLocationType[] availableProjectActualizationLocationField;

        public CodeType ClassificationCode
        {
            get => this.classificationCodeField;
            set => this.classificationCodeField = value;
        }

        [XmlElement("AvailableSubordinateBusinessType")]
        public SubordinateBusinessTypeType[] AvailableSubordinateBusinessType
        {
            get => this.availableSubordinateBusinessTypeField;
            set => this.availableSubordinateBusinessTypeField = value;
        }

        [XmlElement("AvailableProjectActualizationLocation")]
        public ProjectActualizationLocationType[] AvailableProjectActualizationLocation
        {
            get => this.availableProjectActualizationLocationField;
            set => this.availableProjectActualizationLocationField = value;
        }
    }
}