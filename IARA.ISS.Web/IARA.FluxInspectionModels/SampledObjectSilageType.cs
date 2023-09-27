namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SampledObjectSilage", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SampledObjectSilageType
    {

        private DateTimeType cropMowingDateTimeField;

        private TextType processTypeField;

        private DateTimeType silagingDateTimeField;

        private BasePeriodType fieldDryingBasePeriodField;

        public DateTimeType CropMowingDateTime
        {
            get
            {
                return this.cropMowingDateTimeField;
            }
            set
            {
                this.cropMowingDateTimeField = value;
            }
        }

        public TextType ProcessType
        {
            get
            {
                return this.processTypeField;
            }
            set
            {
                this.processTypeField = value;
            }
        }

        public DateTimeType SilagingDateTime
        {
            get
            {
                return this.silagingDateTimeField;
            }
            set
            {
                this.silagingDateTimeField = value;
            }
        }

        public BasePeriodType FieldDryingBasePeriod
        {
            get
            {
                return this.fieldDryingBasePeriodField;
            }
            set
            {
                this.fieldDryingBasePeriodField = value;
            }
        }
    }
}