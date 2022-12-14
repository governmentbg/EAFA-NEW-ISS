using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Inspections.API;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.EventArgs;
using IARA.Mobile.Insp.Application.Filters;
using IARA.Mobile.Insp.Application.Interfaces.Database;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Application.Transactions.Base;
using IARA.Mobile.Insp.Domain.Entities.Inspections;
using IARA.Mobile.Insp.Domain.Entities.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace IARA.Mobile.Insp.Application.Transactions
{
    public class InspectionsTransaction : BaseTransaction, IInspectionsTransaction
    {
        private static bool IsSendingOfflineInspection = false;

        private const string UrlPrefix = "Inspections/";
        private readonly IOfflineFiles _offlineFiles;
        private readonly IMessagingCenter _messagingCenter;

        public InspectionsTransaction(IOfflineFiles offlineFiles, IMessagingCenter messagingCenter, BaseTransactionProvider provider)
            : base(provider)
        {
            _offlineFiles = offlineFiles;
            _messagingCenter = messagingCenter;
        }

        public async Task<FileResponse> GetFile(int fileId)
        {
            HttpResult<FileResponse> result = await RestClient.GetAsync<FileResponse>(UrlPrefix + "DownloadFile", new { id = fileId });

            if (result.IsSuccessful)
            {
                return result.Content;
            }

            return null;
        }

        public InspectorDuringInspectionDto GetInspector(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from insp in context.Inspectors
                    where insp.Id == id
                    select new InspectorDuringInspectionDto
                    {
                        InspectorId = insp.Id,
                        CitizenshipId = insp.CitizenshipId,
                        CardNum = insp.CardNum,
                        InstitutionId = insp.InstitutionId,
                        IsNotRegistered = insp.IsNotRegistered,
                        FirstName = insp.FirstName,
                        MiddleName = insp.MiddleName,
                        LastName = insp.LastName,
                        UnregisteredPersonId = insp.UnregisteredPersonId,
                        UserId = insp.UserId
                    }
                ).FirstOrDefault();
            }
        }

        public InspectorDuringInspectionDto GetInspectorByUserId(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from insp in context.Inspectors
                    where insp.UserId == id
                    select new InspectorDuringInspectionDto
                    {
                        InspectorId = insp.Id,
                        CitizenshipId = insp.CitizenshipId,
                        CardNum = insp.CardNum,
                        InstitutionId = insp.InstitutionId,
                        IsNotRegistered = insp.IsNotRegistered,
                        FirstName = insp.FirstName,
                        MiddleName = insp.MiddleName,
                        LastName = insp.LastName,
                        UnregisteredPersonId = insp.UnregisteredPersonId,
                        UserId = insp.UserId
                    }
                ).FirstOrDefault();
            }
        }

        public List<InspectorDuringInspectionDto> GetInspectorHistory(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from inspHistory in context.InspectorsHistories
                    join recentInsp in context.RecentInspectors on inspHistory.Id equals recentInsp.HistoryId
                    join inspector in context.Inspectors on recentInsp.InspectorId equals inspector.Id
                    where inspHistory.Id == id
                    select new InspectorDuringInspectionDto
                    {
                        InspectorId = inspector.Id,
                        CitizenshipId = inspector.CitizenshipId,
                        CardNum = inspector.CardNum,
                        InstitutionId = inspector.InstitutionId,
                        IsNotRegistered = inspector.IsNotRegistered,
                        FirstName = inspector.FirstName,
                        MiddleName = inspector.MiddleName,
                        LastName = inspector.LastName,
                        UnregisteredPersonId = inspector.UnregisteredPersonId,
                        UserId = inspector.UserId
                    }
                ).ToList();
            }
        }

        public List<RecentInspectorDto> GetRecentInspectors()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                var result = (
                    from inspHistory in context.InspectorsHistories
                    join recentInsp in context.RecentInspectors on inspHistory.Id equals recentInsp.HistoryId
                    join inspector in context.Inspectors on recentInsp.InspectorId equals inspector.Id
                    select new
                    {
                        Id = inspHistory.Id,
                        Inspector = $"{inspector.FirstName} {(inspector.MiddleName == null ? string.Empty : inspector.MiddleName + " ")}{inspector.LastName}",
                    }
                ).ToList();

                return result.GroupBy(f => f.Id)
                    .Select(f => new RecentInspectorDto
                    {
                        Id = f.Key,
                        Inspectors = string.Join(", ", f
                            .Select(s => s.Inspector)
                            .ToArray()
                        )
                    }).ToList();
            }
        }

        public async Task<List<InspectionDto>> GetAll(int page)
        {
            if (IsSendingOfflineInspection)
            {
                await Task.Delay(1000);
            }

            if (!ContextBuilder.DatabaseExists)
            {
                return null;
            }

            if (CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected)
            {
                return PullInspectionsOffline(page);
            }

            DateTime? lastFetchDate = Settings.LastInspectionFetchDate;
            DateTime now = DateTime.Now;

            InspectionsFilters filters = new InspectionsFilters
            {
                UpdatedAfter = lastFetchDate,
                ShowBothActiveAndInactive = lastFetchDate.HasValue,
            };
            GridRequest<InspectionsFilters> gridRequest = new GridRequest<InspectionsFilters>(filters)
            {
                PageSize = int.MaxValue
            };

            HttpResult<List<InspectionApiDto>> result =
                await RestClient.PostAsync<GridResult<InspectionApiDto>>(UrlPrefix + "GetAll", gridRequest)
                    .GetGridRecords();

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return null;
            }

            if (result.IsSuccessful && result.Content?.Count > 0)
            {
                if (!ContextBuilder.DatabaseExists)
                {
                    return null;
                }

                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    List<NInspectionType> inspectionTypes = context.NInspectionTypes.ToList();
                    List<NInspectionState> inspectionStates = context.NInspectionStates.ToList();

                    Inspection MapInspectionDto(InspectionApiDto dto)
                    {
                        return new Inspection
                        {
                            Id = dto.Id,
                            InspectionState = dto.InspectionState,
                            InspectionStateId = inspectionStates.Find(s => s.Code == dto.InspectionState.ToString()).Id,
                            InspectionType = dto.InspectionType,
                            InspectionTypeId = inspectionTypes.Find(s => s.Code == dto.InspectionType.ToString()).Id,
                            IsLocal = false,
                            ReportNr = dto.ReportNumber,
                            StartDate = dto.StartDate,
                            SubmitType = dto.InspectionState == InspectionState.Draft ? SubmitType.Draft : SubmitType.Finish,
                            InspectionSubjects = string.Join(", ", dto.InspectionSubjects.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Distinct()),
                            Inspectors = dto.Inspectors,
                            LastUpdatedDate = dto.LastUpdateDate,
                        };
                    }

                    if (lastFetchDate.HasValue)
                    {
                        List<int> ids = result.Content.ConvertAll(f => f.Id);
                        List<Inspection> dbInspections = (
                            from insp in context.Inspections
                            where ids.Contains(insp.Id)
                            select insp
                        ).ToList();

                        List<(int, string)> toRemove = new List<(int, string)>();
                        List<int> idsToNotAdd = new List<int>();

                        foreach (InspectionApiDto inspection in result.Content)
                        {
                            Inspection dbInsp = dbInspections.Find(f => f.Id == inspection.Id);

                            if (dbInsp != null)
                            {
                                // Ако инспекцията от сървъра е приключена, тогава искаме да я изтрием локално.
                                // Ако тази локално е приключена също отново ще бъде изтрита.
                                if (inspection.InspectionState != InspectionState.Draft)
                                {
                                    toRemove.Add((dbInsp.Id, dbInsp.Identifier));
                                }
                                // Ако локалната инспекция все още не е качена искаме да не я трием / заменяме с тази от сървъра.
                                // Това ни оказва че имаме промяна която е само на устройството и не искаме да я загубим.
                                else if (dbInsp.IsLocal)
                                {
                                    idsToNotAdd.Add(dbInsp.Id);
                                }
                                // Ако има по-нова драфт версия на сървъра или ако е изтрита на сървъра я изтриваме и при нас.
                                // Ако сме стигнали до тука сме сигурни че и 2те инспекции са драфт.
                                else if (dbInsp.LastUpdatedDate < inspection.LastUpdateDate || !inspection.IsActive)
                                {
                                    toRemove.Add((dbInsp.Id, dbInsp.Identifier));
                                }
                                else
                                {
                                    idsToNotAdd.Add(dbInsp.Id);
                                }
                            }
                        }

                        if (toRemove.Count > 0)
                        {
                            List<int> idsToRemove = toRemove.ConvertAll(f => f.Item1);
                            context.Inspections.Delete(f => idsToRemove.Contains(f.Id));

                            foreach ((int id, string identifier) in toRemove)
                            {
                                if (!string.IsNullOrEmpty(identifier))
                                {
                                    _offlineFiles.DeleteFiles(identifier);
                                }
                            }
                        }

                        List<Inspection> addResult = result.Content
                            .Where(f => !idsToNotAdd.Contains(f.Id) && f.IsActive)
                            .Select(MapInspectionDto)
                            .ToList();

                        context.Inspections.AddRange(addResult);
                    }
                    else
                    {
                        context.Inspections.AddRange(result.Content.ConvertAll(MapInspectionDto));
                    }
                }
            }

            if (result.IsSuccessful)
            {
                Settings.LastInspectionFetchDate = now;
            }

            return PullInspectionsOffline(page);
        }

        public int GetPageCount()
        {
            if (!ContextBuilder.DatabaseExists)
            {
                return 0;
            }

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                double pageSize = CommonGlobalVariables.PullItemsCount;
                int count = context.Inspections.Count();

                return (int)Math.Ceiling((count > 1 ? count : 1) / pageSize);
            }
        }

        public Task<ObservationAtSeaDto> GetOFS(int id, bool isLocal)
        {
            return Get<ObservationAtSeaDto>(id, isLocal, InspectionType.OFS);
        }

        public Task<InspectionAtSeaDto> GetIBS(int id, bool isLocal)
        {
            return Get<InspectionAtSeaDto>(id, isLocal, InspectionType.IBS);
        }

        public Task<InspectionTransboardingDto> GetIBP(int id, bool isLocal)
        {
            return Get<InspectionTransboardingDto>(id, isLocal, InspectionType.IBP);
        }

        public Task<InspectionTransboardingDto> GetITB(int id, bool isLocal)
        {
            return Get<InspectionTransboardingDto>(id, isLocal, InspectionType.ITB);
        }

        public Task<InspectionTransportVehicleDto> GetIVH(int id, bool isLocal)
        {
            return Get<InspectionTransportVehicleDto>(id, isLocal, InspectionType.IVH);
        }

        public Task<InspectionFirstSaleDto> GetIFS(int id, bool isLocal)
        {
            return Get<InspectionFirstSaleDto>(id, isLocal, InspectionType.IFS);
        }

        public Task<InspectionAquacultureDto> GetIAQ(int id, bool isLocal)
        {
            return Get<InspectionAquacultureDto>(id, isLocal, InspectionType.IAQ);
        }

        public Task<InspectionFisherDto> GetIFP(int id, bool isLocal)
        {
            return Get<InspectionFisherDto>(id, isLocal, InspectionType.IFP);
        }

        public Task<InspectionCheckWaterObjectDto> GetCWO(int id, bool isLocal)
        {
            return Get<InspectionCheckWaterObjectDto>(id, isLocal, InspectionType.CWO);
        }

        public Task<InspectionCheckToolMarkDto> GetIGM(int id, bool isLocal)
        {
            return Get<InspectionCheckToolMarkDto>(id, isLocal, InspectionType.IGM);
        }

        public Task<InspectionConstativeProtocolDto> GetOTH(int id, bool isLocal)
        {
            return Get<InspectionConstativeProtocolDto>(id, isLocal, InspectionType.OTH);
        }

        public async Task<PostEnum> HandleInspection<TDto>(TDto dto, SubmitType submitType, bool fromOffline = false)
            where TDto : InspectionEditDto
        {
            Task<HttpResult<int>> taskResult;

            if (submitType == SubmitType.Draft || (dto.IsOfflineOnly && submitType == SubmitType.Edit))
            {
                taskResult = RestClient.PostAsFormDataAsync<int>(UrlPrefix + "Add", MapToDraftDto(dto));
            }
            else if (submitType == SubmitType.Finish)
            {
                taskResult = RestClient.PostAsFormDataAsync<int>(
                    UrlPrefix
                        + "Submit"
                        + dto.InspectionType.ToString(),
                    dto
                );
            }
            else
            {
                taskResult = RestClient.PutAsFormDataAsync<int>(UrlPrefix + "Edit", MapToDraftDto(dto));
            }

            HttpResult<int> result = await taskResult;

            if (result.IsSuccessful)
            {
                if (!fromOffline)
                {
                    dto.IsOfflineOnly = false;

                    using (IAppDbContext context = ContextBuilder.CreateContext())
                    {
                        Inspection localInspection = context.Inspections.FirstOrDefault(f => f.Id == dto.Id);
                        List<NInspectionState> inspectionStates = context.NInspectionStates.ToList();

                        if (localInspection != null)
                        {
                            if (submitType == SubmitType.Finish)
                            {
                                localInspection.SubmitType = SubmitType.Finish;
                            }

                            localInspection.InspectionState = dto.InspectionState;
                            localInspection.InspectionStateId = inspectionStates.Find(s => s.Code == dto.InspectionState.ToString()).Id;
                            localInspection.Inspectors = submitType == SubmitType.Finish
                                ? InspectorsToString(dto.Inspectors)
                                : string.Empty;
                            localInspection.InspectionSubjects = submitType == SubmitType.Finish
                                ? PersonnelToString(dto.Personnel)
                                : string.Empty;
                            localInspection.JsonContent = JsonSerializer.Serialize(dto, typeof(TDto));
                            localInspection.HasJsonContent = true;
                            localInspection.IsLocal = false;
                            localInspection.LastUpdatedDate = DateTime.Now;

                            context.Inspections.Update(localInspection);
                        }
                        else
                        {
                            List<NInspectionType> inspectionTypes = context.NInspectionTypes.ToList();

                            dto.Id = result.Content;

                            context.Inspections.Add(new Inspection
                            {
                                Id = result.Content,
                                IsLocal = false,
                                JsonContent = JsonSerializer.Serialize(dto, typeof(TDto)),
                                HasJsonContent = true,
                                Identifier = dto.LocalIdentifier,
                                SubmitType = submitType,
                                InspectionState = dto.InspectionState,
                                InspectionStateId = inspectionStates.Find(s => s.Code == dto.InspectionState.ToString()).Id,
                                InspectionType = dto.InspectionType,
                                InspectionTypeId = inspectionTypes.Find(s => s.Code == dto.InspectionType.ToString()).Id,
                                ReportNr = dto.ReportNum,
                                StartDate = dto.StartDate.Value,
                                Inspectors = submitType == SubmitType.Finish
                                    ? InspectorsToString(dto.Inspectors)
                                    : string.Empty,
                                InspectionSubjects = submitType == SubmitType.Finish
                                    ? PersonnelToString(dto.Personnel)
                                    : string.Empty,
                                LastUpdatedDate = DateTime.Now,
                            });
                        }
                    }
                }
                else if (dto.Id.HasValue && dto.Id.Value != 0 && result.Content != 0)
                {
                    _messagingCenter.SendMessage(new InspectionUploadedEventArgs
                    {
                        OldId = dto.Id.Value,
                        NewId = result.Content
                    });
                }

                SaveInspectionInspectors(dto.Inspectors);

                return PostEnum.Success;
            }
            else if (result.Error?.Code == ErrorCode.AlreadySubmitted)
            {
                if (!fromOffline)
                {
                    using (IAppDbContext context = ContextBuilder.CreateContext())
                    {
                        if (submitType == SubmitType.Edit)
                        {
                            context.Inspections.Delete(f => f.Id == dto.Id);
                        }
                        else
                        {
                            context.Inspections.Delete(f => f.Identifier == dto.LocalIdentifier);
                        }
                    }
                }

                return PostEnum.Success;
            }
            else if (CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected && !fromOffline)
            {
                dto.IsOfflineOnly = true;

                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    Inspection localInspection = context.Inspections.FirstOrDefault(f => f.Id == dto.Id);
                    List<NInspectionState> inspectionStates = context.NInspectionStates.ToList();

                    if (localInspection != null)
                    {
                        if (submitType == SubmitType.Finish)
                        {
                            localInspection.SubmitType = SubmitType.Finish;
                        }

                        localInspection.InspectionState = dto.InspectionState;
                        localInspection.InspectionStateId = inspectionStates.Find(s => s.Code == dto.InspectionState.ToString()).Id;
                        localInspection.Inspectors = submitType == SubmitType.Finish
                            ? InspectorsToString(dto.Inspectors)
                            : string.Empty;
                        localInspection.InspectionSubjects = submitType == SubmitType.Finish
                            ? PersonnelToString(dto.Personnel)
                            : string.Empty;
                        localInspection.JsonContent = JsonSerializer.Serialize(dto, typeof(TDto));
                        localInspection.HasJsonContent = true;
                        localInspection.LastUpdatedDate = DateTime.Now;

                        context.Inspections.Update(localInspection);
                    }
                    else
                    {
                        int id = (
                            from insp in context.Inspections
                            orderby insp.Id
                            select insp.Id
                        ).FirstOrDefault();

                        List<NInspectionType> inspectionTypes = context.NInspectionTypes.ToList();

                        dto.Id = id > 0 ? -1 : id - 1;

                        context.Inspections.Add(new Inspection
                        {
                            Id = dto.Id.Value,
                            IsLocal = true,
                            JsonContent = JsonSerializer.Serialize(dto, typeof(TDto)),
                            HasJsonContent = true,
                            Identifier = dto.LocalIdentifier,
                            SubmitType = submitType,
                            InspectionState = dto.InspectionState,
                            InspectionStateId = inspectionStates.Find(s => s.Code == dto.InspectionState.ToString()).Id,
                            InspectionType = dto.InspectionType,
                            InspectionTypeId = inspectionTypes.Find(s => s.Code == dto.InspectionType.ToString()).Id,
                            ReportNr = dto.ReportNum,
                            StartDate = dto.StartDate.Value,
                            Inspectors = submitType == SubmitType.Finish
                                ? InspectorsToString(dto.Inspectors)
                                : string.Empty,
                            InspectionSubjects = submitType == SubmitType.Finish
                                ? PersonnelToString(dto.Personnel)
                                : string.Empty,
                            LastUpdatedDate = DateTime.MinValue,
                        });
                    }
                }

                SaveInspectionInspectors(dto.Inspectors);

                return PostEnum.Offline;
            }
            else
            {
                return PostEnum.Failed;
            }
        }

        public async Task PostOfflineInspections()
        {
            List<Inspection> localInspections;

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                localInspections = context.Inspections
                    .Where(f => f.IsLocal)
                    .ToList();
            }

            if (localInspections.Count == 0)
            {
                return;
            }

            IsSendingOfflineInspection = true;

            try
            {
                Task<PostEnum>[] postTasks = new Task<PostEnum>[localInspections.Count];

                for (int i = 0; i < localInspections.Count; i++)
                {
                    Inspection inspection = localInspections[i];

                    InspectionEditDto dto = null;

                    switch (inspection.InspectionType)
                    {
                        case InspectionType.OFS:
                            dto = JsonSerializer.Deserialize<ObservationAtSeaDto>(inspection.JsonContent);
                            break;
                        case InspectionType.IBS:
                            dto = JsonSerializer.Deserialize<InspectionAtSeaDto>(inspection.JsonContent);
                            break;
                        case InspectionType.IBP:
                        case InspectionType.ITB:
                            dto = JsonSerializer.Deserialize<InspectionTransboardingDto>(inspection.JsonContent);
                            break;
                        case InspectionType.IVH:
                            dto = JsonSerializer.Deserialize<InspectionTransportVehicleDto>(inspection.JsonContent);
                            break;
                        case InspectionType.IFS:
                            dto = JsonSerializer.Deserialize<InspectionFirstSaleDto>(inspection.JsonContent);
                            break;
                        case InspectionType.IAQ:
                            dto = JsonSerializer.Deserialize<InspectionAquacultureDto>(inspection.JsonContent);
                            break;
                        case InspectionType.IFP:
                            dto = JsonSerializer.Deserialize<InspectionFisherDto>(inspection.JsonContent);
                            break;
                        case InspectionType.CWO:
                            dto = JsonSerializer.Deserialize<InspectionCheckWaterObjectDto>(inspection.JsonContent);
                            break;
                        case InspectionType.IGM:
                            dto = JsonSerializer.Deserialize<InspectionCheckToolMarkDto>(inspection.JsonContent);
                            break;
                    }

                    if (dto != null)
                    {
                        postTasks[i] = HandleInspection(dto, inspection.SubmitType, true);
                    }
                    else
                    {
                        postTasks[i] = Task.FromResult(PostEnum.Success);
                    }
                }

                PostEnum[] postResults = await Task.WhenAll(postTasks).ConfigureAwait(false);

                List<int> idsToRemove = new List<int>(localInspections.Count);

                for (int i = 0; i < localInspections.Count; i++)
                {
                    if (postResults[i] == PostEnum.Success)
                    {
                        Inspection inspection = localInspections[i];

                        idsToRemove.Add(inspection.Id);
                        _offlineFiles.DeleteFiles(inspection.Identifier);
                    }
                }

                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    context.Inspections.Delete(f => idsToRemove.Contains(f.Id));
                }
            }
            catch (Exception ex)
            {
                IsSendingOfflineInspection = false;
                throw new Exception("Posting offline inspections failed", ex);
            }

            IsSendingOfflineInspection = false;
        }

        public Task<bool> DeleteInspection(int id)
        {
            bool deletedLocally = LocallyDeleteInspection(id);

            if (!deletedLocally)
            {
                return RestClient.DeleteAsync(UrlPrefix + "Delete", new { id }).IsSuccessfulResult();
            }

            return Task.FromResult(deletedLocally);
        }

        public List<FishingGearDto> GetFishingGearsForShip(int shipUid, int? permitId = null)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                bool doesNotHavePermitId = permitId == null;
                int permitIdTest = permitId.GetValueOrDefault();

                List<FishingGearDto> fishingGears = (
                    from fishingGear in context.FishingGears
                    where fishingGear.ShipUid == shipUid
                        && (doesNotHavePermitId || fishingGear.PermitId == permitIdTest)
                    select new FishingGearDto
                    {
                        Count = fishingGear.Count,
                        Description = fishingGear.Description,
                        Height = fishingGear.Height,
                        HookCount = fishingGear.HookCount,
                        HouseLength = fishingGear.HouseLength,
                        HouseWidth = fishingGear.HouseWidth,
                        Id = fishingGear.Id,
                        Length = fishingGear.Length,
                        NetEyeSize = fishingGear.NetEyeSize,
                        TowelLength = fishingGear.TowelLength,
                        CordThickness = fishingGear.CordThickness,
                        TypeId = fishingGear.TypeId,
                        PermitId = fishingGear.PermitId,
                    }
                ).ToList();

                List<int> ids = fishingGears.ConvertAll(f => f.Id.Value);

                var fishingGearMarks = (
                    from fishingGearMark in context.FishingGearMarks
                    join status in context.NFishingGearMarkStatuses on fishingGearMark.StatusId equals status.Id
                    where ids.Contains(fishingGearMark.FishingGearId)
                    select new
                    {
                        Dto = new FishingGearMarkDto
                        {
                            Id = fishingGearMark.Id,
                            Number = fishingGearMark.Number,
                            StatusId = fishingGearMark.StatusId,
                            SelectedStatus = Enum.TryParse(status.Code, out FishingGearMarkStatus markStatus)
                                ? markStatus
                                : FishingGearMarkStatus.MARKED,
                        },
                        fishingGearMark.FishingGearId
                    }
                ).ToList();

                var fishingGearPingers = (
                    from fishingGearPinger in context.FishingGearPingers
                    join status in context.NFishingGearPingerStatuses on fishingGearPinger.StatusId equals status.Id
                    where ids.Contains(fishingGearPinger.FishingGearId)
                    select new
                    {
                        Dto = new FishingGearPingerDto
                        {
                            Id = fishingGearPinger.Id,
                            Number = fishingGearPinger.Number,
                            StatusId = fishingGearPinger.StatusId,
                            SelectedStatus = new NomenclatureDto
                            {
                                Value = status.Id,
                                Code = status.Code,
                                DisplayName = status.Name,
                                IsActive = status.IsActive,
                            }
                        },
                        fishingGearPinger.FishingGearId
                    }
                ).ToList();

                foreach (FishingGearDto fishingGear in fishingGears)
                {
                    fishingGear.Marks = fishingGearMarks
                        .Where(f => f.FishingGearId == fishingGear.Id)
                        .Select(f => f.Dto)
                        .ToList();

                    fishingGear.Pingers = fishingGearPingers
                        .Where(f => f.FishingGearId == fishingGear.Id)
                        .Select(f => f.Dto)
                        .ToList();
                }

                return fishingGears;
            }
        }

        public List<FishingGearDto> GetFishingGearsForPoundNet(int poundNetId, int? permitId = null)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                List<FishingGearDto> fishingGears = (
                    from fishingGear in context.PoundNetFishingGears
                    where fishingGear.PoundNetId == poundNetId
                        && (permitId == null || fishingGear.PermitId == permitId.Value)
                    select new FishingGearDto
                    {
                        Count = fishingGear.Count,
                        Description = fishingGear.Description,
                        Height = fishingGear.Height,
                        HookCount = fishingGear.HookCount,
                        HouseLength = fishingGear.HouseLength,
                        HouseWidth = fishingGear.HouseWidth,
                        Id = fishingGear.Id,
                        Length = fishingGear.Length,
                        NetEyeSize = fishingGear.NetEyeSize,
                        TowelLength = fishingGear.TowelLength,
                        CordThickness = fishingGear.CordThickness,
                        TypeId = fishingGear.TypeId,
                    }
                ).ToList();

                List<int> ids = fishingGears.ConvertAll(f => f.Id.Value);

                var fishingGearMarks = (
                    from fishingGearMark in context.PoundNetFishingGearMarks
                    join status in context.NFishingGearMarkStatuses on fishingGearMark.StatusId equals status.Id
                    where ids.Contains(fishingGearMark.FishingGearId)
                    select new
                    {
                        Dto = new FishingGearMarkDto
                        {
                            Id = fishingGearMark.Id,
                            Number = fishingGearMark.Number,
                            StatusId = fishingGearMark.StatusId,
                            SelectedStatus = Enum.TryParse(status.Code, out FishingGearMarkStatus markStatus)
                                ? markStatus
                                : FishingGearMarkStatus.MARKED,
                        },
                        fishingGearMark.FishingGearId
                    }
                ).ToList();

                var fishingGearPingers = (
                    from fishingGearPinger in context.PoundNetFishingGearPingers
                    join status in context.NFishingGearPingerStatuses on fishingGearPinger.StatusId equals status.Id
                    where ids.Contains(fishingGearPinger.FishingGearId)
                    select new
                    {
                        Dto = new FishingGearPingerDto
                        {
                            Id = fishingGearPinger.Id,
                            Number = fishingGearPinger.Number,
                            StatusId = fishingGearPinger.StatusId,
                            SelectedStatus = new NomenclatureDto
                            {
                                Value = status.Id,
                                Code = status.Code,
                                DisplayName = status.Name,
                                IsActive = status.IsActive,
                            }
                        },
                        fishingGearPinger.FishingGearId
                    }
                ).ToList();

                foreach (FishingGearDto fishingGear in fishingGears)
                {
                    fishingGear.Marks = fishingGearMarks
                        .Where(f => f.FishingGearId == fishingGear.Id)
                        .Select(f => f.Dto)
                        .ToList();

                    fishingGear.Pingers = fishingGearPingers
                        .Where(f => f.FishingGearId == fishingGear.Id)
                        .Select(f => f.Dto)
                        .ToList();
                }

                return fishingGears;
            }
        }

        public async Task<List<LogBookPageDto>> GetLogBookPages(List<int> logBookIds)
        {
            HttpResult<List<LogBookPageDto>> result = await RestClient.PostAsync<List<LogBookPageDto>>("InspectionData/GetLogBookPages", logBookIds);

            if (result.IsSuccessful)
            {
                return result.Content;
            }

            return null;
        }

        public async Task<List<DeclarationLogBookPageDto>> GetDeclarationLogBookPages(DeclarationLogBookType type, int shipUid)
        {
            HttpResult<List<DeclarationLogBookPageDto>> result = await RestClient.GetAsync<List<DeclarationLogBookPageDto>>("InspectionData/GetDeclarationLogBookPages", new { type, shipUid });

            if (result.IsSuccessful)
            {
                return result.Content;
            }

            return null;
        }

        public async Task<bool> SignInspection(int inspectionId, List<FileModel> files)
        {
            HttpResult result = await RestClient.PostAsFormDataAsync("Inspections/Sign", files, new { inspectionId });

            return result.IsSuccessful;
        }

        private async Task<TDto> Get<TDto>(int id, bool isLocal, InspectionType type)
            where TDto : InspectionEditDto
        {
            if (isLocal)
            {
                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    Inspection localInspection = (
                        from insp in context.Inspections
                        where insp.Id == id
                        select insp
                    ).FirstOrDefault();

                    if (localInspection == null)
                    {
                        return null;
                    }

                    return JsonSerializer.Deserialize<TDto>(localInspection.JsonContent);
                }
            }
            else
            {
                HttpResult<TDto> result = await RestClient.GetAsync<TDto>(UrlPrefix + "Get" + type.ToString(), new { id });

                if (result.IsSuccessful)
                {
                    using (IAppDbContext context = ContextBuilder.CreateContext())
                    {
                        Inspection localInspection = (
                            from insp in context.Inspections
                            where insp.Id == id
                            select insp
                        ).FirstOrDefault();

                        if (localInspection != null)
                        {
                            localInspection.JsonContent = JsonSerializer.Serialize(result.Content);
                            localInspection.HasJsonContent = true;
                            context.Inspections.Update(localInspection);
                        }
                    }

                    return result.Content;
                }
                else
                {
                    using (IAppDbContext context = ContextBuilder.CreateContext())
                    {
                        Inspection localInspection = (
                            from insp in context.Inspections
                            where insp.Id == id
                            select insp
                        ).FirstOrDefault();

                        if (localInspection?.HasJsonContent != true)
                        {
                            return null;
                        }

                        return JsonSerializer.Deserialize<TDto>(localInspection.JsonContent);
                    }
                }
            }
        }

        private bool LocallyDeleteInspection(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                Inspection inspection = context.Inspections.FirstOrDefault(f => f.Id == id);

                if (inspection == null)
                {
                    return false;
                }

                context.Inspections.Delete(f => f.Id == id);

                return inspection.IsLocal;
            }
        }

        private List<InspectionDto> PullInspectionsOffline(int page)
        {
            if (!ContextBuilder.DatabaseExists)
            {
                return null;
            }

            int pageSize = CommonGlobalVariables.PullItemsCount;

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from insp in context.Inspections
                    join inspType in context.NInspectionTypes on insp.InspectionTypeId equals inspType.Id
                    orderby insp.StartDate descending
                    select new InspectionDto
                    {
                        Id = insp.Id,
                        HasContentLocally = insp.HasJsonContent,
                        SubmitType = insp.SubmitType,
                        Description = inspType.Name,
                        Number = insp.ReportNr,
                        StartDate = insp.StartDate,
                        IsLocal = insp.IsLocal,
                        Type = insp.InspectionType,
                        InspectionState = insp.InspectionState,
                        InspectionSubjects = insp.InspectionSubjects,
                        Inspectors = insp.Inspectors,
                    }
                ).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        private InspectionDraftDto MapToDraftDto<TDto>(TDto dto)
            where TDto : InspectionEditDto
        {
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return new InspectionDraftDto
            {
                ActionsTaken = dto.ActionsTaken,
                AdministrativeViolation = dto.AdministrativeViolation,
                ByEmergencySignal = dto.ByEmergencySignal,
                EndDate = dto.EndDate,
                Files = dto.Files,
                Id = dto.Id,
                InspectionType = dto.InspectionType,
                InspectorComment = dto.InspectorComment,
                StartDate = dto.StartDate,
                Json = JsonSerializer.Serialize(dto, options),
            };
        }

        private void SaveInspectionInspectors(List<InspectorDuringInspectionDto> inspectors)
        {
            List<int> inspectorIds = inspectors
                .Where(f => f.InspectorId.HasValue)
                .Select(f => f.InspectorId.Value)
                .Distinct()
                .OrderBy(f => f)
                .ToList();

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                List<InspectorsHistory> inspectorsHistories = context.InspectorsHistories.ToList();
                List<RecentInspector> recentInspectors = context.RecentInspectors.ToList();

                if (inspectorsHistories.Count >= 5)
                {
                    InspectorsHistory leastUsed = (
                        from inspHistoty in inspectorsHistories
                        orderby inspHistoty.TimesUsed / 5, inspHistoty.LastUsed
                        select inspHistoty
                    ).First();

                    inspectorsHistories.Remove(leastUsed);
                    recentInspectors.RemoveAll(f => f.HistoryId == leastUsed.Id);

                    context.InspectorsHistories.Remove(leastUsed);
                    context.RecentInspectors.Delete(f => f.HistoryId == leastUsed.Id);
                }

                InspectorsHistory match = inspectorsHistories.Find(f =>
                    recentInspectors
                        .Where(s => s.HistoryId == f.Id)
                        .Select(s => s.InspectorId)
                        .OrderBy(s => s)
                        .SequenceEqual(inspectorIds)
                );

                if (match != null)
                {
                    match.LastUsed = DateTime.Now;
                    match.TimesUsed++;
                    context.InspectorsHistories.Update(match);
                }
                else
                {
                    context.InspectorsHistories.Add(new InspectorsHistory
                    {
                        Inspectors = InspectorsToString(inspectors),
                        LastUsed = DateTime.Now,
                        TimesUsed = 1
                    });

                    int id = (
                        from insp in context.InspectorsHistories
                        orderby insp.Id descending
                        select insp.Id
                    ).First();

                    context.RecentInspectors.AddRange(inspectorIds.ConvertAll(f => new RecentInspector
                    {
                        HistoryId = id,
                        InspectorId = f
                    }));
                }
            }
        }

        private string InspectorsToString(List<InspectorDuringInspectionDto> inspectors)
        {
            return inspectors?.Count > 0 ? string.Join(", ", inspectors
                .Select(f => $"{f.FirstName} {(f.MiddleName == null ? string.Empty : f.MiddleName + " ")}{f.LastName}")
                .ToArray()
            ) : string.Empty;
        }

        private string PersonnelToString(List<InspectionSubjectPersonnelDto> personnel)
        {
            return personnel?.Count > 0 ? string.Join(", ", personnel
                .GroupBy(f => f.EgnLnc)
                .Select(f => f.First())
                .Where(f => !string.IsNullOrWhiteSpace(f?.EgnLnc?.EgnLnc))
                .Select(f => $"{f.FirstName} {(f.MiddleName == null ? string.Empty : f.MiddleName + " ")}{f.LastName}")
                .ToArray()
            ) : string.Empty;
        }
    }
}
