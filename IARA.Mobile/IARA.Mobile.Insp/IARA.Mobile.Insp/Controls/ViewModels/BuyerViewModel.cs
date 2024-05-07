using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.ViewModels.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class BuyerViewModel : ViewModel
    {
        private InspectionSubjectPersonnelDto _buyer;

        public BuyerViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            BuyerChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnBuyerChosen);

            this.AddValidation();

            Buyer.ItemsSource = new TLObservableCollection<SelectNomenclatureDto>();
            Buyer.GetMore = (int page, int pageSize, string search) =>
                NomenclaturesTransaction.GetBuyers(page, pageSize, search);
        }

        public InspectionPageViewModel Inspection { get; }

        [Required]
        public ValidStateInfiniteSelect<SelectNomenclatureDto> Buyer { get; set; }

        public ICommand BuyerChosen { get; }

        public void Init()
        {
            Buyer.ItemsSource.AddRange(NomenclaturesTransaction.GetBuyers(0, CommonGlobalVariables.PullItemsCount));
        }

        public void OnEdit(List<InspectionSubjectPersonnelDto> personnel)
        {
            InspectionSubjectPersonnelDto subject = personnel?.Find(f => f.Type == InspectedPersonType.RegBuyer);

            OnEdit(subject);
        }

        public void OnEdit(InspectionSubjectPersonnelDto buyer, bool assignBuyer = true)
        {
            if (buyer != null)
            {
                if (assignBuyer && buyer.Id.HasValue)
                {
                    Buyer.Value = new SelectNomenclatureDto
                    {
                        Id = buyer.Id.Value,
                        Code = buyer.EgnLnc?.EgnLnc ?? buyer.Eik,
                        Name = buyer.FirstName
                            + (buyer.MiddleName != null ? " " + buyer.MiddleName : "")
                            + (buyer.LastName != null ? " " + buyer.LastName : "")
                    };
                }

                _buyer = buyer;
                _buyer.Address = buyer.Address ?? buyer.RegisteredAddress.BuildAddress();
            }
        }

        private void OnBuyerChosen(SelectNomenclatureDto buyer)
        {
            OnEdit(NomenclaturesTransaction.GetBuyer(buyer.Id), false);
        }

        public static implicit operator InspectionSubjectPersonnelDto(BuyerViewModel viewModel)
        {
            return viewModel._buyer;
        }
    }
}
