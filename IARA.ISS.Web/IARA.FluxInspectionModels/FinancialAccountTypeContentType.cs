namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:FinancialAccountType:D11A")]
    [XmlRoot("FinancialAccountType", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:FinancialAccountType:D11A", IsNullable = false)]
    public enum FinancialAccountTypeContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
    }
}