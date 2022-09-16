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
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
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
            PatrolVehicles = new PatrolVehiclesViewModel(this, false);
            FishingGears = new WaterFishingGearsViewModel(this);
            Vessels = new WaterVesselsViewModel(this);
            Engines = new EnginesViewModel(this);
            Catches = new WaterCatchesViewModel(this);
            InspectionFiles = new InspectionFilesViewModel(this);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this);

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

        public InspectionGeneralInfoViewModel InspectionGeneralInfo { get; }
        public PatrolVehiclesViewModel PatrolVehicles { get; }
        public WaterFishingGearsViewModel FishingGears { get; }
        public WaterVesselsViewModel Vessels { get; }
        public EnginesViewModel Engines { get; }
        public WaterCatchesViewModel Catches { get; }
        public InspectionFilesViewModel InspectionFiles { get; }
        public AdditionalInfoViewModel AdditionalInfo { get; }
        public SignaturesViewModel Signatures { get; }

        public ValidStateValidatableTable<ToggleViewModel> Toggles { get; set; }

        [MaxLength(500)]
        public ValidState ObjectName { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> WaterType { get; set; }

        [Required]
        public ValidStateLocation Location { get; set; }

        public List<SelectNomenclatureDto> WaterTypes
        {
            get => _waterTypes;
            set => SetProperty(ref _waterTypes, value);
        }

        public Action ExpandAll { get; set; }

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
            };
        }

        public override async Task Initialize(object sender)
        {
            InspectionHelper.Initialize(this, Edit);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();
            List<InspectionCheckTypeDto> checkTypes = nomTransaction.GetInspectionCheckTypes(InspectionType.CWO);

            InspectionGeneralInfo.Init();
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

                InspectionGeneralInfo.OnEdit(Edit);
                PatrolVehicles.OnEdit(Edit.PatrolVehicles);
                InspectionFiles.OnEdit(Edit);
                Signatures.OnEdit(Edit.Files, fileTypes);
                AdditionalInfo.OnEdit(Edit);

                Toggles.AssignFrom(Edit.Checks);

                if (Edit.FishingGears?.Count > 0)
                {
                    List<SelectNomenclatureDto> fishingGearTypes = nomTransaction.GetFishingGears();

                    foreach (WaterInspectionFishingGearDto fishingGear in Edit.FishingGears)
                    {
                        FishingGears.FishingGears.Value.Add(new WaterFishingGearModel
                        {
                            Type = fishingGearTypes.Find(f => f.Id == fishingGear.TypeId) ?? fishingGearTypes[0],
                            Marks = fishingGear.Marks == null
                                ? string.Empty
                                : string.Join(", ", fishingGear.Marks.Select(f => f.Number).Where(f => !string.IsNullOrEmpty(f))),
                            Dto = fishingGear,
                        });
                    }
                }

                if (Edit.Vessels?.Count > 0)
                {
                    foreach (WaterInspectionVesselDto vessel in Edit.Vessels)
                    {
                        Vessels.Vessels.Value.Add(new WaterVesselModel
                        {
                            Dto = vessel,
                        });
                    }
                }

                if (Edit.Engines?.Count > 0)
                {
                    foreach (WaterInspectionEngineDto engine in Edit.Engines)
                    {
                        Engines.Engines.Value.Add(new EngineModel
                        {
                            Dto = engine,
                        });
                    }
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

                ObjectName.AssignFrom(Edit.ObjectName);
                WaterType.AssignFrom(Edit.WaterObjectTypeId, WaterTypes);
                Location.AssignFrom(Edit.WaterObjectLocation);
            }

            if (ActivityType == ViewActivityType.Review)
            {
                ExpandAll();
            }

            await TLLoadingHelper.HideFullLoadingScreen();
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
                    InspectionCheckWaterObjectDto dto = new InspectionCheckWaterObjectDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = Edit?.ReportNum,
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.CWO,
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.AdministrativeViolation,
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
                    };

                    return InspectionsTransaction.HandleInspection(dto, submitType);
                }
            );
        }
    }
}
