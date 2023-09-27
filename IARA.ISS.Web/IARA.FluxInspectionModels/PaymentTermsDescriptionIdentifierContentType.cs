namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:identifierlist:standard:UNECE:PaymentTermsDescriptionIdentifier:D22A")]
    [XmlRoot("PaymentTermsDescriptionIdentifier", Namespace = "urn:un:unece:uncefact:identifierlist:standard:UNECE:PaymentTermsDescriptionIdentifier:D22A", IsNullable = false)]
    public enum PaymentTermsDescriptionIdentifierContentType
    {

        [XmlEnum("1")]
        Item1,
        [XmlEnum("2")]
        Item2,
        [XmlEnum("3")]
        Item3,
        [XmlEnum("4")]
        Item4,
        [XmlEnum("5")]
        Item5,
        [XmlEnum("6")]
        Item6,
        [XmlEnum("7")]
        Item7,
    }
}