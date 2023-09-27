namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LaboratoryObservationAccessControlList", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LaboratoryObservationAccessControlListType
    {

        private CodeType typeCodeField;

        private CodeType accessRightCodeField;

        private LaboratoryObservationPartyType[] includedLaboratoryObservationPartyField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public CodeType AccessRightCode
        {
            get => this.accessRightCodeField;
            set => this.accessRightCodeField = value;
        }

        [XmlElement("IncludedLaboratoryObservationParty")]
        public LaboratoryObservationPartyType[] IncludedLaboratoryObservationParty
        {
            get => this.includedLaboratoryObservationPartyField;
            set => this.includedLaboratoryObservationPartyField = value;
        }
    }
}