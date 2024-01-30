using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.CatchRecords;
using IARA.Mobile.Pub.Application.DTObjects.CatchRecords.API;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.Filters;
using IARA.Mobile.Pub.Application.Interfaces.Database;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Application.Transactions.Base;
using IARA.Mobile.Pub.Domain.Entities.CatchRecords;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class CatchRecordsTransaction : BaseTransaction, ICatchRecordsTransaction
    {
        private readonly IOfflineFiles _files;
        private static volatile bool _isPostingData = false;

        public CatchRecordsTransaction(BaseTransactionProvider provider, IOfflineFiles files)
            : base(provider)
        {
            _files = files;
        }

        public async Task<List<CatchRecordDto>> GetCatchRecords(CatchRecordPublicFilters filters)
        {
            if (CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected)
            {
                return GetCatchRecordsFromDatabase();
            }

            HttpResult<CatchRecordGroupApiDto> result =
                await RestClient.PostAsync<CatchRecordGroupApiDto>("CatchRecords/GetAll", new GridRequest<CatchRecordPublicFilters>(filters));

            if (result.IsSuccessful && result.Content != null)
            {
                List<CatchRecordApiDto> catchRecords = result.Content.CatchRecords;
                List<CatchRecordFishApiDto> fishes = result.Content.Fishes;
                List<CatchRecordTicketApiDto> tickets = result.Content.Tickets;
                List<CatchRecordFileApiDto> files = result.Content.Files;

                using (IAppDbContext context = ContextBuilder.CreateContext())
                {
                    context.CatchRecords.Clear();
                    context.CatchRecordFishes.Clear();
                    context.CatchRecordTickets.Clear();
                    context.CatchRecordFiles.Clear();

                    context.CatchRecords.AddRange(catchRecords.ConvertAll(f =>
                    {
                        List<CatchRecordFishApiDto> catchRecordFishes = fishes.FindAll(s => s.CatchRecordId == f.Id);
                        return new CatchRecord
                        {
                            Id = f.Id,
                            CatchDate = f.CatchDate,
                            Description = f.Description,
                            Latitude = DMSType.Parse(f.Location?.DMSLatitude)?.ToDecimal(),
                            Longitude = DMSType.Parse(f.Location?.DMSLongitude)?.ToDecimal(),
                            TicketId = f.TicketId,
                            WaterArea = f.WaterArea,
                            TotalCount = catchRecordFishes.Sum(s => s.Count),
                            TotalQuantity = catchRecordFishes.Sum(s => s.Quantity)
                        };
                    }));

                    context.CatchRecordFishes.AddRange(Mapper.Map<List<CatchRecordFish>>(fishes));
                    context.CatchRecordTickets.AddRange(Mapper.Map<List<CatchRecordTicket>>(tickets));
                    context.CatchRecordFiles.AddRange(files.ConvertAll(f => new CatchRecordFile
                    {
                        Id = f.Id.Value,
                        Name = f.Name,
                        Size = f.Size,
                        CatchRecordId = f.CatchRecordId,
                        ContentType = f.ContentType,
                        Deleted = f.Deleted,
                        FullPath = f.FullPath,
                        UploadedOn = f.UploadedOn,
                    }));
                }
            }

            return GetCatchRecordsFromDatabase();
        }

        public CatchRecordInfoDto GetCatchRecord(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                CatchRecordInfoDto info = (
                    from catchRecord in context.CatchRecords
                    join ticket in context.FishingTickets on catchRecord.TicketId equals ticket.Id
                    where catchRecord.Id == id
                    select new CatchRecordInfoDto
                    {
                        Id = catchRecord.Id,
                        IsOfflineOnly = catchRecord.IsLocalOnly,
                        CatchDate = catchRecord.CatchDate,
                        Description = catchRecord.Description,
                        WaterArea = catchRecord.WaterArea,
                        Ticket = new UserTicketShortDto
                        {
                            Id = ticket.Id,
                            PersonFullName = ticket.PersonFullName,
                            TypeName = ticket.TypeName,
                            ValidFrom = ticket.ValidFrom,
                            ValidTo = ticket.ValidTo,
                        },
                        Location = !catchRecord.Latitude.HasValue ? null : new LocationDto
                        {
                            DMSLatitude = DMSType.FromDouble(catchRecord.Latitude.Value).ToString(),
                            DMSLongitude = DMSType.FromDouble(catchRecord.Longitude.Value).ToString()
                        }
                    }
                ).FirstOrDefault();

                if (info == null)
                {
                    return null;
                }

                info.Fishes = (
                    from fish in context.CatchRecordFishes
                    join fishType in context.NFishes on fish.FishTypeId equals fishType.Id
                    where fish.CatchRecordId == info.Id
                    select new CatchRecordFishDto
                    {
                        Id = fish.Id,
                        Count = fish.Count,
                        Quantity = fish.Quantity,
                        RecordCatchId = fish.CatchRecordId,
                        FishType = new NomenclatureDto
                        {
                            Value = fishType.Id,
                            DisplayName = fishType.Name,
                            IsActive = fishType.IsActive
                        }
                    }
                ).ToList();

                info.Files = (
                    from file in context.CatchRecordFiles
                    where file.CatchRecordId == info.Id
                    select new CatchRecordFileDto
                    {
                        ContentType = file.ContentType,
                        Deleted = file.Deleted || file.HasBeenDeletedLocally,
                        FullPath = file.FullPath,
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        UploadedOn = file.UploadedOn,
                        IsLocal = file.IsLocalOnly
                    }
                ).ToList();

                return info;
            }
        }

        public async Task CreateCatchRecord(CreateCatchRecordDto dto)
        {
            CatchRecord catchRecord = new CatchRecord
            {
                CatchDate = dto.CatchDate.Value,
                Description = dto.Description,
                Latitude = DMSType.Parse(dto.Location?.DMSLatitude)?.ToDecimal(),
                Longitude = DMSType.Parse(dto.Location?.DMSLongitude)?.ToDecimal(),
                TicketId = dto.TicketId.Value,
                TotalCount = dto.Fishes.Sum(f => f.Count),
                TotalQuantity = dto.Fishes.Sum(f => f.Quantity),
                WaterArea = dto.WaterArea,
            };

            AddFileTypes(dto.Files);

            AddEntityResult result = await Repository.Add(catchRecord, dto, "CatchRecords/Create", asFormData: true);

            switch (result.Result)
            {
                case AddEntityResultEnum.AddedOnServer:
                    _files.DeleteFiles(dto.Identifier);
                    break;
                case AddEntityResultEnum.AddedLocally:
                    Repository.AddRelatedTables(dto.Fishes.ConvertAll(f => new CatchRecordFish
                    {
                        Count = f.Count,
                        CatchRecordId = result.Id,
                        FishTypeId = f.FishTypeId,
                        Quantity = f.Quantity,
                    }));
                    Repository.AddRelatedTables(dto.Files.ConvertAll(f => new CatchRecordFile
                    {
                        Id = f.Id ?? 0,
                        CatchRecordId = result.Id,
                        ContentType = f.ContentType,
                        Deleted = f.Deleted,
                        FullPath = f.FullPath,
                        Name = f.Name,
                        Size = f.Size,
                        UploadedOn = f.UploadedOn,
                    }));

                    using (IAppDbContext context = ContextBuilder.CreateContext())
                    {
                        bool ticketExists = context.CatchRecordTickets.Any(f => f.Id == catchRecord.TicketId);

                        if (!ticketExists)
                        {
                            CatchRecordTicket catchRecordTicket = (
                                from ticket in context.FishingTickets
                                where ticket.Id == dto.TicketId
                                select new CatchRecordTicket
                                {
                                    Id = ticket.Id,
                                    PersonFullName = ticket.PersonFullName,
                                    StatusName = ticket.StatusName,
                                    ValidFrom = ticket.ValidFrom,
                                    ValidTo = ticket.ValidTo,
                                    TypeName = ticket.TypeName,
                                }
                            ).First();

                            context.CatchRecordTickets.Add(catchRecordTicket);
                        }
                    }
                    break;
            }
        }

        public async Task EditCatchRecord(CreateCatchRecordDto dto)
        {
            AddFileTypes(dto.Files);

            UpdateEntityResultEnum result = await Repository.Update<CatchRecord, CreateCatchRecordDto>(
                dto.Id.Value,
                (entity) =>
                {
                    entity.CatchDate = dto.CatchDate.Value;
                    entity.Description = dto.Description;
                    entity.Latitude = DMSType.Parse(dto.Location?.DMSLatitude)?.ToDecimal();
                    entity.Longitude = DMSType.Parse(dto.Location?.DMSLatitude)?.ToDecimal();
                    entity.TicketId = dto.TicketId.Value;
                    entity.TotalCount = dto.Fishes.Sum(f => f.Count);
                    entity.TotalQuantity = dto.Fishes.Sum(f => f.Quantity);
                    entity.WaterArea = dto.WaterArea;
                },
                dto,
                "CatchRecords/Edit",
                asFormData: true
            );

            switch (result)
            {
                case UpdateEntityResultEnum.UpdatedLocally:
                case UpdateEntityResultEnum.NotUploadedToServer:
                    Repository.Update2RelatedTables(
                        result,
                        dto.Fishes,
                        (table) => (
                            from fish in table
                            where fish.CatchRecordId == dto.Id.Value
                            select fish
                        ).ToList(),
                        (fish) => new CatchRecordFish
                        {
                            Id = fish.Id ?? 0,
                            Count = fish.Count,
                            CatchRecordId = dto.Id.Value,
                            FishTypeId = fish.FishTypeId,
                            Quantity = fish.Quantity,
                        },
                        (fish, entity) =>
                        {
                            entity.Count = fish.Count;
                            entity.CatchRecordId = dto.Id.Value;
                            entity.FishTypeId = fish.FishTypeId;
                            entity.Quantity = fish.Quantity;
                        }
                    );
                    Repository.Update2RelatedTables(
                        result,
                        dto.Files,
                        (table) => (
                            from file in table
                            where file.CatchRecordId == dto.Id.Value
                            select file
                        ).ToList(),
                        (file) => new CatchRecordFile
                        {
                            Id = file.Id ?? 0,
                            CatchRecordId = dto.Id.Value,
                            ContentType = file.ContentType,
                            Deleted = file.Deleted,
                            FullPath = file.FullPath,
                            Name = file.Name,
                            Size = file.Size,
                            UploadedOn = file.UploadedOn,
                        },
                        (file, entity) =>
                        {
                            entity.CatchRecordId = dto.Id.Value;
                            entity.ContentType = file.ContentType;
                            entity.Deleted = file.Deleted;
                            entity.FullPath = file.FullPath;
                            entity.Name = file.Name;
                            entity.Size = file.Size;
                            entity.UploadedOn = file.UploadedOn;
                        }
                    );
                    break;
                case UpdateEntityResultEnum.UpdatedOnServer:
                case UpdateEntityResultEnum.NotInDatabase:
                    _files.DeleteFiles(dto.Identifier);
                    break;
            }
        }

        public Task DeleteCatchRecord(int id)
        {
            return Repository.Delete<CatchRecord>(id, "CatchRecords/Delete");
        }

        public async Task<FileResponse> GetPhoto(int photoId)
        {
            HttpResult<FileResponse> result = await RestClient.GetAsync<FileResponse>("CatchRecords/Photo", new { id = photoId });

            if (result.IsSuccessful && result.Content != null)
            {
                return result.Content;
            }

            return null;
        }

        public async Task<FileResponse> GetGalleryPhoto(int photoId)
        {
            HttpResult<FileResponse> result = await RestClient.GetAsync<FileResponse>("CatchRecords/GalleryPhoto", new { id = photoId });

            if (result.IsSuccessful && result.Content != null)
            {
                return result.Content;
            }

            return null;
        }

        public async Task PostOfflineCatchRecords()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                List<CatchRecord> entitites = context.CatchRecords
                    .Where(f => f.IsLocalOnly || f.HasBeenDeletedLocally || f.HasBeenUpdatedLocally)
                    .ToList();

                if (entitites.Count == 0)
                {
                    return;
                }

                foreach (CatchRecord entity in entitites)
                {
                    int fileTypeId = context.NFileTypes.First(f => f.Code == CommonConstants.PhotoFileType).Id;

                    bool suceeded = false;

                    CreateCatchRecordDto createDto = new CreateCatchRecordDto
                    {
                        CatchDate = entity.CatchDate,
                        Description = entity.Description,
                        Id = entity.Id,
                        TicketId = entity.TicketId,
                        WaterArea = entity.WaterArea,
                        Location = entity.Longitude.HasValue ? new LocationDto
                        {
                            DMSLatitude = DMSType.FromDouble(entity.Longitude.Value).ToString(),
                            DMSLongitude = DMSType.FromDouble(entity.Longitude.Value).ToString(),
                        } : null,
                        Fishes = (
                            from fish in context.CatchRecordFishes
                            where fish.CatchRecordId == entity.Id
                            select new CreateCatchRecordFishDto
                            {
                                Id = fish.Id,
                                Count = fish.Count,
                                Quantity = fish.Quantity,
                                CatchRecordId = fish.CatchRecordId,
                                FishTypeId = fish.FishTypeId,
                            }
                        ).ToList(),
                        Files = (
                            from file in context.CatchRecordFiles
                            where file.CatchRecordId == entity.Id
                            select new FileModel
                            {
                                Id = file.Id > 0 ? (int?)file.Id : null,
                                ContentType = file.ContentType,
                                Deleted = file.Deleted || file.HasBeenDeletedLocally,
                                FileTypeId = fileTypeId,
                                FullPath = file.FullPath,
                                Name = file.Name,
                                Size = file.Size,
                                UploadedOn = file.UploadedOn,
                            }
                        ).ToList(),
                    };

                    if (entity.IsLocalOnly)
                    {
                        HttpResult result = await RestClient.PostAsFormDataAsync("CatchRecords/Create", createDto);

                        suceeded = result.IsSuccessful;
                    }
                    else if (entity.HasBeenUpdatedLocally)
                    {
                        HttpResult result = await RestClient.PostAsFormDataAsync("CatchRecords/Edit", createDto);

                        suceeded = result.IsSuccessful;
                    }
                    else
                    {
                        HttpResult result = await RestClient.DeleteAsync("CatchRecords/Delete", new { id = entity.Id });

                        suceeded = result.IsSuccessful;
                    }

                    if (suceeded)
                    {
                        try
                        {
                            _files.DeleteFiles(entity.Identifier);
                        }
                        catch (Exception) { }
                        context.CatchRecords.Delete(f => f.Id == entity.Id);
                        context.CatchRecordFishes.Delete(f => f.CatchRecordId == entity.Id);
                    }
                }
            }
        }

        private List<CatchRecordDto> GetCatchRecordsFromDatabase()
        {
            DateTime now = DateTimeProvider.Now;

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from record in context.CatchRecords
                    join ticket in context.CatchRecordTickets on record.TicketId equals ticket.Id
                    where !record.HasBeenDeletedLocally
                    orderby record.CatchDate descending, record.Id descending
                    select new CatchRecordDto
                    {
                        Id = record.Id,
                        Identifier = record.Identifier,
                        CatchDate = record.CatchDate,
                        WaterArea = record.WaterArea,
                        TotalCount = record.TotalCount,
                        TotalQuantity = record.TotalQuantity,
                        IsLocal = record.IsLocalOnly || record.HasBeenUpdatedLocally,
                        IsActive = ticket.ValidFrom < now && now < ticket.ValidTo
                    }
                ).ToList();
            }
        }

        private void AddFileTypes(List<FileModel> files)
        {
            int fileTypeId = 0;

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                fileTypeId = context.NFileTypes.First(f => f.Code == CommonConstants.PhotoFileType).Id;
            }

            foreach (FileModel file in files)
            {
                file.FileTypeId = fileTypeId;
            }
        }
    }
}
