using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.StatisticalForms;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces;
using IARA.Interfaces.FSM;
using IARA.RegixAbstractions.Interfaces;

namespace IARA.Infrastructure.Services
{
    public partial class StatisticalFormsService : Service, IStatisticalFormsService
    {
        private readonly IApplicationService applicationService;
        private readonly IApplicationStateMachine stateMachine;
        private readonly IRegixApplicationInterfaceService regixApplicationService;
        private readonly IPersonService personService;

        public StatisticalFormsService(IARADbContext db,
                                        IApplicationService applicationService,
                                        IApplicationStateMachine stateMachine,
                                        IPersonService personService,
                                        IRegixApplicationInterfaceService regixApplicationService)
            : base(db)
        {
            this.applicationService = applicationService;
            this.stateMachine = stateMachine;
            this.personService = personService;
            this.regixApplicationService = regixApplicationService;
        }

        public IQueryable<StatisticalFormDTO> GetAllStatisticalForms(StatisticalFormsFilters filters, List<StatisticalFormTypesEnum> types, int? userId)
        {
            IQueryable<StatisticalFormDTO> result;
            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllStatisticalFormsNoFilter(showInactive, types, userId);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteresStatisticalForms(filters, types, userId)
                   : GetParameterFilteredStatisticalForms(filters, types, userId);
            }

            return result;
        }

        public void DeleteStatisticalForm(int id)
        {
            DeleteRecordWithId(Db.StatisticalFormsRegister, id);
            Db.SaveChanges();
        }

