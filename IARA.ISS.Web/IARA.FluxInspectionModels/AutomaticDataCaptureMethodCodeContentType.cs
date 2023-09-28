namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:AutomaticDataCaptureMethodCode:D22A")]
    [XmlRoot("AutomaticDataCaptureMethodCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:AutomaticDataCaptureMethodCode:D22A", IsNullable = false)]
    public enum AutomaticDataCaptureMethodCodeContentType
    {

        [XmlEnum("50")]
        Item50,
        [XmlEnum("51")]
        Item51,
        [XmlEnum("52")]
        Item52,
        [XmlEnum("64")]
        Item64,
        [XmlEnum("65")]
        Item65,
        [XmlEnum("67")]
        Item67,
        [XmlEnum("78")]
        Item78,
        [XmlEnum("79")]
        Item79,
        [XmlEnum("81")]
        Item81,
        [XmlEnum("82")]
        Item82,
    }
}