namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:PlanningLevel:D10B")]
    [XmlRoot("PlanningLevel", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:PlanningLevel:D10B", IsNullable = false)]
    public enum PlanningLevelContentType
    {

        ACT,
        CA,
        PP,
        SLP,
        WP,
        ZZZ,
    }
}