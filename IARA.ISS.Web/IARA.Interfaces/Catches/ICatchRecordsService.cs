using IARA.DomainModels.DTOModels.Mobile.CatchRecords;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface ICatchRecordsService : IService
    {
        MobileCatchRecordGroupDTO GetCatchRecords(CatchRecordPublicFilters filters, int userId, int pageNumber, int pageSize);

        int CreateCatchRecord(CatchRecordEditDTO edit, int userId);

        void UpdateCatchRecord(CatchRecordEditDTO edit, int userId);

        void DeleteCatchRecord(int id, int userId);

        bool HasAccessToFile(int fileId, int userId);
    }
}
