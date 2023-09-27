namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ResourceCostCategory:D10B")]
    [XmlRoot("ResourceCostCategory", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ResourceCostCategory:D10B", IsNullable = false)]
    public enum ResourceCostCategoryContentType
    {

        LAB,
        MAT,
        ODC,
        SBC,
    }
}