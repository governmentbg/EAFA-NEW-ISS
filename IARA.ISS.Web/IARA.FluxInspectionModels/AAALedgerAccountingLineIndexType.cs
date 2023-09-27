namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAALedgerAccountingLineIndex", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAALedgerAccountingLineIndexType
    {

        private NumericType lineNumericField;

        private NumericType folioNumericField;

        public NumericType LineNumeric
        {
            get
            {
                return this.lineNumericField;
            }
            set
            {
                this.lineNumericField = value;
            }
        }

        public NumericType FolioNumeric
        {
            get
            {
                return this.folioNumericField;
            }
            set
            {
                this.folioNumericField = value;
            }
        }
    }
}