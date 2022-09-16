using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public class ApplicationsRegisterService : Service, IApplicationsRegisterService
    {
        private readonly IUserService userService;
        public ApplicationsRegisterService(IARADbContext db, IUserService userService)
            : base(db)
        {
            this.userService = userService;
        }

        public IQueryable<ApplicationRegisterDTO> GetAllApplications(ApplicationsRegisterFilters filters,
                                                                     int? requesterUserId,
                                                                     PageCodeEnum[] permittedCodesReadAll = null,
                                                                     List<PageCodeEnum> permittedCodesRead = null)
        {
            var query = from app in Db.Applications
                        join appType in Db.NapplicationTypes on app.ApplicationTypeId equals appType.Id
                        join paymentStatus in Db.NPaymentStatuses on app.PaymentStatusId equals paymentStatus.Id
                        join applHierrType in Db.NapplicationStatusHierarchyTypes on app.ApplicationStatusHierTypeId equals applHierrType.Id
                        join appStatus in Db.NapplicationStatuses on app.ApplicationStatusId equals appStatus.Id
                        join submittedPerson in Db.Persons on app.SubmittedForPersonId equals submittedPerson.Id into left
                        from submittedPerson in left.DefaultIfEmpty()
                        join submittedLegal in Db.Legals on app.SubmittedForLegalId equals submittedLegal.Id into left1
                        from submittedLegal in left1.DefaultIfEmpty()
                        join assignedUser in Db.Users on app.AssignedUserId equals assignedUser.Id into left2
                        from assignedUser in left2.DefaultIfEmpty()
                        join assignedPerson in Db.Persons on assignedUser.PersonId equals assignedPerson.Id into left3
                        from assignedPerson in left3.DefaultIfEmpty()
                        select new
                        {
                            app.Id,
                            app.AccessCode,
                            app.SubmitDateTime,
                            OrderByDate = app.UpdatedOn.HasValue ? app.UpdatedOn.Value : app.SubmitDateTime,
                            app.EventisNum,
                            SubmittedByPersonId = app.SubmittedByPersonId,
                            SubmittedByUserId = app.SubmittedByUserId,
                            SubmittedForPerson = submittedPerson != null ? submittedPerson.FirstName + " " + submittedPerson.LastName : null,
                            SubmittedForLegal = submittedLegal != null ? submittedLegal.Name : null,
                            SubmittedForEgnLnc = submittedPerson != null ? submittedPerson.EgnLnc : null,
                            SubmittedForEik = submittedLegal != null ? submittedLegal.Eik : null,
                            Type = app.ApplicationType,
                            SourceName = applHierrType.Name,
                            SourceId = applHierrType.Id,
                            SourceCode = applHierrType.Code,
                            StatusId = appStatus.Id,
                            StatusCode = appStatus.Code,
                            StatusName = appStatus.Name,
                            StatusReason = app.StatusReason,
                            AssignedUser = assignedPerson != null ? assignedPerson.FirstName + " " + assignedPerson.LastName : null,
                            AssignedUserId = app.AssignedUserId,
                            app.IsActive,
                            app.PaymentStatus,
                            appType.PageCode,
                            app.TerritoryUnitId
                        };

            //ORDERING
            query = query.OrderByDescending(x => x.OrderByDate);

            if (permittedCodesReadAll != null && permittedCodesReadAll.Length > 0)
            {
                string[] codes = permittedCodesReadAll.Select(x => x.ToString()).ToArray();

                query = query.Where(x => codes.Contains(x.PageCode));
            }
            else if (requesterUserId.HasValue)
            {
                int personId = (from user in Db.Users
                                where user.Id == requesterUserId.Value
                                select user.PersonId).First();

                query = query.Where(x => (x.SubmittedByPersonId == personId || x.SubmittedByUserId == requesterUserId.Value)
                                       && x.SourceCode != nameof(ApplicationHierarchyTypesEnum.RecreationalFishingTicket));
            }
            else
            {
                query = query.Where(x => x.SourceCode != nameof(ApplicationHierarchyTypesEnum.RecreationalFishingTicket));
            }

            if (permittedCodesRead != null && permittedCodesRead.Count > 0 && filters != null && filters.TerritoryUnitId.HasValue)
            {
                string[] codes = permittedCodesRead.Select(x => x.ToString()).ToArray();
                query = query.Where(x => !codes.Contains(x.PageCode) || x.TerritoryUnitId == filters.TerritoryUnitId.Value);
            }
            else if (filters != null && filters.TerritoryUnitId.HasValue)
            {
                query = query.Where(x => x.AssignedUserId.HasValue && x.TerritoryUnitId == filters.TerritoryUnitId.Value);
            }

            if (filters == null || !filters.HasAnyFilters())
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;

                query = query.Where(x => x.IsActive == !showInactive);
            }
            else
            {
                if (!string.IsNullOrEmpty(filters.FreeTextSearch))
                {
                    filters.FreeTextSearch = filters.FreeTextSearch.ToLower();
                    DateTime? searchDate = DateTimeUtils.TryParseDate(filters.FreeTextSearch);
                    bool? showOnlyNotFinished = filters.ShowOnlyNotFinished;
                    string text = filters.FreeTextSearch;

                    query = query.Where(app => app.IsActive == !filters.ShowInactiveRecords
                                                && ((showOnlyNotFinished.HasValue
                                                    && showOnlyNotFinished.Value
                                                    && app.StatusCode != nameof(ApplicationStatusesEnum.ADM_ACT_COMPLETION)
                                                    && app.StatusCode != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                                                    ) || !showOnlyNotFinished.HasValue || !showOnlyNotFinished.Value)
                                                && (app.AccessCode.ToLower().Contains(text)
                                                    || app.EventisNum.ToLower().Contains(text)
                                                    || app.SubmittedForLegal.ToLower().Contains(text)
                                                    || app.SubmittedForPerson.ToLower().Contains(text)
                                                    || app.Type.Name.ToLower().Contains(text)
                                                    || app.SourceName.ToLower().Contains(text)
                                                    || app.StatusName.ToLower().Contains(text)
                                                    || app.SourceName.ToLower().Contains(text)
                                                    || (app.AssignedUser != null && app.AssignedUser.ToLower().Contains(text))
                                                    || (searchDate.HasValue && app.SubmitDateTime.Date == searchDate.Value.Date)));
                }
                else
                {
                    if (!string.IsNullOrEmpty(filters.AccessCode))
                    {
                        query = query.Where(x => x.AccessCode.ToLower().Contains(filters.AccessCode.ToLower()));
                    }

                    if (!string.IsNullOrEmpty(filters.EventisNum))
                    {
                        query = query.Where(x => x.EventisNum.ToLower().Contains(filters.EventisNum.ToLower()));
                    }

                    if (filters.ApplicationTypeId.HasValue)
                    {
                        query = query.Where(x => x.Type.Id == filters.ApplicationTypeId.Value);
                    }

                    if (filters.ApplicationStatusId.HasValue)
                    {
                        query = query.Where(x => x.StatusId == filters.ApplicationStatusId.Value);
                    }

                    if (!string.IsNullOrEmpty(filters.SubmittedFor))
                    {
                        query = query.Where(x => x.SubmittedForPerson.ToLower().Contains(filters.SubmittedFor.ToLower())
                                              || x.SubmittedForLegal.ToLower().Contains(filters.SubmittedFor.ToLower()));
                    }

                    if (!string.IsNullOrEmpty(filters.SubmittedForEgnLnc))
                    {
                        query = query.Where(x => x.SubmittedForEgnLnc == filters.SubmittedForEgnLnc);
                    }

                    if (filters.ApplicationSourceId.HasValue)
                    {
                        query = query.Where(x => x.SourceId == filters.ApplicationSourceId.Value);
                    }

                    if (filters.ShowAssignedApplications.HasValue)
                    {
                        query = filters.ShowAssignedApplications.Value == true
                            ? query.Where(x => x.AssignedUser != null)
                            : query.Where(x => x.AssignedUser == null);
                    }

                    if (!string.IsNullOrEmpty(filters.AssignedTo))
                    {
                        query = query.Where(x => x.AssignedUser != null && x.AssignedUser.ToLower().Contains(filters.AssignedTo.ToLower()));
                    }

                    if (filters.ShowOnlyNotFinished.HasValue && filters.ShowOnlyNotFinished.Value)
                    {
                        query = query.Where(x => x.StatusCode != nameof(ApplicationStatusesEnum.ADM_ACT_COMPLETION)
                                              && x.StatusCode != nameof(ApplicationStatusesEnum.CANCELED_APPL));
                    }

                    if (filters.DateFrom.HasValue)
                    {
                        query = query.Where(x => x.SubmitDateTime >= filters.DateFrom);
                    }

                    if (filters.DateTo.HasValue)
                    {
                        query = query.Where(x => x.SubmitDateTime <= filters.DateTo);
                    }
                }
            }

            return query.Select(application => new ApplicationRegisterDTO
            {
                ID = application.Id,
                AccessCode = application.AccessCode,
                EventisNum = application.EventisNum,
                SubmitDateTime = application.SubmitDateTime,
                SubmittedFor = application.SubmittedForPerson != null ? application.SubmittedForPerson : application.SubmittedForLegal,
                Type = application.Type.Name,
                SourceName = application.SourceName,
                SourceCode = Enum.Parse<ApplicationHierarchyTypesEnum>(application.SourceCode),
                StatusName = application.StatusName,
                StatusCode = Enum.Parse<ApplicationStatusesEnum>(application.StatusCode),
                StatusReason = application.StatusReason,
                AssignedUser = application.AssignedUser,
                AssignedUserId = application.AssignedUserId,
                IsActive = application.IsActive,
                PaymentStatus = Enum.Parse<PaymentStatusesEnum>(application.PaymentStatus.Code),
                PageCode = Enum.Parse<PageCodeEnum>(application.PageCode)
            });
        }

        public IEnumerable<ApplicationsChangeHistoryDTO> GetApplicationChangeHistoryRecordsForTable(IEnumerable<int> applicationIds)
        {
            DateTime now = DateTime.Now;
            List<ApplicationsChangeHistoryDTO> results = (from applHistory in Db.ApplicationChangeHistories
                                                          join tUnit in Db.NterritoryUnits on applHistory.TerritoryUnitId equals tUnit.Id into terrUnit
                                                          from territoryUnit in terrUnit.DefaultIfEmpty()
                                                          join status in Db.NapplicationStatuses on applHistory.ApplicationStatusId equals status.Id
                                                          join payStatus in Db.NPaymentStatuses on applHistory.PaymentStatusId equals payStatus.Id
                                                          where applicationIds.Contains(applHistory.ApplicationId)
                                                          orderby applHistory.ModifiedDateTime descending
                                                          select new ApplicationsChangeHistoryDTO
                                                          {
                                                              Id = applHistory.Id,
                                                              ApplicationId = applHistory.ApplicationId,
                                                              AssignedUserId = applHistory.AssignedUserId,
                                                              ModifiedDate = applHistory.ModifiedDateTime,
                                                              ModifiedByUserId = applHistory.ModifiedByUserId,
                                                              PaymentStatus = payStatus.Name,
                                                              PaymentStatusCode = payStatus.Code,
                                                              StatusName = status.Name,
                                                              TerritoryUnitName = territoryUnit != null ? territoryUnit.Name : "",
                                                              StatusReason = applHistory.StatusReason,
                                                              HasApplicationDraftContent = !string.IsNullOrEmpty(applHistory.ApplicationDraftContents)
                                                          }).ToList();

            HashSet<int> modifiedByUserIds = results.Select(x => x.ModifiedByUserId).ToHashSet<int>();
            HashSet<int> assignedToUserIds = results.Where(x => x.AssignedUserId.HasValue).Select(x => x.AssignedUserId.Value).ToHashSet<int>();

            var modifiedByUserNames = (from user in Db.Users
                                       join userPerson in Db.Persons on user.PersonId equals userPerson.Id
                                       join person in Db.Persons on new
                                       {
                                           userPerson.EgnLnc,
                                           userPerson.IdentifierType
                                       } equals new
                                       {
                                           person.EgnLnc,
                                           person.IdentifierType
                                       }
                                       where modifiedByUserIds.Contains(user.Id) && person.ValidFrom <= now && person.ValidTo > now
                                       select new
                                       {
                                           ModifiedByUserId = user.Id,
                                           Name = $"{person.FirstName} {person.LastName}"
                                       }).ToDictionary(x => x.ModifiedByUserId, y => y.Name);

            var assignedToUserNames = (from user in Db.Users
                                       join userPerson in Db.Persons on user.PersonId equals userPerson.Id
                                       join person in Db.Persons on new
                                       {
                                           userPerson.EgnLnc,
                                           userPerson.IdentifierType
                                       } equals new
                                       {
                                           person.EgnLnc,
                                           person.IdentifierType
                                       }
                                       where assignedToUserIds.Contains(user.Id) && person.ValidFrom <= now && person.ValidTo > now
                                       select new
                                       {
                                           AssignedUserId = user.Id,
                                           Name = $"{person.FirstName} {person.LastName}"
                                       }).ToDictionary(x => x.AssignedUserId, y => y.Name);

            foreach (ApplicationsChangeHistoryDTO changeHistory in results)
            {
                changeHistory.ModifiedByUserName = modifiedByUserNames[changeHistory.ModifiedByUserId];

                if (changeHistory.AssignedUserId.HasValue)
                {
                    changeHistory.AssingedUserName = assignedToUserNames[changeHistory.AssignedUserId.Value];
                }
            }

            return results;
        }

        public void DeleteApplication(int id)
        {
            DeleteRecordWithId(Db.Applications, id);
            Db.SaveChanges();
        }

        public void UndoDeleteApplication(int id)
        {
            UndoDeleteRecordWithId(Db.Applications, id);
            Db.SaveChanges();
        }

        public List<NomenclatureDTO> GetApplicationTypes(params PageCodeEnum[] pageCodes)
        {
            string[] codes = pageCodes.Select(x => x.ToString()).ToArray();

            DateTime now = DateTime.Now;

            List<NomenclatureDTO> types = (from type in Db.NapplicationTypes
                                           where type.PageCode != nameof(PageCodeEnum.RecFish)
                                                && (codes.Length == 0 || codes.Contains(type.PageCode))
                                           orderby type.Name
                                           select new NomenclatureDTO
                                           {
                                               Value = type.Id,
                                               DisplayName = type.Name,
                                               IsActive = type.ValidFrom <= now && type.ValidTo > now
                                           }).ToList();
            return types;
        }

        public List<NomenclatureDTO> GetApplicationStatuses(params ApplicationHierarchyTypesEnum[] hierarchyTypes)
        {
            DateTime now = DateTime.Now;

            IQueryable<NomenclatureDTO> query = from status in Db.NapplicationStatuses
                                                orderby status.Name
                                                select new NomenclatureDTO
                                                {
                                                    Value = status.Id,
                                                    DisplayName = status.Name,
                                                    IsActive = status.ValidFrom < now && status.ValidTo >= now
                                                };

            if (hierarchyTypes?.Length > 0)
            {
                List<string> types = hierarchyTypes.Select(x => x.ToString()).ToList();
                List<int> hierarchyTypeIds = (from hierarchyType in Db.NapplicationStatusHierarchyTypes
                                              where types.Contains(hierarchyType.Code)
                                              select hierarchyType.Id).ToList();

                List<Tuple<int, int>> statusIdPairs = (from statusHierarchy in Db.NapplicationStatusHierarchies
                                                       where hierarchyTypeIds.Contains(statusHierarchy.ApplicationStatusHierTypeId)
                                                            && statusHierarchy.ValidFrom < now
                                                            && statusHierarchy.ValidTo >= now
                                                       select new Tuple<int, int>(statusHierarchy.ParentStatusId, statusHierarchy.ChildStatusId)).ToList();

                List<int> statusIds = new List<int>();
                foreach (Tuple<int, int> statusPair in statusIdPairs)
                {
                    statusIds.Add(statusPair.Item1);
                    statusIds.Add(statusPair.Item2);
                }

                query = query.Where(x => statusIds.Contains(x.Value));
            }

            List<NomenclatureDTO> result = query.ToList();
            return result;
        }

        public List<NomenclatureDTO> GetApplicationSources(params ApplicationHierarchyTypesEnum[] hierarchyTypes)
        {
            DateTime now = DateTime.Now;

            IQueryable<NomenclatureDTO> query = from hierarchyType in Db.NapplicationStatusHierarchyTypes
                                                select new NomenclatureDTO
                                                {
                                                    Value = hierarchyType.Id,
                                                    Code = hierarchyType.Code,
                                                    DisplayName = hierarchyType.Name,
                                                    IsActive = hierarchyType.ValidFrom <= now && hierarchyType.ValidTo > now
                                                };

            if (hierarchyTypes?.Length > 0)
            {
                List<string> types = hierarchyTypes.Select(x => x.ToString()).ToList();

                query = query.Where(x => types.Contains(x.Code));
            }

            List<NomenclatureDTO> result = query.ToList();
            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.Applications, id);
        }

        public SimpleAuditDTO GetApplicationHistorySimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues<ApplicationChangeHistory>(Db.ApplicationChangeHistories, id);
        }


    }
}