        public void UndoDeleteStatisticalForm(int id)
        {
            UndoDeleteRecordWithId(Db.StatisticalFormsRegister, id);
            Db.SaveChanges();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.StatisticalFormsRegister, id);
            return audit;
        }

        public ApplicationSubmittedByDTO GetUserAsSubmittedBy(int userId, StatisticalFormTypesEnum type)
        {
            switch (type)
            {
                case StatisticalFormTypesEnum.AquaFarm:
                    return GetUserAsSubmittedByForAquaFarm(userId);
                case StatisticalFormTypesEnum.FishVessel:
                    return GetUserAsSubmittedByForFishVessel(userId);
                case StatisticalFormTypesEnum.Rework:
                    return GetUserAsSubmittedByForRework(userId);
            }

            throw new ArgumentException("Invalid statistical form type: " + type.ToString());
        }

        public Task<ApplicationEDeliveryInfo> GetEDeliveryInfo(int applicationId, PageCodeEnum pageCode)
        {
            throw new NotImplementedException("Nothing to deliver for statistical form with page code: " + pageCode.ToString());
        }

        private IQueryable<StatisticalFormDTO> GetAllStatisticalFormsNoFilter(bool showInactive, List<StatisticalFormTypesEnum> types, int? userId)
        {
            List<string> typeStrings = types.Select(x => x.ToString()).ToList();

            var query = from form in Db.StatisticalFormsRegister
                        join formType in Db.NstatisticalFormTypes on form.StatisticalFormTypeId equals formType.Id
                        join appl in Db.Applications on form.ApplicationId equals appl.Id
                        join aquaForm in Db.AquacutlureForms on form.Id equals aquaForm.StatisticalFormId into aF
                        from aquaForm in aF.DefaultIfEmpty()
                        join aquaculture in Db.AquacultureFacilitiesRegister on aquaForm.AquacultureFacilityId equals aquaculture.Id into aqua
                        from aquaculture in aqua.DefaultIfEmpty()
                        join shipForm in Db.FishVesselsForms on form.Id equals shipForm.StatisticalFormId into sF
                        from shipForm in sF.DefaultIfEmpty()
                        join ship in Db.ShipsRegister on shipForm.ShipId equals ship.Id into sh
                        from ship in sh.DefaultIfEmpty()
                        join reworkForm in Db.ReworkForms on form.Id equals reworkForm.StatisticalFormId into rF
                        from reworkForm in rF.DefaultIfEmpty()
                        join legal in Db.Legals on form.SubmittedForLegalId equals legal.Id into leg
                        from legal in leg.DefaultIfEmpty()
                        join processUser in Db.Users on appl.AssignedUserId equals processUser.Id
                        join processPerson in Db.Persons on processUser.PersonId equals processPerson.Id
                        where form.RecordType == nameof(RecordTypesEnum.Register)
                              && form.IsActive != showInactive
                              && typeStrings.Contains(formType.Code)
                        orderby form.RegistrationDate descending
                        select new
                        {
                            form.Id,
                            form.RegistrationNum,
                            appl.SubmittedByUserId,
                            appl.SubmittedForPersonId,
                            ProcessUser = processPerson.FirstName + " " + processPerson.LastName,
                            form.ForYear.Year,
                            SubmissionDate = form.RegistrationDate.Value,
                            FormObject = aquaForm != null
                               ? aquaculture.Name + " | {{URORR}}: " + aquaculture.UrorNum + " | " + "{{REG}}: " + aquaculture.RegNum.ToString()
                               : shipForm != null
                                   ? ship.Name + " | {{CFR}}: " + ship.Cfr + " | {{EXT}}: " + ship.ExternalMark
                                   : reworkForm != null
                                       ? legal.Name
                                       : null,
                            FormTypeName = formType.Name,
                            FormType = Enum.Parse<StatisticalFormTypesEnum>(formType.Code),
                            form.IsActive
                        };

            IQueryable<StatisticalFormDTO> result = from form in query
                                                    select new StatisticalFormDTO
                                                    {
                                                        Id = form.Id,
                                                        RegistryNumber = form.RegistrationNum,
                                                        Year = form.Year,
                                                        SubmissionDate = form.SubmissionDate,
                                                        FormObject = form.FormObject,
                                                        FormTypeName = form.FormTypeName,
                                                        FormType = form.FormType,
                                                        IsActive = form.IsActive
                                                    };

            if (userId.HasValue)
            {
                List<int> personIds = userId.HasValue ? GetUserPersonIds(userId.Value) : null;

                result = from form in query
                         where (form.SubmittedByUserId == userId.Value || personIds.Contains(form.SubmittedForPersonId.Value))
                         select new StatisticalFormDTO
                         {
                             Id = form.Id,
                             RegistryNumber = form.RegistrationNum,
                             Year = form.Year,
                             SubmissionDate = form.SubmissionDate,
                             FormObject = form.FormObject,
                             FormTypeName = form.FormTypeName,
                             FormType = form.FormType,
                             IsActive = form.IsActive
                         };
            }
            else
            {
                result = from form in query
                         select new StatisticalFormDTO
                         {
                             Id = form.Id,
                             RegistryNumber = form.RegistrationNum,
                             ProcessUser = form.ProcessUser,
                             Year = form.Year,
                             SubmissionDate = form.SubmissionDate,
                             FormObject = form.FormObject,
                             FormTypeName = form.FormTypeName,
                             FormType = form.FormType,
                             IsActive = form.IsActive
                         };
            }

            return result;
        }

        private IQueryable<StatisticalFormDTO> GetParameterFilteredStatisticalForms(StatisticalFormsFilters filters, List<StatisticalFormTypesEnum> types, int? userId)
        {
            List<string> typeStrings = types.Select(x => x.ToString()).ToList();

            var forms = from form in Db.StatisticalFormsRegister
                        join appl in Db.Applications on form.ApplicationId equals appl.Id
                        join aquaForm in Db.AquacutlureForms on form.Id equals aquaForm.StatisticalFormId into aF
                        from aquaForm in aF.DefaultIfEmpty()
                        join aquaculture in Db.AquacultureFacilitiesRegister on aquaForm.AquacultureFacilityId equals aquaculture.Id into aqua
                        from aquaculture in aqua.DefaultIfEmpty()
                        join shipForm in Db.FishVesselsForms on form.Id equals shipForm.StatisticalFormId into sF
                        from shipForm in sF.DefaultIfEmpty()
                        join ship in Db.ShipsRegister on shipForm.ShipId equals ship.Id into sh
                        from ship in sh.DefaultIfEmpty()
                        join reworkForm in Db.ReworkForms on form.Id equals reworkForm.StatisticalFormId into rF
                        from reworkForm in rF.DefaultIfEmpty()
                        join legal in Db.Legals on form.SubmittedForLegalId equals legal.Id into leg
                        from legal in leg.DefaultIfEmpty()
                        join formType in Db.NstatisticalFormTypes on form.StatisticalFormTypeId equals formType.Id
                        where form.RecordType == nameof(RecordTypesEnum.Register)
                              && form.IsActive != filters.ShowInactiveRecords
                              && typeStrings.Contains(formType.Code)
                        select new
                        {
                            form.Id,
                            form.RegistrationNum,
                            appl.AssignedUserId,
                            form.RegistrationDate,
                            form.ForYear,
                            form.SubmittedForLegalId,
                            form.SubmittedForPersonId,
                            form.StatisticalFormTypeId,
                            appl.SubmittedByUserId,
                            appl.SubmittedByPersonId,
                            form.IsActive,
                            appl.TerritoryUnitId,
                            FormObject = aquaForm != null
                                ? aquaculture.Name + " | {{URORR}}: " + aquaculture.UrorNum + " | " + "{{REG}}: " + aquaculture.RegNum.ToString()
                                : shipForm != null
                                    ? ship.Name + " | {{CFR}}: " + ship.Cfr + " | {{EXT}}: " + ship.ExternalMark
                                    : reworkForm != null
                                        ? legal.Name
                                        : null,
                            FormType = formType.Code
                        };

            if (userId.HasValue)
            {
                List<int> personIds = userId.HasValue ? GetUserPersonIds(userId.Value) : null;

                forms = from form in forms
                        where form.SubmittedByUserId == userId.Value
                            || personIds.Contains(form.SubmittedByPersonId.Value)
                        select form;
            }

            if (!userId.HasValue && filters.ProcessUserId.HasValue)
            {
                forms = forms.Where(x => x.AssignedUserId == filters.ProcessUserId);
            }

            if (!string.IsNullOrEmpty(filters.RegistryNumber))
            {
                forms = forms.Where(x => x.RegistrationNum.ToLower().Contains(filters.RegistryNumber));
            }

            if (filters.SubmissionDateFrom.HasValue)
            {
                forms = forms.Where(x => x.RegistrationDate >= filters.SubmissionDateFrom);
            }

            if (filters.SubmissionDateTo.HasValue)
            {
                forms = forms.Where(x => x.RegistrationDate <= filters.SubmissionDateTo);
            }

            if (!userId.HasValue && filters.SubmissionUserId.HasValue)
            {
                forms = forms.Where(x => x.SubmittedByUserId == filters.SubmissionUserId);
            }

            if (filters.FormTypeIds != null && filters.FormTypeIds.Count > 0)
            {
                forms = forms.Where(x => filters.FormTypeIds.Contains(x.StatisticalFormTypeId));
            }

            if (!string.IsNullOrEmpty(filters.FormObject))
            {
                forms = forms.Where(x => x.FormObject.ToLower().Replace(" ", "").Contains(filters.FormObject.ToLower().Replace(" ", "")));
            }

            if (filters.PersonId.HasValue)
            {
                forms = forms.Where(x => x.SubmittedForPersonId == filters.PersonId);
            }

            if (filters.LegalId.HasValue)
            {
                forms = forms.Where(x => x.SubmittedForLegalId == filters.LegalId);
            }

            if (filters.TerritoryUnitId.HasValue)
            {
                if (!filters.FilterAquaFarmTerritoryUnit.HasValue
                     && !filters.FilterReworkTerritoryUnit.HasValue
                     && !filters.FilterFishVesselTerritoryUnit.HasValue)
                {
                    forms = forms.Where(x => x.TerritoryUnitId == filters.TerritoryUnitId.Value);
                }
                else if (filters.FilterAquaFarmTerritoryUnit.HasValue && !filters.FilterAquaFarmTerritoryUnit.Value
                         && filters.FilterReworkTerritoryUnit.HasValue && !filters.FilterReworkTerritoryUnit.Value
                         && filters.FilterFishVesselTerritoryUnit.HasValue && !filters.FilterFishVesselTerritoryUnit.Value)
                {
                    forms = forms.Where(x => x.TerritoryUnitId == filters.TerritoryUnitId.Value);
                }
                else
                {
                    if (filters.FilterAquaFarmTerritoryUnit.HasValue && filters.FilterAquaFarmTerritoryUnit.Value)
                    {
                        forms = forms.Where(x => x.FormType != nameof(StatisticalFormTypesEnum.AquaFarm) || x.TerritoryUnitId == filters.TerritoryUnitId.Value);
                    }

                    if (filters.FilterReworkTerritoryUnit.HasValue && filters.FilterReworkTerritoryUnit.Value)
                    {
                        forms = forms.Where(x => x.FormType != nameof(StatisticalFormTypesEnum.Rework) || x.TerritoryUnitId == filters.TerritoryUnitId.Value);
                    }

                    if (filters.FilterFishVesselTerritoryUnit.HasValue && filters.FilterFishVesselTerritoryUnit.Value)
                    {
                        forms = forms.Where(x => x.FormType != nameof(StatisticalFormTypesEnum.FishVessel) || x.TerritoryUnitId == filters.TerritoryUnitId.Value);
                    }
                }
            }

            IQueryable<StatisticalFormDTO> result;

            if (userId.HasValue)
            {
                result = from form in forms
                         join formType in Db.NstatisticalFormTypes on form.StatisticalFormTypeId equals formType.Id
                         select new StatisticalFormDTO
                         {
                             Id = form.Id,
                             RegistryNumber = form.RegistrationNum,
                             Year = form.ForYear.Year,
                             SubmissionDate = form.RegistrationDate.Value,
                             FormObject = form.FormObject,
                             FormTypeName = formType.Name,
                             FormType = Enum.Parse<StatisticalFormTypesEnum>(formType.Code),
                             IsActive = form.IsActive
                         };
            }
            else
            {
                result = from form in forms
                         join formType in Db.NstatisticalFormTypes on form.StatisticalFormTypeId equals formType.Id
                         join processUser in Db.Users on form.AssignedUserId equals processUser.Id
                         join processPerson in Db.Persons on processUser.PersonId equals processPerson.Id
                         select new StatisticalFormDTO
                         {
                             Id = form.Id,
                             RegistryNumber = form.RegistrationNum,
                             ProcessUser = processPerson.FirstName + " " + processPerson.LastName,
                             Year = form.ForYear.Year,
                             SubmissionDate = form.RegistrationDate.Value,
                             FormObject = form.FormObject,
                             FormTypeName = formType.Name,
                             FormType = Enum.Parse<StatisticalFormTypesEnum>(formType.Code),
                             IsActive = form.IsActive
                         };
            }

            return result;
        }

        private IQueryable<StatisticalFormDTO> GetFreeTextFilteresStatisticalForms(StatisticalFormsFilters filters, List<StatisticalFormTypesEnum> types, int? userId)
        {
            string text = filters.FreeTextSearch.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);
            List<string> typeStrings = types.Select(x => x.ToString()).ToList();

            var query = from form in Db.StatisticalFormsRegister
                        join formType in Db.NstatisticalFormTypes on form.StatisticalFormTypeId equals formType.Id
                        join appl in Db.Applications on form.ApplicationId equals appl.Id
                        join aquaForm in Db.AquacutlureForms on form.Id equals aquaForm.StatisticalFormId into aF
                        from aquaForm in aF.DefaultIfEmpty()
                        join aquaculture in Db.AquacultureFacilitiesRegister on aquaForm.AquacultureFacilityId equals aquaculture.Id into aqua
                        from aquaculture in aqua.DefaultIfEmpty()
                        join shipForm in Db.FishVesselsForms on form.Id equals shipForm.StatisticalFormId into sF
                        from shipForm in sF.DefaultIfEmpty()
                        join ship in Db.ShipsRegister on shipForm.ShipId equals ship.Id into sh
                        from ship in sh.DefaultIfEmpty()
                        join reworkForm in Db.ReworkForms on form.Id equals reworkForm.StatisticalFormId into rF
                        from reworkForm in rF.DefaultIfEmpty()
                        join legal in Db.Legals on form.SubmittedForLegalId equals legal.Id into leg
                        from legal in leg.DefaultIfEmpty()
                        where form.RecordType == nameof(RecordTypesEnum.Register)
                              && form.IsActive != filters.ShowInactiveRecords
                              && typeStrings.Contains(formType.Code)
                        orderby form.RegistrationDate descending
                        select new
                        {
                            form.Id,
                            RegistryNumber = form.RegistrationNum,
                            appl.SubmittedByUserId,
                            appl.SubmittedByPersonId,
                            appl.AssignedUserId,
                            appl.TerritoryUnitId,
                            form.ForYear.Year,
                            SubmissionDate = form.RegistrationDate.Value,
                            FormTypeName = formType.Name,
                            FormTypeCode = formType.Code,
                            FormObject = aquaForm != null
                                ? aquaculture.Name + " | {{URORR}}: " + aquaculture.UrorNum + " | " + "{{REG}}: " + aquaculture.RegNum.ToString()
                                : shipForm != null
                                    ? ship.Name + " | {{CFR}}: " + ship.Cfr + " | {{EXT}}: " + ship.ExternalMark
                                    : reworkForm != null
                                        ? legal.Name
                                        : null,
                            form.IsActive
                        };

            if (userId.HasValue)
            {
                if (filters.FilterAquaFarmTerritoryUnit.HasValue && filters.FilterAquaFarmTerritoryUnit.Value)
                {
                    query = query.Where(x => x.FormTypeCode != nameof(StatisticalFormTypesEnum.AquaFarm) || x.TerritoryUnitId == filters.TerritoryUnitId.Value);
                }

                if (filters.FilterReworkTerritoryUnit.HasValue && filters.FilterReworkTerritoryUnit.Value)
                {
                    query = query.Where(x => x.FormTypeCode != nameof(StatisticalFormTypesEnum.Rework) || x.TerritoryUnitId == filters.TerritoryUnitId.Value);
                }

                if (filters.FilterFishVesselTerritoryUnit.HasValue && filters.FilterFishVesselTerritoryUnit.Value)
                {
                    query = query.Where(x => x.FormTypeCode != nameof(StatisticalFormTypesEnum.FishVessel) || x.TerritoryUnitId == filters.TerritoryUnitId.Value);
                }

                List<int> personIds = GetUserPersonIds(userId.Value);

                IQueryable<StatisticalFormDTO> result = from form in query
                                                        where (form.RegistryNumber.ToLower().Contains(text)
                                                                  || form.Year.ToString().Contains(text)
                                                                  || (searchDate.HasValue && form.SubmissionDate == searchDate.Value)
                                                                  || form.FormTypeName.ToLower().Contains(text)
                                                                  || form.FormObject.ToLower().Replace(" ", "").Contains(text.Replace(" ", "")))
                                                              && (form.SubmittedByUserId == userId.Value
                                                                || personIds.Contains(form.SubmittedByPersonId.Value))
                                                        select new StatisticalFormDTO
                                                        {
                                                            Id = form.Id,
                                                            RegistryNumber = form.RegistryNumber,
                                                            Year = form.Year,
                                                            SubmissionDate = form.SubmissionDate,
                                                            FormObject = form.FormObject,
                                                            FormTypeName = form.FormTypeName,
                                                            FormType = Enum.Parse<StatisticalFormTypesEnum>(form.FormTypeCode),
                                                            IsActive = form.IsActive
                                                        };

                return result;
            }
            else
            {
                if (filters.FilterAquaFarmTerritoryUnit.HasValue && filters.FilterAquaFarmTerritoryUnit.Value)
                {
                    query = query.Where(x => x.FormTypeCode != nameof(StatisticalFormTypesEnum.AquaFarm) || x.TerritoryUnitId == filters.TerritoryUnitId.Value);
                }

                if (filters.FilterReworkTerritoryUnit.HasValue && filters.FilterReworkTerritoryUnit.Value)
                {
                    query = query.Where(x => x.FormTypeCode != nameof(StatisticalFormTypesEnum.Rework) || x.TerritoryUnitId == filters.TerritoryUnitId.Value);
                }

                if (filters.FilterFishVesselTerritoryUnit.HasValue && filters.FilterFishVesselTerritoryUnit.Value)
                {
                    query = query.Where(x => x.FormTypeCode != nameof(StatisticalFormTypesEnum.FishVessel) || x.TerritoryUnitId == filters.TerritoryUnitId.Value);
                }

                IQueryable<StatisticalFormDTO> result = from form in query
                                                        join processUser in Db.Users on form.AssignedUserId equals processUser.Id
                                                        join processPerson in Db.Persons on processUser.PersonId equals processPerson.Id
                                                        where form.RegistryNumber.ToLower().Contains(text)
                                                                || processPerson.FirstName.ToLower().Contains(text)
                                                                || processPerson.LastName.ToLower().Contains(text)
                                                                || form.Year.ToString().Contains(text)
                                                                || (searchDate.HasValue && form.SubmissionDate == searchDate.Value)
                                                                || form.FormTypeName.ToLower().Contains(text)
                                                                || form.FormObject.ToLower().Replace(" ", "").Contains(text.Replace(" ", ""))
                                                        select new StatisticalFormDTO
                                                        {
                                                            Id = form.Id,
                                                            RegistryNumber = form.RegistryNumber,
                                                            ProcessUser = processPerson.FirstName + " " + processPerson.LastName,
                                                            Year = form.Year,
                                                            SubmissionDate = form.SubmissionDate,
                                                            FormObject = form.FormObject,
                                                            FormTypeName = form.FormTypeName,
                                                            FormType = Enum.Parse<StatisticalFormTypesEnum>(form.FormTypeCode),
                                                            IsActive = form.IsActive
                                                        };

                return result;
            }
        }

        private List<int> GetUserPersonIds(int userId)
        {
            var egnLnc = (from user in Db.Users
                          join person in Db.Persons on user.PersonId equals person.Id
                          where user.Id == userId
                          select new
                          {
                              person.EgnLnc,
                              person.IdentifierType
                          }).First();

            List<int> personIds = (from person in Db.Persons
                                   where person.EgnLnc == egnLnc.EgnLnc
                                        && person.IdentifierType == egnLnc.IdentifierType
                                   select person.Id).ToList();

            return personIds;
        }

        private void AddNumericStatValues(StatisticalFormsRegister form, List<StatisticalFormNumStatGroupDTO> groups)
        {
            foreach (StatisticalFormNumStatGroupDTO group in groups)
            {
                foreach (StatisticalFormNumStatDTO item in group.NumericStatTypes)
                {
                    EmployeeStatNumericValue entry = new EmployeeStatNumericValue
                    {
                        StatisticalForm = form,
                        NumericStatTypeId = item.Id,
                        StatValue = item.StatValue
                    };

                    Db.EmployeeStatNumericValues.Add(entry);
                }
            }
        }

        private void EditNumericStatValues(int formId, List<StatisticalFormNumStatGroupDTO> groups)
        {
            List<EmployeeStatNumericValue> dbEntries = (from numValue in Db.EmployeeStatNumericValues
                                                        where numValue.StatisticalFormId == formId
                                                        select numValue).ToList();

            foreach (StatisticalFormNumStatGroupDTO group in groups)
            {
                foreach (StatisticalFormNumStatDTO item in group.NumericStatTypes)
                {
                    EmployeeStatNumericValue dbEntry = dbEntries.Where(x => x.NumericStatTypeId == item.Id).SingleOrDefault();

                    if (dbEntry != null)
                    {
                        dbEntry.StatValue = item.StatValue;
                    }
                    else
                    {
                        EmployeeStatNumericValue entry = new EmployeeStatNumericValue
                        {
                            StatisticalFormId = formId,
                            NumericStatTypeId = item.Id,
                            StatValue = item.StatValue
                        };

                        Db.EmployeeStatNumericValues.Add(entry);
                    }
                }
            }
        }

        private void AddEmployeeCounts(StatisticalFormsRegister form, List<StatisticalFormEmployeeInfoGroupDTO> employeeCounts)
        {
            foreach (StatisticalFormEmployeeInfoGroupDTO group in employeeCounts)
            {
                foreach (StatisticalFormEmployeeInfoDTO count in group.EmployeeTypes)
                {
                    EmployeeStatCount entry = new EmployeeStatCount
                    {
                        StatisticalForm = form,
                        EmployeeStatTypeId = count.Id,
                        MenWithPayCount = count.MenWithPay,
                        MenWithouPayCount = count.MenWithoutPay,
                        WomenWithPayCount = count.WomenWithPay,
                        WomenWithoutPayCount = count.WomenWithoutPay
                    };

                    Db.EmployeeStatCounts.Add(entry);
                }
            }
        }

        private void EditEmployeeCounts(int formId, List<StatisticalFormEmployeeInfoGroupDTO> employeeCounts)
        {
            List<EmployeeStatCount> dbEntries = (from empCount in Db.EmployeeStatCounts
                                                 where empCount.StatisticalFormId == formId
                                                 select empCount).ToList();

            foreach (StatisticalFormEmployeeInfoGroupDTO group in employeeCounts)
            {
                foreach (StatisticalFormEmployeeInfoDTO count in group.EmployeeTypes)
                {
                    EmployeeStatCount dbEntry = dbEntries.Where(x => x.EmployeeStatTypeId == count.Id).SingleOrDefault();

                    if (dbEntry != null)
                    {
                        dbEntry.MenWithPayCount = count.MenWithPay;
                        dbEntry.MenWithouPayCount = count.MenWithoutPay;
                        dbEntry.WomenWithPayCount = count.WomenWithPay;
                        dbEntry.WomenWithoutPayCount = count.WomenWithoutPay;
                    }
                    else
                    {
                        EmployeeStatCount entry = new EmployeeStatCount
                        {
                            StatisticalFormId = formId,
                            EmployeeStatTypeId = count.Id,
                            MenWithPayCount = count.MenWithPay,
                            MenWithouPayCount = count.MenWithoutPay,
                            WomenWithPayCount = count.WomenWithPay,
                            WomenWithoutPayCount = count.WomenWithoutPay
                        };

                        Db.EmployeeStatCounts.Add(entry);
                    }
                }
            }
        }

        private List<StatisticalFormNumStatGroupDTO> GetNumericStatValueGroups(int? formId, int formTypeId)
        {
            DateTime now = DateTime.Now;

            List<StatisticalFormNumStatGroupDTO> groups = (from statForm in Db.MapStatFormTypesNumericStatTypeGroups
                                                           join statGroup in Db.NnumericStatTypeGroups on statForm.NumericStatTypeGroupId equals statGroup.Id
                                                           where statForm.StatFormTypeId == formTypeId
                                                                && statForm.ValidFrom <= now && statForm.ValidTo > now
                                                                && statGroup.ValidFrom <= now && statGroup.ValidTo > now
                                                           select new StatisticalFormNumStatGroupDTO
                                                           {
                                                               Id = statGroup.Id,
                                                               GroupName = statGroup.Name,
                                                               GroupType = Enum.Parse<NumericStatTypeGroupsEnum>(statGroup.Code),
                                                               StatFormTypeId = formTypeId,
                                                               IsActive = true
                                                           }).ToList();

            List<int> groupIds = groups.Select(x => x.Id).ToList();

            ILookup<int, StatisticalFormNumStatDTO> groupNumericStats = (from statGroup in Db.NnumericStatTypeGroups
                                                                         join statType in Db.NnumericStatTypes on statGroup.Id equals statType.GroupId
                                                                         where groupIds.Contains(statGroup.Id)
                                                                         orderby statGroup.OrderNum
                                                                         select new
                                                                         {
                                                                             GroupId = statGroup.Id,
                                                                             NumericStat = new StatisticalFormNumStatDTO
                                                                             {
                                                                                 Id = statType.Id,
                                                                                 GroupId = statGroup.Id,
                                                                                 StatFormId = formId,
                                                                                 Name = statType.Name,
                                                                                 Code = statType.Code,
                                                                                 DataType = statType.DataType,
                                                                                 OrderNum = statType.OrderNum,
                                                                                 IsActive = true
                                                                             }
                                                                         }).ToLookup(x => x.GroupId, y => y.NumericStat);

            List<int> numericStatTypeIds = groupNumericStats.SelectMany(x => x.Select(y => y.Id)).ToList();

            if (formId != null)
            {
                Dictionary<int, decimal?> numericStatTypeValue = (from statForm in Db.StatisticalFormsRegister
                                                                  join statNumbericValue in Db.EmployeeStatNumericValues on statForm.Id equals statNumbericValue.StatisticalFormId
                                                                  join numericStatType in Db.NnumericStatTypes on statNumbericValue.NumericStatTypeId equals numericStatType.Id
                                                                  where statForm.Id == formId && numericStatTypeIds.Contains(numericStatType.Id)
                                                                  select new
                                                                  {
                                                                      NumericStatTypeId = numericStatType.Id,
                                                                      StatValue = statNumbericValue.StatValue
                                                                  }).ToDictionary(x => x.NumericStatTypeId, y => y.StatValue);

                foreach (StatisticalFormNumStatGroupDTO group in groups)
                {
                    group.NumericStatTypes = groupNumericStats[group.Id].OrderBy(x => x.OrderNum).ToList();

                    foreach (StatisticalFormNumStatDTO numericStatType in group.NumericStatTypes)
                    {
                        if (numericStatTypeValue.TryGetValue(numericStatType.Id, out decimal? value))
                        {
                            numericStatType.StatValue = numericStatTypeValue[numericStatType.Id];
                        }
                    }
                }
            }
            else
            {
                foreach (StatisticalFormNumStatGroupDTO group in groups)
                {
                    group.NumericStatTypes = groupNumericStats[group.Id].OrderBy(x => x.OrderNum).ToList();
                }
            }

            return groups;
        }

        private List<StatisticalFormEmployeeInfoGroupDTO> GetEmployeeCountGroups(int? formId, int formTypeId)
        {
            DateTime now = DateTime.Now;

            List<StatisticalFormEmployeeInfoGroupDTO> results = (from statForm in Db.MapStatFormTypesEmployeeStatTypeGroups
                                                                 join empGroup in Db.NemployeeStatTypeGroups on statForm.EmployeeStatTypeGroupId equals empGroup.Id
                                                                 where statForm.StatFormTypeId == formTypeId
                                                                 orderby empGroup.OrderNum
                                                                 select new StatisticalFormEmployeeInfoGroupDTO
                                                                 {
                                                                     Id = empGroup.Id,
                                                                     GroupName = empGroup.Name,
                                                                     StatFormTypeId = formTypeId,
                                                                     IsActive = statForm.ValidFrom <= now && statForm.ValidTo > now
                                                                           && empGroup.ValidFrom <= now && empGroup.ValidTo > now
                                                                 }).ToList();

            List<int> groupIds = results.Select(x => x.Id).ToList();

            ILookup<int, StatisticalFormEmployeeInfoDTO> groupEmployeeInfos = (from statForm in Db.MapStatFormTypesEmployeeStatTypeGroups
                                                                               join empGroup in Db.NemployeeStatTypeGroups on statForm.EmployeeStatTypeGroupId equals empGroup.Id
                                                                               join type in Db.NemployeeStatTypes on empGroup.Id equals type.GroupId
                                                                               where groupIds.Contains(empGroup.Id) && statForm.StatFormTypeId == formTypeId
                                                                               orderby empGroup.OrderNum
                                                                               select new
                                                                               {
                                                                                   GroupId = empGroup.Id,
                                                                                   EmployeeInfos = new StatisticalFormEmployeeInfoDTO
                                                                                   {
                                                                                       Id = type.Id,
                                                                                       GroupId = empGroup.Id,
                                                                                       StatFormId = formId,
                                                                                       Name = type.Name,
                                                                                       Code = type.Code,
                                                                                       OrderNum = type.OrderNum,
                                                                                       IsActive = type.ValidFrom < now && type.ValidTo > now
                                                                                   }
                                                                               }).ToLookup(x => x.GroupId, y => y.EmployeeInfos);

            List<int> employeeInfoTypeIds = groupEmployeeInfos.SelectMany(x => x.Select(y => y.Id)).ToList();

            var employeeInfoTypeValue = (from statForm in Db.StatisticalFormsRegister
                                         join employeeCount in Db.EmployeeStatCounts on statForm.Id equals employeeCount.StatisticalFormId
                                         join employeeInfoType in Db.NemployeeStatTypes on employeeCount.EmployeeStatTypeId equals employeeInfoType.Id
                                         where statForm.Id == formId && employeeInfoTypeIds.Contains(employeeInfoType.Id)
                                         select new
                                         {
                                             EmployeeInfoTypeId = employeeInfoType.Id,
                                             Counts = new
                                             {
                                                 MenWithPay = employeeCount.MenWithPayCount,
                                                 MenWithoutPay = employeeCount.MenWithouPayCount,
                                                 WomenWithPay = employeeCount.WomenWithPayCount,
                                                 WomenWithoutPay = employeeCount.WomenWithoutPayCount
                                             }
                                         }).ToDictionary(x => x.EmployeeInfoTypeId, y => y.Counts);

            foreach (StatisticalFormEmployeeInfoGroupDTO group in results)
            {
                group.EmployeeTypes = groupEmployeeInfos[group.Id].ToList();
                foreach (StatisticalFormEmployeeInfoDTO employeeInfoType in group.EmployeeTypes)
                {
                    if (employeeInfoTypeValue.ContainsKey(employeeInfoType.Id))
                    {
                        employeeInfoType.MenWithPay = employeeInfoTypeValue[employeeInfoType.Id].MenWithPay;
                        employeeInfoType.MenWithoutPay = employeeInfoTypeValue[employeeInfoType.Id].MenWithoutPay;
                        employeeInfoType.WomenWithPay = employeeInfoTypeValue[employeeInfoType.Id].WomenWithPay;
                        employeeInfoType.WomenWithoutPay = employeeInfoTypeValue[employeeInfoType.Id].WomenWithoutPay;
                    }
                }
            }

            return results;
        }

        private Dictionary<StatisticalFormTypesEnum, int> GetStatisticalFormTypesCodeToIdDictionary()
        {
            Dictionary<StatisticalFormTypesEnum, int> result = (from type in Db.NstatisticalFormTypes
                                                                select new
                                                                {
                                                                    Type = Enum.Parse<StatisticalFormTypesEnum>(type.Code),
                                                                    type.Id
                                                                }).ToDictionary(x => x.Type, y => y.Id);
            return result;
        }
    }
}
