namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingAccountClassification:D11A")]
    [XmlRoot("AccountingAccountClassification", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingAccountClassification:D11A", IsNullable = false)]
    public enum AccountingAccountClassificationContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
        [XmlEnum("3")]
        Item3,
    }
}