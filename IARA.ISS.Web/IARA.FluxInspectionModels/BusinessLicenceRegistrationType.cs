namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("BusinessLicenceRegistration", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class BusinessLicenceRegistrationType
    {

        private CodeType typeCodeField;

        private DateType recordedDateField;

        private IDType licenceIDField;

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

        public DateType RecordedDate
        {
            get
            {
                return this.recordedDateField;
            }
            set
            {
                this.recordedDateField = value;
            }
        }

        public IDType LicenceID
        {
            get
            {
                return this.licenceIDField;
            }
            set
            {
                this.licenceIDField = value;
            }
        }
    }
}