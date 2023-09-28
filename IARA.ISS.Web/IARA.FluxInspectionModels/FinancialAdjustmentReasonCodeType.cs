namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class FinancialAdjustmentReasonCodeType
    {

        private string listIDField;

        private FinancialAdjustmentReasonCodeListAgencyIDContentType listAgencyIDField;

        private bool listAgencyIDFieldSpecified;

        private string listVersionIDField;

        private AdjustmentReasonDescriptionCodeFinancialContentType valueField;

        public FinancialAdjustmentReasonCodeType()
        {
            this.listIDField = "4465_AdjustmentReasonCode_Financial";
            this.listAgencyIDField = FinancialAdjustmentReasonCodeListAgencyIDContentType.Item6;
            this.listVersionIDField = "D22A";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get => this.listIDField;
            set => this.listIDField = value;
        }

        [XmlAttribute]
        public FinancialAdjustmentReasonCodeListAgencyIDContentType listAgencyID
        {
            get => this.listAgencyIDField;
            set => this.listAgencyIDField = value;
        }

        [XmlIgnore()]
        public bool listAgencyIDSpecified
        {
            get => this.listAgencyIDFieldSpecified;
            set => this.listAgencyIDFieldSpecified = value;
        }

        [XmlAttribute(DataType = "token")]
        public string listVersionID
        {
            get => this.listVersionIDField;
            set => this.listVersionIDField = value;
        }

        [XmlText]
        public AdjustmentReasonDescriptionCodeFinancialContentType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}