namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFBusinessProfile", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFBusinessProfileType
    {

        private CodeType classificationCodeField;

        private RASFFCountryType[] consignmentDistributionRASFFCountryField;

        private RASFFDocumentType[] referencedRASFFDocumentField;

        public CodeType ClassificationCode
        {
            get => this.classificationCodeField;
            set => this.classificationCodeField = value;
        }

        [XmlElement("ConsignmentDistributionRASFFCountry")]
        public RASFFCountryType[] ConsignmentDistributionRASFFCountry
        {
            get => this.consignmentDistributionRASFFCountryField;
            set => this.consignmentDistributionRASFFCountryField = value;
        }

        [XmlElement("ReferencedRASFFDocument")]
        public RASFFDocumentType[] ReferencedRASFFDocument
        {
            get => this.referencedRASFFDocumentField;
            set => this.referencedRASFFDocumentField = value;
        }
    }
}