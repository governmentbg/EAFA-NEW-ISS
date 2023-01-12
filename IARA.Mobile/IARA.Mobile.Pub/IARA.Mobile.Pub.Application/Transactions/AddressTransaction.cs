using System.Collections.Generic;
using System.Linq;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Interfaces.Database;
using IARA.Mobile.Pub.Application.Transactions.Base;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class AddressTransaction : BaseTransaction, IAddressTransaction
    {
        public AddressTransaction(BaseTransactionProvider provider)
        : base(provider)
        {
        }
        public List<SelectNomenclatureDto> GetCountries()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NCountries
                    .Where(x => x.IsActive)
                    .Select(f => new SelectNomenclatureDto
                    {
                        Id = f.Id,
                        Code = f.Code,
                        Name = f.Name,
                    })
                    .ToList();
            }
        }

        public List<SelectNomenclatureDto> GetDistricts()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NDistricts
                    .Select(f => new SelectNomenclatureDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                    })
                    .ToList();
            }
        }

        public List<SelectNomenclatureDto> GetMuncipalities(int districtId)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NMunicipalities
                    .Where(x => x.DistrictId == districtId)
                    .Select(f => new SelectNomenclatureDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                    })
                    .ToList();
            }
        }

        public List<SelectNomenclatureDto> GetPopulatedAreas(int municipalityId)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NPopulatedAreas
                    .Where(x => x.MunicipalityId == municipalityId)
                    .Select(f => new SelectNomenclatureDto
                    {
                        Id = f.Id,
                        Name = f.Name
                    })
                    .ToList();
            }
        }
    }
}
