using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Profile.API;
using IARA.Mobile.Pub.Application.DTObjects.AddressNomenclatures.LocalDb;
using IARA.Mobile.Pub.ViewModels.Base;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Pub.ViewModels.Controls.PersonView
{
    public class AddressViewModel : ViewModel
    {
        private ValidStateSelect<CountrySelectDto> _country;
        private ValidStateSelect<DistrictSelectDto> _district;
        private ValidStateSelect<MunicipalitySelectDto> _municipality;
        private ValidStateSelect<PopulatedAreaSelectDto> _populatedArea;
        private ValidState _address;
        private List<MunicipalitySelectDto> _municipalities;
        private List<PopulatedAreaSelectDto> _populatedAreas;
        private bool _isDistrictSelected;
        private bool _isMunicipalitySelected;
        private int _countryIdBulgaria;
        public AddressViewModel()
        {
            OnDistrictChangedCommand = CommandBuilder.CreateFrom<DistrictSelectDto>(OnDistrictChanged);
            OnMunicipalityChangedCommand = CommandBuilder.CreateFrom<MunicipalitySelectDto>(OnMunicipalityChanged);
        }

        [Required]
        [ValidGroup(Group.ADDRESS)]
        public ValidStateSelect<CountrySelectDto> Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }

        [Required]
        [ValidGroup(Group.BULGARIAN_ADDRESS)]
        public ValidStateSelect<DistrictSelectDto> District
        {
            get => _district;
            set => SetProperty(ref _district, value);
        }

        [Required]
        [ValidGroup(Group.BULGARIAN_ADDRESS)]
        public ValidStateSelect<MunicipalitySelectDto> Municipality
        {
            get => _municipality;
            set => SetProperty(ref _municipality, value);
        }

        [Required]
        [ValidGroup(Group.BULGARIAN_ADDRESS)]
        public ValidStateSelect<PopulatedAreaSelectDto> PopulatedArea
        {
            get => _populatedArea;
            set => SetProperty(ref _populatedArea, value);
        }

        [Required]
        [StringLength(200)]
        [ValidGroup(Group.ADDRESS)]
        public ValidState Street
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public ValidState Region { get; set; }

        public ValidState ZipCode { get; set; }

        public ValidState Number { get; set; }

        public ValidState Block { get; set; }

        public ValidState Entrance { get; set; }

        public ValidState Floor { get; set; }

        public ValidState Apartment { get; set; }

        public List<MunicipalitySelectDto> Municipalities
        {
            get => _municipalities;
            set => SetProperty(ref _municipalities, value);
        }

        public List<PopulatedAreaSelectDto> PopulatedAreas
        {
            get => _populatedAreas;
            set => SetProperty(ref _populatedAreas, value);
        }

        public bool IsDistrictSelected
        {
            get => _isDistrictSelected;
            set => SetProperty(ref _isDistrictSelected, value);
        }

        public bool IsMunicipalitySelected
        {
            get => _isMunicipalitySelected;
            set => SetProperty(ref _isMunicipalitySelected, value);
        }

        public ICommand OnDistrictChangedCommand { get; }
        public ICommand OnMunicipalityChangedCommand { get; }

        private void OnDistrictChanged(DistrictSelectDto district)
        {
            Municipalities = NomenclaturesTransaction.GetMuncipalitiesByDisctrict(district.Id);
            IsDistrictSelected = true;
            Municipality.Value = null;
            PopulatedArea.Value = null;
            IsMunicipalitySelected = false;
        }
        private void OnMunicipalityChanged(MunicipalitySelectDto municipality)
        {
            PopulatedAreas = NomenclaturesTransaction.GetPopulatedAreasByMunicipality(municipality.Id);
            IsMunicipalitySelected = true;
            PopulatedArea.Value = null;
        }

        public void AssignAddressRegistration(AddressRegistrationApiDto dto, List<CountrySelectDto> countries, List<DistrictSelectDto> disticts, bool bulgarianAddressRequired = false)
        {
            _countryIdBulgaria = countries.First(c => c.Code == CommonConstants.NomenclatureBulgaria).Id;

            if (bulgarianAddressRequired)//The user should select bulgarian address if HasBulgarianAddress = true, do not force select Bulgaria in "View" ticket mode.
            {
                Country.Value = countries.Find(f => f.Id == _countryIdBulgaria);//Select Bulgaria as country
            }
            else
            {
                Country.Value = countries.Find(f => f.Id == dto.CountryId); //Select the country form the person's profile
            }

            if (dto.DistrictId.HasValue)
            {
                District.Value = disticts.Find(f => f.Id == dto.DistrictId.Value);
                IsDistrictSelected = true;

                if (dto.MunicipalityId.HasValue)
                {
                    Municipalities = NomenclaturesTransaction.GetMuncipalitiesByDisctrict(dto.DistrictId.Value);
                    Municipality.Value = Municipalities.Find(f => f.Id == dto.MunicipalityId.Value);
                    IsMunicipalitySelected = true;

                    if (dto.PopulatedAreaId.HasValue)
                    {
                        PopulatedAreas = NomenclaturesTransaction.GetPopulatedAreasByMunicipality(dto.MunicipalityId.Value);
                        PopulatedArea.Value = PopulatedAreas.Find(f => f.Id == dto.PopulatedAreaId.Value);
                    }
                }
            }
            Street.Value = dto.Street;
            Region.Value = dto.Region;
            ZipCode.Value = dto.PostalCode;
            Number.Value = dto.StreetNum;
            Block.Value = dto.BlockNum;
            Entrance.Value = dto.EntranceNum;
            Floor.Value = dto.FloorNum;
            Apartment.Value = dto.ApartmentNum;
        }
    }
}
