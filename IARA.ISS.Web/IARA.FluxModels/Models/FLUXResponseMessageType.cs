using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXResponseMessage:6")]
    [XmlRoot("FLUXResponseMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXResponseMessage:6", IsNullable = false)]
    public partial class FLUXResponseMessageType
    {
        public FLUXResponseDocumentType FLUXResponseDocument { get; set; }
    }
}
