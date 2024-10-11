using IARA.Mobile.Application;
using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Shared.Views;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.BoatOnOpenWater
{
    public class BoatOnOpenWaterViewModel : InspectionPageViewModel
    {
        private List<DescrSelectNomenclatureDto> _onBoardObservationTools;
        private List<DescrSelectNomenclatureDto> _centerObservationTools;
        private List<DescrSelectNomenclatureDto> _observedVesselActivities;
        private List<DescrSelectNomenclatureDto> _fishingObservedVesselActivities;
        private ObservationAtSeaDto _edit;

        public BoatOnOpenWaterViewModel()
        {
            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this);
            PatrolVehicles = new PatrolVehiclesViewModel(this, null);
            ObservedVessel = new InspectedShipDataViewModel(this)
            {
                ShipSelected = CommandBuilder.CreateFrom<ShipSelectNomenclatureDto>(OnShipSelected)
            };
            InspectionFiles = new InspectionFilesViewModel(this);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this, hasInspectedPerson: false);

            Finish = CommandBuilder.CreateFrom(OnFinish);
            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);

            this.AddValidation(others: new IValidatableViewModel[]
            {
                InspectionGeneralInfo,
                PatrolVehicles,
                ObservedVessel,
                InspectionFiles,
                AdditionalInfo,
                Signatures,
            });

            OtherCenterObservationTool.HasAsterisk = true;
            OtherOnBoardObservationTool.HasAsterisk = true;
            OtherFishingObservedVesselActivity.HasAsterisk = true;
        }

        public ObservationAtSeaDto Edit
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
        public InspectedShipDataViewModel ObservedVessel { get; }
        public InspectionFilesViewModel InspectionFiles { get; }
        public AdditionalInfoViewModel AdditionalInfo { get; }
        public SignaturesViewModel Signatures { get; }

        public ValidStateCheckList<DescrSelectNomenclatureDto> OnBoardObservationTool { get; set; }

        [MaxLength(100)]
        [RequiredIfHasDescription(nameof(OnBoardObservationTool), ErrorMessageResourceName = "Required")]
        public ValidState OtherOnBoardObservationTool { get; set; }

        [MaxLength(20)]
        public ValidState Course { get; set; }

        [TLRange(1d, 999.99)]
        public ValidState Speed { get; set; }

        public ValidStateCheckList<DescrSelectNomenclatureDto> CenterObservationTool { get; set; }

        [MaxLength(100)]
        [RequiredIfHasDescription(nameof(CenterObservationTool), ErrorMessageResourceName = "Required")]
        public ValidState OtherCenterObservationTool { get; set; }

        public ValidStateBool HasShipCommunication { get; set; }

        public ValidStateBool HasShipContact { get; set; }

        [Required]
        public ValidStateCheckList<DescrSelectNomenclatureDto> ObservedVesselActivity { get; set; }

        [MaxLength(100)]
        [RequiredIfHasDescription(nameof(ObservedVesselActivity), ErrorMessageResourceName = "Required")]
        public ValidState OtherObservedVesselActivity { get; set; }

        public ValidStateCheckList<DescrSelectNomenclatureDto> FishingObservedVesselActivity { get; set; }

        [MaxLength(100)]
        [RequiredIfFishingAndDesc(nameof(ObservedVesselActivity), nameof(FishingObservedVesselActivity), ErrorMessageResourceName = "Required")]
        public ValidState OtherFishingObservedVesselActivity { get; set; }

        [MaxLength(4000)]
        public ValidState ShipCommunicationDescription { get; set; }

        public List<DescrSelectNomenclatureDto> OnBoardObservationTools
        {
            get => _onBoardObservationTools;
            set => SetProperty(ref _onBoardObservationTools, value);
        }
        public List<DescrSelectNomenclatureDto> CenterObservationTools
        {
            get => _centerObservationTools;
            set => SetProperty(ref _centerObservationTools, value);
        }
        public List<DescrSelectNomenclatureDto> ObservedVesselActivities
        {
            get => _observedVesselActivities;
            set => SetProperty(ref _observedVesselActivities, value);
        }
        public List<DescrSelectNomenclatureDto> FishingObservedVesselActivities
        {
            get => _fishingObservedVesselActivities;
            set => SetProperty(ref _fishingObservedVesselActivities, value);
        }

        protected override string GetInspectionJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(Edit);
        }
        public override void OnDisappearing()
        {
            GlobalVariables.IsAddingInspection = false;
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[]
            {
                GroupResourceEnum.BoatOnOpenWater,
                GroupResourceEnum.PatrolVehicle,
                GroupResourceEnum.InspectedShipData,
                GroupResourceEnum.Validation,
            };
        }

        public override async Task Initialize(object sender)
        {
            InspectionHelper.Initialize(this, Edit);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;
            List<ObservationToolNomenclatureDto> obsTools = nomTransaction.GetObservationTools();

            CenterObservationTools = obsTools
                .Where(f => f.OnBoard == ObservationToolOnBoardEnum.Center
                    || f.OnBoard == ObservationToolOnBoardEnum.Both)
                .Select(f => f as DescrSelectNomenclatureDto)
                .ToList();

            OnBoardObservationTools = obsTools
                .Where(f => f.OnBoard == ObservationToolOnBoardEnum.OnBoard
                    || f.OnBoard == ObservationToolOnBoardEnum.Both)
                .Select(f => f as DescrSelectNomenclatureDto)
                .ToList();

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();

            await InspectionGeneralInfo.Init();

            ObservedVessel.Init(countries, nomTransaction.GetVesselTypes(), nomTransaction.GetCatchZones());

            InspectionFiles.Init(nomTransaction.GetFileTypes(Constants.InspectionFileTypeCode));

            List<VesselActivityDto> allVesselActivities = nomTransaction.GetVesselActivities();

            if (allVesselActivities != null)
            {
                ObservedVesselActivities = allVesselActivities
                    .Where(f => !f.IsFishingActivity)
                    .Select(f => f as DescrSelectNomenclatureDto)
                    .ToList();

                FishingObservedVesselActivities = allVesselActivities
                    .Where(f => f.IsFishingActivity)
                    .Select(f => f as DescrSelectNomenclatureDto)
                    .ToList();
            }

            if (Edit != null)
            {
                List<SelectNomenclatureDto> fileTypes = nomTransaction.GetFileTypes();

                InspectionState = Edit.InspectionState;

                await InspectionGeneralInfo.OnEdit(Edit);
                InspectionFiles.OnEdit(Edit);
                ObservedVessel.OnEdit(Edit.ObservedVessel);
                Signatures.OnEdit(Edit.Files, fileTypes);
                PatrolVehicles.OnEdit(Edit.PatrolVehicles);
                AdditionalInfo.OnEdit(Edit);

                Speed.AssignFrom(Edit.Speed);
                Course.AssignFrom(Edit.Course);
                HasShipCommunication.AssignFrom(Edit.HasShipCommunication);
                HasShipContact.AssignFrom(Edit.HasShipContact);
                ShipCommunicationDescription.AssignFrom(Edit.ShipCommunicationDescription);

                if (Edit.ObservationTools?.Count > 0)
                {
                    CenterObservationTool.Value.AddRange(CenterObservationTools.FindAll(f => Edit.ObservationTools.Any(s => !s.IsOnBoard && s.ObservationToolId == f.Id)));
                    OnBoardObservationTool.Value.AddRange(OnBoardObservationTools.FindAll(f => Edit.ObservationTools.Any(s => s.IsOnBoard && s.ObservationToolId == f.Id)));

                    int otherCenterObservationToolId = CenterObservationTools.Find(f => f.Code == CommonConstants.NomenclatureOther).Id;
                    int otherOnBoardObservationToolId = OnBoardObservationTools.Find(f => f.Code == CommonConstants.NomenclatureOther).Id;

                    ObservationToolDto centerDesc = Edit.ObservationTools.Find(f => !f.IsOnBoard && f.ObservationToolId == otherCenterObservationToolId);
                    ObservationToolDto onBoardDesc = Edit.ObservationTools.Find(f => f.IsOnBoard && f.ObservationToolId == otherOnBoardObservationToolId);

                    if (centerDesc != null)
                    {
                        OtherCenterObservationTool.AssignFrom(centerDesc.Description);
                    }
                    if (onBoardDesc != null)
                    {
                        OtherOnBoardObservationTool.AssignFrom(onBoardDesc.Description);
                    }
                }

                if (Edit.ObservedVesselActivities?.Count > 0)
                {
                    ObservedVesselActivity.Value.AddRange(ObservedVesselActivities.FindAll(f => Edit.ObservedVesselActivities.Any(s => s.Value == f.Id)));

                    VesselActivityApiDto desc = Edit.ObservedVesselActivities.Find(f => f.Code == CommonConstants.NomenclatureOther);

                    if (desc != null)
                    {
                        OtherObservedVesselActivity.Value = desc.Description;
                    }

                    List<DescrSelectNomenclatureDto> fishingActivities = FishingObservedVesselActivities.FindAll(f => Edit.ObservedVesselActivities.Any(s => s.Value == f.Id));

                    if (fishingActivities.Count > 0)
                    {
                        FishingObservedVesselActivity.Value.AddRange(fishingActivities);

                        desc = Edit.ObservedVesselActivities.Find(f => f.Code == "OF");

                        if (desc != null)
                        {
                            OtherFishingObservedVesselActivity.Value = desc.Description;
                        }
                    }
                }
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

        private Task OnShipSelected(ShipSelectNomenclatureDto ship)
        {
            if (ActivityType == ViewActivityType.Add)
            {
                ShipDto chosenShip = NomenclaturesTransaction.GetShip(ship.Id);

                if (chosenShip == null)
                {
                    return Task.CompletedTask;
                }

                ObservedVessel.InspectedShip = chosenShip;

                ObservedVessel.CallSign.AssignFrom(chosenShip.CallSign);
                ObservedVessel.MMSI.AssignFrom(chosenShip.MMSI);
                ObservedVessel.CFR.AssignFrom(chosenShip.CFR);
                ObservedVessel.ExternalMarkings.AssignFrom(chosenShip.ExtMarkings);
                ObservedVessel.Name.AssignFrom(chosenShip.Name);
                ObservedVessel.UVI.AssignFrom(chosenShip.UVI);
                ObservedVessel.Flag.AssignFrom(chosenShip.FlagId, ObservedVessel.Flags);
                ObservedVessel.ShipType.AssignFrom(chosenShip.ShipTypeId, ObservedVessel.ShipTypes);
            }

            return Task.CompletedTask;
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
                    ObservationAtSeaDto dto = new ObservationAtSeaDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = InspectionGeneralInfo.BuildReportNum(),
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit || submitType == SubmitType.ReturnForEdit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.OFS,
                        ObservedVesselActivities = MapVesselActivities().ToList(),
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.ViolatedRegulations.HasViolations,
                        InspectorComment = AdditionalInfo.InspectorComment,
                        ByEmergencySignal = InspectionGeneralInfo.ByEmergencySignal,
                        Course = Course,
                        EndDate = InspectionGeneralInfo.EndDate,
                        HasShipCommunication = HasShipCommunication,
                        HasShipContact = HasShipContact,
                        StartDate = InspectionGeneralInfo.StartDate,
                        Speed = ParseHelper.ParseDecimal(Speed),
                        ShipCommunicationDescription = ShipCommunicationDescription,
                        ObservedVessel = ObservedVessel,
                        Inspectors = InspectionGeneralInfo.Inspectors,
                        PatrolVehicles = PatrolVehicles,
                        ObservationTools = MapObservationTools().ToList(),
                        IsOfflineOnly = IsLocal,
                        ObservationTexts = new InspectionObservationTextDto[]
                        {
                            AdditionalInfo.ObservationsOrViolations,
                        }.Where(f => !string.IsNullOrWhiteSpace(f.Text)).ToList(),
                        ViolatedRegulations = AdditionalInfo.ViolatedRegulations.ViolatedRegulations.Value.Select(x => (AuanViolatedRegulationDto)x).ToList(),
                        IsActive = true,
                    };
                    List<FileModel> signatures = null;
                    if (submitType == SubmitType.Finish)
                    {
                        signatures = await InspectionSaveHelper.GetSignatures(dto.Inspectors);
                    }
                    return await InspectionsTransaction.HandleInspection(dto, submitType, signatures);
                }
            );
        }

        private IEnumerable<ObservationToolDto> MapObservationTools()
        {
            foreach (DescrSelectNomenclatureDto obsTool in CenterObservationTool)
            {
                yield return new ObservationToolDto
                {
                    IsOnBoard = false,
                    ObservationToolId = obsTool.Id,
                    Description = obsTool.Code == CommonConstants.NomenclatureOther
                        ? OtherCenterObservationTool.Value
                        : null
                };
            }

            foreach (DescrSelectNomenclatureDto obsTool in OnBoardObservationTool)
            {
                yield return new ObservationToolDto
                {
                    IsOnBoard = true,
                    ObservationToolId = obsTool.Id,
                    Description = obsTool.Code == CommonConstants.NomenclatureOther
                        ? OtherOnBoardObservationTool.Value
                        : null
                };
            }
        }

        private IEnumerable<VesselActivityApiDto> MapVesselActivities()
        {
            foreach (DescrSelectNomenclatureDto activity in ObservedVesselActivity)
            {
                yield return new VesselActivityApiDto
                {
                    Value = activity.Id,
                    Code = activity.Code,
                    DisplayName = activity.Name,
                    IsFishingActivity = false,
                    Description = activity.Code == CommonConstants.NomenclatureOther
                        ? OtherObservedVesselActivity.Value
                        : null
                };
            }

            foreach (DescrSelectNomenclatureDto activity in FishingObservedVesselActivity)
            {
                yield return new VesselActivityApiDto
                {
                    Value = activity.Id,
                    Code = activity.Code,
                    DisplayName = activity.Name,
                    IsFishingActivity = true,
                    Description = activity.Code == "OF"
                        ? OtherFishingObservedVesselActivity.Value
                        : null
                };
            }
        }
    }
}
