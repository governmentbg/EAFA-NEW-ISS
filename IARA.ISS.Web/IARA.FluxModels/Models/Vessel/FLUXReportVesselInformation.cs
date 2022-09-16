using System;
using System.Xml.Serialization;


namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXReportVesselInformation:5")]
    [XmlRoot("FLUXReportVesselInformation", Namespace = "urn:un:unece:uncefact:data:standard:FLUXReportVesselInformation:5", IsNullable = false)]
    public partial class FLUXReportVesselInformationType
    {
        public FLUXReportDocumentType FLUXReportDocument { get; set; }


        [XmlElement("VesselEvent")]
        public VesselEventType[] VesselEvent { get; set; }
    }

}
