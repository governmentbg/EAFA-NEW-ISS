using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class AAPProcessType
    {

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode { get; set; }

        public NumericType ConversionFactorNumeric { get; set; }

        [XmlElement("UsedFACatch")]
        public FACatchType[] UsedFACatch { get; set; }


        [XmlElement("ResultAAPProduct")]
        public AAPProductType[] ResultAAPProduct { get; set; }
    }
}
