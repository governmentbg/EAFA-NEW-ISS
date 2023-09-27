namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:ScenarioType:D11A")]
    [XmlRoot("ScenarioType", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:ScenarioType:D11A", IsNullable = false)]
    public enum ScenarioTypeContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
        [XmlEnum("3")]
        Item3,
    }
}