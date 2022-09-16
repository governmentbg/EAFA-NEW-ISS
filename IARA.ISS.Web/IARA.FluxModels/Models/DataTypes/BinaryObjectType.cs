using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:20")]
    public partial class BinaryObjectType
    {
        [XmlAttribute]
        public string format { get; set; }


        [XmlAttribute(DataType = "token")]
        public string mimeCode { get; set; }


        [XmlAttribute(DataType = "token")]
        public string encodingCode { get; set; }


        [XmlAttribute(DataType = "token")]
        public string characterSetCode { get; set; }


        [XmlAttribute(DataType = "anyURI")]
        public string uri { get; set; }


        [XmlAttribute]
        public string filename { get; set; }


        [XmlText(DataType = "base64Binary")]
        public byte[] Value { get; set; }
    }
}
