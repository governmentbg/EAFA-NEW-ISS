namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AddressFormatType:D11A")]
    [XmlRoot("AddressFormatType", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AddressFormatType:D11A", IsNullable = false)]
    public enum AddressFormatTypeContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
        [XmlEnum("3")]
        Item3,
    }
}