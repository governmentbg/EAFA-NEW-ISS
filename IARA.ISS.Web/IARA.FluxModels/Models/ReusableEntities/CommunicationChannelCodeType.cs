using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:Standard:QualifiedDataType:20")]
    public partial class CommunicationChannelCodeType
    {
        public CommunicationChannelCodeType()
        {
            this.listID = "3155_CommunicationChannelCode";
            this.listAgencyID = CommunicationChannelCodeListAgencyIDContentType.Item6;
            this.listVersionID = "D18B";
        }


        [XmlAttribute(DataType = "token")]
        public string listID { get; set; }


        [XmlAttribute]
        public CommunicationChannelCodeListAgencyIDContentType listAgencyID { get; set; }


        [XmlIgnore()]
        public bool listAgencyIDSpecified { get; set; }


        [XmlAttribute(DataType = "token")]
        public string listVersionID { get; set; }


        [XmlAttribute(DataType = "anyURI")]
        public string listURI { get; set; }


        [XmlText()]
        public CommunicationMeansTypeCodeContentType Value { get; set; }
    }
}
