namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:OrganizationFunctionType:D11A")]
    [XmlRoot("OrganizationFunctionType", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:OrganizationFunctionType:D11A", IsNullable = false)]
    public enum OrganizationFunctionTypeContentType
    {

        [XmlEnum("5")]
        Item5,
        [XmlEnum("6")]
        Item6,
        AUD,
        CER,
        EXC,
        LAU,
    }
}