namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LaboratoryObservationReference", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LaboratoryObservationReferenceType
    {

        private IDType idField;

        private TextType abbreviationField;

        private TextType descriptionField;

        private TextType statusField;

        private TextType commentField;

        private TextType valueField;

        private CodeType propertyReferenceCodeField;

        private CodeType valueCodeField;

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

        public TextType Abbreviation
        {
            get
            {
                return this.abbreviationField;
            }
            set
            {
                this.abbreviationField = value;
            }
        }

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public TextType Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        public TextType Comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        public TextType Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        public CodeType PropertyReferenceCode
        {
            get
            {
                return this.propertyReferenceCodeField;
            }
            set
            {
                this.propertyReferenceCodeField = value;
            }
        }

        public CodeType ValueCode
        {
            get
            {
                return this.valueCodeField;
            }
            set
            {
                this.valueCodeField = value;
            }
        }
    }
}