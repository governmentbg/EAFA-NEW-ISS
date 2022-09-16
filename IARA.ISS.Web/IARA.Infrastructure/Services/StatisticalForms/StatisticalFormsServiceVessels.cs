using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.StatisticalForms;
using IARA.DomainModels.DTOModels.StatisticalForms.FishVessels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class StatisticalFormsService : Service, IStatisticalFormsService
    {
        public StatisticalFormShipDTO GetStatisticalFormShip(int shipId)
        {
            ShipRegister dbShip = (from ship in Db.ShipsRegister
                                   where ship.Id == shipId
                                   select ship).First();

            StatisticalFormShipDTO formShip = new StatisticalFormShipDTO
            {
                ShipId = shipId,
                ShipLenghtId = GetShipLengthInterval(dbShip.TotalLength),
                GrossTonnageId = GetGrossTonnageInterval(dbShip.GrossTonnage),
                ShipYears = DateTime.Now.Year - dbShip.BuildYear,
                FuelTypeId = dbShip.FuelTypeId
            };

            return formShip;
        }

        public StatisticalFormFishVesselApplicationEditDTO GetStatisticalFormFishVesselApplication(int applicationId)
        {
            StatisticalFormsRegister dbStatisticalForm = (from statForm in Db.StatisticalFormsRegister
                                                          where statForm.ApplicationId == applicationId
                                                            && statForm.RecordType == nameof(RecordTypesEnum.Application)
                                                          select statForm).SingleOrDefault();

            int formTypeId = (from type in Db.NstatisticalFormTypes
                              where type.Code == nameof(StatisticalFormTypesEnum.FishVessel)
                              select type.Id).Single();

            StatisticalFormFishVesselApplicationEditDTO result = null;

            if (dbStatisticalForm == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<StatisticalFormFishVesselApplicationEditDTO>(draft);
                    result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);
                    result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                    if (result.EmployeeInfoGroups == null)
                    {
                        result.EmployeeInfoGroups = GetEmployeeCountGroups(null, formTypeId);
                    }

                    if (result.NumStatGroups == null)
                    {
                        result.NumStatGroups = GetNumericStatValueGroups(null, formTypeId);
                    }
                }
                else
                {
                    result = new StatisticalFormFishVesselApplicationEditDTO
                    {
                        EmployeeInfoGroups = GetEmployeeCountGroups(null, formTypeId),
                        NumStatGroups = GetNumericStatValueGroups(null, formTypeId),
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };
                }
            }
            else
            {
                FishVesselsForm dbFishVesselForm = (from fishVesselsForm in Db.FishVesselsForms
                                                    where fishVesselsForm.StatisticalForm == dbStatisticalForm
                                                    select fishVesselsForm).Single();

                result = new StatisticalFormFishVesselApplicationEditDTO
                {
                    Id = dbStatisticalForm.Id,
                    ApplicationId = dbStatisticalForm.ApplicationId,
                    ShipId = dbFishVesselForm.ShipId,
                    ShipPrice = dbFishVesselForm.ShipPrice,
                    Year = dbStatisticalForm.ForYear.Year,
                    SubmittedByWorkPosition = dbStatisticalForm.SubmitPersonWorkPosition,
                    ShipEnginePower = dbFishVesselForm.FuelConsumption,
                    ShipLengthId = dbFishVesselForm.LengthStatIntervalId,
                    ShipTonnageId = dbFishVesselForm.GrossTonnageIntervalId,
                    FuelTypeId = dbFishVesselForm.FuelTypeId,
                    IsFishingMainActivity = dbFishVesselForm.IsFishingMainActivity,
                    IsShipHolderPartOfCrew = dbFishVesselForm.IsOwnerCrewMember,
                    ShipHolderPosition = dbFishVesselForm.OwnerCrewMemberPosition
                };

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);

                result.EmployeeInfoGroups = GetEmployeeCountGroups(dbStatisticalForm.Id, dbStatisticalForm.StatisticalFormTypeId);
                result.NumStatGroups = GetNumericStatValueGroups(dbStatisticalForm.Id, dbStatisticalForm.StatisticalFormTypeId);

                result.SeaDays = GetStatisticalFormFishVesselSeaDays(dbFishVesselForm.Id);

                result.Files = Db.GetFiles(Db.StatisticalFormsRegisterFiles, dbStatisticalForm.Id);
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);
            }

            return result;
        }

        public RegixChecksWrapperDTO<StatisticalFormFishVesselRegixDataDTO> GetStatisticalFormFishVesselRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<StatisticalFormFishVesselRegixDataDTO> result = new RegixChecksWrapperDTO<StatisticalFormFishVesselRegixDataDTO>
            {
                DialogDataModel = GetApplicationFishVesselRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetStatisticalFormFishVesselChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public StatisticalFormFishVesselRegixDataDTO GetApplicationFishVesselRegixData(int applicationId)
        {
            StatisticalFormApplicationDataIds data = GetApplicationDataIds(applicationId);

            StatisticalFormFishVesselRegixDataDTO regixData = new StatisticalFormFishVesselRegixDataDTO
            {
                Id = data.StatisticalFormId,
                ApplicationId = applicationId,
                PageCode = data.PageCode
            };

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId);

            return regixData;
        }

        public StatisticalFormFishVesselEditDTO GetApplicationFishVesselDataForRegister(int applicationId)
        {
            var data = (from statForm in Db.StatisticalFormsRegister
                        join fishVesselsForm in Db.FishVesselsForms on statForm.Id equals fishVesselsForm.StatisticalFormId
                        where statForm.ApplicationId == applicationId
                           && statForm.RecordType == nameof(RecordTypesEnum.Application)
                        select new
                        {
                            statForm.Id,
                            statForm.SubmittedForPersonId,
                            statForm.SubmittedForLegalId,
                            statForm.StatisticalFormTypeId,
                            FishVesselForm = new StatisticalFormFishVesselEditDTO
                            {
                                Id = fishVesselsForm.Id,
                                ApplicationId = statForm.ApplicationId,
                                SubmittedByWorkPosition = statForm.SubmitPersonWorkPosition,
                                ShipId = fishVesselsForm.ShipId,
                                ShipPrice = fishVesselsForm.ShipPrice,
                                Year = statForm.ForYear.Year,
                                ShipEnginePower = fishVesselsForm.FuelConsumption,
                                ShipLengthId = fishVesselsForm.LengthStatIntervalId,
                                ShipTonnageId = fishVesselsForm.GrossTonnageIntervalId,
                                FuelTypeId = fishVesselsForm.FuelTypeId,
                                IsFishingMainActivity = fishVesselsForm.IsFishingMainActivity,
                                IsShipHolderPartOfCrew = fishVesselsForm.IsOwnerCrewMember,
                                ShipHolderPosition = fishVesselsForm.OwnerCrewMemberPosition
                            }
                        }).Single();

            StatisticalFormFishVesselEditDTO result = data.FishVesselForm;

            result.SubmittedFor = applicationService.GetRegisterSubmittedFor(applicationId, data.SubmittedForPersonId, data.SubmittedForLegalId);
            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

            result.EmployeeInfoGroups = GetEmployeeCountGroups(data.Id, data.StatisticalFormTypeId);
            result.NumStatGroups = GetNumericStatValueGroups(data.Id, data.StatisticalFormTypeId);
            result.SeaDays = GetStatisticalFormFishVesselSeaDays(result.Id.Value);

            result.Files = Db.GetFiles(Db.StatisticalFormsRegisterFiles, data.Id);

            return result;
        }

        public StatisticalFormFishVesselEditDTO GetFishVesselRegisterByApplicationId(int applicationId)
        {
            int id = (from statForm in Db.StatisticalFormsRegister
                      where statForm.ApplicationId == applicationId
                        && statForm.RecordType == nameof(RecordTypesEnum.Register)
                      select statForm.Id).SingleOrDefault();

            return GetStatisticalFormFishVessel(id);
        }

        public StatisticalFormFishVesselEditDTO GetStatisticalFormFishVessel(int id)
        {
            var data = (from statForm in Db.StatisticalFormsRegister
                        join fishVesselsForm in Db.FishVesselsForms on statForm.Id equals fishVesselsForm.StatisticalFormId
                        where statForm.Id == id
                            && statForm.RecordType == nameof(RecordTypesEnum.Register)
                        select new
                        {
                            statForm.Id,
                            statForm.SubmittedForPersonId,
                            statForm.SubmittedForLegalId,
                            statForm.StatisticalFormTypeId,
                            FishVesselForm = new StatisticalFormFishVesselEditDTO
                            {
                                Id = fishVesselsForm.Id,
                                ApplicationId = statForm.ApplicationId,
                                SubmittedByWorkPosition = statForm.SubmitPersonWorkPosition,
                                ShipId = fishVesselsForm.ShipId,
                                ShipPrice = fishVesselsForm.ShipPrice,
                                Year = statForm.ForYear.Year,
                                ShipEnginePower = fishVesselsForm.FuelConsumption,
                                ShipLengthId = fishVesselsForm.LengthStatIntervalId,
                                ShipTonnageId = fishVesselsForm.GrossTonnageIntervalId,
                                FuelTypeId = fishVesselsForm.FuelTypeId,
                                IsFishingMainActivity = fishVesselsForm.IsFishingMainActivity,
                                IsShipHolderPartOfCrew = fishVesselsForm.IsOwnerCrewMember,
                                ShipHolderPosition = fishVesselsForm.OwnerCrewMemberPosition,
                                FormNum = statForm.RegistrationNum
                            }
                        }).Single();

            StatisticalFormFishVesselEditDTO result = data.FishVesselForm;

            result.SubmittedFor = applicationService.GetRegisterSubmittedFor(result.ApplicationId.Value, data.SubmittedForPersonId, data.SubmittedForLegalId);
            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(result.ApplicationId.Value, ApplicationHierarchyTypesEnum.Online);

            result.EmployeeInfoGroups = GetEmployeeCountGroups(data.Id, data.StatisticalFormTypeId);
            result.NumStatGroups = GetNumericStatValueGroups(data.Id, data.StatisticalFormTypeId);
            result.SeaDays = GetStatisticalFormFishVesselSeaDays(result.Id.Value);

            result.Files = Db.GetFiles(Db.StatisticalFormsRegisterFiles, data.Id);

            return result;
        }

        public int AddStatisticalFormFishVesselApplication(StatisticalFormFishVesselApplicationEditDTO form, ApplicationStatusesEnum? nextManualStatus = null)
        {
            StatisticalFormsRegister entry;

            using (TransactionScope scope = new TransactionScope())
            {
                Dictionary<StatisticalFormTypesEnum, int> types = GetStatisticalFormTypesCodeToIdDictionary();

                entry = new StatisticalFormsRegister
                {
                    ApplicationId = form.ApplicationId.Value,
                    RecordType = nameof(RecordTypesEnum.Application),
                    RegistrationDate = DateTime.Now,
                    ForYear = new DateTime(form.Year.Value, 1, 1),
                    SubmitPersonWorkPosition = form.SubmittedByWorkPosition,
                    StatisticalFormTypeId = types[StatisticalFormTypesEnum.FishVessel]
                };

                StatisticalFormShipDTO ship = GetStatisticalFormShip(form.ShipId.Value);

                entry.FishVesselsForm = new FishVesselsForm
                {
                    StatisticalForm = entry,
                    ShipId = form.ShipId.Value,
                    ShipPrice = form.ShipPrice.Value,
                    LengthStatIntervalId = ship.ShipLenghtId.Value,
                    GrossTonnageIntervalId = ship.GrossTonnageId.Value,
                    FuelTypeId = ship.FuelTypeId.Value,
                    FuelConsumption = form.ShipEnginePower.Value,
                    IsFishingMainActivity = form.IsFishingMainActivity.Value,
                    IsOwnerCrewMember = form.IsShipHolderPartOfCrew.Value,
                    OwnerCrewMemberPosition = form.ShipHolderPosition
                };

                Application application = (from appl in Db.Applications
                                           where appl.Id == entry.ApplicationId
                                           select appl).First();

                Db.AddOrEditApplicationSubmittedBy(application, form.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedFor(application, form.SubmittedFor);
                entry.SubmittedForPerson = application.SubmittedForPerson;
                entry.SubmittedForLegal = application.SubmittedForLegal;
                Db.SaveChanges();

                AddStatisticalFormFishVesselSeaDays(entry.FishVesselsForm, form.SeaDays);
                AddEmployeeCounts(entry, form.EmployeeInfoGroups);
                AddNumericStatValues(entry, form.NumStatGroups);

                if (form.Files != null)
                {
                    foreach (FileInfoDTO file in form.Files)
                    {
                        Db.AddOrEditFile(entry, entry.StatisticalFormsRegisterFiles, file);
                    }
                }

                Db.StatisticalFormsRegister.Add(entry);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> aquacultureFiles = form.Files;
            form.Files = null;
            stateMachine.Act(entry.ApplicationId, CommonUtils.Serialize(form), aquacultureFiles, nextManualStatus);

            return entry.Id;
        }

        public int AddStatisticalFormFishVessel(StatisticalFormFishVesselEditDTO form)
        {
            StatisticalFormsRegister entry;

            using (TransactionScope scope = new TransactionScope())
            {
                int registerApplicationId = (from st in Db.StatisticalFormsRegister
                                             where st.ApplicationId == form.ApplicationId
                                                  && st.RecordType == nameof(RecordTypesEnum.Application)
                                             select st.Id).Single();

                Dictionary<StatisticalFormTypesEnum, int> types = GetStatisticalFormTypesCodeToIdDictionary();

                entry = new StatisticalFormsRegister
                {
                    ApplicationId = form.ApplicationId.Value,
                    RegisterApplicationId = registerApplicationId,
                    RecordType = nameof(RecordTypesEnum.Register),
                    RegistrationDate = DateTime.Now,
                    ForYear = new DateTime(form.Year.Value, 1, 1),
                    StatisticalFormTypeId = types[StatisticalFormTypesEnum.FishVessel],
                    SubmitPersonWorkPosition = form.SubmittedByWorkPosition
                };

                StatisticalFormShipDTO ship = this.GetStatisticalFormShip(form.ShipId.Value);

                entry.FishVesselsForm = new FishVesselsForm
                {
                    StatisticalForm = entry,
                    ShipId = form.ShipId.Value,
                    ShipPrice = form.ShipPrice.Value,
                    LengthStatIntervalId = ship.ShipLenghtId.Value,
                    GrossTonnageIntervalId = ship.GrossTonnageId.Value,
                    FuelTypeId = ship.FuelTypeId.Value,
                    FuelConsumption = form.ShipEnginePower.Value,
                    IsFishingMainActivity = form.IsFishingMainActivity.Value,
                    IsOwnerCrewMember = form.IsShipHolderPartOfCrew.Value,
                    OwnerCrewMemberPosition = form.ShipHolderPosition
                };

                Db.StatisticalFormsRegister.Add(entry);
                Db.AddOrEditRegisterSubmittedFor(entry, form.SubmittedFor);
                Db.SaveChanges();

                AddStatisticalFormFishVesselSeaDays(entry.FishVesselsForm, form.SeaDays);
                AddEmployeeCounts(entry, form.EmployeeInfoGroups);
                AddNumericStatValues(entry, form.NumStatGroups);

                if (form.Files != null)
                {
                    foreach (FileInfoDTO file in form.Files)
                    {
                        Db.AddOrEditFile(entry, entry.StatisticalFormsRegisterFiles, file);
                    }
                }

                Db.SaveChanges();

                stateMachine.Act(entry.ApplicationId);

                scope.Complete();
            }

            return entry.Id;
        }

        public void EditStatisticalFormFishVesselApplication(StatisticalFormFishVesselApplicationEditDTO form, ApplicationStatusesEnum? manualStatus = null)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                StatisticalFormsRegister dbStatisticalForm = (from statForm in Db.StatisticalFormsRegister
                                                                    .AsSplitQuery()
                                                                    .Include(x => x.StatisticalFormsRegisterFiles)
                                                              where statForm.Id == form.Id
                                                              select statForm).First();

                FishVesselsForm dbFishVesselForm = (from fishVesselsForm in Db.FishVesselsForms
                                                    where fishVesselsForm.StatisticalFormId == dbStatisticalForm.Id
                                                    select fishVesselsForm).Single();

                dbStatisticalForm.SubmitPersonWorkPosition = form.SubmittedByWorkPosition;
                dbStatisticalForm.ForYear = new DateTime(form.Year.Value, 1, 1);
                dbFishVesselForm.ShipId = form.ShipId.Value;
                dbFishVesselForm.ShipPrice = form.ShipPrice.Value;
                dbFishVesselForm.IsFishingMainActivity = form.IsFishingMainActivity.Value;
                dbFishVesselForm.FuelConsumption = form.ShipEnginePower.Value;
                dbFishVesselForm.IsOwnerCrewMember = form.IsShipHolderPartOfCrew.Value;
                dbFishVesselForm.OwnerCrewMemberPosition = form.ShipHolderPosition;

                StatisticalFormShipDTO ship = GetStatisticalFormShip(form.ShipId.Value);
                dbFishVesselForm.GrossTonnageIntervalId = ship.GrossTonnageId.Value;
                dbFishVesselForm.LengthStatIntervalId = ship.ShipLenghtId.Value;
                dbFishVesselForm.FuelTypeId = ship.FuelTypeId.Value;

                EditStatisticalFormFishVesselSeaDays(dbFishVesselForm.Id, form.SeaDays);
                EditEmployeeCounts(dbStatisticalForm.Id, form.EmployeeInfoGroups);
                EditNumericStatValues(dbStatisticalForm.Id, form.NumStatGroups);

                application = (from appl in Db.Applications
                               where appl.Id == dbStatisticalForm.ApplicationId
                               select appl).First();

                Db.AddOrEditApplicationSubmittedBy(application, form.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedFor(application, form.SubmittedFor);
                Db.SaveChanges();

                if (form.Files != null)
                {
                    foreach (FileInfoDTO file in form.Files)
                    {
                        Db.AddOrEditFile(dbStatisticalForm, dbStatisticalForm.StatisticalFormsRegisterFiles, file);
                    }
                }

                scope.Complete();
            }

            List<FileInfoDTO> shipVesselFiles = form.Files;
            form.Files = null;
            stateMachine.Act(application.Id, CommonUtils.Serialize(form), shipVesselFiles, manualStatus, application.StatusReason);
        }

        public void EditStatisticalFormFishVesselApplicationRegixData(StatisticalFormFishVesselRegixDataDTO form)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                application = (from appl in Db.Applications
                               where appl.Id == form.ApplicationId.Value
                               select appl).First();

                Db.AddOrEditApplicationSubmittedByRegixData(application, form.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedForRegixData(application, form.SubmittedFor);
                Db.SaveChanges();

                scope.Complete();
            }

            stateMachine.Act(id: application.Id, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public void EditStatisticalFormFishVessel(StatisticalFormFishVesselEditDTO form)
        {
            using TransactionScope scope = new TransactionScope();

            StatisticalFormsRegister dbStatisticalForm = (from statForm in Db.StatisticalFormsRegister
                                                            .Include(x => x.StatisticalFormsRegisterFiles)
                                                          where statForm.ApplicationId == form.ApplicationId
                                                            && statForm.RecordType == nameof(RecordTypesEnum.Register)
                                                          select statForm).SingleOrDefault();

            FishVesselsForm dbFishVesselForm = (from fishVesselsForm in Db.FishVesselsForms
                                                where fishVesselsForm.StatisticalForm == dbStatisticalForm
                                                select fishVesselsForm).Single();

            Db.AddOrEditRegisterSubmittedFor(dbStatisticalForm, form.SubmittedFor);
            Db.SaveChanges();

            dbStatisticalForm.ForYear = new DateTime(form.Year.Value, 1, 1);
            dbFishVesselForm.ShipId = form.ShipId.Value;
            dbFishVesselForm.ShipPrice = form.ShipPrice.Value;
            dbFishVesselForm.IsFishingMainActivity = form.IsFishingMainActivity.Value;
            dbFishVesselForm.FuelConsumption = form.ShipEnginePower.Value;
            dbFishVesselForm.IsOwnerCrewMember = form.IsShipHolderPartOfCrew.Value;
            dbFishVesselForm.OwnerCrewMemberPosition = form.ShipHolderPosition;

            StatisticalFormShipDTO ship = this.GetStatisticalFormShip(form.ShipId.Value);
            dbFishVesselForm.GrossTonnageIntervalId = ship.GrossTonnageId.Value;
            dbFishVesselForm.LengthStatIntervalId = ship.ShipLenghtId.Value;
            dbFishVesselForm.FuelTypeId = ship.FuelTypeId.Value;

            EditStatisticalFormFishVesselSeaDays(dbFishVesselForm.Id, form.SeaDays);
            EditEmployeeCounts(dbStatisticalForm.Id, form.EmployeeInfoGroups);
            EditNumericStatValues(dbStatisticalForm.Id, form.NumStatGroups);

            if (form.Files != null)
            {
                foreach (FileInfoDTO file in form.Files)
                {
                    Db.AddOrEditFile(dbStatisticalForm, dbStatisticalForm.StatisticalFormsRegisterFiles, file);
                }
            }

            Db.SaveChanges();

            scope.Complete();
        }

        private ApplicationSubmittedByDTO GetUserAsSubmittedByForFishVessel(int userId)
        {
            int personId = (from user in Db.Users
                            where user.Id == userId
                            select user.PersonId).First();

            RegixPersonDataDTO person = personService.GetRegixPersonData(personId);

            ApplicationSubmittedByDTO result = new ApplicationSubmittedByDTO
            {
                Person = person,
                Addresses = new List<AddressRegistrationDTO>()
            };

            return result;
        }

        private int GetGrossTonnageInterval(decimal grossTonnage)
        {
            GrossTonnageIntervalsEnum interval;

            if (grossTonnage < 2)
            {
                interval = GrossTonnageIntervalsEnum.Under2;
            }
            else if (grossTonnage >= 2 && grossTonnage < 10)
            {
                interval = GrossTonnageIntervalsEnum.Over2;
            }
            else if (grossTonnage >= 10 && grossTonnage < 30)
            {
                interval = GrossTonnageIntervalsEnum.Over10;
            }
            else if (grossTonnage >= 30 && grossTonnage < 60)
            {
                interval = GrossTonnageIntervalsEnum.Over30;
            }
            else
            {
                interval = GrossTonnageIntervalsEnum.Over60;
            }

            int result = (from tonnageInterval in Db.NgrossTonageStatIntervals
                          where tonnageInterval.Code == interval.ToString()
                          select tonnageInterval.Id).Single();

            return result;
        }

        private int GetShipLengthInterval(decimal length)
        {
            VesselLengthIntervalsEnum interval;

            if (length < 6)
            {
                interval = VesselLengthIntervalsEnum.Under6;
            }
            else if (length >= 6 && length < 12)
            {
                interval = VesselLengthIntervalsEnum.Over6;
            }
            else if (length >= 12 && length < 18)
            {
                interval = VesselLengthIntervalsEnum.Over12;
            }
            else if (length >= 18 && length < 24)
            {
                interval = VesselLengthIntervalsEnum.Over18;
            }
            else
            {
                interval = VesselLengthIntervalsEnum.Over24;
            }

            int result = (from lengthInterval in Db.NvesselLengthStatIntervals
                          where lengthInterval.Code == interval.ToString()
                          select lengthInterval.Id).Single();

            return result;
        }

        private List<StatisticalFormsSeaDaysDTO> GetStatisticalFormFishVesselSeaDays(int shipVesselId)
        {
            List<StatisticalFormsSeaDaysDTO> result = (from shipVessel in Db.FishVesselsForms
                                                       join seaDay in Db.VesselDaysAtSeas on shipVessel.Id equals seaDay.VesselFormId
                                                       where shipVessel.Id == shipVesselId
                                                       select new StatisticalFormsSeaDaysDTO
                                                       {
                                                           Id = seaDay.Id,
                                                           FishingGearId = seaDay.FishingGearId,
                                                           Days = seaDay.DaysAtSea,
                                                           IsActive = seaDay.IsActive
                                                       }).ToList();
            return result;
        }

        private void AddStatisticalFormFishVesselSeaDays(FishVesselsForm fishVessel, List<StatisticalFormsSeaDaysDTO> seaDays)
        {
            if (seaDays != null)
            {
                foreach (StatisticalFormsSeaDaysDTO seaDay in seaDays)
                {
                    VesselDaysAtSea entry = new VesselDaysAtSea
                    {
                        VesselForm = fishVessel,
                        FishingGearId = seaDay.FishingGearId.Value,
                        DaysAtSea = seaDay.Days.Value,
                        IsActive = seaDay.IsActive.Value
                    };

                    Db.VesselDaysAtSeas.Add(entry);
                }
            }
        }

        private void EditStatisticalFormFishVesselSeaDays(int fishVesselId, List<StatisticalFormsSeaDaysDTO> seaDays)
        {
            List<VesselDaysAtSea> dbEntries = (from dayAtSea in Db.VesselDaysAtSeas
                                               where dayAtSea.VesselFormId == fishVesselId
                                               select dayAtSea).ToList();

            if (seaDays != null)
            {
                foreach (StatisticalFormsSeaDaysDTO seaDay in seaDays)
                {
                    if (seaDay.Id.HasValue)
                    {
                        VesselDaysAtSea dBEntry = dbEntries.Where(x => x.Id == seaDay.Id.Value).Single();
                        dBEntry.FishingGearId = seaDay.FishingGearId.Value;
                        dBEntry.DaysAtSea = seaDay.Days.Value;
                        dBEntry.IsActive = seaDay.IsActive.Value;
                    }
                    else
                    {
                        VesselDaysAtSea entry = new VesselDaysAtSea
                        {
                            VesselFormId = fishVesselId,
                            FishingGearId = seaDay.FishingGearId.Value,
                            DaysAtSea = seaDay.Days.Value,
                            IsActive = seaDay.IsActive.Value
                        };

                        Db.VesselDaysAtSeas.Add(entry);
                    }
                }
            }
            else
            {
                foreach (VesselDaysAtSea entry in dbEntries)
                {
                    entry.IsActive = false;
                }
            }
        }
    }
}
