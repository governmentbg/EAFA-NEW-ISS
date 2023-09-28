namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FLUXReportDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FLUXReportDocumentType
    {

        private IDType[] idField;

        private IDType referencedIDField;

        private DateTimeType creationDateTimeField;

        private CodeType purposeCodeField;

        private TextType purposeField;

        private CodeType typeCodeField;

        private FLUXPartyType ownerFLUXPartyField;

        [XmlElement("ID")]
        public IDType[] ID
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

        public IDType ReferencedID
        {
            get
            {
                return this.referencedIDField;
            }
            set
            {
                this.referencedIDField = value;
            }
        }

        public DateTimeType CreationDateTime
        {
            get
            {
                return this.creationDateTimeField;
            }
            set
            {
                this.creationDateTimeField = value;
            }
        }

        public CodeType PurposeCode
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

        public TextType Purpose
        {
            get
            {
                return this.purposeField;
            }
            set
            {
                this.purposeField = value;
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

        public FLUXPartyType OwnerFLUXParty
        {
            get
            {
                return this.ownerFLUXPartyField;
            }
            set
            {
                this.ownerFLUXPartyField = value;
            }
        }
    }
}