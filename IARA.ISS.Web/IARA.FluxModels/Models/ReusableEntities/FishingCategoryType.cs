using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20")]
    public partial class FishingCategoryType
    {
        public CodeType TypeCode { get; set; }


        [XmlElement("FishingMethodCode")]
        public CodeType[] FishingMethodCode { get; set; }


        [XmlElement("FishingMethod")]
        public TextType[] FishingMethod { get; set; }


        [XmlElement("FishingAreaCode")]
        public CodeType[] FishingAreaCode { get; set; }


        [XmlElement("FishingArea")]
        public TextType[] FishingArea { get; set; }


        public TextType SpecialCondition { get; set; }


        [XmlElement("AuthorizedFishingGear")]
        public FishingGearType[] AuthorizedFishingGear { get; set; }
    }
}
