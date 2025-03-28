namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingAmountType:D11A")]
    [XmlRoot("AccountingAmountType", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingAmountType:D11A", IsNullable = false)]
    public enum AccountingAmountTypeContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
        [XmlEnum("3")]
        Item3,
        [XmlEnum("4")]
        Item4,
        [XmlEnum("5")]
        Item5,
        [XmlEnum("6")]
        Item6,
    }
}