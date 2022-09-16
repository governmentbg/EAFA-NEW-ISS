using System;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Buyers
{
    public class BuyerPremiseUsageDocumentDTO
    {
        public int? Id { get; set; }
        public int BuyerID { get; set; }
        public NomenclatureDTO DocType { get; set; }
        public string DocNum { get; set; }
        public string DocIssuer { get; set; }
        public DateTime DocValidFrom { get; set; }
        public DateTime? DocValidTo { get; set; }
        public bool IsUnlimited { get; set; }
        public int LandlordLegalId { get; set; }
        public string LandlordName { get; set; }
        public string LandlordEik { get; set; }
        public AddressRegistrationDTO LandlordAddress { get; set; }
        public string Comment { get; set; }
        public bool IsActive { get; set; }
    }
}
