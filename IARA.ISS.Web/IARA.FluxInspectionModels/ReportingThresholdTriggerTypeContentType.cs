namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ReportingThresholdTriggerType:D10B")]
    [XmlRoot("ReportingThresholdTriggerType", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:ReportingThresholdTriggerType:D10B", IsNullable = false)]
    public enum ReportingThresholdTriggerTypeContentType
    {

        BTH,
        PCT,
        VAL,
    }
}