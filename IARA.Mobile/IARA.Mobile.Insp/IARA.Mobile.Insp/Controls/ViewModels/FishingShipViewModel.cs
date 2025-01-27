using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Dtos;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class FishingShipViewModel : ViewModel
    {
        private bool _isOwnerUserCheckVisible = false;
        private bool _isOwnerRepresentativeCheckVisible = false;
        private bool _isUserDisabled = true;
        private bool _isRepresentativeDisabled = true;
        public FishingShipViewModel(InspectionPageViewModel inspection, bool canPickLocation = true, bool hasLastPort = true)
        {
            Inspection = inspection;
            HasLastPort = hasLastPort;

            OwnerTypeChosen = CommandBuilder.CreateFrom<InspectedPersonType?>(OnOwnerChosen);
            UserIsOwnerSwitched = CommandBuilder.CreateFrom<bool>(OnUserIsOwnerSwitched);
            IsOwnerRepresentativeSwitched = CommandBuilder.CreateFrom<bool>(OnIsOwnerRepresentativeSwitched);

            ShipOwner = new InspectedPersonViewModel(inspection, InspectedPersonType.OwnerPers, InspectedPersonType.OwnerLegal, OwnerTypeChosen);
            ShipUser = new InspectedPersonViewModel(inspection, InspectedPersonType.LicUsrPers, InspectedPersonType.LicUsrLgl);
            ShipRepresentative = new InspectedPersonViewModel(inspection, InspectedPersonType.ReprsPers)
            {
                IsRepresenter = true
            };
            ShipCaptain = new InspectedPersonViewModel(inspection, InspectedPersonType.CaptFshmn);
            ShipData = new InspectedShipDataViewModel(inspection, new List<InspectedPersonViewModel>()
                {
                    ShipOwner,
                    ShipUser,
                    ShipRepresentative,
                    ShipCaptain
                }, canPickLocation
            );

            IValidatableViewModel[] viewModels;

            if (hasLastPort)
            {
                LastHarbour = new InspectionHarbourViewModel(inspection);

                viewModels = new IValidatableViewModel[]
                {
                    ShipData,
                    ShipOwner,
                    ShipUser,
                    ShipRepresentative,
                    ShipCaptain,
                    LastHarbour,
                };
            }
            else
            {
                viewModels = new IValidatableViewModel[]
                {
                    ShipData,
                    ShipOwner,
                    ShipUser,
                    ShipRepresentative,
                    ShipCaptain,
                };
            }

            this.AddValidation(others: viewModels);

            ObservationsOrViolations.Category = InspectionObservationCategory.ShipData;
        }

        public ICommand OwnerTypeChosen { get; set; }
        public ICommand UserIsOwnerSwitched { get; set; }
        public ICommand IsOwnerRepresentativeSwitched { get; set; }

        public InspectionPageViewModel Inspection { get; }

        public bool HasLastPort { get; }
        public bool IsOwnerUserCheckVisible
        {
            get => _isOwnerUserCheckVisible;
            set => SetProperty(ref _isOwnerUserCheckVisible, value);
        }
        public bool IsOwnerRepresentativeCheckVisible
        {
            get => _isOwnerRepresentativeCheckVisible;
            set => SetProperty(ref _isOwnerRepresentativeCheckVisible, value);
        }
        public bool IsUserDisabled
        {
            get => _isUserDisabled;
            set => SetProperty(ref _isUserDisabled, value);
        }
        public bool IsRepresentativeDisabled
        {
            get => _isRepresentativeDisabled;
            set => SetProperty(ref _isRepresentativeDisabled, value);
        }
        public InspectedShipDataViewModel ShipData { get; }
        public InspectedPersonViewModel ShipOwner { get; }
        public InspectedPersonViewModel ShipUser { get; }
        public InspectedPersonViewModel ShipRepresentative { get; }
        public InspectedPersonViewModel ShipCaptain { get; }
        public InspectionHarbourViewModel LastHarbour { get; }

        public ValidStateValidatableTable<ToggleViewModel> Toggles { get; set; }

        [MaxLength(4000)]
        public ValidStateObservation ObservationsOrViolations { get; set; }

        public ValidStateBool RepresentativeIsOwner { get; set; }
        public ValidStateBool UserIsOwner { get; set; }

        public void Init(List<SelectNomenclatureDto> nationalities, List<SelectNomenclatureDto> shipTypes, List<CatchZoneNomenclatureDto> quadrants)
        {
            ShipData.Init(nationalities, shipTypes, quadrants);
            ShipOwner.Init(nationalities);
            ShipUser.Init(nationalities);
            ShipRepresentative.Init(nationalities);
            ShipCaptain.Init(nationalities);
            LastHarbour?.Init(nationalities);
        }

        public async Task OnEdit(IFishingShipInspection fishingShipInspection, PortVisitDto port = null)
        {
            ShipData.OnEdit(fishingShipInspection.InspectedShip);

            if (Inspection.ActivityType != ViewActivityType.Review && ShipData.Ship.Value != null)
            {
                //await ShipData.ShipSelected.ExecuteAsync(ShipData.Ship.Value);
            }

            Toggles.AssignFrom(fishingShipInspection.Checks);

            if (fishingShipInspection.Personnel == null)
            {
                return;
            }

            InspectionSubjectPersonnelDto owner = null;
            InspectionSubjectPersonnelDto user = null;
            InspectionSubjectPersonnelDto representative = null;
            InspectionSubjectPersonnelDto captain = null;

            foreach (InspectionSubjectPersonnelDto person in fishingShipInspection.Personnel)
            {
                switch (person.Type)
                {
                    case InspectedPersonType.OwnerPers:
                    case InspectedPersonType.OwnerLegal:
                        owner = person;
                        break;
                    case InspectedPersonType.LicUsrPers:
                    case InspectedPersonType.LicUsrLgl:
                        user = person;
                        break;
                    case InspectedPersonType.ReprsPers:
                        representative = person;
                        break;
                    case InspectedPersonType.CaptFshmn:
                        captain = person;
                        break;
                }
            }

            if (owner != null)
            {
                ShipOwner.OnEdit(owner);
            }
            if (captain != null)
            {
                ShipCaptain.OnEdit(user);
            }
            if (user != null)
            {
                if (owner != null)
                {
                    if (owner.EntryId == user.EntryId && (owner.EgnLnc?.EgnLnc == user.EgnLnc?.EgnLnc || owner.Eik == user.Eik))
                    {
                        UserIsOwner.Value = true;
                        IsOwnerUserCheckVisible = true;
                    }
                }
                ShipUser.OnEdit(user);
            }
            if (representative != null)
            {
                if (owner != null && owner.Type == InspectedPersonType.OwnerPers)
                {
                    if (owner.EntryId == representative.EntryId && owner.EgnLnc?.EgnLnc == representative.EgnLnc?.EgnLnc && owner.EgnLnc?.IdentifierType == representative.EgnLnc?.IdentifierType)
                    {
                        RepresentativeIsOwner.Value = true;
                        IsOwnerRepresentativeCheckVisible = true;
                    }
                }
                ShipRepresentative.OnEdit(representative);
            }

            LastHarbour?.OnEdit(port);
        }

        public void Reset()
        {
            IsOwnerRepresentativeCheckVisible = false;
            IsOwnerUserCheckVisible = false;
            RepresentativeIsOwner.Value = false;
            UserIsOwner.Value = false;
            ShipUser.SwitchActivityType(true);
            ShipRepresentative.SwitchActivityType(true);

            ShipCaptain.Reset(false);
            ShipCaptain.ResetForm();
            ShipOwner.Reset(false);
            ShipOwner.ResetForm();
            ShipUser.Reset(false);
            ShipUser.ResetForm();
            ShipRepresentative.Reset(false);
            ShipRepresentative.ResetForm();
        }

        private void OnOwnerChosen(InspectedPersonType? type)
        {
            if (type == null)
            {
                RepresentativeIsOwner.Value = false;
                UserIsOwner.Value = false;
                ShipUser.SwitchActivityType(true);
                ShipRepresentative.SwitchActivityType(true);
                ShipRepresentative.ResetForm();
                ShipUser.ResetForm();
                IsOwnerRepresentativeCheckVisible = false;
                IsOwnerUserCheckVisible = false;
            }
            else
            {
                IsOwnerRepresentativeCheckVisible = type == InspectedPersonType.OwnerPers;

                if (!IsOwnerRepresentativeCheckVisible)
                {
                    ShipRepresentative.SwitchActivityType(true);
                    RepresentativeIsOwner.Value = false;
                    ShipRepresentative.ResetForm();
                }

                IsOwnerUserCheckVisible = true;
            }
        }

        private void OnUserIsOwnerSwitched(bool switchValue)
        {
            if (switchValue)
            {
                ShipUser.OnEdit(ShipOwner);
            }
            else
            {
                ShipUser.ResetForm();
            }
            ShipUser.SwitchActivityType(!switchValue);
        }

        private void OnIsOwnerRepresentativeSwitched(bool switchValue)
        {
            if (switchValue)
            {
                ShipRepresentative.OnEdit(ShipOwner);
            }
            else
            {
                ShipRepresentative.ResetForm();
            }
            ShipRepresentative.SwitchActivityType(!switchValue);
        }

        public static implicit operator List<InspectionSubjectPersonnelDto>(FishingShipViewModel viewModel)
        {
            InspectionSubjectPersonnelDto owner = viewModel.ShipOwner;
            InspectionSubjectPersonnelDto user = viewModel.ShipUser;
            InspectionSubjectPersonnelDto representative = viewModel.ShipRepresentative;
            if (viewModel.UserIsOwner)
            {
                if (owner.IsLegal)
                {
                    user = InspectionSubjectPersonnelDto.CreateCopy(owner, InspectedPersonType.LicUsrLgl, true);
                }
                else
                {
                    user = InspectionSubjectPersonnelDto.CreateCopy(owner, InspectedPersonType.LicUsrPers, true);
                }
            }
            if (viewModel.RepresentativeIsOwner)
            {
                representative = InspectionSubjectPersonnelDto.CreateCopy(owner, InspectedPersonType.ReprsPers, false);
            }
            return new InspectionSubjectPersonnelDto[]
            {
                owner,
                user,
                representative,
                viewModel.ShipCaptain
            }.Where(f => f != null).ToList();
        }
    }
}
