namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccessRightsType:D11A")]
    [XmlRoot("AccessRightsType", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccessRightsType:D11A", IsNullable = false)]
    public enum AccessRightsTypeContentType
    {
        P,
        R,
        U,
    }
}