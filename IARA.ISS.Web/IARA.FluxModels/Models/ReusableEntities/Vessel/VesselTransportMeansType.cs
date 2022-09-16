using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselTransportMeansType
    {
        [XmlElement("ID")]
        public IDType[] ID { get; set; }


        [XmlElement("Name")]
        public TextType[] Name { get; set; }


        [XmlElement("TypeCode")]
        public CodeType[] TypeCode { get; set; }


        public DateTimeType CommissioningDateTime { get; set; }


        public CodeType OperationalStatusCode { get; set; }


        public CodeType HullMaterialCode { get; set; }


        public MeasureType DraughtMeasure { get; set; }


        public MeasureType SpeedMeasure { get; set; }


        public MeasureType TrawlingSpeedMeasure { get; set; }


        public CodeType RoleCode { get; set; }


        public VesselCountryType RegistrationVesselCountry { get; set; }


        [XmlElement("SpecifiedVesselPositionEvent")]
        public VesselPositionEventType[] SpecifiedVesselPositionEvent { get; set; }


        [XmlElement("SpecifiedRegistrationEvent")]
        public RegistrationEventType[] SpecifiedRegistrationEvent { get; set; }


        public ConstructionEventType SpecifiedConstructionEvent { get; set; }


        [XmlElement("AttachedVesselEngine")]
        public VesselEngineType[] AttachedVesselEngine { get; set; }


        [XmlElement("SpecifiedVesselDimension")]
        public VesselDimensionType[] SpecifiedVesselDimension { get; set; }


        [XmlElement("OnBoardFishingGear")]
        public FishingGearType[] OnBoardFishingGear { get; set; }


        [XmlElement("ApplicableVesselEquipmentCharacteristic")]
        public VesselEquipmentCharacteristicType[] ApplicableVesselEquipmentCharacteristic { get; set; }


        [XmlElement("ApplicableVesselAdministrativeCharacteristic")]
        public VesselAdministrativeCharacteristicType[] ApplicableVesselAdministrativeCharacteristic { get; set; }


        [XmlElement("IllustrateFLUXPicture")]
        public FLUXPictureType[] IllustrateFLUXPicture { get; set; }


        [XmlElement("SpecifiedContactParty")]
        public ContactPartyType[] SpecifiedContactParty { get; set; }


        public VesselCrewType SpecifiedVesselCrew { get; set; }


        [XmlElement("ApplicableVesselStorageCharacteristic")]
        public VesselStorageCharacteristicType[] ApplicableVesselStorageCharacteristic { get; set; }


        [XmlElement("ApplicableVesselTechnicalCharacteristic")]
        public VesselTechnicalCharacteristicType[] ApplicableVesselTechnicalCharacteristic { get; set; }


        [XmlElement("GrantedFLAPDocument")]
        public FLAPDocumentType[] GrantedFLAPDocument { get; set; }


        [XmlElement("SpecifiedFLUXLocation")]
        public FLUXLocationType[] SpecifiedFLUXLocation { get; set; }


        [XmlElement("SpecifiedFACatch")]
        public FACatchType[] SpecifiedFACatch { get; set; }
    }
}
