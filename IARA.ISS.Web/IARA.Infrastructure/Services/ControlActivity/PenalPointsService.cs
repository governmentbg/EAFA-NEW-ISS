using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.DTOModels.ControlActivity.PenalPoints;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.ControlActivity;
using IARA.Interfaces.Legals;

namespace IARA.Infrastructure.Services.ControlActivity
{
    public class PenalPointsService : Service, IPenalPointsService
    {
        private readonly IPersonService personService;
        private readonly ILegalService legalService;
        public PenalPointsService(IARADbContext db,
                                    IPersonService personService,
                                    ILegalService legalService)
            : base(db)
        {
            this.personService = personService;
            this.legalService = legalService;
        }

        public IQueryable<PenalPointsDTO> GetAllPenalPoints(PenalPointsFilters filters)
        {
            IQueryable<PenalPointsDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllPenalPoints(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredPenalPoints(filters)
                    : GetFreeTextFilteredPenalPoints(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            return result;
        }

        public PenalPointsEditDTO GetPenalPoints(int id)
        {
            var query = (from points in this.Db.PenalPointsRegister
                         where points.Id == id
                         select new
                         {
                             points.Id,
                             points.PenalRegisterId,
                             points.PointsType,
                             points.IsIncreasePoints,
                             points.DecreeNum,
                             points.PointsAmount,
                             points.IssueDate,
                             points.DeliveryDate,
                             points.EffectiveDate,
                             points.PermitId,
                             points.PointsOwnerPersonId,
                             points.PointsOwnerLegalId,
                             points.ShipId,
                             points.PermitLicenseId,
                             points.QualifiedFisherId,
                             points.Comments,
                             points.Issuer,
                             points.IsPermitOwner
                         }).First();

            PenalPointsEditDTO result = new PenalPointsEditDTO
            {
                Id = query.Id,
                DecreeId = query.PenalRegisterId,
                PointsType = Enum.Parse<PointsTypeEnum>(query.PointsType),
                IsIncreasePoints = query.IsIncreasePoints,
                DecreeNum = query.DecreeNum,
                PointsAmount = query.PointsAmount,
                IssueDate = query.IssueDate,
                DeliveryDate = query.DeliveryDate,
                EffectiveDate = query.EffectiveDate,
                PermitId = query.PermitId,
                PermitLicenseId = query.PermitLicenseId,
                QualifiedFisherId = query.QualifiedFisherId,
                PermitOwnerPersonId = query.PointsOwnerPersonId,
                PermitOwnerLegalId = query.PointsOwnerLegalId,
                ShipId = query.ShipId,
                Comments = query.Comments,
                Issuer = query.Issuer,
                IsPermitOwner = query.IsPermitOwner
            };

            if(query.IsPermitOwner.HasValue && query.IsPermitOwner == false)
            {
                result.PersonOwner = this.personService.GetRegixPersonData(query.PointsOwnerPersonId.Value);
            }

            result.AppealStatuses = GetPenalPointsComplaints(result.Id.Value);

            return result;
        }

        public int AddPenalPoints(PenalPointsEditDTO points)
        {
            using TransactionScope scope = new TransactionScope();

            PenalPointsRegister entry = new PenalPointsRegister
            {
                PenalRegisterId = points.DecreeId.Value,
                PointsType = points.PointsType.ToString(),
                IsIncreasePoints = points.IsIncreasePoints.Value,
                DecreeNum = points.DecreeNum,
                PointsAmount = points.PointsAmount.Value,
                IssueDate = points.IssueDate.Value,
                DeliveryDate = points.DeliveryDate,
                EffectiveDate = points.EffectiveDate,
                PermitLicenseId = points.PermitLicenseId,
                QualifiedFisherId = points.QualifiedFisherId,
                ShipId = points.ShipId.Value,
                Comments = points.Comments,
                Issuer = points.Issuer,
                IsPermitOwner = points.IsPermitOwner
            };

            if (points.PermitId.HasValue)
            {
                entry.PermitId = points.PermitId.Value;

                if (points.IsPermitOwner.HasValue && points.IsPermitOwner == true)
                {
                    entry.PointsOwnerPersonId = points.PermitOwnerPersonId;
                    entry.PointsOwnerLegalId = points.PermitOwnerLegalId;
                }
            }
            else
            {
                entry.PermitId = this.GetPermitLicensesPermitId(points.PermitLicenseId.Value);
                if (points.IsPermitOwner.HasValue && points.IsPermitOwner == true)
                {
                    entry.PointsOwnerPersonId = (from fisher in Db.FishermenRegisters
                                                 where fisher.Id == points.QualifiedFisherId
                                                 select fisher.PersonId).FirstOrDefault();
                }
            }

            if(points.IsPermitOwner.HasValue && points.IsPermitOwner == false)
            {
                entry.PointsOwnerPerson = Db.AddOrEditPerson(points.PersonOwner);

                this.Db.SaveChanges();
            }

            Db.PenalPointsRegister.Add(entry);

            Db.SaveChanges();

            scope.Complete();
            return entry.Id;
        }

        public void EditPenalPoints(PenalPointsEditDTO points)
        {
            using TransactionScope scope = new TransactionScope();

            PenalPointsRegister entry = (from penalPoints in Db.PenalPointsRegister
                                         where penalPoints.Id == points.Id.Value
                                         select penalPoints).First();

            entry.PenalRegisterId = points.DecreeId.Value;
            entry.PointsType = points.PointsType.ToString();
            entry.IsIncreasePoints = points.IsIncreasePoints.Value;
            entry.DecreeNum = points.DecreeNum;
            entry.PointsAmount = points.PointsAmount.Value;
            entry.IssueDate = points.IssueDate.Value;
            entry.DeliveryDate = points.DeliveryDate;
            entry.EffectiveDate = points.EffectiveDate;
            entry.ShipId = points.ShipId.Value;
            entry.Comments = points.Comments;
            entry.QualifiedFisherId = points.QualifiedFisherId;
            entry.PointsAmount = points.PointsAmount.Value;
            entry.PermitLicenseId = points.PermitLicenseId;
            entry.Issuer = points.Issuer;

            if (points.PermitId.HasValue)
            {
                entry.PermitId = points.PermitId.Value;

                if (points.IsPermitOwner.HasValue && points.IsPermitOwner == true)
                {
                    entry.PointsOwnerPersonId = points.PermitOwnerPersonId;
                    entry.PointsOwnerLegalId = points.PermitOwnerLegalId;
                }
            }
            else
            {
                entry.PermitId = this.GetPermitLicensesPermitId(points.PermitLicenseId.Value);
                if (points.IsPermitOwner.HasValue && points.IsPermitOwner == true)
                {
                    entry.PointsOwnerPersonId = (from fisher in Db.FishermenRegisters
                                                 where fisher.Id == points.QualifiedFisherId
                                                 select fisher.PersonId).FirstOrDefault();
                }
            }

            if (points.IsPermitOwner.HasValue && points.IsPermitOwner == false)
            {
                entry.PointsOwnerPerson = Db.AddOrEditPerson(points.PersonOwner, null, entry.PointsOwnerPersonId);
                this.Db.SaveChanges();
            }

            EditPenalPointsComplaints(entry, points.AppealStatuses);

            Db.SaveChanges();

            scope.Complete();
        }

        public void DeletePenalPoints(int id)
        {
            this.DeleteRecordWithId(this.Db.PenalPointsRegister, id);
            this.Db.SaveChanges();
        }

        public void UndoDeletePenalPoints(int id)
        {
            this.UndoDeleteRecordWithId(this.Db.PenalPointsRegister, id);
            this.Db.SaveChanges();
        }

        public PenalPointsAuanDecreeDataDTO GetPenalPointsAuanDecreeData(int decreeId)
        {
            var query = (from decree in Db.PenalDecreesRegisters
                         join auan in Db.AuanRegister on decree.AuanRegisterId equals auan.Id
                         join inspection in this.Db.InspectionsRegister on auan.InspectionId equals inspection.Id
                         where decree.Id == decreeId
                         select new
                         {
                             auan.AuanNum,
                             auan.DraftDate,
                             decree.DecreeNum,
                             decree.IssueDate,
                             decree.EffectiveDate,
                             auan.InspectedLegalId,
                             auan.InspectedPersonId,
                             auan.InspectedPersonWorkPlace,
                             auan.InspectedPersonWorkPosition,
                             inspection.TerritoryUnitId
                         }).First();

            PenalPointsAuanDecreeDataDTO data = new PenalPointsAuanDecreeDataDTO
            {
                TerritoryUnitId = query.TerritoryUnitId,
                AuanNum = query.AuanNum,
                AuanDate = query.DraftDate,
                DecreeNum = query.DecreeNum,
                DecreeIssueDate = query.IssueDate,
                DecreeEffectiveDate = query.EffectiveDate
            };

            if (query.InspectedPersonId.HasValue)
            {
                data.InspectedEntity = new AuanInspectedEntityDTO
                {
                    IsUnregisteredPerson = false,
                    IsPerson = true,
                    PersonWorkPlace = query.InspectedPersonWorkPlace,
                    PersonWorkPosition = query.InspectedPersonWorkPosition
                };

                data.InspectedEntity.Person = this.personService.GetRegixPersonData(query.InspectedPersonId.Value);
                data.InspectedEntity.Addresses = this.personService.GetAddressRegistrations(query.InspectedPersonId.Value);
            }
            else if (query.InspectedLegalId.HasValue)
            {
                data.InspectedEntity = new AuanInspectedEntityDTO
                {
                    IsUnregisteredPerson = false,
                    IsPerson = false
                };

                data.InspectedEntity.Legal = this.legalService.GetRegixLegalData(query.InspectedLegalId.Value);
                data.InspectedEntity.Addresses = this.legalService.GetAddressRegistrations(query.InspectedLegalId.Value);
            }

            return data;
        }
        public List<PenalPointsOrderDTO> GetPermitOrders(int ownerId, bool isFisher, bool isOwnerPerson)
        {
            List<PenalPointsOrderDTO> orders = (from points in Db.PenalPointsRegister
                                                where points.IsActive 
                                                        && points.IsPermitOwner != false
                                                        && ((!isFisher && isOwnerPerson && points.PointsOwnerPersonId == ownerId)
                                                        || (!isFisher && !isOwnerPerson && points.PointsOwnerLegalId == ownerId)
                                                        || (isFisher && points.QualifiedFisherId == ownerId))
                                                select new PenalPointsOrderDTO
                                                {
                                                    Id = points.Id,
                                                    IsIncreasePoints = points.IsIncreasePoints.Value,
                                                    DecreeNum = points.DecreeNum,
                                                    IssueDate = points.IssueDate,
                                                    DeliveryDate = points.DeliveryDate,
                                                    EffectiveDate = points.EffectiveDate,
                                                    PointsAmount = points.PointsAmount
                                                }).ToList();

            return orders;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = this.GetSimpleEntityAuditValues(this.Db.PenalPointsRegister, id);
            return audit;
        }

        public SimpleAuditDTO GetPenalPointsStatusSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.PenalPointComplaintStatuses, id);
            return audit;
        }

