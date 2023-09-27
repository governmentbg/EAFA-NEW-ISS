namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingTechnicalCapability", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingTechnicalCapabilityType
    {

        private QuantityType qualifiedEngineerTotalProfessionalQuantityField;

        private QualifiedStaffType[] professionalQualifiedStaffField;

        private TenderingBusinessTypeType[] applicableTenderingBusinessTypeField;

        public QuantityType QualifiedEngineerTotalProfessionalQuantity
        {
            get => this.qualifiedEngineerTotalProfessionalQuantityField;
            set => this.qualifiedEngineerTotalProfessionalQuantityField = value;
        }

        [XmlElement("ProfessionalQualifiedStaff")]
        public QualifiedStaffType[] ProfessionalQualifiedStaff
        {
            get => this.professionalQualifiedStaffField;
            set => this.professionalQualifiedStaffField = value;
        }

        [XmlElement("ApplicableTenderingBusinessType")]
        public TenderingBusinessTypeType[] ApplicableTenderingBusinessType
        {
            get => this.applicableTenderingBusinessTypeField;
            set => this.applicableTenderingBusinessTypeField = value;
        }
    }
}