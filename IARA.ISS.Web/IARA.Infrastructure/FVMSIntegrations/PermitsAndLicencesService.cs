using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.FVMSModels.ExternalModels;
using IARA.Interfaces.FVMSIntegrations;

namespace IARA.Infrastructure.FVMSIntegrations
{
    public class PermitsAndLicencesService : BaseService, IPermitsAndLicencesService
    {
        public PermitsAndLicencesService(IARADbContext dbContext)
            : base(dbContext)
        { }

        /// <summary>
        /// Заявка за данни за риболовни уреди по номер на удостоверение за риболов
        /// </summary>
        /// <param name="licenseNumber"></param>
        /// <returns></returns>
        public List<FishingGear> GetFishingGears(string licenseNumber)
        {
            return (from permitGear in this.Db.PermitLicenseRegisterFishingGears
                    join gear in this.Db.FishingGearRegisters on permitGear.FishingGearId equals gear.Id
                    join gearType in this.Db.NfishingGears on gear.FishingGearTypeId equals gearType.Id
                    join license in this.Db.CommercialFishingPermitLicensesRegisters on permitGear.PermitLicenseId equals license.Id
                    join ship in this.Db.ShipsRegister on license.ShipId equals ship.Id
                    where license.RegistrationNum == licenseNumber
                       && license.IsActive
                       && permitGear.IsActive
                       && gear.IsActive
                    select new FishingGear
                    {
                        CFR = ship.Cfr,
                        Quantity = gear.GearCount,
                        Eye = (double)(gear.NetEyeSize ?? 0),
                        Notes = gear.Description,
                        Code = gearType.Code,
                        Name = gearType.Name
                    }).ToList();
        }

        /// <summary>
        /// Заявка за данни за удостоверения за риболов по номер на удостоверение за риболов
        /// </summary>
        /// <returns></returns>
        public Certificate GetLicense(string licenseNumber)
        {
            DateTime now = DateTime.Now;

            Certificate dbLicense = (from license in this.Db.CommercialFishingPermitLicensesRegisters
                                     join logbookPermit in this.Db.LogBookPermitLicenses on license.Id equals logbookPermit.PermitLicenseRegisterId
                                     join logbook in this.Db.LogBooks on logbookPermit.LogBookId equals logbook.Id
                                     join ship in this.Db.ShipsRegister on license.ShipId equals ship.Id
                                     where license.RegistrationNum == licenseNumber
                                        && license.IsActive
                                        && logbook.IsActive
                                        && logbookPermit.LogBookValidTo <= now
                                        && logbookPermit.LogBookValidFrom > now
                                        && license.RecordType == RecordTypesEnum.Register.ToString()
                                     select new Certificate
                                     {
                                         CFR = ship.Cfr,
                                         LogBookNumber = logbook.LogNum,
                                         LicenceCreatedOn = license.CreatedOn,
                                         LicenceNumber = license.RegistrationNum,
                                         LicenceValidFrom = license.PermitLicenseValidFrom.Value,
                                         LicenceValidTo = license.PermitLicenseValidTo.Value,
                                         LogBookCreatedOn = logbookPermit.CreatedOn,
                                         LogBookStartDate = logbookPermit.LogBookValidFrom,
                                         LogBookStartPage = logbookPermit.StartPageNum.HasValue
                                                            ? logbookPermit.StartPageNum.Value
                                                            : logbook.StartPageNum,
                                         LogBookEndPage = logbookPermit.EndPageNum.HasValue
                                                          ? logbookPermit.EndPageNum.Value
                                                          : logbook.EndPageNum,
                                         NextPage = logbookPermit.StartPageNum < logbookPermit.EndPageNum,
                                         CurrentPage = logbook.LastPageNum
                                     }).FirstOrDefault();

            return dbLicense;
        }

        /// <summary>
        /// Заявка за данни за удостоверения за риболов по номер на разрешително за риболов
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<Certificate> GetLicensesByPermitNumber(string permitNumber)
        {
            return this.GetPermit(permitNumber).Certificates.ToList();
        }

        /// <summary>
        /// Заявка за данни за удостоверения за риболов по CFR номер на РК
        /// </summary>
        /// <param name="cfr">Номер на кораб</param>
        /// <returns></returns>
        public List<Certificate> GetLicensesByCFR(string cfr)
        {
            return this.GetPermits(cfr).SelectMany(x => x.Certificates).ToList();
        }

