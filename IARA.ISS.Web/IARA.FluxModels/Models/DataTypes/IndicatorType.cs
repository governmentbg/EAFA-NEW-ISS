using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:20")]
    public partial class IndicatorType
    {
        [XmlElement("Indicator", typeof(bool))]
        [XmlElement("IndicatorString", typeof(IndicatorTypeIndicatorString))]
        public object Item { get; set; }
    }
}
