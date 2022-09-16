using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces.ControlActivity
{
    public interface IAuanRegisterService : IService
    {
        IQueryable<AuanRegisterDTO> GetAllAuans(AuanRegisterFilters filters);

        AuanRegisterEditDTO GetAuan(int id);

        int AddAuan(AuanRegisterEditDTO auan);

        void EditAuan(AuanRegisterEditDTO auan);

        void DeleteAuan(int id);

        void UndoDeleteAuan(int id);

        AuanReportDataDTO GetAuanReportDataFromInspection(int inspectionId);

        List<NomenclatureDTO> GetAllDrafters();

        List<AuanWitnessDTO> GetWitnesses(int? auanId, int? deliveryId);

        Task<byte[]> DownloadAuan(int auanId);
    }
}
