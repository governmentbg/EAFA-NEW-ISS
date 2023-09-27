namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHouseCustomerClass", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHouseCustomerClassType
    {

        private CodeType categoryCodeField;

        private TextType categoryNameField;

        private NumericType upperAgeLimitNumericField;

        private NumericType lowerAgeLimitNumericField;

        private CodeType genderCodeField;

        private CodeType mealServiceCategoryCodeField;

        private IndicatorType specialBeddingServiceOfferedIndicatorField;

        private TextType descriptionField;

        public CodeType CategoryCode
        {
            get
            {
                return this.categoryCodeField;
            }
            set
            {
                this.categoryCodeField = value;
            }
        }

        public TextType CategoryName
        {
            get
            {
                return this.categoryNameField;
            }
            set
            {
                this.categoryNameField = value;
            }
        }

        public NumericType UpperAgeLimitNumeric
        {
            get
            {
                return this.upperAgeLimitNumericField;
            }
            set
            {
                this.upperAgeLimitNumericField = value;
            }
        }

        public NumericType LowerAgeLimitNumeric
        {
            get
            {
                return this.lowerAgeLimitNumericField;
            }
            set
            {
                this.lowerAgeLimitNumericField = value;
            }
        }

        public CodeType GenderCode
        {
            get
            {
                return this.genderCodeField;
            }
            set
            {
                this.genderCodeField = value;
            }
        }

        public CodeType MealServiceCategoryCode
        {
            get
            {
                return this.mealServiceCategoryCodeField;
            }
            set
            {
                this.mealServiceCategoryCodeField = value;
            }
        }

        public IndicatorType SpecialBeddingServiceOfferedIndicator
        {
            get
            {
                return this.specialBeddingServiceOfferedIndicatorField;
            }
            set
            {
                this.specialBeddingServiceOfferedIndicatorField = value;
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
    }
}