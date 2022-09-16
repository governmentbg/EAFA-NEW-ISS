using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class PersonService : Service, IPersonService
    {
        public PersonService(IARADbContext db)
              : base(db)
        {
        }

        public PersonDocumentDTO GetPersonDocument(int personId)
        {
            PersonDocumentDTO result = (from personDocument in Db.PersonDocuments
                                        where personDocument.PersonId == personId
                                            && personDocument.IsActive
                                        select new PersonDocumentDTO
                                        {
                                            DocumentTypeID = personDocument.DocumentTypeId,
                                            DocumentNumber = personDocument.DocumentNumber,
                                            DocumentIssuedOn = personDocument.DocumentIssueDate,
                                            DocumentIssuedBy = personDocument.DocumentPublisher
                                        }).SingleOrDefault();
            return result;
        }

        public Dictionary<int, PersonDocumentDTO> GetPersonDocuments(List<int> personIds)
        {
            Dictionary<int, PersonDocumentDTO> result = (from personDocument in Db.PersonDocuments
                                                         where personIds.Contains(personDocument.PersonId)
                                                            && personDocument.IsActive
                                                         select new
                                                         {
                                                             personDocument.PersonId,
                                                             Document = new PersonDocumentDTO
                                                             {
                                                                 DocumentTypeID = personDocument.DocumentTypeId,
                                                                 DocumentNumber = personDocument.DocumentNumber,
                                                                 DocumentIssuedOn = personDocument.DocumentIssueDate,
                                                                 DocumentIssuedBy = personDocument.DocumentPublisher
                                                             }
                                                         }).ToDictionary(x => x.PersonId, y => y.Document);
            return result;
        }

        public List<AddressRegistrationDTO> GetAddressRegistrations(int personId)
        {
            Dictionary<int, string> addressTypes = Db.GetAddressTypesDictionary();

            List<AddressRegistrationDTO> result = (from personAddress in Db.PersonAddresses
                                                   join address in Db.Addresses on personAddress.AddressId equals address.Id
                                                   where personAddress.PersonId == personId
                                                        && personAddress.IsActive
                                                        && address.IsActive
                                                   select new AddressRegistrationDTO
                                                   {
                                                       AddressType = Enum.Parse<AddressTypesEnum>(addressTypes[personAddress.AddressTypeId]),
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

        public ILookup<int, AddressRegistrationDTO> GetAddressRegistrations(List<int> personIds)
        {
            Dictionary<int, string> addressTypes = Db.GetAddressTypesDictionary();

            ILookup<int, AddressRegistrationDTO> result = (from personAddress in Db.PersonAddresses
                                                           join address in Db.Addresses on personAddress.AddressId equals address.Id
                                                           where personIds.Contains(personAddress.PersonId)
                                                                && personAddress.IsActive
                                                                && address.IsActive
                                                           select new
                                                           {
                                                               PersonID = personAddress.PersonId,
                                                               Address = new AddressRegistrationDTO
                                                               {
                                                                   AddressType = Enum.Parse<AddressTypesEnum>(addressTypes[personAddress.AddressTypeId]),
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
                                                           }).ToLookup(x => x.PersonID, y => y.Address);
            return result;
        }

        public string GetPersonPhoneNumber(int personId)
        {
            string result = (from personPhone in Db.PersonPhoneNumbers
                             join phone in Db.PhoneNumbers on personPhone.PhoneId equals phone.Id
                             where personPhone.PersonId == personId
                                   && personPhone.IsActive
                                   && phone.IsActive
                             select phone.Phone).SingleOrDefault();
            return result;
        }

        public Dictionary<int, string> GetPersonPhoneNumbers(List<int> personIds)
        {
            Dictionary<int, string> result = (from personPhone in Db.PersonPhoneNumbers
                                              join phone in Db.PhoneNumbers on personPhone.PhoneId equals phone.Id
                                              where personIds.Contains(personPhone.PersonId)
                                                    && personPhone.IsActive
                                                    && phone.IsActive
                                              select new
                                              {
                                                  personPhone.PersonId,
                                                  phone.Phone
                                              }).ToDictionary(x => x.PersonId, y => y.Phone);
            return result;
        }

        public string GetPersonEmail(int personId)
        {
            string result = (from personEmail in Db.PersonEmailAddresses
                             join email in Db.EmailAddresses on personEmail.EmailAddressId equals email.Id
                             where personEmail.PersonId == personId
                                && personEmail.IsActive
                                && email.IsActive
                             select email.Email).SingleOrDefault();
            return result;
        }

        public Dictionary<int, string> GetPersonEmails(List<int> personIds)
        {
            Dictionary<int, string> result = (from personEmail in Db.PersonEmailAddresses
                                              join email in Db.EmailAddresses on personEmail.EmailAddressId equals email.Id
                                              where personIds.Contains(personEmail.PersonId)
                                                 && personEmail.IsActive
                                                 && email.IsActive
                                              select new
                                              {
                                                  personEmail.PersonId,
                                                  email.Email
                                              }).ToDictionary(x => x.PersonId, y => y.Email);
            return result;
        }

        public string GetPersonPhoto(int personId)
        {
            string result = (from personFile in Db.PersonFiles
                             join file in Db.Files on personFile.FileId equals file.Id
                             join fType in Db.NfileTypes on personFile.FileTypeId equals fType.Id
                             where file.IsActive
                                && personFile.IsActive
                                && personFile.RecordId == personId
                                && fType.Code == FileTypeEnum.PHOTO.ToString()
                             select $"url(data:{file.MimeType};base64,{Convert.ToBase64String(file.Content)}").SingleOrDefault();
            return result;
        }

        public string GetPersonPhotoByFileId(int fileId)
        {
            // Is it ok to call FirstOrDefault here? We are getting the photo by FileID, but the same person could upload the same photo
            // more than once and since a single person can have many persons in the DB, this is necessary
            string result = (from personFile in Db.PersonFiles
                             join file in Db.Files on personFile.FileId equals file.Id
                             join fType in Db.NfileTypes on personFile.FileTypeId equals fType.Id
                             where file.IsActive
                                && personFile.IsActive
                                && personFile.FileId == fileId
                                && fType.Code == FileTypeEnum.PHOTO.ToString()
                             select $"url(data:{file.MimeType};base64,{Convert.ToBase64String(file.Content)}").FirstOrDefault();
            return result;
        }

        public DownloadableFileDTO GetPersonPhotoAsModel(int userId)
        {
            DownloadableFileDTO result = (
                from u in Db.Users
                join pFile in Db.PersonFiles on u.PersonId equals pFile.RecordId
                join photo in Db.Files on pFile.FileId equals photo.Id
                join fType in Db.NfileTypes on pFile.FileTypeId equals fType.Id
                where u.Id == userId
                   && pFile.IsActive
                   && photo.IsActive
                   && fType.Code == nameof(FileTypeEnum.PHOTO)
                select new DownloadableFileDTO
                {
                    MimeType = photo.MimeType,
                    Bytes = photo.Content,
                    FileName = photo.Name
                }
            ).SingleOrDefault();
            return result;
        }

        public Dictionary<int, string> GetPersonPhotos(List<int> personIds)
        {
            Dictionary<int, string> result = (from personFile in Db.PersonFiles
                                              join file in Db.Files on personFile.FileId equals file.Id
                                              where file.IsActive
                                                    && personIds.Contains(personFile.RecordId)
                                              select new
                                              {
                                                  PersonID = personFile.RecordId,
                                                  Photo = $"url(data:{file.MimeType};base64,{Convert.ToBase64String(file.Content)}"
                                              }).ToDictionary(x => x.PersonID, y => y.Photo);
            return result;
        }

        public RegixPersonDataDTO GetRegixPersonData(int personId)
        {
            string phone = GetPersonPhoneNumber(personId);
            string email = GetPersonEmail(personId);
            PersonDocumentDTO document = GetPersonDocument(personId);

            RegixPersonDataDTO regixData = (from person in Db.Persons
                                            where personId == person.Id
                                            select new RegixPersonDataDTO
                                            {
                                                EgnLnc = new EgnLncDTO
                                                {
                                                    EgnLnc = person.EgnLnc,
                                                    IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                                                },
                                                FirstName = person.FirstName,
                                                MiddleName = person.MiddleName,
                                                LastName = person.LastName,
                                                HasBulgarianAddressRegistration = person.HasBulgarianAddressRegistration,
                                                Document = document,
                                                CitizenshipCountryId = person.CitizenshipCountryId,
                                                Phone = phone,
                                                Email = email,
                                                BirthDate = person.BirthDate,
                                                GenderId = person.GenderId
                                            }).First();

            return regixData;
        }

        public Dictionary<int, RegixPersonDataDTO> GetRegixPersonsData(List<int> personIds)
        {
            DateTime now = DateTime.Now;

            Dictionary<int, string> phones = GetPersonPhoneNumbers(personIds);
            Dictionary<int, string> emails = GetPersonEmails(personIds);
            Dictionary<int, PersonDocumentDTO> documents = GetPersonDocuments(personIds);

            Dictionary<int, RegixPersonDataDTO> regixData = (from person in Db.Persons
                                                             where personIds.Contains(person.Id)
                                                             select new
                                                             {
                                                                 PersonID = person.Id,
                                                                 RegixData = new RegixPersonDataDTO
                                                                 {
                                                                     EgnLnc = new EgnLncDTO
                                                                     {
                                                                         EgnLnc = person.EgnLnc,
                                                                         IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                                                                     },
                                                                     FirstName = person.FirstName,
                                                                     MiddleName = person.MiddleName,
                                                                     LastName = person.LastName,
                                                                     HasBulgarianAddressRegistration = person.HasBulgarianAddressRegistration,
                                                                     CitizenshipCountryId = person.CitizenshipCountryId,
                                                                     BirthDate = person.BirthDate,
                                                                     GenderId = person.GenderId
                                                                 }
                                                             }).ToDictionary(x => x.PersonID, y => y.RegixData);

            foreach (KeyValuePair<int, RegixPersonDataDTO> pair in regixData)
            {
                if (phones.TryGetValue(pair.Key, out string phone))
                {
                    pair.Value.Phone = phone;
                }
                if (emails.TryGetValue(pair.Key, out string email))
                {
                    pair.Value.Email = email;
                }
                if (documents.TryGetValue(pair.Key, out PersonDocumentDTO document))
                {
                    pair.Value.Document = document;
                }
            }
            return regixData;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.Persons, id);
        }
    }
}
