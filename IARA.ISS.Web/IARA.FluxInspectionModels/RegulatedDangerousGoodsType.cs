namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RegulatedDangerousGoods", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RegulatedDangerousGoodsType
    {

        private CodeType uNDGIdentificationCodeField;

        private TextType regulatoryAuthorityNameField;

        private TextType technicalNameField;

        private TextType informationField;

        private CodeType packagingDangerLevelCodeField;

        private IDType hazardClassificationIDField;

        public CodeType UNDGIdentificationCode
        {
            get => this.uNDGIdentificationCodeField;
            set => this.uNDGIdentificationCodeField = value;
        }

        public TextType RegulatoryAuthorityName
        {
            get => this.regulatoryAuthorityNameField;
            set => this.regulatoryAuthorityNameField = value;
        }

        public TextType TechnicalName
        {
            get => this.technicalNameField;
            set => this.technicalNameField = value;
        }

        public TextType Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public CodeType PackagingDangerLevelCode
        {
            get => this.packagingDangerLevelCodeField;
            set => this.packagingDangerLevelCodeField = value;
        }

        public IDType HazardClassificationID
        {
            get => this.hazardClassificationIDField;
            set => this.hazardClassificationIDField = value;
        }
    }
}