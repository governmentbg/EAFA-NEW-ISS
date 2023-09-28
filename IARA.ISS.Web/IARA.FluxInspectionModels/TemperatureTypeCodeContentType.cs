namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:TemperatureTypeCode:D22A")]
    [XmlRoot("TemperatureTypeCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:TemperatureTypeCode:D22A", IsNullable = false)]
    public enum TemperatureTypeCodeContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
        [XmlEnum("3")]
        Item3,
        [XmlEnum("4")]
        Item4,
        [XmlEnum("5")]
        Item5,
        [XmlEnum("6")]
        Item6,
        [XmlEnum("7")]
        Item7,
        [XmlEnum("8")]
        Item8,
        [XmlEnum("9")]
        Item9,
    }
}