namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAPProcess", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAPProcessType
    {

        private CodeType[] typeCodeField;

        private NumericType conversionFactorNumericField;

        private FACatchType[] usedFACatchField;

        private AAPProductType[] resultAAPProductField;

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
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

        public NumericType ConversionFactorNumeric
        {
            get
            {
                return this.conversionFactorNumericField;
            }
            set
            {
                this.conversionFactorNumericField = value;
            }
        }

        [XmlElement("UsedFACatch")]
        public FACatchType[] UsedFACatch
        {
            get
            {
                return this.usedFACatchField;
            }
            set
            {
                this.usedFACatchField = value;
            }
        }

        [XmlElement("ResultAAPProduct")]
        public AAPProductType[] ResultAAPProduct
        {
            get
            {
                return this.resultAAPProductField;
            }
            set
            {
                this.resultAAPProductField = value;
            }
        }
    }
}