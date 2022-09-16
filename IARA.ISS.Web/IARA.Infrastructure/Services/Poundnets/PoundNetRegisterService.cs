using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels;
using IARA.DomainModels.DTOModels.PoundnetRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces;
using NetTopologySuite.Geometries;

namespace IARA.Infrastructure.Services
{
    public class PoundNetRegisterService : Service, IPoundNetRegisterService
    {
        public PoundNetRegisterService(IARADbContext db)
                 : base(db)
        {
        }

        public IQueryable<PoundNetDTO> GetAll(PoundNetRegisterFilters filters)
        {
            IQueryable<PoundNetDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllPoundnets(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredPoundnets(filters)
                    : GetFreeTextFilteredPoundnets(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            return result;
        }

        public PoundnetRegisterDTO Get(int id)
        {
            PoundnetRegisterDTO result = (from poundnet in Db.PoundNetRegisters
                                          where poundnet.Id == id
                                          select new PoundnetRegisterDTO
                                          {
                                              Id = poundnet.Id,
                                              PoundNetNum = poundnet.PoundNetNum,
                                              RegistrationDate = poundnet.RegistrationDate,
                                              Name = poundnet.Name,
                                              SeasonTypeId = poundnet.SeasonTypeId,
                                              CategoryTypeId = poundnet.CategoryTypeId,
                                              ActivityOrderNum = poundnet.ActivityOrderNum,
                                              ActivityOrderDate = poundnet.ActivityOrderDate,
                                              Comments = poundnet.Comments,
                                              DepthFrom = poundnet.DepthFrom,
                                              DepthTo = poundnet.DepthTo,
                                              TowelLength = poundnet.TowelLength,
                                              HouseWidth = poundnet.HouseWidth,
                                              HouseLength = poundnet.HouseLength,
                                              BagEyeSize = poundnet.BagEyeSize,
                                              DistrictId = poundnet.DistrictId,
                                              MunicipalityId = poundnet.MunicipalityId,
                                              PopulatedAreaId = poundnet.PopulatedAreaId,
                                              Region = poundnet.Region,
                                              LocationDescription = poundnet.LocationDescription,
                                              StatusId = poundnet.StatusId,
                                              PermitLicensePrice = poundnet.PermitLicencePrice,
                                              AreaDescription = poundnet.AreaDescription
                                          }).First();

            result.PoundnetCoordinates = GetCoordinates(result.Id.Value);

            return result;
        }

        public int Add(PoundnetRegisterDTO poundnet)
        {
            PoundNetRegister dbPoundnet = new PoundNetRegister
            {
                PoundNetNum = poundnet.PoundNetNum,
                RegistrationDate = poundnet.RegistrationDate.Value,
                Name = poundnet.Name,
                SeasonTypeId = poundnet.SeasonTypeId.Value,
                CategoryTypeId = poundnet.CategoryTypeId.Value,
                ActivityOrderNum = poundnet.ActivityOrderNum,
                ActivityOrderDate = poundnet.ActivityOrderDate,
                Comments = poundnet.Comments,
                DepthFrom = poundnet.DepthFrom,
                DepthTo = poundnet.DepthTo,
                TowelLength = (int?)poundnet.TowelLength,
                HouseLength = (int?)poundnet.HouseLength,
                HouseWidth = (int?)poundnet.HouseWidth,
                BagEyeSize = (int?)poundnet.BagEyeSize,
                DistrictId = poundnet.DistrictId,
                MunicipalityId = poundnet.MunicipalityId,
                PopulatedAreaId = poundnet.PopulatedAreaId,
                Region = poundnet.Region,
                LocationDescription = poundnet.LocationDescription,
                StatusId = poundnet.StatusId.Value,
                AreaDescription = poundnet.AreaDescription,
                PermitLicencePrice = poundnet.PermitLicensePrice
            };

            Db.PoundNetRegisters.Add(dbPoundnet);

            AddCoordinates(dbPoundnet, poundnet.PoundnetCoordinates);

            Db.SaveChanges();

            return dbPoundnet.Id;
        }

        public void Edit(PoundnetRegisterDTO poundnet)
        {
            PoundNetRegister dbPoundnet = GetActiveRecord(Db.PoundNetRegisters, poundnet.Id.Value);

            dbPoundnet.PoundNetNum = poundnet.PoundNetNum;
            dbPoundnet.RegistrationDate = poundnet.RegistrationDate.Value;
            dbPoundnet.Name = poundnet.Name;
            dbPoundnet.SeasonTypeId = poundnet.SeasonTypeId.Value;
            dbPoundnet.CategoryTypeId = poundnet.CategoryTypeId.Value;
            dbPoundnet.ActivityOrderNum = poundnet.ActivityOrderNum;
            dbPoundnet.ActivityOrderDate = poundnet.ActivityOrderDate;
            dbPoundnet.Comments = poundnet.Comments;
            dbPoundnet.DepthFrom = poundnet.DepthFrom;
            dbPoundnet.DepthTo = poundnet.DepthTo;
            dbPoundnet.TowelLength = (int?)poundnet.TowelLength;
            dbPoundnet.HouseLength = (int?)poundnet.HouseLength;
            dbPoundnet.HouseWidth = (int?)poundnet.HouseWidth;
            dbPoundnet.BagEyeSize = (int?)poundnet.BagEyeSize;
            dbPoundnet.DistrictId = poundnet.DistrictId;
            dbPoundnet.MunicipalityId = poundnet.MunicipalityId;
            dbPoundnet.PopulatedAreaId = poundnet.PopulatedAreaId;
            dbPoundnet.Region = poundnet.Region;
            dbPoundnet.LocationDescription = poundnet.LocationDescription;
            dbPoundnet.StatusId = poundnet.StatusId.Value;
            dbPoundnet.AreaDescription = poundnet.AreaDescription;
            dbPoundnet.PermitLicencePrice = poundnet.PermitLicensePrice;

            EditCoordinates(poundnet.Id.Value, poundnet.PoundnetCoordinates);

            Db.SaveChanges();
        }

        public void Delete(int id)
        {
            DeleteRecordWithId(Db.PoundNetRegisters, id);
            Db.SaveChanges();
        }

        public void UndoDelete(int id)
        {
            UndoDeleteRecordWithId(Db.PoundNetRegisters, id);
            Db.SaveChanges();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.PoundNetRegisters, id);
        }

        private IQueryable<PoundNetDTO> GetParametersFilteredPoundnets(PoundNetRegisterFilters filters)
        {
            var result = from poundnet in Db.PoundNetRegisters
                         join category in Db.NpoundNetCategoryTypes on poundnet.CategoryTypeId equals category.Id
                         join seasonType in Db.NpoundNetSeasonTypes on poundnet.SeasonTypeId equals seasonType.Id
                         join muncipality in Db.Nmunicipalities on poundnet.MunicipalityId equals muncipality.Id into left
                         from muncipality in left.DefaultIfEmpty()
                         join status in Db.NpoundNetStatuses on poundnet.StatusId equals status.Id
                         where poundnet.IsActive == !filters.ShowInactiveRecords
                                 && (string.IsNullOrEmpty(filters.Name) || poundnet.Name.ToLower().Contains(filters.Name.ToLower()))
                                 && (string.IsNullOrEmpty(filters.Number) || poundnet.PoundNetNum.ToLower().Contains(filters.Number.ToLower()))
                                 && (!filters.MuncipalityId.HasValue || poundnet.MunicipalityId == filters.MuncipalityId)
                                 && (!filters.SeasonTypeId.HasValue || poundnet.SeasonTypeId == filters.SeasonTypeId)
                                 && (!filters.CategoryTypeId.HasValue || poundnet.CategoryTypeId == filters.CategoryTypeId)
                                 && (!filters.StatusId.HasValue || poundnet.StatusId == filters.StatusId)
                                 && (!filters.RegisteredDateFrom.HasValue || poundnet.RegistrationDate >= filters.RegisteredDateFrom)
                                 && (!filters.RegisteredDateTo.HasValue || poundnet.RegistrationDate <= filters.RegisteredDateTo)
                         orderby poundnet.RegistrationDate descending, poundnet.Name
                         select new PoundNetDTO
                         {
                             ID = poundnet.Id,
                             CategoryType = category.Name,
                             Muncipality = muncipality != null ? muncipality.Name : null,
                             Name = poundnet.Name,
                             Number = poundnet.PoundNetNum,
                             RegistrationDate = poundnet.RegistrationDate,
                             SeasonType = seasonType.Name,
                             Status = status.Name,
                             StatusCode = Enum.Parse<PoundNetStatusesEnum>(status.Code),
                             IsActive = poundnet.IsActive
                         };

            return result;
        }

        private IQueryable<PoundNetDTO> GetFreeTextFilteredPoundnets(string text, bool showInactive)
        {
            text = text.ToLowerInvariant();

            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            var result = from poundNet in Db.PoundNetRegisters
                         join categoryType in Db.NpoundNetCategoryTypes on poundNet.CategoryTypeId equals categoryType.Id
                         join seasonType in Db.NpoundNetSeasonTypes on poundNet.SeasonTypeId equals seasonType.Id
                         join muncipality in Db.Nmunicipalities on poundNet.MunicipalityId equals muncipality.Id into left
                         from muncipality in left.DefaultIfEmpty()
                         join status in Db.NpoundNetStatuses on poundNet.StatusId equals status.Id
                         where poundNet.IsActive == !showInactive
                             && (poundNet.Name.ToLower().Contains(text)
                                || (muncipality != null && muncipality.Name.ToLower().Contains(text))
                                || categoryType.Name.ToLower().Contains(text)
                                || seasonType.Name.ToLower().Contains(text)
                                || status.Name.ToLower().Contains(text)
                                || poundNet.RegistrationDate == searchDate)
                         orderby poundNet.RegistrationDate descending, poundNet.Name
                         select new PoundNetDTO
                         {
                             ID = poundNet.Id,
                             CategoryType = categoryType.Name,
                             Muncipality = muncipality != null ? muncipality.Name : null,
                             Name = poundNet.Name,
                             Number = poundNet.PoundNetNum,
                             RegistrationDate = poundNet.RegistrationDate,
                             SeasonType = seasonType.Name,
                             Status = status.Name,
                             StatusCode = Enum.Parse<PoundNetStatusesEnum>(status.Code),
                             IsActive = poundNet.IsActive
                         };

            return result;
        }

        private IQueryable<PoundNetDTO> GetAllPoundnets(bool showInactive)
        {
            var result = from poundNet in Db.PoundNetRegisters
                         join category in Db.NpoundNetCategoryTypes on poundNet.CategoryTypeId equals category.Id
                         join season in Db.NpoundNetSeasonTypes on poundNet.SeasonTypeId equals season.Id
                         join muncipality in Db.Nmunicipalities on poundNet.MunicipalityId equals muncipality.Id into left
                         from muncipality in left.DefaultIfEmpty()
                         join status in Db.NpoundNetStatuses on poundNet.StatusId equals status.Id
                         where poundNet.IsActive == !showInactive
                         orderby poundNet.RegistrationDate descending, poundNet.Name
                         select new PoundNetDTO
                         {
                             ID = poundNet.Id,
                             CategoryType = category.Name,
                             Muncipality = muncipality != null ? muncipality.Name : null,
                             Name = poundNet.Name,
                             Number = poundNet.PoundNetNum,
                             RegistrationDate = poundNet.RegistrationDate,
                             SeasonType = season.Name,
                             Status = status.Name,
                             StatusCode = Enum.Parse<PoundNetStatusesEnum>(status.Code),
                             IsActive = poundNet.IsActive
                         };

            return result;
        }

        private List<PoundnetCoordinateDTO> GetCoordinates(int poundnetId)
        {
            List<PoundnetCoordinateDTO> coordinates = (from x in Db.PoundNetCoordinates
                                                       where x.PoundNetId == poundnetId
                                                       orderby x.PointNum
                                                       select new PoundnetCoordinateDTO
                                                       {
                                                           Id = x.Id,
                                                           IsActive = x.IsActive,
                                                           IsConnectPoint = x.IsConnectPoint,
                                                           Longitude = new DMSType(x.Coordinates.X).ToString(),
                                                           Latitude = new DMSType(x.Coordinates.Y).ToString()
                                                       }).ToList();

            return coordinates;
        }

        private void AddCoordinates(PoundNetRegister poundnet, List<PoundnetCoordinateDTO> coords)
        {
            short counter = 1;
            foreach (PoundnetCoordinateDTO coordinate in coords)
            {
                CoordinatesDMS coordinates = CoordinatesDMS.Parse(coordinate.Longitude, coordinate.Latitude);

                PoundNetCoordinate entry = new PoundNetCoordinate
                {
                    PoundNet = poundnet,
                    PointNum = counter++,
                    IsConnectPoint = coordinate.IsConnectPoint.Value,
                    IsActive = coordinate.IsActive.Value
                };

                entry.Coordinates = new Point(coordinates.Longitude.ToDecimal(), coordinates.Latitude.ToDecimal());

                Db.PoundNetCoordinates.Add(entry);
            }
        }

        private void EditCoordinates(int poundnetId, List<PoundnetCoordinateDTO> coords)
        {
            List<PoundNetCoordinate> dbPoundnetCoordinates = Db.PoundNetCoordinates.Where(x => x.PoundNetId == poundnetId).ToList();

            short counter = 1;
            foreach (PoundnetCoordinateDTO coordinate in coords)
            {
                if (coordinate.Id.HasValue)
                {
                    PoundNetCoordinate dbCoordinate = dbPoundnetCoordinates.Where(x => x.Id == coordinate.Id.Value).Single();

                    dbCoordinate.PointNum = counter++;
                    dbCoordinate.IsConnectPoint = coordinate.IsConnectPoint.Value;

                    CoordinatesDMS dmsCoordinate = CoordinatesDMS.Parse(coordinate.Longitude, coordinate.Latitude);
                    dbCoordinate.Coordinates = new Point(dmsCoordinate.Longitude.ToDecimal(), dmsCoordinate.Latitude.ToDecimal());
                }
                else
                {
                    PoundNetCoordinate dbCoordinate = new PoundNetCoordinate
                    {
                        PointNum = counter++,
                        PoundNetId = poundnetId,
                        IsConnectPoint = coordinate.IsConnectPoint.Value,
                        IsActive = true
                    };

                    CoordinatesDMS dmsCoordinate = CoordinatesDMS.Parse(coordinate.Longitude, coordinate.Latitude);
                    dbCoordinate.Coordinates = new Point(dmsCoordinate.Longitude.ToDecimal(), dmsCoordinate.Latitude.ToDecimal());

                    Db.PoundNetCoordinates.Add(dbCoordinate);
                }
            }

            List<int> dbUpdatedCoordinates = coords.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToList();
            List<PoundNetCoordinate> coordinatesToDelete = dbPoundnetCoordinates.Where(x => !dbUpdatedCoordinates.Contains(x.Id)).ToList();

            foreach (PoundNetCoordinate coordinate in coordinatesToDelete)
            {
                Db.PoundNetCoordinates.Remove(coordinate);
            }
        }
    }
}
