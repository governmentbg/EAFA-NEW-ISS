namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICLDocumentLineDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICLDocumentLineDocumentType
    {

        private IDType lineIDField;

        private LineStatusCodeType lineStatusCodeField;

        private DateTimeType latestRevisionDateTimeField;

        private CINoteType[] includedCINoteField;

        private CISpecifiedPeriodType effectiveCISpecifiedPeriodField;

        public IDType LineID
        {
            get
            {
                return this.lineIDField;
            }
            set
            {
                this.lineIDField = value;
            }
        }

        public LineStatusCodeType LineStatusCode
        {
            get
            {
                return this.lineStatusCodeField;
            }
            set
            {
                this.lineStatusCodeField = value;
            }
        }

        public DateTimeType LatestRevisionDateTime
        {
            get
            {
                return this.latestRevisionDateTimeField;
            }
            set
            {
                this.latestRevisionDateTimeField = value;
            }
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get
            {
                return this.includedCINoteField;
            }
            set
            {
                this.includedCINoteField = value;
            }
        }

        public CISpecifiedPeriodType EffectiveCISpecifiedPeriod
        {
            get
            {
                return this.effectiveCISpecifiedPeriodField;
            }
            set
            {
                this.effectiveCISpecifiedPeriodField = value;
            }
        }
    }
}