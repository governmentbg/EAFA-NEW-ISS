namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:MeasurementUnitCommonCode_FileSize:4")]
    [XmlRoot("MeasurementUnitCommonCodeFileSize", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:MeasurementUnitCommonCode_FileSize:4", IsNullable = false)]
    public enum MeasurementUnitCommonCodeFileSizeContentType
    {

        [XmlEnum("4L")]
        Item4L,
        E34,
        E35,
    }
}