namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:DeliveryTermsCode:2020")]
    [XmlRoot("DeliveryTermsCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:DeliveryTermsCode:2020", IsNullable = false)]
    public enum DeliveryTermsCodeContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
        CFR,
        CIF,
        CIP,
        CPT,
        DAP,
        DDP,
        DPU,
        EXW,
        FAS,
        FCA,
        FOB,
    }
}