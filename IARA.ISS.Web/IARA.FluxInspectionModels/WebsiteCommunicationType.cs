namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("WebsiteCommunication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class WebsiteCommunicationType
    {

        private IDType websiteURIIDField;

        public IDType WebsiteURIID
        {
            get
            {
                return this.websiteURIIDField;
            }
            set
            {
                this.websiteURIIDField = value;
            }
        }
    }
}