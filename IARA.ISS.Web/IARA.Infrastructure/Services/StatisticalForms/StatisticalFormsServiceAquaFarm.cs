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
using IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class StatisticalFormsService : Service, IStatisticalFormsService
    {
        public StatisticalFormAquacultureDTO GetStatisticalFormAquaculture(int aquacultureId)
        {
            DateTime now = DateTime.Now;

            StatisticalFormAquacultureDTO result = (from aqua in Db.AquacultureFacilitiesRegister
                                                    join legal in Db.Legals on aqua.SubmittedForLegalId equals legal.Id
                                                    where aqua.Id == aquacultureId
                                                    select new StatisticalFormAquacultureDTO
                                                    {
                                                        AquacultureId = aqua.Id,
                                                        LegalName = legal.Name,
                                                        Eik = legal.Eik,
                                                        SystemType = Enum.Parse<AquacultureSystemEnum>(aqua.System)
                                                    }).First();

            HashSet<int> installationTypeIds = (from inst in Db.AquacultureFacilityInstallations
                                                where inst.AquacultureFacilityId == result.AquacultureId
                                                select inst.InstallationTypeId).ToHashSet();

            result.FacilityInstalations = (from instalationType in Db.NaquacultureInstallationTypes
                                           where installationTypeIds.Contains(instalationType.Id)
                                           select new NomenclatureDTO
                                           {
                                               Value = instalationType.Id,
                                               Code = instalationType.Code,
                                               DisplayName = instalationType.Name,
                                               IsActive = instalationType.ValidFrom <= now && instalationType.ValidTo > now
                                           }).ToList();

            result.FishTypes = (from aquaFish in Db.AquacultureFacilityFishes
                                join fish in Db.Nfishes on aquaFish.FishTypeId equals fish.Id
                                where aquaFish.AquacultureFacilityId == result.AquacultureId
                                select new NomenclatureDTO
                                {
                                    Value = fish.Id,
                                    Code = fish.Code,
                                    DisplayName = fish.Name,
                                    IsActive = aquaFish.IsActive
                                }).ToList();

            List<int> aquaFishIds = result.FishTypes.Select(x => x.Value).ToList();

            List<NomenclatureDTO> logbookFishTypes = (from logbook in Db.LogBooks
                                                      join page in Db.AquacultureLogBookPages on logbook.Id equals page.LogBookId
                                                      join product in Db.LogBookPageProducts on page.Id equals product.AquacultureLogBookPageId
                                                      join fish in Db.Nfishes on product.FishId equals fish.Id
                                                      where logbook.AquacultureFacilityId == aquacultureId
                                                            && !aquaFishIds.Contains(fish.Id)
                                                      select new NomenclatureDTO
                                                      {
                                                          Value = fish.Id,
                                                          Code = fish.Code,
                                                          DisplayName = fish.Name,
                                                          IsActive = product.IsActive && page.IsActive
                                                      }).ToList();

            result.FishTypes = result.FishTypes.Concat(logbookFishTypes).OrderBy(x => x.DisplayName).ToList();

            return result;
        }

        public StatisticalFormAquaFarmApplicationEditDTO GetStatisticalFormAquaFarmApplication(int applicationId)
        {
            StatisticalFormsRegister dbForm = (from statForm in Db.StatisticalFormsRegister
                                               where statForm.ApplicationId == applicationId
                                                 && statForm.RecordType == nameof(RecordTypesEnum.Application)
                                               select statForm).SingleOrDefault();

            StatisticalFormAquaFarmApplicationEditDTO result = null;

            int formTypeId = (from type in Db.NstatisticalFormTypes
                              where type.Code == nameof(StatisticalFormTypesEnum.AquaFarm)
                              select type.Id).Single();

            if (dbForm == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<StatisticalFormAquaFarmApplicationEditDTO>(draft);
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
                    result = new StatisticalFormAquaFarmApplicationEditDTO
                    {
                        EmployeeInfoGroups = GetEmployeeCountGroups(null, formTypeId),
                        NumStatGroups = GetNumericStatValueGroups(null, formTypeId),
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };
                }
            }
            else
            {
                AquacutlureForm dbAquaFarmForm = (from aquaFarm in Db.AquacutlureForms
                                                  where aquaFarm.StatisticalFormId == dbForm.Id
                                                  select aquaFarm).Single();

                result = new StatisticalFormAquaFarmApplicationEditDTO
                {
                    Id = dbAquaFarmForm.Id,
                    ApplicationId = dbForm.ApplicationId,
                    SubmittedByWorkPosition = dbForm.SubmitPersonWorkPosition,
                    AquacultureFacilityId = dbAquaFarmForm.AquacultureFacilityId,
                    Year = dbForm.ForYear.Year,
                    BreedingMaterialDeathRate = dbAquaFarmForm.BreedingMaterialDeathRate,
                    ConsumationFishDeathRate = dbAquaFarmForm.ConsumationFishDeathRate,
                    BroodstockDeathRate = dbAquaFarmForm.BroodstockDeathRate,
                    MedicineComments = dbAquaFarmForm.GivenMedicineNotes
                };

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);

                result.Medicine = GetStatisticalFormGivenMedicine(dbAquaFarmForm.Id);

                result.ProducedFishOrganism = GetStatisticalFormAquaFarmFishOrganism(dbAquaFarmForm.Id, AquaFarmFishOrganismReportTypeEnum.Produced);
                result.SoldFishOrganism = GetStatisticalFormAquaFarmFishOrganism(dbAquaFarmForm.Id, AquaFarmFishOrganismReportTypeEnum.Sold);
                result.UnrealizedFishOrganism = GetStatisticalFormAquaFarmFishOrganism(dbAquaFarmForm.Id, AquaFarmFishOrganismReportTypeEnum.Unrealized);

                result.Broodstock = GetStatisticalFormAquaFarmBroodstock(dbAquaFarmForm.Id);

                result.InstallationSystemFull = GetStatisticalFormAquaFarmInstallationSystemFull(dbAquaFarmForm.Id);
                result.InstallationSystemNotFull = GetStatisticalFormAquaFarmInstallationSystemNotFull(dbAquaFarmForm.Id);

                result.EmployeeInfoGroups = GetEmployeeCountGroups(dbForm.Id, dbForm.StatisticalFormTypeId);
                result.NumStatGroups = GetNumericStatValueGroups(dbForm.Id, dbForm.StatisticalFormTypeId);

                result.Files = Db.GetFiles(Db.StatisticalFormsRegisterFiles, dbForm.Id);
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);
            }

            return result;
        }

        public RegixChecksWrapperDTO<StatisticalFormAquaFarmRegixDataDTO> GetStatisticalFormAquaFarmRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<StatisticalFormAquaFarmRegixDataDTO> result = new RegixChecksWrapperDTO<StatisticalFormAquaFarmRegixDataDTO>
            {
                DialogDataModel = GetApplicationAquaFarmRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetStatisticalFormAquaFarmChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public StatisticalFormAquaFarmRegixDataDTO GetApplicationAquaFarmRegixData(int applicationId)
        {
            StatisticalFormApplicationDataIds data = GetApplicationDataIds(applicationId);

            StatisticalFormAquaFarmRegixDataDTO regixData = new StatisticalFormAquaFarmRegixDataDTO
            {
                Id = data.StatisticalFormId,
                ApplicationId = applicationId,
                PageCode = data.PageCode
            };

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId);

            return regixData;
        }

        public StatisticalFormAquaFarmEditDTO GetApplicationAquaFarmDataForRegister(int applicationId)
        {
            var data = (from statForm in Db.StatisticalFormsRegister
                        join aquaForm in Db.AquacutlureForms on statForm.Id equals aquaForm.StatisticalFormId
                        where statForm.ApplicationId == applicationId
                           && statForm.RecordType == nameof(RecordTypesEnum.Application)
                        select new
                        {
                            statForm.Id,
                            statForm.SubmittedForPersonId,
                            statForm.SubmittedForLegalId,
                            statForm.StatisticalFormTypeId,
                            AquaForm = new StatisticalFormAquaFarmEditDTO
                            {
                                Id = aquaForm.Id,
                                ApplicationId = statForm.ApplicationId,
                                AquacultureFacilityId = aquaForm.AquacultureFacilityId,
                                Year = statForm.ForYear.Year,
                                BreedingMaterialDeathRate = aquaForm.BreedingMaterialDeathRate,
                                ConsumationFishDeathRate = aquaForm.ConsumationFishDeathRate,
                                BroodstockDeathRate = aquaForm.BroodstockDeathRate,
                                MedicineComments = aquaForm.GivenMedicineNotes
                            }
                        }).Single();

            StatisticalFormAquaFarmEditDTO result = data.AquaForm;

            result.SubmittedFor = applicationService.GetRegisterSubmittedFor(applicationId, data.SubmittedForPersonId, data.SubmittedForLegalId);
            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

            result.Medicine = GetStatisticalFormGivenMedicine(result.Id.Value);

            result.ProducedFishOrganism = GetStatisticalFormAquaFarmFishOrganism(result.Id.Value, AquaFarmFishOrganismReportTypeEnum.Produced);
            result.SoldFishOrganism = GetStatisticalFormAquaFarmFishOrganism(result.Id.Value, AquaFarmFishOrganismReportTypeEnum.Sold);
            result.UnrealizedFishOrganism = GetStatisticalFormAquaFarmFishOrganism(result.Id.Value, AquaFarmFishOrganismReportTypeEnum.Unrealized);

            result.Broodstock = GetStatisticalFormAquaFarmBroodstock(result.Id.Value);

            result.InstallationSystemFull = GetStatisticalFormAquaFarmInstallationSystemFull(result.Id.Value);
            result.InstallationSystemNotFull = GetStatisticalFormAquaFarmInstallationSystemNotFull(result.Id.Value);

            result.NumStatGroups = GetNumericStatValueGroups(data.Id, data.StatisticalFormTypeId);
            result.EmployeeInfoGroups = GetEmployeeCountGroups(data.Id, data.StatisticalFormTypeId);

            result.Files = Db.GetFiles(Db.StatisticalFormsRegisterFiles, data.Id);

            return result;
        }

        public StatisticalFormAquaFarmEditDTO GetAquaFarmRegisterByApplicationId(int applicationId)
        {
            int id = (from statForm in Db.StatisticalFormsRegister
                      where statForm.ApplicationId == applicationId
                        && statForm.RecordType == nameof(RecordTypesEnum.Register)
                      select statForm.Id).SingleOrDefault();

            return GetStatisticalFormAquaFarm(id);
        }

        public StatisticalFormAquaFarmEditDTO GetStatisticalFormAquaFarm(int id)
        {
            var data = (from statForm in Db.StatisticalFormsRegister
                        join aquaForm in Db.AquacutlureForms on statForm.Id equals aquaForm.StatisticalFormId
                        where statForm.Id == id
                           && statForm.RecordType == nameof(RecordTypesEnum.Register)
                        select new
                        {
                            statForm.Id,
                            statForm.SubmittedForPersonId,
                            statForm.SubmittedForLegalId,
                            statForm.StatisticalFormTypeId,
                            AquaForm = new StatisticalFormAquaFarmEditDTO
                            {
                                Id = aquaForm.Id,
                                ApplicationId = aquaForm.StatisticalForm.ApplicationId,
                                AquacultureFacilityId = aquaForm.AquacultureFacilityId,
                                Year = statForm.ForYear.Year,
                                FormNum = statForm.RegistrationNum,
                                BreedingMaterialDeathRate = aquaForm.BreedingMaterialDeathRate,
                                ConsumationFishDeathRate = aquaForm.ConsumationFishDeathRate,
                                BroodstockDeathRate = aquaForm.BroodstockDeathRate,
                                MedicineComments = aquaForm.GivenMedicineNotes
                            }
                        }).Single();

            StatisticalFormAquaFarmEditDTO result = data.AquaForm;

            result.SubmittedFor = applicationService.GetRegisterSubmittedFor(result.ApplicationId.Value, data.SubmittedForPersonId, data.SubmittedForLegalId);
            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(result.ApplicationId.Value, ApplicationHierarchyTypesEnum.Online);

            result.Medicine = GetStatisticalFormGivenMedicine(result.Id.Value);

            result.ProducedFishOrganism = GetStatisticalFormAquaFarmFishOrganism(result.Id.Value, AquaFarmFishOrganismReportTypeEnum.Produced);
            result.SoldFishOrganism = GetStatisticalFormAquaFarmFishOrganism(result.Id.Value, AquaFarmFishOrganismReportTypeEnum.Sold);
            result.UnrealizedFishOrganism = GetStatisticalFormAquaFarmFishOrganism(result.Id.Value, AquaFarmFishOrganismReportTypeEnum.Unrealized);

            result.Broodstock = GetStatisticalFormAquaFarmBroodstock(result.Id.Value);

            result.InstallationSystemFull = GetStatisticalFormAquaFarmInstallationSystemFull(result.Id.Value);
            result.InstallationSystemNotFull = GetStatisticalFormAquaFarmInstallationSystemNotFull(result.Id.Value);

            result.NumStatGroups = GetNumericStatValueGroups(data.Id, data.StatisticalFormTypeId);
            result.EmployeeInfoGroups = GetEmployeeCountGroups(data.Id, data.StatisticalFormTypeId);

            result.Files = Db.GetFiles(Db.StatisticalFormsRegisterFiles, data.Id);

            return result;
        }

        public int AddStatisticalFormAquaFarmApplication(StatisticalFormAquaFarmApplicationEditDTO form, ApplicationStatusesEnum? nextManualStatus = null)
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
                    StatisticalFormTypeId = types[StatisticalFormTypesEnum.AquaFarm]
                };

                entry.AquacutlureForm = new AquacutlureForm
                {
                    StatisticalForm = entry,
                    AquacultureFacilityId = form.AquacultureFacilityId,
                    BreedingMaterialDeathRate = form.BreedingMaterialDeathRate,
                    ConsumationFishDeathRate = form.ConsumationFishDeathRate,
                    BroodstockDeathRate = form.BroodstockDeathRate,
                    GivenMedicineNotes = form.MedicineComments
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

                AddStatisticalFormGivenMedicine(entry.AquacutlureForm, form.Medicine);
                AddStatisticalFormAquaFarmBroodstock(entry.AquacutlureForm, form.Broodstock);
                AddStatisticalFormAquaFarmFishOrganism(entry.AquacutlureForm, form.ProducedFishOrganism, AquaFarmFishOrganismReportTypeEnum.Produced);
                AddStatisticalFormAquaFarmFishOrganism(entry.AquacutlureForm, form.SoldFishOrganism, AquaFarmFishOrganismReportTypeEnum.Sold);
                AddStatisticalFormAquaFarmFishOrganism(entry.AquacutlureForm, form.UnrealizedFishOrganism, AquaFarmFishOrganismReportTypeEnum.Unrealized);
                AddStatisticalFormAquaFarmFullSystemInstallations(entry.AquacutlureForm, form.InstallationSystemFull);
                AddStatisticalFormAquaFarmNotFullSystemInstallations(entry.AquacutlureForm, form.InstallationSystemNotFull);
                AddNumericStatValues(entry, form.NumStatGroups);
                AddEmployeeCounts(entry, form.EmployeeInfoGroups);

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

        public int AddStatisticalFormAquaFarm(StatisticalFormAquaFarmEditDTO form)
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
                    StatisticalFormTypeId = types[StatisticalFormTypesEnum.AquaFarm]
                };

                entry.AquacutlureForm = new AquacutlureForm
                {
                    StatisticalForm = entry,
                    AquacultureFacilityId = form.AquacultureFacilityId,
                    BreedingMaterialDeathRate = form.BreedingMaterialDeathRate,
                    ConsumationFishDeathRate = form.ConsumationFishDeathRate,
                    BroodstockDeathRate = form.BroodstockDeathRate,
                    GivenMedicineNotes = form.MedicineComments
                };

                Db.StatisticalFormsRegister.Add(entry);

                Db.AddOrEditRegisterSubmittedFor(entry, form.SubmittedFor);
                Db.SaveChanges();

                AddStatisticalFormGivenMedicine(entry.AquacutlureForm, form.Medicine);
                AddStatisticalFormAquaFarmBroodstock(entry.AquacutlureForm, form.Broodstock);
                AddStatisticalFormAquaFarmFishOrganism(entry.AquacutlureForm, form.ProducedFishOrganism, AquaFarmFishOrganismReportTypeEnum.Produced);
                AddStatisticalFormAquaFarmFishOrganism(entry.AquacutlureForm, form.SoldFishOrganism, AquaFarmFishOrganismReportTypeEnum.Sold);
                AddStatisticalFormAquaFarmFishOrganism(entry.AquacutlureForm, form.UnrealizedFishOrganism, AquaFarmFishOrganismReportTypeEnum.Unrealized);
                AddStatisticalFormAquaFarmFullSystemInstallations(entry.AquacutlureForm, form.InstallationSystemFull);
                AddStatisticalFormAquaFarmNotFullSystemInstallations(entry.AquacutlureForm, form.InstallationSystemNotFull);
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

        public void EditStatisticalFormAquaFarmApplication(StatisticalFormAquaFarmApplicationEditDTO form, ApplicationStatusesEnum? manualStatus = null)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {

                StatisticalFormsRegister dbStatisticalForm = (from statForm in Db.StatisticalFormsRegister
                                                                  .AsSplitQuery()
                                                                  .Include(x => x.StatisticalFormsRegisterFiles)
                                                              where statForm.Id == form.Id
                                                              select statForm).SingleOrDefault();

                AquacutlureForm dbAquaFarmForm = (from aquaFarm in Db.AquacutlureForms
                                                  where aquaFarm.StatisticalFormId == dbStatisticalForm.Id
                                                  select aquaFarm).Single();

                dbStatisticalForm.ForYear = new DateTime(form.Year.Value, 1, 1);
                dbStatisticalForm.SubmitPersonWorkPosition = form.SubmittedByWorkPosition;
                dbAquaFarmForm.BreedingMaterialDeathRate = form.BreedingMaterialDeathRate;
                dbAquaFarmForm.ConsumationFishDeathRate = form.ConsumationFishDeathRate;
                dbAquaFarmForm.BroodstockDeathRate = form.BroodstockDeathRate;
                dbAquaFarmForm.AquacultureFacilityId = form.AquacultureFacilityId;
                dbAquaFarmForm.GivenMedicineNotes = form.MedicineComments;

                EditStatisticalFormGivenMedicine(dbAquaFarmForm.Id, form.Medicine);
                EditStatisticalFormAquaFarmBroodstock(dbAquaFarmForm.Id, form.Broodstock);
                EditStatisticalFormAquaFarmFishOrganism(dbAquaFarmForm.Id, form.ProducedFishOrganism, AquaFarmFishOrganismReportTypeEnum.Produced);
                EditStatisticalFormAquaFarmFishOrganism(dbAquaFarmForm.Id, form.SoldFishOrganism, AquaFarmFishOrganismReportTypeEnum.Sold);
                EditStatisticalFormAquaFarmFishOrganism(dbAquaFarmForm.Id, form.UnrealizedFishOrganism, AquaFarmFishOrganismReportTypeEnum.Unrealized);
                EditStatisticalFormAquaFarmFullSystemInstallations(dbAquaFarmForm.Id, form.InstallationSystemFull);
                EditStatisticalFormAquaFarmNotFullSystemInstallations(dbAquaFarmForm.Id, form.InstallationSystemNotFull);
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

            List<FileInfoDTO> aquaFarmFiles = form.Files;
            form.Files = null;
            stateMachine.Act(application.Id, CommonUtils.Serialize(form), aquaFarmFiles, manualStatus, application.StatusReason);
        }

        public void EditStatisticalFormAquaFarmApplicationRegixData(StatisticalFormAquaFarmRegixDataDTO form)
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

        public void EditStatisticalFormAquaFarm(StatisticalFormAquaFarmEditDTO form)
        {
            using TransactionScope scope = new TransactionScope();

            StatisticalFormsRegister dbStatisticalForm = (from statForm in Db.StatisticalFormsRegister
                                                            .Include(x => x.StatisticalFormsRegisterFiles)
                                                          where statForm.ApplicationId == form.ApplicationId
                                                            && statForm.RecordType == nameof(RecordTypesEnum.Register)
                                                          select statForm).SingleOrDefault();

            AquacutlureForm dbAquaFarmForm = (from aquaFarm in Db.AquacutlureForms
                                              where aquaFarm.StatisticalForm == dbStatisticalForm
                                              select aquaFarm).SingleOrDefault();

            Db.AddOrEditRegisterSubmittedFor(dbStatisticalForm, form.SubmittedFor);
            Db.SaveChanges();

            dbAquaFarmForm.AquacultureFacilityId = form.AquacultureFacilityId;
            dbAquaFarmForm.GivenMedicineNotes = form.MedicineComments;
            dbAquaFarmForm.BreedingMaterialDeathRate = form.BreedingMaterialDeathRate;
            dbAquaFarmForm.ConsumationFishDeathRate = form.ConsumationFishDeathRate;
            dbAquaFarmForm.BroodstockDeathRate = form.BroodstockDeathRate;
            dbStatisticalForm.ForYear = new DateTime(form.Year.Value, 1, 1);

            EditStatisticalFormGivenMedicine(dbAquaFarmForm.Id, form.Medicine);
            EditStatisticalFormAquaFarmBroodstock(dbAquaFarmForm.Id, form.Broodstock);
            EditStatisticalFormAquaFarmFishOrganism(dbAquaFarmForm.Id, form.ProducedFishOrganism, AquaFarmFishOrganismReportTypeEnum.Produced);
            EditStatisticalFormAquaFarmFishOrganism(dbAquaFarmForm.Id, form.SoldFishOrganism, AquaFarmFishOrganismReportTypeEnum.Sold);
            EditStatisticalFormAquaFarmFishOrganism(dbAquaFarmForm.Id, form.UnrealizedFishOrganism, AquaFarmFishOrganismReportTypeEnum.Unrealized);
            EditStatisticalFormAquaFarmFullSystemInstallations(dbAquaFarmForm.Id, form.InstallationSystemFull);
            EditStatisticalFormAquaFarmNotFullSystemInstallations(dbAquaFarmForm.Id, form.InstallationSystemNotFull);
            EditEmployeeCounts(dbAquaFarmForm.StatisticalFormId, form.EmployeeInfoGroups);
            EditNumericStatValues(dbAquaFarmForm.StatisticalFormId, form.NumStatGroups);

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

        private ApplicationSubmittedByDTO GetUserAsSubmittedByForAquaFarm(int userId)
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

        private StatisticalFormApplicationDataIds GetApplicationDataIds(int applicationId)
        {
            var data = (from statForm in Db.StatisticalFormsRegister
                        join appl in Db.Applications on statForm.ApplicationId equals appl.Id
                        join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                        where statForm.ApplicationId == applicationId
                            && statForm.RecordType == nameof(RecordTypesEnum.Application)
                        select new StatisticalFormApplicationDataIds
                        {
                            StatisticalFormId = statForm.Id,
                            PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode)
                        }).Single();

            return data;
        }

        private List<StatisticalFormGivenMedicineDTO> GetStatisticalFormGivenMedicine(int aquaFarmId)
        {
            List<StatisticalFormGivenMedicineDTO> result = (from aquaFarm in Db.AquacutlureForms
                                                            join medicine in Db.AquacutlureFormGivenMedicines on aquaFarm.Id equals medicine.AquacultureFormId
                                                            where aquaFarm.Id == aquaFarmId
                                                            select new StatisticalFormGivenMedicineDTO
                                                            {
                                                                Id = medicine.Id,
                                                                Grams = medicine.GivenGrams,
                                                                MedicineType = medicine.Medicine,
                                                                IsActive = medicine.IsActive
                                                            }).ToList();

            return result;
        }

        private List<StatisticalFormAquaFarmBroodstockDTO> GetStatisticalFormAquaFarmBroodstock(int aquaFarmId)
        {
            List<StatisticalFormAquaFarmBroodstockDTO> result = (from aquaFarm in Db.AquacutlureForms
                                                                 join broodstock in Db.AquacutlureFormBroodstocks on aquaFarm.Id equals broodstock.AquacultureFormId
                                                                 where aquaFarm.Id == aquaFarmId
                                                                 select new StatisticalFormAquaFarmBroodstockDTO
                                                                 {
                                                                     Id = broodstock.Id,
                                                                     InstallationTypeId = broodstock.InstallationTypeId,
                                                                     FishTypeId = broodstock.FishId,
                                                                     FemaleAge = broodstock.FemaleAge,
                                                                     MaleAge = broodstock.MaleAge,
                                                                     FemaleCount = broodstock.FemaleCount,
                                                                     MaleCount = broodstock.MaleCount,
                                                                     FemaleWeight = broodstock.FemaleWeight,
                                                                     MaleWeight = broodstock.MaleWeight,
                                                                     IsActive = broodstock.IsActive
                                                                 }).ToList();

            return result;
        }

        private List<StatisticalFormAquaFarmInstallationSystemFullDTO> GetStatisticalFormAquaFarmInstallationSystemFull(int aquaFarmId)
        {
            List<StatisticalFormAquaFarmInstallationSystemFullDTO> result = (from aquaFarm in Db.AquacutlureForms
                                                                             join installationSystem in Db.AquacultureFormFullSystemInstallations on aquaFarm.Id equals installationSystem.AquacultureFormId
                                                                             where aquaFarm.Id == aquaFarmId
                                                                             select new StatisticalFormAquaFarmInstallationSystemFullDTO
                                                                             {
                                                                                 Id = installationSystem.Id,
                                                                                 InstallationTypeId = installationSystem.InstallationTypeId,
                                                                                 IsInstallationUsed = installationSystem.IsInstallationUsed,
                                                                                 IsActive = installationSystem.IsActive
                                                                             }).ToList();

            return result;
        }

        private List<StatisticalFormAquaFarmInstallationSystemNotFullDTO> GetStatisticalFormAquaFarmInstallationSystemNotFull(int aquaFarmId)
        {
            List<StatisticalFormAquaFarmInstallationSystemNotFullDTO> result = (from aquaFarm in Db.AquacutlureForms
                                                                                join installationSystem in Db.AquacultureFormNonFullSystemInstallations on aquaFarm.Id equals installationSystem.AquacultureFormId
                                                                                where aquaFarm.Id == aquaFarmId
                                                                                select new StatisticalFormAquaFarmInstallationSystemNotFullDTO
                                                                                {
                                                                                    Id = installationSystem.Id,
                                                                                    InstallationTypeId = installationSystem.InstallationTypeId,
                                                                                    IsInstallationUsedBreedingMaterial = installationSystem.IsUsedForBreedingMaterial,
                                                                                    IsInstallationUsedConsumationFish = installationSystem.IsUsedForConsumationFish,
                                                                                    IsActive = installationSystem.IsActive
                                                                                }).ToList();

            return result;
        }

        private List<StatisticalFormAquaFarmFishOrganismDTO> GetStatisticalFormAquaFarmFishOrganism(int aquaFarmId, AquaFarmFishOrganismReportTypeEnum reportType)
        {
            List<StatisticalFormAquaFarmFishOrganismDTO> result = (from aquaFarm in Db.AquacutlureForms
                                                                   join material in Db.AquacutlureFormStockingMaterials on aquaFarm.Id equals material.AquacultureFormId
                                                                   where aquaFarm.Id == aquaFarmId
                                                                        && material.ReportType == reportType.ToString()
                                                                   select new StatisticalFormAquaFarmFishOrganismDTO
                                                                   {
                                                                       Id = material.Id,
                                                                       InstallationTypeId = material.InstallationTypeId,
                                                                       ReportType = reportType,
                                                                       FishTypeId = material.FishId,
                                                                       FishLarvaeCount = material.FishLarvaeCount,
                                                                       ForConsumption = material.ForConsumptionWeight,
                                                                       CaviarForConsumption = material.CaviarForConsumptionWeight,
                                                                       OneStripBreedingMaterialCount = material.OneStripBreedingMaterialCount,
                                                                       OneStripBreedingMaterialWeight = material.OneStripBreedingMaterialWeight,
                                                                       OneYearBreedingMaterialCount = material.OneYearBreedingMaterialCount,
                                                                       OneYearBreedingMaterialWeight = material.OneYearBreedingMaterialWeight,
                                                                       IsActive = material.IsActive
                                                                   }).ToList();

            return result;
        }

        private void AddStatisticalFormGivenMedicine(AquacutlureForm aquaFarm, List<StatisticalFormGivenMedicineDTO> givenMedicine)
        {
            if (givenMedicine != null)
            {
                foreach (StatisticalFormGivenMedicineDTO medicine in givenMedicine)
                {
                    AquacutlureFormGivenMedicine entry = new AquacutlureFormGivenMedicine
                    {
                        AquacultureForm = aquaFarm,
                        GivenGrams = medicine.Grams.Value,
                        Medicine = medicine.MedicineType,
                        IsActive = medicine.IsActive.Value
                    };

                    Db.AquacutlureFormGivenMedicines.Add(entry);
                }
            }
        }

        private void EditStatisticalFormGivenMedicine(int aquaFarmId, List<StatisticalFormGivenMedicineDTO> givenMedicine)
        {
            List<AquacutlureFormGivenMedicine> dbEntries = (from aquaFarmMedicine in Db.AquacutlureFormGivenMedicines
                                                            where aquaFarmMedicine.AquacultureFormId == aquaFarmId
                                                            select aquaFarmMedicine).ToList();

            if (givenMedicine != null)
            {
                foreach (StatisticalFormGivenMedicineDTO medicine in givenMedicine)
                {
                    if (medicine.Id.HasValue)
                    {
                        AquacutlureFormGivenMedicine dbEntry = dbEntries.Where(x => x.Id == medicine.Id.Value).Single();
                        dbEntry.GivenGrams = medicine.Grams;
                        dbEntry.Medicine = medicine.MedicineType;
                        dbEntry.IsActive = medicine.IsActive.Value;
                    }
                    else
                    {
                        AquacutlureFormGivenMedicine entry = new AquacutlureFormGivenMedicine
                        {
                            AquacultureFormId = aquaFarmId,
                            GivenGrams = medicine.Grams.Value,
                            Medicine = medicine.MedicineType,
                            IsActive = medicine.IsActive.Value
                        };

                        Db.AquacutlureFormGivenMedicines.Add(entry);
                    }
                }
            }
            else
            {
                foreach (AquacutlureFormGivenMedicine entry in dbEntries)
                {
                    entry.IsActive = false;
                }
            }
        }

        private void AddStatisticalFormAquaFarmFullSystemInstallations(AquacutlureForm aquaFarm, List<StatisticalFormAquaFarmInstallationSystemFullDTO> systems)
        {
            if (systems != null)
            {
                foreach (StatisticalFormAquaFarmInstallationSystemFullDTO system in systems)
                {
                    AquacultureFormFullSystemInstallation entry = new AquacultureFormFullSystemInstallation
                    {
                        AquacultureForm = aquaFarm,
                        InstallationTypeId = system.InstallationTypeId.Value,
                        IsInstallationUsed = system.IsInstallationUsed.Value,
                        IsActive = system.IsActive.Value
                    };

                    Db.AquacultureFormFullSystemInstallations.Add(entry);
                }
            }
        }

        private void EditStatisticalFormAquaFarmFullSystemInstallations(int aquaFarmId, List<StatisticalFormAquaFarmInstallationSystemFullDTO> systems)
        {
            List<AquacultureFormFullSystemInstallation> dbEntries = (from aquaFarmSystem in Db.AquacultureFormFullSystemInstallations
                                                                     where aquaFarmSystem.AquacultureFormId == aquaFarmId
                                                                     select aquaFarmSystem).ToList();

            if (systems != null)
            {
                foreach (StatisticalFormAquaFarmInstallationSystemFullDTO system in systems)
                {
                    if (system.Id.HasValue)
                    {
                        AquacultureFormFullSystemInstallation dbEntry = dbEntries.Where(x => x.Id == system.Id.Value).Single();
                        dbEntry.IsInstallationUsed = system.IsInstallationUsed.Value;
                        dbEntry.InstallationTypeId = system.InstallationTypeId.Value;
                        dbEntry.IsActive = system.IsActive.Value;
                    }
                    else
                    {
                        AquacultureFormFullSystemInstallation entry = new AquacultureFormFullSystemInstallation
                        {
                            AquacultureFormId = aquaFarmId,
                            InstallationTypeId = system.InstallationTypeId.Value,
                            IsInstallationUsed = system.IsInstallationUsed.Value,
                            IsActive = system.IsActive.Value
                        };

                        Db.AquacultureFormFullSystemInstallations.Add(entry);
                    }
                }
            }
            else
            {
                foreach (AquacultureFormFullSystemInstallation entry in dbEntries)
                {
                    entry.IsActive = false;
                }
            }
        }

        private void AddStatisticalFormAquaFarmNotFullSystemInstallations(AquacutlureForm aquaFarm, List<StatisticalFormAquaFarmInstallationSystemNotFullDTO> systems)
        {
            if (systems != null)
            {
                foreach (StatisticalFormAquaFarmInstallationSystemNotFullDTO system in systems)
                {
                    AquacultureFormNonFullSystemInstallation entry = new AquacultureFormNonFullSystemInstallation
                    {
                        AquacultureForm = aquaFarm,
                        InstallationTypeId = system.InstallationTypeId.Value,
                        IsUsedForBreedingMaterial = system.IsInstallationUsedBreedingMaterial.Value,
                        IsUsedForConsumationFish = system.IsInstallationUsedConsumationFish.Value,
                        IsActive = system.IsActive.Value
                    };

                    Db.AquacultureFormNonFullSystemInstallations.Add(entry);
                }
            }
        }

        private void EditStatisticalFormAquaFarmNotFullSystemInstallations(int aquaFarmId, List<StatisticalFormAquaFarmInstallationSystemNotFullDTO> systems)
        {
            List<AquacultureFormNonFullSystemInstallation> dbEntries = (from aquaFarmSystem in Db.AquacultureFormNonFullSystemInstallations
                                                                        where aquaFarmSystem.AquacultureFormId == aquaFarmId
                                                                        select aquaFarmSystem).ToList();

            if (systems != null)
            {
                foreach (StatisticalFormAquaFarmInstallationSystemNotFullDTO system in systems)
                {
                    if (system.Id.HasValue)
                    {
                        AquacultureFormNonFullSystemInstallation dbEntry = dbEntries.Where(x => x.Id == system.Id.Value).Single();
                        dbEntry.IsUsedForBreedingMaterial = system.IsInstallationUsedBreedingMaterial.Value;
                        dbEntry.IsUsedForConsumationFish = system.IsInstallationUsedConsumationFish.Value;
                        dbEntry.InstallationTypeId = system.InstallationTypeId.Value;
                        dbEntry.IsActive = system.IsActive.Value;
                    }
                    else
                    {
                        AquacultureFormNonFullSystemInstallation entry = new AquacultureFormNonFullSystemInstallation
                        {
                            AquacultureFormId = aquaFarmId,
                            InstallationTypeId = system.InstallationTypeId.Value,
                            IsUsedForBreedingMaterial = system.IsInstallationUsedBreedingMaterial.Value,
                            IsUsedForConsumationFish = system.IsInstallationUsedConsumationFish.Value,
                            IsActive = system.IsActive.Value
                        };

                        Db.AquacultureFormNonFullSystemInstallations.Add(entry);
                    }
                }
            }
            else
            {
                foreach (AquacultureFormNonFullSystemInstallation entry in dbEntries)
                {
                    entry.IsActive = false;
                }
            }
        }

        private void AddStatisticalFormAquaFarmBroodstock(AquacutlureForm aquaFarm, List<StatisticalFormAquaFarmBroodstockDTO> broodstocks)
        {
            if (broodstocks != null)
            {
                foreach (StatisticalFormAquaFarmBroodstockDTO broodstock in broodstocks)
                {
                    AquacutlureFormBroodstock entry = new AquacutlureFormBroodstock
                    {
                        AquacultureForm = aquaFarm,
                        InstallationTypeId = broodstock.InstallationTypeId.Value,
                        FishId = broodstock.FishTypeId.Value,
                        FemaleAge = broodstock.FemaleAge,
                        FemaleCount = broodstock.FemaleCount,
                        FemaleWeight = broodstock.FemaleWeight,
                        MaleAge = broodstock.MaleAge,
                        MaleCount = broodstock.MaleCount,
                        MaleWeight = broodstock.MaleWeight,
                        IsActive = broodstock.IsActive.Value
                    };

                    Db.AquacutlureFormBroodstocks.Add(entry);
                }
            }
        }

        private void EditStatisticalFormAquaFarmBroodstock(int aquaFarmId, List<StatisticalFormAquaFarmBroodstockDTO> broodstocks)
        {
            List<AquacutlureFormBroodstock> dbBroodstock = (from aquaFarmBroodstock in Db.AquacutlureFormBroodstocks
                                                            where aquaFarmBroodstock.AquacultureFormId == aquaFarmId
                                                            select aquaFarmBroodstock).ToList();

            if (broodstocks != null)
            {
                foreach (StatisticalFormAquaFarmBroodstockDTO broodstock in broodstocks)
                {
                    if (broodstock.Id.HasValue)
                    {
                        AquacutlureFormBroodstock entry = dbBroodstock.Where(x => x.Id == broodstock.Id.Value).Single();

                        entry.InstallationTypeId = broodstock.InstallationTypeId.Value;
                        entry.FishId = broodstock.FishTypeId.Value;
                        entry.FemaleAge = broodstock.FemaleAge;
                        entry.FemaleCount = broodstock.FemaleCount;
                        entry.FemaleWeight = broodstock.FemaleWeight;
                        entry.MaleAge = broodstock.MaleAge;
                        entry.MaleCount = broodstock.MaleCount;
                        entry.MaleWeight = broodstock.MaleWeight;
                        entry.IsActive = broodstock.IsActive.Value;
                    }
                    else
                    {
                        AquacutlureFormBroodstock entry = new AquacutlureFormBroodstock
                        {
                            AquacultureFormId = aquaFarmId,
                            InstallationTypeId = broodstock.InstallationTypeId.Value,
                            FishId = broodstock.FishTypeId.Value,
                            FemaleAge = broodstock.FemaleAge,
                            FemaleCount = broodstock.FemaleCount,
                            FemaleWeight = broodstock.FemaleWeight,
                            MaleAge = broodstock.MaleAge,
                            MaleCount = broodstock.MaleCount,
                            MaleWeight = broodstock.MaleWeight,
                            IsActive = broodstock.IsActive.Value
                        };

                        Db.AquacutlureFormBroodstocks.Add(entry);
                    }
                }
            }
            else
            {
                foreach (AquacutlureFormBroodstock broodstock in dbBroodstock)
                {
                    broodstock.IsActive = false;
                }
            }
        }

        private void AddStatisticalFormAquaFarmFishOrganism(AquacutlureForm aquaFarm, List<StatisticalFormAquaFarmFishOrganismDTO> fishOrganisms, AquaFarmFishOrganismReportTypeEnum reportType)
        {
            if (fishOrganisms != null)
            {
                foreach (StatisticalFormAquaFarmFishOrganismDTO organism in fishOrganisms)
                {
                    AquacutlureFormStockingMaterial entry = new AquacutlureFormStockingMaterial
                    {
                        AquacultureForm = aquaFarm,
                        InstallationTypeId = organism.InstallationTypeId.Value,
                        FishId = organism.FishTypeId.Value,
                        ReportType = reportType.ToString(),
                        FishLarvaeCount = organism.FishLarvaeCount,
                        CaviarForConsumptionWeight = organism.CaviarForConsumption,
                        ForConsumptionWeight = organism.ForConsumption,
                        OneStripBreedingMaterialCount = organism.OneStripBreedingMaterialCount,
                        OneStripBreedingMaterialWeight = organism.OneStripBreedingMaterialWeight,
                        OneYearBreedingMaterialCount = organism.OneYearBreedingMaterialCount,
                        OneYearBreedingMaterialWeight = organism.OneYearBreedingMaterialWeight,
                        IsActive = organism.IsActive.Value
                    };

                    Db.AquacutlureFormStockingMaterials.Add(entry);
                }
            }
        }

        private void EditStatisticalFormAquaFarmFishOrganism(int aquaFarmId, List<StatisticalFormAquaFarmFishOrganismDTO> fishOrganisms, AquaFarmFishOrganismReportTypeEnum reportType)
        {
            List<AquacutlureFormStockingMaterial> dbEntries = (from stockingMaterial in Db.AquacutlureFormStockingMaterials
                                                               where stockingMaterial.AquacultureFormId == aquaFarmId
                                                               select stockingMaterial).ToList();

            if (fishOrganisms != null)
            {
                foreach (StatisticalFormAquaFarmFishOrganismDTO organism in fishOrganisms)
                {
                    AquacutlureFormStockingMaterial dbfishOrganism = (from stockingMaterial in Db.AquacutlureFormStockingMaterials
                                                                      where stockingMaterial.Id == organism.Id
                                                                      && stockingMaterial.AquacultureFormId == aquaFarmId
                                                                      select stockingMaterial).SingleOrDefault();

                    if (dbfishOrganism == null)
                    {
                        AquacutlureFormStockingMaterial entry = new AquacutlureFormStockingMaterial
                        {
                            AquacultureFormId = aquaFarmId,
                            InstallationTypeId = organism.InstallationTypeId.Value,
                            FishId = organism.FishTypeId.Value,
                            ReportType = reportType.ToString(),
                            FishLarvaeCount = organism.FishLarvaeCount,
                            CaviarForConsumptionWeight = organism.CaviarForConsumption,
                            ForConsumptionWeight = organism.ForConsumption,
                            OneStripBreedingMaterialCount = organism.OneStripBreedingMaterialCount,
                            OneStripBreedingMaterialWeight = organism.OneStripBreedingMaterialWeight,
                            OneYearBreedingMaterialCount = organism.OneYearBreedingMaterialCount,
                            OneYearBreedingMaterialWeight = organism.OneYearBreedingMaterialWeight,
                            IsActive = organism.IsActive.Value
                        };
                        Db.AquacutlureFormStockingMaterials.Add(entry);
                    }
                    else
                    {
                        dbfishOrganism.InstallationTypeId = organism.InstallationTypeId.Value;
                        dbfishOrganism.FishId = organism.FishTypeId.Value;
                        dbfishOrganism.FishLarvaeCount = organism.FishLarvaeCount;
                        dbfishOrganism.CaviarForConsumptionWeight = organism.CaviarForConsumption;
                        dbfishOrganism.ForConsumptionWeight = organism.ForConsumption;
                        dbfishOrganism.OneStripBreedingMaterialCount = organism.OneStripBreedingMaterialCount;
                        dbfishOrganism.OneStripBreedingMaterialWeight = organism.OneStripBreedingMaterialWeight;
                        dbfishOrganism.OneYearBreedingMaterialCount = organism.OneYearBreedingMaterialCount;
                        dbfishOrganism.OneYearBreedingMaterialWeight = organism.OneYearBreedingMaterialWeight;
                        dbfishOrganism.IsActive = organism.IsActive.Value;
                    }
                }
            }
            else
            {
                foreach (AquacutlureFormStockingMaterial entry in dbEntries)
                {
                    entry.IsActive = false;
                }
            }
        }
    }
}
