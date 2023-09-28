namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:MessageFunctionCode_Acknowledgement:D22A")]
    [XmlRoot("MessageFunctionCodeAcknowledgement", Namespace = "urn:un:unece:uncefact:codelist:standard:UNECE:MessageFunctionCode_Acknowledgement:D22A", IsNullable = false)]
    public enum MessageFunctionCodeAcknowledgementContentType
    {

        [XmlEnum("5")]
        Item5,
        [XmlEnum("6")]
        Item6,
        [XmlEnum("7")]
        Item7,
        [XmlEnum("9")]
        Item9,
        [XmlEnum("18")]
        Item18,
        [XmlEnum("31")]
        Item31,
        [XmlEnum("35")]
        Item35,
        [XmlEnum("43")]
        Item43,
    }
}