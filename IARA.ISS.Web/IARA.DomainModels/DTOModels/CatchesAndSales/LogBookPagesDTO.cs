using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookPagesDTO
    {
        public List<ShipLogBookPageRegisterDTO> ShipPages { get; set; }

        public List<FirstSaleLogBookPageRegisterDTO> FirstSalePages { get; set; }

        public List<AdmissionLogBookPageRegisterDTO> AdmissionPages { get; set; }

        public List<TransportationLogBookPageRegisterDTO> TransportationPages { get; set; }

        public List<AquacultureLogBookPageRegisterDTO> AquaculturePages { get; set; }

        public List<FishInformationDTO> UnloadingInformation { get; set; }

        public List<FishInformationDTO> FirstSaleProductInformation { get; set; }

        public List<FishInformationDTO> AdmissionProductInformation { get; set; }

        public List<FishInformationDTO> TransportationProductInformation { get; set; }

        public List<FishInformationDTO> AquacultureProductInformation { get; set; }
    }
}
