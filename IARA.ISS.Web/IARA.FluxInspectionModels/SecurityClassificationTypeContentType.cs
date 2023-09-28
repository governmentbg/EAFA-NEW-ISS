namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:SecurityClassificationType:D10B")]
    [XmlRoot("SecurityClassificationType", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:SecurityClassificationType:D10B", IsNullable = false)]
    public enum SecurityClassificationTypeContentType
    {

        CS,
        GC,
        GNC,
        GS,
        GTS,
        UNC,
    }
}