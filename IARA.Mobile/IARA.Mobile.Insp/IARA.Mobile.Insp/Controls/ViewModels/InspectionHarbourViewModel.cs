using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.ViewModels.Models;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class InspectionHarbourViewModel : ViewModel
    {
        private List<SelectNomenclatureDto> _countries;

        public InspectionHarbourViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            this.AddValidation(groups: new Dictionary<string, Func<bool>>
            {
                { Group.REGISTERED, () => HarbourInRegister.Value },
                { Group.NOT_REGISTERED, () => !HarbourInRegister.Value },
            });

            HarbourInRegister.Value = true;

            Harbour.ItemsSource = new TLObservableCollection<SelectNomenclatureDto>();
            Harbour.GetMore = (int page, int pageSize, string search) => NomenclaturesTransaction.GetPorts(page, pageSize, search);
        }

        public InspectionPageViewModel Inspection { get; }

        public ValidStateBool HarbourInRegister { get; set; }

        [Required]
        [ValidGroup(Group.REGISTERED)]
        public ValidStateInfiniteSelect<SelectNomenclatureDto> Harbour { get; set; }

        [Required]
        [MaxLength(500)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState Name { get; set; }

        [Required]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidStateSelect<SelectNomenclatureDto> Country { get; set; }

        public List<SelectNomenclatureDto> Countries
        {
            get => _countries;
            private set => SetProperty(ref _countries, value);
        }

        public void Init(List<SelectNomenclatureDto> countries)
        {
            Countries = countries;
            Harbour.ItemsSource.AddRange(NomenclaturesTransaction.GetPorts(0, CommonGlobalVariables.PullItemsCount));
        }

        public void OnEdit(PortVisitDto portVisit)
        {
            if (portVisit == null)
            {
                return;
            }

            if (portVisit.PortId.HasValue)
            {
                Harbour.Value = new SelectNomenclatureDto
                {
                    Id = portVisit.PortId.Value,
                    Name = portVisit.PortName,
                };

                HarbourInRegister.Value = true;
            }
            else
            {
                Country.AssignFrom(portVisit.PortCountryId, Countries);
                Name.AssignFrom(portVisit.PortName);
                HarbourInRegister.Value = false;
            }
        }

        public static implicit operator PortVisitDto(InspectionHarbourViewModel viewModel)
        {
            if (viewModel == null)
            {
                return null;
            }

            if (!viewModel.HarbourInRegister)
            {
                return new PortVisitDto
                {
                    PortCountryId = viewModel.Country.Value,
                    PortName = viewModel.Name,
                    VisitDate = DateTime.Now,
                };
            }
            else if (viewModel.Harbour.Value != null)
            {
                SelectNomenclatureDto port = viewModel.Harbour.Value;

                return new PortVisitDto
                {
                    PortId = port.Id,
                    PortName = port.Name,
                    VisitDate = DateTime.Now,
                };
            }

            return null;
        }
    }
}
