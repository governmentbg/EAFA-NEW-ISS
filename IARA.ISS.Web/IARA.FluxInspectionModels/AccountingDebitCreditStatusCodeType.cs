namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class AccountingDebitCreditStatusCodeType
    {

        private string listIDField;

        private AccountingDebitCreditStatusCodeListAgencyIDContentType listAgencyIDField;

        private bool listAgencyIDFieldSpecified;

        private string listVersionIDField;

        private string listURIField;

        private StatusDescriptionCodeAccountingDebitCreditContentType valueField;

        public AccountingDebitCreditStatusCodeType()
        {
            this.listIDField = "4405_Accounting Debit Credit";
            this.listAgencyIDField = AccountingDebitCreditStatusCodeListAgencyIDContentType.Item6;
            this.listVersionIDField = "D22A";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        [XmlAttribute]
        public AccountingDebitCreditStatusCodeListAgencyIDContentType listAgencyID
        {
            get
            {
                return this.listAgencyIDField;
            }
            set
            {
                this.listAgencyIDField = value;
            }
        }

        [XmlIgnore]
        public bool listAgencyIDSpecified
        {
            get
            {
                return this.listAgencyIDFieldSpecified;
            }
            set
            {
                this.listAgencyIDFieldSpecified = value;
            }
        }

        [XmlAttribute(DataType = "token")]
        public string listVersionID
        {
            get
            {
                return this.listVersionIDField;
            }
            set
            {
                this.listVersionIDField = value;
            }
        }

        [XmlAttribute(DataType = "anyURI")]
        public string listURI
        {
            get
            {
                return this.listURIField;
            }
            set
            {
                this.listURIField = value;
            }
        }

        [XmlText]
        public StatusDescriptionCodeAccountingDebitCreditContentType Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}