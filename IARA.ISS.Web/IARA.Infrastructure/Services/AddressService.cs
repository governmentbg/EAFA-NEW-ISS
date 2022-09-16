using System;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class AddressService : Service, IAddressService
    {
        public AddressService(IARADbContext db)
              : base(db)
        {
        }

        public AddressRegistrationDTO GetAddressRegistration(int id)
        {
            AddressRegistrationDTO result = (from address in Db.Addresses
                                             where address.Id == id
                                             select new AddressRegistrationDTO
                                             {
                                                 CountryId = address.CountryId.GetValueOrDefault(),
                                                 DistrictId = address.DistrictId,
                                                 MunicipalityId = address.MunicipalityId,
                                                 PopulatedAreaId = address.PopulatedAreaId,
                                                 Region = address.Region,
                                                 PostalCode = address.PostCode,
                                                 Street = address.Street,
                                                 StreetNum = address.StreetNum,
                                                 BlockNum = address.BlockNum,
                                                 EntranceNum = address.EntranceNum,
                                                 FloorNum = address.FloorNum,
                                                 ApartmentNum = address.ApartmentNum
                                             }).First();
            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
