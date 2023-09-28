namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:DangerousGoodsRegulationCode:D22A")]
    [XmlRoot("DangerousGoodsRegulationCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:DangerousGoodsRegulationCode:D22A", IsNullable = false)]
    public enum DangerousGoodsRegulationCodeContentType
    {

        ADR,
        ADS,
        ADT,
        ADU,
        ADV,
        ADW,
        ADX,
        ADY,
        ADZ,
        AEA,
        AEB,
        AGS,
        ANR,
        ARD,
        CFR,
        COM,
        GVE,
        GVS,
        ICA,
        IMD,
        RGE,
        RID,
        UI,
        ZZZ,
    }
}