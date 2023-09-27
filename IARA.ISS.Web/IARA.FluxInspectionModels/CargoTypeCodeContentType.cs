namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:CargoTypeCode:1996Rev2Final")]
    [XmlRoot("CargoTypeCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:CargoTypeCode:1996Rev2Final", IsNullable = false)]
    public enum CargoTypeCodeContentType
    {

        [XmlEnum("0")]
        Item0,
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
        [XmlEnum("9")]
        Item9,
    }
}