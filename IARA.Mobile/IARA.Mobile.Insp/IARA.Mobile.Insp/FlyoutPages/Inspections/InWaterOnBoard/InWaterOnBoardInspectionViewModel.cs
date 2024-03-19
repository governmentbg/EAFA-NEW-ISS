using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
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

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.InWaterOnBoard
{
    public class InWaterOnBoardInspectionViewModel : InspectionPageViewModel
    {
        private InspectionAtSeaDto _edit;

        public InWaterOnBoardInspectionViewModel()
        {
            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);
            Finish = CommandBuilder.CreateFrom(OnFinish);
            ReturnForEdit = CommandBuilder.CreateFrom(OnReturnForEdit);

            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this);
            PatrolVehicles = new PatrolVehiclesViewModel(this, null);
            InspectedShip = new FishingShipViewModel(this);
            ShipChecks = new ShipChecksViewModel(this);
            ShipCatches = new ShipCatchesViewModel(this);
            ShipFishingGears = new ShipFishingGearsViewModel(this);
            InspectionFiles = new InspectionFilesViewModel(this);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this);

            this.AddValidation(others: new IValidatableViewModel[]
            {
                InspectionGeneralInfo,
                PatrolVehicles,
                InspectedShip,
                ShipChecks,
                ShipCatches,
                ShipFishingGears,
                InspectionFiles,
                AdditionalInfo,
                Signatures,
            });
        }

        public InspectionAtSeaDto Edit
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
        public FishingShipViewModel InspectedShip { get; }
        public ShipChecksViewModel ShipChecks { get; }
        public ShipCatchesViewModel ShipCatches { get; }
        public ShipFishingGearsViewModel ShipFishingGears { get; }
        public InspectionFilesViewModel InspectionFiles { get; }
        public AdditionalInfoViewModel AdditionalInfo { get; }
        public SignaturesViewModel Signatures { get; }

        [MaxLength(4000)]
        public ValidState CaptainComment { get; set; }

        public override void OnDisappearing()
        {
            GlobalVariables.IsAddingInspection = false;
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[]
            {
                GroupResourceEnum.CatchInspection,
                GroupResourceEnum.FishingGear,
                GroupResourceEnum.ShipChecks,
                GroupResourceEnum.FishingShip,
                GroupResourceEnum.PatrolVehicle,
                GroupResourceEnum.InspectedPerson,
                GroupResourceEnum.InspectedShipData,
                GroupResourceEnum.InWaterOnBoardInspection,
                GroupResourceEnum.Validation,
            };
        }

        public override async Task Initialize(object sender)
        {
            InspectionHelper.Initialize(this, Edit);
            InspectionHelper.InitShip(InspectedShip, ShipChecks, ShipFishingGears.FishingGears);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();
            List<InspectionCheckTypeDto> checkTypes = nomTransaction.GetInspectionCheckTypes(InspectionType.IBS);
            List<CatchZoneNomenclatureDto> catchAreas = nomTransaction.GetCatchZones();

            await InspectionGeneralInfo.Init();
            InspectedShip.Init(countries, nomTransaction.GetVesselTypes(), catchAreas);
            ShipCatches.Init(nomTransaction.GetFishes(), nomTransaction.GetCatchInspectionTypes(), catchAreas, nomTransaction.GetTurbotSizeGroups());

            ShipChecks.Toggles.Value.AddRange(
                checkTypes
                    .Where(f => !f.Code.StartsWith(Constants.CatchBase) && !f.Code.StartsWith(Constants.ShipBase) && !f.Code.StartsWith(Constants.ObjectCheckBase) && !f.Code.StartsWith(Constants.FishingGearBase))
                    .Select(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
                    {
                        CheckTypeId = f.Id,
                        Text = f.Name,
                        Type = f.Type,
                        DescriptionLabel = f.DescriptionLabel,
                    })
                    .ToList()
            );

            InspectedShip.Toggles.Value.AddRange(
                checkTypes
                    .Where(f => f.Code.StartsWith(Constants.ShipBase))
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

                await InspectionGeneralInfo.OnEdit(Edit);
                PatrolVehicles.OnEdit(Edit.PatrolVehicles);
                InspectionFiles.OnEdit(Edit);
                InspectedShip.OnEdit(Edit, Edit.LastPortVisit);
                ShipCatches.OnEdit(Edit);
                //ShipFishingGears.Toggles.AssignFrom(Edit.Checks);
                ShipFishingGears.FishingGears.OnEdit(Edit.FishingGears,
                    Edit.PermitLicenses
                        .Where(f => f.CheckValue == CheckTypeEnum.Y || f.CheckValue == CheckTypeEnum.N)
                        .Select(f => f.PermitLicenseId.Value)
                        .ToList()
                );
                Signatures.OnEdit(Edit.Files, fileTypes);
                AdditionalInfo.OnEdit(Edit);

                await ShipChecks.OnEdit(Edit, Edit.Checks);

                InspectedShip.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                ShipChecks.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                ShipCatches.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                ShipFishingGears.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);

                CaptainComment.AssignFrom(Edit.CaptainComment);
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

        private Task OnSaveDraft()
        {
            return Save(ActivityType == ViewActivityType.Edit ? SubmitType.Edit : SubmitType.Draft);
        }

        private Task OnFinish()
        {
            return InspectionSaveHelper.Finish(Sections, Validation, Save);
        }

        private Task OnReturnForEdit()
        {
            return Save(SubmitType.ReturnForEdit);
        }

        private Task Save(SubmitType submitType)
        {
            return this.Save(Edit,
                InspectionFiles,
                (inspectionIdentifier, files) =>
                {
                    InspectionAtSeaDto dto = new InspectionAtSeaDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = InspectionGeneralInfo.BuildReportNum(),
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit || submitType == SubmitType.ReturnForEdit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.IBS,
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.AdministrativeViolation,
                        InspectorComment = AdditionalInfo.InspectorComment,
                        ByEmergencySignal = InspectionGeneralInfo.ByEmergencySignal,
                        EndDate = InspectionGeneralInfo.EndDate,
                        StartDate = InspectionGeneralInfo.StartDate,
                        Inspectors = InspectionGeneralInfo.Inspectors,
                        CaptainComment = CaptainComment,
                        CatchMeasures = ShipCatches.Catches,
                        InspectedShip = InspectedShip.ShipData,
                        Personnel = InspectedShip,
                        FishingGears = ShipFishingGears.FishingGears,
                        Checks = ShipChecks.Toggles
                            .Select(f => (InspectionCheckDto)f)
                            .Where(f => f != null)
                            .Concat(ShipCatches.Toggles
                                .Select(f => (InspectionCheckDto)f)
                                .Where(f => f != null)
                            )
                            .Concat(InspectedShip.Toggles
                                .Select(f => (InspectionCheckDto)f)
                                .Where(f => f != null)
                            )
                            //.Concat(ShipFishingGears.Toggles
                            //    .Select(f => (InspectionCheckDto)f)
                            //    .Where(f => f != null)
                            //)
                            .Concat((List<InspectionCheckDto>)ShipChecks)
                            .ToList(),
                        IsOfflineOnly = IsLocal,
                        PatrolVehicles = PatrolVehicles,
                        ObservationTexts = new InspectionObservationTextDto[]
                        {
                            AdditionalInfo.ObservationsOrViolations,
                            InspectedShip.ObservationsOrViolations,
                            ShipChecks.ObservationsOrViolations,
                            ShipCatches.ObservationsOrViolations,
                            ShipFishingGears.ObservationsOrViolations,
                        }.Where(f => !string.IsNullOrWhiteSpace(f.Text)).ToList(),
                        LogBooks = ShipChecks.LogBooks,
                        PermitLicenses = ShipChecks.PermitLicenses,
                        Permits = ShipChecks.Permits,
                        LastPortVisit = InspectedShip.LastHarbour,
                    };

                    return InspectionsTransaction.HandleInspection(dto, submitType);
                }
            );
        }
    }
}
