namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:HierarchicalStructureTypeCode:D10B")]
    [XmlRoot("HierarchicalStructureTypeCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:HierarchicalStructureTypeCode:D10B", IsNullable = false)]
    public enum HierarchicalStructureTypeCodeContentType
    {

        IPT,
        IMP,
        IMS,
        MS,
        OBS,
        RBS,
        WBS,
        ZZZ,
    }
}