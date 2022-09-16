using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
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
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.PatrolVehicleDialog
{
    public class PatrolVehicleDialogViewModel : TLBaseDialogViewModel<PatrolVehicleModel>
    {
        private List<SelectNomenclatureDto> _flags;
        private List<SelectNomenclatureDto> _institutions;
        private List<SelectNomenclatureDto> _patrolVehicleTypes;

        public PatrolVehicleDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
            PatrolVehicleChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnPatrolVehicleChosen);

            this.AddValidation(groups: new Dictionary<string, Func<bool>>
            {
                { Group.REGISTERED, () => IsRegistered },
                { Group.NOT_REGISTERED, () => !IsRegistered },
                { Group.IS_WATER_VEHICLE, () => IsWaterVehicle },
            });

            IsRegistered.Value = true;
            PatrolVehicle.ItemsSource = new TLObservableCollection<SelectNomenclatureDto>();
            PatrolVehicle.GetMore = (int page, int pageSize, string search) =>
                DependencyService.Resolve<INomenclatureTransaction>()
                    .GetPatrolVehicles(IsWaterVehicle, page, pageSize, search)
                        .FindAll(f => !PatrolVehicles.InspectorVehicles.Any(s => s.Dto?.Id == f.Id));
        }

        public bool IsWaterVehicle { get; set; }

        public InspectionPageViewModel Inspection { get; set; }

        public PatrolVehiclesViewModel PatrolVehicles { get; set; }

        public PatrolVehicleModel Edit { get; set; }

        public ViewActivityType DialogType { get; set; }

        public ValidStateBool IsRegistered { get; set; }

        [Required]
        [ValidGroup(Group.REGISTERED)]
        public ValidStateInfiniteSelect<SelectNomenclatureDto> PatrolVehicle { get; set; }

        [Required]
        [MaxLength(500)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState Name { get; set; }

        [MaxLength(20)]
        [ValidGroup(Group.NOT_REGISTERED)]
        [ValidGroup(Group.IS_WATER_VEHICLE)]
        public ValidState RegistrationNumber { get; set; }

        [MaxLength(50)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState ExternalMarkings { get; set; }

        [MaxLength(50)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState CallSign { get; set; }

        [Required]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidStateSelect<SelectNomenclatureDto> Flag { get; set; }

        [Required]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidStateSelect<SelectNomenclatureDto> Institution { get; set; }

        [Required]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidStateSelect<SelectNomenclatureDto> PatrolVehicleType { get; set; }

        [Required]
        public ValidStateLocation Location { get; set; }

        public List<SelectNomenclatureDto> Flags
        {
            get => _flags;
            private set => SetProperty(ref _flags, value);
        }
        public List<SelectNomenclatureDto> Institutions
        {
            get => _institutions;
            private set => SetProperty(ref _institutions, value);
        }
        public List<SelectNomenclatureDto> PatrolVehicleTypes
        {
            get => _patrolVehicleTypes;
            private set => SetProperty(ref _patrolVehicleTypes, value);
        }

        public override Task Initialize(object sender)
        {
            INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

            Flags = nomTransaction.GetCountries();
            Institutions = nomTransaction.GetInstitutions();
            PatrolVehicleTypes = nomTransaction.GetPatrolVehicleTypes(IsWaterVehicle);

            Flag.Value = Flags.Find(f => f.Code == CommonConstants.NomenclatureBulgaria);
            Institution.Value = Institutions.Find(f => f.Code == Constants.IARAInstitution);
            PatrolVehicleType.Value = PatrolVehicleTypes[0];

            PatrolVehicle.ItemsSource.AddRange(nomTransaction.GetPatrolVehicles(IsWaterVehicle, 0, CommonGlobalVariables.PullItemsCount));

            if (Edit != null)
            {
                IsRegistered.Value = (Edit.Dto.IsRegistered ?? false) && Edit.Dto.UnregisteredVesselId.HasValue;

                if (IsRegistered)
                {
                    PatrolVehicle.Value = new SelectNomenclatureDto
                    {
                        Id = Edit.Dto.UnregisteredVesselId.Value,
                        Code = Edit.Dto.CFR,
                        Name = Edit.Dto.Name,
                    };
                }
                else
                {
                    Name.AssignFrom(Edit.Dto.Name);
                    RegistrationNumber.AssignFrom(Edit.Dto.CFR);
                }

                CallSign.AssignFrom(Edit.Dto.RegularCallsign);
                ExternalMarkings.AssignFrom(Edit.Dto.ExternalMark);
                Flag.AssignFrom(Edit.Dto.FlagCountryId, Flags);
                Institution.AssignFrom(Edit.Dto.InstitutionId, Institutions);
                PatrolVehicleType.AssignFrom(Edit.Dto.PatrolVehicleTypeId, PatrolVehicleTypes);
                Location.AssignFrom(Edit.Dto.Location);

                if (DialogType == ViewActivityType.Edit)
                {
                    Validation.Force();
                }
            }

            return Task.CompletedTask;
        }

        public ICommand PatrolVehicleChosen { get; }
        public ICommand Save { get; }

        private void OnPatrolVehicleChosen(SelectNomenclatureDto nomenclatureDto)
        {
            if (PatrolVehicles.InspectorVehicles.Any(f => f != Edit && f.Dto.UnregisteredVesselId == nomenclatureDto.Id))
            {
                PatrolVehicle.Value = null;
            }
            else
            {
                INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

                VesselDuringInspectionDto patrolVehicle = nomTransaction.GetPatrolVehicle(PatrolVehicle.Value.Id);

                CallSign.AssignFrom(patrolVehicle.RegularCallsign);
                ExternalMarkings.AssignFrom(patrolVehicle.ExternalMark);
                Flag.AssignFrom(patrolVehicle.FlagCountryId, Flags);
                Institution.AssignFrom(patrolVehicle.InstitutionId, Institutions);
                PatrolVehicleType.AssignFrom(patrolVehicle.PatrolVehicleTypeId, PatrolVehicleTypes);
            }
        }

        private Task OnSave()
        {
            VesselDuringInspectionDto patrolVehicle;

            if (IsRegistered)
            {
                if (PatrolVehicle.Value == null)
                {
                    return HideDialog(null);
                }

                INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

                patrolVehicle = nomTransaction.GetPatrolVehicle(PatrolVehicle.Value.Id);

                if (patrolVehicle == null)
                {
                    return HideDialog(null);
                }

                patrolVehicle.Id = Edit?.Dto.Id;
                patrolVehicle.Location = Location;
                patrolVehicle.IsRegistered = true;
            }
            else
            {
                patrolVehicle = new VesselDuringInspectionDto
                {
                    Id = Edit?.Dto.Id,
                    Name = Name,
                    CFR = RegistrationNumber,
                    ExternalMark = ExternalMarkings,
                    FlagCountryId = Flag.Value,
                    InstitutionId = Institution.Value,
                    IsRegistered = false,
                    Location = Location,
                    PatrolVehicleTypeId = PatrolVehicleType.Value,
                    RegularCallsign = CallSign,
                };
            }

            return HideDialog(new PatrolVehicleModel
            {
                Institution = Institution.Value?.Code,
                PatrolVehicleType = PatrolVehicleType.Value?.Name,
                Dto = patrolVehicle,
            });
        }
    }
}
