using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Insp.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.FishingGearInspection
{
    public class FishingGearInspectionViewModel : InspectionPageViewModel
    {
        public const string FishingShip = nameof(FishingShip);
        public const string FishingPoundNet = nameof(FishingPoundNet);
        private InspectionCheckToolMarkDto _edit;
        private List<SelectNomenclatureDto> _fishingGearTypes;
        private SelectNomenclatureDto _fishingGearType;
        private List<SelectNomenclatureDto> _checkReasons;
        private List<SelectNomenclatureDto> _recheckReasons;

        public FishingGearInspectionViewModel()
        {
            FishingGearTypeSwitched = CommandBuilder.CreateFrom(OnFishingGearTypeSwitched);
            PoundNetChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnPoundNetChosen);
            PermitChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnPermitChosen);
            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);
            Finish = CommandBuilder.CreateFrom(OnFinish);

            ShipData = new InspectedShipDataViewModel(this, canPickLocation: false)
            {
                ShipSelected = CommandBuilder.CreateFrom<ShipSelectNomenclatureDto>(OnShipChosen)
            };
            LastHarbour = new InspectionHarbourViewModel(this);
            Owner = new InspectedPersonViewModel(this, InspectedPersonType.OwnerPers, InspectedPersonType.OwnerLegal)
            {
                PersonChosen = CommandBuilder.CreateFrom<ShipPersonnelDto>(OnOwnerChosen),
            };
            FishingGears = new FishingGearsViewModel(this, hasPingers: true);
            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this);
            InspectionFiles = new InspectionFilesViewModel(this);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this);

            this.AddValidation(groups: new Dictionary<string, Func<bool>>
            {
                { FishingShip, () => FishingGearType?.Code == FishingShip },
                { FishingPoundNet, () => FishingGearType?.Code == FishingPoundNet },
                { Group.OTHER, () => RecheckReason.Value?.Code == CommonConstants.NomenclatureOther }
            }, others: new IValidatableViewModel[]
            {
                ShipData,
                Owner,
                FishingGears,
                InspectionGeneralInfo,
                InspectionFiles,
                AdditionalInfo,
                Signatures,
            });

            PoundNet.ItemsSource = new TLObservableCollection<SelectNomenclatureDto>();
            PoundNet.GetMore = (int page, int pageSize, string search) =>
                NomenclaturesTransaction.GetPoundNets(page, pageSize, search);

            Permit.ItemsSource = new TLObservableCollection<PermitNomenclatureDto>();

            ShipData.Validation.GlobalGroups = new List<string> { FishingShip };
        }

        public InspectionCheckToolMarkDto Edit
        {
            get => _edit;
            set
            {
                _edit = value;
                ProtectedEdit = value;
            }
        }

        public InspectedShipDataViewModel ShipData { get; }
        public InspectionHarbourViewModel LastHarbour { get; }
        public InspectedPersonViewModel Owner { get; }
        public FishingGearsViewModel FishingGears { get; }
        public InspectionGeneralInfoViewModel InspectionGeneralInfo { get; }
        public InspectionFilesViewModel InspectionFiles { get; }
        public AdditionalInfoViewModel AdditionalInfo { get; }
        public SignaturesViewModel Signatures { get; }

        [Required]
        [ValidGroup(FishingPoundNet)]
        public ValidStateInfiniteSelect<SelectNomenclatureDto> PoundNet { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> CheckReason { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> RecheckReason { get; set; }

        [Required]
        [MaxLength(500)]
        [ValidGroup(Group.OTHER)]
        public ValidState OtherRecheckReason { get; set; }

        public ValidStateInfiniteSelect<PermitNomenclatureDto> Permit { get; set; }

        public ValidStateValidatableTable<ToggleViewModel> Toggles { get; set; }

        public Action ExpandAll { get; set; }

        public List<SelectNomenclatureDto> CheckReasons
        {
            get => _checkReasons;
            set => SetProperty(ref _checkReasons, value);
        }
        public List<SelectNomenclatureDto> RecheckReasons
        {
            get => _recheckReasons;
            set => SetProperty(ref _recheckReasons, value);
        }
        public List<SelectNomenclatureDto> FishingGearTypes
        {
            get => _fishingGearTypes;
            set => SetProperty(ref _fishingGearTypes, value);
        }
        public SelectNomenclatureDto FishingGearType
        {
            get => _fishingGearType;
            set => SetProperty(ref _fishingGearType, value);
        }

        public ICommand FishingGearTypeSwitched { get; }
        public ICommand PoundNetChosen { get; }
        public ICommand PermitChosen { get; }

        public override void OnDisappearing()
        {
            GlobalVariables.IsAddingInspection = false;
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[]
            {
                GroupResourceEnum.InspectedShipData,
                GroupResourceEnum.FishingGear,
                GroupResourceEnum.FishingGearInspection,
                GroupResourceEnum.Validation,
            };
        }

        public override async Task Initialize(object sender)
        {
            InspectionHelper.Initialize(this, Edit);

            FishingGearTypes = new List<SelectNomenclatureDto>
            {
                new SelectNomenclatureDto
                {
                    Id = 1,
                    Code = FishingShip,
                    Name = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingGearInspection) + "/ShipFishingGearType"]
                },
                new SelectNomenclatureDto
                {
                    Id = 2,
                    Code = FishingPoundNet,
                    Name = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingGearInspection) + "/PoundNetFishingGearType"]
                }
            };

            FishingGearType = FishingGearTypes.Find(f => f.Code == FishingShip);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();

            InspectionGeneralInfo.Init();
            ShipData.Init(countries, nomTransaction.GetVesselTypes(), nomTransaction.GetCatchZones());
            Owner.Init(countries);
            LastHarbour.Init(countries);
            PoundNet.ItemsSource.AddRange(NomenclaturesTransaction.GetPoundNets(0, CommonGlobalVariables.PullItemsCount));

            CheckReasons = nomTransaction.GetFishingGearCheckReasons();
            RecheckReasons = nomTransaction.GetFishingGearRecheckReasons();

            InspectionFiles.Init(nomTransaction.GetFileTypes(Constants.InspectionFileTypeCode));

            Toggles.Value.AddRange(nomTransaction.GetInspectionCheckTypes(InspectionType.IGM)
                .ConvertAll(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
                {
                    CheckTypeId = f.Id,
                    Text = f.Name,
                    Type = f.Type,
                    DescriptionLabel = f.DescriptionLabel,
                })
            );

            if (Edit != null)
            {
                List<SelectNomenclatureDto> fileTypes = nomTransaction.GetFileTypes();

                InspectionGeneralInfo.OnEdit(Edit);
                InspectionFiles.OnEdit(Edit);
                Signatures.OnEdit(Edit.Files, fileTypes);
                AdditionalInfo.OnEdit(Edit);

                InspectionState = Edit.InspectionState;

                CheckReason.AssignFrom(Edit.CheckReasonId, CheckReasons);
                RecheckReason.AssignFrom(Edit.RecheckReasonId, RecheckReasons);

                LastHarbour.OnEdit(Edit.Port);

                if (Edit.PoundNetId != null)
                {
                    FishingGearType = FishingGearTypes.Find(f => f.Code == FishingPoundNet);
                    PoundNet.Value = nomTransaction.GetPoundNet(Edit.PoundNetId.Value);

                    if (PoundNet.Value != null && ActivityType != ViewActivityType.Review)
                    {
                        OnPoundNetChosen(PoundNet.Value);

                        if (Edit.PermitId != null)
                        {
                            Permit.Value = nomTransaction.GetPermit(Edit.PermitId.Value);

                            if (Permit.Value != null)
                            {
                                OnPermitChosen(Permit.Value);
                            }
                        }
                    }

                    if (Edit.PermitId != null && Permit.Value == null)
                    {
                        Permit.Value = nomTransaction.GetPoundNetPermit(Edit.PermitId.Value);

                        if (Permit.Value != null && ActivityType != ViewActivityType.Review)
                        {
                            OnPermitChosen(Permit.Value);
                        }
                    }
                }
                else if (Edit.InspectedShip != null)
                {
                    FishingGearType = FishingGearTypes.Find(f => f.Code == FishingShip);
                    ShipData.OnEdit(Edit.InspectedShip);

                    if (ShipData.Ship.Value != null && ActivityType != ViewActivityType.Review)
                    {
                        OnShipChosen(ShipData.Ship.Value);

                        if (Edit.PermitId != null)
                        {
                            Permit.Value = nomTransaction.GetPermit(Edit.PermitId.Value);

                            if (Permit.Value != null)
                            {
                                OnPermitChosen(Permit.Value);
                            }
                        }
                    }

                    if (Edit.PermitId != null && Permit.Value == null)
                    {
                        Permit.Value = nomTransaction.GetPermit(Edit.PermitId.Value);

                        if (Permit.Value != null && ActivityType != ViewActivityType.Review)
                        {
                            OnPermitChosen(Permit.Value);
                        }
                    }
                }

                Owner.OnEdit(Edit.Personnel);
                FishingGears.OnEdit(Edit.FishingGears,
                    Edit.PermitId.HasValue
                        ? new List<int> { Edit.PermitId.Value }
                        : new List<int>()
                );

                Toggles.AssignFrom(Edit.Checks);
            }

            if (ActivityType == ViewActivityType.Review)
            {
                ExpandAll();
            }

            await TLLoadingHelper.HideFullLoadingScreen();
        }

        private void OnFishingGearTypeSwitched()
        {
            Owner.Person.Value = null;
            Owner.People = new List<ShipPersonnelDto>();
            Permit.Value = null;
            Permit.ItemsSource.Clear();
            FishingGears.FishingGears.Value.Clear();
        }

        private void OnShipChosen(ShipSelectNomenclatureDto nomenclatureDto)
        {
            ShipDto chosenShip = NomenclaturesTransaction.GetShip(nomenclatureDto.Id);

            if (chosenShip == null)
            {
                return;
            }

            Permit.Value = null;
            Permit.GetMore = (int page, int pageSize, string search) =>
                NomenclaturesTransaction.GetPermits(nomenclatureDto.Uid, page, pageSize, search);
            Permit.ItemsSource.ReplaceRange(
                NomenclaturesTransaction.GetPermits(nomenclatureDto.Uid, 0, CommonGlobalVariables.PullItemsCount)
            );

            ShipData.InspectedShip = chosenShip;

            ShipData.CallSign.AssignFrom(chosenShip.CallSign);
            ShipData.MMSI.AssignFrom(chosenShip.MMSI);
            ShipData.CFR.AssignFrom(chosenShip.CFR);
            ShipData.ExternalMarkings.AssignFrom(chosenShip.ExtMarkings);
            ShipData.Name.AssignFrom(chosenShip.Name);
            ShipData.UVI.AssignFrom(chosenShip.UVI);
            ShipData.Flag.AssignFrom(chosenShip.FlagId, ShipData.Flags);
            ShipData.ShipType.AssignFrom(chosenShip.ShipTypeId, ShipData.ShipTypes);

            Owner.Person.Value = null;
            Owner.People = NomenclaturesTransaction.GetShipPersonnel(nomenclatureDto.Uid)
                .FindAll(f => f.Type == InspectedPersonType.OwnerPers || f.Type == InspectedPersonType.OwnerLegal);

            Owner.InRegister.Value = Owner.People.Count > 0;

            FishingGears.FishingGears.Value.Clear();
        }

        private void OnPoundNetChosen(SelectNomenclatureDto nomenclatureDto)
        {
            Permit.Value = null;
            Permit.GetMore = (int page, int pageSize, string search) =>
                NomenclaturesTransaction.GetPoundNetPermits(nomenclatureDto.Id, page, pageSize, search);
            Permit.ItemsSource.ReplaceRange(
                NomenclaturesTransaction.GetPoundNetPermits(nomenclatureDto.Id, 0, CommonGlobalVariables.PullItemsCount)
            );

            Owner.Person.Value = null;
            Owner.People = NomenclaturesTransaction.GetPoundNetOwners(nomenclatureDto.Id);

            Owner.InRegister.Value = Owner.People.Count > 0;

            FishingGears.FishingGears.Value.Clear();
        }

        private void OnOwnerChosen(ShipPersonnelDto person)
        {
            Owner.ShipUser = FishingGearType.Code == FishingShip
                ? NomenclaturesTransaction.GetDetailedShipPerson(person.EntryId.Value, person.Type)
                : NomenclaturesTransaction.GetDetailedPoundNetPerson(person.EntryId.Value, person.Type);

            if (Owner.ShipUser == null)
            {
                return;
            }

            if (Owner.ShipUser.Address != null)
            {
                Owner.Nationality.AssignFrom(Owner.ShipUser.Address.CountryId, Owner.Nationalities);
                Owner.Address.Value = Owner.ShipUser.Address.BuildAddress();
            }

            Owner.InRegister.Value = true;
            Owner.FirstName.Value = Owner.ShipUser.FirstName;
            Owner.MiddleName.Value = Owner.ShipUser.MiddleName;
            Owner.LastName.Value = Owner.ShipUser.LastName;
            Owner.Egn.AssignFrom(Owner.ShipUser.EgnLnc);
        }

        private void OnPermitChosen(SelectNomenclatureDto nom)
        {
            FishingGears.FishingGears.Value.Clear();

            if (FishingGearType.Code == FishingPoundNet)
            {
                if (PoundNet.Value != null)
                {
                    List<FishingGearDto> fishingGears = InspectionsTransaction.GetFishingGearsForPoundNet(PoundNet.Value, Permit.Value);
                    LoadFishingGears(fishingGears);
                }
            }
            else
            {
                if (ShipData.ShipInRegister.Value && ShipData.Ship.Value != null)
                {
                    List<FishingGearDto> fishingGears = InspectionsTransaction.GetFishingGearsForShip(ShipData.Ship.Value.Uid, Permit.Value);
                    LoadFishingGears(fishingGears);
                }
            }
        }

        private void LoadFishingGears(List<FishingGearDto> fishingGears)
        {
            List<SelectNomenclatureDto> fishingGearTypes = NomenclaturesTransaction.GetFishingGears();

            FishingGears.FishingGears.Value.ReplaceRange(
                fishingGears.ConvertAll(f => new FishingGearModel
                {
                    Count = f.Count,
                    NetEyeSize = f.NetEyeSize,
                    Marks = string.Join(", ", f.Marks.Select(s => s.Number)),
                    Type = fishingGearTypes.Find(s => s.Id == f.TypeId) ?? fishingGearTypes[0],
                    Dto = new InspectedFishingGearDto
                    {
                        PermittedFishingGear = f
                    },
                })
            );
        }

        private Task OnSaveDraft()
        {
            return Save(ActivityType == ViewActivityType.Edit ? SubmitType.Edit : SubmitType.Draft);
        }

        private Task OnFinish()
        {
            return InspectionSaveHelper.Finish(ExpandAll, Validation, Save);
        }

        private Task Save(SubmitType submitType)
        {
            return this.Save(Edit,
                InspectionFiles,
                (inspectionIdentifier, files) =>
                {
                    InspectionCheckToolMarkDto dto = new InspectionCheckToolMarkDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = Edit?.ReportNum,
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.IGM,
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.AdministrativeViolation,
                        InspectorComment = AdditionalInfo.InspectorComment,
                        ByEmergencySignal = InspectionGeneralInfo.ByEmergencySignal,
                        EndDate = InspectionGeneralInfo.EndDate,
                        StartDate = InspectionGeneralInfo.StartDate,
                        Inspectors = InspectionGeneralInfo.Inspectors,
                        IsOfflineOnly = IsLocal,
                        FishingGears = FishingGears,
                        PermitId = Permit.Value,
                        CheckReasonId = CheckReason.Value,
                        RecheckReasonId = RecheckReason.Value,
                        Port = LastHarbour,
                        OtherRecheckReason = RecheckReason.Value?.Code == CommonConstants.NomenclatureOther
                            ? OtherRecheckReason.Value
                            : null,
                        Checks = Toggles
                            .Select(f => (InspectionCheckDto)f)
                            .Where(f => f != null)
                            .ToList(),
                        Personnel = new InspectionSubjectPersonnelDto[]
                        {
                            Owner,
                        }.Where(f => f != null).ToList(),
                        ObservationTexts = new InspectionObservationTextDto[]
                        {
                            AdditionalInfo.ObservationsOrViolations,
                        }.Where(f => !string.IsNullOrWhiteSpace(f.Text)).ToList(),
                    };

                    if (FishingGearType.Code == FishingPoundNet)
                    {
                        dto.PoundNetId = PoundNet.Value;
                    }
                    else
                    {
                        dto.InspectedShip = ShipData;
                    }

                    return InspectionsTransaction.HandleInspection(dto, submitType);
                }
            );
        }
    }
}
