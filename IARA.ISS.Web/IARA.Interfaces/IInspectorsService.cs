using System.Linq;
using IARA.DomainModels.DTOModels.Inspectors;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IInspectorsService : IService
    {
        IQueryable<InspectorsRegisterDTO> GetAll(InspectorsFilters filters, bool isRegistered);
        int AddInspector(InspectorsRegisterEditDTO inspector);
        int AddUnregisteredInspector(UnregisteredPersonEditDTO inspector);
        void EditInspector(InspectorsRegisterEditDTO inspectorRegister);
        void EditUnregisteredInspector(UnregisteredPersonEditDTO unregisteredInspector);
        void DeleteInspector(int id);
        void UndoDeleteInspector(int id);
    }
}
