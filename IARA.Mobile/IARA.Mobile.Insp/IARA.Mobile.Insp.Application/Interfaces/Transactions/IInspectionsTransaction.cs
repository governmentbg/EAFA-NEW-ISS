using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Filters;
using IARA.Mobile.Insp.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IARA.Mobile.Insp.Application.Interfaces.Transactions
{
    public interface IInspectionsTransaction
    {
        Task<List<InspectionCatchMeasureDto>> GetCatchesFromLogBookPaheNumber(int logBookId, string pageNum);
        Task<string> GetNextReportNumber(int userId = -1);

        Task<FileResponse> GetFile(int fileId);

        InspectorInfoDto GetInspectorInfo(int id);

        InspectorDuringInspectionDto GetInspector(int id);

        InspectorDuringInspectionDto GetInspectorByUserId(int id);

        List<InspectorDuringInspectionDto> GetInspectorHistory(int id);

        List<RecentInspectorDto> GetRecentInspectors();

        Task<List<InspectionDto>> GetAll(int page, InspectionsFilters filters = null, bool reset = false);

        int GetPageCount(InspectionsFilters filters = null);

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

        Task<PostEnum> HandleInspection<TDto>(TDto dto, SubmitType submitType, List<FileModel> signatures, bool fromOffline = false)
            where TDto : InspectionEditDto;

        Task<bool> SignInspection(int inspectionId, List<FileModel> files, int localInspectionId);

        List<FishingGearDto> GetFishingGearsForShip(int shipUid, int? permitId = null);

        List<FishingGearDto> GetFishingGearsForPoundNet(int poundNetId, int? permitId = null);

        Task<List<LogBookPageDto>> GetLogBookPages(List<int> logBookIds);

        Task<bool> DeleteInspection(int id);

        Task PostOfflineInspections();

        Task<List<DeclarationLogBookPageDto>> GetDeclarationLogBookPages(DeclarationLogBookType type, int shipUid);

        Task<PersonFullDataDto> GetPersonFullData(IdentifierTypeEnum identifierType, string identifier);

        Task<LegalFullDataDto> GetLegalFullData(string eik);
        InspectionDraftDto MapToDraftDto<TDto>(TDto dto) where TDto : InspectionEditDto;

        string GetInspectionJson(int id);
        void ReturnForEdit(int id);
        int GetInspectionStateId(InspectionState inspectionState);
        int GetUnsignedInspectionCount();
        Task<InspectedLogBookPageDataDto> GetInspectedLogBookPageData(DeclarationLogBookType type, int logBookPageId);
    }
}
