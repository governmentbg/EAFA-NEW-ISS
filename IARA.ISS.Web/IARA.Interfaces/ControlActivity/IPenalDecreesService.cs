using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces.ControlActivity
{
    public interface IPenalDecreesService : IService
    {
        IQueryable<PenalDecreeDTO> GetAllPenalDecrees(PenalDecreesFilters filters);

        PenalDecreeEditDTO GetPenalDecree(int id);

        int AddPenalDecree(PenalDecreeEditDTO decree);

        void EditPenalDecree(PenalDecreeEditDTO decree);

        void DeletePenalDecree(int id);

        void UndoDeletePenalDecree(int id);

        PenalDecreeAuanDataDTO GetPenalDecreeAuanData(int auanId);

        SimpleAuditDTO GetPenalDecreeStatusSimpleAudit(int id);

        Task<DownloadableFileDTO> GetRegisterFileForDownload(int decreeId);
    }
}
