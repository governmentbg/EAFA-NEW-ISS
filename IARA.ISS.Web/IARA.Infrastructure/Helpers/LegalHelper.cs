using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.EntityModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.Internal
{
    internal static class LegalHelper
    {
        public static Legal AddOrEditLegal(this IARADbContext db,
                                           ApplicationRegisterDataDTO applicationRegisterData,
                                           RegixLegalDataDTO data,
                                           List<AddressRegistrationDTO> addresses = null,
                                           int? oldLegalId = null)
        {
            DateTime now = DateTime.Now;

            Legal current;

            // creating a new entry
            if (oldLegalId == null)
            {
                // if an old entry for this EIK exists, set Validto = now and proceed to create a new entry
                Legal lastValidEntry = (from legal in db.Legals
                                        where legal.Eik == data.EIK
                                              && legal.RecordType == applicationRegisterData.RecordType.ToString()
                                              && legal.ValidFrom <= now
                                              && legal.ValidTo > now
                                        select legal).SingleOrDefault();

                if (lastValidEntry != null)
                {
                    lastValidEntry.ValidTo = now;
                }

                current = new Legal
                {
                    ApplicationId = applicationRegisterData.ApplicationId,
                    RecordType = applicationRegisterData.RecordType.ToString(),
                    RegisterApplicationId = data.Id,
                    Name = data.Name,
                    Eik = data.EIK,
                    IsLegalOwnerSameAsApplicant = data.IsCustodianOfPropertySameAsApplicant.Value,
                    ValidFrom = now,
                    ValidTo = DefaultConstants.MAX_VALID_DATE
                };

                if (data.CustodianOfProperty != null)
                {
                    RegixPersonDataDTO custodian = new RegixPersonDataDTO
                    {
                        EgnLnc = new EgnLncDTO
                        {
                            EgnLnc = data.CustodianOfProperty.EgnLnc.EgnLnc,
                            IdentifierType = data.CustodianOfProperty.EgnLnc.IdentifierType
                        },
                        FirstName = data.CustodianOfProperty.FirstName,
                        MiddleName = data.CustodianOfProperty.MiddleName,
                        LastName = data.CustodianOfProperty.LastName
                    };

                    current.LegalOwner = db.AddOrEditPerson(custodian);
                }

                db.Legals.Add(current);
            }
            else
            {
                // Ако е подадено oldLegalId, но ЕИК-то е различно, то тогава създаваме нов Legal
                string eik = (from legal in db.Legals
                              where legal.Id == oldLegalId
                              select legal.Eik).Single();

                if (data.EIK != eik)
                {
                    return db.AddOrEditLegal(applicationRegisterData, data, addresses, null);
                }

                // В противен случай редактираме стария
                current = (from legal in db.Legals
                                .AsSplitQuery()
                                .Include(x => x.LegalPhoneNumbers)
                                .Include(x => x.LegalEmailAddresses)
                                .Include(x => x.LegalsAddresses)
                           where legal.Id == oldLegalId
                           select legal).Single();

                current.Name = data.Name;

                if (data.CustodianOfProperty != null)
                {
                    RegixPersonDataDTO custodian = new RegixPersonDataDTO
                    {
                        EgnLnc = new EgnLncDTO
                        {
                            EgnLnc = data.CustodianOfProperty.EgnLnc.EgnLnc,
                            IdentifierType = data.CustodianOfProperty.EgnLnc.IdentifierType
                        },
                        FirstName = data.CustodianOfProperty.FirstName,
                        MiddleName = data.CustodianOfProperty.MiddleName,
                        LastName = data.CustodianOfProperty.LastName
                    };

                    current.IsLegalOwnerSameAsApplicant = data.IsCustodianOfPropertySameAsApplicant.Value;
                    current.LegalOwner = db.AddOrEditPerson(custodian, null, current.LegalOwnerId);
                }
                else
                {
                    current.LegalOwnerId = null;
                }
            }

            AddOrEditPhoneNumber(db, current, data.Phone);
            AddOrEditEmailAddress(db, current, data.Email);
            AddOrEditAddresses(db, current, addresses);

            return current;
        }

        private static void AddOrEditAddresses(IARADbContext db, Legal legal, List<AddressRegistrationDTO> addresses)
        {
            if (addresses != null)
            {
                Dictionary<string, int> addressTypes = (from addrType in db.NaddressTypes
                                                        select new
                                                        {
                                                            addrType.Code,
                                                            addrType.Id
                                                        }).ToDictionary(x => x.Code, y => y.Id);

                List<LegalsAddress> result = (from legAddress in legal.LegalsAddresses
                                              where legAddress.IsActive
                                              select legAddress).ToList();

                // remove addresses
                List<int> currentAddressTypes = result.Select(x => x.AddressTypeId).ToList();
                List<int> newAddressTypes = addresses.Select(x => addressTypes[x.AddressType.ToString()]).ToList();
                List<int> addressTypesToDelete = currentAddressTypes.Where(x => !newAddressTypes.Contains(x)).ToList();

                foreach (LegalsAddress addr in result)
                {
                    if (addressTypesToDelete.Contains(addr.AddressTypeId))
                    {
                        addr.IsActive = false;
                    }
                }

                // add or update old addresses
                foreach (AddressRegistrationDTO addr in addresses)
                {
                    LegalsAddress legAddress = result.Where(x => x.AddressTypeId == addressTypes[addr.AddressType.ToString()]).SingleOrDefault();
                    Address address;

                    if (legAddress != null)
                    {
                        address = db.Addresses.Where(x => x.Id == legAddress.AddressId).Select(x => x).Single();
                    }
                    else
                    {
                        LegalsAddress newLegAddr = new LegalsAddress
                        {
                            Legal = legal,
                            AddressTypeId = addressTypes[addr.AddressType.ToString()],
                            Address = new Address()
                        };

                        db.LegalsAddresses.Add(newLegAddr);
                        address = newLegAddr.Address;
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
        }

        private static void AddOrEditEmailAddress(IARADbContext db, Legal legal, string emailAddress)
        {
            foreach (LegalEmailAddress address in legal.LegalEmailAddresses)
            {
                address.IsActive = false;
            }

            if (!string.IsNullOrEmpty(emailAddress))
            {
                LegalEmailAddress dbLegEmail = (from legEmail in legal.LegalEmailAddresses
                                                join email in db.EmailAddresses on legEmail.EmailAddressId equals email.Id
                                                where email.Email == emailAddress
                                                select legEmail).SingleOrDefault();

                if (dbLegEmail != null)
                {
                    dbLegEmail.IsActive = true;
                }
                else
                {
                    EmailAddress email = ActivateOrAddEmailAddress(db, emailAddress);

                    LegalEmailAddress legEmailAddress = new LegalEmailAddress
                    {
                        Legal = legal,
                        EmailAddress = email
                    };

                    db.LegalEmailAddresses.Add(legEmailAddress);
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

        private static void AddOrEditPhoneNumber(IARADbContext db, Legal legal, string phoneNumber)
        {
            foreach (LegalPhoneNumber phone in legal.LegalPhoneNumbers)
            {
                phone.IsActive = false;
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                LegalPhoneNumber dbLegPhone = (from legPhone in legal.LegalPhoneNumbers
                                               join phone in db.PhoneNumbers on legPhone.PhoneId equals phone.Id
                                               where phone.Phone == phoneNumber
                                               select legPhone).SingleOrDefault();

                if (dbLegPhone != null)
                {
                    dbLegPhone.IsActive = true;
                }
                else
                {
                    PhoneNumber phone = ActivateOrAddPhoneNumber(db, phoneNumber);

                    LegalPhoneNumber legPhoneNumber = new LegalPhoneNumber
                    {
                        Legal = legal,
                        Phone = phone
                    };

                    db.LegalPhoneNumbers.Add(legPhoneNumber);
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
    }
}
