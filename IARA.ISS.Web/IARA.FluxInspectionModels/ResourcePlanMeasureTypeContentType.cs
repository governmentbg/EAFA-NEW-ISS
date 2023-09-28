namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ResourcePlanMeasureType:D10B")]
    [XmlRoot("ResourcePlanMeasureType", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ResourcePlanMeasureType:D10B", IsNullable = false)]
    public enum ResourcePlanMeasureTypeContentType
    {

        DC,
        HR,
        UN,
    }
}