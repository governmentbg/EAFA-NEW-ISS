namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FLAPRequestDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FLAPRequestDocumentType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private CodeType fADATypeCodeField;

        private CodeType purposeCodeField;

        private TextType purposeField;

        private FishingCategoryType relatedFishingCategoryField;

        private ValidationResultDocumentType[] relatedValidationResultDocumentField;

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

        public CodeType FADATypeCode
        {
            get
            {
                return this.fADATypeCodeField;
            }
            set
            {
                this.fADATypeCodeField = value;
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

        public FishingCategoryType RelatedFishingCategory
        {
            get
            {
                return this.relatedFishingCategoryField;
            }
            set
            {
                this.relatedFishingCategoryField = value;
            }
        }

        [XmlElement("RelatedValidationResultDocument")]
        public ValidationResultDocumentType[] RelatedValidationResultDocument
        {
            get
            {
                return this.relatedValidationResultDocumentField;
            }
            set
            {
                this.relatedValidationResultDocumentField = value;
            }
        }
    }
}