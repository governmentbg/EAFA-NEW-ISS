using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class CapacityCertificateHistoryDTO
    {
        public int CertificateId { get; set; }

        public CapacityCertificateHistoryApplDTO CreatedFromApplication { get; set; }

        public CapacityCertificateHistoryApplDTO UsedInApplication { get; set; }

        public List<CapacityCertificateHistoryTransferredToDTO> TransferredTo { get; set; }

        public List<CapacityCertificateHistoryTransferredToDTO> RemainderTransferredTo { get; set; }
    }
}
