using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces.ControlActivity
{
    public interface IInspectionsService : IService
    {
        int AddRegisterEntry(InspectionDraftDTO item, InspectionTypesEnum inspectionType, int userId);
        void EditRegisterEntry(InspectionDraftDTO item, InspectionTypesEnum inspectionType);
        int SubmitReport(InspectionEditDTO itemDTO, InspectionTypesEnum inspectionType, int userId);
        InspectionEditDTO GetRegisterEntry(int id);
        IQueryable<InspectionDTO> GetAll(InspectionsFilters filter, int? userId);
        void SignInspection(int inspectionId, List<FileInfoDTO> files);
        Task<byte[]> DownloadInspection(int inspectionId);
        List<AuanRegisterDTO> GetInspectionAUANs(List<int> inspectionIds);
        IQueryable<NomenclatureDTO> GetInspectors();
        InspectorDTO GetInspector(int id);
        InspectorDTO GetInspectorByUserId(int userId);
        IQueryable<NomenclatureDTO> GetPatrolVehicles(bool isWaterVehicle);
        VesselDTO GetPatrolVehicle(int id);
        VesselDTO GetShip(int id);
        List<InspectionShipSubjectNomenclatureDTO> GetShipPersonnel(int shipId);
        List<FishingGearDTO> GetPermittedFishingGears(int subjectId, InspectionSubjectEnum subjectType);
        List<InspectionCheckTypeNomenclatureDTO> GetCheckTypesForInspectionType(InspectionTypesEnum inspectionType);
        List<InspectionPermitLicenseDTO> GetShipPermitLicenses(int shipId);
        List<InspectionPermitLicenseDTO> GetPoundNetPermitLicenses(int poundNetId);
        List<InspectionShipLogBookDTO> GetShipLogBooks(int shipId);
        List<InspectedBuyerNomenclatureDTO> GetBuyers();
        List<NomenclatureDTO> GetAquacultures();
        List<DeclarationLogBookPageDTO> GetDeclarationLogBookPages(DeclarationLogBookTypeEnum type, int shipId);
        InspectionSubjectPersonnelDTO GetAquacultureOwner(int aquacultureId);
        void Delete(int inspectionId);
        void Undelete(int inspectionId);
        void SafeDelete(int inspectionId);
    }
}
