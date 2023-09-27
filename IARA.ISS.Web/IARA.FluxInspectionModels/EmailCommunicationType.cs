namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("EmailCommunication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class EmailCommunicationType
    {

        private IDType uRIIDField;

        public IDType URIID
        {
            get
            {
                return this.uRIIDField;
            }
            set
            {
                this.uRIIDField = value;
            }
        }
    }
}