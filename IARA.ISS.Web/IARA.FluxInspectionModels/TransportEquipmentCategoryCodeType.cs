namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class TransportEquipmentCategoryCodeType
    {

        private string listIDField;

        private TransportEquipmentCategoryCodeListAgencyIDContentType listAgencyIDField;

        private bool listAgencyIDFieldSpecified;

        private string listVersionIDField;

        private string nameField;

        private TransportEquipmentCategoryCodeContentType valueField;

        public TransportEquipmentCategoryCodeType()
        {
            this.listIDField = "8053";
            this.listAgencyIDField = TransportEquipmentCategoryCodeListAgencyIDContentType.Item6;
            this.listVersionIDField = "D22A";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get => this.listIDField;
            set => this.listIDField = value;
        }

        [XmlAttribute]
        public TransportEquipmentCategoryCodeListAgencyIDContentType listAgencyID
        {
            get => this.listAgencyIDField;
            set => this.listAgencyIDField = value;
        }

        [XmlIgnore()]
        public bool listAgencyIDSpecified
        {
            get => this.listAgencyIDFieldSpecified;
            set => this.listAgencyIDFieldSpecified = value;
        }

        [XmlAttribute(DataType = "token")]
        public string listVersionID
        {
            get => this.listVersionIDField;
            set => this.listVersionIDField = value;
        }

        [XmlAttribute]
        public string name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlText]
        public TransportEquipmentCategoryCodeContentType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}