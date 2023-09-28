namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:PartyType:D11A")]
    [XmlRoot("PartyType", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:PartyType:D11A", IsNullable = false)]
    public enum PartyTypeContentType
    {

        BRA,
        DEP,
        DIR,
        SEC,
        SER,
    }
}