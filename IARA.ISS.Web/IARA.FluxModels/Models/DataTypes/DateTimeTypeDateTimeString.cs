using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:20")]
    public partial class DateTimeTypeDateTimeString
    {
        [XmlAttribute]
        public string format { get; set; }


        [XmlText()]
        public string Value { get; set; }
    }
}
