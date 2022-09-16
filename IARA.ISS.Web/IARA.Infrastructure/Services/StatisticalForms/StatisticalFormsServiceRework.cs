using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.StatisticalForms.Reworks;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class StatisticalFormsService : Service, IStatisticalFormsService
    {
        public StatisticalFormReworkApplicationEditDTO GetStatisticalFormReworkApplication(int applicationId)
        {
            StatisticalFormsRegister dbStatisticalForm = (from statForm in Db.StatisticalFormsRegister
                                                          where statForm.ApplicationId == applicationId
                                                             && statForm.RecordType == nameof(RecordTypesEnum.Application)
                                                          select statForm).SingleOrDefault();

            StatisticalFormReworkApplicationEditDTO result = null;

            int formTypeId = (from type in Db.NstatisticalFormTypes
                              where type.Code == nameof(StatisticalFormTypesEnum.Rework)
                              select type.Id).Single();

            if (dbStatisticalForm == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<StatisticalFormReworkApplicationEditDTO>(draft);
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
                    result = new StatisticalFormReworkApplicationEditDTO
                    {
                        EmployeeInfoGroups = GetEmployeeCountGroups(null, formTypeId),
                        NumStatGroups = GetNumericStatValueGroups(null, formTypeId),
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };
                }
            }
            else
            {
                ReworkForm dbReworkForm = (from reworkForm in Db.ReworkForms
                                           where reworkForm.StatisticalForm == dbStatisticalForm
                                           select reworkForm).Single();

                result = new StatisticalFormReworkApplicationEditDTO
                {
                    Id = dbStatisticalForm.Id,
                    ApplicationId = dbStatisticalForm.ApplicationId,
                    Year = dbStatisticalForm.ForYear.Year,
                    SubmittedByWorkPosition = dbStatisticalForm.SubmitPersonWorkPosition,
                    VetRegistrationNum = dbReworkForm.VetRegistrationNum,
                    LicenceDate = dbReworkForm.LicenseDate,
                    LicenceNum = dbReworkForm.LicenceNum,
                    TotalRawMaterialTons = dbReworkForm.TotalRawMaterialTons,
                    TotalReworkedProductTons = dbReworkForm.TotalReworkedProductTons,
                    TotalYearTurnover = dbReworkForm.TotalYearTurnover
                };

                result.RawMaterial = GetStatisticalFormReworkRawMaterials(dbReworkForm.Id);
                result.Products = GetStatisticalFormReworkProducts(dbReworkForm.Id);

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);

                result.EmployeeInfoGroups = GetEmployeeCountGroups(dbStatisticalForm.Id, dbStatisticalForm.StatisticalFormTypeId);
                result.NumStatGroups = GetNumericStatValueGroups(dbStatisticalForm.Id, dbStatisticalForm.StatisticalFormTypeId);

                result.Files = Db.GetFiles(Db.StatisticalFormsRegisterFiles, dbStatisticalForm.Id);
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);
            }

            return result;
        }

        public RegixChecksWrapperDTO<StatisticalFormReworkRegixDataDTO> GetStatisticalFormReworkRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<StatisticalFormReworkRegixDataDTO> result = new RegixChecksWrapperDTO<StatisticalFormReworkRegixDataDTO>
            {
                DialogDataModel = GetApplicationReworkRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetStatisticalFormReworkChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public StatisticalFormReworkRegixDataDTO GetApplicationReworkRegixData(int applicationId)
        {
            StatisticalFormApplicationDataIds data = GetApplicationDataIds(applicationId);

            StatisticalFormReworkRegixDataDTO regixData = new StatisticalFormReworkRegixDataDTO
            {
                Id = data.StatisticalFormId,
                ApplicationId = applicationId,
                PageCode = data.PageCode
            };

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId);

            return regixData;
        }

        public StatisticalFormReworkEditDTO GetApplicationReworkDataForRegister(int applicationId)
        {
            var data = (from statForm in Db.StatisticalFormsRegister
                        join reworkForm in Db.ReworkForms on statForm.Id equals reworkForm.StatisticalFormId
                        where statForm.ApplicationId == applicationId
                            && statForm.RecordType == nameof(RecordTypesEnum.Application)
                        select new
                        {
                            statForm.Id,
                            statForm.SubmittedForPersonId,
                            statForm.SubmittedForLegalId,
                            statForm.StatisticalFormTypeId,
                            ReworkForm = new StatisticalFormReworkEditDTO
                            {
                                Id = reworkForm.Id,
                                ApplicationId = reworkForm.StatisticalForm.ApplicationId,
                                VetRegistrationNum = reworkForm.VetRegistrationNum,
                                LicenceDate = reworkForm.LicenseDate,
                                LicenceNum = reworkForm.LicenceNum,
                                TotalRawMaterialTons = reworkForm.TotalRawMaterialTons,
                                TotalReworkedProductTons = reworkForm.TotalReworkedProductTons,
                                TotalYearTurnover = reworkForm.TotalYearTurnover,
                                Year = statForm.ForYear.Year
                            }
                        }).Single();

            StatisticalFormReworkEditDTO result = data.ReworkForm;

            result.SubmittedFor = applicationService.GetRegisterSubmittedFor(result.ApplicationId.Value, data.SubmittedForPersonId, data.SubmittedForLegalId);
            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(result.ApplicationId.Value, ApplicationHierarchyTypesEnum.Online);

            result.RawMaterial = GetStatisticalFormReworkRawMaterials(result.Id.Value);
            result.Products = GetStatisticalFormReworkProducts(result.Id.Value);

            result.EmployeeInfoGroups = GetEmployeeCountGroups(data.Id, data.StatisticalFormTypeId);
            result.NumStatGroups = GetNumericStatValueGroups(data.Id, data.StatisticalFormTypeId);

            result.Files = Db.GetFiles(Db.StatisticalFormsRegisterFiles, data.Id);

            return result;
        }

        public StatisticalFormReworkEditDTO GetReworkRegisterByApplicationId(int applicationId)
        {
            int id = (from statForm in Db.StatisticalFormsRegister
                      where statForm.ApplicationId == applicationId
                        && statForm.RecordType == nameof(RecordTypesEnum.Register)
                      select statForm.Id).SingleOrDefault();

            return GetStatisticalFormRework(id);
        }

        public StatisticalFormReworkEditDTO GetStatisticalFormRework(int id)
        {
            var data = (from statForm in Db.StatisticalFormsRegister
                        join reworkForm in Db.ReworkForms on statForm.Id equals reworkForm.StatisticalFormId
                        where statForm.Id == id
                           && statForm.RecordType == nameof(RecordTypesEnum.Register)
                        select new
                        {
                            statForm.Id,
                            statForm.SubmittedForPersonId,
                            statForm.SubmittedForLegalId,
                            statForm.StatisticalFormTypeId,
                            ReworkForm = new StatisticalFormReworkEditDTO
                            {
                                Id = reworkForm.Id,
                                ApplicationId = reworkForm.StatisticalForm.ApplicationId,
                                VetRegistrationNum = reworkForm.VetRegistrationNum,
                                LicenceDate = reworkForm.LicenseDate,
                                LicenceNum = reworkForm.LicenceNum,
                                TotalRawMaterialTons = reworkForm.TotalRawMaterialTons,
                                TotalReworkedProductTons = reworkForm.TotalReworkedProductTons,
                                TotalYearTurnover = reworkForm.TotalYearTurnover,
                                FormNum = statForm.RegistrationNum,
                                Year = statForm.ForYear.Year
                            }
                        }).Single();

            StatisticalFormReworkEditDTO result = data.ReworkForm;

            result.SubmittedFor = applicationService.GetRegisterSubmittedFor(result.ApplicationId.Value, data.SubmittedForPersonId, data.SubmittedForLegalId);
            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(result.ApplicationId.Value, ApplicationHierarchyTypesEnum.Online);

            result.RawMaterial = GetStatisticalFormReworkRawMaterials(result.Id.Value);
            result.Products = GetStatisticalFormReworkProducts(result.Id.Value);

            result.EmployeeInfoGroups = GetEmployeeCountGroups(data.Id, data.StatisticalFormTypeId);
            result.NumStatGroups = GetNumericStatValueGroups(data.Id, data.StatisticalFormTypeId);

            result.Files = Db.GetFiles(Db.StatisticalFormsRegisterFiles, data.Id);

            return result;
        }

        public int AddStatisticalFormReworkApplication(StatisticalFormReworkApplicationEditDTO form, ApplicationStatusesEnum? nextManualStatus = null)
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
                    StatisticalFormTypeId = types[StatisticalFormTypesEnum.Rework]
                };

                entry.ReworkForm = new ReworkForm
                {
                    StatisticalForm = entry,
                    VetRegistrationNum = form.VetRegistrationNum,
                    LicenseDate = form.LicenceDate,
                    LicenceNum = form.LicenceNum,
                    TotalRawMaterialTons = form.TotalRawMaterialTons.Value,
                    TotalReworkedProductTons = form.TotalReworkedProductTons.Value,
                    TotalYearTurnover = form.TotalYearTurnover.Value
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

                AddStatisticalFormReworkRawMaterial(entry.ReworkForm, form.RawMaterial);
                AddStatisticalFormReworkProducts(entry.ReworkForm, form.Products);

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

            List<FileInfoDTO> statisticalFormFiles = form.Files;
            form.Files = null;
            stateMachine.Act(entry.ApplicationId, CommonUtils.Serialize(form), statisticalFormFiles, nextManualStatus);

            return entry.Id;
        }

        public int AddStatisticalFormRework(StatisticalFormReworkEditDTO form)
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
                    StatisticalFormTypeId = types[StatisticalFormTypesEnum.Rework]
                };

                entry.ReworkForm = new ReworkForm
                {
                    StatisticalForm = entry,
                    VetRegistrationNum = form.VetRegistrationNum,
                    LicenseDate = form.LicenceDate,
                    LicenceNum = form.LicenceNum,
                    TotalRawMaterialTons = form.TotalRawMaterialTons.Value,
                    TotalReworkedProductTons = form.TotalReworkedProductTons.Value,
                    TotalYearTurnover = form.TotalYearTurnover.Value
                };

                Db.StatisticalFormsRegister.Add(entry);

                Db.AddOrEditRegisterSubmittedFor(entry, form.SubmittedFor);
                Db.SaveChanges();

                AddStatisticalFormReworkRawMaterial(entry.ReworkForm, form.RawMaterial);
                AddStatisticalFormReworkProducts(entry.ReworkForm, form.Products);

                AddNumericStatValues(entry, form.NumStatGroups);
                AddEmployeeCounts(entry, form.EmployeeInfoGroups);

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

        public void EditStatisticalFormReworkApplication(StatisticalFormReworkApplicationEditDTO form, ApplicationStatusesEnum? manualStatus = null)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                StatisticalFormsRegister dbStatisticalForm = (from statForm in Db.StatisticalFormsRegister
                                                                     .AsSplitQuery()
                                                                     .Include(x => x.StatisticalFormsRegisterFiles)
                                                              where statForm.Id == form.Id
                                                              select statForm).SingleOrDefault();

                ReworkForm dbReworkForm = (from reworkForm in Db.ReworkForms
                                           where reworkForm.StatisticalFormId == dbStatisticalForm.Id
                                           select reworkForm).SingleOrDefault();

                dbReworkForm.LicenceNum = form.LicenceNum;
                dbReworkForm.LicenseDate = form.LicenceDate;
                dbReworkForm.TotalRawMaterialTons = form.TotalRawMaterialTons.Value;
                dbReworkForm.TotalReworkedProductTons = form.TotalReworkedProductTons.Value;
                dbReworkForm.TotalYearTurnover = form.TotalYearTurnover.Value;
                dbStatisticalForm.SubmitPersonWorkPosition = form.SubmittedByWorkPosition;
                dbStatisticalForm.ForYear = new DateTime(form.Year.Value, 1, 1);

                EditStatisticalFormReworkRawMaterial(dbReworkForm.Id, form.RawMaterial);
                EditStatisticalFormReworkProducts(dbReworkForm.Id, form.Products);

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

            List<FileInfoDTO> reworkFiles = form.Files;
            form.Files = null;
            stateMachine.Act(application.Id, CommonUtils.Serialize(form), reworkFiles, manualStatus, application.StatusReason);
        }

        public void EditStatisticalFormReworkApplicationRegixData(StatisticalFormReworkRegixDataDTO form)
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

        public void EditStatisticalFormRework(StatisticalFormReworkEditDTO form)
        {
            using TransactionScope scope = new TransactionScope();

            StatisticalFormsRegister dbStatisticalForm = (from statForm in Db.StatisticalFormsRegister
                                                            .Include(x => x.StatisticalFormsRegisterFiles)
                                                          where statForm.ApplicationId == form.ApplicationId
                                                            && statForm.RecordType == nameof(RecordTypesEnum.Register)
                                                          select statForm).SingleOrDefault();

            ReworkForm dbReworkForm = (from rework in Db.ReworkForms
                                       where rework.StatisticalForm == dbStatisticalForm
                                       select rework).SingleOrDefault();

            Db.AddOrEditRegisterSubmittedFor(dbStatisticalForm, form.SubmittedFor);
            Db.SaveChanges();

            dbReworkForm.LicenceNum = form.LicenceNum;
            dbReworkForm.LicenseDate = form.LicenceDate;
            dbReworkForm.TotalRawMaterialTons = form.TotalRawMaterialTons.Value;
            dbReworkForm.TotalReworkedProductTons = form.TotalReworkedProductTons.Value;
            dbReworkForm.TotalYearTurnover = form.TotalYearTurnover.Value;
            dbStatisticalForm.ForYear = new DateTime(form.Year.Value, 1, 1);

            EditStatisticalFormReworkRawMaterial(dbReworkForm.Id, form.RawMaterial);
            EditStatisticalFormReworkProducts(dbReworkForm.Id, form.Products);

            EditNumericStatValues(dbReworkForm.StatisticalFormId, form.NumStatGroups);
            EditEmployeeCounts(dbReworkForm.StatisticalFormId, form.EmployeeInfoGroups);

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

        private ApplicationSubmittedByDTO GetUserAsSubmittedByForRework(int userId)
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

        private List<StatisticalFormReworkRawMaterialDTO> GetStatisticalFormReworkRawMaterials(int reworkId)
        {
            List<StatisticalFormReworkRawMaterialDTO> result = (from rework in Db.ReworkForms
                                                                join material in Db.ReworkRawMaterials on rework.Id equals material.ReworkFormId
                                                                where rework.Id == reworkId
                                                                select new StatisticalFormReworkRawMaterialDTO
                                                                {
                                                                    Id = material.Id,
                                                                    FishTypeId = material.FishId,
                                                                    Tons = material.QuantityTons,
                                                                    Origin = Enum.Parse<StatisticalFormRawMaterialOriginEnum>(material.MaterialOrigin),
                                                                    IsActive = material.IsActive,
                                                                    CountryZone = material.CountryZone
                                                                }).ToList();
            return result;
        }

        private List<StatisticalFormReworkProductDTO> GetStatisticalFormReworkProducts(int reworkId)
        {
            List<StatisticalFormReworkProductDTO> result = (from rework in Db.ReworkForms
                                                            join product in Db.ReworkProducts on rework.Id equals product.ReworkFormId
                                                            where rework.Id == reworkId
                                                            select new StatisticalFormReworkProductDTO
                                                            {
                                                                Id = product.Id,
                                                                ProductTypeId = product.ProductId,
                                                                Tons = product.QuantityTons,
                                                                IsActive = product.IsActive
                                                            }).ToList();
            return result;
        }

        private void AddStatisticalFormReworkRawMaterial(ReworkForm rework, List<StatisticalFormReworkRawMaterialDTO> rawMaterials)
        {
            if (rawMaterials != null)
            {
                foreach (StatisticalFormReworkRawMaterialDTO material in rawMaterials)
                {
                    ReworkRawMaterial entry = new ReworkRawMaterial
                    {
                        ReworkForm = rework,
                        FishId = material.FishTypeId.Value,
                        QuantityTons = material.Tons.Value,
                        MaterialOrigin = material.Origin.ToString(),
                        IsActive = material.IsActive.Value,
                        CountryZone = material.CountryZone
                    };

                    Db.ReworkRawMaterials.Add(entry);
                }
            }
        }

        private void EditStatisticalFormReworkRawMaterial(int reworkId, List<StatisticalFormReworkRawMaterialDTO> rawMaterials)
        {
            List<ReworkRawMaterial> dbEntries = (from reworkRawMaterial in Db.ReworkRawMaterials
                                                 where reworkRawMaterial.ReworkFormId == reworkId
                                                 select reworkRawMaterial).ToList();

            if (rawMaterials != null)
            {
                foreach (StatisticalFormReworkRawMaterialDTO material in rawMaterials)
                {
                    if (material.Id.HasValue)
                    {
                        ReworkRawMaterial dbEntry = dbEntries.Where(x => x.Id == material.Id.Value).Single();
                        dbEntry.FishId = material.FishTypeId.Value;
                        dbEntry.QuantityTons = material.Tons.Value;
                        dbEntry.MaterialOrigin = material.Origin.ToString();
                        dbEntry.IsActive = material.IsActive.Value;
                        dbEntry.CountryZone = material.CountryZone;
                    }
                    else
                    {
                        ReworkRawMaterial entry = new ReworkRawMaterial
                        {
                            ReworkFormId = reworkId,
                            FishId = material.FishTypeId.Value,
                            QuantityTons = material.Tons.Value,
                            MaterialOrigin = material.Origin.ToString(),
                            IsActive = material.IsActive.Value,
                            CountryZone = material.CountryZone
                        };

                        Db.ReworkRawMaterials.Add(entry);
                    }
                }
            }
            else
            {
                foreach (ReworkRawMaterial entry in dbEntries)
                {
                    entry.IsActive = false;
                }
            }
        }

        private void AddStatisticalFormReworkProducts(ReworkForm rework, List<StatisticalFormReworkProductDTO> products)
        {
            if (products != null)
            {
                Dictionary<int, NreworkProductType> newProductTypes = new Dictionary<int, NreworkProductType>();

                foreach (StatisticalFormReworkProductDTO product in products)
                {
                    ReworkProduct entry = new ReworkProduct
                    {
                        ReworkForm = rework,
                        QuantityTons = product.Tons.Value,
                        IsActive = product.IsActive.Value
                    };

                    if (product.IsNewProductType.Value)
                    {
                        if (newProductTypes.TryGetValue(product.ProductTypeId.Value, out NreworkProductType productType))
                        {
                            entry.Product = productType;
                        }
                        else
                        {
                            entry.Product = new NreworkProductType { Name = product.ProductTypeName };
                            newProductTypes.Add(product.ProductTypeId.Value, entry.Product);
                        }
                    }
                    else
                    {
                        entry.ProductId = product.ProductTypeId.Value;
                    }

                    Db.ReworkProducts.Add(entry);
                }
            }
        }

        private void EditStatisticalFormReworkProducts(int reworkId, List<StatisticalFormReworkProductDTO> products)
        {
            List<ReworkProduct> dbEntries = (from reworkProduct in Db.ReworkProducts
                                             where reworkProduct.ReworkFormId == reworkId
                                             select reworkProduct).ToList();

            foreach (ReworkProduct entry in dbEntries)
            {
                entry.IsActive = false;
            }

            if (products != null)
            {
                DateTime now = DateTime.Now;

                Dictionary<int, NreworkProductType> newProductTypes = new Dictionary<int, NreworkProductType>();

                foreach (StatisticalFormReworkProductDTO product in products)
                {
                    if (product.Id.HasValue)
                    {
                        ReworkProduct dbEntry = dbEntries.Where(x => x.Id == product.Id.Value).Single();
                        if (product.IsNewProductType.Value)
                        {
                            dbEntry.IsActive = false;

                            ReworkProduct entry = new ReworkProduct
                            {
                                ReworkFormId = reworkId,
                                QuantityTons = product.Tons.Value,
                                IsActive = product.IsActive.Value
                            };

                            if (newProductTypes.TryGetValue(product.ProductTypeId.Value, out NreworkProductType productType))
                            {
                                entry.Product = productType;
                            }
                            else
                            {
                                entry.Product = new NreworkProductType { Name = product.ProductTypeName };
                                newProductTypes.Add(product.ProductTypeId.Value, entry.Product);
                            }

                            Db.ReworkProducts.Add(entry);
                        }
                        else
                        {
                            dbEntry.ProductId = product.ProductTypeId.Value;
                            dbEntry.QuantityTons = product.Tons.Value;
                            dbEntry.IsActive = product.IsActive.Value;
                        }
                    }
                    else
                    {
                        ReworkProduct entry = new ReworkProduct
                        {
                            ReworkFormId = reworkId,
                            QuantityTons = product.Tons.Value,
                            IsActive = product.IsActive.Value
                        };

                        if (newProductTypes.TryGetValue(product.ProductTypeId.Value, out NreworkProductType productType))
                        {
                            entry.Product = productType;
                        }
                        else
                        {
                            entry.Product = new NreworkProductType { Name = product.ProductTypeName };
                            newProductTypes.Add(product.ProductTypeId.Value, entry.Product);
                        }

                        Db.ReworkProducts.Add(entry);
                    }
                }
            }
        }
    }
}
