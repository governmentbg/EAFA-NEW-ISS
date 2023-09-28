namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:EventTimeReferenceCodePaymentTermsEvent:D22A")]
    [XmlRoot("EventTimeReferenceCodePaymentTermsEvent", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:EventTimeReferenceCodePaymentTermsEvent:D22A", IsNullable = false)]
    public enum EventTimeReferenceCodePaymentTermsEventContentType
    {

        [XmlEnum("5")]
        Item5,
        [XmlEnum("24")]
        Item24,
        [XmlEnum("29")]
        Item29,
        [XmlEnum("45")]
        Item45,
        [XmlEnum("71")]
        Item71,
    }
}