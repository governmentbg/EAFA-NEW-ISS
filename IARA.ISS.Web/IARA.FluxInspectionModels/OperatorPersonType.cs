namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("OperatorPerson", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class OperatorPersonType
    {

        private IDType idField;

        private TextType nameField;

        private AgriculturalCertificateType[] specifiedAgriculturalCertificateField;

        private CropProductionAgriculturalProcessType[][] specifiedAgriculturalManMachineSystemField;

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

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalCertificate")]
        public AgriculturalCertificateType[] SpecifiedAgriculturalCertificate
        {
            get
            {
                return this.specifiedAgriculturalCertificateField;
            }
            set
            {
                this.specifiedAgriculturalCertificateField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("SpecifiedCropProductionAgriculturalProcess", typeof(CropProductionAgriculturalProcessType), IsNullable = false)]
        public CropProductionAgriculturalProcessType[][] SpecifiedAgriculturalManMachineSystem
        {
            get
            {
                return this.specifiedAgriculturalManMachineSystemField;
            }
            set
            {
                this.specifiedAgriculturalManMachineSystemField = value;
            }
        }
    }
}