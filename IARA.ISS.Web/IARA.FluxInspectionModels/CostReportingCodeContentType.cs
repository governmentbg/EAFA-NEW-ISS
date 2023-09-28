namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:CostReportingCode:D10B")]
    [XmlRoot("CostReportingCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:CostReportingCode:D10B", IsNullable = false)]
    public enum CostReportingCodeContentType
    {

        COM,
        DPO,
        MR,
        OVR,
        UB,
        DEL,
        DIR,
        DML,
        DRS,
        DTE,
        DTL,
        ENO,
        ENT,
        FEE,
        FNG,
        GA,
        LAB,
        MAT,
        MOO,
        MOT,
        PE,
        PP,
        QCL,
        RM,
        TM,
        TOT,
        ZZZ,
    }
}