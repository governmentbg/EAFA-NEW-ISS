using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.API;
using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb;
using IARA.Mobile.Pub.Application.Filters;
using IARA.Mobile.Pub.Application.Interfaces.Database;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Transactions.Base;
using IARA.Mobile.Pub.Domain.Entities.ScientificFishing;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class ScientificFishingTransaction : BaseTransaction, IScientificFishingTransaction
    {
        private const string URL_PREFIX = "ScientificFishing/";

        private readonly IDownloader _downloader;

        public ScientificFishingTransaction(BaseTransactionProvider provider, IDownloader downloader) : base(provider)
        {
            _downloader = downloader;
        }

        public async Task<List<SFPermitDto>> GetAll(ScientificFishingFilters filters)
        {
            if (CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected)
            {
                return GetAllPermits(filters);
            }

            HttpResult<SFDataApiDto> result =
                await RestClient.GetAsync<SFDataApiDto>(URL_PREFIX + "GetAllPermits");

            if (result.IsSuccessful && result.Content != null)
            {
                List<SFPermit> permits = Mapper.Map<List<SFPermit>>(result.Content.Permits);
                List<SFHolder> holders = Mapper.Map<List<SFHolder>>(result.Content.Holders);
                List<SFOuting> outings = Mapper.Map<List<SFOuting>>(result.Content.Outings);
                List<SFCatch> catches = Mapper.Map<List<SFCatch>>(result.Content.Catches);
                List<SFPermitReason> permitsReasons = Mapper.Map<List<SFPermitReason>>(result.Content.PermitReasons);

                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    context.SFPermits.Clear();
                    context.SFHolders.Clear();
                    context.SFOutings.Clear();
                    context.SFCatches.Clear();
                    context.SFPermitReasons.Clear();

                    context.SFPermits.AddRange(permits);
                    context.SFHolders.AddRange(holders);
                    context.SFOutings.AddRange(outings);
                    context.SFCatches.AddRange(catches);
                    context.SFPermitReasons.AddRange(permitsReasons);
                }
            }

            return GetAllPermits(filters);
        }

        public SFPermitReviewDto Get(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                SFPermitReviewDto permitDto = (
                    from permit in context.SFPermits
                    where permit.Id == id
                    select new SFPermitReviewDto
                    {
                        Id = permit.Id,
                        CoordinationCommittee = permit.CoordinationCommittee,
                        CoordinationDate = permit.CoordinationDate,
                        CoordinationLetterNo = permit.CoordinationLetterNo,
                        FishingGearDescription = permit.FishingGearDescription,
                        FishTypesApp4ZBRDesc = permit.FishTypesApp4ZBRDesc,
                        FishTypesCrayFish = permit.FishTypesCrayFish,
                        FishTypesDescription = permit.FishTypesDescription,
                        IsAllowedDuringMatingSeason = permit.IsAllowedDuringMatingSeason,
                        IsShipRegistered = permit.IsShipRegistered,
                        RequestDate = permit.RequestDate,
                        RequesterEgn = permit.RequesterEgn,
                        RequesterFirstName = permit.RequesterFirstName,
                        RequesterLastName = permit.RequesterLastName,
                        RequesterMiddleName = permit.RequesterMiddleName,
                        RequesterPosition = permit.RequesterPosition,
                        RequesterScientificOrganizationName = permit.RequesterScientificOrganizationName,
                        ResearchGoalsDescription = permit.ResearchGoalsDescription,
                        ResearchPeriodFrom = permit.ResearchPeriodFrom,
                        ResearchPeriodTo = permit.ResearchPeriodTo,
                        ResearchWaterArea = permit.ResearchWaterArea,
                        ShipCaptainName = permit.ShipCaptainName,
                        ShipExternalMark = permit.ShipExternalMark,
                        ShipId = permit.ShipId,
                        ShipName = permit.ShipName,
                        ValidFrom = permit.ValidFrom,
                        ValidTo = permit.ValidTo
                    }
                ).FirstOrDefault();

                if (permitDto == null)
                {
                    return null;
                }

                permitDto.PermitReasonsIds = (
                    from permitReason in context.SFPermitReasons
                    where permitReason.PermitId == id
                    select permitReason.NPermitReasonId
                ).ToList();

                permitDto.Holders = (
                    from holder in context.SFHolders
                    where holder.RequestNumber == id
                    select new SFHolderDto
                    {
                        Id = holder.Id,
                        Name = holder.Name,
                        OwnerId = holder.OwnerId,
                        RequestNumber = holder.RequestNumber,
                        ScientificPosition = holder.ScientificPosition
                    }
                ).ToList();

                return permitDto;
            }
        }

        public List<SFOutingDto> GetOutings(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                List<SFOutingDto> outings = (
                    from outing in context.SFOutings
                    where outing.PermitId == id
                        && !outing.HasBeenDeletedLocally
                    select new SFOutingDto
                    {
                        Id = outing.Id,
                        PermitId = outing.PermitId,
                        DateOfOuting = outing.DateOfOuting,
                        WaterArea = outing.WaterArea
                    }
                ).ToList();

                if (outings.Count == 0)
                {
                    return new List<SFOutingDto>();
                }

                List<int> outingIds = outings.ConvertAll(f => f.Id);

                List<SFCatchDto> catches = (
                    from sfCatch in context.SFCatches
                    join fishType in context.NFishes on sfCatch.FishTypeId equals fishType.Id
                    where outingIds.Contains(sfCatch.OutingId)
                        && !sfCatch.HasBeenDeletedLocally
                    select new SFCatchDto
                    {
                        Id = sfCatch.Id,
                        FishTypeId = fishType.Id,
                        FishType = new NomenclatureDto
                        {
                            DisplayName = fishType.Name,
                            Value = fishType.Id
                        },
                        CatchUnder100 = sfCatch.CatchUnder100,
                        CatchOver1000 = sfCatch.CatchOver1000,
                        OutingId = sfCatch.OutingId,
                        TotalCatch = sfCatch.TotalCatch,
                        TotalKeptCount = sfCatch.TotalKeptCount,
                        Catch100To500 = sfCatch.Catch100To500,
                        Catch500To1000 = sfCatch.Catch500To1000,
                    }
                ).ToList();

                foreach (SFOutingDto outing in outings)
                {
                    outing.Catches = catches.FindAll(f => f.OutingId == outing.Id);
                }

                return outings;
            }
        }

        public async Task<AddEntityResultEnum> AddOuting(SFOutingDto dto)
        {
            AddEntityResult addResult = await Repository.Add(
                Mapper.Map<SFOuting>(dto),
                Mapper.Map<SFOutingEditApiDto>(dto),
                URL_PREFIX + "AddOuting"
            );

            List<SFCatch> catches = Mapper.Map<List<SFCatch>>(dto.Catches);

            foreach (SFCatch cat in catches)
            {
                cat.OutingId = addResult.Id;
            }

            Repository.AddRelatedTables(catches);

            dto.Id = addResult.Id;

            return addResult.Result;
        }

        public async Task<UpdateEntityResultEnum> EditOuting(SFOutingDto dto)
        {
            UpdateEntityResultEnum updateResult = await Repository.Update(dto.Id,
                (SFOuting outing) =>
                {
                    outing.DateOfOuting = dto.DateOfOuting;
                    outing.WaterArea = dto.WaterArea;
                },
                Mapper.Map<SFOutingEditApiDto>(dto),
                URL_PREFIX + "EditOuting"
            );

            Repository.UpdateRelatedTables(updateResult, dto.Catches, (sfCatch) => Mapper.Map<SFCatch>(sfCatch), (dtoo, entity) =>
            {
                entity.Catch100To500 = dtoo.Catch100To500;
                entity.Catch500To1000 = dtoo.Catch500To1000;
                entity.CatchOver1000 = dtoo.CatchOver1000;
                entity.CatchUnder100 = dtoo.CatchUnder100;
                entity.FishTypeId = dtoo.FishType.Value;
                entity.TotalCatch = dtoo.TotalCatch;
                entity.TotalKeptCount = dtoo.TotalKeptCount;
            }, dto.Id, nameof(SFCatch.OutingId));

            return updateResult;
        }

        public async Task<DeleteEntityResultEnum> DeleteOuting(SFOutingDto dto)
        {
            DeleteEntityResultEnum deleteResult = await Repository.Delete<SFOuting>(dto.Id, URL_PREFIX + "DeleteOuting");

            Repository.DeleteRelatedTables(deleteResult, Mapper.Map<List<SFCatch>>(dto.Catches));

            return deleteResult;
        }

        public Task<bool> DownloadFile(FileModel file)
        {
            return _downloader.DownloadFile(file.Name, file.ContentType, URL_PREFIX + "DownloadFile", new { id = file.Id });
        }

        private List<SFPermitDto> GetAllPermits(ScientificFishingFilters filters)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from permit in context.SFPermits
                    select new SFPermitDto
                    {
                        Id = permit.Id,
                        RequesterName = permit.RequesterFirstName + " " + permit.RequesterLastName,
                        ScientificOrganizationName = permit.RequesterScientificOrganizationName,
                        ValidFrom = permit.ValidFrom,
                        ValidTo = permit.ValidTo,
                        HasOutings = (
                            from outing in context.SFOutings
                            where outing.PermitId == permit.Id
                            select outing
                        ).Any()
                    }
                ).ToList();
            }
        }
    }
}
