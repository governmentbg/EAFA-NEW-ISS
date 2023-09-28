namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:DangerousGoodsPackingCode:D22A")]
    [XmlRoot("DangerousGoodsPackingCode", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:DangerousGoodsPackingCode:D22A", IsNullable = false)]
    public enum DangerousGoodsPackingCodeContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
        [XmlEnum("3")]
        Item3,
        [XmlEnum("4")]
        Item4,
    }
}