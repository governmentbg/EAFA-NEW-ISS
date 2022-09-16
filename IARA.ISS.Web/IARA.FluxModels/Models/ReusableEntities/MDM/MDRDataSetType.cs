using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class MDRDataSetType
    {
        public IDType ID { get; set; }


        public TextType Description { get; set; }


        public TextType Origin { get; set; }


        public TextType Name { get; set; }


        [XmlElement("SpecifiedDataSetVersion")]
        public DataSetVersionType[] SpecifiedDataSetVersion { get; set; }


        [XmlElement("ContainedMDRDataNode")]
        public MDRDataNodeType[] ContainedMDRDataNode { get; set; }
    }
}
