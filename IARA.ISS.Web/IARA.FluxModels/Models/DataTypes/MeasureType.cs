using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:20")]
    public partial class MeasureType
    {
        [XmlAttribute(DataType = "token")]
        public string unitCode { get; set; }


        [XmlAttribute(DataType = "token")]
        public string unitCodeListVersionID { get; set; }


        [XmlText()]
        public decimal Value { get; set; }
    }
}
