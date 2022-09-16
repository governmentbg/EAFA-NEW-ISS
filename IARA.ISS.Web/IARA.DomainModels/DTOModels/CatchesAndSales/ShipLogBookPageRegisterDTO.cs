using System;
using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class ShipLogBookPageRegisterDTO
    {
        public int Id { get; set; }

        public int LogBookId { get; set; }

        public string PageNumber { get; set; }

        public bool IsLogBookFinished { get; set; }

        public DateTime? OutingStartDate { get; set; }

        public string FishingGear { get; set; }

        public string PortOfUnloading { get; set; }

        public string UnloadingInformation { get; set; }

        public LogBookPageStatusesEnum Status { get; set; }

        public string StatusName { get; set; } // For UI

        public bool HasOriginDeclaration { get; set; }

        public string CancellationReason { get; set; }

        public bool IsActive { get; set; }

        public List<AdmissionLogBookPageRegisterDTO> AdmissionDeclarations { get; set; }

        public List<TransportationLogBookPageRegisterDTO> TransportationDocuments { get; set; }

        public List<FirstSaleLogBookPageRegisterDTO> FirstSaleDocuments { get; set; }
    }
}
