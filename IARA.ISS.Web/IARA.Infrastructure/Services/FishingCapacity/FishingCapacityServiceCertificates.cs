using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Excel.Tools.Models;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public partial class FishingCapacityService : Service, IFishingCapacityService
    {
        public IQueryable<FishingCapacityCertificateDTO> GetAllCapacityCertificates(FishingCapacityCertificatesFilters filters)
        {
            IQueryable<FishingCapacityCertificateDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllCapacityCertificates(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredCapacityCertificates(filters)
                    : GetFreeTextFilteredCapacityCertificates(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }
            return result;
        }

        public void SetCapacityCertificateHistory(List<FishingCapacityCertificateDTO> certificates)
        {
            List<int> certificateIds = certificates.Select(x => x.Id).ToList();

            List<CapacityCertificateHistoryDTO> result = (from certificateId in certificateIds
                                                          select new CapacityCertificateHistoryDTO
                                                          {
                                                              CertificateId = certificateId,
                                                              TransferredTo = new List<CapacityCertificateHistoryTransferredToDTO>(),
                                                              RemainderTransferredTo = new List<CapacityCertificateHistoryTransferredToDTO>()
                                                          }).ToList();

            Dictionary<int, CapacityCertificateHistoryApplDTO> createdFrom = GetCapacityCertificateHistoryCreatedFrom(certificateIds);
            Dictionary<int, CapacityCertificateHistoryApplDTO> usedInIncrease = GetCapacityCertificateHistoryUsedInIncrease(certificateIds);
            Dictionary<int, CapacityCertificateHistoryApplDTO> usedInTransfer = GetCapacityCertificateHistoryUsedInTransfer(certificateIds);
            Dictionary<int, CapacityCertificateHistoryApplDTO> usedInDuplicate = GetCapacityCertificateHistoryUsedInDuplicate(certificateIds);

            List<int> usedInIncreaseApplicationIds = usedInIncrease.Select(x => x.Value.ApplicationId).ToList();
            ILookup<int, CapacityCertificateHistoryTransferredToDTO> remainderTransferredTo = GetCapacityCertificateHistoryRemainderTransferredTo(usedInIncreaseApplicationIds);

            List<int> transferCertificateIds = usedInTransfer.Select(x => x.Key).ToList();
            ILookup<int, CapacityCertificateHistoryTransferredToDTO> transferredTo = GetCapacityCertificateHistoryTransferredTo(transferCertificateIds);

            foreach (CapacityCertificateHistoryDTO entry in result)
            {
                entry.CreatedFromApplication = createdFrom[entry.CertificateId];

                if (usedInIncrease.TryGetValue(entry.CertificateId, out CapacityCertificateHistoryApplDTO usedInIncreaseAppl))
                {
                    entry.UsedInApplication = usedInIncreaseAppl;
                }
                else if (usedInTransfer.TryGetValue(entry.CertificateId, out CapacityCertificateHistoryApplDTO usedInTransferAppl))
                {
                    entry.UsedInApplication = usedInTransferAppl;
                }
                else if (usedInDuplicate.TryGetValue(entry.CertificateId, out CapacityCertificateHistoryApplDTO usedInDuplicateAppl))
                {
                    entry.UsedInApplication = usedInDuplicateAppl;
                }

                if (entry.UsedInApplication != null)
                {
                    if (entry.UsedInApplication.PageCode == PageCodeEnum.TransferFishCap)
                    {
                        entry.TransferredTo = transferredTo[entry.CertificateId].ToList();
                        entry.UsedInApplication.TransferredCapacityCertificate = string.Join(", ", entry.TransferredTo.Select(x => x.CertificateNum));
                    }
                    else if (entry.UsedInApplication.PageCode == PageCodeEnum.RegVessel || entry.UsedInApplication.PageCode == PageCodeEnum.IncreaseFishCap)
                    {
                        entry.RemainderTransferredTo = remainderTransferredTo[entry.UsedInApplication.ApplicationId].ToList();
                    }
                }
            }

            foreach (CapacityCertificateHistoryDTO hist in result)
            {
                FishingCapacityCertificateDTO entry = certificates.Where(x => x.Id == hist.CertificateId).SingleOrDefault();

                if (entry != null)
                {
                    entry.History = hist;
                }
            }
        }

        public FishingCapacityCertificateEditDTO GetCapacityCertificate(int id)
        {
            var capacity = (from cap in Db.CapacityCertificatesRegister
                            where cap.Id == id
                            select new
                            {
                                cap.Id,
                                cap.CertificateNum,
                                cap.CertificateValidFrom,
                                cap.CertificateValidTo,
                                cap.GrossTonnage,
                                cap.MainEnginePower,
                                cap.PersonId,
                                cap.LegalId,
                                cap.Comments
                            }).First();

            int? duplicateOf = (from change in Db.CapacityChangeHistory
                                join changeCert in Db.CapacityChangeHistoryCertificates on change.Id equals changeCert.CapacityChangeHistoryId
                                join transferCap in Db.CapacityCertificatesRegister on change.CapacityCertificateTransferId equals transferCap.Id
                                where change.RecordType == nameof(RecordTypesEnum.Register)
                                     && change.TypeOfChange == nameof(FishingCapacityChangeTypeEnum.Duplicate)
                                     && changeCert.CapacityCertificateId == id
                                     && changeCert.IsActive
                                select transferCap.CertificateNum).SingleOrDefault();

            FishingCapacityCertificateEditDTO result = new FishingCapacityCertificateEditDTO
            {
                Id = capacity.Id,
                DuplicateOfCertificateNum = duplicateOf.HasValue ? duplicateOf.ToString() : null,
                CertificateNum = capacity.CertificateNum.ToString(),
                ValidFrom = capacity.CertificateValidFrom,
                ValidTo = capacity.CertificateValidTo,
                GrossTonnage = capacity.GrossTonnage,
                Power = capacity.MainEnginePower,
                Comments = capacity.Comments,
                IsHolderPerson = capacity.PersonId.HasValue
            };

            if (result.IsHolderPerson.Value)
            {
                result.Person = personService.GetRegixPersonData(capacity.PersonId.Value);
                result.Addresses = personService.GetAddressRegistrations(capacity.PersonId.Value);
            }
            else
            {
                result.Legal = legalService.GetRegixLegalData(capacity.LegalId.Value);
                result.Addresses = legalService.GetAddressRegistrations(capacity.LegalId.Value);
            }

            return result;
        }

        public void EditCapacityCertificate(FishingCapacityCertificateEditDTO certificate)
        {
            CapacityCertificatesRegister dbCertificate = (from cert in Db.CapacityCertificatesRegister
                                                          where cert.Id == certificate.Id.Value
                                                          select cert).First();

            dbCertificate.Comments = certificate.Comments;

            if (dbCertificate.PersonId.HasValue)
            {
                dbCertificate.Person = Db.AddOrEditPerson(certificate.Person, certificate.Addresses, dbCertificate.PersonId.Value);
            }
            else
            {
                int applicationId = (from changeCert in Db.CapacityChangeHistoryCertificates
                                     join change in Db.CapacityChangeHistory on changeCert.CapacityChangeHistoryId equals change.Id
                                     where changeCert.CapacityCertificateId == dbCertificate.Id
                                        && change.RecordType == nameof(RecordTypesEnum.Register)
                                     select change.ApplicationId.Value).First();

                dbCertificate.Legal = Db.AddOrEditLegal(
                    new ApplicationRegisterDataDTO
                    {
                        ApplicationId = applicationId,
                        RecordType = RecordTypesEnum.Register
                    },
                    certificate.Legal, certificate.Addresses, dbCertificate.LegalId.Value
                );
            }

            Db.SaveChanges();
        }

        public void DeleteCapacityCertificate(int id)
        {
            DeleteRecordWithId(Db.CapacityCertificatesRegister, id);
            Db.SaveChanges();
        }

        public void UndoDeleteCapacityCertificate(int id)
        {
            UndoDeleteRecordWithId(Db.CapacityCertificatesRegister, id);
            Db.SaveChanges();
        }

        public SimpleAuditDTO GetFishingCapacityCertificateSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.CapacityCertificatesRegister, id);
        }

        public List<FishingCapacityCertificateNomenclatureDTO> GetAllCapacityCertificateNomenclatures(int? userId = null)
        {
            DateTime now = DateTime.Now;

            var query = from certificate in Db.CapacityCertificatesRegister
                        join changeCert in Db.CapacityChangeHistoryCertificates on certificate.Id equals changeCert.CapacityCertificateId
                        join change in Db.CapacityChangeHistory on changeCert.CapacityChangeHistoryId equals change.Id
                        join person in Db.Persons on certificate.PersonId equals person.Id into per
                        from person in per.DefaultIfEmpty()
                        join legal in Db.Legals on certificate.LegalId equals legal.Id into leg
                        from legal in leg.DefaultIfEmpty()
                        where certificate.IsActive
                            && changeCert.IsActive
                            && change.RecordType == nameof(RecordTypesEnum.Register)
                        select new
                        {
                            certificate.Id,
                            certificate.CertificateNum,
                            certificate.PersonId,
                            certificate.LegalId,
                            certificate.GrossTonnage,
                            certificate.MainEnginePower,
                            certificate.CertificateValidTo,
                            Holder = legal != null ? legal.Name : $"{person.FirstName} {person.LastName}",
                            IsActive = certificate.IsActive && certificate.CertificateValidFrom <= now && certificate.CertificateValidTo > now
                        };

            if (userId.HasValue)
            {
                List<int> personIds = userService.GetPersonIdsByUserId(userId.Value);
                List<int> legalIds = userService.GetApprovedLegalIdsByUserId(userId.Value);

                query = from certificate in query
                        where (certificate.PersonId.HasValue && personIds.Contains(certificate.PersonId.Value))
                            || (certificate.LegalId.HasValue && legalIds.Contains(certificate.LegalId.Value))
                        select certificate;
            }

            List<FishingCapacityCertificateNomenclatureDTO> result = (from certificate in query
                                                                      orderby certificate.Id descending
                                                                      select new FishingCapacityCertificateNomenclatureDTO
                                                                      {
                                                                          Value = certificate.Id,
                                                                          DisplayName = certificate.CertificateNum.ToString(),
                                                                          Description = certificate.Holder,
                                                                          GrossTonnage = certificate.GrossTonnage,
                                                                          Power = certificate.MainEnginePower,
                                                                          ValidTo = certificate.CertificateValidTo,
                                                                          IsActive = certificate.IsActive
                                                                      }).ToList();

            return result;
        }

        public Task<byte[]> DownloadFishingCapacityCertificate(int certificateId)
        {
            return jasperReportExecutionService.GetShipCapacityLicenseRegister(certificateId);
        }

        public Stream DownloadFishingCapacityCertificateExcel(ExcelExporterRequestModel<FishingCapacityCertificatesFilters> request)
        {
            ExcelExporterData<FishingCapacityCertificateDTO> data = new ExcelExporterData<FishingCapacityCertificateDTO>
            {
                PrimaryKey = nameof(FishingCapacityCertificateDTO.Id),
                Query = GetAllCapacityCertificates(request.Filters),
                HeaderNames = request.HeaderNames
            };

            return excelExporterService.BuildExcelFile(request, data);
        }

        private IQueryable<FishingCapacityCertificateDTO> GetAllCapacityCertificates(bool showInactive)
        {
            DateTime now = DateTime.Now;

            IQueryable<FishingCapacityCertificateDTO> result = from cap in Db.CapacityCertificatesRegister
                                                               join changeCap in Db.CapacityChangeHistoryCertificates on cap.Id equals changeCap.CapacityCertificateId
                                                               join change in Db.CapacityChangeHistory on changeCap.CapacityChangeHistoryId equals change.Id
                                                               join appl in Db.Applications on change.ApplicationId equals appl.Id
                                                               join person in Db.Persons on cap.PersonId equals person.Id into per
                                                               from person in per.DefaultIfEmpty()
                                                               join legal in Db.Legals on cap.LegalId equals legal.Id into leg
                                                               from legal in leg.DefaultIfEmpty()
                                                               where change.RecordType == nameof(RecordTypesEnum.Register)
                                                                    && cap.IsActive == !showInactive
                                                               orderby cap.CertificateNum descending
                                                               select new FishingCapacityCertificateDTO
                                                               {
                                                                   Id = cap.Id,
                                                                   ApplicationId = change.ApplicationId.Value,
                                                                   CertificateNum = cap.CertificateNum.Value,
                                                                   CertificateValidFrom = cap.CertificateValidFrom,
                                                                   CertificateValidTo = cap.CertificateValidTo,
                                                                   GrossTonnage = cap.GrossTonnage,
                                                                   Power = cap.MainEnginePower,
                                                                   HolderNames = legal != null ? legal.Name : person.FirstName + " " + person.LastName,
                                                                   Invalid = cap.CertificateValidTo <= now,
                                                                   DeliveryId = appl.DeliveryId,
                                                                   IsActive = cap.IsActive
                                                               };
            return result;
        }

        private IQueryable<FishingCapacityCertificateDTO> GetParametersFilteredCapacityCertificates(FishingCapacityCertificatesFilters filters)
        {
            DateTime now = DateTime.Now;

            var query = from cap in Db.CapacityCertificatesRegister
                        join changeCap in Db.CapacityChangeHistoryCertificates on cap.Id equals changeCap.CapacityCertificateId
                        join change in Db.CapacityChangeHistory on changeCap.CapacityChangeHistoryId equals change.Id
                        join appl in Db.Applications on change.ApplicationId equals appl.Id
                        join person in Db.Persons on cap.PersonId equals person.Id into per
                        from person in per.DefaultIfEmpty()
                        join legal in Db.Legals on cap.LegalId equals legal.Id into leg
                        from legal in leg.DefaultIfEmpty()
                        where change.RecordType == nameof(RecordTypesEnum.Register)
                            && cap.IsActive == !filters.ShowInactiveRecords
                        orderby cap.CertificateNum descending
                        select new
                        {
                            cap.Id,
                            ApplicationId = change.ApplicationId.Value,
                            CertificateNum = cap.CertificateNum.Value,
                            cap.CertificateValidFrom,
                            cap.CertificateValidTo,
                            cap.GrossTonnage,
                            cap.MainEnginePower,
                            HolderNames = legal != null ? legal.Name : person.FirstName + " " + person.LastName,
                            HolderEgnEik = legal != null ? legal.Eik : person.EgnLnc,
                            cap.PersonId,
                            cap.LegalId,
                            Invalid = cap.CertificateValidTo <= now,
                            appl.DeliveryId,
                            cap.IsActive
                        };

            if (filters.CertificateId.HasValue)
            {
                query = query.Where(x => x.Id == filters.CertificateId.Value);
            }

            if (filters.CertificateNum.HasValue)
            {
                query = query.Where(x => x.CertificateNum == filters.CertificateNum);
            }

            if (filters.ValidFrom.HasValue)
            {
                query = query.Where(x => x.CertificateValidFrom >= filters.ValidFrom.Value);
            }

            if (filters.ValidTo.HasValue)
            {
                query = query.Where(x => x.CertificateValidTo <= filters.ValidTo.Value);
            }

            if (!string.IsNullOrEmpty(filters.HolderNames))
            {
                query = query.Where(x => x.HolderNames.ToLower().Contains(filters.HolderNames.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.HolderEgnEik))
            {
                query = query.Where(x => x.HolderEgnEik == filters.HolderEgnEik);
            }

            if (filters.GrossTonnageFrom.HasValue)
            {
                query = query.Where(x => x.GrossTonnage >= filters.GrossTonnageFrom.Value);
            }

            if (filters.GrossTonnageTo.HasValue)
            {
                query = query.Where(x => x.GrossTonnage <= filters.GrossTonnageTo.Value);
            }

            if (filters.PowerFrom.HasValue)
            {
                query = query.Where(x => x.MainEnginePower >= filters.PowerFrom.Value);
            }

            if (filters.PowerTo.HasValue)
            {
                query = query.Where(x => x.MainEnginePower <= filters.PowerTo.Value);
            }

            if (filters.PersonId.HasValue)
            {
                query = query.Where(x => x.PersonId == filters.PersonId);
            }

            if (filters.PersonId.HasValue)
            {
                query = query.Where(x => x.LegalId == filters.LegalId);
            }

            if (filters.IsCertificateActive.HasValue)
            {
                query = query.Where(x => x.Invalid == !filters.IsCertificateActive.Value);
            }

            IQueryable<FishingCapacityCertificateDTO> result = from cap in query
                                                               select new FishingCapacityCertificateDTO
                                                               {
                                                                   Id = cap.Id,
                                                                   ApplicationId = cap.ApplicationId,
                                                                   CertificateNum = cap.CertificateNum,
                                                                   CertificateValidFrom = cap.CertificateValidFrom,
                                                                   CertificateValidTo = cap.CertificateValidTo,
                                                                   GrossTonnage = cap.GrossTonnage,
                                                                   Power = cap.MainEnginePower,
                                                                   HolderNames = cap.HolderNames,
                                                                   Invalid = cap.Invalid,
                                                                   DeliveryId = cap.DeliveryId,
                                                                   IsActive = cap.IsActive
                                                               };
            return result;
        }

        private IQueryable<FishingCapacityCertificateDTO> GetFreeTextFilteredCapacityCertificates(string text, bool showInactive)
        {
            DateTime now = DateTime.Now;

            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            IQueryable<FishingCapacityCertificateDTO> result = from cap in Db.CapacityCertificatesRegister
                                                               join changeCap in Db.CapacityChangeHistoryCertificates on cap.Id equals changeCap.CapacityCertificateId
                                                               join change in Db.CapacityChangeHistory on changeCap.CapacityChangeHistoryId equals change.Id
                                                               join appl in Db.Applications on change.ApplicationId equals appl.Id
                                                               join person in Db.Persons on cap.PersonId equals person.Id into per
                                                               from person in per.DefaultIfEmpty()
                                                               join legal in Db.Legals on cap.LegalId equals legal.Id into leg
                                                               from legal in leg.DefaultIfEmpty()
                                                               where change.RecordType == nameof(RecordTypesEnum.Register)
                                                                    && cap.IsActive == !showInactive
                                                                    && (cap.CertificateNum.ToString().Contains(text)
                                                                        || cap.GrossTonnage.ToString().Contains(text)
                                                                        || cap.MainEnginePower.ToString().Contains(text)
                                                                        || (legal != null && legal.Name.ToLower().Contains(text))
                                                                        || (person != null && (person.FirstName + " " + person.LastName).ToLower().Contains(text))
                                                                        || (searchDate.HasValue && cap.CertificateValidFrom == searchDate.Value))
                                                               orderby cap.CertificateNum descending
                                                               select new FishingCapacityCertificateDTO
                                                               {
                                                                   Id = cap.Id,
                                                                   ApplicationId = change.ApplicationId.Value,
                                                                   CertificateNum = cap.CertificateNum.Value,
                                                                   CertificateValidFrom = cap.CertificateValidFrom,
                                                                   CertificateValidTo = cap.CertificateValidTo,
                                                                   GrossTonnage = cap.GrossTonnage,
                                                                   Power = cap.MainEnginePower,
                                                                   HolderNames = legal != null ? legal.Name : person.FirstName + " " + person.LastName,
                                                                   Invalid = cap.CertificateValidTo <= now,
                                                                   DeliveryId = appl.DeliveryId,
                                                                   IsActive = cap.IsActive
                                                               };
            return result;
        }

        private Dictionary<int, CapacityCertificateHistoryApplDTO> GetCapacityCertificateHistoryCreatedFrom(List<int> certificateIds)
        {
            // заявление, от които са създадени удостоверенията
            var createdFrom = (from change in Db.CapacityChangeHistory
                               join changeCert in Db.CapacityChangeHistoryCertificates on change.Id equals changeCert.CapacityChangeHistoryId
                               join shipCapacity in Db.ShipCapacityRegister on change.ShipCapacityId equals shipCapacity.Id into shipCap
                               from shipCapacity in shipCap.DefaultIfEmpty()
                               join transferCap in Db.CapacityCertificatesRegister on change.CapacityCertificateTransferId equals transferCap.Id into tranCap
                               from transferCap in tranCap.DefaultIfEmpty()
                               where change.RecordType == nameof(RecordTypesEnum.Register)
                                    && certificateIds.Contains(changeCert.CapacityCertificateId)
                                    && changeCert.IsActive
                               select new
                               {
                                   CertificateId = changeCert.CapacityCertificateId,
                                   CreatedFromAppl = new CapacityCertificateHistoryApplDTO
                                   {
                                       ApplicationId = change.ApplicationId.Value,
                                       ApplicationDate = change.DateOfChange,
                                       // отписан кораб или кораб, чийто капацитет е намален
                                       ShipId = shipCapacity != null ? shipCapacity.ShipId : null,
                                       // удостоверение, от което е прехвърлен капацитет
                                       TransferredCapacityCertificate = change.TypeOfChange != nameof(FishingCapacityChangeTypeEnum.Duplicate)
                                                                             && transferCap != null ? transferCap.CertificateNum.ToString() : null,
                                       // удостоверение, на което е издадено дубликат
                                       DuplicateCapacityCertificateId = change.TypeOfChange == nameof(FishingCapacityChangeTypeEnum.Duplicate) ? transferCap.Id : null,
                                       DuplicateCapacityCertificate = change.TypeOfChange == nameof(FishingCapacityChangeTypeEnum.Duplicate)
                                                                             && transferCap != null ? transferCap.CertificateNum.ToString() : null
                                   }
                               }).ToDictionary(x => x.CertificateId, y => y.CreatedFromAppl);

            SetCapacityCertificateHistoryPageCodes(createdFrom);
            return createdFrom;
        }

        private Dictionary<int, CapacityCertificateHistoryApplDTO> GetCapacityCertificateHistoryUsedInIncrease(List<int> certificateIds)
        {
            // удостоверения, използвани при регистриране на кораб или увеличаване на капацитет
            var usedInIncrease = (from change in Db.CapacityChangeHistory
                                  join acquired in Db.AcquiredCapacityRegister on change.AcquiredFishingCapacityId equals acquired.Id
                                  join acquiredCert in Db.AcquiredCapacityCertificates on acquired.Id equals acquiredCert.AcquiredCapacityId
                                  join shipCapacity in Db.ShipCapacityRegister on change.ShipCapacityId equals shipCapacity.Id
                                  where change.RecordType == nameof(RecordTypesEnum.Register)
                                        && change.TypeOfChange != nameof(FishingCapacityChangeTypeEnum.Duplicate)
                                        && certificateIds.Contains(acquiredCert.CapacityCertificateId)
                                        && acquiredCert.IsActive
                                  select new
                                  {
                                      CertificateId = acquiredCert.CapacityCertificateId,
                                      UsedInAppl = new CapacityCertificateHistoryApplDTO
                                      {
                                          ApplicationId = change.ApplicationId.Value,
                                          ApplicationDate = change.DateOfChange,
                                          // регистриран кораб или кораб, чийто капацитет е увеличен
                                          ShipId = shipCapacity.ShipId
                                      }
                                  }).ToDictionary(x => x.CertificateId, y => y.UsedInAppl);

            SetCapacityCertificateHistoryPageCodes(usedInIncrease);
            return usedInIncrease;
        }

        private Dictionary<int, CapacityCertificateHistoryApplDTO> GetCapacityCertificateHistoryUsedInTransfer(List<int> certificateIds)
        {
            // удостоверения, използвани за прехвърляне на капацитет
            var usedInTransfer = (from change in Db.CapacityChangeHistory
                                  join transferCap in Db.CapacityCertificatesRegister on change.CapacityCertificateTransferId equals transferCap.Id
                                  where change.RecordType == nameof(RecordTypesEnum.Register)
                                        && change.TypeOfChange != nameof(FishingCapacityChangeTypeEnum.Duplicate)
                                        && certificateIds.Contains(transferCap.Id)
                                  select new
                                  {
                                      CertificateId = change.CapacityCertificateTransferId.Value,
                                      UsedInAppl = new CapacityCertificateHistoryApplDTO
                                      {
                                          ApplicationId = change.ApplicationId.Value,
                                          ApplicationDate = change.DateOfChange
                                      }
                                  }).ToDictionary(x => x.CertificateId, y => y.UsedInAppl);

            SetCapacityCertificateHistoryPageCodes(usedInTransfer);
            return usedInTransfer;
        }

        private Dictionary<int, CapacityCertificateHistoryApplDTO> GetCapacityCertificateHistoryUsedInDuplicate(List<int> certificateIds)
        {
            // удостоверения, използвани за издаване на дубликат
            var usedInDuplicate = (from change in Db.CapacityChangeHistory
                                   join transferCap in Db.CapacityCertificatesRegister on change.CapacityCertificateTransferId equals transferCap.Id
                                   join changeCert in Db.CapacityChangeHistoryCertificates on change.Id equals changeCert.CapacityChangeHistoryId
                                   join cert in Db.CapacityCertificatesRegister on changeCert.CapacityCertificateId equals cert.Id
                                   where change.RecordType == nameof(RecordTypesEnum.Register)
                                         && change.TypeOfChange == nameof(FishingCapacityChangeTypeEnum.Duplicate)
                                         && certificateIds.Contains(transferCap.Id)
                                   select new
                                   {
                                       CertificateId = change.CapacityCertificateTransferId.Value,
                                       UsedInAppl = new CapacityCertificateHistoryApplDTO
                                       {
                                           ApplicationId = change.ApplicationId.Value,
                                           ApplicationDate = change.DateOfChange,
                                           DuplicateCapacityCertificateId = cert.Id,
                                           DuplicateCapacityCertificate = cert.CertificateNum.ToString()
                                       }
                                   }).ToDictionary(x => x.CertificateId, y => y.UsedInAppl);

            SetCapacityCertificateHistoryPageCodes(usedInDuplicate);
            return usedInDuplicate;
        }

        private void SetCapacityCertificateHistoryPageCodes(Dictionary<int, CapacityCertificateHistoryApplDTO> history)
        {
            List<int> applicationIds = history.Select(x => x.Value.ApplicationId).ToList();

            var pageCodes = (from application in Db.Applications
                             join applType in Db.NapplicationTypes on application.ApplicationTypeId equals applType.Id
                             where applicationIds.Contains(application.Id)
                             select new
                             {
                                 application.Id,
                                 Data = new
                                 {
                                     PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                     Reason = applType.Name
                                 }
                             }).ToDictionary(x => x.Id, y => y.Data);

            foreach (KeyValuePair<int, CapacityCertificateHistoryApplDTO> cf in history)
            {
                history[cf.Key].PageCode = pageCodes[cf.Value.ApplicationId].PageCode;
                history[cf.Key].Reason = pageCodes[cf.Value.ApplicationId].Reason;
            }
        }

        private ILookup<int, CapacityCertificateHistoryTransferredToDTO> GetCapacityCertificateHistoryRemainderTransferredTo(List<int> applicationIds)
        {
            DateTime now = DateTime.Now;

            var transferredTo = (from change in Db.CapacityChangeHistory
                                 join changeCert in Db.CapacityChangeHistoryCertificates on change.Id equals changeCert.CapacityChangeHistoryId
                                 join cap in Db.CapacityCertificatesRegister on changeCert.CapacityCertificateId equals cap.Id
                                 join person in Db.Persons on cap.PersonId equals person.Id into per
                                 from person in per.DefaultIfEmpty()
                                 join legal in Db.Legals on cap.LegalId equals legal.Id into leg
                                 from legal in leg.DefaultIfEmpty()
                                 where change.RecordType == nameof(RecordTypesEnum.Register)
                                    && change.TypeOfChange == nameof(FishingCapacityChangeTypeEnum.Increase)
                                    && applicationIds.Contains(change.ApplicationId.Value)
                                    && cap.IsActive
                                 select new
                                 {
                                     ApplicationId = change.ApplicationId.Value,
                                     TransferredTo = new CapacityCertificateHistoryTransferredToDTO
                                     {
                                         Id = cap.Id,
                                         CertificateNum = cap.CertificateNum.ToString(),
                                         Holder = legal != null ? legal.Name : $"{person.FirstName} {person.LastName}",
                                         Tonnage = cap.GrossTonnage,
                                         Power = cap.MainEnginePower,
                                         ValidFrom = cap.CertificateValidFrom,
                                         ValidTo = cap.CertificateValidTo,
                                         Invalid = cap.CertificateValidTo <= now,
                                         IsActive = true
                                     }
                                 }).ToLookup(x => x.ApplicationId, y => y.TransferredTo);

            return transferredTo;
        }

        private ILookup<int, CapacityCertificateHistoryTransferredToDTO> GetCapacityCertificateHistoryTransferredTo(List<int> certificateIds)
        {
            DateTime now = DateTime.Now;

            // удостоверения, към които е прехвърлен капацитет
            var transferredTo = (from change in Db.CapacityChangeHistory
                                 join changeCert in Db.CapacityChangeHistoryCertificates on change.Id equals changeCert.CapacityChangeHistoryId
                                 join cap in Db.CapacityCertificatesRegister on changeCert.CapacityCertificateId equals cap.Id
                                 join person in Db.Persons on cap.PersonId equals person.Id into per
                                 from person in per.DefaultIfEmpty()
                                 join legal in Db.Legals on cap.LegalId equals legal.Id into leg
                                 from legal in leg.DefaultIfEmpty()
                                 where change.RecordType == nameof(RecordTypesEnum.Register)
                                    && change.TypeOfChange == nameof(FishingCapacityChangeTypeEnum.Transfer)
                                    && certificateIds.Contains(change.CapacityCertificateTransferId.Value)
                                    && cap.IsActive
                                 select new
                                 {
                                     CertificateId = change.CapacityCertificateTransferId.Value,
                                     TransferredTo = new CapacityCertificateHistoryTransferredToDTO
                                     {
                                         Id = cap.Id,
                                         CertificateNum = cap.CertificateNum.ToString(),
                                         Holder = legal != null ? legal.Name : $"{person.FirstName} {person.LastName}",
                                         Tonnage = cap.GrossTonnage,
                                         Power = cap.MainEnginePower,
                                         ValidFrom = cap.CertificateValidFrom,
                                         ValidTo = cap.CertificateValidTo,
                                         Invalid = cap.CertificateValidTo <= now,
                                         IsActive = true
                                     }
                                 }).ToLookup(x => x.CertificateId, y => y.TransferredTo);

            return transferredTo;
        }

        private void AddCapacityCertificate(CapacityChangeHistory change,
                                            Person person,
                                            Legal legal,
                                            decimal tonnage,
                                            decimal power,
                                            DateTime validFrom,
                                            DateTime validTo,
                                            bool isActive)
        {
            CapacityCertificatesRegister certificate = new CapacityCertificatesRegister
            {
                RecordType = change.RecordType,
                Person = person,
                Legal = legal,
                GrossTonnage = tonnage,
                MainEnginePower = power,
                CertificateValidFrom = validFrom,
                CertificateValidTo = validTo
            };

            CapacityChangeHistoryCertificate changeCertificate = new CapacityChangeHistoryCertificate
            {
                CapacityCertificate = certificate,
                CapacityChangeHistory = change,
                IsActive = isActive
            };

            Db.CapacityChangeHistoryCertificates.Add(changeCertificate);
            Db.SaveChanges();
        }

        private void AddCapacityCertificateForSubmittedFor(int applicationId,
                                                           CapacityChangeHistory change,
                                                           decimal tonnage,
                                                           decimal power,
                                                           DateTime validFrom,
                                                           DateTime validTo)
        {
            var submittedFor = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select new
                                {
                                    appl.SubmittedForPerson,
                                    appl.SubmittedForLegal
                                }).First();

            AddCapacityCertificate(change,
                                   submittedFor.SubmittedForPerson,
                                   submittedFor.SubmittedForLegal,
                                   tonnage,
                                   power,
                                   validFrom,
                                   validTo,
                                   true);
        }

        private void EditCapacityCertificateForSubmittedFor(CapacityChangeHistory change,
                                                            decimal tonnage,
                                                            decimal power,
                                                            DateTime validFrom,
                                                            DateTime validTo)
        {
            // инвалидираме всички записи, тъй като трябва да бъде издадено само едно удостоверение на получателя
            List<CapacityChangeHistoryCertificate> capChangeHistoryCerts = (from cchc in Db.CapacityChangeHistoryCertificates
                                                                            where cchc.CapacityChangeHistoryId == change.Id
                                                                            select cchc).ToList();

            foreach (CapacityChangeHistoryCertificate changeCert in capChangeHistoryCerts)
            {
                changeCert.IsActive = false;
            }

            List<int> dbCertificateIds = capChangeHistoryCerts.Select(x => x.CapacityCertificateId).ToList();

            // проверяваме дали вече е добавено удостоверение към получателя
            // ако е добавено, редактираме тонажа, мощността и валидността, в противен случай добавяме ново
            var submittedFor = (from appl in Db.Applications
                                where appl.Id == change.ApplicationId
                                select new
                                {
                                    appl.SubmittedForLegal,
                                    appl.SubmittedForPerson
                                }).First();

            int? submittedForCertId;

            if (submittedFor.SubmittedForPerson != null)
            {
                submittedForCertId = (from cert in Db.CapacityCertificatesRegister
                                      where dbCertificateIds.Contains(cert.Id)
                                         && cert.PersonId.HasValue && cert.PersonId.Value == submittedFor.SubmittedForPerson.Id
                                      select cert.Id).FirstOrDefault();
            }
            else
            {
                submittedForCertId = (from cert in Db.CapacityCertificatesRegister
                                      where dbCertificateIds.Contains(cert.Id)
                                             && cert.LegalId.HasValue && cert.LegalId.Value == submittedFor.SubmittedForLegal.Id
                                      select cert.Id).FirstOrDefault();
            }

            if (submittedForCertId.HasValue)
            {
                CapacityChangeHistoryCertificate changeCert = capChangeHistoryCerts.Where(x => x.CapacityCertificateId == submittedForCertId).First();
                changeCert.IsActive = true;

                CapacityCertificatesRegister cert = (from capCert in Db.CapacityCertificatesRegister
                                                     where capCert.Id == submittedForCertId
                                                     select capCert).First();

                cert.GrossTonnage = tonnage;
                cert.MainEnginePower = power;
                cert.CertificateValidFrom = validFrom;
                cert.CertificateValidTo = validTo;

                Db.SaveChanges();
            }
            else
            {
                AddCapacityCertificate(change,
                                       submittedFor.SubmittedForPerson,
                                       submittedFor.SubmittedForLegal,
                                       tonnage,
                                       power,
                                       validFrom,
                                       validTo,
                                       true);
            }
        }

        private void AddCapacityCertificateForHolders(RecordTypesEnum recordType,
                                                      CapacityChangeHistory change,
                                                      List<FishingCapacityHolderDTO> holders,
                                                      DateTime validFrom,
                                                      DateTime validTo)
        {
            using TransactionScope scope = new TransactionScope();

            foreach (FishingCapacityHolderDTO holder in holders)
            {
                Person person = null;
                Legal legal = null;

                if (holder.IsHolderPerson.Value)
                {
                    person = Db.AddOrEditPerson(holder.Person, holder.Addresses);
                }
                else
                {
                    legal = Db.AddOrEditLegal(new ApplicationRegisterDataDTO
                    {
                        ApplicationId = change.ApplicationId,
                        RecordType = recordType
                    }, holder.Legal, holder.Addresses);
                }

                AddCapacityCertificate(change,
                                       person,
                                       legal,
                                       holder.TransferredTonnage.Value,
                                       holder.TransferredPower.Value,
                                       validFrom,
                                       validTo,
                                       holder.IsActive.Value);

                Db.SaveChanges();
            }

            scope.Complete();
        }

        private void EditCapacityCertificatesForHolders(int applicationId,
                                                        CapacityChangeHistory change,
                                                        List<FishingCapacityHolderDTO> holders,
                                                        DateTime validFrom,
                                                        DateTime validTo)
        {
            using TransactionScope scope = new TransactionScope();

            List<CapacityChangeHistoryCertificate> capChangeHistoryCerts = (from cchc in Db.CapacityChangeHistoryCertificates
                                                                            where cchc.CapacityChangeHistoryId == change.Id
                                                                            select cchc).ToList();

            foreach (CapacityChangeHistoryCertificate changeCert in capChangeHistoryCerts)
            {
                changeCert.IsActive = false;
            }

            List<int> dbCertificateIds = capChangeHistoryCerts.Select(x => x.CapacityCertificateId).ToList();

            List<CapacityCertificatesRegister> dbCertificates = (from cert in Db.CapacityCertificatesRegister
                                                                 where dbCertificateIds.Contains(cert.Id)
                                                                 select cert).ToList();

            foreach (FishingCapacityHolderDTO holder in holders)
            {
                // лицето вече има удостоверение в базата, редактираме данните му
                if (holder.Id != null)
                {
                    CapacityChangeHistoryCertificate capChangeHistoryCert = capChangeHistoryCerts.Where(x => x.Id == holder.Id.Value).First();
                    CapacityCertificatesRegister certificate = dbCertificates.Where(x => x.Id == capChangeHistoryCert.CapacityCertificateId).First();

                    if (holder.IsHolderPerson.Value)
                    {
                        certificate.LegalId = null;
                        certificate.Person = Db.AddOrEditPerson(holder.Person, holder.Addresses, certificate.PersonId);
                    }
                    else
                    {
                        certificate.PersonId = null;
                        certificate.Legal = Db.AddOrEditLegal(new ApplicationRegisterDataDTO
                        {
                            ApplicationId = applicationId,
                            RecordType = RecordTypesEnum.Application
                        }, holder.Legal, holder.Addresses, certificate.LegalId);
                    }

                    certificate.GrossTonnage = holder.TransferredTonnage.Value;
                    certificate.MainEnginePower = holder.TransferredPower.Value;

                    capChangeHistoryCert.IsActive = holder.IsActive.Value;
                }
                // лицето няма удостоверение в базата, добавяме ново
                else
                {
                    Person person = null;
                    Legal legal = null;

                    if (holder.IsHolderPerson.Value)
                    {
                        person = Db.AddOrEditPerson(holder.Person, holder.Addresses);
                    }
                    else
                    {
                        legal = Db.AddOrEditLegal(new ApplicationRegisterDataDTO
                        {
                            ApplicationId = applicationId,
                            RecordType = RecordTypesEnum.Application
                        }, holder.Legal, holder.Addresses);
                    }

                    AddCapacityCertificate(change,
                                           person,
                                           legal,
                                           holder.TransferredTonnage.Value,
                                           holder.TransferredPower.Value,
                                           validFrom,
                                           validTo,
                                           true);
                }

                Db.SaveChanges();
            }
            scope.Complete();
        }

        private void EditCapacityCertificatesForHoldersRegix(int applicationId, List<FishingCapacityHolderRegixDataDTO> holders)
        {
            using TransactionScope scope = new TransactionScope();

            int changeId = (from ch in Db.CapacityChangeHistory
                            where ch.ApplicationId == applicationId
                            select ch.Id).First();

            List<CapacityChangeHistoryCertificate> dbCapChangeHistoryCerts = (from cchc in Db.CapacityChangeHistoryCertificates
                                                                              where cchc.CapacityChangeHistoryId == changeId
                                                                              select cchc).ToList();

            List<int> dbCertificateIds = dbCapChangeHistoryCerts.Select(x => x.CapacityCertificateId).ToList();

            List<CapacityCertificatesRegister> dbCertificates = (from cert in Db.CapacityCertificatesRegister
                                                                 where dbCertificateIds.Contains(cert.Id)
                                                                 select cert).ToList();

            foreach (FishingCapacityHolderRegixDataDTO holder in holders)
            {
                CapacityChangeHistoryCertificate capChangeHistoryCert = dbCapChangeHistoryCerts.Where(x => x.Id == holder.Id.Value).First();
                CapacityCertificatesRegister certificate = dbCertificates.Where(x => x.Id == capChangeHistoryCert.CapacityCertificateId).First();

                if (holder.IsHolderPerson.Value)
                {
                    certificate.LegalId = null;
                    certificate.Person = Db.AddOrEditPerson(holder.Person, holder.Addresses, certificate.PersonId);
                }
                else
                {
                    certificate.PersonId = null;
                    certificate.Legal = Db.AddOrEditLegal(new ApplicationRegisterDataDTO
                    {
                        ApplicationId = applicationId,
                        RecordType = RecordTypesEnum.Application
                    }, holder.Legal, holder.Addresses, certificate.LegalId);
                }

                capChangeHistoryCert.IsActive = holder.IsActive.Value;

                Db.SaveChanges();
            }

            Db.SaveChanges();

            scope.Complete();
        }
    }
}
