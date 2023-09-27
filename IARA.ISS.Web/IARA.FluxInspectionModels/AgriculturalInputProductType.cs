namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalInputProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalInputProductType
    {

        private IDType idField;



        private TextType descriptionField;

        private CodeType typeCodeField;

        private TextType useField;

        private CodeType speciesCodeField;

        private CodeType descriptionCodeField;

        private CodeType productionModeCodeField;

        private AgriculturalCharacteristicType[] applicableAgriculturalCharacteristicField;

        private AgriculturalInputMaterialType[] compositionAgriculturalInputMaterialField;

        private AgriculturalCertificateType[] specifiedAgriculturalCertificateField;

        private AgriculturalInputChemicalType[] specifiedAgriculturalInputChemicalField;

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

        private TextType name;
        public TextType Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
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

        public TextType Use
        {
            get
            {
                return this.useField;
            }
            set
            {
                this.useField = value;
            }
        }

        public CodeType SpeciesCode
        {
            get
            {
                return this.speciesCodeField;
            }
            set
            {
                this.speciesCodeField = value;
            }
        }

        public CodeType DescriptionCode
        {
            get
            {
                return this.descriptionCodeField;
            }
            set
            {
                this.descriptionCodeField = value;
            }
        }

        public CodeType ProductionModeCode
        {
            get
            {
                return this.productionModeCodeField;
            }
            set
            {
                this.productionModeCodeField = value;
            }
        }

        [XmlElement("ApplicableAgriculturalCharacteristic")]
        public AgriculturalCharacteristicType[] ApplicableAgriculturalCharacteristic
        {
            get
            {
                return this.applicableAgriculturalCharacteristicField;
            }
            set
            {
                this.applicableAgriculturalCharacteristicField = value;
            }
        }

        [XmlElement("CompositionAgriculturalInputMaterial")]
        public AgriculturalInputMaterialType[] CompositionAgriculturalInputMaterial
        {
            get
            {
                return this.compositionAgriculturalInputMaterialField;
            }
            set
            {
                this.compositionAgriculturalInputMaterialField = value;
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

        [XmlElement("SpecifiedAgriculturalInputChemical")]
        public AgriculturalInputChemicalType[] SpecifiedAgriculturalInputChemical
        {
            get
            {
                return this.specifiedAgriculturalInputChemicalField;
            }
            set
            {
                this.specifiedAgriculturalInputChemicalField = value;
            }
        }
    }
}