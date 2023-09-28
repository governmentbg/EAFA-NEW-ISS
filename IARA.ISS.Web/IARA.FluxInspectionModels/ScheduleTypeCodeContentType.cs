namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ScheduleTypeCode:D10B")]
    [XmlRoot("ScheduleTypeCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ScheduleTypeCode:D10B", IsNullable = false)]
    public enum ScheduleTypeCodeContentType
    {

        LOB,
        MFG,
        MSL,
        NS,
        RNS,
        TL,
        ZZZ,
    }
}