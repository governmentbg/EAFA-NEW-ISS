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

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.TranshipmentInspection
{
    public class TranshipmentInspectionViewModel : InspectionPageViewModel
    {
        private InspectionTransboardingDto _edit;

        public TranshipmentInspectionViewModel()
        {
            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);
            Finish = CommandBuilder.CreateFrom(OnFinish);

            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this);
            PatrolVehicles = new PatrolVehiclesViewModel(this, true);
            InspectedShip = new FishingShipViewModel(this);
            InspectedShipChecks = new ShipChecksViewModel(this);
            InspectedShipCatches = new ShipCatchesViewModel(this);
            AcceptingShip = new FishingShipViewModel(this);
            AcceptingShipChecks = new ShipChecksViewModel(this);
            AcceptingShipCatches = new ShipCatchesViewModel(this);
            TranshippedCatches = new CatchInspectionsViewModel(this);
            InspectionFiles = new InspectionFilesViewModel(this);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this);

            this.AddValidation(others: new IValidatableViewModel[]
            {
                InspectionGeneralInfo,
                PatrolVehicles,
                InspectedShip,
                InspectedShipChecks,
                InspectedShipCatches,
                AcceptingShip,
                AcceptingShipChecks,
                AcceptingShipCatches,
                TranshippedCatches,
                InspectionFiles,
                AdditionalInfo,
                Signatures,
            });

            TranshippedCatchObservation.Category = InspectionObservationCategory.TransshippedCatch;

            InspectedShip.ObservationsOrViolations.Category = InspectionObservationCategory.InspectedShipData;
            InspectedShipChecks.ObservationsOrViolations.Category = InspectionObservationCategory.InspectedShipCheck;
            InspectedShipCatches.ObservationsOrViolations.Category = InspectionObservationCategory.InspectedShipCatch;

            AcceptingShip.ObservationsOrViolations.Category = InspectionObservationCategory.AcceptingShipData;
            AcceptingShipChecks.ObservationsOrViolations.Category = InspectionObservationCategory.AcceptingShipCheck;
            AcceptingShipCatches.ObservationsOrViolations.Category = InspectionObservationCategory.AcceptingShipCatch;
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
        public PatrolVehiclesViewModel PatrolVehicles { get; }
        public FishingShipViewModel InspectedShip { get; }
        public ShipChecksViewModel InspectedShipChecks { get; }
        public ShipCatchesViewModel InspectedShipCatches { get; }
        public FishingShipViewModel AcceptingShip { get; }
        public ShipChecksViewModel AcceptingShipChecks { get; }
        public ShipCatchesViewModel AcceptingShipCatches { get; }
        public CatchInspectionsViewModel TranshippedCatches { get; }
        public InspectionFilesViewModel InspectionFiles { get; }
        public AdditionalInfoViewModel AdditionalInfo { get; }
        public SignaturesViewModel Signatures { get; }

        [MaxLength(4000)]
        public ValidStateObservation TranshippedCatchObservation { get; set; }

        [MaxLength(4000)]
        public ValidState InspectedShipCaptainComment { get; set; }

        [MaxLength(4000)]
        public ValidState AcceptingShipCaptainComment { get; set; }

        public Action ExpandAll { get; set; }

        public override void OnDisappearing()
        {
            GlobalVariables.IsAddingInspection = false;
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[]
            {
                GroupResourceEnum.FishingShip,
                GroupResourceEnum.ShipChecks,
                GroupResourceEnum.CatchInspection,
                GroupResourceEnum.FishingGear,
                GroupResourceEnum.InspectedPerson,
                GroupResourceEnum.InspectedShipData,
                GroupResourceEnum.PatrolVehicle,
                GroupResourceEnum.TranshipmentInspection,
                GroupResourceEnum.Validation,
            };
        }

        public override async Task Initialize(object sender)
        {
            InspectionHelper.Initialize(this, Edit);
            InspectionHelper.InitShip(InspectedShip, InspectedShipChecks);
            InspectionHelper.InitShip(AcceptingShip, AcceptingShipChecks);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();
            List<SelectNomenclatureDto> vesselTypes = nomTransaction.GetVesselTypes();
            List<InspectionCheckTypeDto> checkTypes = nomTransaction.GetInspectionCheckTypes(InspectionType.ITB);
            List<SelectNomenclatureDto> fishTypes = nomTransaction.GetFishes();
            List<SelectNomenclatureDto> catchTypes = nomTransaction.GetCatchInspectionTypes();
            List<CatchZoneNomenclatureDto> catchAreas = nomTransaction.GetCatchZones();
            List<SelectNomenclatureDto> associations = nomTransaction.GetAssociations();
            List<SelectNomenclatureDto> turbotSizeGroups = nomTransaction.GetTurbotSizeGroups();

            InspectionGeneralInfo.Init();
            InspectedShip.Init(countries, vesselTypes, catchAreas);
            InspectedShipCatches.Init(fishTypes, catchTypes, catchAreas, turbotSizeGroups);
            AcceptingShip.Init(countries, vesselTypes, catchAreas);
            AcceptingShipCatches.Init(fishTypes, catchTypes, catchAreas, turbotSizeGroups);
            TranshippedCatches.Init(fishTypes, catchTypes, catchAreas, turbotSizeGroups, null);

            InspectedShip.Toggles.Value.AddRange(checkTypes
                .Where(f => f.Code.StartsWith(Constants.ShipBase) && !f.Code.StartsWith(Constants.ObjectCheckBase))
                .Select(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
                {
                    CheckTypeId = f.Id,
                    Text = f.Name,
                    Type = f.Type,
                    DescriptionLabel = f.DescriptionLabel,
                }).ToList()
            );

            InspectedShipChecks.Toggles.Value.AddRange(checkTypes
                .Where(f => !f.Code.StartsWith(Constants.ShipBase) && !f.Code.StartsWith(Constants.ObjectCheckBase))
                .Select(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
                {
                    CheckTypeId = f.Id,
                    Text = f.Name,
                    Type = f.Type,
                    DescriptionLabel = f.DescriptionLabel,
                }).ToList()
            );

            AcceptingShip.Toggles.Value.AddRange(checkTypes
                .Where(f => f.Code.StartsWith(Constants.ShipBase) && !f.Code.StartsWith(Constants.ObjectCheckBase))
                .Select(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
                {
                    CheckTypeId = f.Id,
                    Text = f.Name,
                    Type = f.Type,
                    DescriptionLabel = f.DescriptionLabel,
                }).ToList()
            );

            AcceptingShipChecks.Toggles.Value.AddRange(checkTypes
                .Where(f => !f.Code.StartsWith(Constants.ShipBase) && !f.Code.StartsWith(Constants.ObjectCheckBase))
                .Select(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
                {
                    CheckTypeId = f.Id,
                    Text = f.Name,
                    Type = f.Type,
                    DescriptionLabel = f.DescriptionLabel,
                }).ToList()
            );

            InspectedShipChecks.Init(checkTypes, associations);
            AcceptingShipChecks.Init(checkTypes, associations);

            InspectionFiles.Init(nomTransaction.GetFileTypes(Constants.InspectionFileTypeCode));

            if (Edit != null)
            {
                List<SelectNomenclatureDto> fileTypes = nomTransaction.GetFileTypes();

                InspectionState = Edit.InspectionState;

                InspectionGeneralInfo.OnEdit(Edit);
                InspectionFiles.OnEdit(Edit);
                Signatures.OnEdit(Edit.Files, fileTypes);
                PatrolVehicles.OnEdit(Edit.PatrolVehicles);
                TranshippedCatches.OnEdit(Edit.TransboardedCatchMeasures);
                AdditionalInfo.OnEdit(Edit);

                if (Edit.SendingShipInspection != null)
                {
                    InspectedShip.OnEdit(Edit.SendingShipInspection);
                    InspectedShipCatches.OnEdit(Edit.SendingShipInspection);
                    InspectedShipCaptainComment.AssignFrom(Edit.SendingShipInspection.CaptainComment);

                    await InspectedShipChecks.OnEdit(Edit.SendingShipInspection, Edit.SendingShipInspection.Checks);
                }

                if (Edit.ReceivingShipInspection != null)
                {
                    InspectedShip.OnEdit(Edit.ReceivingShipInspection);
                    InspectedShipCatches.OnEdit(Edit.ReceivingShipInspection);
                    AcceptingShipCaptainComment.AssignFrom(Edit.ReceivingShipInspection.CaptainComment);

                    await AcceptingShipChecks.OnEdit(Edit.ReceivingShipInspection, Edit.ReceivingShipInspection.Checks);
                }

                InspectedShip.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                InspectedShipChecks.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                InspectedShipCatches.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);

                AcceptingShip.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                AcceptingShipChecks.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                AcceptingShipCatches.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);

                TranshippedCatchObservation.AssignFrom(Edit.ObservationTexts);
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
                        InspectionType = InspectionType.ITB,
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.AdministrativeViolation,
                        InspectorComment = AdditionalInfo.InspectorComment,
                        ByEmergencySignal = InspectionGeneralInfo.ByEmergencySignal,
                        EndDate = InspectionGeneralInfo.EndDate,
                        StartDate = InspectionGeneralInfo.StartDate,
                        Inspectors = InspectionGeneralInfo.Inspectors,
                        SendingShipInspection = new InspectionTransboardingShipDto
                        {
                            InspectedShip = InspectedShip.ShipData,
                            CaptainComment = InspectedShipCaptainComment,
                            CatchMeasures = InspectedShipCatches.Catches,
                            Personnel = InspectedShip,
                            LogBooks = InspectedShipChecks.LogBooks,
                            PermitLicenses = InspectedShipChecks.PermitLicenses,
                            Permits = InspectedShipChecks.Permits,
                            Checks = InspectedShipChecks.Toggles
                                .Select(f => (InspectionCheckDto)f)
                                .Where(f => f != null)
                                .Concat(InspectedShipCatches.Toggles
                                    .Select(f => (InspectionCheckDto)f)
                                    .Where(f => f != null)
                                )
                                .Concat((List<InspectionCheckDto>)InspectedShipChecks)
                                .ToList(),
                        },
                        ReceivingShipInspection = new InspectionTransboardingShipDto
                        {
                            InspectedShip = AcceptingShip.ShipData,
                            CaptainComment = AcceptingShipCaptainComment,
                            CatchMeasures = AcceptingShipCatches.Catches,
                            Personnel = AcceptingShip,
                            LogBooks = AcceptingShipChecks.LogBooks,
                            PermitLicenses = AcceptingShipChecks.PermitLicenses,
                            Permits = AcceptingShipChecks.Permits,
                            Checks = AcceptingShipChecks.Toggles
                                .Select(f => (InspectionCheckDto)f)
                                .Where(f => f != null)
                                .Concat(AcceptingShipCatches.Toggles
                                    .Select(f => (InspectionCheckDto)f)
                                    .Where(f => f != null)
                                )
                                .Concat((List<InspectionCheckDto>)AcceptingShipChecks)
                                .ToList(),
                        },
                        TransboardedCatchMeasures = TranshippedCatches,
                        IsOfflineOnly = IsLocal,
                        PatrolVehicles = PatrolVehicles,
                        ObservationTexts = new InspectionObservationTextDto[]
                        {
                            AdditionalInfo.ObservationsOrViolations,
                            InspectedShip.ObservationsOrViolations,
                            InspectedShipChecks.ObservationsOrViolations,
                            InspectedShipCatches.ObservationsOrViolations,
                            AcceptingShip.ObservationsOrViolations,
                            AcceptingShipChecks.ObservationsOrViolations,
                            AcceptingShipCatches.ObservationsOrViolations,
                            TranshippedCatchObservation,
                        }.Where(f => !string.IsNullOrWhiteSpace(f.Text)).ToList(),
                    };

                    return InspectionsTransaction.HandleInspection(dto, submitType);
                }
            );
        }
    }
}
