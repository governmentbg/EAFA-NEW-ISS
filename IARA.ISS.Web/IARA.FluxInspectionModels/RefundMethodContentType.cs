namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:RefundMethod:D11A")]
    [XmlRoot("RefundMethod", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:RefundMethod:D11A", IsNullable = false)]
    public enum RefundMethodContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
        [XmlEnum("3")]
        Item3,
    }
}