using IARA.Mobile.Application;
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

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.VehicleInspection
{
    public class VehicleInspectionViewModel : InspectionPageViewModel
    {
        private List<SelectNomenclatureDto> _vehicleTypes;
        private List<SelectNomenclatureDto> _countries;
        private List<SelectNomenclatureDto> _institutions;
        private InspectionTransportVehicleDto _edit;

        public VehicleInspectionViewModel()
        {
            SaveDraft = CommandBuilder.CreateFrom(OnSaveDraft);
            Finish = CommandBuilder.CreateFrom(OnFinish);

            InspectionGeneralInfo = new InspectionGeneralInfoViewModel(this);
            Owner = new SubjectViewModel(this, InspectedPersonType.OwnerPers, InspectedPersonType.OwnerLegal);
            Driver = new PersonViewModel(this, InspectedPersonType.Driver);
            Buyer = new BuyerViewModel(this);
            Catches = new DeclarationCatchesViewModel(this, false);
            InspectionFiles = new InspectionFilesViewModel(this);
            AdditionalInfo = new AdditionalInfoViewModel(this);
            Signatures = new SignaturesViewModel(this);

            this.AddValidation(others: new IValidatableViewModel[]
            {
                InspectionGeneralInfo,
                Owner,
                Driver,
                Buyer,
                Catches,
                InspectionFiles,
                AdditionalInfo,
                Signatures,
            });

            IsSealed.Value = true;
        }

        public InspectionTransportVehicleDto Edit
        {
            get => _edit;
            set
            {
                _edit = value;
                ProtectedEdit = value;
            }
        }

        public InspectionGeneralInfoViewModel InspectionGeneralInfo { get; }
        public SubjectViewModel Owner { get; }
        public PersonViewModel Driver { get; }
        public BuyerViewModel Buyer { get; }
        public DeclarationCatchesViewModel Catches { get; set; }
        public InspectionFilesViewModel InspectionFiles { get; }
        public AdditionalInfoViewModel AdditionalInfo { get; }
        public SignaturesViewModel Signatures { get; }

        public ValidStateLocation Location { get; set; }

        [MaxLength(500)]
        public ValidState Address { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> VehicleType { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Country { get; set; }

        [MaxLength(20)]
        public ValidState TractorLicensePlateNum { get; set; }

        [MaxLength(50)]
        public ValidState TractorBrand { get; set; }

        [MaxLength(50)]
        public ValidState TractorModel { get; set; }

        [MaxLength(20)]
        public ValidState TrailerLicensePlateNum { get; set; }

        public ValidStateValidatableTable<ToggleViewModel> InspectedVehicleToggles { get; set; }

        public ValidStateBool IsSealed { get; set; }

        public ValidStateSelect<SelectNomenclatureDto> InstitutionWhoPutTheSeals { get; set; }

        [MaxLength(500)]
        public ValidState SealCondition { get; set; }

        [MaxLength(500)]
        public ValidState TransporterComment { get; set; }

        [MaxLength(4000)]
        public ValidState TransportDestination { get; set; }

        public List<SelectNomenclatureDto> VehicleTypes
        {
            get => _vehicleTypes;
            set => SetProperty(ref _vehicleTypes, value);
        }
        public List<SelectNomenclatureDto> Countries
        {
            get => _countries;
            set => SetProperty(ref _countries, value);
        }
        public List<SelectNomenclatureDto> Institutions
        {
            get => _institutions;
            set => SetProperty(ref _institutions, value);
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
                GroupResourceEnum.InspectedShipData,
                GroupResourceEnum.DeclarationCatch,
                GroupResourceEnum.CatchInspection,
                GroupResourceEnum.VehicleInspection,
                GroupResourceEnum.Validation,
            };
        }

        public override async Task Initialize(object sender)
        {
            InspectionHelper.Initialize(this, Edit);

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> countries = nomTransaction.GetCountries();
            List<SelectNomenclatureDto> institutions = nomTransaction.GetInstitutions();

            InspectionGeneralInfo.Init();
            Owner.Init(countries);
            Driver.Init(countries);
            Buyer.Init();

            Institutions = institutions;
            Countries = countries;
            VehicleTypes = nomTransaction.GetTransportVehicleTypes();
            Country.Value = countries.Find(f => f.Code == CommonConstants.NomenclatureBulgaria);

            List<InspectionCheckTypeDto> checkTypes = nomTransaction.GetInspectionCheckTypes(InspectionType.IVH);

            InspectedVehicleToggles.Value.AddRange(
                nomTransaction.GetInspectionCheckTypes(InspectionType.IVH)
                    .ConvertAll(f => new ToggleViewModel(f.IsMandatory, f.HasDescription)
                    {
                        CheckTypeId = f.Id,
                        Text = f.Name,
                        Type = f.Type,
                    })
            );

            InspectionFiles.Init(nomTransaction.GetFileTypes(Constants.InspectionFileTypeCode));

            if (Edit != null)
            {
                List<SelectNomenclatureDto> fileTypes = nomTransaction.GetFileTypes();

                InspectionState = Edit.InspectionState;

                InspectionGeneralInfo.OnEdit(Edit);
                InspectionFiles.OnEdit(Edit);
                Signatures.OnEdit(Edit.Files, fileTypes);
                Catches.OnEdit(Edit.CatchMeasures);
                Owner.OnEdit(Edit.Personnel);
                Driver.OnEdit(Edit.Personnel);
                Buyer.OnEdit(Edit.Personnel);
                AdditionalInfo.OnEdit(Edit);

                InspectedVehicleToggles.AssignFrom(Edit.Checks);

                TractorBrand.AssignFrom(Edit.TractorBrand);
                TractorModel.AssignFrom(Edit.TractorModel);
                Location.AssignFrom(Edit.InspectionLocation);
                Address.AssignFrom(Edit.InspectionAddress);
                VehicleType.AssignFrom(Edit.VehicleTypeId, VehicleTypes);
                Country.AssignFrom(Edit.CountryId, Countries);
                TractorLicensePlateNum.AssignFrom(Edit.TractorLicensePlateNum);
                TrailerLicensePlateNum.AssignFrom(Edit.TrailerLicensePlateNum);
                IsSealed.AssignFrom(Edit.IsSealed);
                InstitutionWhoPutTheSeals.AssignFrom(Edit.SealInstitutionId, Institutions);
                SealCondition.AssignFrom(Edit.SealCondition);
                TransporterComment.AssignFrom(Edit.TransporterComment);
                TransportDestination.AssignFrom(Edit.TransportDestination);
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
                    InspectionTransportVehicleDto dto = new InspectionTransportVehicleDto
                    {
                        Id = Edit?.Id ?? 0,
                        ReportNum = Edit?.ReportNum,
                        LocalIdentifier = inspectionIdentifier,
                        Files = files,
                        InspectionState = submitType == SubmitType.Draft || submitType == SubmitType.Edit ? InspectionState.Draft : InspectionState.Submitted,
                        InspectionType = InspectionType.IVH,
                        ActionsTaken = AdditionalInfo.ActionsTaken,
                        AdministrativeViolation = AdditionalInfo.AdministrativeViolation,
                        InspectorComment = AdditionalInfo.InspectorComment,
                        ByEmergencySignal = InspectionGeneralInfo.ByEmergencySignal,
                        EndDate = InspectionGeneralInfo.EndDate,
                        StartDate = InspectionGeneralInfo.StartDate,
                        Inspectors = InspectionGeneralInfo.Inspectors,
                        Checks = InspectedVehicleToggles
                            .Select(f => (InspectionCheckDto)f)
                            .Where(f => f != null)
                            .ToList(),
                        IsOfflineOnly = IsLocal,
                        CountryId = Country.Value,
                        CatchMeasures = Catches,
                        IsSealed = IsSealed,
                        SealCondition = SealCondition,
                        SealInstitutionId = InstitutionWhoPutTheSeals.Value,
                        TractorLicensePlateNum = TractorLicensePlateNum,
                        TrailerLicensePlateNum = TrailerLicensePlateNum,
                        TransporterComment = TransporterComment,
                        VehicleTypeId = VehicleType.Value,
                        Personnel = new InspectionSubjectPersonnelDto[]
                        {
                            Owner,
                            Driver,
                            Buyer,
                        }.Where(f => f != null).ToList(),
                        InspectionLocation = Location,
                        InspectionAddress = Address,
                        TractorBrand = TractorBrand,
                        TractorModel = TractorModel,
                        TransportDestination = TransportDestination,
                        ObservationTexts = new InspectionObservationTextDto[]
                        {
                            AdditionalInfo.ObservationsOrViolations,
                        }.Where(f => !string.IsNullOrWhiteSpace(f.Text)).ToList(),
                    };

                    return InspectionsTransaction.HandleInspection(dto, submitType);
                }
            );
        }
    }
}
