namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class PaymentTermsEventTimeReferenceCodeType
    {

        private string listIDField;

        private PaymentTermsEventTimeReferenceCodeListAgencyIDContentType listAgencyIDField;

        private bool listAgencyIDFieldSpecified;

        private string listVersionIDField;

        private EventTimeReferenceCodePaymentTermsEventContentType valueField;

        public PaymentTermsEventTimeReferenceCodeType()
        {
            this.listIDField = "2475_Payment Terms Event";
            this.listAgencyIDField = PaymentTermsEventTimeReferenceCodeListAgencyIDContentType.Item6;
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
        public PaymentTermsEventTimeReferenceCodeListAgencyIDContentType listAgencyID
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

        [XmlText]
        public EventTimeReferenceCodePaymentTermsEventContentType Value
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