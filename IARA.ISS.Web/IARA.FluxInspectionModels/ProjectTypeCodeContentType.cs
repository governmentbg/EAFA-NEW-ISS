namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ProjectTypeCode:D10B")]
    [XmlRoot("ProjectTypeCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ProjectTypeCode:D10B", IsNullable = false)]
    public enum ProjectTypeCodeContentType
    {

        GCN,
        DEP,
        FRP,
        LRP,
        OS,
        PRC,
        PRD,
        RDT,
        SDD,
        ZZZ,
    }
}