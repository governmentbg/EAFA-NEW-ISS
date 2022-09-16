using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IARA.DomainModels.DTOModels.Vehicles;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IPatrolVehiclesService : IService
    {
        IQueryable<PatrolVehiclesDTO> GetAll(PatrolVehiclesFilters filters);
        IQueryable<PatrolVehiclesDTO> GetAllPatrolVehicles(bool showInactiveRecords);
        IQueryable<PatrolVehiclesDTO> GetFreeTextPatrolVehicles(string freeTextSearch, bool showInactiveRecords);
        IQueryable<PatrolVehiclesDTO> GetParameterFilteredPatrolVehicles(PatrolVehiclesFilters filters);
        void DeletePatrolVehicle(int id);
        void UndoDeletePatrolVehicle(int id);
        int AddPatrolVehicle(PatrolVehiclesEditDTO patrolVehicle);
        void EditPatrolVehicle(PatrolVehiclesEditDTO patrolVehicle);
        PatrolVehiclesDTO GetPatrolVehicle(int id);
    }
}
