using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:20")]
    public partial class TextType
    {
        [XmlAttribute(DataType = "token")]
        public string languageID { get; set; }


        [XmlAttribute(DataType = "token")]
        public string languageLocaleID { get; set; }


        [XmlText()]
        public string Value { get; set; }
    }
}
