namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedAgriculturalDevice", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedAgriculturalDeviceType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private CodeType subordinateTypeCodeField;

        private CropDataSheetPartyType ownerCropDataSheetPartyField;

        private SpecifiedAgriculturalDeviceType[] combinedSpecifiedAgriculturalDeviceField;

        private CropProductionAgriculturalProcessType[][] specifiedAgriculturalManMachineSystemField;

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

        public CropDataSheetPartyType OwnerCropDataSheetParty
        {
            get
            {
                return this.ownerCropDataSheetPartyField;
            }
            set
            {
                this.ownerCropDataSheetPartyField = value;
            }
        }

        [XmlElement("CombinedSpecifiedAgriculturalDevice")]
        public SpecifiedAgriculturalDeviceType[] CombinedSpecifiedAgriculturalDevice
        {
            get
            {
                return this.combinedSpecifiedAgriculturalDeviceField;
            }
            set
            {
                this.combinedSpecifiedAgriculturalDeviceField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("SpecifiedCropProductionAgriculturalProcess", typeof(CropProductionAgriculturalProcessType), IsNullable = false)]
        public CropProductionAgriculturalProcessType[][] SpecifiedAgriculturalManMachineSystem
        {
            get
            {
                return this.specifiedAgriculturalManMachineSystemField;
            }
            set
            {
                this.specifiedAgriculturalManMachineSystemField = value;
            }
        }
    }
}