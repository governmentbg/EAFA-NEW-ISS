using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb;
using IARA.Mobile.Pub.Application.Filters;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IScientificFishingTransaction
    {
        Task<List<SFPermitDto>> GetAll(ScientificFishingFilters filters);

        SFPermitReviewDto Get(int id);

        List<SFOutingDto> GetOutings(int id);

        Task<AddEntityResultEnum> AddOuting(SFOutingDto dto);

        Task<UpdateEntityResultEnum> EditOuting(SFOutingDto dto);

        Task<DeleteEntityResultEnum> DeleteOuting(SFOutingDto dto);

        Task<bool> DownloadFile(FileModel file);
    }
}
