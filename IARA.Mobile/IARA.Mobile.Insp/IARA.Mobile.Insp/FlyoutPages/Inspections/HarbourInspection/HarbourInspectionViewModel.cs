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
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Insp.ViewModels.Models;
using IARA.Mobile.Shared.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.HarbourInspection
{
    public class HarbourInspectionViewModel : InspectionPageViewModel
    {
        private string _transhippedCatch;
        private List<SelectNomenclatureDto> _countries;
        private InspectionTransboardingDto _edit;

        public static HarbourInspectionViewModel Static { get; set; }
        public HarbourInspectionViewModel()
        {
            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this);
            InspectionHarbour = new InspectionHarbourViewModel(this, hasDate: false);
            InspectedShip = new FishingShipViewModel(this, canPickLocation: false);
            ShipChecks = new ShipChecksViewModel(this, ShipCatches);
            ShipFishingGears = new ShipFishingGearsViewModel(this, ShipChecks.PermitLicenses, true);
            ShipCatches = new ShipCatchesViewModel(this, ShipFishingGears.FishingGears, isUnloadedQuantityRequired: true);
            TransshippedShip = new InspectedShipDataViewModel(this, canPickLocation: false)
            {
                ShipSelected = CommandBuilder.CreateFrom<ShipSelectNomenclatureDto>(OnShipSelected),
            };
            InspectionFiles = new InspectionFilesViewModel(this);
            TransshippedCatches = new CatchInspectionsViewModel(this, ShipFishingGears.FishingGears);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this);

            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);
            Finish = CommandBuilder.CreateFrom(OnFinish);
            SwitchTransshipping = CommandBuilder.CreateFrom<bool>(OnSwitchTransshipping);

            this.AddValidation(
                groups: new Dictionary<string, Func<bool>>
                {
                    { Group.IS_REQUIRED, () => false },
                    { Group.IS_TRANSHIPMENT, () => HasTranshipment.Value }
                },
                others: new IValidatableViewModel[]
                {
                    InspectionGeneralInfo,
                    InspectionHarbour,
                    InspectedShip,
                    ShipChecks,
                    ShipCatches,
                    ShipFishingGears,
                    TransshippedShip,
                    TransshippedCatches,
                    InspectionFiles,
                    AdditionalInfo,
                    Signatures,
                }
            );

            TransshippedShip.Validation.GlobalGroups = new List<string> { Group.IS_REQUIRED };
            TransshippedCatches.Validation.GlobalGroups = new List<string> { Group.IS_REQUIRED };

            TransshipmentObservation.Category = InspectionObservationCategory.Transshipment;

            Static = this;
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

        public ICommand SwitchTransshipping { get; set; }

        public TLForwardSections Sections { get; set; }

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

        [Required]
        [ValidGroup(Group.IS_TRANSHIPMENT)]
        public ValidStateLocation Location { get; set; }

        public ValidStateBool HasTranshipment { get; set; }

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
                GroupResourceEnum.ShipChecks,
                GroupResourceEnum.FishingShip,
                GroupResourceEnum.CatchInspection,
                GroupResourceEnum.FishingGear,
                GroupResourceEnum.InspectedPerson,
                GroupResourceEnum.InspectedShipData,
                GroupResourceEnum.PatrolVehicle,
                GroupResourceEnum.HarbourInspection,
                GroupResourceEnum.Validation,
                GroupResourceEnum.DeclarationCatch,
            };
        }

        public override async Task Initialize(object sender)
        {
            //await OnGetStartupData();
            InspectionHelper.Initialize(this, Edit);
            InspectionHelper.InitShip(InspectedShip, ShipChecks, ShipCatches, ShipFishingGears.FishingGears);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();
            List<InspectionCheckTypeDto> checkTypes = nomTransaction.GetInspectionCheckTypes(InspectionType.IBP);
            List<SelectNomenclatureDto> fishTypes = nomTransaction.GetFishes();
            List<SelectNomenclatureDto> catchTypes = nomTransaction.GetCatchInspectionTypes();
            List<CatchZoneNomenclatureDto> catchZones = nomTransaction.GetCatchZones();
            List<SelectNomenclatureDto> turbotSizeGroups = nomTransaction.GetTurbotSizeGroups();

            await InspectionGeneralInfo.Init();
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

                await InspectionGeneralInfo.OnEdit(Edit);
                InspectionFiles.OnEdit(Edit);
                Signatures.OnEdit(Edit.Files, fileTypes);
                TransshippedCatches.OnEdit(Edit.TransboardedCatchMeasures);
                AdditionalInfo.OnEdit(Edit);
                Location.AssignFrom(Edit?.ReceivingShipInspection?.InspectedShip?.Location);
                if (Edit.ReceivingShipInspection != null)
                {
                    ShipCatches.OnEdit(Edit.ReceivingShipInspection);
                    ShipFishingGears.FishingGears.OnEdit(Edit.FishingGears,
                        Edit.ReceivingShipInspection.PermitLicenses
                            .Select(f => f.PermitLicenseId.Value)
                            .ToList()
                    );
                    await InspectedShip.OnEdit(Edit.ReceivingShipInspection, Edit.ReceivingShipInspection.LastPortVisit);
                    //ShipFishingGears.Toggles.AssignFrom(Edit.ReceivingShipInspection.Checks);

                    if (Edit.ReceivingShipInspection.InspectionPortId != null || Edit.ReceivingShipInspection.UnregisteredPortName != null)
                    {
                        InspectionHarbour.OnEdit(new PortVisitDto
                        {
                            PortId = Edit.ReceivingShipInspection.InspectionPortId,
                            PortName = Edit.ReceivingShipInspection.UnregisteredPortName,
                            PortCountryId = Edit.ReceivingShipInspection.UnregisteredPortCountryId,
                        });
                    }

                    CaptainComment.AssignFrom(Edit.ReceivingShipInspection.CaptainComment);

                    await ShipChecks.OnEdit(Edit.ReceivingShipInspection, Edit.ReceivingShipInspection.Checks);
                }

                if (Edit.SendingShipInspection?.InspectedShip != null)
                {
                    HasTranshipment.Value = true;
                    SwitchTransshipping.Execute(true);
                    TransshippedShip.OnEdit(Edit.SendingShipInspection.InspectedShip);
                }
                else
                {
                    SwitchTransshipping.Execute(false);
                }

                InspectedShip.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                ShipChecks.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                ShipCatches.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                ShipFishingGears.ObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
                TransshipmentObservation.AssignFrom(Edit.ObservationTexts);
            }
            else
            {
                SwitchTransshipping.Execute(false);
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

        private void OnSwitchTransshipping(bool hasTransshipping)
        {
            TLValidator validator = Location.Validations.Where(f => f.Name == nameof(RequiredAttribute)).FirstOrDefault();
            if (hasTransshipping)
            {
                if (validator == null)
                {
                    Location.Validations.Add(new TLValidator(new RequiredAttribute(), nameof(RequiredAttribute)));
                }
            }
            else
            {
                if (validator != null)
                {
                    Location.Validations.Remove(validator);
                }
            }

            TransshippedShip.OnSwitchTransshipping(hasTransshipping);
            TransshippedCatches.OnSwitchTransshipping(hasTransshipping);
        }

        private Task OnShipSelected(ShipSelectNomenclatureDto nom)
        {
            ShipDto chosenShip = NomenclaturesTransaction.GetShip(nom.Id);

            if (chosenShip == null)
            {
                return Task.CompletedTask;
            }

            TransshippedShip.InspectedShip = chosenShip;

            TransshippedShip.CallSign.AssignFrom(chosenShip.CallSign);
            TransshippedShip.MMSI.AssignFrom(chosenShip.MMSI);
            TransshippedShip.CFR.AssignFrom(chosenShip.CFR);
            TransshippedShip.ExternalMarkings.AssignFrom(chosenShip.ExtMarkings);
            TransshippedShip.Name.AssignFrom(chosenShip.Name);
            TransshippedShip.UVI.AssignFrom(chosenShip.UVI);
            TransshippedShip.Flag.AssignFrom(chosenShip.FlagId, TransshippedShip.Flags);
            TransshippedShip.ShipType.AssignFrom(chosenShip.ShipTypeId, TransshippedShip.ShipTypes);

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
                    VesselDuringInspectionDto transshippedShip = TransshippedShip;

                    if (transshippedShip != null)
                    {
                        transshippedShip.Location = Location;
                    }

                    InspectionTransboardingDto dto = new InspectionTransboardingDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = InspectionGeneralInfo.BuildReportNum(),
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit || submitType == SubmitType.ReturnForEdit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.IBP,
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.ViolatedRegulations.HasViolations,
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
                            LastPortVisit = InspectedShip.LastHarbour,
                            InspectionPortId = InspectionHarbour.Harbour.Value,
                            UnregisteredPortCountryId = InspectionHarbour.Country.Value,
                            UnregisteredPortName = InspectionHarbour.Name,
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
                        SendingShipInspection = transshippedShip != null ? new InspectionTransboardingShipDto
                        {
                            InspectedShip = transshippedShip,
                        } : null,
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
                        ViolatedRegulations = AdditionalInfo.ViolatedRegulations.ViolatedRegulations.Value.Select(x => (AuanViolatedRegulationDto)x).ToList(),
                        IsActive = true,
                    };
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
