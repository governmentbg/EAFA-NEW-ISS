namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:FundingTypeCode:D10B")]
    [XmlRoot("FundingTypeCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:FundingTypeCode:D10B", IsNullable = false)]
    public enum FundingTypeCodeContentType
    {

        INC,
        MYR,
        SYR,
    }
}