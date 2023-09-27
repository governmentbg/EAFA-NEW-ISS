namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ScheduleTaskRelationshipType:D10B")]
    [XmlRoot("ScheduleTaskRelationshipType", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ScheduleTaskRelationshipType:D10B", IsNullable = false)]
    public enum ScheduleTaskRelationshipTypeContentType
    {

        F2F,
        F2S,
        S2F,
        S2S,
    }
}