using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class MDRDataNodeType
    {
        public IDType ID { get; set; }

        public IDType ParentID { get; set; }

        public NumericType HierarchicalLevelNumeric { get; set; }

        public DelimitedPeriodType EffectiveDelimitedPeriod { get; set; }

        [XmlElement("SubordinateMDRElementDataNode")]
        public MDRElementDataNodeType[] SubordinateMDRElementDataNode { get; set; }
    }
}
