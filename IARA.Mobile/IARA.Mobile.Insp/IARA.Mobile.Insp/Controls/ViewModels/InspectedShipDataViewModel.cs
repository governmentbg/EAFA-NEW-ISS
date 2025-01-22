using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.FlyoutPages.Inspections.HarbourInspection;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class InspectedShipDataViewModel : ViewModel
    {
        private IAsyncCommand<ShipSelectNomenclatureDto> _shipSelected;
        private ICommand _inRegisterChecked;
        private List<SelectNomenclatureDto> _flags;
        private List<SelectNomenclatureDto> _shipTypes;

        private List<InspectedPersonViewModel> _inspectedPeople;

        public InspectedShipDataViewModel(InspectionPageViewModel inspection, List<InspectedPersonViewModel> inspectedPeople = null, bool canPickLocation = true)
        {
            Inspection = inspection;
            CanPickLocation = canPickLocation;
            InRegisterChecked = CommandBuilder.CreateFrom(OnInRegisterChecked);
            _inspectedPeople = inspectedPeople;
            this.AddValidation(
                groups: new Dictionary<string, Func<bool>>
                {
                    { Group.REGISTERED, () => ShipInRegister },
                    { Group.NOT_REGISTERED, () => !ShipInRegister },
                    { Group.CAN_PICK_LOCATION, () => canPickLocation }
                }
            );

            ShipInRegister.Value = true;

            Ship.ItemsSource = new TLObservableCollection<ShipSelectNomenclatureDto>();
            Ship.GetMore = (int page, int pageSize, string search) => NomenclaturesTransaction.GetShips(page, pageSize, search);

            Location.AddFakeValidation();

            if (canPickLocation)
            {
                Location.HasAsterisk = true;
            }
            else
            {
                Location.Validations.RemoveAt(Location.Validations.FindIndex(f => f.Name == nameof(RequiredAttribute)));
                Location.HasAsterisk = false;
            }
            RequiredValidStates = ReflectionHelper.GetAllRequiredValidStates(GetType());
        }

        public List<PropertyInfo> RequiredValidStates { get; set; }
        public InspectionPageViewModel Inspection { get; }

        public ShipDto InspectedShip { get; set; }

        public bool CanPickLocation { get; }
        public bool ValidStatesHaveRequiredAttribute { get; set; } = true;

        public ValidStateBool ShipInRegister { get; set; }

        [Required]
        [ValidGroup(Group.CAN_PICK_LOCATION)]
        public ValidStateLocation Location { get; set; }

        [Required]
        [ValidGroup(Group.REGISTERED)]
        public ValidStateInfiniteSelect<ShipSelectNomenclatureDto> Ship { get; set; }

        [Required]
        [MaxLength(500)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState Name { get; set; }

        [Required]
        [MaxLength(50)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState ExternalMarkings { get; set; }

        [Required]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidStateSelect<SelectNomenclatureDto> Flag { get; set; }

        [MaxLength(20)]
        public ValidState CFR { get; set; }

        [MaxLength(20)]
        public ValidState UVI { get; set; }

        [MaxLength(50)]
        public ValidState CallSign { get; set; }

        [Required]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidStateSelect<SelectNomenclatureDto> ShipType { get; set; }

        [MaxLength(20)]
        public ValidState MMSI { get; set; }

        public List<SelectNomenclatureDto> Flags
        {
            get => _flags;
            private set => SetProperty(ref _flags, value);
        }
        public List<SelectNomenclatureDto> ShipTypes
        {
            get => _shipTypes;
            private set => SetProperty(ref _shipTypes, value);
        }

        public IAsyncCommand<ShipSelectNomenclatureDto> ShipSelected
        {
            get => _shipSelected;
            set => SetProperty(ref _shipSelected, value);
        }
        public ICommand InRegisterChecked
        {
            get => _inRegisterChecked;
            set => SetProperty(ref _inRegisterChecked, value);
        }

        private void OnInRegisterChecked()
        {
            Ship.Value = null;
            Name.Value = "";
            ExternalMarkings.Value = "";
            Flag.Value = null;
            CFR.Value = "";
            UVI.Value = "";
            CallSign.Value = "";
            ShipType.Value = null;
            MMSI.Value = "";

            if (_inspectedPeople != null)
            {
                foreach (var person in _inspectedPeople)
                {
                    person.Reset(!ShipInRegister);
                }
            }
        }

        public void Init(List<SelectNomenclatureDto> flags, List<SelectNomenclatureDto> shipTypes, List<CatchZoneNomenclatureDto> quadrants)
        {
            Flags = flags;
            ShipTypes = shipTypes;
            Ship.ItemsSource.AddRange(NomenclaturesTransaction.GetShips(0, CommonGlobalVariables.PullItemsCount));
        }

        public void OnSwitchTransshipping(bool hasTransshipping)
        {
            if (hasTransshipping)
            {
                if (!ValidStatesHaveRequiredAttribute)
                {
                    foreach (PropertyInfo property in RequiredValidStates)
                    {
                        IValidState validState = property.GetValue(this) as IValidState;
                        validState.Validations.Add(new TLValidator(new RequiredAttribute(), nameof(RequiredAttribute)));
                    }
                }
            }
            else
            {
                foreach (PropertyInfo property in RequiredValidStates)
                {
                    IValidState validState = property.GetValue(this) as IValidState;
                    TLValidator validator = validState.Validations.Where(x => x.Name == nameof(RequiredAttribute)).FirstOrDefault();
                    if (validator != null)
                    {
                        validState.Validations.Remove(validator);
                    }
                }
            }

            ValidStatesHaveRequiredAttribute = hasTransshipping;
        }

        public void OnEdit(VesselDuringInspectionDto dto)
        {
            if (dto == null)
            {
                return;
            }

            Location.AssignFrom(dto.Location);

            ShipInRegister.Value = dto.IsRegistered.GetValueOrDefault() && dto.ShipId.HasValue;

            if (ShipInRegister)
            {
                InspectedShip = new ShipDto
                {
                    AssociationId = dto.ShipAssociationId,
                    CallSign = dto.RegularCallsign,
                    CFR = dto.CFR,
                    ExtMarkings = dto.ExternalMark,
                    FlagId = dto.FlagCountryId,
                    MMSI = dto.MMSI,
                    Name = dto.Name,
                    ShipTypeId = dto.VesselTypeId,
                    UVI = dto.UVI,
                    Id = dto.ShipId.Value,
                    Uid = NomenclaturesTransaction.GetShip(dto.ShipId.Value).Uid,
                };

                Ship.Value = new ShipSelectNomenclatureDto
                {
                    Id = dto.ShipId.Value,
                    Name = dto.Name,
                    ExtMarkings = dto.ExternalMark,
                    Code = dto.CFR,
                    Uid = NomenclaturesTransaction.GetShip(dto.ShipId.Value).Uid,
                };
                //ShipSelected.Execute(Ship.Value);

                if (dto.ShipAssociationId.HasValue)
                {
                    SelectNomenclatureDto association = NomenclaturesTransaction.GetAssociation(dto.ShipAssociationId.Value);

                    if (association != null)
                    {
                        Ship.Value.AssociationName = association.Name;
                    }
                }
            }

            CallSign.Value = dto.RegularCallsign ?? string.Empty;
            MMSI.Value = dto.MMSI ?? string.Empty;
            CFR.Value = dto.CFR ?? string.Empty;
            ExternalMarkings.Value = dto.ExternalMark ?? string.Empty;
            Name.Value = dto.Name ?? string.Empty;
            UVI.Value = dto.UVI ?? string.Empty;

            if (dto.FlagCountryId.HasValue)
            {
                Flag.Value = Flags.Find(f => f.Id == dto.FlagCountryId.Value);
            }
            if (dto.VesselTypeId.HasValue)
            {
                ShipType.Value = ShipTypes.Find(f => f.Id == dto.VesselTypeId.Value);
            }
        }

        public string GetShortName()
        {
            if (!ShipInRegister)
            {
                return $"{Name.Value} ({CFR.Value})";
            }
            else if (Ship != null)
            {
                return Ship.Value.DisplayValue;
            }

            return null;
        }

        public static implicit operator VesselDuringInspectionDto(InspectedShipDataViewModel viewModel)
        {
            if (!viewModel.ShipInRegister)
            {
                return new VesselDuringInspectionDto
                {
                    IsRegistered = false,
                    Name = viewModel.Name,
                    CFR = viewModel.CFR,
                    ExternalMark = viewModel.ExternalMarkings,
                    FlagCountryId = viewModel.Flag.Value,
                    MMSI = viewModel.MMSI,
                    RegularCallsign = viewModel.CallSign,
                    UVI = viewModel.UVI,
                    VesselTypeId = viewModel.ShipType.Value,
                    Location = viewModel.Location,
                    //CatchZoneId = viewModel.Location.Quadrant.Value,
                    //LocationDescription = viewModel.Location.Description,
                };
            }
            else if (viewModel.InspectedShip != null)
            {
                ShipDto ship = viewModel.InspectedShip;

                return new VesselDuringInspectionDto
                {
                    CFR = ship.CFR,
                    ExternalMark = ship.ExtMarkings,
                    FlagCountryId = ship.FlagId,
                    ShipId = ship.Id,
                    IsRegistered = true,
                    MMSI = ship.MMSI,
                    Name = ship.Name,
                    VesselTypeId = ship.ShipTypeId,
                    RegularCallsign = ship.CallSign,
                    ShipAssociationId = ship.AssociationId,
                    UVI = ship.UVI,
                    Location = viewModel.Location,
                    //CatchZoneId = viewModel.Location.Quadrant.Value,
                    //LocationDescription = viewModel.Location.Description,
                };
            }

            return null;
        }
    }
}
