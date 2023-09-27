namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AmortizationMethod:D11A")]
    [XmlRoot("AmortizationMethod", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AmortizationMethod:D11A", IsNullable = false)]
    public enum AmortizationMethodContentType
    {

        [XmlEnum("11")]
        Item11,
        [XmlEnum("12")]
        Item12,
        [XmlEnum("14")]
        Item14,
        [XmlEnum("20")]
        Item20,
        [XmlEnum("21")]
        Item21,
        [XmlEnum("22")]
        Item22,
        [XmlEnum("31")]
        Item31,
        [XmlEnum("32")]
        Item32,
        [XmlEnum("33")]
        Item33,
        [XmlEnum("40")]
        Item40,
        [XmlEnum("41")]
        Item41,
        [XmlEnum("42")]
        Item42,
        [XmlEnum("43")]
        Item43,
        [XmlEnum("44")]
        Item44,
        [XmlEnum("45")]
        Item45,
        [XmlEnum("300")]
        Item300,
        [XmlEnum("301")]
        Item301,
    }
}