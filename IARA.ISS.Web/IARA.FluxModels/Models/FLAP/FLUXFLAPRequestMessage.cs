using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXFLAPRequestMessage:7")]
    [XmlRoot("FLUXFLAPRequestMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXFLAPRequestMessage:7", IsNullable = false)]
    public partial class FLUXFLAPRequestMessageType
    {
        public FLUXReportDocumentType FLUXReportDocument { get; set; }


        public FLAPRequestDocumentType FLAPRequestDocument { get; set; }


        public FLAPDocumentType FLAPDocument { get; set; }


        [XmlElement("VesselEvent")]
        public VesselEventType[] VesselEvent { get; set; }
    }


}