        private IQueryable<PenalPointsDTO> GetAllPenalPoints(bool showInactive)
        {
            IQueryable<PenalPointsDTO> result = from points in Db.PenalPointsRegister
                                                join decree in Db.PenalDecreesRegisters on points.PenalRegisterId equals decree.Id
                                                join permit in Db.CommercialFishingPermitRegisters on points.PermitId equals permit.Id
                                                join pointsOwnerPerson in this.Db.Persons on points.PointsOwnerPersonId equals pointsOwnerPerson.Id into ownerPer
                                                from pointsOwnerPerson in ownerPer.DefaultIfEmpty()
                                                join pointsOwnerLegal in this.Db.Legals on points.PointsOwnerLegalId equals pointsOwnerLegal.Id into ownerLeg
                                                from pointsOwnerLegal in ownerLeg.DefaultIfEmpty()
                                                join ship in Db.ShipsRegister on points.ShipId equals ship.Id into shipReg
                                                from ship in shipReg.DefaultIfEmpty()
                                                where points.IsActive == !showInactive
                                                orderby points.IssueDate descending
                                                select new PenalPointsDTO
                                                {
                                                    Id = points.Id,
                                                    PenalDecreeId = points.PenalRegisterId,
                                                    DecreeNum = points.DecreeNum,
                                                    PenalDecreeNum = decree.DecreeNum,
                                                    PointsType = Enum.Parse<PointsTypeEnum>(points.PointsType),
                                                    Name = pointsOwnerPerson != null
                                                                ? pointsOwnerPerson.FirstName + " " + pointsOwnerPerson.LastName
                                                                : pointsOwnerLegal.Name,
                                                    PointsAmount = points.PointsAmount,
                                                    Ship = ship.Name,
                                                    IssueDate = points.IssueDate,
                                                    IsIncreasePoints = points.IsIncreasePoints,
                                                    IsActive = points.IsActive
                                                };

            return result;
        }

