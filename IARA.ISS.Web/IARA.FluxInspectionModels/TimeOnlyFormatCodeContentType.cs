namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:TimeOnlyFormatCode:D22A")]
    [XmlRoot("TimeOnlyFormatCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:TimeOnlyFormatCode:D22A", IsNullable = false)]
    public enum TimeOnlyFormatCodeContentType
    {

        [XmlEnum("209")]
        Item209,
        [XmlEnum("401")]
        Item401,
        [XmlEnum("402")]
        Item402,
        [XmlEnum("404")]
        Item404,
        [XmlEnum("406")]
        Item406,
    }
}