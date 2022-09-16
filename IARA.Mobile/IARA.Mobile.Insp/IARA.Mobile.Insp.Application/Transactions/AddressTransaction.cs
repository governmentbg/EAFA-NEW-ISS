using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Application.Interfaces.Database;
using IARA.Mobile.Insp.Application.Transactions.Base;
using System.Collections.Generic;
using System.Linq;

namespace IARA.Mobile.Insp.Application.Transactions
{
    public class AddressTransaction : BaseTransaction, IAddressTransaction
    {
        public AddressTransaction(BaseTransactionProvider provider)
            : base(provider) { }

        public List<SelectNomenclatureDto> GetCountries()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NCountries
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
                        Code = f.Code
                    })
                    .ToList();
            }
        }

        public List<SelectNomenclatureDto> GetMuncipalities(int districtId)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from municipality in context.NMunicipalities
                    where municipality.DistrictId == districtId
                    orderby municipality.Id
                    select new SelectNomenclatureDto
                    {
                        Id = municipality.Id,
                        Code = municipality.Code,
                        Name = municipality.Name,
                    }
                ).ToList();
            }
        }

        public List<SelectNomenclatureDto> GetPopulatedAreas(int municipalityId)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from populatedArea in context.NPopulatedAreas
                    where populatedArea.MunicipalityId == municipalityId
                    orderby populatedArea.Id
                    select new SelectNomenclatureDto
                    {
                        Id = populatedArea.Id,
                        Code = populatedArea.Code,
                        Name = populatedArea.Name,
                    }
                ).ToList();
            }
        }
    }
}
