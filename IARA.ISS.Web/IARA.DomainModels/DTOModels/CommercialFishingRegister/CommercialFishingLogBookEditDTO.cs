using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.CatchesAndSales;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class CommercialFishingLogBookEditDTO : LogBookEditDTO
    {
        public int? LogBookLicenseId { get; set; }

        public int? LastLogBookLicenseId { get; set; }

        public int? LastPermitLicenseId { get; set; }

        public long? PermitLicenseStartPageNumber { get; set; }

        public long? PermitLicenseEndPageNumber { get; set; }

        /// <summary>
        /// For UI purposes only
        /// </summary>
        public string PermitLicenseRegistrationNumber { get; set; }

        /// <summary>
        /// For UI purposes only
        /// </summary>
        public DateTime? LogBookLicenseValidForm { get; set; }

        /// <summary>
        /// For UI purposes only
        /// </summary>
        public DateTime? LogBookLicenseValidTo { get; set; }

        public long LastPageNumber { get; set; }

        public bool IsForRenewal { get; set; }

        public bool PermitLicenseIsActive { get; set; }

        public List<ShipLogBookPageRegisterDTO> ShipPagesAndDeclarations { get; set; }
    }
}
