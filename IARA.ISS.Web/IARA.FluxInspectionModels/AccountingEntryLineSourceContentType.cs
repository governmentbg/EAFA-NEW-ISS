namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingEntryLineSource:D11A")]
    [XmlRoot("AccountingEntryLineSource", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingEntryLineSource:D11A", IsNullable = false)]
    public enum AccountingEntryLineSourceContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
        [XmlEnum("3")]
        Item3,
        [XmlEnum("4")]
        Item4,
    }
}