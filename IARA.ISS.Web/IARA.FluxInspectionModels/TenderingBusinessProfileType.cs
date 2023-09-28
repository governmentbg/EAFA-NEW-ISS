namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingBusinessProfile", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingBusinessProfileType
    {

        private QuantityType operatingYearsQuantityField;

        private CodeType businessClassificationCodeField;

        private CodeType procuringProjectClassificationCodeField;

        private QuantityType totalEmployeeQuantityField;

        private IndicatorType manufacturerIndicatorField;

        private TextType countryNameField;

        private TenderingWorkCapabilityType applicationTenderingWorkCapabilityField;

        private SupplierFactoryType[] ownedSupplierFactoryField;

        private TenderingCompletedWorkType experienceTenderingCompletedWorkField;

        public QuantityType OperatingYearsQuantity
        {
            get => this.operatingYearsQuantityField;
            set => this.operatingYearsQuantityField = value;
        }

        public CodeType BusinessClassificationCode
        {
            get => this.businessClassificationCodeField;
            set => this.businessClassificationCodeField = value;
        }

        public CodeType ProcuringProjectClassificationCode
        {
            get => this.procuringProjectClassificationCodeField;
            set => this.procuringProjectClassificationCodeField = value;
        }

        public QuantityType TotalEmployeeQuantity
        {
            get => this.totalEmployeeQuantityField;
            set => this.totalEmployeeQuantityField = value;
        }

        public IndicatorType ManufacturerIndicator
        {
            get => this.manufacturerIndicatorField;
            set => this.manufacturerIndicatorField = value;
        }

        public TextType CountryName
        {
            get => this.countryNameField;
            set => this.countryNameField = value;
        }

        public TenderingWorkCapabilityType ApplicationTenderingWorkCapability
        {
            get => this.applicationTenderingWorkCapabilityField;
            set => this.applicationTenderingWorkCapabilityField = value;
        }

        [XmlElement("OwnedSupplierFactory")]
        public SupplierFactoryType[] OwnedSupplierFactory
        {
            get => this.ownedSupplierFactoryField;
            set => this.ownedSupplierFactoryField = value;
        }

        public TenderingCompletedWorkType ExperienceTenderingCompletedWork
        {
            get => this.experienceTenderingCompletedWorkField;
            set => this.experienceTenderingCompletedWorkField = value;
        }
    }
}