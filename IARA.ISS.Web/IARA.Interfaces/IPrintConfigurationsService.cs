using IARA.DomainModels.DTOModels.PrintConfigurations;
using IARA.DomainModels.RequestModels;
using System.Linq;

namespace IARA.Interfaces
{
    public interface IPrintConfigurationsService : IService
    {
        IQueryable<PrintConfigurationDTO> GetAllPrintConfigurations(PrintConfigurationFilters filters);

        PrintConfigurationEditDTO GetPrintConfiguration(int id);

        PrintConfigurationEditDTO AddOrEditPringConfiguration(PrintConfigurationEditDTO model);

        void DeletePrintConfiguration(int id);

        void UndoDeletePrintConfiguration(int id);
    }
}
