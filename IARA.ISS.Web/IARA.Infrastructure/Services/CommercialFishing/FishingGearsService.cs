using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.CommercialFishing;
using IARA.EntityModels.Entities;
using IARA.Common.Enums;

namespace IARA.Infrastructure.Services.CommercialFishing
{
    public class FishingGearsService : Service, IFishingGearsService
    {
        public FishingGearsService(IARADbContext dbContext)
            : base(dbContext)
        {
        }

        public List<FishingGearDTO> GetCommercialFishingPermitLicenseFishingGears(int permitLicenseId, bool noId = false)
        {
            List<FishingGearDTO> result = (from fishingGear in Db.FishingGearRegisters
                                           join fishingGearType in Db.NfishingGears on fishingGear.FishingGearTypeId equals fishingGearType.Id
                                           where fishingGear.PermitLicenseId == permitLicenseId
                                           select new FishingGearDTO
                                           {
                                               Id = fishingGear.Id,
                                               TypeId = fishingGear.FishingGearTypeId,
                                               Type = fishingGearType.Name,
                                               Count = fishingGear.GearCount,
                                               HookCount = fishingGear.HookCount,
                                               NetEyeSize = fishingGear.NetEyeSize,
                                               Length = fishingGear.Length,
                                               IsActive = fishingGear.IsActive,
                                               Description = fishingGear.Description,
                                               HasPingers = fishingGear.HasPinger
                                           }).ToList();

            MapFishingGearMarksAndPingers(result, noId);

            if (noId)
            {
                foreach (FishingGearDTO fishingGear in result)
                {
                    fishingGear.Id = default;
                }
            }

            return result;
        }

        public List<FishingGearDTO> GetShipFishingGears(int shipUId)
        {
            DateTime now = DateTime.Now;

            List<int> shipIds = (from ship in Db.ShipsRegister
                                 where ship.ShipUid == shipUId
                                 select ship.Id).ToList();

            List<FishingGearDTO> result = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                           join fishingGear in Db.FishingGearRegisters on permitLicense.Id equals fishingGear.PermitLicenseId
                                           join fishingGearType in Db.NfishingGears on fishingGear.FishingGearTypeId equals fishingGearType.Id
                                           where shipIds.Contains(permitLicense.ShipId)
                                                && permitLicense.IsActive
                                                && permitLicense.PermitLicenseValidFrom <= now
                                                && permitLicense.PermitLicenseValidTo > now
                                                && !permitLicense.IsSuspended
                                           select new FishingGearDTO
                                           {
                                               Id = fishingGear.Id,
                                               TypeId = fishingGear.FishingGearTypeId,
                                               Type = fishingGearType.Name,
                                               Count = fishingGear.GearCount,
                                               HookCount = fishingGear.HookCount,
                                               NetEyeSize = fishingGear.NetEyeSize,
                                               Length = fishingGear.Length,
                                               Description = fishingGear.Description,
                                               IsActive = fishingGear.IsActive
                                           }).ToList();

            MapFishingGearMarksAndPingers(result);

            return result;
        }

        public List<NomenclatureDTO> GetShipFishingGearNomenclatures(int shipId, int year)
        {
            DateTime now = DateTime.Now;

            int shipUid = (from ship in Db.ShipsRegister
                           where ship.Id == shipId
                           select ship.ShipUid).First();

            List<int> shipIds = (from ship in Db.ShipsRegister
                                 where ship.ShipUid == shipUid
                                 select ship.Id).ToList();

            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = new DateTime(year, 12, 31);

            // взимаме всички активни удостоверения
            List<int> permitLicenceIds = (from permitLicence in Db.CommercialFishingPermitLicensesRegisters
                                          where shipIds.Contains(permitLicence.ShipId)
                                                && permitLicence.IsActive
                                                && permitLicence.PermitLicenseValidFrom >= startDate
                                                && permitLicence.PermitLicenseValidTo <= endDate
                                          select permitLicence.Id).ToList();

            // взимаме всички видове уреди
            HashSet<int> gearIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                    join fishingGear in Db.FishingGearRegisters on permitLicense.Id equals fishingGear.PermitLicenseId
                                    join fishingGearType in Db.NfishingGears on fishingGear.FishingGearTypeId equals fishingGearType.Id
                                    where permitLicenceIds.Contains(permitLicense.Id)
                                    select fishingGearType.Id).ToHashSet();

