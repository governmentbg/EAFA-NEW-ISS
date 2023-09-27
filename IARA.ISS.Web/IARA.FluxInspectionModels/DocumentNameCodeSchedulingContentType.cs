namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:DocumentNameCode_Scheduling:D22A")]
    [XmlRoot("DocumentNameCodeScheduling", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:DocumentNameCode_Scheduling:D22A", IsNullable = false)]
    public enum DocumentNameCodeSchedulingContentType
    {

        [XmlEnum("240")]
        Item240,
        [XmlEnum("241")]
        Item241,
        [XmlEnum("242")]
        Item242,
        [XmlEnum("245")]
        Item245,
        [XmlEnum("288")]
        Item288,
        [XmlEnum("291")]
        Item291,
    }
}