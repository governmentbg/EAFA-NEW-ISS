using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.Mobile.CatchRecords;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace IARA.Infrastructure.Services
{
    public class CatchRecordsService : Service, ICatchRecordsService
    {
        public CatchRecordsService(IARADbContext dbContext)
            : base(dbContext) { }

        public MobileCatchRecordGroupDTO GetCatchRecords(CatchRecordPublicFilters filters, int userId, int pageNumber, int pageSize)
        {
            List<CatchRecordDTO> catchRecords = (
                from record in this.Db.FishingCatchRecords
                join ticket in this.Db.FishingTickets on record.TicketId equals ticket.Id
                where record.IsActive
                    && (filters == null || !filters.TicketId.HasValue || filters.TicketId == record.TicketId)
                    && ticket.CreatedByUserId == userId
                orderby record.CatchDate descending, record.CreatedOn descending
                select new CatchRecordDTO
                {
                    Id = record.Id,
                    TicketId = record.TicketId,
                    CatchDate = record.CatchDate,
                    Description = record.Description,
                    WaterArea = record.WaterArea,
                    Location = record.Coordinates == null ? null : new LocationDTO
                    {
                        Latitude = record.Coordinates.Y,
                        Longitude = record.Coordinates.X
                    }
                }
            ).Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (catchRecords.Count == 0)
            {
                return new MobileCatchRecordGroupDTO
                {
                    CatchRecords = new List<CatchRecordDTO>(),
                    Fishes = new List<CatchRecordFishDTO>(),
                    Tickets = new List<CatchRecordTicketDTO>(),
                    Files = new List<CatchRecordFileDTO>(),
                };
            }

            List<int> ids = catchRecords.ConvertAll(f => f.Id);

            List<CatchRecordFishDTO> fishes = (
                from fish in this.Db.FishingCatchRecordFishes
                join nFish in this.Db.Nfishes on fish.FishId equals nFish.Id
                where ids.Contains(fish.CatchRecordId)
                select new CatchRecordFishDTO
                {
                    Id = fish.Id,
                    CatchRecordId = fish.CatchRecordId,
                    Count = fish.Count,
                    Quantity = Convert.ToDouble(fish.Quantity),
                    FishType = new NomenclatureDTO
                    {
                        Value = nFish.Id,
                        DisplayName = nFish.Name,
                        Code = nFish.Code
                    }
                }
            ).ToList();

            List<int> ticketIds = catchRecords
                .Select(f => f.TicketId)
                .Distinct()
                .ToList();

            List<CatchRecordTicketDTO> tickets = (
                from ticket in this.Db.FishingTickets
                join ticketPerson in this.Db.Persons on ticket.PersonId equals ticketPerson.Id
                join ticketStatus in this.Db.NticketStatuses on ticket.TicketStatusId equals ticketStatus.Id
                join ticketType in this.Db.NticketTypes on ticket.TicketTypeId equals ticketType.Id
                where ticketIds.Contains(ticket.Id)
                select new CatchRecordTicketDTO
                {
                    Id = ticket.Id,
                    PersonFullName = ticketPerson.FirstName + " "
                        + (ticketPerson.MiddleName != null ? (ticketPerson.MiddleName + " ") : string.Empty)
                        + ticketPerson.LastName,
                    StatusName = ticketStatus.Name,
                    TypeName = ticketType.Name,
                    ValidFrom = ticket.TicketValidFrom,
                    ValidTo = ticket.TicketValidTo
                }
            ).ToList();

            List<CatchRecordFileDTO> files = (
                from catchFile in this.Db.CatchRecordFiles
                join file in this.Db.Files on catchFile.FileId equals file.Id
                where ids.Contains(catchFile.RecordId) && catchFile.IsActive && file.IsActive
                select new CatchRecordFileDTO
                {
                    Id = file.Id,
                    CatchRecordId = catchFile.RecordId,
                    ContentType = file.MimeType,
                    FileTypeId = catchFile.FileTypeId,
                    Name = file.Name,
                    Size = file.ContentLength,
                    UploadedOn = file.UploadedOn,
                }
            ).ToList();

            return new MobileCatchRecordGroupDTO
            {
                CatchRecords = catchRecords,
                Fishes = fishes,
                Tickets = tickets,
                Files = files,
            };
        }

        public int CreateCatchRecord(CatchRecordEditDTO edit, int userId)
        {
            bool ownsTicket = this.Db.FishingTickets.Any(f => f.Id == edit.TicketId.Value && f.CreatedByUserId == userId);

            if (!ownsTicket)
            {
                return 0;
            }

            using TransactionScope scope = new TransactionScope();

            FishingCatchRecord record = new FishingCatchRecord
            {
                CreatedOn = DateTime.Now,
                WaterArea = edit.WaterArea,
                CatchDate = edit.CatchDate.Value,
                Description = edit.Description,
                TicketId = edit.TicketId.Value,
                Coordinates = edit.Location == null ? null : new Point(edit.Location.Longitude, edit.Location.Latitude),
                FishingCatchRecordFishes = edit.Fishes.ConvertAll(f => new FishingCatchRecordFish
                {
                    Count = f.Count.Value,
                    FishId = f.FishTypeId.Value,
                    Quantity = f.Quantity.Value
                })
            };

            this.Db.FishingCatchRecords.Add(record);

            this.Db.SaveChanges();

            if (edit.Files != null)
            {
                foreach (FileInfoDTO file in edit.Files)
                {
                    this.Db.AddOrEditFile(record, record.FishingCatchRecordFiles, file);
                }
            }

            this.Db.SaveChanges();

            scope.Complete();

            return record.Id;
        }

        public void UpdateCatchRecord(CatchRecordEditDTO edit, int userId)
        {
            bool ownsTicket = this.Db.FishingTickets.Any(f => f.Id == edit.TicketId.Value && f.CreatedByUserId == userId);

            if (!ownsTicket)
            {
                return;
            }

            using TransactionScope scope = new TransactionScope();

            FishingCatchRecord catchRecord = this.Db.FishingCatchRecords
                .AsSplitQuery()
                .Include(f => f.FishingCatchRecordFiles)
                .Include(f => f.FishingCatchRecordFishes)
                .Single(f => f.Id == edit.Id);

            this.Db.FishingCatchRecordFishes.RemoveRange(catchRecord.FishingCatchRecordFishes);

            catchRecord.WaterArea = edit.WaterArea;
            catchRecord.CatchDate = edit.CatchDate.Value;
            catchRecord.Description = edit.Description;

            catchRecord.FishingCatchRecordFishes = edit.Fishes.ConvertAll(f => new FishingCatchRecordFish
            {
                CatchRecordId = catchRecord.Id,
                Count = f.Count.Value,
                FishId = f.FishTypeId.Value,
                Quantity = f.Quantity.Value
            });

            if (edit.Files != null)
            {
                foreach (FileInfoDTO file in edit.Files)
                {
                    this.Db.AddOrEditFile(catchRecord, catchRecord.FishingCatchRecordFiles, file);
                }
            }

            this.Db.FishingCatchRecords.Update(catchRecord);
            this.Db.SaveChanges();

            scope.Complete();
        }

        public void DeleteCatchRecord(int id, int userId)
        {
            FishingCatchRecord catchRecord = this.Db.FishingCatchRecords
                                                    .AsSplitQuery()
                                                    .Include(f => f.FishingCatchRecordFishes)
                                                    .Single(f => f.Id == id);

            bool ownsTicket = this.Db.FishingTickets.Any(f => f.Id == catchRecord.TicketId && f.CreatedByUserId == userId);

            if (!ownsTicket)
            {
                return;
            }

            catchRecord.IsActive = false;
            this.Db.FishingCatchRecords.Update(catchRecord);
            this.Db.SaveChanges();
        }

        public bool HasAccessToFile(int fileId, int userId)
        {
            return (
                from file in this.Db.CatchRecordFiles
                join catchRecord in this.Db.FishingCatchRecords on file.RecordId equals catchRecord.Id
                join ticket in this.Db.FishingTickets on catchRecord.TicketId equals ticket.Id
                where file.FileId == fileId
                    && ticket.CreatedByUserId == userId
                select file
            ).Any();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(this.Db.FishingCatchRecords, id);
        }
    }
}
