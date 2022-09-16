using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IARA.Mobile.Insp.Application.Interfaces.Transactions
{
    public interface IInspectionsTransaction
    {
        Task<FileResponse> GetFile(int fileId);

        InspectorDuringInspectionDto GetInspector(int id);

        InspectorDuringInspectionDto GetInspectorByUserId(int id);

        List<InspectorDuringInspectionDto> GetInspectorHistory(int id);

        List<RecentInspectorDto> GetRecentInspectors();

        Task<List<InspectionDto>> GetAll(int page);

        int GetPageCount();

        Task<ObservationAtSeaDto> GetOFS(int id, bool isLocal);

        Task<InspectionAtSeaDto> GetIBS(int id, bool isLocal);

        Task<InspectionTransboardingDto> GetIBP(int id, bool isLocal);

        Task<InspectionTransboardingDto> GetITB(int id, bool isLocal);

        Task<InspectionTransportVehicleDto> GetIVH(int id, bool isLocal);

        Task<InspectionFirstSaleDto> GetIFS(int id, bool isLocal);

        Task<InspectionAquacultureDto> GetIAQ(int id, bool isLocal);

        Task<InspectionFisherDto> GetIFP(int id, bool isLocal);

        Task<InspectionCheckWaterObjectDto> GetCWO(int id, bool isLocal);

        Task<InspectionCheckToolMarkDto> GetIGM(int id, bool isLocal);

        Task<InspectionConstativeProtocolDto> GetOTH(int id, bool isLocal);

        Task<PostEnum> HandleInspection<TDto>(TDto dto, SubmitType submitType, bool fromOffline = false)
            where TDto : InspectionEditDto;

        Task<bool> SignInspection(int inspectionId, List<FileModel> files);

        List<FishingGearDto> GetFishingGearsForShip(int shipUid, int? permitId = null);

        List<FishingGearDto> GetFishingGearsForPoundNet(int poundNetId, int? permitId = null);

        Task<List<LogBookPageDto>> GetLogBookPages(List<int> logBookIds);

        Task<bool> DeleteInspection(int id);

        Task PostOfflineInspections();

        Task<List<DeclarationLogBookPageDto>> GetDeclarationLogBookPages(DeclarationLogBookType type, int shipUid);
    }
}
