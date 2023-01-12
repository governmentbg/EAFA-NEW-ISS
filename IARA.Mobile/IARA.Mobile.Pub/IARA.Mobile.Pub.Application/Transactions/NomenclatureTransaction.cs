using System.Collections.Generic;
using System.Linq;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Pub.Application.DTObjects.AddressNomenclatures.LocalDb;
using IARA.Mobile.Pub.Application.DTObjects.DocumentTypes.LocalDb;
using IARA.Mobile.Pub.Application.Interfaces.Database;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Transactions.Base;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class NomenclatureTransaction : BaseTransaction, INomenclatureTransaction
    {
        public NomenclatureTransaction(BaseTransactionProvider provider)
            : base(provider)
        {
        }

        public List<CountrySelectDto> GetCountries()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NCountries
                    .Where(x => x.IsActive)
                    .Select(f => new CountrySelectDto
                    {
                        Id = f.Id,
                        Code = f.Code,
                        Name = f.Name,
                    })
                    .ToList();
            }
        }

        public List<DistrictSelectDto> GetDistricts()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NDistricts
                    .Select(f => new DistrictSelectDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                    })
                    .ToList();
            }
        }

        public List<MunicipalitySelectDto> GetMuncipalitiesByDisctrict(int districtId)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NMunicipalities
                    .Where(x => x.DistrictId == districtId)
                    .Select(f => new MunicipalitySelectDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                        DistrictId = f.DistrictId
                    })
                    .ToList();
            }
        }

        public List<PopulatedAreaSelectDto> GetPopulatedAreasByMunicipality(int municipalityId)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NPopulatedAreas
                    .Where(x => x.MunicipalityId == municipalityId)
                    .Select(f => new PopulatedAreaSelectDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                        MunicipalityId = f.MunicipalityId
                    })
                    .ToList();
            }
        }

        public List<DocumentTypeSelectDto> GetDocumentTypes()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NDocumentTypes
                    .Select(f => new DocumentTypeSelectDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                    })
                    .ToList();
            }
        }

        public int GetDocumentTypeIdByCode(string documentTypeCode)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NDocumentTypes
                        .Where(x => x.Code == documentTypeCode && x.IsActive)
                        .Select(f => f.Id)
                        .First();
            }
        }

        public List<NomenclatureDto> GetPermitReasons()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from pr in context.NPermitReasons
                    select new NomenclatureDto
                    {
                        DisplayName = pr.Name,
                        IsActive = pr.IsActive,
                        Value = pr.Id
                    }
                ).ToList();
            }
        }

        public List<NomenclatureDto> GetFishTypes()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from ft in context.NFishes
                    where ft.IsActive
                    select new NomenclatureDto
                    {
                        DisplayName = ft.Name,
                        IsActive = ft.IsActive,
                        Value = ft.Id
                    }
                ).ToList();
            }
        }

        public List<NomenclatureDto> GetViolationSignalTypes()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from vst in context.NViolationSignalTypes
                    select new NomenclatureDto
                    {
                        DisplayName = vst.Name,
                        IsActive = vst.IsActive,
                        Value = vst.Id
                    }
                ).ToList();
            }
        }

        public int GetActiveFileTypeIdByCode(string fileType)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (from fType in context.NFileTypes
                        where fType.Code == fileType && fType.IsActive
                        select fType.Id).First();
            }
        }

        public List<int> GetAllFileTypeIdsByCode(string fileType)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (from fType in context.NFileTypes
                        where fType.Code == fileType
                        select fType.Id).ToList();
            }
        }

        public int GetActiveGenderId(string genderCode)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.Genders
                    .Where(x => x.Code == genderCode && x.IsActive)
                    .Select(f => f.Id).First();
            }
        }

        public string GetGenderCodeById(int genderId)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.Genders
                    .Where(x => x.Id == genderId)
                    .Select(f => f.Code).First();
            }
        }

        public List<NomenclatureDto> GetGenders(List<string> codes)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.Genders
                    .Where(x => codes.Contains(x.Code) && x.IsActive)
                    .Select(f => new NomenclatureDto { Code = f.Code, DisplayName = f.Name, Value = f.Id }).ToList();
            }
        }

        public List<NomenclatureDto> GetGenders()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.Genders
                    .Where(x => x.IsActive)
                    .Select(f => new NomenclatureDto { Code = f.Code, DisplayName = f.Name, Value = f.Id }).ToList();
            }
        }

        public string GetSystemParameter(string code)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.SystemParameters
                    .Where(x => x.Code == code && x.IsActive)
                    .Select(f => f.ParamValue).First();
            }
        }

        public List<NomenclatureDto> GetPaymentTypes(List<string> codes)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.PaymentTypes
                    .Where(x => codes.Contains(x.Code) && x.IsActive)
                    .OrderByDescending(x => x.Id)
                    .Select(f => new NomenclatureDto { Code = f.Code, DisplayName = f.Name, Value = f.Id })
                    .ToList();
            }
        }

        public List<string> GetPermissions()
        {
            if (!ContextBuilder.DatabaseExists)
            {
                return new List<string>();
            }

            try
            {
                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    return context.NPermissions
                        .OrderBy(f => f.Permission)
                        .Select(f => f.Permission)
                        .ToList();
                }
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
