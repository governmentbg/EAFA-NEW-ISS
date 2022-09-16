using System.IO;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.RecreationalFishing;

namespace IARA.Interfaces.Reports
{
    public interface IJasperReportExecutionService
    {
        Task<byte[]> GetBuyersApplication(int applicationId);
        Task<byte[]> GetBuyerChangeApplication(int applicationId);
        Task<byte[]> GetBuyerDeregistrationApplication(int applicationId);
        Task<byte[]> GetFirstSaleCenterChangeApplication(int applicationId);
        Task<byte[]> GetFirstSaleCenterDeregistrationApplication(int applicationId);
        Task<byte[]> GetCapacityApplicationReduce(int id);
        Task<byte[]> GetCapacityApplicationTransfer(int id);
        Task<byte[]> GetCapacityApplicationIncrease(int id);
        Task<byte[]> GetCapacityApplicationDuplicate(int id);
        Task<byte[]> GetCommercialFishingApplication(int id);
        Task<byte[]> GetFishermanApplication(int id);
        Task<byte[]> GetFishingVesselsApplication(int id);
        Task<byte[]> GetFishingVesselsChangeApplication(int id);
        Task<byte[]> GetFishingVesselsDestroyApplication(int id);
        Task<byte[]> GetLegalApplication(int id);
        Task<byte[]> GetSalesCentersApplication(int id);
        Task<byte[]> GetScientificFishingApplication(int id);
        Task<byte[]> GetAquacultureFacilityApplication(int applicationId);
        Task<byte[]> GetAquacultureFacilityDeregApplication(int applicationId);
        Task<byte[]> GetAquacultureFacilityChangeApplication(int applicationId);
        Task<byte[]> GetStatisticalFormAquacultureApplication(int applicationId);
        Task<byte[]> GetStatisticalFormFishingVesselApplication(int applicationId);
        Task<byte[]> GetStatisticalFormReworkApplication(int applicationId);
        Task<byte[]> GetScientificFishingPermitRegister(int id);
        Task<byte[]> GetScientificFishingPermitProject(int id);
        Task<byte[]> GetScientificFishingPermitGovRegister(int id);
        Task<byte[]> GetScientificFishingPermitGovProject(int id);
        Task<byte[]> GetFishermanRegister(int id, bool duplicate = false);
        Task<byte[]> GetFirstSaleBuyerRegister(int id, bool duplicate = false);
        Task<byte[]> GetFirstSaleCenterRegister(int id, bool duplicate = false);
        Task<byte[]> GetAquacultureRegister(int id);
        Task<byte[]> GetShipCapacityLicenseRegister(int id);
        Task<byte[]> GetDanubePermitRegister(int id, bool duplicate = false);
        Task<byte[]> GetBlackSeaPermitRegister(int id, bool duplicate = false);
        Task<byte[]> GetPoundNetPermitRegister(int id, bool duplicate = false);
        Task<byte[]> GetBlackSeaThirdCountryPermitRegister(int id, bool duplicate = false);
        Task<byte[]> GetDanubeThirdCountryPermitRegister(int id, bool duplicate = false);
        Task<byte[]> GetPermitLicenseRegister(int id, bool duplicate = false);
        Task<byte[]> GetPoundNetPermitLicenseRegister(int id, bool duplicate = false);
        Task<byte[]> GetQuotaSpeciesPermitLicenseRegister(int id, bool duplicate = false);
        Task<Stream> GetFishingTicket(int ticketId);
        Task<Stream> GetFishingTicketDeclaration(RecreationalFishingTicketDeclarationParametersDTO parameters);
        Task<byte[]> GetInspectionReport(int id, InspectionTypesEnum inspectionType);
        Task<byte[]> GetAuanRegister(int id);
        Task<byte[]> GetPenalDecreesRegister(int id);
        Task<byte[]> GetPenalDecreesAgreement(int id);
        Task<byte[]> GetPenalDecreesWarning(int id);
    }
}
