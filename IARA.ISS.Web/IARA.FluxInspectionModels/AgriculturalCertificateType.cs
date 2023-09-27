namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalCertificate", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalCertificateType
    {

        private IDType idField;

        private DateTimeType issueDateTimeField;

        private DateTimeType expiryDateTimeField;

        private CodeType issueReasonCodeField;

        private CodeType typeCodeField;

        private DateTimeType requestedEffectiveDateTimeField;

        private DateTimeType actualEffectiveDateTimeField;

        private TextType descriptionField;

        private CodeType[] applicableObjectCodeField;

        private IDType issuingPartyIDField;

        private CodeType[] purposeCodeField;

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

        public DateTimeType IssueDateTime
        {
            get
            {
                return this.issueDateTimeField;
            }
            set
            {
                this.issueDateTimeField = value;
            }
        }

        public DateTimeType ExpiryDateTime
        {
            get
            {
                return this.expiryDateTimeField;
            }
            set
            {
                this.expiryDateTimeField = value;
            }
        }

        public CodeType IssueReasonCode
        {
            get
            {
                return this.issueReasonCodeField;
            }
            set
            {
                this.issueReasonCodeField = value;
            }
        }

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        public DateTimeType RequestedEffectiveDateTime
        {
            get
            {
                return this.requestedEffectiveDateTimeField;
            }
            set
            {
                this.requestedEffectiveDateTimeField = value;
            }
        }

        public DateTimeType ActualEffectiveDateTime
        {
            get
            {
                return this.actualEffectiveDateTimeField;
            }
            set
            {
                this.actualEffectiveDateTimeField = value;
            }
        }

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        [XmlElement("ApplicableObjectCode")]
        public CodeType[] ApplicableObjectCode
        {
            get
            {
                return this.applicableObjectCodeField;
            }
            set
            {
                this.applicableObjectCodeField = value;
            }
        }

        public IDType IssuingPartyID
        {
            get
            {
                return this.issuingPartyIDField;
            }
            set
            {
                this.issuingPartyIDField = value;
            }
        }

        [XmlElement("PurposeCode")]
        public CodeType[] PurposeCode
        {
            get
            {
                return this.purposeCodeField;
            }
            set
            {
                this.purposeCodeField = value;
            }
        }
    }
}