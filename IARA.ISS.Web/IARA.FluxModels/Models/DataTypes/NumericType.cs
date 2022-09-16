using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:20")]
    public partial class NumericType
    {

        [XmlAttribute]
        public string format { get; set; }


        [XmlText()]
        public decimal Value { get; set; }
    }
}
