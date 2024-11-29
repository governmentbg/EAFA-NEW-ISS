using IARA.FVMSModels.Common;
using IARA.Interfaces.FVMSIntegrations;

namespace IARA.Infrastructure.FVMSIntegrations
{
    public class FVMSNomenclatureDataService : BaseService, IFVMSNomenclatureDataService
    {
        public FVMSNomenclatureDataService(IARADbContext dbContext)
            : base(dbContext)
        { }

        public List<FvmsFish> GetFishTypes()
        {
            DateTime now = DateTime.Now;

            List<FvmsFish> result = (from fish in Db.Nfishes
                                     where fish.ValidFrom <= now
                                        && fish.ValidTo > now
                                        && fish.IsBlackSea
                                     select new FvmsFish
                                     {
                                         Name = fish.Name,
                                         NameLatin = fish.NameLatin,
                                         Code = fish.Code
                                     }).ToList();

            return result;
        }

        public List<FvmsInspector> GetInspectors()
        {
            List<FvmsInspector> result = (from inspector in Db.Inspectors
                                          join user in Db.Users on inspector.UserId equals user.Id
                                          join person in Db.Persons on user.PersonId equals person.Id
                                          where inspector.IsActive
                                          select new FvmsInspector
                                          {
                                              FirstName = person.FirstName,
                                              LastName = person.LastName,
                                              CardNumber = inspector.InspectorCardNum
                                          }).ToList();

            return result;
        }
    }
}
