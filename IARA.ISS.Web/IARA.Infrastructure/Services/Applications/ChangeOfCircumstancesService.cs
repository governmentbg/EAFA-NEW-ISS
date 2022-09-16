using System;
using System.Linq;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces.Applications;
using IARA.Interfaces;
using IARA.Interfaces.Legals;

namespace IARA.Infrastructure.Services.Applications
{
    public class ChangeOfCircumstancesService : Service, IChangeOfCircumstancesService
    {
        private readonly IPersonService personService;
        private readonly ILegalService legalService;
        private readonly IAddressService addressService;

        public ChangeOfCircumstancesService(IARADbContext db,
                                            IPersonService personService,
                                            ILegalService legalService,
                                            IAddressService addressService)
            : base(db)
        {
            this.personService = personService;
            this.legalService = legalService;
            this.addressService = addressService;
        }

        public List<ChangeOfCircumstancesDTO> GetChangeOfCircumstances(int applicationId)
        {
            var data = (from cof in Db.ApplicationChangeOfCircumstances
                        join type in Db.NchangeOfCircumstancesTypes on cof.ChangeOfCircumstancesTypeId equals type.Id
                        where cof.ApplicationId == applicationId
                            && cof.IsActive
                        orderby cof.EventDateTime, cof.Id
                        select new
                        {
                            cof.Id,
                            cof.EventDateTime,
                            cof.ChangeOfCircumstancesTypeId,
                            cof.ChangeDescription,
                            DataType = type.DataType,
                            cof.AddressId,
                            cof.LegalId,
                            cof.PersonId,
                            cof.ShipId,
                            cof.AquacultureFacilityId,
                            cof.BuyerId,
                            cof.PermitId,
                            cof.PermitLicenceId,
                            cof.IsActive
                        }).ToList();

            List<ChangeOfCircumstancesDTO> result = new List<ChangeOfCircumstancesDTO>();

            foreach (var cof in data)
            {
                ChangeOfCircumstancesDTO change = new ChangeOfCircumstancesDTO
                {
                    Id = cof.Id,
                    TypeId = cof.ChangeOfCircumstancesTypeId,
                    Description = cof.ChangeDescription,
                    DataType = Enum.Parse<ChangeOfCircumstancesDataTypeEnum>(cof.DataType),
                    ShipId = cof.ShipId,
                    AquacultureFacilityId = cof.AquacultureFacilityId,
                    BuyerId = cof.BuyerId,
                    PermitId = cof.PersonId,
                    PermitLicenceId = cof.PermitLicenceId
                };

                switch (change.DataType)
                {
                    case ChangeOfCircumstancesDataTypeEnum.Person:
                        change.Person = personService.GetRegixPersonData(cof.PersonId.Value);
                        break;
                    case ChangeOfCircumstancesDataTypeEnum.Legal:
                        change.Legal = legalService.GetRegixLegalData(cof.LegalId.Value);
                        break;
                    case ChangeOfCircumstancesDataTypeEnum.Address:
                        change.Address = addressService.GetAddressRegistration(cof.AddressId.Value);
                        break;
                }

                result.Add(change);
            }

            return result;
        }

        public void AddOrEditChangeOfCircumstances(int applicationId,
                                                   List<ChangeOfCircumstancesDTO> changes,
                                                   int? shipId = null,
                                                   int? aquacultureFacilityId = null,
                                                   int? buyerId = null,
                                                   int? permitId = null,
                                                   int? permitLicenseId = null)
        {
            DateTime now = DateTime.Now;
            List<int> changesIds = changes.Select(x => x.Id).ToList();

            List<ApplicationChangeOfCircumstance> oldChanges = (from coc in Db.ApplicationChangeOfCircumstances
                                                                where coc.ApplicationId == applicationId
                                                                select coc).ToList();

            List<ApplicationChangeOfCircumstance> changesToDel = oldChanges.Where(x => x.IsActive && !changesIds.Contains(x.Id)).ToList();
            List<ApplicationChangeOfCircumstance> changesToEdit = oldChanges.Where(x => changesIds.Contains(x.Id)).ToList();
            List<ChangeOfCircumstancesDTO> changesToAdd = changes.Where(x => x.Id == default).ToList();

            // Delete
            foreach (ApplicationChangeOfCircumstance change in changesToDel)
            {
                change.IsActive = false;
            }

            // Edit
            foreach (ApplicationChangeOfCircumstance change in changesToEdit)
            {
                ChangeOfCircumstancesDTO newChange = changes.Where(x => x.Id == change.Id).Single();

                change.ChangeOfCircumstancesTypeId = newChange.TypeId;
                change.ChangeDescription = newChange.Description;
                change.EventDateTime = now;
                change.IsActive = true;

                EditChangeOfCircumstancesExtraData(change, newChange, shipId, aquacultureFacilityId, buyerId, permitId, permitLicenseId);
            }

            // Add
            foreach (ChangeOfCircumstancesDTO change in changesToAdd)
            {
                ApplicationChangeOfCircumstance entry = new ApplicationChangeOfCircumstance
                {
                    ApplicationId = applicationId,
                    ChangeOfCircumstancesTypeId = change.TypeId,
                    EventDateTime = now,
                    ChangeDescription = change.Description
                };

                EditChangeOfCircumstancesExtraData(entry, change, shipId, aquacultureFacilityId, buyerId, permitId, permitLicenseId);

                Db.ApplicationChangeOfCircumstances.Add(entry);
            }

            Db.SaveChanges();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.ApplicationChangeOfCircumstances, id);
        }

        private void EditChangeOfCircumstancesExtraData(ApplicationChangeOfCircumstance entry,
                                                        ChangeOfCircumstancesDTO change,
                                                        int? shipId = null,
                                                        int? aquacultureFacilityId = null,
                                                        int? buyerId = null,
                                                        int? permitId = null,
                                                        int? permitLicenseId = null)
        {
            if (shipId.HasValue)
            {
                entry.ShipId = shipId.Value;
            }

            if (aquacultureFacilityId.HasValue)
            {
                entry.AquacultureFacilityId = aquacultureFacilityId.Value;
            }

            if (buyerId.HasValue)
            {
                entry.BuyerId = buyerId.Value;
            }

            if (permitId.HasValue)
            {
                entry.PermitId = permitId.Value;
            }

            if (permitLicenseId.HasValue)
            {
                entry.PermitLicenceId = permitLicenseId.Value;
            }

            switch (change.DataType)
            {
                case ChangeOfCircumstancesDataTypeEnum.Person:
                    entry.LegalId = null;
                    entry.AddressId = null;
                    entry.Person = Db.AddOrEditPerson(change.Person, null, entry.PersonId);
                    break;
                case ChangeOfCircumstancesDataTypeEnum.Legal:
                    entry.PersonId = null;
                    entry.AddressId = null;
                    entry.Legal = Db.AddOrEditLegal(new ApplicationRegisterDataDTO
                    {
                        RecordType = RecordTypesEnum.Application,
                        ApplicationId = entry.ApplicationId
                    }, change.Legal, null, entry.LegalId);
                    break;
                case ChangeOfCircumstancesDataTypeEnum.Address:
                    entry.PersonId = null;
                    entry.LegalId = null;
                    entry.Address = Db.AddOrEditAddress(change.Address, true, entry.AddressId);
                    break;
            }
        }
    }
}
