using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.Nomenclatures;
using System.Collections.Generic;

namespace IARA.Interfaces.CommercialFishing
{
    public interface ISuspensionsService
    {
        List<SuspensionDataDTO> GetPermitSuspensions(int permitId);

        List<SuspensionDataDTO> GetPermitLicenseSuspensions(int permitLicenseId);

        void AddEditPermitSuspensions(int permitId, List<SuspensionDataDTO> suspensions, int currentUserId, SuspensionPermissionsDTO permissions);
        
        void AddPermitSuspension(SuspensionDataDTO suspension, int permitId, int currentUserId);
        
        void AddEditPermitLicenseSuspensions(int permitLicenseId, List<SuspensionDataDTO> suspensions, int currentUserId, SuspensionPermissionsDTO permissions);
        
        void AddPermitLicenseSuspension(SuspensionDataDTO suspension, int permitLicenseId, int currentUserId);

        void UpdatePermitsIsSuspendedFlag();

        void UpdatePermitLicensesIsSuspendedFlag();

        void SuspendPermitLicensesAndLogBooksFromPreviousYears();

        SimpleAuditDTO GetPermitSuspensionSimpleAudit(int id);

        SimpleAuditDTO GetPermitLicenseSuspensionSimpleAudit(int id);
    }
}
