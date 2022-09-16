using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
[Serializable]


    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20" +
        "")]
    public partial class GearProblemType
    {

        private CodeType typeCodeField;

        private QuantityType affectedQuantityField;

        private CodeType[] recoveryMeasureCodeField;

        private FLUXLocationType[] specifiedFLUXLocationField;

        private FishingGearType[] relatedFishingGearField;


        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }


        public QuantityType AffectedQuantity
        {
            get
            {
                return this.affectedQuantityField;
            }
            set
            {
                this.affectedQuantityField = value;
            }
        }


        [XmlElement("RecoveryMeasureCode")]
        public CodeType[] RecoveryMeasureCode
        {
            get
            {
                return this.recoveryMeasureCodeField;
            }
            set
            {
                this.recoveryMeasureCodeField = value;
            }
        }


        [XmlElement("SpecifiedFLUXLocation")]
        public FLUXLocationType[] SpecifiedFLUXLocation
        {
            get
            {
                return this.specifiedFLUXLocationField;
            }
            set
            {
                this.specifiedFLUXLocationField = value;
            }
        }


        [XmlElement("RelatedFishingGear")]
        public FishingGearType[] RelatedFishingGear
        {
            get
            {
                return this.relatedFishingGearField;
            }
            set
            {
                this.relatedFishingGearField = value;
            }
        }
    }
}