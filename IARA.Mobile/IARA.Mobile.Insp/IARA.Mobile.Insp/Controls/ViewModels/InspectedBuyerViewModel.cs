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
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class InspectedBuyerViewModel : ViewModel
    {
        private ICommand _buyerChosen;
        private List<SelectNomenclatureDto> _buyers;

        public InspectedBuyerViewModel(InspectionPageViewModel inspection, InspectedPersonType buyerLegalType = InspectedPersonType.RegBuyer)
        {
            Inspection = inspection;
            BuyerLegalType = buyerLegalType;
            Owner = new SubjectViewModel(inspection, buyerLegalType, buyerLegalType);

            this.AddValidation(
                others: new IValidatableViewModel[]
                {
                    Owner
                },
                groups: new Dictionary<string, Func<bool>>
                {
                    { Group.REGISTERED, () => InRegister },
                    { Group.NOT_REGISTERED, () => !InRegister }
                });

            InRegister.Value = true;

            Buyer.ItemsSource = new TLObservableCollection<SelectNomenclatureDto>();
            Buyer.GetMore = (int page, int pageSize, string search) =>
                NomenclaturesTransaction.GetBuyers(page, pageSize, search);

            Owner.Validation.GlobalGroups = new List<string> { Group.NOT_REGISTERED };
        }

        public InspectionPageViewModel Inspection { get; }
        public SubjectViewModel Owner { get; }
        public InspectedPersonType BuyerLegalType { get; }

        public InspectionSubjectPersonnelDto SelectedBuyer { get; set; }

        public ValidStateBool InRegister { get; set; }

        [Required]
        [ValidGroup(Group.REGISTERED)]
        public ValidStateInfiniteSelect<SelectNomenclatureDto> Buyer { get; set; }

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
            Owner.Init(nationalities);
            Buyer.ItemsSource.AddRange(NomenclaturesTransaction.GetBuyers(0, CommonGlobalVariables.PullItemsCount));
        }

        public void OnEdit(List<InspectionSubjectPersonnelDto> personnel)
        {
            OnEditBuyer(personnel?.Find(f => f.Type == BuyerLegalType || f.Type == InspectedPersonType.OwnerPers || f.Type == InspectedPersonType.OwnerLegal));
        }

        public void OnEditBuyer(InspectionSubjectPersonnelDto buyer, bool assignBuyer = true)
        {
            if (buyer.EntryId.HasValue)
            {
                SelectedBuyer = buyer;
                string name = buyer.FirstName
                    + (buyer.MiddleName != null ? " " + buyer.MiddleName : "")
                    + (buyer.LastName != null ? " " + buyer.LastName : "");

                if (assignBuyer)
                {
                    InRegister.Value = true;
                    Buyer.Value = new SelectNomenclatureDto
                    {
                        Id = buyer.EntryId.Value,
                        Code = buyer.Eik,
                        Name = name
                    };
                }
            }
            else
            {
                if (assignBuyer)
                {
                    InRegister.Value = false;
                    Owner.OnEdit(buyer);
                }
            }
        }

        public static implicit operator InspectionSubjectPersonnelDto(InspectedBuyerViewModel viewModel)
        {
            if (!viewModel.InRegister)
            {
                return viewModel.Owner;
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
