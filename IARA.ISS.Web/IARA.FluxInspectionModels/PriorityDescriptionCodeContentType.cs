namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:PriorityDescriptionCode:D22A")]
    [XmlRoot("PriorityDescriptionCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:PriorityDescriptionCode:D22A", IsNullable = false)]
    public enum PriorityDescriptionCodeContentType
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
    }
}