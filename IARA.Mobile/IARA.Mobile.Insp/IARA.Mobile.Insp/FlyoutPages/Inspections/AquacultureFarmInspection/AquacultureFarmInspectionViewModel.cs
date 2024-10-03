using IARA.Mobile.Application;
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
using IARA.Mobile.Insp.ViewModels.Models;
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

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.AquacultureFarmInspection
{
    public class AquacultureFarmInspectionViewModel : InspectionPageViewModel
    {
        private InspectionAquacultureDto _edit;

        public AquacultureFarmInspectionViewModel()
        {
            AquacultureChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnAquacultureChosen);
            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);
            Finish = CommandBuilder.CreateFrom(OnFinish);
            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this);
            PatrolVehicles = new PatrolVehiclesViewModel(this, false);
            LegalEntity = new LegalViewModel(this, InspectedPersonType.LicUsrLgl)
            {
                IsEnabled = true,
            };
            Representative = new PersonViewModel(this, InspectedPersonType.ReprsPers, false);
            Catches = new CatchInspectionsViewModel(this,
                showCatchArea: false,
                showAllowedDeviation: false,
                showAverageSize: true,
                showFishSex: true,
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
                LegalEntity,
                Representative,
                Catches,
                InspectionFiles,
                AdditionalInfo,
            });

            Aquaculture.ItemsSource = new TLObservableCollection<SelectNomenclatureDto>();
            Aquaculture.GetMore = (int page, int pageSize, string search) =>
                NomenclaturesTransaction.GetAquacultures(page, pageSize, search);
        }

        public InspectionAquacultureDto Edit
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
        public LegalViewModel LegalEntity { get; }
        public PersonViewModel Representative { get; }
        public CatchInspectionsViewModel Catches { get; set; }
        public InspectionFilesViewModel InspectionFiles { get; }
        public AdditionalInfoViewModel AdditionalInfo { get; }
        public SignaturesViewModel Signatures { get; }

        public ValidStateValidatableTable<ToggleViewModel> InspectionToggles { get; set; }

        [Required]
        public ValidStateLocation Location { get; set; }

        [Required]
        public ValidStateInfiniteSelect<SelectNomenclatureDto> Aquaculture { get; set; }

        [MaxLength(4000)]
        public ValidState OtherFishingGear { get; set; }

        [MaxLength(4000)]
        public ValidState RepresentativeComment { get; set; }

        public ICommand AquacultureChosen { get; }

        public override void OnDisappearing()
        {
            GlobalVariables.IsAddingInspection = false;
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[]
            {
                GroupResourceEnum.FishingGear,
                GroupResourceEnum.PatrolVehicle,
                GroupResourceEnum.CatchInspection,
                GroupResourceEnum.AquacultureFarmInspection,
                GroupResourceEnum.Validation,
            };
        }

        public override async Task Initialize(object sender)
        {
            InspectionHelper.Initialize(this, Edit);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();

            await InspectionGeneralInfo.Init();
            Catches.Init(
                nomTransaction.GetFishes(),
                nomTransaction.GetCatchInspectionTypes(),
                nomTransaction.GetCatchZones(),
                nomTransaction.GetTurbotSizeGroups(),
                nomTransaction.GetFishSex()
            );
            LegalEntity.Init(countries);
            Representative.Init(countries);

            Aquaculture.ItemsSource.AddRange(NomenclaturesTransaction.GetAquacultures(0, CommonGlobalVariables.PullItemsCount));

            InspectionToggles.Value.AddRange(
                nomTransaction.GetInspectionCheckTypes(InspectionType.IAQ)
                    .ConvertAll(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
                    {
                        CheckTypeId = f.Id,
                        Text = f.Name,
                        Type = f.Type,
                        DescriptionLabel = f.DescriptionLabel,
                    })
            );

            InspectionFiles.Init(nomTransaction.GetFileTypes(Constants.InspectionFileTypeCode));

            if (Edit != null)
            {
                List<SelectNomenclatureDto> fileTypes = nomTransaction.GetFileTypes();

                InspectionState = Edit.InspectionState;

                await InspectionGeneralInfo.OnEdit(Edit);
                PatrolVehicles.OnEdit(Edit.PatrolVehicles);
                InspectionFiles.OnEdit(Edit);
                Signatures.OnEdit(Edit.Files, fileTypes);
                Catches.OnEdit(Edit.CatchMeasures);
                Representative.OnEdit(Edit.Personnel);
                AdditionalInfo.OnEdit(Edit);

                InspectionToggles.AssignFrom(Edit.Checks);

                if (Edit.AquacultureId != null)
                {
                    Aquaculture.Value = nomTransaction.GetAquaculture(Edit.AquacultureId.Value);

                    ShipPersonnelDetailedDto owner = NomenclaturesTransaction.GetAquacultureOwner(Edit.AquacultureId.Value);

                    if (owner != null)
                    {
                        LegalEntity.Nationality.AssignFrom(owner.Address.CountryId, LegalEntity.Nationalities);
                        LegalEntity.Address.Value = owner.Address.BuildAddress();
                    }

                    LegalEntity.Name.Value = owner.FirstName;
                    LegalEntity.EIK.AssignFrom(owner.Eik);
                }

                Location.AssignFrom(Edit.Location);
                OtherFishingGear.AssignFrom(Edit.OtherFishingGear);
                RepresentativeComment.AssignFrom(Edit.RepresentativeComment);
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

        private void OnAquacultureChosen(SelectNomenclatureDto aquaculture)
        {
            ShipPersonnelDetailedDto owner = NomenclaturesTransaction.GetAquacultureOwner(aquaculture.Id);

            if (owner.Address != null)
            {
                LegalEntity.Nationality.AssignFrom(owner.Address.CountryId, LegalEntity.Nationalities);
                LegalEntity.Address.Value = owner.Address.BuildAddress();
            }

            LegalEntity.Name.Value = owner.FirstName;
            LegalEntity.EIK.AssignFrom(owner.Eik);
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
                    InspectionAquacultureDto dto = new InspectionAquacultureDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = InspectionGeneralInfo.BuildReportNum(),
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit || submitType == SubmitType.ReturnForEdit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.IAQ,
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.ViolatedRegulations.HasViolations,
                        InspectorComment = AdditionalInfo.InspectorComment,
                        ByEmergencySignal = InspectionGeneralInfo.ByEmergencySignal,
                        EndDate = InspectionGeneralInfo.EndDate,
                        StartDate = InspectionGeneralInfo.StartDate,
                        Inspectors = InspectionGeneralInfo.Inspectors,
                        Checks = InspectionToggles
                            .Select(f => (InspectionCheckDto)f)
                            .Where(f => f != null)
                            .ToList(),
                        IsOfflineOnly = IsLocal,
                        CatchMeasures = Catches,
                        OtherFishingGear = OtherFishingGear,
                        AquacultureId = Aquaculture.Value,
                        Location = Location,
                        RepresentativeComment = RepresentativeComment,
                        Personnel = new InspectionSubjectPersonnelDto[]
                        {
                            Representative
                        }.ToList(),
                        ObservationTexts = new InspectionObservationTextDto[]
                        {
                            AdditionalInfo.ObservationsOrViolations,
                        }.Where(f => !string.IsNullOrWhiteSpace(f.Text)).ToList(),
                        PatrolVehicles = PatrolVehicles,
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
