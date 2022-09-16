using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.DTObjects.Profile.API;
using IARA.Mobile.Application.Interfaces.Transactions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.ViewModels
{
    public class DetailedAddressViewModel : TLBaseViewModel
    {
        private List<SelectNomenclatureDto> _countries;
        private List<SelectNomenclatureDto> _districts;
        private List<SelectNomenclatureDto> _municipalities;
        private List<SelectNomenclatureDto> _populatedAreas;
        private bool _districtSelected;
        private bool _municipalitySelected;
        private bool _isBulgariaSelected;

        public DetailedAddressViewModel()
        {
            DistrictChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnDistrictChosen);
            MunicipalityChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnMunicipalityChosen);
            CountryChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnCountryChosen);

            this.AddValidation();
        }
        protected IAddressTransaction AddressTransaction =>
        DependencyService.Resolve<IAddressTransaction>();
        public bool DistrictSelected
        {
            get => _districtSelected;
            set => SetProperty(ref _districtSelected, value);
        }

        public bool MunicipalitySelected
        {
            get => _municipalitySelected;
            set => SetProperty(ref _municipalitySelected, value);
        }

        public bool IsBulgariaSelected
        {
            get => _isBulgariaSelected;
            set => SetProperty(ref _isBulgariaSelected, value);
        }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Country { get; set; }
        [Required]
        public ValidStateSelect<SelectNomenclatureDto> District { get; set; }
        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Municipality { get; set; }
        [Required]
        public ValidStateSelect<SelectNomenclatureDto> PopulatedArea { get; set; }

        [StringLength(200)]
        public ValidState Region { get; set; }

        [Required]
        [StringLength(200)]
        public ValidState Street { get; set; }

        [StringLength(10)]
        public ValidState ZipCode { get; set; }

        [StringLength(10)]
        public ValidState Number { get; set; }

        [StringLength(10)]
        public ValidState Block { get; set; }

        [StringLength(10)]
        public ValidState Entrance { get; set; }

        [StringLength(10)]
        public ValidState Floor { get; set; }

        [StringLength(10)]
        public ValidState Apartment { get; set; }

        public List<SelectNomenclatureDto> Countries
        {
            get => _countries;
            set => SetProperty(ref _countries, value);
        }

        public List<SelectNomenclatureDto> Districts
        {
            get => _districts;
            set => SetProperty(ref _districts, value);
        }

        public List<SelectNomenclatureDto> Municipalities
        {
            get => _municipalities;
            set => SetProperty(ref _municipalities, value);
        }

        public List<SelectNomenclatureDto> PopulatedAreas
        {
            get => _populatedAreas;
            set => SetProperty(ref _populatedAreas, value);
        }

        public ICommand CountryChosen { get; }
        public ICommand DistrictChosen { get; }
        public ICommand MunicipalityChosen { get; }

        public void AssignAddressRegistration(AddressRegistrationApiDto dto)
        {
            Country.Value = Countries.Find(f => f.Id == dto.CountryId);
            IsBulgariaSelected = Country?.Value?.Id == Countries.First(c => c.Code == CommonConstants.NomenclatureBulgaria).Id;
            if (dto.DistrictId.HasValue)
            {
                District.Value = Districts.Find(f => f.Id == dto.DistrictId.Value);
                DistrictSelected = true;

                if (dto.MunicipalityId.HasValue)
                {
                    List<SelectNomenclatureDto> municipalities = AddressTransaction.GetMuncipalities(dto.DistrictId.Value);
                    Municipality.Value = municipalities.Find(f => f.Id == dto.MunicipalityId.Value);
                    MunicipalitySelected = true;
                    Municipalities = municipalities;

                    if (dto.PopulatedAreaId.HasValue)
                    {
                        PopulatedAreas = AddressTransaction.GetPopulatedAreas(dto.MunicipalityId.Value);
                        PopulatedArea.Value = PopulatedAreas.Find(f => f.Id == dto.PopulatedAreaId.Value);
                    }
                }
            }

            Region.Value = dto.Region;
            Street.Value = dto.Street;
            ZipCode.Value = dto.PostalCode;
            Number.Value = dto.StreetNum;
            Block.Value = dto.BlockNum;
            Entrance.Value = dto.EntranceNum;
            Floor.Value = dto.FloorNum;
            Apartment.Value = dto.ApartmentNum;
        }

        private void OnCountryChosen(SelectNomenclatureDto country)
        {
            IsBulgariaSelected = country.Id == Countries.First(c => c.Code == CommonConstants.NomenclatureBulgaria).Id;
            DistrictSelected = false;
            District.Value = null;

            MunicipalitySelected = false;
            Municipality.Value = null;

            PopulatedArea.Value = null;
        }

        private void OnDistrictChosen(SelectNomenclatureDto district)
        {
            Municipalities = AddressTransaction.GetMuncipalities(district.Id);
            DistrictSelected = true;
            MunicipalitySelected = false;
            Municipality.Value = null;
            PopulatedArea.Value = null;
        }

        private void OnMunicipalityChosen(SelectNomenclatureDto municipality)
        {
            PopulatedAreas = AddressTransaction.GetPopulatedAreas(municipality.Id);
            MunicipalitySelected = true;

            PopulatedArea.Value = null;
        }
    }
}
