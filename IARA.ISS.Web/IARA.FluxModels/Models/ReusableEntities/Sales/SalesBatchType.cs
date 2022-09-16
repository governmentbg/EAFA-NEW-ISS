using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class SalesBatchType
    {
        [XmlElement("ID")]
        public IDType[] ID { get; set; }


        [XmlElement("SpecifiedAAPProduct")]
        public AAPProductType[] SpecifiedAAPProduct { get; set; }


        [XmlArrayItem("ChargeAmount", IsNullable = false)]
        public AmountType[] TotalSalesPrice { get; set; }
    }
}
