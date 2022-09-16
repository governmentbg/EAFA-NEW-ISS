using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselEngineType
    {
        public IDType SerialNumberID { get; set; }


        public CodeType TypeCode { get; set; }


        public CodeType RoleCode { get; set; }


        public CodeType PropulsionTypeCode { get; set; }


        [XmlElement("PowerMeasure")]
        public MeasureType[] PowerMeasure { get; set; }


        public CodeType PowerMeasurementMethodCode { get; set; }


        public CodeType ManufacturerCode { get; set; }


        public TextType Model { get; set; }


        public TextType Manufacturer { get; set; }


        [XmlElement("IllustrateFLUXPicture")]
        public FLUXPictureType[] IllustrateFLUXPicture { get; set; }
    }
}
