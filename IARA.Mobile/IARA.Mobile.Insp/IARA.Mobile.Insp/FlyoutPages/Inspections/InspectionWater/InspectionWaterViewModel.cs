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
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.OffenderDialog;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Insp.ViewModels.Models;
using IARA.Mobile.Shared.Views;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.InspectionWater
{
    public class InspectionWaterViewModel : InspectionPageViewModel
    {
        private List<SelectNomenclatureDto> _waterTypes;
        private InspectionCheckWaterObjectDto _edit;

        public InspectionWaterViewModel()
        {
            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);
            Finish = CommandBuilder.CreateFrom(OnFinish);

            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this);
            PatrolVehicles = new PatrolVehiclesViewModel(this, null);
            FishingGears = new WaterFishingGearsViewModel(this);
            Vessels = new WaterVesselsViewModel(this);
            Engines = new EnginesViewModel(this);
            Catches = new WaterCatchesViewModel(this);
            InspectionFiles = new InspectionFilesViewModel(this);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this);

            AddOffender = CommandBuilder.CreateFrom(OnAddOffender);
            ViewOffender = CommandBuilder.CreateFrom<InspectionSubjectPersonnelDto>(OnViewOffender);
            EditOffender = CommandBuilder.CreateFrom<InspectionSubjectPersonnelDto>(OnEditOffender);
            RemoveOffender = CommandBuilder.CreateFrom<InspectionSubjectPersonnelDto>(OnRemoveOffender);

            this.AddValidation(others: new IValidatableViewModel[]
            {
                InspectionGeneralInfo,
                PatrolVehicles,
                FishingGears,
                Vessels,
                Engines,
                Catches,
                InspectionFiles,
                AdditionalInfo,
                Signatures,
            });
        }
        public InspectionCheckWaterObjectDto Edit
        {
            get => _edit;
            set
            {
                _edit = value;
                ProtectedEdit = value;
            }
        }

        public TLForwardSections Sections { get; set; }

        public InspectionGeneralInfoViewModel InspectionGeneralInfo { get; }
        public PatrolVehiclesViewModel PatrolVehicles { get; }
        public WaterFishingGearsViewModel FishingGears { get; }
        public WaterVesselsViewModel Vessels { get; }
        public EnginesViewModel Engines { get; }
        public WaterCatchesViewModel Catches { get; }
        public InspectionFilesViewModel InspectionFiles { get; }
        public AdditionalInfoViewModel AdditionalInfo { get; }
        public SignaturesViewModel Signatures { get; }
        public List<SelectNomenclatureDto> Counties { get; set; }

        public ValidStateValidatableTable<ToggleViewModel> Toggles { get; set; }

        [Required]
        [MaxLength(500)]
        public ValidState ObjectName { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> WaterType { get; set; }

        [Required]
        public ValidStateLocation Location { get; set; }

        public ValidStateBool HasOffenders { get; set; }

        public ValidStateTable<InspectionSubjectPersonnelDto> Offenders { get; set; }

        public ICommand AddOffender { get; set; }
        public ICommand ViewOffender { get; set; }
        public ICommand EditOffender { get; set; }
        public ICommand RemoveOffender { get; set; }

        public List<SelectNomenclatureDto> WaterTypes
        {
            get => _waterTypes;
            set => SetProperty(ref _waterTypes, value);
        }

        public override void OnDisappearing()
        {
            GlobalVariables.IsAddingInspection = false;
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[]
            {
                GroupResourceEnum.WaterFishingGear,
                GroupResourceEnum.PatrolVehicle,
                GroupResourceEnum.Engine,
                GroupResourceEnum.WaterVessel,
                GroupResourceEnum.WaterCatch,
                GroupResourceEnum.InspectionWater,
                GroupResourceEnum.Validation,
                GroupResourceEnum.FishingGear,
            };
        }

        public override async Task Initialize(object sender)
        {
            InspectionHelper.Initialize(this, Edit);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            Counties = nomTransaction.GetCountries();
            List<InspectionCheckTypeDto> checkTypes = nomTransaction.GetInspectionCheckTypes(InspectionType.CWO);

            await InspectionGeneralInfo.Init();
            InspectionFiles.Init(nomTransaction.GetFileTypes(Constants.InspectionFileTypeCode));

            WaterTypes = nomTransaction.GetWaterBodyTypes();

            Toggles.Value.AddRange(
                checkTypes.ConvertAll(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
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

                InspectionState = Edit.InspectionState;

                await InspectionGeneralInfo.OnEdit(Edit);
                PatrolVehicles.OnEdit(Edit.PatrolVehicles);
                InspectionFiles.OnEdit(Edit);
                Signatures.OnEdit(Edit.Files, fileTypes);
                AdditionalInfo.OnEdit(Edit);

                Toggles.AssignFrom(Edit.Checks);

                if (Edit.FishingGears?.Count > 0)
                {
                    List<SelectNomenclatureDto> fishingGearTypes = nomTransaction.GetFishingGears().Select(x => new SelectNomenclatureDto()
                    {
                        Code = x.Code,
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();

                    foreach (WaterInspectionFishingGearDto fishingGear in Edit.FishingGears)
                    {
                        FishingGears.FishingGears.Value.Add(new WaterFishingGearModel
                        {
                            Type = fishingGearTypes.Find(f => f.Id == fishingGear.TypeId) ?? fishingGearTypes[0],
                            Marks = fishingGear.Marks == null
                                ? string.Empty
                                : string.Join(", ", fishingGear.Marks.Select(f => f.FullNumber?.ToString()).Where(f => !string.IsNullOrEmpty(f))),
                            Dto = fishingGear,
                        });
                    }
                }

                if (Edit.Vessels?.Count > 0)
                {
                    Vessels.Vessels.Value.AddRange(Edit.Vessels);
                }

                if (Edit.Engines?.Count > 0)
                {
                    Engines.Engines.Value.AddRange(Edit.Engines);
                }

                if (Edit.Catches?.Count > 0)
                {
                    List<SelectNomenclatureDto> fishTypes = nomTransaction.GetFishes();

                    foreach (InspectionCatchMeasureDto catchMeasure in Edit.Catches)
                    {
                        Catches.CatchMeasures.Value.Add(new WaterCatchModel
                        {
                            FishName = fishTypes.Find(f => f.Id == catchMeasure.FishId)?.DisplayValue,
                            Dto = catchMeasure,
                        });
                    }
                }

                if (Edit.Personnel?.Count > 0)
                {
                    HasOffenders.Value = true;
                    Offenders.Value.AddRange(Edit.Personnel.Where(x => x.Type == InspectedPersonType.Offender).ToList());
                }

                ObjectName.AssignFrom(Edit.ObjectName);
                WaterType.AssignFrom(Edit.WaterObjectTypeId, WaterTypes);
                Location.AssignFrom(Edit.WaterObjectLocation);
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
        private async Task OnRemoveOffender(InspectionSubjectPersonnelDto dto)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                Offenders.Value.Remove(dto);
            }
        }

        private async Task OnEditOffender(InspectionSubjectPersonnelDto dto)
        {
            InspectionSubjectPersonnelDto offender = await TLDialogHelper.ShowDialog(new OffenderDialog(this, Counties, ViewActivityType.Edit, dto));
            if (offender != null)
            {
                dto.Id = offender.Id;
                dto.FirstName = offender.FirstName;
                dto.MiddleName = offender.MiddleName;
                dto.LastName = offender.LastName;
                dto.Address = offender.Address;
                dto.HasBulgarianAddressRegistration = offender.HasBulgarianAddressRegistration;
                dto.EgnLnc = offender.EgnLnc;
                dto.Eik = offender.Eik;
                dto.IsLegal = offender.IsLegal;
                dto.CitizenshipId = offender.CitizenshipId;
                dto.Comment = offender.Comment;
                dto.IsActive = offender.IsActive;
                dto.IsRegistered = offender.IsRegistered;
                dto.EntryId = offender.EntryId;
                dto.Type = offender.Type;
                dto.RegisteredAddress = offender.RegisteredAddress;

                Offenders.Value.Replace(dto, offender);
            }
        }

        private async Task OnViewOffender(InspectionSubjectPersonnelDto dto)
        {
            await TLDialogHelper.ShowDialog(new OffenderDialog(this, Counties, ViewActivityType.Review, dto));
        }

        private async Task OnAddOffender()
        {
            InspectionSubjectPersonnelDto offender = await TLDialogHelper.ShowDialog(new OffenderDialog(this, Counties, ViewActivityType.Add, null));
            if (offender != null)
            {
                Offenders.Value.Add(offender);
            }
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
                    InspectionCheckWaterObjectDto dto = new InspectionCheckWaterObjectDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = InspectionGeneralInfo.BuildReportNum(),
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit || submitType == SubmitType.ReturnForEdit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.CWO,
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.ViolatedRegulations.HasViolations,
                        InspectorComment = AdditionalInfo.InspectorComment,
                        ByEmergencySignal = InspectionGeneralInfo.ByEmergencySignal,
                        EndDate = InspectionGeneralInfo.EndDate,
                        StartDate = InspectionGeneralInfo.StartDate,
                        Inspectors = InspectionGeneralInfo.Inspectors,
                        IsOfflineOnly = IsLocal,
                        WaterObjectLocation = Location,
                        WaterObjectTypeId = WaterType.Value,
                        FishingGears = FishingGears,
                        Vessels = Vessels,
                        Engines = Engines,
                        PatrolVehicles = PatrolVehicles,
                        ObjectName = ObjectName,
                        Checks = Toggles
                            .Select(f => (InspectionCheckDto)f)
                            .Where(f => f != null)
                            .ToList(),
                        ObservationTexts = new InspectionObservationTextDto[]
                        {
                            AdditionalInfo.ObservationsOrViolations,
                        }.Where(f => !string.IsNullOrWhiteSpace(f.Text)).ToList(),
                        Catches = Catches,
                        ViolatedRegulations = AdditionalInfo.ViolatedRegulations.ViolatedRegulations.Value.Select(x => (AuanViolatedRegulationDto)x).ToList(),
                        Personnel = HasOffenders.Value ? Offenders.Value.ToList() : null,
                    };
                    List<FileModel> signatures = null;
                    if (submitType == SubmitType.Finish)
                    {
                        signatures = await InspectionSaveHelper.GetSignatures(dto.Inspectors, HasOffenders.Value ? Offenders.Value.ToList() : null);
                    }
                    return await InspectionsTransaction.HandleInspection(dto, submitType, signatures);
                }
            );
        }
    }
}
