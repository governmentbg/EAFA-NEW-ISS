using System.Collections.Generic;
using System.Linq;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.EventArgs;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Helpers
{
    public static class InspectionHelper
    {
        public static void Initialize(InspectionPageViewModel inspectionPageModel, InspectionEditDto dto)
        {
            if (dto != null)
            {
                MessagingCenter.Instance.Subscribe<InspectionUploadedEventArgs>(
                    inspectionPageModel,
                    string.Empty,
                    (eventArgs) =>
                    {
                        if (dto.Id.Value == eventArgs.OldId)
                        {
                            inspectionPageModel.IsLocal = false;
                            dto.Id = eventArgs.NewId;
                        }
                    }
                );
            }
        }

        public static void InitShip(FishingShipViewModel fishingShip, ShipChecksViewModel shipChecks, FishingGearsViewModel fishingGears = null)
        {
            fishingShip.ShipData.ShipSelected = CommandBuilder.CreateFrom(
                async (ShipSelectNomenclatureDto ship) =>
                {
                    INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();
                    int uid = ship.Uid;

                    if (fishingShip.Inspection.ActivityType == ViewActivityType.Add)
                    {
                        ShipDto chosenShip = nomTransaction.GetShip(ship.Id);

                        if (chosenShip == null)
                        {
                            return;
                        }

                        fishingShip.ShipData.InspectedShip = chosenShip;

                        fishingShip.ShipData.CallSign.AssignFrom(chosenShip.CallSign);
                        fishingShip.ShipData.MMSI.AssignFrom(chosenShip.MMSI);
                        fishingShip.ShipData.CFR.AssignFrom(chosenShip.CFR);
                        fishingShip.ShipData.ExternalMarkings.AssignFrom(chosenShip.ExtMarkings);
                        fishingShip.ShipData.Name.AssignFrom(chosenShip.Name);
                        fishingShip.ShipData.UVI.AssignFrom(chosenShip.UVI);
                        fishingShip.ShipData.Flag.AssignFrom(chosenShip.FlagId, fishingShip.ShipData.Flags);
                        fishingShip.ShipData.ShipType.AssignFrom(chosenShip.ShipTypeId, fishingShip.ShipData.ShipTypes);

                        List<PermitLicenseDto> permitLicenses = nomTransaction.GetPermitLicenses(uid);
                        List<PermitDto> permits = nomTransaction.GetPermits(uid);

                        if (permitLicenses.Count > 0)
                        {
                            shipChecks.PermitLicenses.ActionSelected = CommandBuilder.CreateFrom(() =>
                            {
                                if (fishingGears != null)
                                {
                                    List<int> permitIds = shipChecks.PermitLicenses.PermitLicenses
                                        .Where(f => (f.Corresponds.Value == nameof(CheckTypeEnum.Y) || f.Corresponds.Value == nameof(CheckTypeEnum.N)) && f.Dto?.PermitLicenseId != null)
                                        .Select(f => f.Dto.PermitLicenseId.Value)
                                        .ToList();

                                    fishingGears.FishingGears.Value.ReplaceRange(
                                        fishingGears.AllFishingGears
                                            .FindAll(f => f.Dto.PermittedFishingGear == null || permitIds.Contains(f.Dto.PermittedFishingGear.PermitId.Value))
                                    );
                                }
                            });

                            shipChecks.PermitLicenses.PermitLicenses.Value.ReplaceRange(
                                permitLicenses.ConvertAll(f =>
                                {
                                    PermitLicenseModel permit = new PermitLicenseModel
                                    {
                                        Dto = new InspectionPermitDto
                                        {
                                            PermitLicenseId = f.Id,
                                            LicenseNumber = f.LicenseNumber,
                                            PermitNumber = f.PermitNumber,
                                            From = f.From,
                                            To = f.To,
                                            TypeId = f.TypeId,
                                            TypeName = f.TypeName,
                                        }
                                    };

                                    permit.LicenseNumber.AssignFrom(f.LicenseNumber);

                                    return permit;
                                })
                            );
                        }

                        if (permits.Count > 0)
                        {
                            shipChecks.Permits.Permits.Value.ReplaceRange(
                                permits.ConvertAll(f =>
                                {
                                    PermitModel permit = new PermitModel
                                    {
                                        Dto = new InspectionPermitDto
                                        {
                                            PermitLicenseId = f.Id,
                                            PermitNumber = f.PermitNumber,
                                            From = f.From,
                                            To = f.To,
                                            TypeId = f.TypeId,
                                            TypeName = f.TypeName,
                                        }
                                    };

                                    permit.Number.AssignFrom(f.PermitNumber);

                                    return permit;
                                })
                            );
                        }

                        List<LogBookDto> logBooks = nomTransaction.GetLogBooks(uid);

                        if (logBooks.Count > 0)
                        {
                            IInspectionsTransaction inspTransaction = DependencyService.Resolve<IInspectionsTransaction>();

                            List<LogBookPageDto> pages = await inspTransaction.GetLogBookPages(logBooks.ConvertAll(f => f.Id));

                            shipChecks.LogBooks.LogBooks.Value.ReplaceRange(
                                logBooks.ConvertAll(f =>
                                {
                                    LogBookModel logBook = new LogBookModel
                                    {
                                        Pages = pages?.FindAll(s => s.LogBookId == f.Id) ?? new List<LogBookPageDto>(),
                                        Dto = new InspectionLogBookDto
                                        {
                                            LogBookId = f.Id,
                                            Number = f.Number,
                                            EndPage = f.EndPage,
                                            StartPage = f.StartPage,
                                            From = f.From,
                                        }
                                    };

                                    logBook.EndPage.Value = f.EndPage.ToString();
                                    logBook.Number.AssignFrom(f.Number);
                                    logBook.Corresponds.Value = nameof(CheckTypeEnum.X);

                                    return logBook;
                                })
                            );
                        }

                        if (fishingGears != null)
                        {
                            IInspectionsTransaction inspectionTransaction = DependencyService.Resolve<IInspectionsTransaction>();

                            List<SelectNomenclatureDto> fishingGearTypes = nomTransaction.GetFishingGears();
                            List<FishingGearDto> shipFishingGears = inspectionTransaction.GetFishingGearsForShip(uid);

                            fishingGears.AllFishingGears = shipFishingGears.ConvertAll(f => new FishingGearModel
                            {
                                Count = f.Count,
                                NetEyeSize = f.NetEyeSize,
                                Marks = string.Join(", ", f.Marks.Select(s => s.Number)),
                                Type = fishingGearTypes.Find(s => s.Id == f.TypeId) ?? fishingGearTypes[0],
                                CheckedValue = InspectedFishingGearEnum.R,
                                Dto = new InspectedFishingGearDto
                                {
                                    PermittedFishingGear = f
                                },
                            });

                            fishingGears.FishingGears.Value.Clear();
                        }
                    }

                    List<ShipPersonnelDto> shipUsers = nomTransaction.GetShipPersonnel(uid);

                    fishingShip.ShipOwner.Person.Value = null;
                    fishingShip.ShipUser.Person.Value = null;
                    fishingShip.ShipRepresentative.Person.Value = null;
                    fishingShip.ShipCaptain.Person.Value = null;

                    fishingShip.ShipOwner.People = shipUsers
                        .FindAll(f => f.Type == InspectedPersonType.OwnerPers || f.Type == InspectedPersonType.OwnerLegal);
                    fishingShip.ShipOwner.InRegister.Value = fishingShip.ShipOwner.People.Count > 0;

                    fishingShip.ShipUser.People = shipUsers
                        .FindAll(f => f.Type == InspectedPersonType.LicUsrPers || f.Type == InspectedPersonType.LicUsrLgl);
                    fishingShip.ShipUser.InRegister.Value = fishingShip.ShipUser.People.Count > 0;

                    fishingShip.ShipRepresentative.People = shipUsers
                        .Where(f => f.Type != InspectedPersonType.OwnerLegal && f.Type != InspectedPersonType.LicUsrLgl)
                        .GroupBy(f => f.Id)
                        .Select(f => f.First())
                        .Select(f =>
                        {
                            const string prefix = nameof(GroupResourceEnum.FishingShip);

                            string typeName = string.Empty;

                            switch (f.Type)
                            {
                                case InspectedPersonType.ReprsPers:
                                    typeName = TranslateExtension.Translator[prefix + "/ShipRepresentative"];
                                    break;
                                case InspectedPersonType.OwnerPers:
                                case InspectedPersonType.OwnerLegal:
                                case InspectedPersonType.ActualOwn:
                                case InspectedPersonType.OwnerBuyer:
                                    typeName = TranslateExtension.Translator[prefix + "/ShipOwner"];
                                    break;
                                case InspectedPersonType.LicUsrPers:
                                case InspectedPersonType.LicUsrLgl:
                                    typeName = TranslateExtension.Translator[prefix + "/ShipUser"];
                                    break;
                                case InspectedPersonType.CaptFshmn:
                                    typeName = TranslateExtension.Translator[prefix + "/ShipCaptain"];
                                    break;
                            }

                            return new ShipPersonnelDto
                            {
                                Id = f.Id,
                                Code = f.Code,
                                EntryId = f.EntryId,
                                Name = $"{f.Name} ({typeName})",
                                Type = f.Type
                            };
                        })
                        .ToList();
                    fishingShip.ShipRepresentative.InRegister.Value = fishingShip.ShipRepresentative.People.Count > 0;

                    fishingShip.ShipCaptain.People = shipUsers
                        .FindAll(f => f.Type == InspectedPersonType.CaptFshmn);
                    fishingShip.ShipCaptain.InRegister.Value = fishingShip.ShipCaptain.People.Count > 0;
                }
            );
        }

        public static void Dispose(InspectionPageViewModel inspectionPageModel)
        {
            MessagingCenter.Instance.Unsubscribe<InspectionUploadedEventArgs>(inspectionPageModel, string.Empty);
        }
    }
}
