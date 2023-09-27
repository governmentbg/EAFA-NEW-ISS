namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ContractorType:D10B")]
    [XmlRoot("ContractorType", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ContractorType:D10B", IsNullable = false)]
    public enum ContractorTypeContentType
    {

        DRS,
        PAC,
    }
}