        /// <summary>
        /// Заявка за данни за разрешителни за риболов по уникален номер
        /// </summary>
        /// <param name="permitNumber">Уникален номер на разрешително</param>
        /// <returns></returns>
        public Permit GetPermit(string permitNumber)
        {
            InternalPermit dbPermit = (from permit in this.Db.CommercialFishingPermitRegisters
                                       join ship in this.Db.ShipsRegister on permit.ShipId equals ship.Id
                                       where permit.RegistrationNum == permitNumber
                                          && permit.IsActive
                                          && permit.RecordType == RecordTypesEnum.Register.ToString()
                                       select new InternalPermit
                                       {
                                           Id = permit.Id,
                                           CFR = ship.Cfr,
                                           CreatedOn = permit.CreatedOn,
                                           IsSuspended = permit.IsSuspended,
                                           RegistrationNum = permit.RegistrationNum,
                                           VesselName = ship.Name
                                       }).FirstOrDefault();

            if (dbPermit != null)
            {

                ILookup<int, InternalLicense> licenses = this.GetLicencesByPermitIds(dbPermit.CFR, dbPermit.VesselName, new List<int> { dbPermit.Id });

                ILookup<int, InternalGear> gears = this.GetGearsByLicenceIds(dbPermit.CFR, licenses.SelectMany(x => x.Select(y => y.Id)).ToList());

                ILookup<int, string> marks = this.GetMarksByGearIds(gears.SelectMany(x => x.Select(x => x.Id)).ToList());

                return this.BuildPermit(dbPermit, licenses, gears, marks);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Заявка за данни за разрешителни за риболов по CFR номер на РК
        /// </summary>
        /// <param name="cfr">CFR номер на кораб</param>
        /// <returns></returns>
        public List<Permit> GetPermits(string cfr)
        {
            Dictionary<int, InternalPermit> permitDict = (from permit in this.Db.CommercialFishingPermitRegisters
                                                          join ship in this.Db.ShipsRegister on permit.ShipId equals ship.Id
                                                          where ship.Cfr == cfr
                                                             && permit.IsActive
                                                             && permit.RecordType == RecordTypesEnum.Register.ToString()
                                                          select new InternalPermit
                                                          {
                                                              Id = permit.Id,
                                                              CFR = cfr,
                                                              CreatedOn = permit.CreatedOn,
                                                              IsSuspended = permit.IsSuspended,
                                                              RegistrationNum = permit.RegistrationNum,
                                                              VesselName = ship.Name
                                                          }).ToDictionary(x => x.Id, permit => permit);

            if (permitDict != null && permitDict.Any())
            {
                ILookup<int, InternalLicense> licenses = this.GetLicencesByPermitIds(cfr, permitDict.First().Value.VesselName, permitDict.Keys);

                ILookup<int, InternalGear> gears = this.GetGearsByLicenceIds(cfr, licenses.SelectMany(x => x.Select(x => x.Id)).Distinct().ToList());

                ILookup<int, string> marks = this.GetMarksByGearIds(gears.SelectMany(x => x.Select(x => x.Id)).ToList());

                List<Permit> permits = new List<Permit>();

                foreach (KeyValuePair<int, InternalPermit> item in permitDict)
                {
                    Permit permit = this.BuildPermit(item.Value, licenses, gears, marks);
                    permits.Add(permit);
                }

                return permits;
            }
            else
            {
                return null;
            }
        }



        private Permit BuildPermit(InternalPermit internalPermit, ILookup<int, InternalLicense> licenses, ILookup<int, InternalGear> gears, ILookup<int, string> marks)
        {
            Permit permit = new Permit
            {
                CFR = internalPermit.CFR,
                CreatedOn = internalPermit.CreatedOn,
                IsRevoked = internalPermit.IsSuspended,
                Number = internalPermit.RegistrationNum,
                Certificates = new List<Certificate>()
            };

            IEnumerable<InternalLicense> permitLicenses = licenses[internalPermit.Id];

            foreach (InternalLicense dbLicense in permitLicenses)
            {
                Certificate license = new Certificate
                {
                    CFR = dbLicense.CFR,
                    CurrentPage = dbLicense.CurrentPage,
                    LicenceCreatedOn = dbLicense.LicenceCreatedOn,
                    LicenceNumber = dbLicense.LicenceNumber,
                    LicenceValidFrom = dbLicense.LicenceValidFrom,
                    LicenceValidTo = dbLicense.LicenceValidTo,
                    LogBookCreatedOn = dbLicense.LogBookCreatedOn,
                    LogBookEndPage = dbLicense.LogBookEndPage,
                    LogBookNumber = dbLicense.LogBookNumber,
                    LogBookStartDate = dbLicense.LogBookStartDate,
                    LogBookStartPage = dbLicense.LogBookStartPage,
                    NextPage = dbLicense.NextPage,
                    Name = dbLicense.VesselName
                };

                license.FishingGears = new List<FishingGear>();

                foreach (InternalGear dbGear in gears[dbLicense.Id])
                {
                    FishingGear gear = new FishingGear
                    {
                        CFR = dbGear.CFR,
                        Code = dbGear.Code,
                        Eye = (double)dbGear.Eye,
                        Name = dbGear.Name,
                        Notes = dbGear.Notes,
                        Quantity = dbGear.Quantity
                    };

                    gear.Marks = string.Join(",", marks[dbGear.Id]);
                    license.FishingGears.Add(gear);
                }

                permit.Certificates.Add(license);
            }

            return permit;
        }


        private ILookup<int, InternalGear> GetGearsByLicenceIds(string cfr, ICollection<int> licenseIds)
        {
            ILookup<int, InternalGear> gears = (from gear in this.Db.FishingGearRegisters
                                                join gearType in this.Db.NfishingGears on gear.FishingGearTypeId equals gearType.Id
                                                where gear.PermitLicenseId.HasValue && licenseIds.Contains(gear.PermitLicenseId.Value)
                                                select new InternalGear
                                                {
                                                    Id = gear.Id,
                                                    PermitLicenseId = gear.PermitLicenseId.Value,
                                                    CFR = cfr,
                                                    Quantity = gear.GearCount,
                                                    Eye = gear.NetEyeSize ?? 0,
                                                    Notes = gear.Description,
                                                    Code = gearType.Code,
                                                    Name = gearType.Name
                                                }).ToLookup(x => x.PermitLicenseId, x => x);

            return gears;
        }

        private ILookup<int, InternalLicense> GetLicencesByPermitIds(string cfr, string vesselName, ICollection<int> permitIds)
        {
            DateTime now = DateTime.Now;

            ILookup<int, InternalLicense> licenses = (from license in this.Db.CommercialFishingPermitLicensesRegisters
                                                      join logbookLicense in this.Db.LogBookPermitLicenses on license.Id equals logbookLicense.PermitLicenseRegisterId
                                                      join logbook in this.Db.LogBooks on logbookLicense.LogBookId equals logbook.Id
                                                      where permitIds.Contains(license.PermitId)
                                                         && license.IsActive
                                                         && logbook.IsActive
                                                         && logbookLicense.LogBookValidTo >= now
                                                         && logbookLicense.LogBookValidFrom <= now
                                                         && license.RecordType == RecordTypesEnum.Register.ToString()
                                                         && logbookLicense.StartPageNum.HasValue
                                                         && logbookLicense.EndPageNum.HasValue
                                                      select new InternalLicense
                                                      {
                                                          Id = license.Id,
                                                          PermitId = license.PermitId,
                                                          CFR = cfr,
                                                          VesselName = vesselName,
                                                          LogBookNumber = logbook.LogNum,
                                                          LicenceCreatedOn = license.CreatedOn,
                                                          LicenceNumber = license.RegistrationNum,
                                                          LicenceValidFrom = license.PermitLicenseValidFrom.Value,
                                                          LicenceValidTo = license.PermitLicenseValidTo.Value,
                                                          LogBookCreatedOn = logbookLicense.CreatedOn,
                                                          LogBookStartDate = logbookLicense.LogBookValidFrom,
                                                          LogBookStartPage = logbookLicense.StartPageNum.Value,
                                                          LogBookEndPage = logbookLicense.EndPageNum.Value,
                                                          NextPage = logbookLicense.StartPageNum.Value < logbookLicense.EndPageNum.Value,
                                                          CurrentPage = logbook.LastPageNum == 0
                                                                        ? logbookLicense.StartPageNum.Value
                                                                        : logbook.LastPageNum > logbookLicense.EndPageNum.Value
                                                                            ? logbookLicense.EndPageNum.Value
                                                                            : logbook.LastPageNum
                                                      }).ToLookup(x => x.PermitId, x => x);

            return licenses;
        }

        private ILookup<int, string> GetMarksByGearIds(ICollection<int> gearIds)
        {
            ILookup<int, string> marks = (from mark in this.Db.FishingGearMarks
                                          where mark.IsActive && gearIds.Contains(mark.FishingGearId)
                                          select new
                                          {
                                              mark.FishingGearId,
                                              mark.MarkNum
                                          }).ToLookup(x => x.FishingGearId, x => x.MarkNum);

            return marks;
        }
    }
}
