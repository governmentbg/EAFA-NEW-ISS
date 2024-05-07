using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class InspectedBuyerViewModel : ViewModel
    {
        private ICommand _buyerChosen;
        private List<SelectNomenclatureDto> _nationalities;
        private List<SelectNomenclatureDto> _buyers;

        public InspectedBuyerViewModel(InspectionPageViewModel inspection, InspectedPersonType buyerLegalType = InspectedPersonType.RegBuyer)
        {
            Inspection = inspection;
            BuyerLegalType = buyerLegalType;

            this.AddValidation(groups: new Dictionary<string, Func<bool>>
            {
                { Group.REGISTERED, () => InRegister },
                { Group.NOT_REGISTERED, () => !InRegister }
            });

            InRegister.Value = true;

            Buyer.ItemsSource = new TLObservableCollection<SelectNomenclatureDto>();
            Buyer.GetMore = (int page, int pageSize, string search) =>
                NomenclaturesTransaction.GetBuyers(page, pageSize, search);
        }

        public InspectionPageViewModel Inspection { get; }
        public InspectedPersonType BuyerLegalType { get; }

        public InspectionSubjectPersonnelDto SelectedBuyer { get; set; }

        public ValidStateBool InRegister { get; set; }

        [Required]
        [ValidGroup(Group.REGISTERED)]
        public ValidStateInfiniteSelect<SelectNomenclatureDto> Buyer { get; set; }

        [MaxLength(200)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState Name { get; set; }

        [MaxLength(20)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState EIK { get; set; }

        [MaxLength(4000)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState Address { get; set; }

        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidStateSelect<SelectNomenclatureDto> Nationality { get; set; }

        public List<SelectNomenclatureDto> Nationalities
        {
            get => _nationalities;
            private set => SetProperty(ref _nationalities, value);
        }
        public List<SelectNomenclatureDto> Buyers
        {
            get => _buyers;
            set => SetProperty(ref _buyers, value);
        }

        public ICommand BuyerChosen
        {
            get => _buyerChosen;
            set => SetProperty(ref _buyerChosen, value);
        }

        public void Init(List<SelectNomenclatureDto> nationalities)
        {
            Nationalities = nationalities;

            Nationality.Value = nationalities.Find(f => f.Code == CommonConstants.NomenclatureBulgaria);

            Buyer.ItemsSource.AddRange(NomenclaturesTransaction.GetBuyers(0, CommonGlobalVariables.PullItemsCount));
        }

        public void OnEdit(List<InspectionSubjectPersonnelDto> personnel)
        {
            InspectionSubjectPersonnelDto subject = personnel?.Find(f => f.Type == BuyerLegalType);

            OnEdit(subject);
        }

        public void OnEdit(InspectionSubjectPersonnelDto buyer, bool assignBuyer = true)
        {
            SelectedBuyer = buyer;

            if (buyer != null)
            {
                string name = buyer.FirstName
                    + (buyer.MiddleName != null ? " " + buyer.MiddleName : "")
                    + (buyer.LastName != null ? " " + buyer.LastName : "");

                if (assignBuyer)
                {
                    if (buyer.Id.HasValue)
                    {
                        Buyer.Value = new SelectNomenclatureDto
                        {
                            Id = buyer.Id.Value,
                            Code = buyer.Eik,
                            Name = name
                        };

                        InRegister.Value = true;
                    }
                    else
                    {
                        InRegister.Value = false;
                    }
                }

                Name.Value = name;
                EIK.AssignFrom(buyer.Eik);
                Address.Value = buyer.Address ?? buyer.RegisteredAddress.BuildAddress();
                Nationality.AssignFrom(buyer.CitizenshipId, Nationalities);
            }
        }

        public static implicit operator InspectionSubjectPersonnelDto(InspectedBuyerViewModel viewModel)
        {
            if (!viewModel.InRegister)
            {
                return new InspectionSubjectPersonnelDto
                {
                    Type = viewModel.BuyerLegalType,
                    Address = viewModel.Address,
                    CitizenshipId = viewModel.Nationality.Value,
                    FirstName = viewModel.Name,
                    Eik = viewModel.EIK,
                    IsRegistered = false,
                };
            }
            else if (viewModel.SelectedBuyer != null)
            {
                InspectionSubjectPersonnelDto user = viewModel.SelectedBuyer;
                user.IsRegistered = true;
                user.Type = viewModel.BuyerLegalType;

                if (user.Address == null && user.RegisteredAddress != null)
                {
                    user.Address = user.RegisteredAddress.BuildAddress();
                }

                return user;
            }

            return null;
        }
    }
}
