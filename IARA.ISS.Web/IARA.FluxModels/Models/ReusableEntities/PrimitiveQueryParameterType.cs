using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
[Serializable]


    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20" +
        "")]
    public partial class PrimitiveQueryParameterType
    {

        private IndicatorType applicableIndicatorField;

        private TextType comparatorField;

        private DateTimeType documentPublicationDateTimeField;

        private TextType scopeField;

        private NumericType realNumberNumericField;

        private NumericType integerNumberNumericField;

        private IndicatorType negationIndicatorField;

        private TextType keywordField;

        private CodeType typeCodeField;


        public IndicatorType ApplicableIndicator
        {
            get
            {
                return this.applicableIndicatorField;
            }
            set
            {
                this.applicableIndicatorField = value;
            }
        }


        public TextType Comparator
        {
            get
            {
                return this.comparatorField;
            }
            set
            {
                this.comparatorField = value;
            }
        }


        public DateTimeType DocumentPublicationDateTime
        {
            get
            {
                return this.documentPublicationDateTimeField;
            }
            set
            {
                this.documentPublicationDateTimeField = value;
            }
        }


        public TextType Scope
        {
            get
            {
                return this.scopeField;
            }
            set
            {
                this.scopeField = value;
            }
        }


        public NumericType RealNumberNumeric
        {
            get
            {
                return this.realNumberNumericField;
            }
            set
            {
                this.realNumberNumericField = value;
            }
        }


        public NumericType IntegerNumberNumeric
        {
            get
            {
                return this.integerNumberNumericField;
            }
            set
            {
                this.integerNumberNumericField = value;
            }
        }


        public IndicatorType NegationIndicator
        {
            get
            {
                return this.negationIndicatorField;
            }
            set
            {
                this.negationIndicatorField = value;
            }
        }


        public TextType Keyword
        {
            get
            {
                return this.keywordField;
            }
            set
            {
                this.keywordField = value;
            }
        }


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
    }
}