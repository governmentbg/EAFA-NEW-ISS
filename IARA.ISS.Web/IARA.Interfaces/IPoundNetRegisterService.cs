using System.Linq;
using IARA.DomainModels.DTOModels;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IPoundNetRegisterService : IService
    {
        public IQueryable<PoundNetDTO> GetAll(PoundNetRegisterFilters filters);

        public PoundnetRegisterDTO Get(int id);

        int Add(PoundnetRegisterDTO poundnet);

        void Edit(PoundnetRegisterDTO poundnet);

        void UndoDelete(int id);

        void Delete(int id);
    }
}
