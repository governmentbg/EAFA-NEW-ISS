
using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Application;

namespace IARA.Interfaces.Applications
{
    public interface IChangeOfCircumstancesService
    {
        List<ChangeOfCircumstancesDTO> GetChangeOfCircumstances(int applicationId);

        void AddOrEditChangeOfCircumstances(int applicationId,
                                            List<ChangeOfCircumstancesDTO> changes,
                                            int? shipId = null,
                                            int? aquacultureFacilityId = null,
                                            int? buyerId = null,
                                            int? permitId = null,
                                            int? permitLicenseId = null);
    }
}
