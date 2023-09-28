namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:DateOnlyFormatCode:D22A")]
    [XmlRoot("DateOnlyFormatCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:DateOnlyFormatCode:D22A", IsNullable = false)]
    public enum DateOnlyFormatCodeContentType
    {

        [XmlEnum("2")]
        Item2,
        [XmlEnum("3")]
        Item3,
        [XmlEnum("4")]
        Item4,
        [XmlEnum("101")]
        Item101,
        [XmlEnum("102")]
        Item102,
        [XmlEnum("105")]
        Item105,
        [XmlEnum("106")]
        Item106,
        [XmlEnum("107")]
        Item107,
        [XmlEnum("110")]
        Item110,
        [XmlEnum("609")]
        Item609,
    }
}