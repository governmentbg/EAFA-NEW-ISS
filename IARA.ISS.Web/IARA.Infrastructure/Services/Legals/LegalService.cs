using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces.Legals;

namespace IARA.Infrastructure.Services.Legals
{
    public class LegalService : Service, ILegalService
    {
        public LegalService(IARADbContext db)
               : base(db)
        { }

        public List<NomenclatureDTO> GetActiveLegals()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from legal in Db.Legals
                                            where legal.ValidFrom <= now
                                                && legal.ValidTo > now
                                            orderby legal.Name
                                            select new NomenclatureDTO
                                            {
                                                Value = legal.Id,
                                                DisplayName = $"{legal.Name} – {legal.Eik}"
                                            }).ToList();

            return result;
        }

        public List<AddressRegistrationDTO> GetAddressRegistrations(int legalId)
        {
            Dictionary<int, string> addressTypes = Db.GetAddressTypesDictionary();

            List<AddressRegistrationDTO> result = (from legalAddress in Db.LegalsAddresses
                                                   join address in Db.Addresses on legalAddress.AddressId equals address.Id
                                                   where legalAddress.LegalId == legalId
                                                        && legalAddress.IsActive
                                                        && address.IsActive
                                                   select new AddressRegistrationDTO
                                                   {
                                                       AddressType = Enum.Parse<AddressTypesEnum>(addressTypes[legalAddress.AddressTypeId]),
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
                                                   }).ToList();
            return result;
        }

        public ILookup<int, AddressRegistrationDTO> GetAddressRegistrations(List<int> legalIds)
        {
            Dictionary<int, string> addressTypes = Db.GetAddressTypesDictionary();

            ILookup<int, AddressRegistrationDTO> result = (from legalAddress in Db.LegalsAddresses
                                                           join address in Db.Addresses on legalAddress.AddressId equals address.Id
                                                           where legalIds.Contains(legalAddress.LegalId)
                                                                && legalAddress.IsActive
                                                                && address.IsActive
                                                           select new
                                                           {
                                                               LegalID = legalAddress.LegalId,
                                                               Address = new AddressRegistrationDTO
                                                               {
                                                                   AddressType = Enum.Parse<AddressTypesEnum>(addressTypes[legalAddress.AddressTypeId]),
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
                                                               }
                                                           }).ToLookup(x => x.LegalID, y => y.Address);
            return result;
        }

        public string GetLegalPhoneNumber(int legalId)
        {
            string result = (from legalPhone in Db.LegalPhoneNumbers
                             join phone in Db.PhoneNumbers on legalPhone.PhoneId equals phone.Id
                             where legalPhone.LegalId == legalId
                                && legalPhone.IsActive
                                && phone.IsActive
                             select phone.Phone).SingleOrDefault();
            return result;
        }

        public Dictionary<int, string> GetLegalPhoneNumbers(List<int> legalsIds)
        {
            Dictionary<int, string> result = (from legalPhone in Db.LegalPhoneNumbers
                                              join phone in Db.PhoneNumbers on legalPhone.PhoneId equals phone.Id
                                              where legalsIds.Contains(legalPhone.LegalId)
                                                    && legalPhone.IsActive
                                                    && phone.IsActive
                                              select new
                                              {
                                                  legalPhone.LegalId,
                                                  phone.Phone
                                              }).ToDictionary(x => x.LegalId, y => y.Phone);
            return result;
        }

        public string GetLegalEmail(int legalId)
        {
            string result = (from legalEmail in Db.LegalEmailAddresses
                             join email in Db.EmailAddresses on legalEmail.EmailAddressId equals email.Id
                             where legalEmail.LegalId == legalId
                                && legalEmail.IsActive
                                && email.IsActive
                             select email.Email).SingleOrDefault();
            return result;
        }

        public Dictionary<int, string> GetLegalEmails(List<int> legalIds)
        {
            Dictionary<int, string> result = (from legalEmail in Db.LegalEmailAddresses
                                              join email in Db.EmailAddresses on legalEmail.EmailAddressId equals email.Id
                                              where legalIds.Contains(legalEmail.LegalId)
                                                 && legalEmail.IsActive
                                                 && email.IsActive
                                              select new
                                              {
                                                  legalEmail.LegalId,
                                                  email.Email
                                              }).ToDictionary(x => x.LegalId, y => y.Email);
            return result;
        }

        public RegixLegalDataDTO GetRegixLegalData(int legalId)
        {
            string phone = GetLegalPhoneNumber(legalId);
            string email = GetLegalEmail(legalId);

            RegixLegalDataDTO regixData = (from legal in Db.Legals
                                           join owner in Db.Persons on legal.LegalOwnerId equals owner.Id into lw
                                           from owner in lw.DefaultIfEmpty()
                                           where legal.Id == legalId
                                           select new RegixLegalDataDTO
                                           {
                                               Id = legal.Id,
                                               EIK = legal.Eik,
                                               Name = legal.Name,
                                               Phone = phone,
                                               Email = email,
                                               IsCustodianOfPropertySameAsApplicant = legal.IsLegalOwnerSameAsApplicant,
                                               CustodianOfProperty = owner != null
                                                   ? new CustodianOfPropertyDTO
                                                   {
                                                       EgnLnc = new EgnLncDTO
                                                       {
                                                           EgnLnc = owner.EgnLnc,
                                                           IdentifierType = Enum.Parse<IdentifierTypeEnum>(owner.IdentifierType)
                                                       },
                                                       FirstName = owner.FirstName,
                                                       MiddleName = owner.MiddleName,
                                                       LastName = owner.LastName
                                                   }
                                                   : null
                                           }).First();

            return regixData;
        }

        public Dictionary<int, RegixLegalDataDTO> GetRegixLegalsData(List<int> legalIds)
        {
            DateTime now = DateTime.Now;

            Dictionary<int, string> phones = GetLegalPhoneNumbers(legalIds);
            Dictionary<int, string> emails = GetLegalEmails(legalIds);

            Dictionary<int, RegixLegalDataDTO> regixData = (from legal in Db.Legals
                                                            join owner in Db.Persons on legal.LegalOwnerId equals owner.Id into lw
                                                            from owner in lw.DefaultIfEmpty()
                                                            where legalIds.Contains(legal.Id)
                                                            select new
                                                            {
                                                                LegalID = legal.Id,
                                                                RegixData = new RegixLegalDataDTO
                                                                {
                                                                    Id = legal.Id,
                                                                    EIK = legal.Eik,
                                                                    Name = legal.Name,
                                                                    IsCustodianOfPropertySameAsApplicant = legal.IsLegalOwnerSameAsApplicant,
                                                                    CustodianOfProperty = owner != null
                                                                        ? new CustodianOfPropertyDTO
                                                                        {
                                                                            EgnLnc = new EgnLncDTO
                                                                            {
                                                                                EgnLnc = owner.EgnLnc,
                                                                                IdentifierType = Enum.Parse<IdentifierTypeEnum>(owner.IdentifierType)
                                                                            },
                                                                            FirstName = owner.FirstName,
                                                                            MiddleName = owner.MiddleName,
                                                                            LastName = owner.LastName
                                                                        }
                                                                        : null
                                                                }
                                                            }).ToDictionary(x => x.LegalID, y => y.RegixData);

            foreach (KeyValuePair<int, RegixLegalDataDTO> pair in regixData)
            {
                if (phones.TryGetValue(pair.Key, out string phone))
                {
                    pair.Value.Phone = phone;
                }
                if (emails.TryGetValue(pair.Key, out string email))
                {
                    pair.Value.Email = email;
                }
            }
            return regixData;
        }

        public string GetLegalPostalCode(int legalId)
        {
            return (from legalAddress in Db.LegalsAddresses
                    join address in Db.Addresses on legalAddress.AddressId equals address.Id
                    where legalAddress.LegalId == legalId
                                && legalAddress.IsActive
                                && address.IsActive
                    select address.PostCode).SingleOrDefault();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.Legals, id);
        }
    }
}
