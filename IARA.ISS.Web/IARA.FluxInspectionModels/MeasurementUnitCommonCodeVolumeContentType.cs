namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:MeasurementUnitCommonCodeVolume:4")]
    [XmlRoot("MeasurementUnitCommonCodeVolume", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:MeasurementUnitCommonCodeVolume:4", IsNullable = false)]
    public enum MeasurementUnitCommonCodeVolumeContentType
    {

        CMQ,
        FTQ,
        MMQ,
        MTQ,
    }
}