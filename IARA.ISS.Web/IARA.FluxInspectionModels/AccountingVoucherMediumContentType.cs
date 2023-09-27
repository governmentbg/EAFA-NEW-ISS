namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingVoucherMedium:D11A")]
    [XmlRoot("AccountingVoucherMedium", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingVoucherMedium:D11A", IsNullable = false)]
    public enum AccountingVoucherMediumContentType
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
        [XmlEnum("7")]
        Item7,
        [XmlEnum("8")]
        Item8,
        [XmlEnum("9")]
        Item9,
        [XmlEnum("10")]
        Item10,
    }
}