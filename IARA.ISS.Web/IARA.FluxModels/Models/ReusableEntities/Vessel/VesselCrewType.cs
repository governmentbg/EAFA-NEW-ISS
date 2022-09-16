using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class VesselCrewType
    {
        public CodeType TypeCode { get; set; }

        public QuantityType MemberQuantity { get; set; }

        public QuantityType MinimumSizeQuantity { get; set; }

        public QuantityType MaximumSizeQuantity { get; set; }

        public QuantityType OnDeckSizeQuantity { get; set; }

        public QuantityType AboveDeckSizeQuantity { get; set; }
    }
}
