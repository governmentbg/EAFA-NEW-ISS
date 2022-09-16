using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:20")]
    public partial class DateTimeType
    {
        //[XmlElement("DateTime", typeof(DateTime))]
        //[XmlElement("DateTimeString", typeof(DateTimeTypeDateTimeString))]
        //public object Item { get; set; }


        [XmlElement("DateTime", typeof(DateTime))]
        //[XmlElement("DateTimeString", typeof(DateTimeTypeDateTimeString))]
        public DateTime Item { get; set; }
    }
}
