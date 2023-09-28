namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("QualifiedStaff", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class QualifiedStaffType
    {

        private QuantityType totalQuantityField;

        private CodeType gradeCodeField;

        private CodeType licenceLevelOneClassificationCodeField;

        private CodeType licenceLevelTwoClassificationCodeField;

        private CodeType licenceLevelThreeClassificationCodeField;

        public QuantityType TotalQuantity
        {
            get => this.totalQuantityField;
            set => this.totalQuantityField = value;
        }

        public CodeType GradeCode
        {
            get => this.gradeCodeField;
            set => this.gradeCodeField = value;
        }

        public CodeType LicenceLevelOneClassificationCode
        {
            get => this.licenceLevelOneClassificationCodeField;
            set => this.licenceLevelOneClassificationCodeField = value;
        }

        public CodeType LicenceLevelTwoClassificationCode
        {
            get => this.licenceLevelTwoClassificationCodeField;
            set => this.licenceLevelTwoClassificationCodeField = value;
        }

        public CodeType LicenceLevelThreeClassificationCode
        {
            get => this.licenceLevelThreeClassificationCodeField;
            set => this.licenceLevelThreeClassificationCodeField = value;
        }
    }
}