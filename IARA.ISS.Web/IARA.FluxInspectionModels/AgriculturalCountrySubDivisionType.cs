namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalCountrySubDivision", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalCountrySubDivisionType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private TextType descriptionField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

        private AgriculturalAreaType[] includedAgriculturalAreaField;

        private CropDataSheetPartyType ownerCropDataSheetPartyField;

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

        [XmlElement("ApplicableTechnicalCharacteristic")]
        public TechnicalCharacteristicType[] ApplicableTechnicalCharacteristic
        {
            get
            {
                return this.applicableTechnicalCharacteristicField;
            }
            set
            {
                this.applicableTechnicalCharacteristicField = value;
            }
        }

        [XmlElement("IncludedAgriculturalArea")]
        public AgriculturalAreaType[] IncludedAgriculturalArea
        {
            get
            {
                return this.includedAgriculturalAreaField;
            }
            set
            {
                this.includedAgriculturalAreaField = value;
            }
        }

        public CropDataSheetPartyType OwnerCropDataSheetParty
        {
            get
            {
                return this.ownerCropDataSheetPartyField;
            }
            set
            {
                this.ownerCropDataSheetPartyField = value;
            }
        }
    }
}