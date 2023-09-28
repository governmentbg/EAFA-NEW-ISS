namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:EarnedValueCalculationMethod:D10B")]
    [XmlRoot("EarnedValueCalculationMethod", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:EarnedValueCalculationMethod:D10B", IsNullable = false)]
    public enum EarnedValueCalculationMethodContentType
    {

        APF,
        LOE,
        MTN,
        P01,
        P10,
        P50,
        PLN,
        PTC,
        STD,
        UNT,
        ZZZ,
    }
}