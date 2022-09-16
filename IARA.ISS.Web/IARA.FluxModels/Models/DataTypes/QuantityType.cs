using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:20")]
    public partial class QuantityType
    {
        [XmlAttribute(DataType = "token")]
        public string unitCode { get; set; }


        [XmlAttribute(DataType = "token")]
        public string unitCodeListID { get; set; }


        [XmlAttribute(DataType = "token")]
        public string unitCodeListAgencyID { get; set; }


        [XmlAttribute]
        public string unitCodeListAgencyName { get; set; }


        [XmlText()]
        public decimal Value { get; set; }
    }
}
