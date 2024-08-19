using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.DTObjects.Reports;
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
using IARA.Mobile.Insp.ViewModels.Models;
using IARA.Mobile.Shared.Attributes;
using IARA.Mobile.Shared.Menu;
using IARA.Mobile.Shared.Views;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.FishermanInspection
{
    public class FishermanInspectionViewModel : InspectionPageViewModel
    {
        private InspectionFisherDto _edit;

        public FishermanInspectionViewModel()
        {
            OpenTicketReport = CommandBuilder.CreateFrom(OnOpenTicketReport);
            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);
            Finish = CommandBuilder.CreateFrom(OnFinish);

            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this);
            PatrolVehicles = new PatrolVehiclesViewModel(this, false);
            InspectedPerson = new PersonViewModel(this, InspectedPersonType.CaptFshmn, false);
            Catches = new CatchInspectionsViewModel(this,
                showCatchArea: false,
                showAllowedDeviation: false,
                showUndersizedCheck: true,
                showType: false,
                showTurbotSizeGroups: false
            );
            InspectionFiles = new InspectionFilesViewModel(this);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this);

            this.AddValidation(others: new IValidatableViewModel[]
            {
                InspectionGeneralInfo,
                PatrolVehicles,
                InspectedPerson,
                Catches,
                InspectionFiles,
                AdditionalInfo,
                Signatures,
            });

            TicketNumber.HasAsterisk = true;
            HasTicket.Value = true;
        }

        public InspectionFisherDto Edit
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
        public PersonViewModel InspectedPerson { get; set; }
        public CatchInspectionsViewModel Catches { get; set; }
        public InspectionFilesViewModel InspectionFiles { get; }
        public AdditionalInfoViewModel AdditionalInfo { get; }
        public SignaturesViewModel Signatures { get; }

        public ValidStateLocation Location { get; set; }

        [MaxLength(500)]
        public ValidState Address { get; set; }

        public ValidStateBool HasTicket { get; set; }

        [RequiredIfBooleanEquals(nameof(HasTicket), false, ErrorMessageResourceName = "Required")]
        [MaxLength(100)]
        public ValidState TicketNumber { get; set; }

        [Required]
        [TLRange(0, 10000)]
        public ValidState FishingRodsCount { get; set; }

        [Required]
        [TLRange(0, 10000)]
        public ValidState FishingHooksCount { get; set; }

        public ValidStateValidatableTable<ToggleViewModel> InspectionToggles { get; set; }

        [MaxLength(4000)]
        public ValidState FishermanComment { get; set; }

        public ICommand OpenTicketReport { get; }

        public override void OnDisappearing()
        {
            GlobalVariables.IsAddingInspection = false;
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[]
            {
                GroupResourceEnum.PatrolVehicle,
                GroupResourceEnum.CatchInspection,
                GroupResourceEnum.FishermanInspection,
                GroupResourceEnum.Validation,
            };
        }

        public override async Task Initialize(object sender)
        {
            InspectionHelper.Initialize(this, Edit);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();

            await InspectionGeneralInfo.Init();
            Catches.Init(nomTransaction.GetFishes(), nomTransaction.GetCatchInspectionTypes(), nomTransaction.GetCatchZones(), nomTransaction.GetTurbotSizeGroups(), null);
            InspectedPerson.Init(countries);
            InspectionFiles.Init(nomTransaction.GetFileTypes(Constants.InspectionFileTypeCode));

            InspectionToggles.Value.AddRange(
                nomTransaction.GetInspectionCheckTypes(InspectionType.IFP)
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

                InspectionState = Edit.InspectionState;

                await InspectionGeneralInfo.OnEdit(Edit);
                PatrolVehicles.OnEdit(Edit.PatrolVehicles);
                InspectionFiles.OnEdit(Edit);
                Signatures.OnEdit(Edit.Files, fileTypes);
                Catches.OnEdit(Edit.CatchMeasures);
                InspectedPerson.OnEdit(Edit.Personnel);
                AdditionalInfo.OnEdit(Edit);

                HasTicket.Value = Edit.TicketNum != null;

                InspectionToggles.AssignFrom(Edit.Checks);

                Address.AssignFrom(Edit.InspectionAddress);
                Location.AssignFrom(Edit.InspectionLocation);
                TicketNumber.AssignFrom(Edit.TicketNum);
                FishingRodsCount.AssignFrom(Edit.FishingRodsCount);
                FishingHooksCount.AssignFrom(Edit.FishingHooksCount);
                FishermanComment.AssignFrom(Edit.FishermanComment);
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

        private async Task OnOpenTicketReport()
        {
            await TLLoadingHelper.ShowFullLoadingScreen();
            ReportDto report = await ReportsTransaction.GetByCode(Constants.TicketReportCode);

            if (report != null)
            {
                List<(string, object)> defaultValues = new List<(string, object)>
                {
                    (Constants.EgnReportParameter, InspectedPerson.EGN.Value),
                    (Constants.TicketNumReportParameter, TicketNumber.Value),
                };

                await MainNavigator.Current.GoToPageAsync(new ReportPage(report, defaultValues));
            }

            await TLLoadingHelper.HideFullLoadingScreen();
        }

        private Task OnSaveDraft()
        {
            return Save(ActivityType == ViewActivityType.Edit ? SubmitType.Edit : SubmitType.Draft);
        }

        private Task OnFinish()
        {
            List<SelectNomenclatureDto> fileTypes = DependencyService.Resolve<INomenclatureTransaction>().GetFileTypes();

            return InspectionSaveHelper.Finish(Sections, Validation, Save);
        }

        private Task Save(SubmitType submitType)
        {
            return this.Save(Edit,
                InspectionFiles,
                async (inspectionIdentifier, files) =>
                {
                    InspectionFisherDto dto = new InspectionFisherDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = InspectionGeneralInfo.BuildReportNum(),
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit || submitType == SubmitType.ReturnForEdit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.IFP,
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.ViolatedRegulations.HasViolations,
                        InspectorComment = AdditionalInfo.InspectorComment,
                        ByEmergencySignal = InspectionGeneralInfo.ByEmergencySignal,
                        EndDate = InspectionGeneralInfo.EndDate,
                        StartDate = InspectionGeneralInfo.StartDate,
                        Inspectors = InspectionGeneralInfo.Inspectors,
                        IsOfflineOnly = IsLocal,
                        Checks = InspectionToggles
                            .Select(f => (InspectionCheckDto)f)
                            .Where(f => f != null)
                            .ToList(),
                        CatchMeasures = Catches,
                        FishermanComment = FishermanComment,
                        FishingHooksCount = ParseHelper.ParseInteger(FishingHooksCount),
                        FishingRodsCount = ParseHelper.ParseInteger(FishingRodsCount),
                        TicketNum = HasTicket.Value ? TicketNumber.Value : null,
                        Personnel = new InspectionSubjectPersonnelDto[]
                        {
                            InspectedPerson
                        }.Where(f => f != null).ToList(),
                        InspectionAddress = Address,
                        InspectionLocation = Location,
                        ObservationTexts = new InspectionObservationTextDto[]
                        {
                            AdditionalInfo.ObservationsOrViolations,
                        }.Where(f => !string.IsNullOrWhiteSpace(f.Text)).ToList(),
                        PatrolVehicles = PatrolVehicles,
                        ViolatedRegulations = AdditionalInfo.ViolatedRegulations.ViolatedRegulations.Value.Select(x => (AuanViolatedRegulationDto)x).ToList(),
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
    }
}
