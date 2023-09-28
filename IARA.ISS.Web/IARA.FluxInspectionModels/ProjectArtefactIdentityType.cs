namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectArtefactIdentity", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectArtefactIdentityType
    {

        private IDType classificationIDField;

        private IDType idField;

        public IDType ClassificationID
        {
            get => this.classificationIDField;
            set => this.classificationIDField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }
    }
}