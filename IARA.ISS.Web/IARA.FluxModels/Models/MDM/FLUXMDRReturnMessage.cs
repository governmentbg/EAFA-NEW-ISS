using System;
using System.Xml.Serialization;


namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXMDRReturnMessage:5")]
    [XmlRoot("FLUXMDRReturnMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXMDRReturnMessage:5", IsNullable = false)]
    public partial class FLUXMDRReturnMessageType
    {
        public FLUXResponseDocumentType FLUXResponseDocument { get; set; }

        public MDRDataSetType MDRDataSet { get; set; }
    }

}