        private IQueryable<PenalPointsDTO> GetParametersFilteredPenalPoints(PenalPointsFilters filters)
        {
            var query = from points in Db.PenalPointsRegister
                        join decree in Db.PenalDecreesRegisters on points.PenalRegisterId equals decree.Id
                        join permit in Db.CommercialFishingPermitRegisters on points.PermitId equals permit.Id
                        join permitLicense in Db.CommercialFishingPermitLicensesRegisters on points.PermitLicenseId equals permitLicense.Id into licenses
                        from permitLicense in licenses.DefaultIfEmpty()
                        join pointsOwnerPerson in this.Db.Persons on points.PointsOwnerPersonId equals pointsOwnerPerson.Id into ownerPer
                        from pointsOwnerPerson in ownerPer.DefaultIfEmpty()
                        join pointsOwnerLegal in this.Db.Legals on points.PointsOwnerLegalId equals pointsOwnerLegal.Id into ownerLeg
                        from pointsOwnerLegal in ownerLeg.DefaultIfEmpty()
                        join ship in Db.ShipsRegister on points.ShipId equals ship.Id into shipReg
                        from ship in shipReg.DefaultIfEmpty()
                        join fisher in Db.FishermenRegisters on points.QualifiedFisherId equals fisher.Id into fishReg
                        from fisher in fishReg.DefaultIfEmpty()
                        join fisherPerson in this.Db.Persons on fisher.PersonId equals fisherPerson.Id into fishPer
                        from fisherPerson in fishPer.DefaultIfEmpty()
                        where points.IsActive == !filters.ShowInactiveRecords
                        select new
                        {
                            Id = points.Id,
                            PenalDecreeId = points.PenalRegisterId,
                            DecreeNum = points.DecreeNum,
                            PenalDecreeNum = decree.DecreeNum,
                            PermitNum = permit.RegistrationNum,
                            PermitLicenseNum = permitLicense.RegistrationNum,
                            PenalDecreeDate = decree.IssueDate,
                            PointsType = points.PointsType,
                            PointsAmount = points.PointsAmount,
                            ShipName = ship.Name,
                            ShipId = ship.Id,
                            ShipCfr = ship.Cfr,
                            ShipExternalMarking = ship.ExternalMark,
                            ShipCaptain = fisherPerson.FirstName + " " + fisherPerson.LastName,
                            ShipCaptainIdentifier = fisherPerson.EgnLnc,
                            PermitOwner = pointsOwnerPerson != null
                                            ? pointsOwnerPerson.FirstName + " " + pointsOwnerPerson.LastName
                                            : pointsOwnerLegal.Name,
                            PermitOwnerPersonId = pointsOwnerPerson.Id,
                            PermitOwnerLegalId = pointsOwnerLegal.Id,
                            PermitOwnerIdentifier = pointsOwnerPerson != null
                                                        ? pointsOwnerPerson.EgnLnc
                                                        : pointsOwnerLegal.Eik,
                            IssueDate = points.IssueDate,
                            IsIncreasePoints = points.IsIncreasePoints,
                            IsActive = points.IsActive
                        };

            if (!string.IsNullOrEmpty(filters.DecreeNum))
            {
                query = query.Where(x => x.DecreeNum.ToLower().Contains(filters.DecreeNum.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.PenalDecreeNum))
            {
                query = query.Where(x => x.PenalDecreeNum.ToLower().Contains(filters.PenalDecreeNum.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.PermitNum))
            {
                query = query.Where(x => x.PermitNum.ToLower().Contains(filters.PermitNum.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.PermitLicenseNum))
            {
                query = query.Where(x => x.PermitLicenseNum.ToLower().Contains(filters.PermitLicenseNum.ToLower()));
            }

            if (filters.DecreeDateFrom.HasValue)
            {
                query = query.Where(x => x.IssueDate >= filters.DecreeDateFrom.Value);
            }

            if (filters.DecreeDateTo.HasValue)
            {
                query = query.Where(x => x.IssueDate <= filters.DecreeDateTo.Value);
            }

            if (filters.PenalDecreeDateFrom.HasValue)
            {
                query = query.Where(x => x.IssueDate >= filters.PenalDecreeDateFrom.Value);
            }

            if (filters.PenalDecreeDateTo.HasValue)
            {
                query = query.Where(x => x.IssueDate <= filters.PenalDecreeDateTo.Value);
            }

            if (filters.isIncreasePoints.HasValue)
            {
                query = query.Where(x => x.IsIncreasePoints == filters.isIncreasePoints);
            }

            if (filters.PointsType.HasValue)
            {
                query = query.Where(x => x.PointsType.ToLower().Contains(filters.PointsType.ToString().ToLower()));
            }

            if (filters.ShipId.HasValue)
            {
                int shipUId = (from ship in Db.ShipsRegister
                               where ship.Id == filters.ShipId.Value
                               select ship.ShipUid).First();

                List<int> shipIds = (from ship in Db.ShipsRegister
                                     where ship.ShipUid == shipUId
                                     select ship.Id).ToList();

                query = query.Where(x => shipIds.Contains(x.ShipId));
            }

            if (!string.IsNullOrEmpty(filters.ShipName))
            {
                query = query.Where(x => x.ShipName.ToLower().Contains(filters.ShipName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.ShipCfr))
            {
                query = query.Where(x => x.ShipCfr.ToLower().Contains(filters.ShipCfr.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.ShipExternalMarking))
            {
                query = query.Where(x => x.ShipExternalMarking.ToLower().Contains(filters.ShipExternalMarking.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.CaptainName))
            {
                query = query.Where(x => x.ShipCaptain.ToLower().Contains(filters.CaptainName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.CaptainIdentifier))
            {
                query = query.Where(x => x.ShipCaptainIdentifier.ToLower().Contains(filters.CaptainIdentifier.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.PermitOwnerName))
            {
                query = query.Where(x => x.PermitOwner.ToLower().Contains(filters.PermitOwnerName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.PermitOwnerIdentifier))
            {
                query = query.Where(x => x.PermitOwnerIdentifier.ToLower().Contains(filters.PermitOwnerIdentifier.ToLower()));
            }

            if (filters.PersonId.HasValue)
            {
                query = query.Where(x => x.PermitOwnerPersonId == filters.PersonId);
            }

            if (filters.LegalId.HasValue)
            {
                query = query.Where(x => x.PermitOwnerLegalId == filters.LegalId);
            }

            IQueryable<PenalPointsDTO> result = from points in query
                                                orderby points.IssueDate descending
                                                select new PenalPointsDTO
                                                {
                                                    Id = points.Id,
                                                    PenalDecreeId = points.PenalDecreeId,
                                                    PointsType = Enum.Parse<PointsTypeEnum>(points.PointsType),
                                                    DecreeNum = points.DecreeNum,
                                                    PenalDecreeNum = points.PenalDecreeNum,
                                                    PointsAmount = points.PointsAmount,
                                                    Name = !string.IsNullOrEmpty(points.PermitOwner)
                                                            ? points.PermitOwner
                                                            : points.ShipCaptain,
                                                    Ship = points.ShipName,
                                                    IssueDate = points.IssueDate,
                                                    IsIncreasePoints = points.IsIncreasePoints,
                                                    IsActive = points.IsActive
                                                };

            return result;
        }

        private IQueryable<PenalPointsDTO> GetFreeTextFilteredPenalPoints(string text, bool showInactive)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            IQueryable<PenalPointsDTO> result = from points in Db.PenalPointsRegister
                                                join decree in Db.PenalDecreesRegisters on points.PenalRegisterId equals decree.Id
                                                join permit in Db.CommercialFishingPermitRegisters on points.PermitId equals permit.Id
                                                join pointsOwnerPerson in this.Db.Persons on points.PointsOwnerPersonId equals pointsOwnerPerson.Id into ownerPer
                                                from pointsOwnerPerson in ownerPer.DefaultIfEmpty()
                                                join pointsOwnerLegal in this.Db.Legals on points.PointsOwnerLegalId equals pointsOwnerLegal.Id into ownerLeg
                                                from pointsOwnerLegal in ownerLeg.DefaultIfEmpty()
                                                join ship in Db.ShipsRegister on points.ShipId equals ship.Id into shipReg
                                                from ship in shipReg.DefaultIfEmpty()
                                                where points.IsActive == !showInactive
                                                    && (points.DecreeNum.ToLower().Contains(text)
                                                        || decree.DecreeNum.ToLower().Contains(text)
                                                        || ship.Name.ToLower().Contains(text)
                                                        || (pointsOwnerPerson != null ? pointsOwnerPerson.FirstName + " " + pointsOwnerPerson.LastName : pointsOwnerLegal.Name).ToLower().Contains(text)
                                                        || (searchDate.HasValue && points.IssueDate == searchDate.Value))
                                                orderby points.IssueDate descending
                                                select new PenalPointsDTO
                                                {
                                                    Id = points.Id,
                                                    PenalDecreeId = points.PenalRegisterId,
                                                    DecreeNum = points.DecreeNum,
                                                    PenalDecreeNum = decree.DecreeNum,
                                                    PointsType = Enum.Parse<PointsTypeEnum>(points.PointsType),
                                                    Name = pointsOwnerPerson != null
                                                                     ? pointsOwnerPerson.FirstName + " " + pointsOwnerPerson.LastName
                                                                     : pointsOwnerLegal.Name,
                                                    PointsAmount = points.PointsAmount,
                                                    Ship = ship.Name,
                                                    IssueDate = points.IssueDate,
                                                    IsIncreasePoints = points.IsIncreasePoints,
                                                    IsActive = points.IsActive
                                                };

            return result;
        }

        private int GetPermitLicensesPermitId(int permitLicenseId)
        {
            int permitId = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                            where permitLicense.Id == permitLicenseId
                            select permitLicense.PermitId).First();

            return permitId;
        }

        private List<PenalPointsAppealDTO> GetPenalPointsComplaints(int pointsId)
        {
            List<PenalPointsAppealDTO> result = (from complaint in Db.PenalPointComplaintStatuses
                                                 join status in Db.NpenalPointStatuses on complaint.StatusId equals status.Id
                                                 where complaint.PenalPointsId == pointsId
                                                 select new PenalPointsAppealDTO
                                                 {
                                                     Id = complaint.Id,
                                                     StatusName = status.Name,
                                                     CourtId = complaint.CourtId,
                                                     DecreeDate = complaint.DecreeDate,
                                                     DecreeNum = complaint.DecreeNum,
                                                     StatusId = complaint.StatusId,
                                                     DateOfChange = complaint.UpdatedOn != null
                                                                        ? complaint.UpdatedOn
                                                                        : complaint.CreatedOn,
                                                     AppealDate = complaint.AppealDate,
                                                     AppealNum = complaint.AppealNum,
                                                     IsActive = complaint.IsActive
                                                 }).ToList();

            return result;
        }

        private void EditPenalPointsComplaints(PenalPointsRegister points, List<PenalPointsAppealDTO> complaints)
        {
            if (complaints != null)
            {
                List<PenalPointComplaintStatus> dbComplaints = complaints.Any(x => x.Id != null)
                    ? Db.PenalPointComplaintStatuses.Where(x => x.PenalPointsId == points.Id).ToList()
                    : new List<PenalPointComplaintStatus>();

                foreach (PenalPointsAppealDTO complaint in complaints)
                {
                    if (complaint.Id == null)
                    {
                        AddPenalPointsComplaintsEntry(points, complaint);
                    }
                    else
                    {
                        PenalPointComplaintStatus dbComplaint = dbComplaints.Where(x => x.Id == complaint.Id).Single();
                        dbComplaint.StatusId = complaint.StatusId.Value;
                        dbComplaint.CourtId = complaint.CourtId;
                        dbComplaint.AppealDate = complaint.AppealDate;
                        dbComplaint.AppealNum = complaint.AppealNum;
                        dbComplaint.DecreeDate = complaint.DecreeDate;
                        dbComplaint.DecreeNum = complaint.DecreeNum;
                        dbComplaint.IsActive = complaint.IsActive.Value;
                    }
                }
            }
            else
            {
                List<PenalPointComplaintStatus> dbComplaints = this.Db.PenalPointComplaintStatuses.Where(x => x.PenalPointsId == points.Id).ToList();

                foreach (PenalPointComplaintStatus complaint in dbComplaints)
                {
                    complaint.IsActive = false;
                }
            }
        }

        private void AddPenalPointsComplaintsEntry(PenalPointsRegister points, PenalPointsAppealDTO complaint)
        {
            PenalPointComplaintStatus entry = new PenalPointComplaintStatus
            {
                PenalPoints = points,
                CourtId = complaint.CourtId,
                StatusId = complaint.StatusId.Value,
                AppealDate = complaint.AppealDate,
                AppealNum = complaint.AppealNum,
                DecreeDate = complaint.DecreeDate,
                DecreeNum = complaint.DecreeNum,
                IsActive = complaint.IsActive.Value
            };

            Db.PenalPointComplaintStatuses.Add(entry);
        }
    }
}
