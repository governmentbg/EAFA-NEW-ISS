namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AmountWeightType:D11A")]
    [XmlRoot("AmountWeightType", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AmountWeightType:D11A", IsNullable = false)]
    public enum AmountWeightTypeContentType
    {

        D,
        MDC,
        MNC,
        R,
        T,
        TC,
    }
}