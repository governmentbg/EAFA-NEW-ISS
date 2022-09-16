using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.EntityModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.Internal
{
    internal static class PersonHelper
    {
        public static Person AddOrEditPerson(this IARADbContext db,
                                             RegixPersonDataDTO data,
                                             List<AddressRegistrationDTO> addresses = null,
                                             int? oldPersonId = null,
                                             FileInfoDTO photo = null,
                                             string photoBase64 = null)
        {
            DateTime now = DateTime.Now;

            Person current;
            if ((data.BirthDate == null || !data.BirthDate.HasValue) && data.EgnLnc?.IdentifierType == IdentifierTypeEnum.EGN)
            {
                if (IsEGNValid(data.EgnLnc.EgnLnc))
                {
                    data.BirthDate = ParseUserEGN(data.EgnLnc.EgnLnc);
                }
            }

            // creating a new entry
            if (oldPersonId == null)
            {
                // if an old entry for this EGN exists, set Validto = now and proceed to create a new entry
                Person lastValidEntry = (from person in db.Persons
                                         where person.EgnLnc == data.EgnLnc.EgnLnc
                                           && person.IdentifierType == data.EgnLnc.IdentifierType.ToString()
                                           && person.ValidFrom <= now
                                           && person.ValidTo >= now
                                         select person).SingleOrDefault();

                if (lastValidEntry != null)
                {
                    lastValidEntry.ValidTo = now;
                }

                current = new Person
                {
                    FirstName = data.FirstName,
                    MiddleName = data.MiddleName,
                    LastName = data.LastName,
                    EgnLnc = data.EgnLnc.EgnLnc,
                    IdentifierType = data.EgnLnc.IdentifierType.ToString(),
                    HasBulgarianAddressRegistration = data.HasBulgarianAddressRegistration,
                    CitizenshipCountryId = data.CitizenshipCountryId,
                    BirthDate = data.BirthDate,
                    ValidFrom = now,
                    ValidTo = DefaultConstants.MAX_VALID_DATE,
                    GenderId = data.GenderId
                };

                db.Persons.Add(current);
            }
            else
            {
                // Ако е подаден oldPersonId, но ЕГН-то е различно, то тогава създаваме нов Person
                EgnLncDTO egnLnc = (from person in db.Persons
                                    where person.Id == oldPersonId
                                    select new EgnLncDTO
                                    {
                                        EgnLnc = person.EgnLnc,
                                        IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                                    }).Single();

                if (data.EgnLnc.EgnLnc != egnLnc.EgnLnc || data.EgnLnc.IdentifierType != egnLnc.IdentifierType)
                {
                    return db.AddOrEditPerson(data, addresses, null, photo, photoBase64);
                }

                // В противен случай редактираме стария
                current = (from person in db.Persons
                                .AsSplitQuery()
                                .Include(x => x.PersonPhoneNumbers)
                                .Include(x => x.PersonEmailAddresses)
                                .Include(x => x.PersonAddresses)
                                .Include(x => x.PersonDocuments)
                                .Include(x => x.PersonFiles)
                           where person.Id == oldPersonId
                           select person).Single();

                current.FirstName = data.FirstName;
                current.MiddleName = data.MiddleName;
                current.LastName = data.LastName;
                current.HasBulgarianAddressRegistration = data.HasBulgarianAddressRegistration;
                current.CitizenshipCountryId = data.CitizenshipCountryId;
                current.BirthDate = data.BirthDate;
                current.GenderId = data.GenderId;
            }

            AddOrEditPhoneNumber(db, current, data.Phone);
            AddOrEditEmailAddress(db, current, data.Email);
            AddOrEditAddresses(db, current, addresses);
            AddOrEditPersonDocument(db, current, data.Document);
            AddOrEditPersonPhoto(db, current, photo, photoBase64);

            return current;
        }

        public static Dictionary<int, string> GetAddressTypesDictionary(this IARADbContext db)
        {
            return (from addrType in db.NaddressTypes
                    select new
                    {
                        addrType.Id,
                        addrType.Code
                    }).ToDictionary(x => x.Id, y => y.Code);
        }

        public static Dictionary<string, int> GetAddressTypesCodeDictionary(this IARADbContext db)
        {
            return (from addrType in db.NaddressTypes
                    select new
                    {
                        addrType.Code,
                        addrType.Id
                    }).ToDictionary(x => x.Code, y => y.Id);
        }

        private static void AddOrEditAddresses(this IARADbContext db, Person person, List<AddressRegistrationDTO> addresses)
        {
            List<PersonAddress> result = (from perAddress in person.PersonAddresses
                                          where perAddress.IsActive
                                          select perAddress).ToList();

            if (addresses != null)
            {
                Dictionary<string, int> addressTypes = db.GetAddressTypesCodeDictionary();

                // remove addresses
                List<int> currentAddressTypes = result.Select(x => x.AddressTypeId).ToList();
                List<int> newAddressTypes = addresses.Select(x => addressTypes[x.AddressType.ToString()]).ToList();
                List<int> addressTypesToDelete = currentAddressTypes.Where(x => !newAddressTypes.Contains(x)).ToList();

                foreach (PersonAddress addr in result)
                {
                    if (addressTypesToDelete.Contains(addr.AddressTypeId))
                    {
                        addr.IsActive = false;
                    }
                }

                // add or update old addresses
                foreach (AddressRegistrationDTO addr in addresses)
                {
                    PersonAddress perAddress = result.Where(x => x.AddressTypeId == addressTypes[addr.AddressType.ToString()]).SingleOrDefault();
                    Address address;

                    if (perAddress != null)
                    {
                        address = db.Addresses.Where(x => x.Id == perAddress.AddressId).Select(x => x).Single();
                    }
                    else
                    {
                        PersonAddress newPerAddr = new PersonAddress
                        {
                            Person = person,
                            AddressTypeId = addressTypes[addr.AddressType.ToString()],
                            Address = new Address()
                        };

                        db.PersonAddresses.Add(newPerAddr);
                        address = newPerAddr.Address;
                    }

                    address.CountryId = addr.CountryId;
                    address.DistrictId = addr.DistrictId;
                    address.MunicipalityId = addr.MunicipalityId;
                    address.PopulatedAreaId = addr.PopulatedAreaId;
                    address.Region = addr.Region;
                    address.PostCode = addr.PostalCode;
                    address.Street = addr.Street;
                    address.StreetNum = addr.StreetNum;
                    address.BlockNum = addr.BlockNum;
                    address.EntranceNum = addr.EntranceNum;
                    address.FloorNum = addr.FloorNum;
                    address.ApartmentNum = addr.ApartmentNum;
                }
            }
            else
            {
                foreach (PersonAddress addr in result)
                {
                    addr.IsActive = false;
                }
            }
        }

        private static void AddOrEditPhoneNumber(IARADbContext db, Person person, string phoneNumber)
        {
            foreach (PersonPhoneNumber phone in person.PersonPhoneNumbers)
            {
                phone.IsActive = false;
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                PersonPhoneNumber dbPerPhone = (from perPhone in person.PersonPhoneNumbers
                                                join phone in db.PhoneNumbers on perPhone.PhoneId equals phone.Id
                                                where phone.Phone == phoneNumber
                                                select perPhone).SingleOrDefault();

                if (dbPerPhone != null)
                {
                    dbPerPhone.IsActive = true;
                }
                else
                {
                    PhoneNumber phone = ActivateOrAddPhoneNumber(db, phoneNumber);

                    PersonPhoneNumber perPhoneNumber = new PersonPhoneNumber
                    {
                        Person = person,
                        Phone = phone
                    };

                    db.PersonPhoneNumbers.Add(perPhoneNumber);
                }
            }
        }

        private static PhoneNumber ActivateOrAddPhoneNumber(IARADbContext db, string phoneNumber)
        {
            PhoneNumber phone = db.PhoneNumbers.Where(x => x.Phone == phoneNumber).SingleOrDefault();

            if (phone != null)
            {
                phone.IsActive = true;
            }
            else
            {
                phone = new PhoneNumber
                {
                    Phone = phoneNumber
                };

                db.PhoneNumbers.Add(phone);
            }

            return phone;
        }

        private static void AddOrEditEmailAddress(IARADbContext db, Person person, string emailAddress)
        {
            foreach (PersonEmailAddress address in person.PersonEmailAddresses)
            {
                address.IsActive = false;
            }

            if (!string.IsNullOrEmpty(emailAddress))
            {
                PersonEmailAddress dbPerEmail = (from perEmail in person.PersonEmailAddresses
                                                 join email in db.EmailAddresses on perEmail.EmailAddressId equals email.Id
                                                 where email.Email == emailAddress
                                                 select perEmail).SingleOrDefault();

                if (dbPerEmail != null)
                {
                    dbPerEmail.IsActive = true;
                }
                else
                {
                    EmailAddress email = ActivateOrAddEmailAddress(db, emailAddress);

                    PersonEmailAddress perEmailAddress = new PersonEmailAddress
                    {
                        Person = person,
                        EmailAddress = email
                    };

                    db.PersonEmailAddresses.Add(perEmailAddress);
                }
            }
        }

        private static EmailAddress ActivateOrAddEmailAddress(IARADbContext db, string emailAddress)
        {
            EmailAddress email = db.EmailAddresses.Where(x => x.Email == emailAddress).SingleOrDefault();

            if (email != null)
            {
                email.IsActive = true;
            }
            else
            {
                email = new EmailAddress
                {
                    Email = emailAddress
                };

                db.EmailAddresses.Add(email);
            }

            return email;
        }

        private static void AddOrEditPersonDocument(IARADbContext db, Person person, PersonDocumentDTO document)
        {
            PersonDocument dbDocument = null;

            if (document != null)
            {
                dbDocument = (from perDocument in person.PersonDocuments
                              where perDocument.DocumentTypeId == document.DocumentTypeID
                              select perDocument).SingleOrDefault();
            }

            if (dbDocument != null)
            {
                if (document == null)
                {
                    dbDocument.IsActive = false;
                }
                else
                {
                    dbDocument.DocumentTypeId = document.DocumentTypeID;
                    dbDocument.DocumentNumber = document.DocumentNumber;
                    dbDocument.DocumentIssueDate = document.DocumentIssuedOn;
                    dbDocument.DocumentPublisher = document.DocumentIssuedBy;
                    dbDocument.IsActive = true;
                }
            }
            else
            {
                if (document != null)
                {
                    PersonDocument perDocument = new PersonDocument
                    {
                        Person = person,
                        DocumentTypeId = document.DocumentTypeID,
                        DocumentNumber = document.DocumentNumber,
                        DocumentIssueDate = document.DocumentIssuedOn,
                        DocumentPublisher = document.DocumentIssuedBy
                    };

                    db.PersonDocuments.Add(perDocument);
                }
            }
        }

        private static void AddOrEditPersonPhoto(IARADbContext db, Person person, FileInfoDTO photo, string photoBase64)
        {
            if (photo != null)
            {
                // photo exists in this context
                if (photo.Id.HasValue)
                {
                    PersonFile personFile = db.GetPersonPhoto(person.PersonFiles, photo.Id.Value);
                    if (personFile != null)
                    {
                        personFile.IsActive = !photo.Deleted;
                        db.AddOrEditFile(photo, increaseReferenceCounter: false);
                    }
                    else
                    {
                        File updatedReferenceFile = db.AddOrEditFile(photo, increaseReferenceCounter: true);
                        personFile = new PersonFile
                        {
                            Record = person,
                            File = updatedReferenceFile,
                            FileTypeId = db.GetPhotoFileTypeId()
                        };

                        db.PersonFiles.Add(personFile);
                    }
                }
                // file is new in this context
                else if (photo.File != null)
                {
                    photo.FileTypeId = db.GetPhotoFileTypeId();
                    File newFile = db.AddOrEditFile(photo, increaseReferenceCounter: true);
                    PersonFile oldPhoto = db.GetPersonPhoto(person.PersonFiles);
                    if (oldPhoto != null)
                    {
                        oldPhoto.IsActive = false;
                    }
                    PersonFile personFile = db.GetPersonPhoto(person.PersonFiles, newFile.Id);
                    if (personFile != null)
                    {
                        personFile.IsActive = true;
                    }
                    else
                    {
                        personFile = new PersonFile
                        {
                            Record = person,
                            File = newFile,
                            FileTypeId = photo.FileTypeId
                        };
                        db.PersonFiles.Add(personFile);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(photoBase64))
            {
                File newFile = db.AddOrEditBase64Image(photoBase64, increaseReferenceCounter: true);
                PersonFile oldPhoto = db.GetPersonPhoto(person.PersonFiles);
                if (oldPhoto != null)
                {
                    oldPhoto.IsActive = false;
                }
                PersonFile personFile = db.GetPersonPhoto(person.PersonFiles, newFile.Id);
                if (personFile != null)
                {
                    personFile.IsActive = true;
                }
                else
                {
                    personFile = new PersonFile
                    {
                        Record = person,
                        File = newFile,
                        FileTypeId = db.GetPhotoFileTypeId()
                    };
                    db.PersonFiles.Add(personFile);
                }
            }
        }

        private static PersonFile GetPersonPhoto(this IARADbContext db, ICollection<PersonFile> personFiles, int? fileId = null)
        {
            IQueryable<PersonFile> query = from personFile in db.PersonFiles
                                           join file in db.Files on personFile.FileId equals file.Id
                                           join fileType in db.NfileTypes on personFile.FileTypeId equals fileType.Id
                                           where file.IsActive
                                           && fileType.Code == nameof(FileTypeEnum.PHOTO)
                                           && personFiles.Select(x => x.Id).Contains(personFile.Id)
                                           select personFile;

            if (fileId.HasValue)
            {
                query = from personFile in query
                        where personFile.FileId == fileId.Value
                        select personFile;
            }
            else
            {
                query = from personFile in query
                        where personFile.IsActive
                        select personFile;
            }

            return query.SingleOrDefault();
        }

        private static int GetPhotoFileTypeId(this IARADbContext db)
        {
            return (from fileType in db.NfileTypes
                    where fileType.Code == nameof(FileTypeEnum.PHOTO)
                    select fileType.Id).Single();
        }

        private static bool IsEGNValid(string egn)
        {
            if (egn.Length != 10 || egn.Any(x => !char.IsDigit(x)))
            {
                return false;
            }

            int[] weights = new int[] { 2, 4, 8, 5, 10, 9, 7, 3, 6 };

            int year = int.Parse(egn.Substring(0, 2));
            int month = int.Parse(egn.Substring(2, 2));
            int day = int.Parse(egn.Substring(4, 2));

            if (month > 40)
            {
                year += 2000;
                month -= 40;

                if (!IsValidDate(year, month, day))
                {
                    return false;
                }
            }
            else if (month > 20)
            {
                year += 1800;
                month -= 20;

                if (!IsValidDate(year, month, day))
                {
                    return false;
                }
            }
            else
            {
                year += 1900;

                if (!IsValidDate(year, month, day))
                {
                    return false;
                }
            }

            int sum = 0;
            for (int i = 0; i < 9; ++i)
            {
                sum += (egn[i] - '0') * weights[i];
            }

            int validCheckSum = sum % 11;
            if (validCheckSum == 10)
            {
                validCheckSum = 0;
            }

            int checksum = int.Parse(egn.Substring(9, 1));
            return checksum == validCheckSum;
        }

        private static bool IsValidDate(int year, int month, int day)
        {
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
            {
                return false;
            }

            if (month < 1 || month > 12)
            {
                return false;
            }

            return day > 0 && day <= DateTime.DaysInMonth(year, month);
        }

        private static DateTime ParseUserEGN(string egn)
        {
            string dateStr = egn.Substring(0, 6);

            int[] dateArray = Split(dateStr, 2).Select(x => int.Parse(x)).ToArray();

            if (dateArray[1] > 40)
            {
                dateArray[0] += 2000;
                dateArray[1] -= 40;
            }
            else
            {
                dateArray[0] += 1900;
            }

            return new DateTime(dateArray[0], dateArray[1], dateArray[2]);
        }

        private static IEnumerable<string> Split(string str, int byCount)
        {
            char[] array = new char[byCount];

            int count = 0;

            for (int i = 0; i < str.Length; i++)
            {
                array[count] = str[i];
                count++;

                if (count == byCount)
                {
                    count = 0;
                    yield return new string(array);
                }
            }

            if (count > 0)
            {
                yield return new string(array.Take(count).ToArray());
            }
            else
            {
                yield break;
            }
        }
    }
}
