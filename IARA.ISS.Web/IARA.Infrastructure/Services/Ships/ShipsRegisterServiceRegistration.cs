using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class ShipsRegisterService : Service, IShipsRegisterService
    {
        public RegixChecksWrapperDTO<ShipRegisterRegixDataDTO> GetShipRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<ShipRegisterRegixDataDTO> result = new RegixChecksWrapperDTO<ShipRegisterRegixDataDTO>
            {
                DialogDataModel = GetApplicationRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetShipRegisterChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public ShipRegisterRegixDataDTO GetApplicationRegixData(int applicationId)
        {
            ShipRegisterApplicationDataIds regixDataIds = GetDataIds(applicationId);

            ShipRegisterRegixDataDTO regixData = new ShipRegisterRegixDataDTO
            {
                Id = regixDataIds.ShipId,
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode,
                CFR = regixDataIds.Cfr,
                Name = regixDataIds.Name,
                RegLicenceNum = regixDataIds.RegLicenceNum,
                RegLicencePublishVolume = regixDataIds.RegLicensePublishVolume,
                RegLicencePublishPage = regixDataIds.RegLicensePublishPage,
                VesselTypeId = regixDataIds.VesselTypeId,
                VesselTypeName = regixDataIds.VesselTypeName,
                GrossTonnage = regixDataIds.GrossTonnage,
                NetTonnage = regixDataIds.NetTonnage,
                TotalLength = regixDataIds.TotalLength,
                TotalWidth = regixDataIds.TotalWidth,
                BoardHeight = regixDataIds.BoardHeight,
                ShipDraught = regixDataIds.ShipDraught,
                LengthBetweenPerpendiculars = regixDataIds.LengthBetweenPerpendiculars,
                FuelTypeId = regixDataIds.FuelTypeId,
                FuelTypeName = regixDataIds.FuelTypeName
            };

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId);
            regixData.Owners = GetOwnersRegix(regixDataIds.ShipId);

            if (regixDataIds.HasFishingCapacity)
            {
                CapacityChangeHistoryDTO change = fishingCapacityService.GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);
                regixData.RemainingCapacityAction = fishingCapacityService.GetCapacityFreeActionsRegixFromChangeHistory(change);
            }

            return regixData;
        }

        public ShipRegisterEditDTO GetApplicationDataForRegister(int applicationId)
        {
            ShipRegister dbShip = (from ship in Db.ShipsRegister
                                   where ship.ApplicationId == applicationId
                                        && ship.RecordType == nameof(RecordTypesEnum.Application)
                                   select ship).First();

            ShipRegisterEditDTO result = MapDbShipToDTO(dbShip, true);
            return result;
        }

        public ShipRegisterApplicationEditDTO GetShipApplication(int applicationId)
        {
            ShipRegister dbShip = (from ship in Db.ShipsRegister
                                   where ship.ApplicationId == applicationId
                                       && ship.RecordType == nameof(RecordTypesEnum.Application)
                                   select ship).FirstOrDefault();

            ShipRegisterApplicationEditDTO result = null;

            if (dbShip == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<ShipRegisterApplicationEditDTO>(draft);
                    result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);
                    result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                    result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                    result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                    if (result.IsPaid && result.PaymentInformation == null)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
                else
                {
                    result = new ShipRegisterApplicationEditDTO
                    {
                        IsPaid = applicationService.IsApplicationPaid(applicationId),
                        HasDelivery = deliveryService.HasApplicationDelivery(applicationId),
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };

                    if (result.IsPaid)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
            }
            else
            {
                result = MapDbShipApplicationToDTO(dbShip);
            }

            return result;
        }

        public int AddShipApplication(ShipRegisterApplicationEditDTO ship, ApplicationStatusesEnum? nextManualStatus)
        {
            ShipRegister entry;

            using (TransactionScope scope = new TransactionScope())
            {
                entry = new ShipRegister
                {
                    ApplicationId = ship.ApplicationId.Value,
                    RecordType = nameof(RecordTypesEnum.Application),
                    EventTypeId = CalculateNewShipEventTypeId(ship.BuildYear.Value, ship.ImportCountryId),
                    EventDate = DateTime.Now,
                    Cfr = ship.CFR,
                    Name = ship.Name,
                    ExternalMark = ship.ExternalMark,
                    RegistrationNum = ship.RegistrationNumber,
                    RegistrationDate = ship.RegistrationDate.Value,
                    FleetTypeId = ship.FleetTypeId.Value,
                    IrcscallSign = ship.IRCSCallSign,
                    Mmsi = ship.MMSI,
                    Uvi = ship.UVI,
                    FlagCountryId = ship.CountryFlagId.Value,
                    HasAis = ship.HasAIS.Value,
                    HasErs = ship.HasERS.Value,
                    HasVms = ship.HasVMS.Value,
                    VesselTypeId = ship.VesselTypeId,
                    RegLicenceNum = ship.RegLicenceNum,
                    RegLicenseDate = ship.RegLicenceDate.Value,
                    RegLicensePublisher = ship.RegLicencePublisher,
                    RegLicensePublishVolume = ship.RegLicencePublishVolume,
                    RegLicensePublishPage = ship.RegLicencePublishPage,
                    RegLicensePublishNum = ship.RegLicencePublishNum,
                    ExploitationStartDate = ship.ExploitationStartDate,
                    BuildYear = ship.BuildYear.Value,
                    BuildPlace = ship.BuildPlace,
                    AdminDecisionNum = ship.AdminDecisionNum,
                    AdminDecisionDate = ship.AdminDecisionDate,
                    PublicAidTypeId = ship.PublicAidTypeId.Value,
                    FleetSegmentId = ship.FleetSegmentId.Value,
                    PortId = ship.PortId.Value,
                    StayPortId = ship.StayPortId,
                    SailAreaId = ship.SailAreaId,
                    TotalLength = ship.TotalLength.Value,
                    TotalWidth = ship.TotalWidth.Value,
                    GrossTonnage = ship.GrossTonnage.Value,
                    NetTonnage = ship.NetTonnage,
                    OtherTonnage = ship.OtherTonnage,
                    BoardHeight = ship.BoardHeight.Value,
                    ShipDraught = ship.ShipDraught.Value,
                    LengthBetweenPerpendiculars = ship.LengthBetweenPerpendiculars,
                    MainEnginePower = ship.MainEnginePower.Value,
                    AuxiliaryEnginePower = ship.AuxiliaryEnginePower,
                    MainEngineNum = ship.MainEngineNum,
                    MainEngineModel = ship.MainEngineModel,
                    MainFishingGearId = ship.MainFishingGearId.Value,
                    AdditionalFishingGearId = ship.AdditionalFishingGearId,
                    HullMaterialId = ship.HullMaterialId.Value,
                    FuelTypeId = ship.FuelTypeId.Value,
                    TotalPassengerCapacity = ship.TotalPassengerCapacity.Value,
                    CrewCount = ship.CrewCount.Value,
                    HasControlCard = ship.HasControlCard.Value,
                    HasValidityCertificate = ship.HasValidityCertificate.Value,
                    HasFoodLawLicense = ship.HasFoodLawLicence.Value,
                    ImportCountryId = ship.ImportCountryId,
                    ShipAssociationId = ship.ShipAssociationId,
                    Comments = ship.Comments
                };

                if (ship.SailAreaId.HasValue)
                {
                    entry.SailAreaId = ship.SailAreaId;
                }
                else if (!string.IsNullOrEmpty(ship.SailAreaName))
                {
                    NsailArea sailArea = new NsailArea { Name = ship.SailAreaName };
                    entry.SailArea = sailArea;
                }

                if (ship.HasFoodLawLicence.Value)
                {
                    entry.FoodLawLicenseNum = ship.FoodLawLicenceNum;
                    entry.FoodLawLicenseDate = ship.FoodLawLicenceDate;
                }

                if (ship.HasControlCard.Value)
                {
                    entry.ControlCardNum = ship.ControlCardNum;
                    entry.ControlCardDate = ship.ControlCardDate.Value;
                }

                if (ship.HasValidityCertificate.Value)
                {
                    entry.ControlCardValidityCertificateNum = ship.ControlCardValidityCertificateNum;
                    entry.ControlCardValidityCertificateDate = ship.ControlCardValidityCertificateDate.Value;
                    entry.ControlCardDateOfLastAttestation = ship.ControlCardDateOfLastAttestation.Value;
                }

                Db.ShipsRegister.Add(entry);
                Db.SaveChanges();

                Application application = (from appl in Db.Applications
                                           where appl.Id == entry.ApplicationId
                                           select appl).First();

                Db.AddOrEditApplicationSubmittedBy(application, ship.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedFor(application, ship.SubmittedFor);
                Db.SaveChanges();

                FleetTypeNomenclatureDTO fleet = GetFleetType(ship.FleetTypeId.Value);
                if (fleet.HasFishingCapacity)
                {
                    fishingCapacityService.AddIncreaseCapacityChangeHistory(RecordTypesEnum.Application,
                                                                            entry.ApplicationId.Value,
                                                                            entry.Id,
                                                                            ship.GrossTonnage.Value,
                                                                            ship.MainEnginePower.Value,
                                                                            ship.AcquiredFishingCapacity,
                                                                            ship.RemainingCapacityAction);
                }

                if (ship.Files != null)
                {
                    foreach (FileInfoDTO file in ship.Files)
                    {
                        Db.AddOrEditFile(entry, entry.ShipRegisterFiles, file);
                    }
                }

                foreach (ShipOwnerDTO owner in ship.Owners)
                {
                    AddShipOwner(entry, owner);
                    Db.SaveChanges();
                }

                Db.AddDeliveryData(application, ship);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> shipFiles = ship.Files;
            ship.Files = null;
            stateMachine.Act(entry.ApplicationId.Value, CommonUtils.Serialize(ship), shipFiles, nextManualStatus);

            return entry.Id;
        }

        public void EditShipApplication(ShipRegisterApplicationEditDTO ship, ApplicationStatusesEnum? manualStatus = null)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                ShipRegister dbShip = (from sh in Db.ShipsRegister
                                            .AsSplitQuery()
                                            .Include(x => x.ShipRegisterFiles)
                                       where sh.Id == ship.Id
                                       select sh).First();

                // има промяна в годината на построяване, трябва да се преизчисли EventTypeId
                if (dbShip.BuildYear != ship.BuildYear)
                {
                    dbShip.EventTypeId = CalculateNewShipEventTypeId(ship.BuildYear.Value, ship.ImportCountryId);
                    dbShip.EventDate = DateTime.Now;
                }

                EditShipRegixDataFields(dbShip, ship);

                dbShip.ExternalMark = ship.ExternalMark;
                dbShip.RegistrationNum = ship.RegistrationNumber;
                dbShip.RegistrationDate = ship.RegistrationDate.Value;
                dbShip.FleetTypeId = ship.FleetTypeId.Value;
                dbShip.FleetSegmentId = ship.FleetSegmentId.Value;
                dbShip.IrcscallSign = ship.IRCSCallSign;
                dbShip.Mmsi = ship.MMSI;
                dbShip.Uvi = ship.UVI;
                dbShip.FlagCountryId = ship.CountryFlagId.Value;
                dbShip.HasAis = ship.HasAIS.Value;
                dbShip.HasErs = ship.HasERS.Value;
                dbShip.HasVms = ship.HasVMS.Value;
                dbShip.RegLicenseDate = ship.RegLicenceDate.Value;
                dbShip.RegLicensePublisher = ship.RegLicencePublisher;
                dbShip.RegLicensePublishNum = ship.RegLicencePublishNum;
                dbShip.ExploitationStartDate = ship.ExploitationStartDate;
                dbShip.BuildYear = ship.BuildYear.Value;
                dbShip.BuildPlace = ship.BuildPlace;
                dbShip.AdminDecisionNum = ship.AdminDecisionNum;
                dbShip.AdminDecisionDate = ship.AdminDecisionDate;
                dbShip.PublicAidTypeId = ship.PublicAidTypeId.Value;
                dbShip.PortId = ship.PortId.Value;
                dbShip.StayPortId = ship.StayPortId;
                dbShip.OtherTonnage = ship.OtherTonnage;
                dbShip.MainEnginePower = ship.MainEnginePower.Value;
                dbShip.AuxiliaryEnginePower = ship.AuxiliaryEnginePower;
                dbShip.MainEngineNum = ship.MainEngineNum;
                dbShip.MainEngineModel = ship.MainEngineModel;
                dbShip.MainFishingGearId = ship.MainFishingGearId.Value;
                dbShip.AdditionalFishingGearId = ship.AdditionalFishingGearId;
                dbShip.HullMaterialId = ship.HullMaterialId.Value;
                dbShip.TotalPassengerCapacity = ship.TotalPassengerCapacity.Value;
                dbShip.CrewCount = ship.CrewCount.Value;
                dbShip.HasFoodLawLicense = ship.HasFoodLawLicence.Value;
                dbShip.HasControlCard = ship.HasControlCard.Value;
                dbShip.HasValidityCertificate = ship.HasValidityCertificate.Value;
                dbShip.ImportCountryId = ship.ImportCountryId;
                dbShip.ShipAssociationId = ship.ShipAssociationId;
                dbShip.Comments = ship.Comments;

                if (ship.SailAreaId.HasValue)
                {
                    dbShip.SailAreaId = ship.SailAreaId;
                }
                else if (!string.IsNullOrEmpty(ship.SailAreaName))
                {
                    NsailArea sailArea = new NsailArea { Name = ship.SailAreaName };
                    dbShip.SailArea = sailArea;
                }

                if (ship.HasFoodLawLicence.Value)
                {
                    dbShip.FoodLawLicenseNum = ship.FoodLawLicenceNum;
                    dbShip.FoodLawLicenseDate = ship.FoodLawLicenceDate;
                }
                else
                {
                    dbShip.FoodLawLicenseNum = null;
                    dbShip.FoodLawLicenseDate = null;
                }

                if (ship.HasControlCard.Value)
                {
                    dbShip.ControlCardNum = ship.ControlCardNum;
                    dbShip.ControlCardDate = ship.ControlCardDate.Value;
                }
                else
                {
                    dbShip.ControlCardNum = null;
                    dbShip.ControlCardDate = null;
                }

                if (ship.HasValidityCertificate.Value)
                {
                    dbShip.ControlCardValidityCertificateNum = ship.ControlCardValidityCertificateNum;
                    dbShip.ControlCardValidityCertificateDate = ship.ControlCardValidityCertificateDate.Value;
                    dbShip.ControlCardDateOfLastAttestation = ship.ControlCardDateOfLastAttestation.Value;
                }
                else
                {
                    dbShip.ControlCardValidityCertificateNum = null;
                    dbShip.ControlCardValidityCertificateDate = null;
                    dbShip.ControlCardDateOfLastAttestation = null;
                }

                application = (from appl in Db.Applications
                               where appl.Id == dbShip.ApplicationId
                               select appl).First();

                Db.AddOrEditApplicationSubmittedBy(application, ship.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedFor(application, ship.SubmittedFor);
                Db.SaveChanges();

                Db.EditDeliveryData(application, ship);

                FleetTypeNomenclatureDTO fleet = GetFleetType(ship.FleetTypeId.Value);
                if (fleet.HasFishingCapacity)
                {
                    bool editResult = fishingCapacityService.EditIncreaseCapacityChangeHistory(dbShip.ApplicationId.Value,
                                                                                               ship.GrossTonnage.Value,
                                                                                               ship.MainEnginePower.Value,
                                                                                               ship.AcquiredFishingCapacity,
                                                                                               ship.RemainingCapacityAction);

                    if (!editResult)
                    {
                        fishingCapacityService.AddIncreaseCapacityChangeHistory(RecordTypesEnum.Application,
                                                                                dbShip.ApplicationId.Value,
                                                                                dbShip.Id,
                                                                                ship.GrossTonnage.Value,
                                                                                ship.MainEnginePower.Value,
                                                                                ship.AcquiredFishingCapacity,
                                                                                ship.RemainingCapacityAction);
                    }
                }
                else
                {
                    fishingCapacityService.EditIncreaseCapacityChangeHistory(dbShip.ApplicationId.Value,
                                                                             ship.GrossTonnage.Value,
                                                                             ship.MainEnginePower.Value,
                                                                             null,
                                                                             null,
                                                                             isActive: false);
                }

                foreach (ShipOwnerDTO owner in ship.Owners)
                {
                    EditShipOwner(dbShip, owner);
                    Db.SaveChanges();
                }

                if (ship.Files != null)
                {
                    foreach (FileInfoDTO file in ship.Files)
                    {
                        Db.AddOrEditFile(dbShip, dbShip.ShipRegisterFiles, file);
                    }
                }

                Db.SaveChanges();
                scope.Complete();
            }

            List<FileInfoDTO> shipFiles = ship.Files;
            ship.Files = null;
            stateMachine.Act(application.Id, CommonUtils.Serialize(ship), shipFiles, manualStatus, ship.StatusReason);
        }

        public void EditShipApplicationRegixData(ShipRegisterRegixDataDTO ship)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                ShipRegister dbShip = (from sh in Db.ShipsRegister
                                       where sh.Id == ship.Id
                                       select sh).First();

                application = (from appl in Db.Applications
                               where appl.Id == dbShip.ApplicationId
                               select appl).First();

                EditShipRegixDataFields(dbShip, ship);

                Db.AddOrEditApplicationSubmittedByRegixData(application, ship.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedForRegixData(application, ship.SubmittedFor);
                Db.SaveChanges();

                fishingCapacityService.EditIncreaseCapacityChangeHistory(dbShip.ApplicationId.Value, ship.RemainingCapacityAction);
                Db.SaveChanges();

                foreach (ShipOwnerRegixDataDTO owner in ship.Owners)
                {
                    ShipOwner oldOwner = (from shipOwner in Db.ShipOwners
                                          where shipOwner.Id == owner.Id
                                          select shipOwner).First();

                    oldOwner.OwnerIsPerson = owner.IsOwnerPerson.Value;

                    if (owner.IsOwnerPerson.Value)
                    {
                        oldOwner.OwnerLegalId = null;
                        oldOwner.OwnerPerson = Db.AddOrEditPerson(owner.RegixPersonData, owner.AddressRegistrations, oldOwner.OwnerPersonId);
                    }
                    else
                    {
                        oldOwner.OwnerPersonId = null;
                        oldOwner.OwnerLegal = Db.AddOrEditLegal(
                            new ApplicationRegisterDataDTO
                            {
                                ApplicationId = application.Id,
                                RecordType = RecordTypesEnum.Application
                            },
                            owner.RegixLegalData,
                            owner.AddressRegistrations,
                            oldOwner.OwnerLegalId
                        );
                    }

                    Db.SaveChanges();
                }

                scope.Complete();
            }

            stateMachine.Act(id: application.Id, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public async Task<ApplicationEDeliveryInfo> GetEDeliveryInfo(int applicationId, PageCodeEnum pageCode)
        {
            if (pageCode == PageCodeEnum.RegVessel)
            {
                // TODO
            }
            else if (pageCode == PageCodeEnum.DeregShip)
            {
                // TODO
            }

            throw new ArgumentException("Nothing to deliver for page code: " + pageCode.ToString());
        }

        private ShipRegisterApplicationDataIds GetDataIds(int applicationId)
        {
            ShipRegisterApplicationDataIds dataIds = (from ship in Db.ShipsRegister
                                                      join vesselType in Db.NvesselTypes on ship.VesselTypeId equals vesselType.Id into vt
                                                      from vesselType in vt.DefaultIfEmpty()
                                                      join fuelType in Db.NfuelTypes on ship.FuelTypeId equals fuelType.Id
                                                      join fleetType in Db.NfleetTypes on ship.FleetTypeId equals fleetType.Id
                                                      join appl in Db.Applications on ship.ApplicationId equals appl.Id
                                                      join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                                      where ship.ApplicationId == applicationId
                                                         && ship.RecordType == nameof(RecordTypesEnum.Application)
                                                      select new ShipRegisterApplicationDataIds
                                                      {
                                                          ShipId = ship.Id,
                                                          PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                                          Cfr = ship.Cfr,
                                                          Name = ship.Name,
                                                          RegLicenceNum = ship.RegLicenceNum,
                                                          RegLicensePublishVolume = ship.RegLicensePublishVolume,
                                                          RegLicensePublishPage = ship.RegLicensePublishPage,
                                                          VesselTypeId = ship.VesselTypeId,
                                                          VesselTypeName = vesselType != null ? vesselType.Name : null,
                                                          GrossTonnage = ship.GrossTonnage,
                                                          NetTonnage = ship.NetTonnage,
                                                          TotalLength = ship.TotalLength,
                                                          TotalWidth = ship.TotalWidth,
                                                          BoardHeight = ship.BoardHeight,
                                                          ShipDraught = ship.ShipDraught,
                                                          LengthBetweenPerpendiculars = ship.LengthBetweenPerpendiculars,
                                                          FuelTypeId = ship.FuelTypeId,
                                                          FuelTypeName = fuelType.Name,
                                                          HasFishingCapacity = fleetType.HasFishingCapacity
                                                      }).First();

            return dataIds;
        }

        private ShipRegisterApplicationEditDTO MapDbShipApplicationToDTO(ShipRegister dbShip)
        {
            ShipRegisterApplicationEditDTO result = new ShipRegisterApplicationEditDTO
            {
                Id = dbShip.Id,
                ApplicationId = dbShip.ApplicationId,
                CFR = dbShip.Cfr,
                Name = dbShip.Name,
                ExternalMark = dbShip.ExternalMark,
                RegistrationNumber = dbShip.RegistrationNum,
                RegistrationDate = dbShip.RegistrationDate,
                FleetTypeId = dbShip.FleetTypeId,
                FleetSegmentId = dbShip.FleetSegmentId,
                IRCSCallSign = dbShip.IrcscallSign,
                MMSI = dbShip.Mmsi,
                UVI = dbShip.Uvi,
                CountryFlagId = dbShip.FlagCountryId,
                HasAIS = dbShip.HasAis,
                HasERS = dbShip.HasErs,
                HasVMS = dbShip.HasVms,
                VesselTypeId = dbShip.VesselTypeId,
                RegLicenceNum = dbShip.RegLicenceNum,
                RegLicenceDate = dbShip.RegLicenseDate,
                RegLicencePublisher = dbShip.RegLicensePublisher,
                RegLicencePublishVolume = dbShip.RegLicensePublishVolume,
                RegLicencePublishPage = dbShip.RegLicensePublishPage,
                RegLicencePublishNum = dbShip.RegLicensePublishNum,
                ExploitationStartDate = dbShip.ExploitationStartDate,
                BuildYear = dbShip.BuildYear,
                BuildPlace = dbShip.BuildPlace,
                AdminDecisionNum = dbShip.AdminDecisionNum,
                AdminDecisionDate = dbShip.AdminDecisionDate,
                PublicAidTypeId = dbShip.PublicAidTypeId,
                PortId = dbShip.PortId,
                StayPortId = dbShip.StayPortId,
                SailAreaId = dbShip.SailAreaId,
                TotalLength = dbShip.TotalLength,
                TotalWidth = dbShip.TotalWidth,
                GrossTonnage = dbShip.GrossTonnage,
                NetTonnage = dbShip.NetTonnage,
                OtherTonnage = dbShip.OtherTonnage,
                BoardHeight = dbShip.BoardHeight,
                ShipDraught = dbShip.ShipDraught,
                LengthBetweenPerpendiculars = dbShip.LengthBetweenPerpendiculars,
                MainEnginePower = dbShip.MainEnginePower,
                AuxiliaryEnginePower = dbShip.AuxiliaryEnginePower,
                MainEngineNum = dbShip.MainEngineNum,
                MainEngineModel = dbShip.MainEngineModel,
                MainFishingGearId = dbShip.MainFishingGearId,
                AdditionalFishingGearId = dbShip.AdditionalFishingGearId,
                HullMaterialId = dbShip.HullMaterialId,
                FuelTypeId = dbShip.FuelTypeId,
                TotalPassengerCapacity = dbShip.TotalPassengerCapacity,
                CrewCount = dbShip.CrewCount,
                HasControlCard = dbShip.HasControlCard,
                HasValidityCertificate = dbShip.HasValidityCertificate,
                HasFoodLawLicence = dbShip.HasFoodLawLicense,
                ImportCountryId = dbShip.ImportCountryId,
                ShipAssociationId = dbShip.ShipAssociationId,
                Comments = dbShip.Comments
            };

            if (result.HasControlCard.Value)
            {
                result.ControlCardNum = dbShip.ControlCardNum;
                result.ControlCardDate = dbShip.ControlCardDate.Value;
            }

            if (result.HasValidityCertificate.Value)
            {
                result.ControlCardValidityCertificateNum = dbShip.ControlCardValidityCertificateNum;
                result.ControlCardValidityCertificateDate = dbShip.ControlCardValidityCertificateDate.Value;
                result.ControlCardDateOfLastAttestation = dbShip.ControlCardDateOfLastAttestation.Value;
            }

            if (result.HasFoodLawLicence.Value)
            {
                result.FoodLawLicenceNum = dbShip.FoodLawLicenseNum;
                result.FoodLawLicenceDate = dbShip.FoodLawLicenseDate.Value;
            }

            FleetTypeNomenclatureDTO fleet = GetFleetType(result.FleetTypeId.Value);

            if (fleet.HasFishingCapacity)
            {
                result.AcquiredFishingCapacity = fishingCapacityService.GetAcquiredFishingCapacityByApplicationId(dbShip.ApplicationId.Value);

                CapacityChangeHistoryDTO change = fishingCapacityService.GetCapacityChangeHistory(result.ApplicationId.Value, RecordTypesEnum.Application);
                result.RemainingCapacityAction = fishingCapacityService.GetCapacityFreeActionsFromChangeHistory(change);
            }

            result.SubmittedBy = applicationService.GetApplicationSubmittedBy(result.ApplicationId.Value);
            result.SubmittedFor = applicationService.GetApplicationSubmittedFor(result.ApplicationId.Value);

            result.Owners = GetOwners(result.Id.Value);
            result.Files = Db.GetFiles(Db.ShipRegisterFiles, result.Id.Value);

            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(result.ApplicationId.Value, ApplicationHierarchyTypesEnum.Online);
            result.IsPaid = applicationService.IsApplicationPaid(result.ApplicationId.Value);
            result.HasDelivery = deliveryService.HasApplicationDelivery(result.ApplicationId.Value);

            if (result.IsPaid)
            {
                result.PaymentInformation = applicationService.GetApplicationPaymentInformation(result.ApplicationId.Value);
            }

            if (result.HasDelivery && fleet.HasFishingCapacity && result.RemainingCapacityAction.Action != FishingCapacityRemainderActionEnum.NoCertificate)
            {
                result.DeliveryData = deliveryService.GetApplicationDeliveryData(result.ApplicationId.Value);
            }

            return result;
        }

        private void EditShipOwner(ShipRegister dbShip, ShipOwnerDTO owner)
        {
            ShipOwner oldOwner = (from shipOwner in Db.ShipOwners
                                  where shipOwner.Id == owner.Id
                                  select shipOwner).FirstOrDefault();

            Person person = null;
            Legal legal = null;

            if (owner.IsOwnerPerson.Value)
            {
                person = Db.AddOrEditPerson(owner.RegixPersonData, owner.AddressRegistrations, oldOwner?.OwnerPersonId);
            }
            else
            {
                legal = Db.AddOrEditLegal(
                    new ApplicationRegisterDataDTO
                    {
                        ApplicationId = dbShip.ApplicationId,
                        RecordType = Enum.Parse<RecordTypesEnum>(dbShip.RecordType)
                    },
                    owner.RegixLegalData,
                    owner.AddressRegistrations,
                    oldOwner?.OwnerLegalId
                );
            }

            if (oldOwner != null)
            {
                oldOwner.OwnerIsPerson = owner.IsOwnerPerson.Value;
                oldOwner.OwnerPerson = person;
                oldOwner.OwnerLegal = legal;
                oldOwner.OwnedShare = owner.OwnedShare.Value;
                oldOwner.IsShipHolder = owner.IsShipHolder.Value;
                oldOwner.IsActive = owner.IsActive.Value;
            }
            else
            {
                ShipOwner entry = new ShipOwner
                {
                    ShipRegister = dbShip,
                    OwnerIsPerson = owner.IsOwnerPerson.Value,
                    OwnerPerson = person,
                    OwnerLegal = legal,
                    OwnedShare = owner.OwnedShare.Value,
                    IsShipHolder = owner.IsShipHolder.Value,
                    IsActive = owner.IsActive.Value
                };

                Db.ShipOwners.Add(entry);
            }
        }

        private static void EditShipRegixDataFields<TModel>(ShipRegister dbShip, TModel ship)
            where TModel : ShipRegisterBaseRegixDataDTO
        {
            dbShip.Cfr = ship.CFR;
            dbShip.Name = ship.Name;
            dbShip.VesselTypeId = ship.VesselTypeId;
            dbShip.RegLicenceNum = ship.RegLicenceNum;
            dbShip.RegLicensePublishVolume = ship.RegLicencePublishVolume;
            dbShip.RegLicensePublishPage = ship.RegLicencePublishPage;
            dbShip.TotalLength = ship.TotalLength.Value;
            dbShip.TotalWidth = ship.TotalWidth.Value;
            dbShip.GrossTonnage = ship.GrossTonnage.Value;
            dbShip.NetTonnage = ship.NetTonnage;
            dbShip.BoardHeight = ship.BoardHeight.Value;
            dbShip.ShipDraught = ship.ShipDraught.Value;
            dbShip.LengthBetweenPerpendiculars = ship.LengthBetweenPerpendiculars;
            dbShip.FuelTypeId = ship.FuelTypeId.Value;
        }

        private FleetTypeNomenclatureDTO GetFleetTypeByShipId(int shipId)
        {
            int fleetTypeId = (from ship in Db.ShipsRegister
                               where ship.Id == shipId
                               select ship.FleetTypeId).First();

            return GetFleetType(fleetTypeId);
        }
    }
}
