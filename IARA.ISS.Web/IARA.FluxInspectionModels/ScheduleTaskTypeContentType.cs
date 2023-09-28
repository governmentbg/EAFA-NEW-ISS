namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ScheduleTaskType:D10B")]
    [XmlRoot("ScheduleTaskType", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ScheduleTaskType:D10B", IsNullable = false)]
    public enum ScheduleTaskTypeContentType
    {

        CM,
        AC,
        FA,
        HAM,
        MS,
        SA,
        SUM,
    }
}