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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.HarbourInspection
{
    public class HarbourInspectionViewModel : InspectionPageViewModel
    {
        private bool _hasTranshipment;
        private string _transhippedCatch;
        private List<SelectNomenclatureDto> _countries;
        private InspectionTransboardingDto _edit;

        public HarbourInspectionViewModel()
        {
            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this);
            InspectionHarbour = new InspectionHarbourViewModel(this);
            InspectedShip = new FishingShipViewModel(this, canPickLocation: false);
            ShipChecks = new ShipChecksViewModel(this);
            ShipCatches = new ShipCatchesViewModel(this);
            ShipFishingGears = new ShipFishingGearsViewModel(this);
            TransshippedShip = new InspectedShipDataViewModel(this, canPickLocation: false);
            InspectionFiles = new InspectionFilesViewModel(this);
            TransshippedCatches = new CatchInspectionsViewModel(this);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this);

            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);
            Finish = CommandBuilder.CreateFrom(OnFinish);

            this.AddValidation(
                groups: new Dictionary<string, Func<bool>>
                {
                    { Group.IS_REQUIRED, () => false }
                },
                others: new IValidatableViewModel[]
                {
                    InspectionGeneralInfo,
                    InspectionHarbour,
                    InspectedShip,
                    TransshippedShip,
                    TransshippedCatches,
                    InspectionFiles,
                    AdditionalInfo,
                    Signatures,
                }
            );

            TransshippedShip.Ship.Groups.Add(Group.IS_REQUIRED);

            TransshipmentObservation.Category = InspectionObservationCategory.Transshipment;
        }

        public InspectionTransboardingDto Edit
        {
            get => _edit;
            set
            {
                _edit = value;
                ProtectedEdit = value;
            }
        }

        public InspectionGeneralInfoViewModel InspectionGeneralInfo { get; }
        public InspectionHarbourViewModel InspectionHarbour { get; }
        public FishingShipViewModel InspectedShip { get; }
        public ShipChecksViewModel ShipChecks { get; }
        public ShipCatchesViewModel ShipCatches { get; }
        public ShipFishingGearsViewModel ShipFishingGears { get; }
        public InspectedShipDataViewModel TransshippedShip { get; }
        public CatchInspectionsViewModel TransshippedCatches { get; }
        public InspectionFilesViewModel InspectionFiles { get; }
        public AdditionalInfoViewModel AdditionalInfo { get; }
        public SignaturesViewModel Signatures { get; }

        [MaxLength(4000)]
        public ValidStateObservation TransshipmentObservation { get; set; }

        [MaxLength(4000)]
        public ValidState CaptainComment { get; set; }

        public bool HasTranshipment
        {
            get => _hasTranshipment;
            set => SetProperty(ref _hasTranshipment, value);
        }

        public string TranshippedCatch
        {
            get => _transhippedCatch;
            set => SetProperty(ref _transhippedCatch, value);
        }

        public List<SelectNomenclatureDto> Countries
        {
            get => _countries;
            set => SetProperty(ref _countries, value);
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
                GroupResourceEnum.ShipChecks,
                GroupResourceEnum.FishingShip,
                GroupResourceEnum.CatchInspection,
                GroupResourceEnum.FishingGear,
                GroupResourceEnum.InspectedPerson,
                GroupResourceEnum.InspectedShipData,
                GroupResourceEnum.PatrolVehicle,
                GroupResourceEnum.HarbourInspection,
                GroupResourceEnum.Validation,
            };
        }

        public override async Task Initialize(object sender)
        {
            InspectionHelper.Initialize(this, Edit);
            InspectionHelper.InitShip(InspectedShip, ShipChecks, ShipFishingGears.FishingGears);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();
            List<InspectionCheckTypeDto> checkTypes = nomTransaction.GetInspectionCheckTypes(InspectionType.IBP);
            List<SelectNomenclatureDto> fishTypes = nomTransaction.GetFishes();
            List<SelectNomenclatureDto> catchTypes = nomTransaction.GetCatchInspectionTypes();
            List<CatchZoneNomenclatureDto> catchZones = nomTransaction.GetCatchZones();
            List<SelectNomenclatureDto> turbotSizeGroups = nomTransaction.GetTurbotSizeGroups();

            InspectionGeneralInfo.Init();
            InspectedShip.Init(countries, nomTransaction.GetVesselTypes(), catchZones);
            ShipCatches.Init(fishTypes, catchTypes, catchZones, turbotSizeGroups);
            InspectionHarbour.Init(countries);
            TransshippedShip.Init(countries, nomTransaction.GetVesselTypes(), catchZones);
            TransshippedCatches.Init(fishTypes, catchTypes, catchZones, turbotSizeGroups, null);

            Countries = countries;

            ShipChecks.Toggles.Value.AddRange(
                checkTypes
                    .Where(f => !f.Code.StartsWith(Constants.CatchBase))
                    .Select(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
                    {
                        CheckTypeId = f.Id,
                        Text = f.Name,
                        Type = f.Type,
                        DescriptionLabel = f.DescriptionLabel,
                    })
                    .ToList()
            );

            ShipCatches.Toggles.Value.AddRange(
                checkTypes
                    .Where(f => f.Code.StartsWith(Constants.CatchBase))
                    .Select(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
                    {
                        CheckTypeId = f.Id,
                        Text = f.Name,
                        Type = f.Type,
                        DescriptionLabel = f.DescriptionLabel,
                    })
                    .ToList()
            );

            //ShipFishingGears.Toggles.Value.AddRange(
            //    checkTypes
            //        .Where(f => f.Code.StartsWith(Constants.FishingGearBase))
            //        .Select(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
            //        {
            //            CheckTypeId = f.Id,
            //            Text = f.Name,
            //            Type = f.Type,
            //            DescriptionLabel = f.DescriptionLabel,
            //        })
            //        .ToList()
            //);

            ShipChecks.Init(checkTypes, nomTransaction.GetAssociations());

            InspectionFiles.Init(nomTransaction.GetFileTypes(Constants.InspectionFileTypeCode));

            if (Edit != null)
            {
                List<SelectNomenclatureDto> fileTypes = nomTransaction.GetFileTypes();

                InspectionState = Edit.InspectionState;

                InspectionGeneralInfo.OnEdit(Edit);
                InspectionFiles.OnEdit(Edit);
                Signatures.OnEdit(Edit.Files, fileTypes);
                TransshippedCatches.OnEdit(Edit.TransboardedCatchMeasures);
                AdditionalInfo.OnEdit(Edit);

                if (Edit.ReceivingShipInspection != null)
                {
                    ShipFishingGears.FishingGears.OnEdit(Edit.FishingGears,
                        Edit.ReceivingShipInspection.PermitLicenses
                            .Where(f => f.CheckValue == CheckTypeEnum.Y || f.CheckValue == CheckTypeEnum.N)
                            .Select(f => f.PermitLicenseId.Value)
                            .ToList()
                    );
                    InspectedShip.OnEdit(Edit.ReceivingShipInspection);
                    ShipCatches.OnEdit(Edit.ReceivingShipInspection);
                    //ShipFishingGears.Toggles.AssignFrom(Edit.ReceivingShipInspection.Checks);
                    InspectionHarbour.OnEdit(Edit.ReceivingShipInspection.LastPortVisit);
                    CaptainComment.AssignFrom(Edit.ReceivingShipInspection.CaptainComment);

                    await ShipChecks.OnEdit(Edit.ReceivingShipInspection, Edit.ReceivingShipInspection.Checks);
                }

                if (Edit.SendingShipInspection?.InspectedShip != null)
                {
                    TransshippedShip.OnEdit(Edit.SendingShipInspection.InspectedShip);
                }

                InspectedShip.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                ShipChecks.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                ShipCatches.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                ShipFishingGears.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                TransshipmentObservation.AssignFrom(Edit.ObservationTexts);
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
                    InspectionTransboardingDto dto = new InspectionTransboardingDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = Edit?.ReportNum,
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.IBP,
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.AdministrativeViolation,
                        InspectorComment = AdditionalInfo.InspectorComment,
                        ByEmergencySignal = InspectionGeneralInfo.ByEmergencySignal,
                        EndDate = InspectionGeneralInfo.EndDate,
                        StartDate = InspectionGeneralInfo.StartDate,
                        Inspectors = InspectionGeneralInfo.Inspectors,
                        ReceivingShipInspection = new InspectionTransboardingShipDto
                        {
                            CaptainComment = CaptainComment,
                            InspectedShip = InspectedShip.ShipData,
                            CatchMeasures = ShipCatches.Catches,
                            Personnel = InspectedShip,
                            LastPortVisit = InspectionHarbour,
                            LogBooks = ShipChecks.LogBooks,
                            PermitLicenses = ShipChecks.PermitLicenses,
                            Permits = ShipChecks.Permits,
                            Checks = ShipChecks.Toggles
                                .Select(f => (InspectionCheckDto)f)
                                .Where(f => f != null)
                                .Concat(ShipCatches.Toggles
                                    .Select(f => (InspectionCheckDto)f)
                                    .Where(f => f != null)
                                )
                                //.Concat(ShipFishingGears.Toggles
                                //    .Select(f => (InspectionCheckDto)f)
                                //    .Where(f => f != null)
                                //)
                                .Concat((List<InspectionCheckDto>)ShipChecks)
                                .ToList(),
                        },
                        SendingShipInspection = new InspectionTransboardingShipDto
                        {
                            InspectedShip = TransshippedShip,
                        },
                        TransboardedCatchMeasures = TransshippedCatches,
                        FishingGears = ShipFishingGears.FishingGears,
                        IsOfflineOnly = IsLocal,
                        ObservationTexts = new InspectionObservationTextDto[]
                        {
                            AdditionalInfo.ObservationsOrViolations,
                            InspectedShip.ObservationsOrViolations,
                            ShipChecks.ObservationsOrViolations,
                            ShipCatches.ObservationsOrViolations,
                            ShipFishingGears.ObservationsOrViolations,
                            TransshipmentObservation,
                        }.Where(f => !string.IsNullOrWhiteSpace(f.Text)).ToList(),
                    };

                    return InspectionsTransaction.HandleInspection(dto, submitType);
                }
            );
        }
    }
}
