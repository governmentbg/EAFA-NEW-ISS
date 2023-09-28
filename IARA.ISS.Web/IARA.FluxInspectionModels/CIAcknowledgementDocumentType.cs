namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIAcknowledgementDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIAcknowledgementDocumentType
    {

        private IDType idField;

        private DocumentCodeType[] typeCodeField;

        private AcknowledgementCodeType[] acknowledgementStatusCodeField;

        private TextType[] reasonInformationField;

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

        [XmlElement("TypeCode")]
        public DocumentCodeType[] TypeCode
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

        [XmlElement("AcknowledgementStatusCode")]
        public AcknowledgementCodeType[] AcknowledgementStatusCode
        {
            get
            {
                return this.acknowledgementStatusCodeField;
            }
            set
            {
                this.acknowledgementStatusCodeField = value;
            }
        }

        [XmlElement("ReasonInformation")]
        public TextType[] ReasonInformation
        {
            get
            {
                return this.reasonInformationField;
            }
            set
            {
                this.reasonInformationField = value;
            }
        }
    }
}