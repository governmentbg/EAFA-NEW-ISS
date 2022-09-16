using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.Applications;

namespace IARA.Interfaces
{
    public interface IQualifiedFishersService : IService, IRegisterDeliveryService
    {
        IQueryable<QualifiedFisherDTO> GetAll(QualifiedFishersFilters filters);

        QualifiedFisherEditDTO GetRegisterEntry(int id);

        int AddRegisterEntry(QualifiedFisherEditDTO newFisherDto);

        int EditRegisterEntry(QualifiedFisherEditDTO fisher);

        void Delete(int id);

        void UndoDelete(int id);

        QualifiedFisherApplicationEditDTO GetApplicationEntry(int id);

        RegixChecksWrapperDTO<QualifiedFisherRegixDataDTO> GetApplicationRegixData(int id);

        QualifiedFisherRegixDataDTO GetApplicationData(int applicationId);

        QualifiedFisherEditDTO GetEntryByApplicationId(int applicationId);

        QualifiedFisherEditDTO GetRegisterByApplicationId(int applicationId);

        int AddApplicationEntry(QualifiedFisherApplicationEditDTO newFisherApplicationDto, ApplicationStatusesEnum? nextManualStatus);

        int EditApplicationEntry(QualifiedFisherApplicationEditDTO application, ApplicationStatusesEnum? manualStatus = null);

        ApplicationStatusesEnum EditApplicationRegixData(QualifiedFisherRegixDataDTO regixData);

        Task<DownloadableFileDTO> GetRegisterFileForDownload(int registerId, bool duplicate = false);

        bool PersonIsQualifiedFisherCheck(EgnLncDTO personIdentifier, int? entryId);
    }
}
