namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAEntryDayBook", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAEntryDayBookType
    {

        private IDType idField;

        private TextType commentField;

        private AAAEntryAccountingVoucherType[] includedAAAEntryAccountingVoucherField;

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

        public TextType Comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        [XmlElement("IncludedAAAEntryAccountingVoucher")]
        public AAAEntryAccountingVoucherType[] IncludedAAAEntryAccountingVoucher
        {
            get
            {
                return this.includedAAAEntryAccountingVoucherField;
            }
            set
            {
                this.includedAAAEntryAccountingVoucherField = value;
            }
        }
    }
}