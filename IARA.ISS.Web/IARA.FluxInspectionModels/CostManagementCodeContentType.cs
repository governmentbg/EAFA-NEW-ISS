namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:CostManagementCode:D10B")]
    [XmlRoot("CostManagementCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:CostManagementCode:D10B", IsNullable = false)]
    public enum CostManagementCodeContentType
    {

        ABC,
        TBR,
        ACP,
        AE,
        AF,
        BCP,
        BCS,
        CA,
        DAW,
        EST,
        FC,
        FOB,
        NAF,
        NDW,
        NRC,
        OC,
        OCC,
        OCN,
        OF,
        REC,
        SA,
        TC,
        TOT,
        VCA,
        VSA,
        ZZZ,
    }
}