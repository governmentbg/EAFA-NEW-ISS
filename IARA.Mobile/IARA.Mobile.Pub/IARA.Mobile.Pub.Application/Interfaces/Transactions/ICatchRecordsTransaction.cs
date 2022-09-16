using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.CatchRecords;
using IARA.Mobile.Pub.Application.Filters;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface ICatchRecordsTransaction
    {
        Task<List<CatchRecordDto>> GetCatchRecords(CatchRecordPublicFilters filters);

        CatchRecordInfoDto GetCatchRecord(int id);

        Task CreateCatchRecord(CreateCatchRecordDto dto);

        Task EditCatchRecord(CreateCatchRecordDto dto);

        Task DeleteCatchRecord(int id);

        Task<FileResponse> GetPhoto(int photoId);

        Task<FileResponse> GetGalleryPhoto(int photoId);

        Task PostOfflineCatchRecords();
    }
}
