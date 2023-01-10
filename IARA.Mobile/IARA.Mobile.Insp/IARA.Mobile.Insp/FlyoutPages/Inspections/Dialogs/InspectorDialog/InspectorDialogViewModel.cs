using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Insp.ViewModels.Models;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.InspectorDialog
{
    public class InspectorDialogViewModel : TLBaseDialogViewModel<InspectorModel>
    {
        private List<SelectNomenclatureDto> _nationalities;
        private List<SelectNomenclatureDto> _institutions;

        public InspectorDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
            InspectorChosen = CommandBuilder.CreateFrom<InspectorNomenclatureDto>(OnInspectorChosen);

            this.AddValidation(groups: new Dictionary<string, Func<bool>>
            {
                { Group.REGISTERED, () => IsRegistered },
                { Group.NOT_REGISTERED, () => !IsRegistered },
            });

            IsRegistered.Value = true;
            Inspector.ItemsSource = new TLObservableCollection<InspectorNomenclatureDto>();
            Inspector.GetMore = (int page, int pageSize, string search) =>
                DependencyService.Resolve<INomenclatureTransaction>()
                    .GetInspectors(page, pageSize, search)
                        .FindAll(f => !Inspectors.Inspectors.Any(s => s.Dto?.Id == f.Id));
        }

        public InspectionPageViewModel Inspection { get; set; }

        public InspectorsViewModel Inspectors { get; set; }

        public InspectorModel Edit { get; set; }

        public ViewActivityType DialogType { get; set; }

        public bool IsCurrentUser { get; set; }

        public ValidStateBool IsRegistered { get; set; }

        public ValidStateBool IsInCharge { get; set; }

        public ValidStateBool HasIdentified { get; set; }

        [Required]
        [ValidGroup(Group.REGISTERED)]
        public ValidStateInfiniteSelect<InspectorNomenclatureDto> Inspector { get; set; }

        [Required]
        [MaxLength(5)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState CardNum { get; set; }

        [Required]
        [MaxLength(200)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState FirstName { get; set; }

        [Required]
        [MaxLength(200)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState MiddleName { get; set; }

        [Required]
        [MaxLength(200)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState LastName { get; set; }

        [Required]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidStateSelect<SelectNomenclatureDto> Nationality { get; set; }

        [Required]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidStateSelect<SelectNomenclatureDto> Institution { get; set; }

        public List<SelectNomenclatureDto> Nationalities
        {
            get => _nationalities;
            private set => SetProperty(ref _nationalities, value);
        }
        public List<SelectNomenclatureDto> Institutions
        {
            get => _institutions;
            private set => SetProperty(ref _institutions, value);
        }

        public override Task Initialize(object sender)
        {
            INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

            Nationalities = nomTransaction.GetCountries();
            Institutions = nomTransaction.GetInstitutions();

            Nationality.Value = Nationalities.Find(f => f.Code == CommonConstants.NomenclatureBulgaria);
            Institution.Value = Institutions.Find(f => f.Code == Constants.IARAInstitution);

            Inspector.ItemsSource.AddRange(nomTransaction.GetInspectors(0, CommonGlobalVariables.PullItemsCount));

            if (Edit != null)
            {
                IsRegistered.Value = !Edit.Dto.IsNotRegistered && Edit.Dto.InspectorId.HasValue;
                IsInCharge.Value = Edit.IsInCharge;
                HasIdentified.Value = Edit.HasIdentified;

                if (IsRegistered)
                {
                    Inspector.Value = new InspectorNomenclatureDto
                    {
                        Id = Edit.Dto.InspectorId.Value,
                        Code = Edit.Dto.CardNum,
                        Name = $"{Edit.Dto.FirstName} {(Edit.Dto.MiddleName == null ? string.Empty : Edit.Dto.MiddleName + " ")}{Edit.Dto.LastName}",
                        UserId = Edit.Dto.UserId,
                    };
                }
                else
                {
                    CardNum.AssignFrom(Edit.Dto.CardNum);
                    FirstName.AssignFrom(Edit.Dto.FirstName);
                    MiddleName.AssignFrom(Edit.Dto.MiddleName);
                    LastName.AssignFrom(Edit.Dto.LastName);
                }
                Nationality.AssignFrom(Edit.Dto.CitizenshipId, Nationalities);
                Institution.AssignFrom(Edit.Dto.InstitutionId, Institutions);

                if (DialogType == ViewActivityType.Edit)
                {
                    Validation.Force();
                }
            }

            return Task.CompletedTask;
        }

        public ICommand InspectorChosen { get; }
        public ICommand Save { get; }

        private void OnInspectorChosen(InspectorNomenclatureDto nomenclatureDto)
        {
            if (Inspectors.Inspectors.Any(f => f != Edit && f.Dto.InspectorId == nomenclatureDto.Id))
            {
                Inspector.Value = null;
            }
            else
            {
                IInspectionsTransaction inspTransaction = DependencyService.Resolve<IInspectionsTransaction>();

                InspectorDuringInspectionDto inspector = inspTransaction.GetInspector(Inspector.Value.Id);

                Nationality.AssignFrom(inspector.CitizenshipId, Nationalities);
                Institution.AssignFrom(inspector.InstitutionId, Institutions);
            }
        }

        private Task OnSave()
        {
            InspectorDuringInspectionDto inspector;

            if (IsRegistered)
            {
                if (Inspector.Value == null)
                {
                    return HideDialog(null);
                }

                IInspectionsTransaction inspTransaction = DependencyService.Resolve<IInspectionsTransaction>();

                inspector = inspTransaction.GetInspector(Inspector.Value.Id);

                if (inspector == null)
                {
                    return HideDialog(null);
                }

                inspector.Id = Edit?.Dto.Id;
                inspector.IsInCharge = IsInCharge;
                inspector.HasIdentifiedHimself = HasIdentified;
                inspector.IsNotRegistered = false;
            }
            else
            {
                inspector = new InspectorDuringInspectionDto
                {
                    Id = Edit?.Dto.Id,
                    IsInCharge = false,
                    IsNotRegistered = true,
                    HasIdentifiedHimself = HasIdentified,
                    CardNum = CardNum.Value,
                    HasBulgarianAddressRegistration = Nationality.Value?.Code == CommonConstants.NomenclatureBulgaria,
                    InstitutionId = Institution.Value,
                    CitizenshipId = Nationality.Value,
                    FirstName = FirstName.Value,
                    MiddleName = MiddleName.Value,
                    LastName = LastName.Value,
                    TerritoryCode = Edit?.Dto.TerritoryCode
                };
            }

            return HideDialog(new InspectorModel
            {
                Dto = inspector,
                HasIdentified = inspector.HasIdentifiedHimself,
                IsInCharge = inspector.IsInCharge,
                Institution = Institution.Value?.Code,
            });
        }
    }
}
