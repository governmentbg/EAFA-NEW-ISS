namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LogisticsShippingMarks", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LogisticsShippingMarksType
    {

        private TextType[] markingField;

        private MarkingInstructionCodeType[] markingInstructionCodeField;

        private LogisticsLabelType[] barcodeLogisticsLabelField;

        private LogisticsLabelType[] rFIDLogisticsLabelField;

        private LogisticsLabelType[] vINLogisticsLabelField;

        [XmlElement("Marking")]
        public TextType[] Marking
        {
            get
            {
                return this.markingField;
            }
            set
            {
                this.markingField = value;
            }
        }

        [XmlElement("MarkingInstructionCode")]
        public MarkingInstructionCodeType[] MarkingInstructionCode
        {
            get
            {
                return this.markingInstructionCodeField;
            }
            set
            {
                this.markingInstructionCodeField = value;
            }
        }

        [XmlElement("BarcodeLogisticsLabel")]
        public LogisticsLabelType[] BarcodeLogisticsLabel
        {
            get
            {
                return this.barcodeLogisticsLabelField;
            }
            set
            {
                this.barcodeLogisticsLabelField = value;
            }
        }

        [XmlElement("RFIDLogisticsLabel")]
        public LogisticsLabelType[] RFIDLogisticsLabel
        {
            get
            {
                return this.rFIDLogisticsLabelField;
            }
            set
            {
                this.rFIDLogisticsLabelField = value;
            }
        }

        [XmlElement("VINLogisticsLabel")]
        public LogisticsLabelType[] VINLogisticsLabel
        {
            get
            {
                return this.vINLogisticsLabelField;
            }
            set
            {
                this.vINLogisticsLabelField = value;
            }
        }
    }
}