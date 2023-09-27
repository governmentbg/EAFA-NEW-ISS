namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CropProduce", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CropProduceType
    {

        private IDType[] idField;

        private CodeType typeCodeField;

        private CodeType subordinateTypeCodeField;

        private TextType nameField;

        private CropProduceBatchType[] inputSpecifiedCropProduceBatchField;

        private CropProduceBatchType[] outputSpecifiedCropProduceBatchField;

        [XmlElement("ID")]
        public IDType[] ID
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

        public CodeType SubordinateTypeCode
        {
            get
            {
                return this.subordinateTypeCodeField;
            }
            set
            {
                this.subordinateTypeCodeField = value;
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

        [XmlElement("InputSpecifiedCropProduceBatch")]
        public CropProduceBatchType[] InputSpecifiedCropProduceBatch
        {
            get
            {
                return this.inputSpecifiedCropProduceBatchField;
            }
            set
            {
                this.inputSpecifiedCropProduceBatchField = value;
            }
        }

        [XmlElement("OutputSpecifiedCropProduceBatch")]
        public CropProduceBatchType[] OutputSpecifiedCropProduceBatch
        {
            get
            {
                return this.outputSpecifiedCropProduceBatchField;
            }
            set
            {
                this.outputSpecifiedCropProduceBatchField = value;
            }
        }
    }
}