            List<NomenclatureDTO> result = (from fishingGearType in Db.NfishingGears
                                            where gearIds.Contains(fishingGearType.Id)
                                            select new NomenclatureDTO
                                            {
                                                Value = fishingGearType.Id,
                                                Code = fishingGearType.Code,
                                                DisplayName = fishingGearType.Name,
                                                IsActive = true
                                            }).ToList();

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(Db.FishingGearRegisters, id);
        }

        public int AddOrEditFishingGear(FishingGearDTO fishingGear, int? permitLicenseId, int? inspectionId)
        {
            int id = fishingGear.Id.HasValue ? fishingGear.Id.Value : default;

            FishingGearRegister oldEntry = (from fg in Db.FishingGearRegisters
                                            where fg.Id == fishingGear.Id
                                            select fg).FirstOrDefault();
            if (oldEntry != null)
            {
                string fishingGearType = (from fishGearType in Db.NfishingGears
                                          where fishGearType.Id == fishingGear.TypeId
                                          select fishGearType.Code).First();
                FishingGearTypesEnum fishingGearTypeEnum;
                bool isCastSuccessful = Enum.TryParse<FishingGearTypesEnum>(fishingGearType, out fishingGearTypeEnum);

                oldEntry.FishingGearTypeId = fishingGear.TypeId;
                oldEntry.NetEyeSize = fishingGear.NetEyeSize;
                oldEntry.HasPinger = fishingGear.Pingers != null ? fishingGear.Pingers.Count > 0 : false;
                oldEntry.Description = fishingGear.Description;
                oldEntry.IsActive = fishingGear.IsActive;

                if (isCastSuccessful)
                {
                    if (fishingGearTypeEnum == FishingGearTypesEnum.DLN)
                    {
                        oldEntry.TowelLength = fishingGear.TowelLength;
                        oldEntry.HouseLength = fishingGear.HouseLength;
                        oldEntry.HouseWidth = fishingGear.HouseWidth;

                        oldEntry.GearCount = 0;
                        oldEntry.HookCount = null;
                        oldEntry.Length = null;
                        oldEntry.Height = null;
                    }
                    else
                    {
                        UpdateCustomFishingGearProperties(oldEntry, fishingGear);
                    }
                }
                else
                {
                    UpdateCustomFishingGearProperties(oldEntry, fishingGear);
                }

                if (fishingGear.Marks != null)
                {
                    AddOrEditFishingGearMarks(fishingGear.Marks, oldEntry);
                }

                if (fishingGear.Pingers != null)
                {
                    AddOrEditFishingGearPingers(fishingGear.Pingers, oldEntry);
                }
            }
            else
            {
                id = AddFishingGear(fishingGear, permitLicenseId, inspectionId);
            }

            Db.SaveChanges();

            return id;
        }

        private int AddFishingGear(FishingGearDTO fishingGear, int? permitLicenseId, int? inspectionId)
        {
            FishingGearRegister entry = new FishingGearRegister
            {
                FishingGearTypeId = fishingGear.TypeId,
                NetEyeSize = fishingGear.NetEyeSize,
                HasPinger = fishingGear.Pingers != null ? fishingGear.Pingers.Count > 0 : false,
                Description = fishingGear.Description
            };

            if (permitLicenseId.HasValue)
            {
                entry.PermitLicenseId = permitLicenseId.Value;
            }
            else
            {
                entry.InspectionId = inspectionId.Value;
            }

            string fishingGearType = (from fishGearType in Db.NfishingGears
                                      where fishGearType.Id == fishingGear.TypeId
                                      select fishGearType.Code).First();
            FishingGearTypesEnum fishingGearTypeEnum;
            bool isCastSuccessful = Enum.TryParse<FishingGearTypesEnum>(fishingGearType, out fishingGearTypeEnum);

            if (isCastSuccessful)
            {
                if (fishingGearTypeEnum == FishingGearTypesEnum.DLN)
                {
                    entry.TowelLength = fishingGear.TowelLength;
                    entry.HouseLength = fishingGear.HouseLength;
                    entry.HouseWidth = fishingGear.HouseWidth;
                }
                else
                {
                    UpdateCustomFishingGearProperties(entry, fishingGear);
                }
            }
            else
            {
                UpdateCustomFishingGearProperties(entry, fishingGear);
            }

            if (fishingGear.Marks != null)
            {
                AddOrEditFishingGearMarks(fishingGear.Marks, entry);
            }

            if (fishingGear.Pingers != null)
            {
                AddOrEditFishingGearPingers(fishingGear.Pingers, entry);
            }

            Db.FishingGearRegisters.Add(entry);
            Db.SaveChanges();

            return entry.Id;
        }

        public void MapFishingGearMarksAndPingers(List<FishingGearDTO> result, bool noIds = false)
        {
            FishingGearMarksAndPingersData marksAndPingersData = GetFishingGearMarksAndPingers(result);

            foreach (FishingGearDTO fishingGear in result)
            {
                fishingGear.Marks = marksAndPingersData.Marks[fishingGear.Id.Value].ToList();
                fishingGear.MarksNumbers = String.Join("; ", marksAndPingersData.Marks[fishingGear.Id.Value].Select(x => x.Number));
                fishingGear.Pingers = marksAndPingersData.Pingers[fishingGear.Id.Value].ToList();

                if (noIds)
                {
                    foreach (FishingGearMarkDTO mark in fishingGear.Marks)
                    {
                        mark.Id = null;
                    }

                    foreach (FishingGearPingerDTO pinger in fishingGear.Pingers)
                    {
                        pinger.Id = null;
                    }
                }
            }
        }

        private FishingGearMarksAndPingersData GetFishingGearMarksAndPingers(List<FishingGearDTO> fishingGears)
        {
            DateTime now = DateTime.Now;
            FishingGearMarksAndPingersData result = new FishingGearMarksAndPingersData();

            List<int> fishingGearIds = fishingGears.Select(x => x.Id.Value).ToList();

            result.Marks = (from fishingGear in Db.FishingGearRegisters
                            join mark in Db.FishingGearMarks on fishingGear.Id equals mark.FishingGearId
                            join markStatus in Db.NfishingGearMarkStatuses on mark.MarkStatusId equals markStatus.Id
                            where fishingGearIds.Contains(fishingGear.Id)
                            select new
                            {
                                FishingGearId = fishingGear.Id,
                                Mark = new FishingGearMarkDTO
                                {
                                    Id = mark.Id,
                                    Number = mark.MarkNum,
                                    StatusId = mark.MarkStatusId,
                                    SelectedStatus = Enum.Parse<FishingGearMarkStatusesEnum>(markStatus.Code),
                                    IsActive = mark.IsActive
                                }
                            }).ToLookup(x => x.FishingGearId, y => y.Mark);

            result.Pingers = (from fishingGear in Db.FishingGearRegisters
                              join pinger in Db.FishingGearPingers on fishingGear.Id equals pinger.FishingGearId
                              join pingerStatus in Db.NfishingGearPingerStatuses on pinger.PingerStatusId equals pingerStatus.Id
                              where fishingGearIds.Contains(fishingGear.Id)
                              select new
                              {
                                  FishingGearId = fishingGear.Id,
                                  Pinger = new FishingGearPingerDTO
                                  {
                                      Id = pinger.Id,
                                      Number = pinger.PingerNum,
                                      StatusId = pinger.PingerStatusId,
                                      SelectedStatus = new NomenclatureDTO
                                      {
                                          Value = pinger.PingerStatusId,
                                          Code = pingerStatus.Code,
                                          DisplayName = pingerStatus.Name,
                                          IsActive = pingerStatus.ValidFrom <= now && pingerStatus.ValidTo > now
                                      },
                                      IsActive = pinger.IsActive,
                                      Brand = pinger.Brand,
                                      Model = pinger.Model,
                                  }
                              }).ToLookup(x => x.FishingGearId, x => x.Pinger);

            return result;
        }

        private void UpdateCustomFishingGearProperties(FishingGearRegister entry, FishingGearDTO dto)
        {
            entry.GearCount = dto.Count;
            entry.HookCount = dto.HookCount;
            entry.Length = dto.Length;
            entry.Height = dto.Height;

            entry.TowelLength = null;
            entry.HouseLength = null;
            entry.HouseWidth = null;
        }

        private void AddOrEditFishingGearMarks(List<FishingGearMarkDTO> marks, FishingGearRegister fishingGearEntry)
        {
            foreach (FishingGearMarkDTO mark in marks)
            {
                if (mark.Id == null)
                {
                    AddFishingGearMark(mark, fishingGearEntry);
                }
                else
                {
                    EditFishingGearMark(mark);
                }
            }
        }

        private void AddFishingGearMark(FishingGearMarkDTO mark, FishingGearRegister fishingGearEntry)
        {
            FishingGearMark entry = new FishingGearMark
            {
                FishingGear = fishingGearEntry,
                MarkNum = mark.Number,
                MarkStatusId = mark.StatusId
            };

            Db.FishingGearMarks.Add(entry);
        }

        private void EditFishingGearMark(FishingGearMarkDTO mark)
        {
            FishingGearMark entry = (from m in Db.FishingGearMarks
                                     where m.Id == mark.Id
                                     select m).First();

            entry.MarkNum = mark.Number;
            entry.MarkStatusId = mark.StatusId;
            entry.IsActive = mark.IsActive;
        }

        private void AddOrEditFishingGearPingers(List<FishingGearPingerDTO> pingers, FishingGearRegister fishingGearEntry)
        {
            foreach (FishingGearPingerDTO pinger in pingers)
            {
                if (pinger.Id == null)
                {
                    AddFishingGearPinger(pinger, fishingGearEntry);
                }
                else
                {
                    EditFishingGearPinger(pinger);
                }
            }
        }

        private void AddFishingGearPinger(FishingGearPingerDTO pinger, FishingGearRegister fishingGearEntry)
        {
            FishingGearPinger entry = new FishingGearPinger
            {
                FishingGear = fishingGearEntry,
                PingerNum = pinger.Number,
                PingerStatusId = pinger.StatusId,
                Brand = pinger.Brand,
                Model = pinger.Model,
            };

            Db.FishingGearPingers.Add(entry);
        }

        private void EditFishingGearPinger(FishingGearPingerDTO pinger)
        {
            FishingGearPinger entry = (from p in Db.FishingGearPingers
                                       where p.Id == pinger.Id
                                       select p).First();

            entry.PingerNum = pinger.Number;
            entry.PingerStatusId = pinger.StatusId;
            entry.IsActive = pinger.IsActive;
            entry.Brand = pinger.Brand;
            entry.Model = pinger.Model;
        }
    }

    internal class FishingGearMarksAndPingersData
    {
        public ILookup<int, FishingGearMarkDTO> Marks { get; set; }

        public ILookup<int, FishingGearPingerDTO> Pingers { get; set; }
    }
}
