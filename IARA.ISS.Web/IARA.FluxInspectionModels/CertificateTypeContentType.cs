namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:CertificateType:D11A")]
    [XmlRoot("CertificateType", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:CertificateType:D11A", IsNullable = false)]
    public enum CertificateTypeContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
        [XmlEnum("3")]
        Item3,
    }
}