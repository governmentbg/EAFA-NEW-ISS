namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingAccountBalanceReopeningType:D11A")]
    [XmlRoot("AccountingAccountBalanceReopeningType", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingAccountBalanceReopeningType:D11A", IsNullable = false)]
    public enum AccountingAccountBalanceReopeningTypeContentType
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
    }
}