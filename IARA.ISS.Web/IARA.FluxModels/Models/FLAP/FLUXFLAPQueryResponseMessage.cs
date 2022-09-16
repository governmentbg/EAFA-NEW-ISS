using System;
using System.Xml.Serialization;


namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXFLAPQueryResponseMessage:1")]
    [XmlRoot("FLUXFLAPQueryResponseMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXFLAPQueryResponseMessage:1", IsNullable = false)]
    public partial class FLUXFLAPQueryResponseMessageType
    {
        public FLUXResponseDocumentType FLUXResponseDocument { get; set; }


        [XmlElement("FLAPDocument")]
        public FLAPDocumentType[] FLAPDocument { get; set; }
    }

}
