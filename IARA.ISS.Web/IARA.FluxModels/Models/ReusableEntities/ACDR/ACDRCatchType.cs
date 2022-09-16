using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:18")]
    public partial class ACDRCatchType
    {

        public CodeType FAOSpeciesCode { get; set; }


        public QuantityType UnitQuantity { get; set; }

        public MeasureType WeightMeasure { get; set; }


        public CodeType UsageCode { get; set; }
    }
}
