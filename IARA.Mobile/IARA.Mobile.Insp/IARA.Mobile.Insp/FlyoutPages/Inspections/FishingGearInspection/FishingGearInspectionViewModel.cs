using IARA.Mobile.Application;
using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Entities.Inspections;
using IARA.Mobile.Insp.Domain.Entities.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog.PingerDialog;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.LoadFromOldPermitsDialog;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Insp.ViewModels.Models;
using IARA.Mobile.Shared.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
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

        public const string PermitRegistered = nameof(PermitRegistered);
        public const string PermitUnregistered = nameof(PermitUnregistered);

        private InspectionCheckToolMarkDto _edit;
        private List<SelectNomenclatureDto> _fishingGearTypes;
        private List<SelectNomenclatureDto> _permitTypes;
        private SelectNomenclatureDto _fishingGearType;
        private SelectNomenclatureDto _permitType;
        private List<SelectNomenclatureDto> _checkReasons;
        private List<SelectNomenclatureDto> _recheckReasons;
        private int? chosenOldPermit = null;
        private int? chosenOldPermitYear = null;

        public FishingGearInspectionViewModel()
        {
            FishingGearTypeSwitched = CommandBuilder.CreateFrom(OnFishingGearTypeSwitched);
            PoundNetChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnPoundNetChosen);
            PermitChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>((permit) => OnPermitChosen(permit));
            CheckReasonChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnCheckReasonChosen);
            PermitTypeChosen = CommandBuilder.CreateFrom(OnPermitTypeChosen);
            OpenLoadFromOldPermitDialog = CommandBuilder.CreateFrom(OnOpenLoadFromOldPermitDialog);
            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);
            Finish = CommandBuilder.CreateFrom(OnFinish);

            ShipData = new InspectedShipDataViewModel(this, canPickLocation: false)
            {
                ShipSelected = CommandBuilder.CreateFrom<ShipSelectNomenclatureDto>(OnShipChosen)
            };
            LastHarbour = new InspectionHarbourViewModel(this, false);
            Owner = new InspectedPersonViewModel(this, InspectedPersonType.OwnerPers, InspectedPersonType.OwnerLegal)
            {
                PersonChosen = CommandBuilder.CreateFrom<ShipPersonnelDto>(OnOwnerChosen),
            };
            FishingGears = new FishingGearsViewModel(this, null, hasPingers: true, showXIconWhenUnregistered: false);
            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this, false);
            InspectionFiles = new InspectionFilesViewModel(this);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this);

            this.AddValidation(groups: new Dictionary<string, Func<bool>>
            {
                { FishingShip, () => FishingGearType?.Code == FishingShip },
                { FishingPoundNet, () => FishingGearType?.Code == FishingPoundNet },
                { PermitRegistered, () => PermitType?.Code == PermitRegistered },
                { PermitUnregistered, () => PermitType?.Code == PermitUnregistered },
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

        public TLForwardSections Sections { get; set; }

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

        public ValidStateSelect<SelectNomenclatureDto> RecheckReason { get; set; }

        [Required]
        [MaxLength(500)]
        [ValidGroup(Group.OTHER)]
        public ValidState OtherRecheckReason { get; set; }

        [MaxLength(4000)]
        public ValidState OwnerComment { get; set; }

        [Required]
        [ValidGroup(PermitRegistered)]
        public ValidStateInfiniteSelect<PermitNomenclatureDto> Permit { get; set; }

        [MaxLength(50)]
        [ValidGroup(PermitUnregistered)]
        public ValidState PermitNumber { get; set; }

        [Required]
        [TLRange(1900, 2100)]
        [ValidGroup(PermitUnregistered)]
        public ValidState PermitYear { get; set; }

        public ValidStateValidatableTable<ToggleViewModel> Toggles { get; set; }

        private bool _isPermitTypeEditable = true;

        public bool IsPermitTypeEditable
        {
            get => _isPermitTypeEditable;
            set => SetProperty(ref _isPermitTypeEditable, value);
        }


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
        public List<SelectNomenclatureDto> PermitTypes
        {
            get => _permitTypes;
            set => SetProperty(ref _permitTypes, value);
        }
        public SelectNomenclatureDto FishingGearType
        {
            get => _fishingGearType;
            set => SetProperty(ref _fishingGearType, value);
        }
        public SelectNomenclatureDto PermitType
        {
            get => _permitType;
            set => SetProperty(ref _permitType, value);
        }

        public ICommand FishingGearTypeSwitched { get; }
        public ICommand PoundNetChosen { get; }
        public ICommand PermitChosen { get; }
        public ICommand CheckReasonChosen { get; }
        public ICommand PermitTypeChosen { get; }
        public ICommand OpenLoadFromOldPermitDialog { get; }

        public override void OnDisappearing()
        {
            GlobalVariables.IsAddingInspection = false;
        }
        protected override string GetInspectionJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(Edit);
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

            FishingGearType = FishingGearTypes[0];

            PermitTypes = new List<SelectNomenclatureDto>
            {
                new SelectNomenclatureDto
                {
                    Id = 1,
                    Code = PermitRegistered,
                    Name = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingGearInspection) + "/PermitRegistered"]
                },
                new SelectNomenclatureDto
                {
                    Id = 2,
                    Code = PermitUnregistered,
                    Name = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingGearInspection) + "/PermitUnregistered"]
                }
            };

            PermitType = PermitTypes[0];

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();

            await InspectionGeneralInfo.Init();
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
                chosenOldPermit = Edit.PermitLicenseRegisterId;
                chosenOldPermitYear = Edit.PermitLicenseYear;
                List<SelectNomenclatureDto> fileTypes = nomTransaction.GetFileTypes();

                await InspectionGeneralInfo.OnEdit(Edit);
                InspectionFiles.OnEdit(Edit);
                Signatures.OnEdit(Edit.Files, fileTypes);
                AdditionalInfo.OnEdit(Edit);

                InspectionState = Edit.InspectionState;

                CheckReason.AssignFrom(Edit.CheckReasonId, CheckReasons);
                RecheckReason.AssignFrom(Edit.RecheckReasonId, RecheckReasons);
                OtherRecheckReason.AssignFrom(Edit.OtherRecheckReason);
                OwnerComment.AssignFrom(Edit.OwnerComment);

                LastHarbour.OnEdit(Edit.Port);

                if (Edit.CheckReasonId != null)
                {
                    OnCheckReasonChosen(CheckReasons.Where(x => x.Id == Edit.CheckReasonId).First());
                }

                if (Edit.PermitId != null)
                {
                    PermitType = PermitTypes[0];
                }
                else
                {
                    PermitType = PermitTypes[1];
                    PermitNumber.Value = Edit.UnregisteredPermitNumber;
                    PermitYear.Value = Edit.UnregisteredPermitYear?.ToString();
                }

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
                if (Edit.PermitId == null && Edit.FishingGears?.Count() > 0)
                {
                    FishingGears.FishingGears.Value.Clear();
                    FishingGears.FishingGears.Value.AddRange(FishingGears.AllFishingGears);
                }

                Toggles.AssignFrom(Edit.Checks);
            }

            if (ActivityType == ViewActivityType.Review)
            {
                foreach (SectionView item in Sections.Children.OfType<SectionView>())
                {
                    item.IsExpanded = true;
                }
            }

            await TLLoadingHelper.HideFullLoadingScreen();
        }

        private async Task OnOpenLoadFromOldPermitDialog()
        {
            PermitNomenclatureDto permit = null;
            if (FishingGearType.Code == FishingGearTypes[0].Code)
            {
                if (ShipData.Ship.Value != null)
                {
                    permit = await TLDialogHelper.ShowDialog(new LoadFromOldPermitsDialog(null, ShipData.Ship.Value.Uid));
                }
            }
            else
            {
                if (PoundNet.Value != null)
                {
                    permit = await TLDialogHelper.ShowDialog(new LoadFromOldPermitsDialog(PoundNet.Value, null));
                }
            }


            if (permit != null)
            {
                chosenOldPermit = permit.Id;
                chosenOldPermitYear = permit.From.Year;
                OnPermitChosen(permit, false);
            }
            else
            {
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingGear) + "/NoShipOrPoundNetSelected"], App.GetResource<Color>("ErrorColor"));
            }
        }

        private void OnCheckReasonChosen(SelectNomenclatureDto dto)
        {
            IsPermitTypeEditable = dto.Code == "Recheck";
            PermitType = dto.Code == "New" ? PermitTypes[1] : PermitTypes[0];
            ResetPermitData();
        }

        private void OnPermitTypeChosen()
        {
            ResetPermitData();
        }

        private void ResetPermitData()
        {
            FishingGears.Reset();
            Permit.Value = null;
            PermitNumber.Value = "";
            PermitYear.Value = "";
            chosenOldPermit = null;
            chosenOldPermitYear = null;
        }

        private void OnFishingGearTypeSwitched()
        {
            Owner.Person.Value = null;
            Owner.People = new List<ShipPersonnelDto>();
            Permit.Value = null;
            PermitNumber.Value = "";
            PermitYear.Value = "";
            chosenOldPermit = null;
            chosenOldPermitYear = null;
            Permit.ItemsSource.Clear();
            FishingGears.FishingGears.Value.Clear();
        }

        private Task OnShipChosen(ShipSelectNomenclatureDto nomenclatureDto)
        {
            ShipDto chosenShip = NomenclaturesTransaction.GetShip(nomenclatureDto.Id);

            if (chosenShip == null)
            {
                return Task.CompletedTask;
            }

            PermitNumber.Value = "";
            PermitYear.Value = "";
            chosenOldPermit = null;
            chosenOldPermitYear = null;
            Permit.Value = null;
            FishingGears.FishingGears.Value.Clear();
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

            return Task.CompletedTask;
        }

        private void OnPoundNetChosen(SelectNomenclatureDto nomenclatureDto)
        {
            PermitNumber.Value = "";
            PermitYear.Value = "";
            chosenOldPermit = null;
            chosenOldPermitYear = null;
            Permit.Value = null;
            FishingGears.FishingGears.Value.Clear();
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

        private void OnPermitChosen(SelectNomenclatureDto nom, bool areFishingRegister = true)
        {
            FishingGears.FishingGears.Value.Clear();

            if (FishingGearType.Code == FishingPoundNet)
            {
                if (PoundNet.Value != null)
                {
                    List<FishingGearDto> fishingGears = InspectionsTransaction.GetFishingGearsForPoundNet(PoundNet.Value, nom.Id);
                    LoadFishingGears(fishingGears, areFishingRegister);
                }
            }
            else
            {
                if (ShipData.ShipInRegister.Value && ShipData.Ship.Value != null)
                {
                    List<FishingGearDto> fishingGears = InspectionsTransaction.GetFishingGearsForShip(ShipData.Ship.Value.Uid, nom.Id);
                    LoadFishingGears(fishingGears, areFishingRegister);
                }
            }
        }

        private void LoadFishingGears(List<FishingGearDto> fishingGears, bool isRegister)
        {
            List<SelectNomenclatureDto> fishingGearTypes = NomenclaturesTransaction.GetFishingGears().Select(x => new SelectNomenclatureDto()
            {
                Code = x.Code,
                Id = x.Id,
                Name = x.Name
            }).ToList();

            List<FishingGearModel> models = new List<FishingGearModel>();
            if (isRegister)
            {
                models = fishingGears.ConvertAll(f => new FishingGearModel
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
            }
            else
            {
                fishingGears.ForEach(x => x.Id = null);
                models = fishingGears.ConvertAll(f => new FishingGearModel
                {
                    Count = f.Count,
                    NetEyeSize = f.NetEyeSize,
                    Marks = string.Join(", ", f.Marks.Select(s => s.FullNumber?.ToString())),
                    Type = fishingGearTypes.Find(s => s.Id == f.TypeId) ?? fishingGearTypes[0],
                    CheckedValue = InspectedFishingGearEnum.I,
                    IsAddedByInspector = true,
                    Dto = new InspectedFishingGearDto
                    {
                        InspectedFishingGear = f
                    },
                });
            }


            FishingGears.AllFishingGears.Clear();
            FishingGears.AllFishingGears.AddRange(models);

            FishingGears.FishingGears.Value.ReplaceRange(models);
        }

        private Task OnSaveDraft()
        {
            return Save(ActivityType == ViewActivityType.Edit ? SubmitType.Edit : SubmitType.Draft);
        }

        private Task OnFinish()
        {
            return InspectionSaveHelper.Finish(Sections, Validation, Save);
        }

        private Task Save(SubmitType submitType)
        {
            return this.Save(Edit,
                InspectionFiles,
                async (inspectionIdentifier, files) =>
                {
                    InspectionCheckToolMarkDto dto = new InspectionCheckToolMarkDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = InspectionGeneralInfo.BuildReportNum(),
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit || submitType == SubmitType.ReturnForEdit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.IGM,
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.ViolatedRegulations.HasViolations,
                        InspectorComment = AdditionalInfo.InspectorComment,
                        ByEmergencySignal = InspectionGeneralInfo.ByEmergencySignal,
                        EndDate = InspectionGeneralInfo.EndDate,
                        StartDate = InspectionGeneralInfo.StartDate,
                        Inspectors = InspectionGeneralInfo.Inspectors,
                        IsOfflineOnly = IsLocal,
                        FishingGears = FishingGears,
                        PermitId = PermitType?.Code == PermitRegistered ? Permit.Value : null,
                        CheckReasonId = CheckReason.Value,
                        RecheckReasonId = RecheckReason.Value,
                        OwnerComment = OwnerComment,
                        Port = LastHarbour,
                        PoundNetId = PoundNet.Value,
                        UnregisteredPermitNumber = PermitNumber.Value,
                        UnregisteredPermitYear = ParseHelper.ParseInteger(PermitYear.Value),
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
                        PermitLicenseRegisterId = chosenOldPermit,
                        PermitLicenseYear = chosenOldPermitYear,
                        ViolatedRegulations = AdditionalInfo.ViolatedRegulations.ViolatedRegulations.Value.Select(x => (AuanViolatedRegulationDto)x).ToList(),
                        IsActive = true,
                    };

                    if (FishingGearType.Code == FishingPoundNet)
                    {
                        dto.PoundNetId = PoundNet.Value;
                    }
                    else
                    {
                        dto.InspectedShip = ShipData;
                    }
                    List<FileModel> signatures = null;
                    if (submitType == SubmitType.Finish)
                    {
                        signatures = await InspectionSaveHelper.GetSignatures(dto.Inspectors, DefaultInspecterPerson);
                    }
                    return await InspectionsTransaction.HandleInspection(dto, submitType, signatures);
                }
            );
        }
    }
}
