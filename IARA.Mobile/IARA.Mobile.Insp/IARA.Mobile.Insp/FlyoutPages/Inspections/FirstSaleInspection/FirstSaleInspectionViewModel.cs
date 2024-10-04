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
using IARA.Mobile.Shared.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.FirstSaleInspection
{
    public class FirstSaleInspectionViewModel : InspectionPageViewModel
    {
        private InspectionFirstSaleDto _edit;
        private bool _hasImporter = true;

        public FirstSaleInspectionViewModel()
        {
            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);
            Finish = CommandBuilder.CreateFrom(OnFinish);

            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this);
            Owner = new InspectedBuyerViewModel(this)
            {
                BuyerChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnBuyerChosen)
            };
            Representative = new PersonViewModel(this, InspectedPersonType.ReprsPers, false);
            Catches = new DeclarationCatchesViewModel(this, hasUndersizedFishControl: false);
            Importer = new LegalViewModel(this, InspectedPersonType.Importer);
            InspectionFiles = new InspectionFilesViewModel(this);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this);

            this.AddValidation(
                groups: new Dictionary<string, Func<bool>>
                {
                    { "HasImporter", () => HasImporter },
                },
                others: new IValidatableViewModel[]
                {
                    InspectionGeneralInfo,
                    Owner,
                    Representative,
                    Catches,
                    Importer,
                    InspectionFiles,
                    AdditionalInfo,
                    Signatures,
                });

            Importer.Validation.GlobalGroups = new List<string> { "HasImporter" };
            CatchObservationsOrViolations.Category = InspectionObservationCategory.Catch;
        }

        public InspectionFirstSaleDto Edit
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
        public InspectedBuyerViewModel Owner { get; }
        public PersonViewModel Representative { get; }
        public DeclarationCatchesViewModel Catches { get; set; }
        public LegalViewModel Importer { get; }
        public InspectionFilesViewModel InspectionFiles { get; }
        public AdditionalInfoViewModel AdditionalInfo { get; }
        public SignaturesViewModel Signatures { get; }

        public bool HasImporter
        {
            get => _hasImporter;
            set => SetProperty(ref _hasImporter, value);
        }

        [Required]
        [MaxLength(500)]
        public ValidState SubjectName { get; set; }

        [Required]
        [MaxLength(500)]
        public ValidState SubjectAddress { get; set; }

        public ValidStateValidatableTable<ToggleViewModel> InspectionToggles { get; set; }

        [MaxLength(4000)]
        public ValidStateObservation CatchObservationsOrViolations { get; set; }

        [MaxLength(4000)]
        public ValidState RepresentativeComment { get; set; }

        public override void OnDisappearing()
        {
            GlobalVariables.IsAddingInspection = false;
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[]
            {
                GroupResourceEnum.DeclarationCatch,
                GroupResourceEnum.CatchInspection,
                GroupResourceEnum.FirstSaleInspection,
                GroupResourceEnum.InspectedShipData,
                GroupResourceEnum.Validation,
            };
        }

        public override async Task Initialize(object sender)
        {
            InspectionHelper.Initialize(this, Edit);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();

            await InspectionGeneralInfo.Init();
            Owner.Init(countries);
            Representative.Init(countries);
            Importer.Init(countries);

            List<InspectionCheckTypeDto> checkTypes = nomTransaction.GetInspectionCheckTypes(InspectionType.IFS);

            InspectionToggles.Value.AddRange(
                checkTypes.ConvertAll(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
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
                InspectionFiles.OnEdit(Edit);
                Signatures.OnEdit(Edit.Files, fileTypes);
                Catches.OnEdit(Edit.InspectionLogBookPages);
                Owner.OnEdit(Edit.Personnel);
                Representative.OnEdit(Edit.Personnel);
                Importer.OnEdit(Edit.Personnel);
                AdditionalInfo.OnEdit(Edit);

                HasImporter = Edit.Personnel?.Exists(f => f.Type == InspectedPersonType.Importer) == true;

                InspectionToggles.AssignFrom(Edit.Checks);

                SubjectName.AssignFrom(Edit.SubjectName);
                SubjectAddress.AssignFrom(Edit.SubjectAddress);
                RepresentativeComment.AssignFrom(Edit.RepresentativeComment);

                CatchObservationsOrViolations.AssignFrom(Edit.ObservationTexts);
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

        private void OnBuyerChosen(SelectNomenclatureDto buyer)
        {
            Owner.SelectedBuyer = NomenclaturesTransaction.GetBuyer(buyer.Id);
            Owner.OnEditBuyer(Owner.SelectedBuyer, false);

            BuyerUtilityDto buyerUtility = NomenclaturesTransaction.GetBuyerUtility(buyer.Id);

            if (buyerUtility != null)
            {
                SubjectName.AssignFrom(buyerUtility.Name);
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
                    InspectionFirstSaleDto dto = new InspectionFirstSaleDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = InspectionGeneralInfo.BuildReportNum(),
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit || submitType == SubmitType.ReturnForEdit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.IFS,
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
                        InspectionLogBookPages = Catches,
                        RepresentativeComment = RepresentativeComment,
                        SubjectAddress = SubjectAddress,
                        SubjectName = SubjectName,
                        Personnel = new InspectionSubjectPersonnelDto[]
                        {
                            Owner,
                            Representative,
                            HasImporter ? (InspectionSubjectPersonnelDto)Importer : null,
                        }.Where(f => f != null).ToList(),
                        ObservationTexts = new InspectionObservationTextDto[]
                        {
                            AdditionalInfo.ObservationsOrViolations,
                            CatchObservationsOrViolations,
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
