namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReferencedStandard", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReferencedStandardType
    {

        private IDType idField;

        private IDType versionIDField;

        private IDType elementVersionIDField;

        private IDType uRIIDField;

        private IDType partIDField;

        private IDType agencyIDField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public IDType VersionID
        {
            get
            {
                return this.versionIDField;
            }
            set
            {
                this.versionIDField = value;
            }
        }

        public IDType ElementVersionID
        {
            get
            {
                return this.elementVersionIDField;
            }
            set
            {
                this.elementVersionIDField = value;
            }
        }

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

        public IDType PartID
        {
            get
            {
                return this.partIDField;
            }
            set
            {
                this.partIDField = value;
            }
        }

        public IDType AgencyID
        {
            get
            {
                return this.agencyIDField;
            }
            set
            {
                this.agencyIDField = value;
            }
        }
    }
}