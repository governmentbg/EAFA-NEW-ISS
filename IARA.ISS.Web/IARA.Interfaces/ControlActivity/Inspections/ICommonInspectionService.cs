using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IARA.Interfaces.ControlActivity.Inspections
{
    public interface ICommonInspectionService : IService
    {
        int AddRegisterEntry(InspectionDraftDTO itemDTO, InspectionTypesEnum inspectionType, int userId);
        void Delete(int inspectionId);
        Task<byte[]> DownloadInspection(int inspectionId);
        void EditRegisterEntry(InspectionDraftDTO itemDTO);
        IQueryable<InspectionDTO> GetAll(InspectionsFilters filter, int? userId);
        InspectionSubjectPersonnelDTO GetAquacultureOwner(int aquacultureId);
        List<NomenclatureDTO> GetAquacultures();
        List<InspectedBuyerNomenclatureDTO> GetBuyers();
        List<InspectionCheckTypeNomenclatureDTO> GetCheckTypesForInspectionType(InspectionTypesEnum inspectionTypeParam);
        List<DeclarationLogBookPageDTO> GetDeclarationLogBookPages(DeclarationLogBookTypeEnum type, int? shipId, int? aquacultureId);
        List<AuanRegisterDTO> GetInspectionAUANs(List<int> inspectionIds);
        InspectorDTO GetInspector(int id);
        InspectorDTO GetInspectorByUserId(int userId);
        IQueryable<NomenclatureDTO> GetInspectors();
        VesselDTO GetPatrolVehicle(int id);
        IQueryable<NomenclatureDTO> GetPatrolVehicles(bool isWaterVehicle);
        List<FishingGearDTO> GetPermittedFishingGears(int subjectId, InspectionSubjectEnum subjectType);
        List<InspectionPermitLicenseDTO> GetPoundNetPermitLicenses(int poundNetId);
        VesselDTO GetShip(int id);
        List<InspectionShipLogBookDTO> GetShipLogBooks(int shipId);
        List<InspectionPermitLicenseDTO> GetShipPermitLicenses(int shipId);
        List<InspectionPermitLicenseDTO> GetShipPermits(int shipId);
        List<InspectionShipSubjectNomenclatureDTO> GetShipPersonnel(int shipId);
        bool IsInspector(int userId);
        void SafeDelete(int inspectionId);
        void SignInspection(int inspectionId, List<FileInfoDTO> files);
        void Undelete(int inspectionId);
    }
}
