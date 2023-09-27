namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:SealConditionCode:D22A")]
    [XmlRoot("SealConditionCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:SealConditionCode:D22A", IsNullable = false)]
    public enum SealConditionCodeContentType
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
    }
}