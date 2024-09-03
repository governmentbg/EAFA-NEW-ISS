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
using System.Collections.Generic;
using System.Linq;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
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

        public static void InitShip(FishingShipViewModel fishingShip, ShipChecksViewModel shipChecks, ShipCatchesViewModel shipCatches, FishingGearsViewModel fishingGears = null)
        {
            fishingShip.ShipData.ShipSelected = CommandBuilder.CreateFrom(
                async (ShipSelectNomenclatureDto ship) =>
                {
                    await TLLoadingHelper.ShowFullLoadingScreen();

                    INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();
                    int uid = ship.Uid;

                    if (fishingShip.Inspection.ActivityType == ViewActivityType.Add)
                    {
                        ShipDto chosenShip = nomTransaction.GetShip(ship.Id);

                        if (chosenShip == null)
                        {
                            await TLLoadingHelper.HideFullLoadingScreen();
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

                        shipChecks.PermitLicenses.PermitLicenses.Value.Clear();
                        shipChecks.Permits.Permits.Value.Clear();
                        shipChecks.LogBooks.LogBooks.Value.Clear();

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

                            shipChecks.PermitLicenses.PermitLicenses.Value.AddRange(
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
                            shipChecks.Permits.Permits.Value.AddRange(
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

                            shipChecks.LogBooks.LogBooks.Value.AddRange(
                                logBooks.ConvertAll(f =>
                                {
                                    LogBookModel logBook = new LogBookModel(shipCatches)
                                    {
                                        Pages = pages?.FindAll(s => s.LogBookId == f.Id).OrderByDescending(x => x.IssuedOn).ToList() ?? new List<LogBookPageDto>(),
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

                            List<SelectNomenclatureDto> fishingGearTypes = nomTransaction.GetFishingGears().Select(x => new SelectNomenclatureDto()
                            {
                                Code = x.Code,
                                Id = x.Id,
                                Name = x.Name
                            }).ToList();
                            List<FishingGearDto> shipFishingGears = inspectionTransaction.GetFishingGearsForShip(uid);

                            fishingGears.AllFishingGears = shipFishingGears.ConvertAll(f => new FishingGearModel
                            {
                                Count = f.Count,
                                NetEyeSize = f.NetEyeSize,
                                Marks = string.Join(", ", f.Marks.Select(s => s.FullNumber?.ToString())),
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

                    SelectNomenclatureDto defaultAction = new SelectNomenclatureDto
                    {
                        Id = 1,
                        Code = nameof(SubjectType.Person),
                        Name = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Person"],
                    };

                    ResetPerson(fishingShip.ShipOwner, defaultAction);
                    ResetPerson(fishingShip.ShipUser, defaultAction);
                    ResetPerson(fishingShip.ShipRepresentative, defaultAction);
                    ResetPerson(fishingShip.ShipCaptain, defaultAction);

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

                    if (fishingShip.ShipOwner.People.Count == 1)
                    {
                        fishingShip.ShipOwner.Person.Value = fishingShip.ShipOwner.People[0];
                        fishingShip.ShipOwner.PersonChosen.Execute(fishingShip.ShipOwner.People[0]);
                        fishingShip.ShipOwner.Validation.Force();
                    }
                    if (fishingShip.ShipUser.People.Count == 1)
                    {
                        fishingShip.ShipUser.Person.Value = fishingShip.ShipUser.People[0];
                        fishingShip.ShipUser.PersonChosen.Execute(fishingShip.ShipUser.People[0]);
                        fishingShip.ShipUser.Validation.Force();
                    }
                    if (fishingShip.ShipRepresentative.People.Count == 1)
                    {
                        fishingShip.ShipRepresentative.Person.Value = fishingShip.ShipRepresentative.People[0];
                        fishingShip.ShipRepresentative.PersonChosen.Execute(fishingShip.ShipRepresentative.People[0]);
                        fishingShip.ShipRepresentative.Validation.Force();
                    }
                    if (fishingShip.ShipCaptain.People.Count == 1)
                    {
                        fishingShip.ShipCaptain.Person.Value = fishingShip.ShipCaptain.People[0];
                        fishingShip.ShipCaptain.PersonChosen.Execute(fishingShip.ShipCaptain.People[0]);
                        fishingShip.ShipCaptain.Validation.Force();
                    }

                    await TLLoadingHelper.HideFullLoadingScreen();
                }
            );
        }

        private static void ResetPerson(InspectedPersonViewModel person, SelectNomenclatureDto defaultAction)
        {
            person.Action = defaultAction;
            person.FirstName.Value = null;
            person.MiddleName.Value = null;
            person.LastName.Value = null;
            person.Egn.Value = null;
            person.EIK.Value = null;
            person.Address.Value = null;
            person.Nationality.Value = null;
            person.Person.Value = null;
        }

        public static void Dispose(InspectionPageViewModel inspectionPageModel)
        {
            MessagingCenter.Instance.Unsubscribe<InspectionUploadedEventArgs>(inspectionPageModel, string.Empty);
        }
    }
}
