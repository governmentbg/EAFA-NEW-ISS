namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TransportCargo", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TransportCargoType
    {

        private CargoCategoryCodeType typeCodeField;

        private TextType[] identificationField;

        private CargoOperationalCategoryCodeType operationalCategoryCodeField;

        private CargoCommodityCategoryCodeType statisticalClassificationCodeField;

        public CargoCategoryCodeType TypeCode
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

        [XmlElement("Identification")]
        public TextType[] Identification
        {
            get
            {
                return this.identificationField;
            }
            set
            {
                this.identificationField = value;
            }
        }

        public CargoOperationalCategoryCodeType OperationalCategoryCode
        {
            get
            {
                return this.operationalCategoryCodeField;
            }
            set
            {
                this.operationalCategoryCodeField = value;
            }
        }

        public CargoCommodityCategoryCodeType StatisticalClassificationCode
        {
            get
            {
                return this.statisticalClassificationCodeField;
            }
            set
            {
                this.statisticalClassificationCodeField = value;
            }
        }
    }
}