namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SoilOccupationContract", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SoilOccupationContractType
    {

        private IDType idField;

        private TextType descriptionField;

        private DateType startDateField;

        private CropDataSheetPartyType identifiedCropDataSheetPartyField;

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

        public DateType StartDate
        {
            get
            {
                return this.startDateField;
            }
            set
            {
                this.startDateField = value;
            }
        }

        public CropDataSheetPartyType IdentifiedCropDataSheetParty
        {
            get
            {
                return this.identifiedCropDataSheetPartyField;
            }
            set
            {
                this.identifiedCropDataSheetPartyField = value;
            }
        }
    }
}