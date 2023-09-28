using IARA.FluxInspectionModels;

namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAAccountingAccountDimension", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAAccountingAccountDimensionType
    {

        private NumericType rankingNumeric;

        private IDType id;

        private TextType name;

        private NumericType lengthNumeric;

        public NumericType RankingNumeric
        {
            get => this.rankingNumeric;
            set => this.rankingNumeric = value;
        }

        public IDType ID
        {
            get => this.id;
            set => this.id = value;
        }

        public TextType Name
        {
            get => this.name;
            set => this.name = value;
        }

        public NumericType LengthNumeric
        {
            get => this.lengthNumeric;
            set => this.lengthNumeric = value;
        }
    }
}
