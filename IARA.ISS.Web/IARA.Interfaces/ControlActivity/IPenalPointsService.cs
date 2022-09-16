using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.ControlActivity.PenalPoints;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces.ControlActivity
{
    public interface IPenalPointsService : IService
    {
        IQueryable<PenalPointsDTO> GetAllPenalPoints(PenalPointsFilters filters);

        PenalPointsEditDTO GetPenalPoints(int id);

        int AddPenalPoints(PenalPointsEditDTO points);

        void EditPenalPoints(PenalPointsEditDTO points);

        void DeletePenalPoints(int id);

        void UndoDeletePenalPoints(int id);

        PenalPointsAuanDecreeDataDTO GetPenalPointsAuanDecreeData(int decreeId);

        public List<PenalPointsOrderDTO> GetPermitOrders(int ownerId, bool isFisher, bool isPermitOwnerPerson);

        SimpleAuditDTO GetPenalPointsStatusSimpleAudit(int id);
    }
}
