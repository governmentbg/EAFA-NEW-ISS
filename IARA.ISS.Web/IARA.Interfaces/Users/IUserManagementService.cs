using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.UserManagement;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IUserManagementService : IService
    {
        IQueryable<UserDTO> GetAll(UserManagementFilters filters, bool IsInternalUsers);

        InternalUserDTO GetInternalUser(int id);

        ExternalUserDTO GetExternalUser(int id);

        List<MobileDeviceDTO> GetUserMobileDevices(int id);

        void AddInternalUser(InternalUserDTO user);

        void AddOrEditInternalUserMobileDevices(int userId, List<MobileDeviceDTO> devices);

        void EditExternalUser(ExternalUserDTO user);

        void EditInternalUser(InternalUserDTO user);

        void UndoDelete(int id);

        void Delete(int id);

        void SendChangePasswordEmail(int userId);

        void AddOrEditPublicUserDevice(int userId, bool isInspector, PublicMobileDeviceDTO device);

        void ChangeUserToInternal(int userId);
    }
}
