using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXFLAPResponseMessage:7")]
    [XmlRoot("FLUXFLAPResponseMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXFLAPResponseMessage:7", IsNullable = false)]
    public partial class FLUXFLAPResponseMessageType
    {
        public FLUXResponseDocumentType FLUXResponseDocument { get; set; }


        public FLAPRequestDocumentType FLAPRequestDocument { get; set; }


        public FLAPDocumentType FLAPDocument { get; set; }
    }

}
