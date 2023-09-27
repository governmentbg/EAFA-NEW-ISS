namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CILogisticsTransportMovement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CILogisticsTransportMovementType
    {

        private TransportModeCodeType modeCodeField;

        private TextType modeField;

        private TransportMovementStageCodeType stageCodeField;

        private CodeType serviceCodeField;

        private TextType serviceField;

        private IDType idField;

        private TextType typeField;

        private TextType cycleField;

        private CILogisticsTransportMeansType usedCILogisticsTransportMeansField;

        public TransportModeCodeType ModeCode
        {
            get
            {
                return this.modeCodeField;
            }
            set
            {
                this.modeCodeField = value;
            }
        }

        public TextType Mode
        {
            get
            {
                return this.modeField;
            }
            set
            {
                this.modeField = value;
            }
        }

        public TransportMovementStageCodeType StageCode
        {
            get
            {
                return this.stageCodeField;
            }
            set
            {
                this.stageCodeField = value;
            }
        }

        public CodeType ServiceCode
        {
            get
            {
                return this.serviceCodeField;
            }
            set
            {
                this.serviceCodeField = value;
            }
        }

        public TextType Service
        {
            get
            {
                return this.serviceField;
            }
            set
            {
                this.serviceField = value;
            }
        }

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public TextType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        public TextType Cycle
        {
            get
            {
                return this.cycleField;
            }
            set
            {
                this.cycleField = value;
            }
        }

        public CILogisticsTransportMeansType UsedCILogisticsTransportMeans
        {
            get
            {
                return this.usedCILogisticsTransportMeansField;
            }
            set
            {
                this.usedCILogisticsTransportMeansField = value;
            }
        }
    }
}