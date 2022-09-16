using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:20")]
    public partial class CodeType
    {
        [XmlAttribute(DataType = "token")]
        public string listID { get; set; }


        [XmlAttribute(DataType = "token")]
        public string listAgencyID { get; set; }


        [XmlAttribute]
        public string listAgencyName { get; set; }


        [XmlAttribute]
        public string listName { get; set; }


        [XmlAttribute(DataType = "token")]
        public string listVersionID { get; set; }


        [XmlAttribute]
        public string name { get; set; }


        [XmlAttribute(DataType = "token")]
        public string languageID { get; set; }


        [XmlAttribute(DataType = "anyURI")]
        public string listURI { get; set; }


        [XmlAttribute(DataType = "anyURI")]
        public string listSchemeURI { get; set; }


        [XmlText(DataType = "token")]
        public string Value { get; set; }
    }
}
