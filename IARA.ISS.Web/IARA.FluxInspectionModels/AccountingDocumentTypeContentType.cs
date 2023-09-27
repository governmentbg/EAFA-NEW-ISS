namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingDocumentType:D11A")]
    [XmlRoot("AccountingDocumentType", Namespace = "urn:un:unece:uncefact:codelist:standard:EDIFICAS-EU:AccountingDocumentType:D11A", IsNullable = false)]
    public enum AccountingDocumentTypeContentType
    {

        BC,
        COA,
        DB,
        JL,
        JN,
        LG,
        RP,
        TR,
    }
}