namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingDeliverable", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingDeliverableType
    {

        private IDType idField;

        private IDType secondaryIDField;

        private CodeType[] classificationCodeField;

        private TextType descriptionField;

        private TextType nameField;

        private TextType[] specificationField;

        private TextType[] specificationReferenceField;

        private BinaryObjectType[] referenceBinaryObjectField;

        private TextType[] originCountryNameField;

        private QuantityType procurementQuantityField;

        private AmountType procurementAmountField;

        private TenderingDeliverableType[] supplementaryTenderingDeliverableField;

        private TenderingPeriodType[] applicableTenderingPeriodField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType SecondaryID
        {
            get => this.secondaryIDField;
            set => this.secondaryIDField = value;
        }

        [XmlElement("ClassificationCode")]
        public CodeType[] ClassificationCode
        {
            get => this.classificationCodeField;
            set => this.classificationCodeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlElement("Specification")]
        public TextType[] Specification
        {
            get => this.specificationField;
            set => this.specificationField = value;
        }

        [XmlElement("SpecificationReference")]
        public TextType[] SpecificationReference
        {
            get => this.specificationReferenceField;
            set => this.specificationReferenceField = value;
        }

        [XmlElement("ReferenceBinaryObject")]
        public BinaryObjectType[] ReferenceBinaryObject
        {
            get => this.referenceBinaryObjectField;
            set => this.referenceBinaryObjectField = value;
        }

        [XmlElement("OriginCountryName")]
        public TextType[] OriginCountryName
        {
            get => this.originCountryNameField;
            set => this.originCountryNameField = value;
        }

        public QuantityType ProcurementQuantity
        {
            get => this.procurementQuantityField;
            set => this.procurementQuantityField = value;
        }

        public AmountType ProcurementAmount
        {
            get => this.procurementAmountField;
            set => this.procurementAmountField = value;
        }

        [XmlElement("SupplementaryTenderingDeliverable")]
        public TenderingDeliverableType[] SupplementaryTenderingDeliverable
        {
            get => this.supplementaryTenderingDeliverableField;
            set => this.supplementaryTenderingDeliverableField = value;
        }

        [XmlElement("ApplicableTenderingPeriod")]
        public TenderingPeriodType[] ApplicableTenderingPeriod
        {
            get => this.applicableTenderingPeriodField;
            set => this.applicableTenderingPeriodField = value;
        }
    }
}