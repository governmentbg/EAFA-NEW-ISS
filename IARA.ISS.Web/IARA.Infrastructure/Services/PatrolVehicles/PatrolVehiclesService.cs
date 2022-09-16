using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Vehicles;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services.PatrolVehicles
{
    public class PatrolVehiclesService : Service, IPatrolVehiclesService
    {
        public PatrolVehiclesService(IARADbContext dbContext)
            : base(dbContext)
        {
        }
        public IQueryable<PatrolVehiclesDTO> GetAll(PatrolVehiclesFilters filters)
        {
            IQueryable<PatrolVehiclesDTO> result;
            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllPatrolVehicles(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParameterFilteredPatrolVehicles(filters)
                    : GetFreeTextPatrolVehicles(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }
            return result;
        }

        public IQueryable<PatrolVehiclesDTO> GetFreeTextPatrolVehicles(string text, bool showInactiveRecords)
        {
            text = text.ToLowerInvariant();
            IQueryable<PatrolVehiclesDTO> patrolVehicles = from unregVessel in Db.UnregisteredVessels
                                                           join country in Db.Ncountries on unregVessel.FlagCountryId equals country.Id into countries
                                                           from flagCountry in countries.DefaultIfEmpty()
                                                           join patrolVehicleType in Db.NpatrolVehicleTypes on unregVessel.PatrolVehicleTypeId equals patrolVehicleType.Id
                                                           join vesselType in Db.NvesselTypes on unregVessel.VesselTypeId equals vesselType.Id into vesselTypes
                                                           from vesselT in vesselTypes.DefaultIfEmpty()
                                                           join institution in Db.Ninstitutions on unregVessel.InstitutionId equals institution.Id into institutions
                                                           from vesselInstitution in institutions.DefaultIfEmpty()
                                                           where unregVessel.IsActive == !showInactiveRecords
                                                                && unregVessel.Name.ToLower().Contains(text)
                                                                || flagCountry.Name.ToLower().Contains(text)
                                                                || unregVessel.ExternalMark.ToLower().Contains(text)
                                                                || patrolVehicleType.Name.ToLower().Contains(text)
                                                                || unregVessel.Cfr.ToLower().Contains(text)
                                                                || unregVessel.Uvi.ToLower().Contains(text)
                                                                || unregVessel.IrcscallSign.ToLower().Contains(text)
                                                                || unregVessel.Mmsi.ToLower().Contains(text)
                                                                || vesselT.Name.ToLower().Contains(text)
                                                                || vesselInstitution.Name.ToLower().Contains(text)
                                                           select new PatrolVehiclesDTO
                                                           {
                                                               Id = unregVessel.Id,
                                                               Name = unregVessel.Name,
                                                               FlagCountry = unregVessel.PatrolVehicleTypeId != null
                                                                                ? flagCountry.Name
                                                                                : null,
                                                               ExternalMark = unregVessel.ExternalMark,
                                                               PatrolVehicleType = patrolVehicleType.Name,
                                                               Cfr = unregVessel.Cfr,
                                                               Uvi = unregVessel.Uvi,
                                                               IrcscallSign = unregVessel.IrcscallSign,
                                                               Mmsi = unregVessel.Mmsi,
                                                               VesselType = unregVessel.VesselTypeId != null
                                                                                ? vesselT.Name
                                                                                : null,
                                                               Institution = unregVessel.InstitutionId != null
                                                                                ? vesselInstitution.Name
                                                                                : null
                                                           };
            return patrolVehicles;
        }

        public IQueryable<PatrolVehiclesDTO> GetParameterFilteredPatrolVehicles(PatrolVehiclesFilters filters)
        {
            var query = from unregVessel in Db.UnregisteredVessels
                        join country in Db.Ncountries on unregVessel.FlagCountryId equals country.Id into countries
                        from flagCountry in countries.DefaultIfEmpty()
                        join patrolVehicleType in Db.NpatrolVehicleTypes on unregVessel.PatrolVehicleTypeId equals patrolVehicleType.Id
                        join vesselType in Db.NvesselTypes on unregVessel.VesselTypeId equals vesselType.Id into vesselTypes
                        from vesselT in vesselTypes.DefaultIfEmpty()
                        join institution in Db.Ninstitutions on unregVessel.InstitutionId equals institution.Id into institutions
                        from vesselInstitution in institutions.DefaultIfEmpty()
                        where unregVessel.IsActive == !filters.ShowInactiveRecords
                        select new
                        {
                            unregVessel.Id,
                            unregVessel.Name,
                            FlagCountryId = flagCountry.Id,
                            FlagCountry = unregVessel.FlagCountryId != null
                                              ? flagCountry.Name
                                              : null,
                            unregVessel.ExternalMark,
                            PatrolVehicleTypeId = patrolVehicleType.Id,
                            PatrolVehicleType = patrolVehicleType.Name,
                            unregVessel.Cfr,
                            unregVessel.Uvi,
                            unregVessel.IrcscallSign,
                            unregVessel.Mmsi,
                            VesselTypeId = vesselT.Id,
                            VesselType = unregVessel.VesselTypeId != null
                                             ? vesselT.Name
                                             : null,
                            InstitutionId = unregVessel.InstitutionId,
                            Institution = unregVessel.InstitutionId != null
                                              ? vesselInstitution.Name
                                              : null
                        };
            if (!string.IsNullOrEmpty(filters.Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(filters.Name));
            }
            if (filters.FlagCountryId.HasValue)
            {
                query = query.Where(x => x.FlagCountryId == filters.FlagCountryId.Value);
            }
            if (!string.IsNullOrEmpty(filters.ExternalMark))
            {
                query = query.Where(x => x.ExternalMark.ToLower().Contains(filters.ExternalMark));
            }
            if (filters.PatrolVehicleTypeId.HasValue)
            {
                query = query.Where(x => x.PatrolVehicleTypeId == filters.PatrolVehicleTypeId.Value);
            }
            if (!string.IsNullOrEmpty(filters.CFR))
            {
                query = query.Where(x => x.Cfr.ToLower().Contains(filters.CFR));
            }
            if (!string.IsNullOrEmpty(filters.UVI))
            {
                query = query.Where(x => x.Uvi.ToLower().Contains(filters.UVI));
            }
            if (!string.IsNullOrEmpty(filters.IRCSCallSign))
            {
                query = query.Where(x => x.IrcscallSign.ToLower().Contains(filters.IRCSCallSign));
            }
            if (!string.IsNullOrEmpty(filters.MMSI))
            {
                query = query.Where(x => x.Mmsi.ToLower().Contains(filters.MMSI));
            }
            if (filters.VesselTypeId.HasValue)
            {
                query = query.Where(x => x.VesselTypeId == filters.VesselTypeId.Value);
            }
            if (filters.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == filters.InstitutionId.Value);
            }

            IQueryable<PatrolVehiclesDTO> patrolVehicles = from patrolVehicle in query
                                                           select new PatrolVehiclesDTO
                                                           {
                                                               Id = patrolVehicle.Id,
                                                               Name = patrolVehicle.Name,
                                                               FlagCountry = patrolVehicle.FlagCountry,
                                                               ExternalMark = patrolVehicle.ExternalMark,
                                                               PatrolVehicleType = patrolVehicle.PatrolVehicleType,
                                                               Cfr = patrolVehicle.Cfr,
                                                               Uvi = patrolVehicle.Uvi,
                                                               IrcscallSign = patrolVehicle.IrcscallSign,
                                                               Mmsi = patrolVehicle.Mmsi,
                                                               VesselType = patrolVehicle.VesselType,
                                                               Institution = patrolVehicle.Institution
                                                           };
            return patrolVehicles;
        }

        public IQueryable<PatrolVehiclesDTO> GetAllPatrolVehicles(bool showInactiveRecords)
        {
            IQueryable<PatrolVehiclesDTO> patrolVehicles = from unregVessel in Db.UnregisteredVessels
                                                           join country in Db.Ncountries on unregVessel.FlagCountryId equals country.Id into countries
                                                           from flagCountry in countries.DefaultIfEmpty()
                                                           join patrolVehicleType in Db.NpatrolVehicleTypes on unregVessel.PatrolVehicleTypeId equals patrolVehicleType.Id
                                                           join vesselType in Db.NvesselTypes on unregVessel.VesselTypeId equals vesselType.Id into vesselTypes
                                                           from vesselT in vesselTypes.DefaultIfEmpty()
                                                           join institution in Db.Ninstitutions on unregVessel.InstitutionId equals institution.Id into institutions
                                                           from vesselInstitution in institutions.DefaultIfEmpty()
                                                           where unregVessel.IsActive == !showInactiveRecords
                                                           select new PatrolVehiclesDTO
                                                           {
                                                               Id = unregVessel.Id,
                                                               Name = unregVessel.Name,
                                                               FlagCountry = unregVessel.FlagCountryId != null
                                                                                 ? flagCountry.Name
                                                                                 : null,
                                                               ExternalMark = unregVessel.ExternalMark,
                                                               PatrolVehicleType = patrolVehicleType.Name,
                                                               Cfr = unregVessel.Cfr,
                                                               Uvi = unregVessel.Uvi,
                                                               IrcscallSign = unregVessel.IrcscallSign,
                                                               Mmsi = unregVessel.Mmsi,
                                                               VesselType = unregVessel.VesselTypeId != null
                                                                                  ? vesselT.Name
                                                                                  : null,
                                                               Institution = unregVessel.InstitutionId != null
                                                                                  ? vesselInstitution.Name
                                                                                  : null,
                                                               IsActive = unregVessel.IsActive
                                                           };
            return patrolVehicles;
        }

        public void DeletePatrolVehicle(int id)
        {
            DeleteRecordWithId(Db.UnregisteredVessels, id);
            Db.SaveChanges();
        }

        public void UndoDeletePatrolVehicle(int id)
        {
            UndoDeleteRecordWithId(Db.UnregisteredVessels, id);
            Db.SaveChanges();
        }

        public int AddPatrolVehicle(PatrolVehiclesEditDTO patrolVehicle)
        {
            UnregisteredVessel entry = new UnregisteredVessel
            {
                Name = patrolVehicle.Name,
                FlagCountryId = patrolVehicle.FlagCountryId,
                ExternalMark = patrolVehicle.ExternalMark,
                PatrolVehicleTypeId = patrolVehicle.PatrolVehicleTypeId,
                Cfr = patrolVehicle.Cfr,
                Uvi = patrolVehicle.Uvi,
                IrcscallSign = patrolVehicle.IrcscallSign,
                Mmsi = patrolVehicle.Mmsi,
                VesselTypeId = patrolVehicle.VesselTypeId,
                InstitutionId = patrolVehicle.InstitutionId
            };

            Db.UnregisteredVessels.Add(entry);
            Db.SaveChanges();

            return entry.Id;
        }

        public void EditPatrolVehicle(PatrolVehiclesEditDTO patrolVehicle)
        {
            UnregisteredVessel dbUnregisteredVessel = (from vessel in Db.UnregisteredVessels
                                                       where vessel.Id == patrolVehicle.Id
                                                       select vessel).First();

            dbUnregisteredVessel.Name = patrolVehicle.Name;
            dbUnregisteredVessel.FlagCountryId = patrolVehicle.FlagCountryId;
            dbUnregisteredVessel.ExternalMark = patrolVehicle.ExternalMark;
            dbUnregisteredVessel.PatrolVehicleTypeId = patrolVehicle.PatrolVehicleTypeId;
            dbUnregisteredVessel.Cfr = patrolVehicle.Cfr;
            dbUnregisteredVessel.Uvi = patrolVehicle.Uvi;
            dbUnregisteredVessel.IrcscallSign = patrolVehicle.IrcscallSign;
            dbUnregisteredVessel.Mmsi = patrolVehicle.Mmsi;
            dbUnregisteredVessel.VesselTypeId = patrolVehicle.VesselTypeId;
            dbUnregisteredVessel.InstitutionId = patrolVehicle.InstitutionId;

            Db.SaveChanges();
        }

        public PatrolVehiclesDTO GetPatrolVehicle(int id)
        {
            PatrolVehiclesDTO patrolVehicle = (from unregVessel in Db.UnregisteredVessels
                                               join country in Db.Ncountries on unregVessel.FlagCountryId equals country.Id into countries
                                               from flagCountry in countries.DefaultIfEmpty()
                                               join patrolVehicleType in Db.NpatrolVehicleTypes on unregVessel.PatrolVehicleTypeId equals patrolVehicleType.Id
                                               join vesselType in Db.NvesselTypes on unregVessel.VesselTypeId equals vesselType.Id into vesselTypes
                                               from vesselT in vesselTypes.DefaultIfEmpty()
                                               join institution in Db.Ninstitutions on unregVessel.InstitutionId equals institution.Id into institutions
                                               from vesselInstitution in institutions.DefaultIfEmpty()
                                               where unregVessel.Id == id
                                               select new PatrolVehiclesDTO
                                               {
                                                   Id = unregVessel.Id,
                                                   Name = unregVessel.Name,
                                                   FlagCountry = flagCountry.Name,
                                                   FlagCountryId = flagCountry.Id,
                                                   ExternalMark = unregVessel.ExternalMark,
                                                   PatrolVehicleType = patrolVehicleType.Name,
                                                   PatrolVehicleTypeId = patrolVehicleType.Id,
                                                   Cfr = unregVessel.Cfr,
                                                   Uvi = unregVessel.Uvi,
                                                   IrcscallSign = unregVessel.IrcscallSign,
                                                   Mmsi = unregVessel.Mmsi,
                                                   VesselType = vesselT.Name,
                                                   VesselTypeId = vesselT.Id,
                                                   Institution = vesselInstitution.Name,
                                                   InstitutionId = vesselInstitution.Id
                                               }).First();
            return patrolVehicle;
        }
        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.UnregisteredVessels, id);
        }
    }
}
