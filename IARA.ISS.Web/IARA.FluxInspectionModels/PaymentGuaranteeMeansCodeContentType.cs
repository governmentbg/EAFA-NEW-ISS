namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:PaymentGuaranteeMeansCode:D22A")]
    [XmlRoot("PaymentGuaranteeMeansCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:PaymentGuaranteeMeansCode:D22A", IsNullable = false)]
    public enum PaymentGuaranteeMeansCodeContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("10")]
        Item10,
        [XmlEnum("11")]
        Item11,
        [XmlEnum("12")]
        Item12,
        [XmlEnum("13")]
        Item13,
        [XmlEnum("14")]
        Item14,
        [XmlEnum("20")]
        Item20,
        [XmlEnum("21")]
        Item21,
        [XmlEnum("23")]
        Item23,
        [XmlEnum("24")]
        Item24,
        [XmlEnum("45")]
        Item45,
        ZZZ,
    }
}