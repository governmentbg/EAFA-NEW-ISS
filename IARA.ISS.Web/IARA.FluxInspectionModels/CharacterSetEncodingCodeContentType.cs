namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:6:0133:40106")]
    [XmlRoot("CharacterSetEncodingCode", Namespace = "urn:un:unece:uncefact:codelist:standard:6:0133:40106", IsNullable = false)]
    public enum CharacterSetEncodingCodeContentType
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
        ZZZ,
    }
}